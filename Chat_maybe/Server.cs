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
        Thread thread, thread1;

        List<TcpClient> tcpClients = new List<TcpClient>(2);
        List<NetworkStream> networkStreams = new List<NetworkStream>(2);
        TcpListener Listener;

        static object locker = new object();

        public Server(string ip,int port)
        {
            var ip_buff = System.Net.IPAddress.Parse("192.168.0.103");
            Listener = new TcpListener(ip_buff, port);

        }
        public void Connect()
        {
            Listener.Start();
            thread = new Thread(new ThreadStart(Connect_Thread));
            thread1 = new Thread(new ThreadStart(Try_ping));
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
            TcpClient buffer = null;
            //Поток, который чекает подключаемые соеденения 
            while (true)
            {
                //Не забыть решить проблему с этим try catch - сделать это красивее
                try
                {
                    buffer = Listener.AcceptTcpClient();
                }
                catch
                {
                    break;
                }
                lock (locker)
                {
                    tcpClients.Add(buffer);
                    networkStreams.Add(buffer.GetStream());
                }
                Send(tcpClients.Count - 1);
                Send("Пользователь "+ (tcpClients.Count - 1) + " подключился");
            }

        }

        public void Disconect(int id)
        {
            lock (locker)
            {
                tcpClients.RemoveAt(id);
                networkStreams.RemoveAt(id);
            }
        }
        public void Stop()
        {
            //thread.Abort();
            thread1.Abort();
            lock (locker)
            {
                for (int i = 0; i < tcpClients.Count; i++)
                {
                    Send(new message_error("Хост умер"));
                    Disconect(i);
                }
            }
            Listener.Stop();
        }
        public object Read_ms()
        {
            BinaryFormatter ser = new BinaryFormatter();
            //byte[] mass = new byte[4096];
            for (int i = 0; i < networkStreams.Count; i++)
            {
                try
                {
                    if (networkStreams[i].DataAvailable != true)
                        continue;
                    object message = ser.Deserialize(networkStreams[i]);
                    if (message.Equals(null))
                    {
                        continue;
                    }
                    else if (message.GetType() == typeof(message_error))
                    {
                        message_error buff = (message_error)message;
                        Disconect(buff.id);
                    }
                    else
                    {
                        return message;
                    }
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
            //Send(message);
            return null;
        }

        public void Send(object message)
        {
            //byte[] buff = Encoding.ASCII.GetBytes((string)message);

            for (int i = 0; i < tcpClients.Count; i++)
            {
                //networkStreams.Add(tcpClients[i].GetStream());
                //await networkStreams[i].WriteAsync(buff, 0, buff.Length);
                BinaryFormatter ser = new BinaryFormatter();
                ser.Serialize(networkStreams[i], message);
            }

        }
    }
}
