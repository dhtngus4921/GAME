using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);

            //clientSocket.Receive(recvBuff), 버퍼 연결하는 과정
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            RegisterRecv(recvArgs);
        }

        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (pending == false)
                OnRecvCompleted(null, args);
        }

        public void Disconnect()
        {
            //disconnect가 두번 사용되는 것을 방지, 1번 사용했을 때 1로 바꿔줌
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        public void Send(byte[] sendBuff)
        {
            _socket.Send(sendBuff);
        }

        //내부에서만 사용하는 부분이므로 region으로 감싸줌
        #region 네트워크 통신
        //서버에서 클라이언트의 데이터를 받아오는 부분
        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            //성공적으로 통신을 끝낸 경우
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    //받아 온 데이터를 긁어 recvData에 저장해줌
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[from Client] {recvData}");

                    RegisterRecv(args);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                //DisConnect
            }
        }
        #endregion
    }
}
