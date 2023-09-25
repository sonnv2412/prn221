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

namespace TwowayRemote01
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
        StreamWriter streamWriter;

        Thread th_message, th_beep, th_playsound;

        //Commands from Client:
        const string HELP = "h",
                    MESSAGE = "m",
                    BEEP = "b",
                    PLAYSOUND = "p",
                    SHUTDOWNSERVER = "s";

        const string strHelp = "Command Menu:\r\n" +
                                "h This Help\r\n" +
                                "m Message\r\n" +
                                "b Beep\r\n" +
                                "p Playsound\r\n" +
                                "s Shutdown the Server Process and Port\r\n";
        public MainWindow()
        {
            InitializeComponent();
            tcpListener = new TcpListener(System.Net.IPAddress.Any, 4444);
            tcpListener.Start();
            for (; ; ) RunServer();
        }
    
        private void RunServer()
        {
            socketForClient = tcpListener.AcceptSocket();
            networkStream = new NetworkStream(socketForClient);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);

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
                    if (line.LastIndexOf(HELP) >= 0)
                    {
                        streamWriter.Write(strHelp);
                        streamWriter.Flush();
                    }
                    if (line.LastIndexOf(MESSAGE) >= 0)
                    {
                        th_message =
                        new Thread(new ThreadStart(MessageCommand));
                        th_message.Start();
                    }
                    if (line.LastIndexOf(BEEP) >= 0)
                    {
                        th_beep = new Thread(new ThreadStart(BeepCommand));
                        th_beep.Start();
                    }
                    if (line.LastIndexOf(PLAYSOUND) >= 0)
                    {
                        th_playsound = new Thread(new ThreadStart(PlaySoundCommand));
                        th_playsound.Start();
                    }
                    if (line.LastIndexOf(SHUTDOWNSERVER) >= 0)
                    {
                        streamWriter.Flush();
                        CleanUp();
                        System.Environment.Exit(System.Environment.ExitCode);
                    }
                }//--end of while loop
            }
            catch (Exception exc)
            {
                CleanUp();
            }
        }//--end of RunServer()

        private void MessageCommand()
        {
            MessageBox.Show("Hello World");
        }
        private void BeepCommand()
        {
            Console.Beep(500, 2000);
        }
        private void PlaySoundCommand()
        {
            System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer();
            soundPlayer.SoundLocation = @"C:\Windows\Media\Ring01.wav";
            soundPlayer.Play();
        }
        private void CleanUp()
        {
            streamReader.Close();
            networkStream.Close();
            socketForClient.Close();
        }
    }
}
