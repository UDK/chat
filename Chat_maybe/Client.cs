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
        public int id;
        readonly string ip;
        readonly int port;

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
                Send(id+" Подключился");
            }
            catch
            {
                
            }
        }

        public void Disconect(int id)
        {
            Send(new message_error("Соеденение прервано",id));
            Clienttcp.Close();
            stream.Close();
            //stream = null;
        }
        public void Disconect()
        {
            Clienttcp.Close();
            stream.Close();
        }
        public void Send(object message)
        {
            string buf = id + ": ";
            message = buf + message;
            if (stream == null)
                return;
            BinaryFormatter ser = new BinaryFormatter();
            ser.Serialize(stream, message);
        }
        public object Read_ms()
        {
            try
            {
                if (Clienttcp.Available > 0)
                {
                    object buff = new BinaryFormatter().Deserialize(stream);
                    if (buff.GetType() == typeof(int))
                    {
                        id = (int)buff;
                        return null;
                    }
                    if(buff.GetType() == typeof(message_error))
                    {
                        Disconect();
                        return buff;
                    }
                    return buff;
                    //byte[] buffer = new byte[4096];
                    //int count = stream.Read(buffer, 0, buffer.Length);
                    //return Encoding.ASCII.GetString(buffer, 0, count);
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

