using System.Threading;
using System.Windows;
using System.Collections.Generic;
using System.Text;

namespace Chat_maybe
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Не очень мне это нравитсяс=, стоит это потом это решить
        Thread thread_serv = null;
        //////////
        Client client = null;
        Server server = null;
        public MainWindow()
        {
            InitializeComponent();
            Mask.Text = "1921680  103";
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder ss = new StringBuilder(Mask.Text);
            ss.Replace(",", "."); ss.Replace("_", "");
            client = new Client(ss.ToString(), 80);
            client.Connect();
            Thread thread = new Thread(new ThreadStart(set_msg));
            thread.Start();
            Connect.IsEnabled = false;
            Disconnect.IsEnabled = true;
            Send.IsEnabled = true;
            ServerPanel.IsEnabled = false;
            //server.Send("ping");
        }
        //Связь между listbox и потоками
        async private void set_msg_server()
        {
            while (true)
            {
                object s = server.Read_ms();
                if (s != null)
                {
                    if (s.GetType() == typeof(string))
                    {
                        await Dispatcher.InvokeAsync(() => ListBoxe_Server.Items.Add(s));
                        server.Send(s);
                        //ListBoxee.Items.Add(s);
                    }
                    else if (s.GetType() == typeof(message_error))
                    {
                        var buff = (message_error)s;
                        //await Dispatcher.InvokeAsync(() => ListBoxee.Items.Add("Отключился: id"+buff.message));
                        await Dispatcher.InvokeAsync(() => ListBoxe_Server.Items.Add(buff.message));
                        server.Disconect((int)buff.message);
                    }
                }
            }
        }
        //Связь между listbox и потоками
        async private void set_msg()
        {
            while (true)
            {
                object s = client.Read_ms();
                if (s != null)
                {
                    if (s.GetType() == typeof(string))
                    {
                        await Dispatcher.InvokeAsync(() => ListBoxee.Items.Add(s));
                    }
                    else if (s.GetType() == typeof(message_error))
                    {
                        var buff = (message_error)s;
                        await Dispatcher.InvokeAsync(() => ListBoxee.Items.Add(buff.message));
                        await Dispatcher.InvokeAsync(() => Connect.IsEnabled = true);
                        await Dispatcher.InvokeAsync(() => Disconnect.IsEnabled = false);
                        await Dispatcher.InvokeAsync(() => Send.IsEnabled = false);
                        await Dispatcher.InvokeAsync(() => ServerPanel.IsEnabled = true);
                        break;
                    }
                }
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if(client== null)
            {
                ListBoxee.Items.Add("Клиент и так не поключен");
                return;
            }
            client.Disconect(client.id);
            Connect.IsEnabled = true;
            Disconnect.IsEnabled = false;
            Send.IsEnabled = false;
            ServerPanel.IsEnabled = true;
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            server = new Server(8080);
            server.Connect();
            thread_serv = new Thread(new ThreadStart(set_msg_server));
            thread_serv.Name = "get_msg_server";
            thread_serv.Start();
            ListBoxe_Server.Items.Add("Хост старт");
            ClientPanel.IsEnabled = false;
        }

        private void Stop_Server_Click(object sender, RoutedEventArgs e)
        {
            //if()
            thread_serv.Abort();
            server.Stop();
            ListBoxe_Server.Items.Add("Хост стоп");
            ClientPanel.IsEnabled = true;
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            client.Send(textbox.Text);
            textbox.Text = "";
        }
    }
}
