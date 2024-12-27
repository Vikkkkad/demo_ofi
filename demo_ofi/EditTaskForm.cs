using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demo_ofi
{
    public partial class EditTaskForm : Form
    {
        public string Priority { get; private set; }
        public string AssignedTo { get; private set; }
        public string Status { get; private set; }
        public string Description { get; private set; }

        // Конструктор, принимающий задачу
        public EditTaskForm(Task task)
        {
            InitializeComponent();

            // Заполняем поля текущими значениями задачи
            txtDescription.Text = task.Description;
            comboBoxPriority.SelectedItem = task.Priority;
            
            comboBoxStatus.SelectedItem = task.Status;

            comboBoxPriority.Items.Add("Низкий");
            comboBoxPriority.Items.Add("Средний");
            comboBoxPriority.Items.Add("Высокий");

            comboBoxStatus.Items.Add("В ожидании");
            comboBoxStatus.Items.Add("В работе");
            comboBoxStatus.Items.Add("Выполнено");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Проверка на заполнение обязательных полей
            if (string.IsNullOrWhiteSpace(txtDescription.Text) || comboBoxPriority.SelectedItem == null || comboBoxStatus.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Сохраняем изменения
            Description = txtDescription.Text;
            Priority = comboBoxPriority.SelectedItem.ToString();
            Status = comboBoxStatus.SelectedItem.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
