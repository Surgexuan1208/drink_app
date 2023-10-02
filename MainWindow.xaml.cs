using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<String, int> drinks = new Dictionary<String, int>();
        Dictionary<String, int> orders = new Dictionary<String, int>();
        string takeout = "";
        public MainWindow()
        {
            InitializeComponent();
            AddNewDrink(drinks);
            ShowMenu(drinks);
        }
        private void ShowMenu(Dictionary<string, int> MyDrinks)
        {
            foreach (var drink in MyDrinks)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;

                CheckBox cb = new CheckBox();
                cb.Foreground = Brushes.Blue;
                cb.Content = $"{drink.Key}:{drink.Value}元";
                cb.FontFamily = new FontFamily("Consolas");
                cb.FontSize = 18;
                cb.Margin = new Thickness(5);
                cb.Width = 300;
                sp.Children.Add(cb);

                Slider sd = new Slider
                {
                    Width = 450,
                    Value = 0,
                    Minimum = 0,
                    Maximum = 50,
                    IsSnapToTickEnabled = true,
                };
                sp.Children.Add(sd);

                Label lb = new Label();
                lb.Width = 50;
                lb.Content = "0";
                lb.FontFamily = new FontFamily("Consolas");
                lb.FontSize = 18;
                lb.Foreground = Brushes.Red;
                sp.Children.Add(lb);

                Binding bd = new Binding("Value");
                bd.Source = sd;
                lb.SetBinding(ContentProperty, bd);

                stackpanel_dispaly.Children.Add(sp);
            }
        }
        private void AddNewDrink(Dictionary<string, int> MyDrinks)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV檔|*.csv|文字檔案|*.txt|所有檔案 (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(filename);
                foreach (var line in lines)
                {
                    string[] tokens = line.Split(',');
                    string drinkName = tokens[0];
                    int price = int.Parse(tokens[1]);
                    MyDrinks.Add(drinkName, price);
                }
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.IsChecked == true) takeout = rb.Content.ToString();
        }
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceOrder(orders);
            DisplayOrder_textblock_ver();
            WriteOutFile();
        }
        private void WriteOutFile()
        {
            var result = MessageBox.Show("是否輸出明細表?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Stream myStream;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "文字檔案|*.txt|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    FileStream filestream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(filestream);
                    sw.Write(textarea.Text);
                    sw.Close();
                }
            }
        }
        private void DisplayOrder_textblock_ver()
        {
            textarea.Inlines.Clear();
            Run titleString = new Run
            {
                Text = "您所訂購的飲品為",
                FontSize = 16,
                Foreground = Brushes.Brown
            };
            Run takeoutString = new Run
            {
                Text = $"{takeout}",
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            textarea.Inlines.Add(titleString);
            textarea.Inlines.Add(takeoutString);
            textarea.Inlines.Add(new Run { Text = $"，本次訂購清單如下：\n", FontSize = 16 });
            double total = 0.0;
            double sellPrice = 0.0;
            string message;
            int i = 0;
            int shower=0;
            foreach (KeyValuePair<string, int> item in orders)
            {
                i++;
                string drinkname = item.Key;
                int amount = orders[drinkname];
                int price = drinks[drinkname];
                
                total += price * amount;
                textarea.Inlines.Add(new Run { Text = $"飲料品項{i}：{drinkname}*{amount}杯，每杯{price}元，總共{price * amount}元\n", FontSize = 16 });
            }
            if (total >= 500)
            {
                message = "訂購滿500元以上者8折";
                shower = 1;
                sellPrice = total * 0.8;
            }
            else if (total >= 300)
            {
                message = "訂購滿300元以上者85折";
                shower = 2;
                sellPrice = total * 0.85;
            }
            else if (total >= 200)
            {
                message = "訂購滿200元以上者9折";
                shower = 3;
                sellPrice = total * 0.9;
            }
            else
            {
                message = "訂購未滿200元不打折";
                shower = 4;
                sellPrice = total;
            }
            Run show=new Run();
            show.Text = $"您總共訂購{orders.Count}項飲料，總計{total}元。{message}，總計需付款{(int)sellPrice}元。";
            show.FontSize = 16;
            FontWeight = FontWeights.Bold;
            switch(shower){
                case 1:
                    show.Background = Brushes.Wheat;
                    show.Foreground = Brushes.Green;
                    break;
                case 2:
                    show.Background = Brushes.Gray;
                    show.Foreground = Brushes.Gold;
                    break;
                case 3:
                    show.Background = Brushes.White;
                    show.Foreground = Brushes.Red;
                    break;
                case 4:
                    show.Background = Brushes.Azure;
                    show.Foreground = Brushes.Gray;
                    break;
            }
            Italic summary = new Italic(show);
            textarea.Inlines.Add(summary);
        }

        /*private void DisplayOrder_textbox_ver()
        {
            double total = 0.0;
            string message = "";
            double sellPrice = 0.0;
            string displaystring = $"本次訂購為{takeout}，清單如下：\n";
            foreach (KeyValuePair<string, int> item in orders)
            {
                string drinkname = item.Key;
                int amount = orders[drinkname];
                int price = drinks[drinkname];
                total += price * amount;
                displaystring += $"{drinkname}*{amount}杯，每杯{price}元，總共{price * amount}元\n";
            }
            if (total >= 500)
            {
                message = "訂購滿500元以上者8折";
                sellPrice = total * 0.8;
            }
            else if (total >= 300)
            {
                message = "訂購滿300元以上者85折";
                sellPrice = total * 0.85;
            }
            else if (total >= 200)
            {
                message = "訂購滿200元以上者9折";
                sellPrice = total * 0.9;
            }
            else
            {
                message = "訂購未滿200元不打折";
                sellPrice = total;
            }
            displaystring += $"您總共訂購{orders.Count}項飲料，總計{total}元。{message}，總計需付款{(int)sellPrice}元。";
            textarea.Text = displaystring;
        }*/
        private void PlaceOrder(Dictionary<string, int> myOrders)
        {
            myOrders.Clear();
            for (int i = 0; i < stackpanel_dispaly.Children.Count; i++)
            {
                StackPanel sp = stackpanel_dispaly.Children[i] as StackPanel;
                CheckBox cb = sp.Children[0] as CheckBox;
                Slider sd = sp.Children[1] as Slider;
                string buff = cb.Content.ToString();
                int cblen = 0;
                for (int j = 0; j < buff.Length; j++)
                {
                    if (buff[j] == ':')
                    {
                        cblen = j;
                        break;
                    }
                }
                string drinkname = cb.Content.ToString().Substring(0, cblen);
                int quantity = (int)sd.Value;
                if (quantity != 0 && cb.IsChecked == true)
                {
                    myOrders.Add(drinkname, quantity);
                }
            }
        }
    }
}