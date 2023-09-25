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

namespace TwowayRemote02
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

        Thread th_message,
                th_beep,
                th_playsound;

        private enum command
        {
            HELP = 1,
            MESSAGE = 2,
            BEEP = 3,
            PLAYSOUND = 4,
            SHUTDOWNSERVER = 5
        }
 
        const string strHelp = "Command Menu:\r\n" +
                                "1 This Help\r\n" +
                                "2 Message\r\n" +
                                "3 Beep\r\n" +
                                "4 Playsound\r\n" +
                                "5 Shutdown the Server Process and Port\r\n";

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
                streamWriter.Write("Connected to RAT Server. Type 1 for help\r\n");
                streamWriter.Flush();
                string line; Int16 intCommand = 0;

                while (true)
                {
                    line = "";
                    line = streamReader.ReadLine();
                    intCommand = GetCommandFromLine(line);

                    switch ((command)intCommand)
                    {
                        case command.HELP:
                            streamWriter.Write(strHelp);
                            streamWriter.Flush(); break;
                        case command.MESSAGE:
                            th_message =
                            new Thread(new ThreadStart(MessageCommand));
                            th_message.Start(); break;
                        case command.BEEP:
                            th_beep = new Thread(new ThreadStart(BeepCommand));
                            th_beep.Start(); break;
                        case command.PLAYSOUND:
                            th_playsound = new Thread(new ThreadStart(PlaySoundCommand));
                            th_playsound.Start(); break;
                        case command.SHUTDOWNSERVER:
                            streamWriter.Flush();
                            CleanUp();
                            System.Environment.Exit(System.Environment.ExitCode);
                            break;
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

        private Int16 GetCommandFromLine(string strline)
        {
            Int16 intExtractedCommand = 0;
            int i; Char character;
            StringBuilder stringBuilder = new StringBuilder();
            for (i = 0; i < strline.Length; i++)
            {
                character = Convert.ToChar(strline[i]);
                if (Char.IsDigit(character))
                {
                    stringBuilder.Append(character);
                }
            }
            try
            {
                intExtractedCommand =
                Convert.ToInt16(stringBuilder.ToString());
            }
            catch (Exception err) { }
            return intExtractedCommand;
        }
    }
}
