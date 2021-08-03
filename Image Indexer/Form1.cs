using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace Image_indexer
{
    public partial class imageIndexerMainWindow : Form
    {
        private Image imgOriginal { get; set; }
        private string folder_location { get; set; }

        private string[] fileList { get; set; }
        private int currentIndex { get; set; }

        public imageIndexerMainWindow()
        {
            InitializeComponent();
        }

        private bool load_image(int indexNumber)
        {
            if (indexNumber < this.fileList.Length)
                if (this.fileList[indexNumber].Contains(".jpg") == true ||
                this.fileList[indexNumber].Contains(".jpeg") == true ||
                this.fileList[indexNumber].Contains(".tiff") == true ||
                this.fileList[indexNumber].Contains(".tif") == true)
                {
                    this.imgOriginal = Image.FromFile(this.fileList[indexNumber]);
                    this.pictureBox1.Image = imgOriginal;
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    // reduce flickering
                    this.DoubleBuffered = true;
                    fitTheImage();
                    return true;
                }
                else
                {
                    this.currentIndex += 1;
                    load_image(currentIndex);
                    return false;
                }
            else
            {
                MessageBox.Show("No valid files to open", "File opening error");
                return false;
            }

        }





        private void selectTheFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.folder_location = fbd.SelectedPath;
                    this.fileList = Directory.GetFiles(fbd.SelectedPath);

                    MessageBox.Show(this.fileList.Length.ToString());

                    this.folder_location = fbd.SelectedPath;

                    MessageBox.Show("Files found: " + this.fileList.Length.ToString(), "Message");
                    for (int i = 0; i < this.fileList.Length; i++)
                    {
                        Console.WriteLine(this.fileList[i]);
                    }
                    if (this.fileList.Length > 0)
                    {
                        this.currentIndex = 0;
                        if (load_image(this.currentIndex) == true)
                        {
                            load_images_list();
                        }

                    }
                }
            }
        }

        private void load_images_list()
        {
            for (int i = 0; i < this.fileList.Length; i++)
            {
                this.fileListView.Items.Add(this.fileList[i]);
            }

        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            return;
        }

        private void nextDocumentButton_Click(object sender, EventArgs e)
        {

        }

        private void previousDocumentButton_Click(object sender, EventArgs e)
        {

        }


        #region Image Control Buttons
        private void fitImageButton_Click(object sender, EventArgs e)
        {
            fitTheImage();
        }

        private void fitTheImage()
        {
            this.pictureBox1.Location = new System.Drawing.Point(6, 19);
            this.pictureBox1.Size = new System.Drawing.Size(600, 800);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
                return;
            this.pictureBox1.Location = new Point(this.pictureBox1.Location.X, this.pictureBox1.Location.Y + 25);
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
                return;
            this.pictureBox1.Location = new Point(this.pictureBox1.Location.X - 25, this.pictureBox1.Location.Y);

        }
        private void leftButton_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
                return;
            this.pictureBox1.Location = new Point(this.pictureBox1.Location.X + 25, this.pictureBox1.Location.Y);
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
                return;
            this.pictureBox1.Location = new Point(this.pictureBox1.Location.X, this.pictureBox1.Location.Y - 25);
        }

        private void pictureBox1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.pictureBox1.Image == null)
            {
                return;
            }
            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = pictureBox1.Width;
            int oh = pictureBox1.Height;
            int VX, VY;
            int zoomStep = 100;
            if (e.Delta > 0)
            {
                this.pictureBox1.Width += zoomStep;
                this.pictureBox1.Height += zoomStep;

                System.Reflection.PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);

                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }
            if (e.Delta < 0) // shrink
            {
                if (this.pictureBox1.Width < this.imgOriginal.Width / 10)
                    return;

                pictureBox1.Width -= zoomStep;
                pictureBox1.Height -= zoomStep;
                System.Reflection.PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", System.Reflection.BindingFlags.Instance |
                 System.Reflection.BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);
                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }
            //Step 4: calculate the displacement caused by scaling, compensate and realize the effect of anchor scaling
            VX = (int)((double)x * (ow - pictureBox1.Width) / ow);
            VY = (int)((double)y * (oh - pictureBox1.Height) / oh);
            pictureBox1.Location = new Point(pictureBox1.Location.X + VX, pictureBox1.Location.Y + VY);

        }

        #endregion



        #region Applying settings for validation / indexing
        /// <summary>
        /// This method refreshes the indexing fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyButton_Click(object sender, EventArgs e)
        {
            if (this.enableBox2.Checked == true)
            {
                this.fieldnameField2.ReadOnly = false;
                this.indexField2.ReadOnly = false;
                this.stickyBox2.Enabled = true;
                this.requiredBox2.Enabled = true;
                this.filenameBox2.Enabled = true;
            }
            else
            {
                this.fieldnameField2.ReadOnly = true;
                this.fieldnameField2.Text = string.Empty;
                this.indexField2.ReadOnly = true;
                this.indexField2.Text = string.Empty;
                this.stickyBox2.Enabled = false;
                this.stickyBox2.Checked = false;
                this.requiredBox2.Enabled = false;
                this.requiredBox2.Checked = false;
                this.filenameBox2.Enabled = false;
                this.filenameBox2.Checked = false;
            }
            if (this.enableBox3.Checked == true &
                this.enableBox2.Checked == true)
            {
                this.fieldnameField3.ReadOnly = false;
                this.indexField3.ReadOnly = false;
                this.stickyBox3.Enabled = true;
                this.requiredBox3.Enabled = true;
                this.filenameBox3.Enabled = true;
            }
            else
            {
                this.fieldnameField3.ReadOnly = true;
                this.fieldnameField3.Text = string.Empty;
                this.indexField3.ReadOnly = true;
                this.indexField3.Text = string.Empty;
                this.stickyBox3.Enabled = false;
                this.stickyBox3.Checked = false;
                this.requiredBox3.Enabled = false;
                this.requiredBox3.Checked = false;
                this.filenameBox3.Enabled = false;
                this.filenameBox3.Checked = false;
            }
            if (this.enableBox4.Checked == true &
                this.enableBox3.Checked == true &
                this.enableBox2.Checked == true)
            {
                this.fieldnameField4.ReadOnly = false;
                this.indexField4.ReadOnly = false;
                this.stickyBox4.Enabled = true;
                this.requiredBox4.Enabled = true;
                this.filenameBox4.Enabled = true;
            }
            else
            {
                this.fieldnameField4.ReadOnly = true;
                this.fieldnameField4.Text = string.Empty;
                this.indexField4.ReadOnly = true;
                this.indexField4.Text = string.Empty;
                this.stickyBox4.Enabled = false;
                this.stickyBox4.Checked = false;
                this.requiredBox4.Enabled = false;
                this.requiredBox4.Checked = false;
                this.filenameBox4.Enabled = false;
                this.filenameBox4.Checked = false;
            }
            if (this.enableBox5.Checked == true &
                this.enableBox4.Checked == true &
                this.enableBox3.Checked == true &
                this.enableBox2.Checked == true)
            {
                this.fieldnameField5.ReadOnly = false;
                this.indexField5.ReadOnly = false;
                this.stickyBox5.Enabled = true;
                this.requiredBox5.Enabled = true;
                this.filenameBox5.Enabled = true;
            }
            else
            {
                this.fieldnameField5.ReadOnly = true;
                this.fieldnameField5.Text = string.Empty;
                this.indexField5.ReadOnly = true;
                this.indexField5.Text = string.Empty;
                this.stickyBox5.Enabled = false;
                this.stickyBox5.Checked = false;
                this.requiredBox5.Enabled = false;
                this.requiredBox5.Checked = false;
                this.filenameBox5.Enabled = false;
                this.filenameBox5.Checked = false;
            }
            if (this.enableBox6.Checked == true &
                this.enableBox5.Checked == true &
                this.enableBox4.Checked == true &
                this.enableBox3.Checked == true &
                this.enableBox2.Checked == true)
            {
                this.fieldnameField6.ReadOnly = false;
                this.indexField6.ReadOnly = false;
                this.stickyBox6.Enabled = true;
                this.requiredBox6.Enabled = true;
                this.filenameBox6.Enabled = true;
            }
            else
            {
                this.fieldnameField6.ReadOnly = true;
                this.fieldnameField6.Text = string.Empty;
                this.indexField6.ReadOnly = true;
                this.indexField6.Text = string.Empty;
                this.stickyBox6.Enabled = false;
                this.stickyBox6.Checked = false;
                this.requiredBox6.Enabled = false;
                this.requiredBox6.Checked = false;
                this.filenameBox6.Enabled = false;
                this.filenameBox6.Checked = false;
            }
            if (this.enableBox7.Checked == true &
                this.enableBox6.Checked == true &
                this.enableBox5.Checked == true &
                this.enableBox4.Checked == true &
                this.enableBox3.Checked == true &
                this.enableBox2.Checked == true)
            {
                this.fieldnameField7.ReadOnly = false;
                this.indexField7.ReadOnly = false;
                this.stickyBox7.Enabled = true;
                this.requiredBox7.Enabled = true;
                this.filenameBox7.Enabled = true;
            }
            else
            {
                this.fieldnameField7.ReadOnly = true;
                this.fieldnameField7.Text = string.Empty;
                this.indexField7.ReadOnly = true;
                this.indexField7.Text = string.Empty;
                this.stickyBox7.Enabled = false;
                this.stickyBox7.Checked = false;
                this.requiredBox7.Enabled = false;
                this.requiredBox7.Checked = false;
                this.filenameBox7.Enabled = false;
                this.filenameBox7.Checked = false;
            }
            if (this.enableBox8.Checked == true &
                this.enableBox7.Checked == true &
                this.enableBox6.Checked == true &
                this.enableBox5.Checked == true &
                this.enableBox4.Checked == true &
                this.enableBox3.Checked == true &
                this.enableBox2.Checked == true)
            {
                this.fieldnameField8.ReadOnly = false;
                this.indexField8.ReadOnly = false;
                this.requiredBox8.Enabled = true;
                this.stickyBox8.Enabled = true;
                this.filenameBox8.Enabled = true;
            }
            else
            {
                this.fieldnameField8.ReadOnly = true;
                this.fieldnameField8.Text = string.Empty;
                this.indexField8.ReadOnly = true;
                this.indexField8.Text = string.Empty;
                this.stickyBox8.Enabled = false;
                this.stickyBox8.Checked = false;
                this.requiredBox8.Enabled = false;
                this.requiredBox8.Checked = false;
                this.filenameBox8.Enabled = false;
                this.filenameBox8.Checked = false;
            }

        }

        #endregion




    }
}
