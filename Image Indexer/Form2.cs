using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_indexer
{
    public partial class savingSettingsWindow : Form
    {
        public List<string> BasicData = new List<string>();
        public savingSettingsWindow()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {

            if (this.projectIDField.TextLength==0 || this.projectNameField.TextLength == 0)
            {
                MessageBox.Show("Both fields cannot be blank");
                return;
            }
            BasicData.Add(this.projectNameField.Text);
            BasicData.Add(this.projectIDField.Text);
            this.Close();
        }

        public List<string> getBasicData()
        {
            return BasicData;
        }
    }
}
