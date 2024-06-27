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
using static System.Net.Mime.MediaTypeNames;

namespace TaskMenager
{
    public partial class CommandLine : Form
    {
        private const string DataFilePath = "combobox_data.txt";
        private ComboBox _comboBoxFileName;
        public ComboBox ComboBoxFileName
        {
            get
            {
                return _comboBoxFileName;
            }
        }
        public CommandLine()
        {
            InitializeComponent();
            LoadComboBoxData();
            _comboBoxFileName = this.comboBoxFileName;
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                string newText = comboBoxFileName.Text;
                if (!comboBoxFileName.Items.Contains(newText))//Проверка на дубликат
                {
                    comboBoxFileName.Items.Insert(0, newText);
                    SaveComboBoxData();
                }
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(newText);
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = startInfo;
                process.Start();
                comboBoxFileName.Items.Remove(newText);
                comboBoxFileName.Text = (newText);
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void SaveComboBoxData()
        {
            StreamWriter sw = new StreamWriter(DataFilePath);
            //Сохраняем все элементы в обратном порядке, чтобы при загрузке они были в нужном порядке
            for (int i = comboBoxFileName.Items.Count - 1; i >= 0; i--)
            {
                sw.WriteLine(comboBoxFileName.Items[i]);
            }
            sw.Close();
        }
        private void LoadComboBoxData()
        {
            try
            {
                StreamReader sr = new StreamReader("combobox_data.txt");
                while (!sr.EndOfStream)
                {
                    string item = sr.ReadLine();
                    comboBoxFileName.Items.Insert(0, item);
                }
                sr.Close();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                comboBoxFileName.Text = filePath;
            }
        }
        private void CommandLine_FormClosing(object sender, FormClosingEventArgs e)
        {
            comboBoxFileName.Focus();
        }
        private void comboBoxFileName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Enter)
            {
                buttonOK_Click(sender,e);
            }
        }
        private void CommandLine_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                Close();
            }
        }
    }
}