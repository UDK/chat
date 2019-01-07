using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_maybe
{
    interface Interface
    {
        void Connect();
        void Disconect(int id);
        void Send(object message);
        object Read_ms();
    }
    [Serializable]
    class message_error
    {
        public message_error(string message)
        {
            DateTime_error = DateTime.Now;
            flag_error = true;
            this.message = message;
        }
        public message_error(string message,int id)
        {
            DateTime_error = DateTime.Now;
            flag_error = true;
            this.message = message;
            this.id = id;
        }
        public readonly DateTime DateTime_error;
        public readonly bool flag_error;
        public readonly object message;
        public readonly int id;
    }
    
}
