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


namespace Dema
{
    public partial class UserWindow : Window
    {
        public UserWindow(string login)
        {
            InitializeComponent();

            lblWelcome.Text = $"👋 Здравствуйте,пользователь!";
            lblInfo.Text = $"Вы вошли в систему как ПОЛЬЗОВАТЕЛЬ\n\n" +
                          $"📅 Дата входа: {System.DateTime.Now:dd.MM.yyyy HH:mm}\n\n" +
                          $"🖥 Ваши права: просмотр информации";

            btnExit.Click += btnExit_Click;
        }

        void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWin = new MainWindow();
            mainWin.Show();
            this.Close();
        }
    }
}