using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chat_maybe
{
    class Server : Interface
    {
        List<TcpClient> tcpClients = new List<TcpClient>(2);
        List<NetworkStream> networkStreams = new List<NetworkStream>(2);
        TcpListener Listener;

        static object locker = new object();

        public Server(int port)
        {
            var ip = System.Net.IPAddress.Parse("192.168.0.103");
            Listener = new TcpListener(ip, 80);

        }
        public void Connect()
        {
            Listener.Start();
            Thread thread = new Thread(new ThreadStart(Connect_Thread)), thread1 = new Thread(new ThreadStart(Try_ping));
            thread.Name = "get_connect";
            thread1.Name = "Ping";
            thread.Start();
            thread1.Start();
        }
        private void Try_ping()
        {

            while (true)
            {
                for (int i = 0; i < tcpClients.Count; i++)
                {
                    if (tcpClients.Count == 0)
                        break;
                    lock (locker)
                    {
                        if (tcpClients[i].Connected == false)
                        {
                            Disconect(i);
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }
        private void Connect_Thread()
        {
            while (true)
            {
                var buffer = Listener.AcceptTcpClient();
                lock (locker)
                {
                    tcpClients.Add(buffer);
                    networkStreams.Add(buffer.GetStream());
                }
            }

        }

        public void Disconect(int id)
        {
            tcpClients.RemoveAt(id);
        }

        public string Read_ms()
        {
            byte[] mass = new byte[4096];
            for (int i = 0; i < mass.Length; i++)
            {
                networkStreams[i].Read(mass, 0, mass.Length);
            }
            Send(Encoding.ASCII.GetString(mass));
            return null;
        }

        async public void Send(string message)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            try
            {
                for (int i = 0; i < tcpClients.Count; i++)
                {
                    networkStreams.Add(tcpClients[i].GetStream());
                    await networkStreams[i].WriteAsync(buff, 0, buff.Length);
                }
            }
            catch
            {

            }
        }
    }
}
