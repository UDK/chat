using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net.Sockets;

namespace Chat_maybe
{
    class Client : Interface
    {
        NetworkStream stream;
        TcpClient Clienttcp;
        int id;
        string ip;
        int port;

        //public Client() { }
        public Client(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
        public void Connect()
        {
            Clienttcp = new TcpClient(ip, port);
            stream = Clienttcp.GetStream();
        }

        public void Disconect(int id)
        {
            Clienttcp.Close();
            //stream = null;
        }

        public void Send(string message)
        {
            if (stream == null)
                return;
            byte[] ms = Encoding.Unicode.GetBytes(message);
            stream.Write(ms, 0, ms.Length);

        }
        public string Read_ms()
        {
            //string s = null;
            //Thread thread = new Thread(delegate () { s = Read_stream(); });
            //thread.Start();
            if (Clienttcp.Available > 0)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int count = stream.Read(buffer, 0, buffer.Length);
                    return Encoding.ASCII.GetString(buffer, 0, count);
                }
                catch
                {
                    return "Соеденение прервано";
                }
            }
            return null;
        }

    }
}
