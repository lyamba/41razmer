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
using System.Windows.Shapes;

namespace Кудрявцева41размер
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, User user)
        {
            InitializeComponent();

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;

            if (user != null)
            {
                ClientFIO.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
            }
            else
            {
                ClientFIO.Text = "Вы вошли как гость";
            }

            OrderListView.ItemsSource = selectedOrderProducts;

            PickupCombo.ItemsSource = Kudryavtseva41Entities.GetContext().PickupPoint.ToList();
            PickupCombo.DisplayMemberPath = "FullAddress";
            PickupCombo.SelectedValuePath = "PickupPointID";

            DateOrder.SelectedDate = DateTime.Now;
            UpdateDeliveryDate();
            UpdateSums();
        }

        private void UpdateDeliveryDate()
        {
            int deliveryDays = 3;

            foreach (var item in selectedOrderProducts)
            {
                int stockRemaining = item.Product.ProductQuantityInStock - item.ProductCount;

                if (stockRemaining < 3)
                {
                    deliveryDays = 6;
                    break;
                }
            }

            if (selectedOrderProducts.Count == 0) deliveryDays = 6;

            DateDelivery.SelectedDate = DateTime.Now.AddDays(deliveryDays);
        }

        private void UpdateSums()
        {
            decimal totalSum = 0;
            decimal discountSum = 0;

            foreach (var item in selectedOrderProducts)
            {
                decimal cost = (decimal)item.Product.ProductCost;
                decimal discount = (decimal)item.Product.ProductDiscountAmount;

                decimal finalCost = cost - (cost * discount / 100);

                totalSum += finalCost * item.ProductCount;
                discountSum += (cost - finalCost) * item.ProductCount;
            }

            TotalSumTB.Text = $"{totalSum} руб.";
            DiscountSumTB.Text = $"{discountSum} руб.";
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as OrderProduct;
            item.ProductCount++;
            OrderListView.Items.Refresh();

            UpdateSums();
            UpdateDeliveryDate();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as OrderProduct;
            if (item.ProductCount > 1)
            {
                item.ProductCount--;
            }
            else
            {
                selectedOrderProducts.Remove(item);
            }
            OrderListView.Items.Refresh();

            UpdateSums();
            UpdateDeliveryDate();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PickupCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи");
                return;
            }

            var currentUser = Kudryavtseva41Entities.GetContext().User.ToList().FirstOrDefault(p => p.UserSurname + " " + p.UserName + " " + p.UserPatronymic == ClientFIO.Text);

            currentOrder.OrderDate = DateOrder.SelectedDate.Value;
            currentOrder.OrderDeliveryDate = DateDelivery.SelectedDate.Value;
            currentOrder.PickupPointID = (int)PickupCombo.SelectedValue;
            currentOrder.OrderStatus = "Новый";
            currentOrder.OrderCode = new Random().Next(100, 999);

            if (ClientFIO.Text != "Вы вошли как гость")
            {
                var userInDb = Kudryavtseva41Entities.GetContext().User.ToList()
                    .FirstOrDefault(p => p.UserSurname + " " + p.UserName + " " + p.UserPatronymic == ClientFIO.Text);

                if (userInDb != null)
                    currentOrder.UserID = userInDb.UserID;
            }
            else
            {
                currentOrder.UserID = null;
            }

            try
            {
                Kudryavtseva41Entities.GetContext().Order.Add(currentOrder);
                Kudryavtseva41Entities.GetContext().SaveChanges();

                foreach (var item in selectedOrderProducts)
                {
                    item.OrderID = currentOrder.OrderID;
                    Kudryavtseva41Entities.GetContext().OrderProduct.Add(item);
                }

                Kudryavtseva41Entities.GetContext().SaveChanges();

                MessageBox.Show("Заказ оформлен");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}