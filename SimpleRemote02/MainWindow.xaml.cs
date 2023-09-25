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

namespace SimpleRemote02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpListener tcpListener;
        Socket socketForClient;
        NetworkStream networkStream;
        StreamReader streamReader;
        public MainWindow()
        {
            InitializeComponent();
            tcpListener = new TcpListener(System.Net.IPAddress.Any, 7777);
            tcpListener.Start();
            RunServer();
        }
        private void RunServer()
        {
            socketForClient = tcpListener.AcceptSocket();
            networkStream = new NetworkStream(socketForClient);
            streamReader = new StreamReader(networkStream);
            try
            {
                string line;
                while (true)
                {
                    line = "";
                    line = streamReader.ReadLine();
                    if (line.LastIndexOf("m") >= 0)
                        MessageBox.Show("Hello World");
                    if (line.LastIndexOf("b") >= 0)
                        Console.Beep(500, 2000);
                    if (line.LastIndexOf("q") >= 0)
                        throw new Exception();
                }//end while
            }
            catch (Exception err)
            {
                streamReader.Close();
                networkStream.Close();
                socketForClient.Close();
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }
    }
}
