using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleRemote01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.Hide();

            TcpListener tcpListener = new TcpListener(System.Net.IPAddress.Any, 9999);
            tcpListener.Start();

            Socket socketForClient = tcpListener.AcceptSocket();
            NetworkStream networkStream = new NetworkStream(socketForClient);
            StreamReader streamReader = new StreamReader(networkStream);

            string line = streamReader.ReadLine();

            if (line.LastIndexOf("m") >= 0) MessageBox.Show("Hello");

            streamReader.Close();
            networkStream.Close();
            socketForClient.Close();
            System.Environment.Exit(0);
        }
    }
}
