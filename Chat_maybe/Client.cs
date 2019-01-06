using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Chat_maybe
{
    class Client : Interface
    {
        NetworkStream stream;
        TcpClient Clienttcp;
        string ip;
        int port;

        //public Client() { }
        public Client (string ip,int port)
        {
            this.ip = ip;
            this.port = port;
            Connect();
        }
        public void Connect()
        {
            Clienttcp = new TcpClient(ip, port);
            stream = Clienttcp.GetStream();
        }

        public void Disconect()
        {
            Clienttcp.Close();
            stream = null;
        }

        public void Send(string message)
        {
            if (stream == null)
                return;
            byte[] ms = Encoding.Unicode.GetBytes(message);
            stream.Write(ms, 0, ms.Length);
            
        }
        async public Task<string> Read_ms()
        {
            if (stream == null)
                return null;
            byte[] buffer = new byte[4096];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string message = Encoding.ASCII.GetString(buffer);
            return message;
        }
    }
}
