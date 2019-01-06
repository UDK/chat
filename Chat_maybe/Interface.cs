using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_maybe
{
    interface Interface
    {
        void Connect();
        void Disconect();
        void Send(string message);
        Task<string> Read_ms();
    }
}
