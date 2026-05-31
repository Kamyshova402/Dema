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

namespace Dema
{
    public partial class MainWindow : Window
    {
        List<User> users = new List<User>();
        string captchaCode;
        int errorCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            users.Add(new User { Login = "admin", Password = "123", Role = "admin", IsBlocked = false });
            users.Add(new User { Login = "user", Password = "123", Role = "user", IsBlocked = false });
          

            GenerateCaptcha();

            btnLogin.Click += btnLogin_Click;
            btnRefresh.Click += btnRefresh_Click;
        }

        void GenerateCaptcha()
        {
            Random rnd = new Random();
            captchaCode = rnd.Next(1000, 9999).ToString();
            lblCaptcha.Text = captchaCode;
        }

        void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
            txtCaptcha.Clear();
        }

        void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string pass = txtPass.Password;
            string captcha = txtCaptcha.Text.Trim();

            if (captcha != captchaCode)
            {
                lblError.Text = "❌ Неверная капча!";
                txtCaptcha.Clear();
                GenerateCaptcha();
                return;
            }

            User user = users.Find(u => u.Login == login && u.Password == pass);

            if (user != null)
            {
                if (user.IsBlocked)
                {
                    lblError.Text = "🔒 Вы заблокированы! Обратитесь к администратору.";
                    return;
                }

                errorCount = 0;
                lblError.Text = "";

                if (user.Role == "admin")
                {
                    AdminWindow adminWin = new AdminWindow(users);
                    adminWin.Show();
                    this.Close();
                }
                else
                {
                    UserWindow userWin = new UserWindow(login);
                    userWin.Show();
                    this.Close();
                }
            }
            else
            {
                errorCount++;
                lblError.Text = $"❌ Неверный логин или пароль! Попыток: {errorCount}/3";

                if (errorCount >= 3)
                {
                    User blockUser = users.Find(u => u.Login == login);
                    if (blockUser != null)
                    {
                        blockUser.IsBlocked = true;
                        lblError.Text = $"⚠️ Пользователь {login} ЗАБЛОКИРОВАН!";
                    }
                    else
                    {
                        lblError.Text = "⚠️ 3 ошибки! Попробуйте позже.";
                        btnLogin.IsEnabled = false;
                    }
                }

                txtCaptcha.Clear();
                GenerateCaptcha();
            }
        }
    }

    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsBlocked { get; set; }
    }
}