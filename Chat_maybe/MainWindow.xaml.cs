using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat_maybe
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Interface client = null;
        Interface server = null;
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
        }

        async private void set_msg()
        {
            while (true)
            {
                string s = client.Read_ms();
                if (s != null)
                {
                    await Dispatcher.InvokeAsync(() => ListBoxee.Items.Add(s));
                    //ListBoxee.Items.Add(s);
                }
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            client.Disconect(0);
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            server = new Server(8080);
            server.Connect();
        }
    }
}
