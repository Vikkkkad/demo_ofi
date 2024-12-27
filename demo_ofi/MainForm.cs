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
    public partial class MainForm : Form
    {
        private DataBase database;
        private UserRole userRole;
        private List<Task> tasks = new List<Task>();
        public MainForm(UserRole role)
        {
            InitializeComponent();
            userRole = role;
            ConfigureUI();

            database = new DataBase();

            comboBoxPriority.Items.Add("Высокий");
            comboBoxPriority.Items.Add("Средний");
            comboBoxPriority.Items.Add("Низкий");

        }

        private void ConfigureUI()
        {
            switch (userRole)
            {
                case UserRole.Администратор:
                    lblRole.Text = "Вы вошли как Администратор";
                    btnAddTask.Visible = true;
                    btnEditTask.Visible = true;
                    btnDeleteTask.Visible = true;
                   
                    break;

                case UserRole.Руководитель:
                    lblRole.Text = "Вы вошли как Руководитель";
                    btnAddTask.Visible = true;
                    btnEditTask.Visible = true;
                    btnDeleteTask.Visible = false; // Только администратор может удалять
                    break;

                case UserRole.Сотрудник:
                    lblRole.Text = "Вы вошли как Сотрудник";
                    btnAddTask.Visible = false; // Сотрудник не может добавлять задачи
                    btnEditTask.Visible = true; // Может изменять статус своих задач
                    btnDeleteTask.Visible = false; // Сотрудник не может удалять задачи
                    break;
            }
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtTaskDescription.Text) || string.IsNullOrWhiteSpace(txtProjectName.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Task newTask = new Task
            {
                TaskId = tasks.Count + 1, // Можно изменить на автоинкремент в БД
                CreationDate = DateTime.Now,
                ProjectName = txtProjectName.Text,
                Description = txtTaskDescription.Text,
                Priority = comboBoxPriority.SelectedItem.ToString(),
                AssignedTo = userRole.ToString(),
                Status = "В ожидании"
            };

            tasks.Add(newTask);
            // Обновляем ListBox, добавляя дату создания
            lstTasks.Items.Add($"{newTask.TaskId} - {newTask.CreationDate.ToShortDateString()} - {newTask.Description}");

            // Обновляем Label для отображения даты создания
            CreationDate.Text = $"Дата создания: {newTask.CreationDate.ToShortDateString()}";
            CreationDate.Visible = true; // Делаем Label видимым

            if (SaveTaskToDatabase(newTask))
            {
                MessageBox.Show("Задача успешно добавлена в базу данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении задачи в базу данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            txtTaskDescription.Clear();
            txtProjectName.Clear();
            comboBoxPriority.SelectedIndex = -1;
        }
        

        private bool SaveTaskToDatabase(Task task)
        {
            bool isSaved = false;

            database.openConnection();

            string query = "INSERT INTO Tasks (TaskId, CreationDate, Description, Status, AssignedTo) VALUES (@taskId, @creationDate, @description, @status, @assignedTo)";
            using (SqlCommand command = new SqlCommand(query, database.getConnection()))
            {
                command.Parameters.AddWithValue("@taskId", task.TaskId);
                command.Parameters.AddWithValue("@creationDate", task.CreationDate); // Убедитесь, что это поле есть в вашей таблице
                command.Parameters.AddWithValue("@description", task.Description);
                command.Parameters.AddWithValue("@status", task.Status);
                command.Parameters.AddWithValue("@assignedTo", task.AssignedTo);

                try
                {
                    command.ExecuteNonQuery();
                    isSaved = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    database.closeConnection();
                }
            }

            return isSaved;
    
        }

        private bool UpdateTaskInDatabase(Task task)
        {
            bool isUpdated = false;

            database.openConnection();

            string query = "UPDATE Tasks SET Status = @status WHERE Description = @description AND AssignedTo = @assignedTo";
            using (SqlCommand command = new SqlCommand(query, database.getConnection()))
            {
                command.Parameters.AddWithValue("@description", task.Description);
                command.Parameters.AddWithValue("@status", task.Status);
                command.Parameters.AddWithValue("@assignedTo", task.AssignedTo);

                // Добавьте отладочные сообщения
                Console.WriteLine($"Updating task: {task.Description}, Status: {task.Status}, AssignedTo: {task.AssignedTo}");

                try
                {
                    command.ExecuteNonQuery();
                    isUpdated = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    database.closeConnection();
                }
            }

            return isUpdated;
        }



        private void btnEditTask_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedItem != null)
            {
                Task selectedTask = tasks.First(t => t.Description == lstTasks.SelectedItem.ToString().Split('-')[2].Trim());

                EditTaskForm editForm = new EditTaskForm(selectedTask);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    selectedTask.Priority = editForm.Priority;
                    selectedTask.AssignedTo = editForm.AssignedTo;
                    selectedTask.Status = editForm.Status;
                    selectedTask.Description = editForm.Description;

                    // Обновляем элемент в ListBox, включая дату создания
                    lstTasks.Items[lstTasks.SelectedIndex] = $"{selectedTask.TaskId} - {selectedTask.CreationDate.ToShortDateString()} - {selectedTask.Description} - {selectedTask.Status}";

                    CreationDate.Text = $"Дата создания: {selectedTask.CreationDate.ToShortDateString()}";

                    if (UpdateTaskInDatabase(selectedTask))
                    {
                        MessageBox.Show("Задача успешно обновлена в базе данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении задачи в базе данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        

       
        private bool DeleteTaskFromDatabase(Task task)
        {
            bool isDeleted = false;

            database.openConnection();

            string query = "DELETE FROM Tasks WHERE Description = @description AND AssignedTo = @assignedTo";
            using (SqlCommand command = new SqlCommand(query, database.getConnection()))
            {
                command.Parameters.AddWithValue("@description", task.Description);
                command.Parameters.AddWithValue("@assignedTo", task.AssignedTo);

                try
                {
                    command.ExecuteNonQuery();
                    isDeleted = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    database.closeConnection();
                }
            }

            return isDeleted;
        }


        private void btnDeleteTask_Click(object sender, EventArgs e)
        {

            if (lstTasks.SelectedItem != null)
            {
                // Извлекаем описание задачи из выбранного элемента
                string selectedItem = lstTasks.SelectedItem.ToString();
                string description = selectedItem.Split('-')[2].Trim(); // Извлекаем описание

                // Находим задачу в списке
                Task selectedTask = tasks.FirstOrDefault(t => t.Description == description);

                if (selectedTask != null)
                {
                    // Удаляем задачу из базы данных
                    if (DeleteTaskFromDatabase(selectedTask))
                    {
                        tasks.Remove(selectedTask); // Удаляем задачу из списка
                        lstTasks.Items.Remove(lstTasks.SelectedItem);
                        MessageBox.Show("Задача успешно удалена из базы данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении задачи из базы данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Задача не найдена в списке.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите задачу для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        


    }
}


