using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetCsharpFluentAPI2
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FluentContext bd = new FluentContext())
            {
                User user = bd.Users.FirstOrDefault(u => u.Login == textBox1.Text);
                if (textBox1.Text != "" && textBox2.Text != "" && user == null)
                {
                    User us1 = new User { Login = textBox1.Text, Password = textBox2.Text };
                    bd.Users.Add(us1);
                    bd.SaveChanges();
                    MessageBox.Show("Регистрация успешна!");
                    WriteToJsonFile(new { CurrentUser = us1.Login, Message = "Пользователь зарегистрировался!" });

                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FluentContext bd = new FluentContext())
            {
                User user = bd.Users.FirstOrDefault(u => u.Login == textBox1.Text);
                if (textBox1.Text != "" && textBox2.Text != "" && user != null)
                {
                    if (user.Password == textBox2.Text)
                    {
                        MessageBox.Show("Вы вошли в систему!");
                        WriteToJsonFile(new { CurrentUser = user.Login, Message = "Пользователь вошел в систему!" });
                        this.Hide();
                        Form1 mainform = new Form1(user.Login);
                        mainform.ShowDialog();
                        this.Close();
                    }
                }
            }
        }
        private void WriteToJsonFile(object data)
        {
            string filePath = "C:\\Users\\fasta\\Desktop\\PROG\\C#\\2 semestr\\HW\\NetCsharpFluentAPI2\\NetCsharpFluentAPI2\\json1.json";

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true 
            };
            using (StreamWriter sw = new StreamWriter(filePath, append: true))
            {
                string json = JsonSerializer.Serialize(data, options);
                sw.WriteLine(json);
            }
        }
    }
}
