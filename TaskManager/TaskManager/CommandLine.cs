using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskMenager
{
    public partial class CommandLine : Form
    {
        private const string DataFilePath = "combobox_data.txt";
        public ComboBox ComboBoxFileName
        {
            get
            {
                return ComboBoxFileName;
            }
        }
        public CommandLine()
        {
            InitializeComponent();
            LoadComboBoxData();
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                string newText = comboBoxFileName.Text;
                //if (!string.IsNullOrEmpty(newText))
                //{
                //    SaveComboBoxData();
                //}
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(comboBoxFileName.Text);
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = startInfo;
                process.Start();
                // comboBoxFIleName.Items.Insert(0,comboBoxFIleName.Text);
                comboBoxFileName.Items.Remove(newText);
                comboBoxFileName.Text = (newText);
                comboBoxFileName.Items.Insert(0, newText);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveComboBoxData()
        {
            using (StreamWriter writer = new StreamWriter(DataFilePath))
            {
                foreach (string item in comboBoxFileName.Items)
                {
                    writer.WriteLine(item);
                }
            }
        }
        private void LoadComboBoxData()
        {
            if (File.Exists(DataFilePath))
            {
                using (StreamReader reader = new StreamReader(DataFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        comboBoxFileName.Items.Add(line);
                    }
                }
            }
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Все файлы (.)|.";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                comboBoxFileName.Text = filePath;
            }
        }
        private void comboBoxFIleName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                buttonOK_Click(sender, e);
            }
        }
        private void CommandLine_FormClosing(object sender, FormClosingEventArgs e)
        {
            comboBoxFileName.Focus();
        }
    }
}