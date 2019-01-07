using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
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
            try
            {
                Clienttcp = new TcpClient(ip, port);
                stream = Clienttcp.GetStream();
            }
            catch
            {
                
            }
        }

        public void Disconect(int id)
        {
            Send(new message_error("Соеденение закрыто"));
            Clienttcp.Close();
            stream.Close();
            //stream = null;
        }

        public void Send(object message)
        {
            if (stream == null)
                return;
            BinaryFormatter ser = new BinaryFormatter();
            ser.Serialize(stream, message);//Encoding.Unicode.GetBytes(message);
            

        }
        public object Read_ms()
        {
            //string s = null;
            //Thread thread = new Thread(delegate () { s = Read_stream(); });
            //thread.Start();
            try
            {
                if (Clienttcp.Available > 0)
                {
                    byte[] buffer = new byte[4096];
                    int count = stream.Read(buffer, 0, buffer.Length);
                    return Encoding.ASCII.GetString(buffer, 0, count);
                }
            }
            catch
            {
                return new message_error("Соеденение прервано");
            }
            return null;
        }
            
    }

}

