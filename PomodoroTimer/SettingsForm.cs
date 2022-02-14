using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PomodoroTimer
{
    public partial class SettingsForm : Form
    {
        BindingList<SprintItem> bindingList = new BindingList<SprintItem>(Settings.SprintItems);

        public SettingsForm()
        {
            InitializeComponent();

            SprintItems.DataSource = new BindingSource(bindingList, null);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.Save();

            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bindingList.Add(new SprintItem(0, 0));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (SprintItems.SelectedRows.Count != 0)
            {
                int id = (int) SprintItems.SelectedRows[0].Cells["Id"].Value;

                var itemToRemove = Settings.SprintItems.Find(item => item.Id == id);

                bindingList.Remove(itemToRemove);
            }
        }
    }
}
