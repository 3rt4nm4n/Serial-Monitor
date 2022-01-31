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
using System.IO.Ports;
using System.Threading;

namespace Serial_Monitor
{

    public partial class MainWindow : Window
    {
        public static SerialPort sp = new SerialPort("COM1", 9600);//Default value
        public RichTextBox rbx = new RichTextBox();
        public MainWindow()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            
            foreach (var p in ports)
            {
                string[] prt = p.Split();
                PortsComboBox.Items.Add(prt[0]);
            }
            SerialMonTextBox.AppendText("["+DateTime.Now.Hour+":"+DateTime.Now.Minute+ ":" + DateTime.Now.Second+"] "+"Please choose the port name...");
        }

        private void PortsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SerialMonTextBox.Document.Blocks.Clear();
            ConnectButton.IsEnabled = true;
            BaudTextBox.IsEnabled = true;
            DataBitsTextBox.IsEnabled = true;
            StopBitsComboBox.IsEnabled = true;
            ParityComboBox.IsEnabled = true;
            PortsComboBox.IsEnabled = true;
            SendButton.IsEnabled = true;
        }

        public Parity ParityHandler(ComboBox cb)
        {
            switch (cb.SelectedIndex)
            {
                case 0:
                    return Parity.None;
                case 1:
                    return Parity.Odd;
                case 2:
                    return Parity.Even;
                case 3:
                    return Parity.Mark;
                case 4:
                    return Parity.Space;
                default:
                    return Parity.None;
            }
        }

        public StopBits StopBitsHandler(ComboBox cb)
        {
            switch (cb.SelectedIndex)
            {
                case 0:
                    return StopBits.None;
                case 1:
                    return StopBits.One;
                case 2:
                    return StopBits.Two;
                case 3:
                    return StopBits.OnePointFive;
                default:
                    return StopBits.One;
            }
        }

        public void PrintCurrentTİme()
        {
            SerialMonTextBox.AppendText("\n[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "] ");
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string portname = PortsComboBox.SelectedItem.ToString();
            var s = StopBitsHandler(StopBitsComboBox);
            var p = ParityHandler(PortsComboBox);
            var d = Convert.ToInt32(DataBitsTextBox.Text);
            sp = new SerialPort(portname, Convert.ToInt32(BaudTextBox.Text), p,d,s);
            try
            {
                sp.Open();
                if (sp.IsOpen)
                {
                    PrintCurrentTİme();
                    SerialMonTextBox.AppendText("Connected!");
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access to the port is denied. The port may be already in use. Try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
                
            ConnectButton.IsEnabled = false;
            BaudTextBox.IsEnabled = false;
            DisconnectButton.IsEnabled = true;
            DataBitsTextBox.IsEnabled = false;
            StopBitsComboBox.IsEnabled = false;
            ParityComboBox.IsEnabled = false;
            PortsComboBox.IsEnabled = false;
            
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            string portname = PortsComboBox.SelectedItem.ToString();
            var s = StopBitsHandler(StopBitsComboBox);
            var p = ParityHandler(PortsComboBox);
            var d = Convert.ToInt32(DataBitsTextBox.Text);
            sp = new SerialPort(portname, Convert.ToInt32(BaudTextBox.Text), p, d, s);
            sp.Close();
            if (!sp.IsOpen)
            {
                PrintCurrentTİme();
                SerialMonTextBox.AppendText("Disconnected!");
               
            }
                
            ConnectButton.IsEnabled = true;
            SendButton.IsEnabled = false;
            BaudTextBox.IsEnabled = true;
            DataBitsTextBox.IsEnabled = true;
            StopBitsComboBox.IsEnabled = true;
            ParityComboBox.IsEnabled = true;
            PortsComboBox.IsEnabled = true;
            Thread.Sleep(1500);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            sp.Write(InputTextBox.Text);
            PrintCurrentTİme();
            SerialMonTextBox.AppendText("Input:"+InputTextBox.Text);
            Thread.Sleep(200);
            InputTextBox.Text = "";
        }
        private void SerialPort_DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            sp = (SerialPort)sender;
            string id = sp.ReadExisting();
            PrintCurrentTİme();
            SerialMonTextBox.AppendText("Output:" + id);
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SendButton_Click(sender, e);
            }
        }

        private void SerialMonTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SerialMonTextBox.ScrollToEnd();


        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            System.Diagnostics.Process.Start(Environment.GetCommandLineArgs()[0]);
        }
    }
}
