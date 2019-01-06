using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Chat_maybe
{
    class Server : Interface
    {
        List<TcpClient> tcpClients = new List<TcpClient>(2);
        List<NetworkStream> networkStreams = new List<NetworkStream>(2);
        TcpListener Listener;

        public Server(int port)
        {
            var ip = System.Net.IPAddress.Parse("192.168.0.103");
            Listener = new TcpListener(ip,80);
            Connect();
        }

        async public void Connect()
        {
            Listener.Start();
            tcpClients.Add(await Listener.AcceptTcpClientAsync());
            networkStreams.Add(tcpClients[tcpClients.Count-1].GetStream());
        }

        public void Disconect()
        {
            throw new NotImplementedException();
        }

        async public Task<string> Read_ms()
        {
            byte[] mass = new byte[4096];
            for(int i = 0; i < mass.Length; i++)
            {
                //Мне это не nice
                await networkStreams[i].ReadAsync(mass, 0, mass.Length);
            }
            Send(Encoding.ASCII.GetString(mass));
            return null;
        }

        public void Send(string message)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            foreach (var buf in networkStreams)
            {
                buf.Write(buff, 0, buff.Length);
            }
        }
    }
}
