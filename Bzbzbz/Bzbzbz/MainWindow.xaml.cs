using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bzbzbz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        StreamReader? Reader = null;
        StreamWriter? Writer = null;
        ~MainWindow()
        {
            Writer?.Close();
            Reader?.Close();
        }
        TcpClient client = new TcpClient();
        private void Connection_Click(object sender, RoutedEventArgs e)
        {
            string host = Host.Text;
            int port = 8888;
            string? userName = Name.Text;

            try
            {
                client.ConnectAsync(host, port); //подключение клиента
                Reader = new StreamReader(client.GetStream());
                Writer = new StreamWriter(client.GetStream());
                if (Writer is null || Reader is null) return;
                // запускаем новый поток для получения данных
                Task.Run(() => ReceiveMessageAsync(Reader));
                //welcom
                MassegeGenerate($"Добро пожаловать, {userName}", HorizontalAlignment.Center);
                // сначала отправляем имя
                Writer.WriteLineAsync(Name.Text.ToString());
                Writer.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            // запускаем ввод сообщений
            SendMessageAsync();
            MassegeGenerate(Message.Text, HorizontalAlignment.Right);
            Message.Text = default;
        }
        // отправка сообщений
        async Task SendMessageAsync()
        {
            if (Writer is null || Reader is null) return;
            try
            {
                await Writer.WriteLineAsync(Message.Text.ToString());
                await Writer.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        void MassegeGenerate(string massage, HorizontalAlignment horizontalAlignment)
        {
            Border border = new Border();
            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Text = massage;
            textBlock.Foreground = Brushes.White;
            border.CornerRadius = new CornerRadius(8);
            border.Padding = new Thickness(5);
            border.Child = textBlock;
            border.Margin = new Thickness(5);
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.White;
            border.HorizontalAlignment = horizontalAlignment;
            border.VerticalAlignment = VerticalAlignment.Top;
            Chat.Children.Add(border);
        }
        // получение сообщений
        async Task ReceiveMessageAsync(StreamReader reader)
        {
            while (true)
            {
                try
                {
                    // считываем ответ в виде строки
                    string? message = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(message)) continue;
                    Dispatcher.Invoke(() => MassegeGenerate(message, HorizontalAlignment.Left));
                }
                catch
                {
                }
            }
        }
    }
}