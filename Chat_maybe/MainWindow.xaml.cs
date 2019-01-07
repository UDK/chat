using System.Threading;
using System.Windows;

namespace Chat_maybe
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client = null;
        Server server = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            client = new Client("192.168.0.103", 80);
            client.Connect();
            server.Send("ping");
            Thread thread = new Thread(new ThreadStart(set_msg));
            thread.Start();
            Connect.IsEnabled = false;
            Disconnect.IsEnabled = true;
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
                        //ListBoxee.Items.Add(s);
                    }
                    else if (s.GetType() == typeof(message_error))
                    {
                        var buff = (message_error)s;
                        await Dispatcher.InvokeAsync(() => ListBoxee.Items.Add(buff.message));
                        break;
                    }
                }
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            client.Disconect(0);
            Connect.IsEnabled = true;
            Disconnect.IsEnabled = false;
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            server = new Server(8080);
            server.Connect();
        }
    }
}
