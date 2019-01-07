using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;


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
            //Поток, чтобы чекать не отвалился ли кто
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
            //Поток, который чекает подключаемые соеденения 
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
            networkStreams.RemoveAt(id);
        }

        public object Read_ms()
        {
            BinaryFormatter ser = new BinaryFormatter();

            byte[] mass = new byte[4096];
            for (int i = 0; i < networkStreams.Count; i++)
            {
                object message = ser.Deserialize(networkStreams[i]);
                if (message.Equals(null))
                {
                    continue;
                }
            }
            Send(Encoding.ASCII.GetString(mass));
            return null;
        }

        async public void Send(object message)
        {
            byte[] buff = Encoding.ASCII.GetBytes((string)message);

            for (int i = 0; i < tcpClients.Count; i++)
            {
                try
                {
                    //networkStreams.Add(tcpClients[i].GetStream());
                    await networkStreams[i].WriteAsync(buff, 0, buff.Length);
                }
                catch
                {

                }
            }

        }
    }
}
