using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace izmenenie_parola
{
    /// <summary>
    /// Логика взаимодействия для Смена_логина__пароля.xaml
    /// </summary>
    public partial class Смена_логина__пароля : Window
    {
        public Смена_логина__пароля()
        {
            InitializeComponent();
        }
        public static string CalculateHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // Преобразование в шестнадцатеричное представление
                }
                return builder.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var _db = new IzmenenieEntities1())
            {
                string oldPass = CalculateHash(lastpass.Text);

                var temp = _db.ДанныеПользователя.FirstOrDefault(x => x.Логин == loginnn.Text && x.Пароль == oldPass);
                if (temp == null)
                {
                    MessageBox.Show("Такого пользователя нет");
                    return;
                }

                string pattern = "^(?=.*[a-zа-я])(?=.*[A-ZА-Я])(?=.*\\d)[a-zA-Zа-яА-Я\\d~!?@#$%^&*_+\\(\\)\\[\\]{}><\\/\\\\|\"'.,:;]{8,128}$";
                bool isValidPassword = Regex.IsMatch(newpassword.Text, pattern);
                if (!isValidPassword)
                {
                    MessageBox.Show("Пароль должен быть: не менее 8 символов;\r\nне более 128 символов;\r\nкак минимум одна заглавная и одна" +
                        " строчная буква;\r\nтолько латинские или кириллические буквы;\r\nкак минимум одна цифра;\r\nтолько арабские циф" +
                        "ры;\r\nбез пробелов;\r\nДругие допустимые символы:~ ! ? @ # $ % ^ & * _ - + ( ) [ ] { } > < / \\ | \" ' . , : ;");
                    return;
                }

                if(oldPass == CalculateHash(newpassword.Text))
                {
                    MessageBox.Show("Одинаковый пароль, измените его!");
                    return;

                }

                temp.Логин = loginnn.Text;
                temp.Пароль = CalculateHash(newpassword.Text);
                _db.SaveChanges();
                MessageBox.Show("Вы сменили пароль!");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }
    }
}
