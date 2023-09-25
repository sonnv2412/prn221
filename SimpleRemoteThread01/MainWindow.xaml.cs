using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace SimpleRemoteThread01
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

        Thread th_message, th_beep;

        public MainWindow()
        {
            InitializeComponent();
            tcpListener = new TcpListener(System.Net.IPAddress.Any, 2222);
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
                //Command loop, LastIndexOf is to search within
                //the Network Stream for any command strings
                //sent by the Client

                while (true)
                {
                    line = "";
                    line = streamReader.ReadLine();
                    if (line.LastIndexOf("m") >= 0)
                    {
                        th_message = new Thread(new ThreadStart(MessageCommand));
                        th_message.Start();
                    }
                    if (line.LastIndexOf("b") >= 0)
                    {
                        th_beep = new Thread(new ThreadStart(BeepCommand));
                        th_beep.Start();
                    }
                    if (line.LastIndexOf("q") >= 0)
                        throw new Exception(); //so that it will be caught below and gracefully close
                }//end while
            }
            catch (Exception exc)
            {
                streamReader.Close();
                networkStream.Close();
                socketForClient.Close();
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        private void MessageCommand()
        {
            MessageBox.Show("Hello World");
        }
        private void BeepCommand()
        {
            Console.Beep(500, 2000);
        }
    }
}
