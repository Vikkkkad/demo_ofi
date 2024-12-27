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
    public partial class Form1 : Form
    {
        private DataBase database;

        public Form1()
        {
            InitializeComponent();
            database = new DataBase();

            comboBox1.Items.Add("Администратор");
            comboBox1.Items.Add("Руководитель");
            comboBox1.Items.Add("Сотрудник");
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Regis regis = new Regis();
            regis.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            UserRole role = AuthenticateUser(username, password);

            if (role != UserRole.None)
            {
                MainForm mainForm = new MainForm(role);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверные учетные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private UserRole AuthenticateUser(string username, string password)
        {
            UserRole role = UserRole.None;
            database.openConnection();

            string query = "SELECT UserRole FROM Users WHERE Username = @username AND Password = @password";
            using (SqlCommand command = new SqlCommand(query, database.getConnection()))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string userRole = reader["UserRole"].ToString();
                    role = (UserRole)Enum.Parse(typeof(UserRole), userRole);
                }
            }

            database.closeConnection(); 
            return role; 
        }

       
    }

    public enum UserRole
    {
        None,
        Администратор,
        Сотрудник,
        Руководитель
    }
}