using System;
using System.Collections.Generic;
using System.Linq;
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

using System.IO;
using System.Net.Sockets;
using System.Diagnostics;  //truyen cac message giua cac class, process

namespace ReverseRemoteServer02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    TcpClient tcpClient;            // khai bao client de lang nghe co client ket noi
    NetworkStream networkStream;    // Luong data network
    StreamWriter streamWriter;      // Luong data send to client
    StreamReader streamReader;      // Luong data receive from client
    Process processCmd;             // Khai bao 1 tien trinh trong window       
    StringBuilder strInput;         // tao chuoi string tu luong StreamWriter/ StreamReader
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
