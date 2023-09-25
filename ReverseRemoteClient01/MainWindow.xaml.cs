using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace ReverseRemoteClient01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpListener tcpListener;
        Socket socketForServer;
        NetworkStream networkStream;
        StreamWriter streamWriter;
        StreamReader streamReader;
        StringBuilder strInput;
        Thread th_StartListen, th_RunClient;
        public MainWindow()
        {
            InitializeComponent();
            th_StartListen = new Thread(new ThreadStart(StartListen));
            th_StartListen.Start();
            //textBox2.Focus();
        }
        private void StartListen()
        {
            tcpListener = new TcpListener(System.Net.IPAddress.Any, 6666);
            tcpListener.Start();
            //toolStripStatusLabel1.Text = "Listening on port 6666 ...";
            for (; ; )
            {
                socketForServer = tcpListener.AcceptSocket();
                IPEndPoint ipend = (IPEndPoint)socketForServer.RemoteEndPoint;
                th_RunClient = new Thread(new ThreadStart(RunClient));
                th_RunClient.Start();
            }
        }

        private void RunClient()
        {
            networkStream = new NetworkStream(socketForServer);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
            strInput = new StringBuilder();

            while (true)
            {
                try
                {
                    strInput.Append(streamReader.ReadLine());
                    strInput.Append("\r\n");
                }
                catch (Exception err)
                {
                    Cleanup();
                    break;
                }
                DisplayMessage(strInput.ToString());
                strInput.Remove(0, strInput.Length);
            }
        }

        private void Cleanup()
        {
            try
            {
                streamReader.Close();
                streamWriter.Close();
                networkStream.Close();
                socketForServer.Close();
            }
            catch (Exception err) { }

        }

        private delegate void DisplayDelegate(string message);
        private void DisplayMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new DisplayDelegate(DisplayMessage), new object[] { message });
                return;
            }
            else
            {
                textBox1.AppendText(message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String fileContent = getFileContent(FilePathTextBox.Text);
            try
            {
                strInput.Append(fileContent);
                streamWriter.WriteLine(strInput);
                streamWriter.Flush();
                strInput.Remove(0, strInput.Length);
                if (fileContent == "exit") Cleanup();
                if (fileContent == "terminate") Cleanup();
                if (fileContent == "cls") textBox1.Text = "";
                fileContent = "";

            }
            catch (Exception err) { }
            StudentManager studentManager = new StudentManager(FilePathTextBox.Text);
            studentListView.ItemsSource = studentManager.GetStudents();
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt;*.json)|*.txt;*.json"; // Restrict to .txt files

            if (openFileDialog.ShowDialog() == true)
            {
                // Display the selected file path in the TextBox
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private String getFileContent(String filePath)
        {
            // Create a StringBuilder to store the file contents
            StringBuilder stringBuilder = new StringBuilder();
            // Check if the file exists
            if (File.Exists(filePath))
            {            
                // Read the file contents and append them to the StringBuilder
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        stringBuilder.AppendLine(line);
                    }
                }

                // The file contents are now stored in the StringBuilder
                string fileContents = stringBuilder.ToString();

                // You can use fileContents as needed in your application
                Console.WriteLine("File Contents:");
                Console.WriteLine(fileContents);
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
            return stringBuilder.ToString();
        }

    }
}
