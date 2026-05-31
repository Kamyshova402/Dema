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
    public partial class AdminWindow : Window
    {
        List<User> users;

        public AdminWindow(List<User> userList)
        {
            InitializeComponent();
            users = userList;

            RefreshList();

            btnUnblock.Click += btnUnblock_Click;
            btnDelete.Click += btnDelete_Click;
            btnAdd.Click += btnAdd_Click;
            btnRefresh.Click += btnRefresh_Click;
        }

        void RefreshList()
        {
            lstUsers.Items.Clear();
            int blockedCount = 0;

            foreach (var u in users)
            {
                string status = u.IsBlocked ? "🔴 ЗАБЛОКИРОВАН" : "🟢 АКТИВЕН";
                lstUsers.Items.Add($"{u.Login,-15} | {u.Role,-10} | {status}");
                if (u.IsBlocked) blockedCount++;
            }

            lblStats.Text = $"📊 Всего: {users.Count} | Заблокировано: {blockedCount}";
        }

        void btnUnblock_Click(object sender, RoutedEventArgs e)
        {
            if (lstUsers.SelectedIndex >= 0)
            {
                string selected = lstUsers.SelectedItem.ToString();
                string login = selected.Split('|')[0].Trim();
                User user = users.Find(u => u.Login == login);

                if (user != null && user.IsBlocked)
                {
                    user.IsBlocked = false;
                    RefreshList();
                    MessageBox.Show($"✅ Пользователь {login} разблокирован!", "Успех");
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя", "Ошибка");
            }
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstUsers.SelectedIndex >= 0)
            {
                string selected = lstUsers.SelectedItem.ToString();
                string login = selected.Split('|')[0].Trim();

                if (login == "admin")
                {
                    MessageBox.Show("❌ Нельзя удалить администратора!", "Ошибка");
                    return;
                }

                MessageBoxResult result = MessageBox.Show($"Удалить пользователя {login}?", "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    users.RemoveAll(u => u.Login == login);
                    RefreshList();
                    MessageBox.Show($"✅ Пользователь {login} удален", "Успех");
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя", "Ошибка");
            }
        }

        void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Window addWin = new Window();
            addWin.Title = "➕ Добавление пользователя";
            addWin.Width = 350;
            addWin.Height = 300;
            addWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addWin.Owner = this;
            addWin.ResizeMode = ResizeMode.NoResize;

            Grid grid = new Grid { Margin = new Thickness(15) };
            for (int i = 0; i < 6; i++) grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Логин
            grid.Children.Add(new TextBlock { Text = "Логин:", FontSize = 14, VerticalAlignment = VerticalAlignment.Center });
            TextBox txtLogin = new TextBox { Height = 32, FontSize = 13 };
            Grid.SetRow(txtLogin, 0);
            Grid.SetColumn(txtLogin, 1);
            grid.Children.Add(txtLogin);
            Grid.SetRow(grid.Children[0], 0);
            Grid.SetColumn(grid.Children[0], 0);

            // Пароль
            grid.Children.Add(new TextBlock { Text = "Пароль:", FontSize = 14, VerticalAlignment = VerticalAlignment.Center });
            PasswordBox txtPass = new PasswordBox { Height = 32, FontSize = 13 };
            Grid.SetRow(txtPass, 1);
            Grid.SetColumn(txtPass, 1);
            grid.Children.Add(txtPass);
            Grid.SetRow(grid.Children[2], 1);
            Grid.SetColumn(grid.Children[2], 0);

            // Роль
            grid.Children.Add(new TextBlock { Text = "Роль:", FontSize = 14, VerticalAlignment = VerticalAlignment.Center });
            ComboBox cmbRole = new ComboBox { Height = 32, FontSize = 13 };
            cmbRole.Items.Add("user");
            cmbRole.Items.Add("admin");
            cmbRole.SelectedIndex = 0;
            Grid.SetRow(cmbRole, 2);
            Grid.SetColumn(cmbRole, 1);
            grid.Children.Add(cmbRole);
            Grid.SetRow(grid.Children[4], 2);
            Grid.SetColumn(grid.Children[4], 0);

            // Кнопки
            StackPanel btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };

            Button btnSave = new Button { Content = "Сохранить", Width = 100, Height = 35, Margin = new Thickness(5), FontSize = 13 };
            Button btnCancel = new Button { Content = "Отмена", Width = 100, Height = 35, Margin = new Thickness(5), FontSize = 13 };

            btnSave.Click += (s, ev) =>
            {
                string login = txtLogin.Text.Trim();
                string pass = txtPass.Password;
                string role = cmbRole.SelectedItem.ToString();

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
                {
                    MessageBox.Show("Заполните логин и пароль!", "Ошибка");
                    return;
                }

                if (users.Exists(u => u.Login == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка");
                    return;
                }

                users.Add(new User { Login = login, Password = pass, Role = role, IsBlocked = false });
                RefreshList();
                addWin.Close();
                MessageBox.Show("Добро пожаловать, пользователь!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            btnCancel.Click += (s, ev) => addWin.Close();

            btnPanel.Children.Add(btnSave);
            btnPanel.Children.Add(btnCancel);

            Grid.SetRow(btnPanel, 4);
            Grid.SetColumnSpan(btnPanel, 2);
            grid.Children.Add(btnPanel);

            addWin.Content = grid;
            addWin.ShowDialog();
        }

        void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWin = new MainWindow();
            mainWin.Show();
            this.Close();
        }
    }
}
