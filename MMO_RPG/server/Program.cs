using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

/* sendBuff의 경우 내부(session)에서 복사해서 사용하는 것보다 외부에서 선언하고 끌어가서 사용하는 것이 더 효율적임
*  예)
*   byte[] sendBuff = new byte[1024];
*   for( ;;)
*     Session.Send();
*/
namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        //달라진점. 어떤 세션을 만들것인지만 정의해주고 나머지는 내부에서 처리, 프로그램 보안성 향상   
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            //gameSession을 만들기로 정의
            _listener.Init(endPoint, () => { return new ClientSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }
        }
    }
}
