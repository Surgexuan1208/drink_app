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
                StackPanel sp=new StackPanel();
                sp.Orientation=Orientation.Horizontal;

                CheckBox cb=new CheckBox();
                cb.Foreground = Brushes.Blue;
                cb.Content = $"{drink.Key}:{drink.Value}元";
                cb.FontFamily = new FontFamily("Consolas");
                cb.FontSize = 18;
                cb.Margin=new Thickness(5);
                cb.Width = 200;
                sp.Children.Add(cb);

                Slider sd=new Slider();
                sd.Width = 500;
                sd.Value = 0;
                sd.Minimum = 0;
                sd.Maximum = 50;
                sd.IsSnapToTickEnabled = true;
                sp.Children.Add(sd);

                Label lb=new Label();
                lb.Width = 50;
                lb.Content = "0";
                lb.FontFamily = new FontFamily("Consolas");
                lb.FontSize = 18;
                lb.Foreground = Brushes.Red;
                sp.Children.Add(lb);

                Binding bd=new Binding("Value");
                bd.Source = sd;
                lb.SetBinding(ContentProperty, bd);

                stackpanel_dispaly.Children.Add(sp);
            }
        }
        private void AddNewDrink(Dictionary<string, int> MyDrinks)
        {
            MyDrinks.Add("紅茶大杯",60);
            MyDrinks.Add("紅茶小杯",40);
            MyDrinks.Add("綠茶大杯",60);
            MyDrinks.Add("綠茶小杯",40);
            MyDrinks.Add("咖啡大杯",80);
            MyDrinks.Add("咖啡小杯",50);
            MyDrinks.Add("可樂大杯",40);
            MyDrinks.Add("可樂小杯",20);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.IsChecked == true) takeout = rb.Content.ToString();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceOrder(orders);
            double total = 0.0;
            string message = "";
            double sellPrice = 0.0;
            string displaystring = $"本次訂購為{takeout}，清單如下：\n";
            foreach(KeyValuePair<string, int> item in orders)
            {
                string drinkname=item.Key;
                int amount = orders[drinkname];
                int price = drinks[drinkname];
                total += price * amount;
                displaystring += $"{drinkname}*{amount}杯，每杯{price}元，總共{price*amount}元\n";
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
        }
        private void PlaceOrder(Dictionary<string, int> myOrders)
        {
            myOrders.Clear();
            for(int i = 0; i < stackpanel_dispaly.Children.Count; i++)
            {
                StackPanel sp = stackpanel_dispaly.Children[i] as StackPanel;
                CheckBox cb= sp.Children[0] as CheckBox;
                Slider sd= sp.Children[1] as Slider;
                string drinkname=cb.Content.ToString().Substring(0,4);
                int quantity = (int)sd.Value;
                if(quantity != 0 && cb.IsChecked==true)
                {
                    myOrders.Add(drinkname, quantity);
                }
            }
        }
    }
}