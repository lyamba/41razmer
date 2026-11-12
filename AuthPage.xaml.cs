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
using System.Windows.Threading;

namespace Кудрявцева41размер
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
        public partial class AuthPage : Page
        {
            private string currentCaptcha = "";
            private int failedAttempts = 0;
public AuthPage()
            {
                InitializeComponent();
            CaptchaPanel.Visibility = Visibility.Collapsed;
            CaptchaTB.Visibility = Visibility.Collapsed;
            GenerateCaptcha();
            }
            

            private void LoginBtn_Click(object sender, RoutedEventArgs e)
            {
                string login = LoginTB.Text;
                string password = PassTB.Text;

                if (failedAttempts > 0)
                {
                    if (CaptchaTB.Text != currentCaptcha)
                    {
                    MessageBox.Show("Капча не пройдена");
                        LoginBtn.IsEnabled = false;
                        DispatcherTimer timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(10);
                        timer.Tick += (s, args) => { LoginBtn.IsEnabled = true; timer.Stop(); };
                        timer.Start();
                        GenerateCaptcha();
                        return;
                    }
                }

                var user = Kudryavtseva41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);

                if (user != null)
                {
                    Manager.MainFrame.Navigate(new ProductPage(user));
                    LoginTB.Text = "";
                    PassTB.Text = "";
                }
            else
            {
                MessageBox.Show("Неверно введены логин или пароль");
                failedAttempts++;

                CaptchaPanel.Visibility = Visibility.Visible;
                CaptchaTB.Visibility = Visibility.Visible;

                CaptchaTB.Focus();
                GenerateCaptcha();
            }
        }

            private void Guest_Click(object sender, RoutedEventArgs e)
            {
                Manager.MainFrame.Navigate(new ProductPage(null));
            }

            private void GenerateCaptcha()
            {
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random random = new Random();

                currentCaptcha = "";
                for (int i = 0; i < 4; i++)
                {
                    currentCaptcha += chars[random.Next(chars.Length)];
                }

                CaptchaOneWord.Text = currentCaptcha[0].ToString();
                CaptchaTwoWord.Text = currentCaptcha[1].ToString();
                CaptchaThreeWord.Text = currentCaptcha[2].ToString();
                CaptchaFourWord.Text = currentCaptcha[3].ToString();
            }
        }
    }
