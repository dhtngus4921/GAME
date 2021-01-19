using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(1024);

        object _lock = new object();
        //_pending: registerSend시 true로 변환, 전송하고 있음을 알려주는 역할, onsendCompleted호출 시 _pending을 다시 false로(전송 완료)
        Queue<byte[]> _sendQueue = new Queue<byte[]>();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBuffer);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            //_sendArgs: 미리 생성 해놓고 필요할 때 registerSend()를 호출해서 그안에서 처리하는 방식
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        //원하는 시점에 호출하여 사용하는 것이기 때문에 까다로움
        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                //지금 바로 전송 가능한 상태, multithread에서 실행하는 경우 lock의 개념이 추가되어야 함.
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        void RegisterRecv()
        {
            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            //offset부터 count까지를 빈공간이라고 알려줌
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
            
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
                OnRecvCompleted(null, _recvArgs);
        }

        void RegisterSend()
        {
            //sendQueue.count, pending과 같은 역할
            while(_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();
                //큐의 내용을 받아 새로 받은 버퍼로 set
                //_sendArgs.BufferList.Add(new ArraySegment<byte>(buff, 0, buff.Length)); -> 오류 나는 방식, list 만들어서 처리
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            _sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
                OnSendCompleted(null, _sendArgs);
        }

        //내부에서만 사용하는 부분이므로 region으로 감싸줌
        #region 네트워크 통신
        //서버에서 클라이언트의 데이터를 받아오는 부분
        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            //성공적으로 통신을 끝낸 경우
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    //빈 공간을 확인하고 그 부분으로 커서 이동, bytetransffered로 이동
                    if(_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        //버그 확인 disconnect()
                        Disconnect();
                        return;
                    }

                    //컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다.
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if(processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    //처리 한 readPos 이동
                    if(_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
        #endregion

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);

                        //_pending이 true인 상태에서 큐에 들어온 데이터를 처리해야 함(enqueue하고 대기하고 있는상태)
                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnRecvCompleted Failed {e}");
                    }
                }
                else
                {
                    //전송 실패
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            //disconnect가 두번 사용되는 것을 방지, 1번 사용했을 때 1로 바꿔줌
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            //disconnect시 gamesession에서 처리하고자 하는 작업을 처리 하기 위함
            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
