using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demo_ofi
{
    public partial class Regis : Form
    {
        private DataBase database;
        public Regis()
        {
            InitializeComponent();

            database = new DataBase();

            // Добавляем роли в ComboBox
            cbRole.Items.Add("Админситратор");
            cbRole.Items.Add("Сотрудник");
            cbRole.Items.Add("Руководитель");
        }

        

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Проверяем, выбрана ли роль
            if (cbRole.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите роль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string role = cbRole.SelectedItem.ToString();

            // Проверяем, уникально ли имя пользователя
            if (CheckIfUserExists(username))
            {
                MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Регистрируем пользователя
            if (RegisterUser(username, password, role))
            {
                MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide(); // Скрываем форму регистрации
                Form1 loginForm = new Form1(); // Возвращаемся к форме входа
                loginForm.Show();
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool RegisterUser(string username, string password, string role)
        {
            bool isRegistered = false;

            // Открываем соединение с базой данных
            database.openConnection();

            // SQL-запрос для добавления пользователя
            string query = "INSERT INTO Users (Username, Password, Role) VALUES (@username, @password, @role)";
            SqlCommand command = new SqlCommand(query, database.getConnection());

            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password); 
            command.Parameters.AddWithValue("@role", role);

            try
            {
                command.ExecuteNonQuery(); // Выполняем команду
                isRegistered = true; // Успешно зарегистрирован
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Закрываем соединение
                database.closeConnection();
            }

            return isRegistered;
        }

        private bool CheckIfUserExists(string username)
        {
            bool exists = false;

            // Открываем соединение с базой данных
            database.openConnection();

            // SQL-запрос для проверки существования пользователя
            string query = "SELECT COUNT(1) FROM Users WHERE Username = @username";
            SqlCommand command = new SqlCommand(query, database.getConnection());

            command.Parameters.AddWithValue("@username", username);

            try
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                exists = count > 0; // Пользователь существует
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Закрываем соединение
                database.closeConnection();
            }

            return exists;
        }
    }
}

