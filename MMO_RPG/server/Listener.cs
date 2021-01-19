using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

//ServerCore를 라이브러리로 빼서 DummyClient에서 사용
namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        //new로 생성하지 않고 _sessionFactory.Invoke()
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            _listenSocket.Bind(endPoint);

            _listenSocket.Listen(10);

            //pending이 true여도 이곳에서 onAcceptCompleted 함수를 실행해줌 -> 처음 시작은 인위적으로 만들어줌 
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            //다시 실행하기 전에 기존에 있던 잔재를 지워주는 단계
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            //실행 이후 바로 client 요청이 들어오면 pending이 없는 상태 -> 직접 지정하여 이동
            if (pending == false)
                OnAcceptCompleted(null, args);
            
        }

        //multiThread로 처리될 수 있기 때문에 항상 염두해 두어야 한다. 
        //ServerCore의 while문이 돌아갈때 실행된 경우가 있음
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }else
                Console.WriteLine(args.SocketError.ToString());

            //작업 완료 후 다음 소켓을 위해 등록
            RegisterAccept(args);
        }

        public Socket Accept()
        {
            //Async: 비동기, 동시에 처리되지 않고 나중에 처리 될 수 있음, nonblocking
            _listenSocket.AcceptAsync();

            //blocking 계열 함수 사용, 게임x
            return _listenSocket.Accept();
        }
    }
}
