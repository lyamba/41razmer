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
using System.ComponentModel;

namespace Кудрявцева41размер
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    
        public partial class ProductPage : Page, INotifyPropertyChanged
        {

        public ProductPage(User user)
        {
            InitializeComponent();
            DataContext = this;

            if (user != null)
            {
                FIOTB.Text = "Вы авторизованы как " + user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;

                switch (user.RoleID)
                {
                    case 1: RoleTB.Text = "Роль: Администратор"; break;
                    case 2: RoleTB.Text = "Роль: Менеджер"; break;
                    case 3: RoleTB.Text = "Роль: Клиент"; break;
                }
            }
            else
            {
                FIOTB.Text = "Вы вошли как гость";
                RoleTB.Text = "Роль: Гость";
            }

            UpdateData();
        }

        private void LoadData()
            {
                var products = Kudryavtseva41Entities.GetContext().Product.ToList();
                ProductListView.ItemsSource = products;
            }
        private int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalCount"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateData()
        {
            var products = Kudryavtseva41Entities.GetContext().Product.ToList();
            TotalCount = Kudryavtseva41Entities.GetContext().Product.Count();

            if (!string.IsNullOrWhiteSpace(TBoxSearch.Text))
                products = products.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            if (ComboDiscount.SelectedIndex == 1)
                products = products.Where(p => p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount < 10).ToList();
            else if (ComboDiscount.SelectedIndex == 2)
                products = products.Where(p => p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount < 15).ToList();
            else if (ComboDiscount.SelectedIndex == 3)
                products = products.Where(p => p.ProductDiscountAmount >= 15).ToList();

            if (RadioAsc.IsChecked == true)
                products = products.OrderBy(p => p.ProductCost).ToList();
            else if (RadioDesc.IsChecked == true)
                products = products.OrderByDescending(p => p.ProductCost).ToList();

            ProductListView.ItemsSource = products;
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e) { if (IsLoaded) UpdateData(); }
        private void ComboDiscount_SelectionChanged(object sender, SelectionChangedEventArgs e) { if (IsLoaded) UpdateData(); }
        private void Sort_Checked(object sender, RoutedEventArgs e) { if (IsLoaded) UpdateData(); }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AuthPage());
        }
    }
    }