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
    public class indexedValues
    {
        public string[] indexedText = new string [8];
        public string newFilename;

        public void setEmptyValues()
        {
            for (int i = 0; i < 8; i++)
                this.indexedText[i] = "";
        }
    }
    /// <summary>
    /// Main class
    /// </summary>
    public partial class imageIndexerMainWindow : Form
    {
        private Image imgOriginal { get; set; }
        private string folder_location { get; set; }
        private string[] fileList { get; set; }
        private int currentIndex { get; set; }

        private indexedValues[] indexedText;

        static List<string> validList;

        private int indexFieldsCount = 1;
        public imageIndexerMainWindow()
        {
            InitializeComponent();
            this.versionLabel.Text = "Version: " + typeof(imageIndexerMainWindow).Assembly.GetName().Version;
            lockIncompleteFunctions();
        }

        private void lockIncompleteFunctions()
        {
            this.rotateClockwiseToolStripMenuItem.Enabled = false;
            this.rotateCounterwiseToolStripMenuItem.Enabled = false;
            this.loadConfigFileToolStripMenuItem.Enabled = false;
            this.helpToolStripMenuItem.Enabled = false;
            this.aboutToolStripMenuItem.Enabled = false;
            this.nextPageToolStripMenuItem.Enabled = false;
            this.previousPageToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// This method checks if the file extension is either jpg jpeg tiff tif or png
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool check_if_valid_extension(string file)
        {
            if (file.Contains(".jpg") == true ||
               file.Contains(".jpeg") == true ||
               file.Contains(".tiff") == true ||
               file.Contains(".tif") == true ||
               file.Contains(".png")==true)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method loads the image on the screen and sets its zoom to fit on the screen
        /// </summary>
        /// <param name="indexNumber">position in the validList List</param>
        /// <returns>true for success, false for fail - fail due to going over the range or file exception</returns>
        private bool loadImage(int indexNumber)
        {
            if (indexNumber < 0)
            {
                this.currentIndex = 0;
                return false;
            }
            if (indexNumber < validList.Count)
            {
                try
                {
                    if(this.pictureBox1.Image != null)
                        this.pictureBox1.Image.Dispose();
                    this.imgOriginal = Image.FromFile(validList[indexNumber]);
                    this.pictureBox1.Image = imgOriginal;
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    // reduce flickering
                    this.DoubleBuffered = true;
                    fitTheImage();
                    this.fileListBox.SelectedItem = validList[indexNumber];
                    this.indexedText = new indexedValues[validList.Count];
                    for (int i = 0; i < validList.Count; i++)
                        this.indexedText[i].setEmptyValues();
                    loadMetadata(this.currentIndex);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error 0x1001 - Error while opening file. File is potentially corrupted. \n\n" + e.ToString(),"Error!");
                    this.pictureBox1.Image.Dispose();
                    return false;
                }
            }
            else
            {
                this.currentIndex = validList.Count - 1;
                MessageBox.Show("No more valid files to open", "File opening");
                return false;
            }

        }



        private void loadMetadata(int currentIndex)
        {
            this.indexField1.Text = this.indexedText[currentIndex].indexedText[0];
            this.indexField2.Text = this.indexedText[currentIndex].indexedText[1];
            this.indexField3.Text = this.indexedText[currentIndex].indexedText[2];
            this.indexField4.Text = this.indexedText[currentIndex].indexedText[3];
            this.indexField5.Text = this.indexedText[currentIndex].indexedText[4];
            this.indexField6.Text = this.indexedText[currentIndex].indexedText[5];
            this.indexField7.Text = this.indexedText[currentIndex].indexedText[6];
            this.indexField8.Text = this.indexedText[currentIndex].indexedText[7];
        }

        #region loadingFolder
        /// <summary>
        /// This method allows to select the folder with the images loads up images into the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectTheFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.folder_location = fbd.SelectedPath;
                    this.fileList = Directory.GetFiles(fbd.SelectedPath);
                    validList = new List<string>();
                    this.folder_location = fbd.SelectedPath;

                    
                    for (int i = 0; i < this.fileList.Length; i++)
                    {
                        Console.WriteLine(this.fileList[i]);
                    }
                    if (this.fileList.Length > 0)
                    {
                        for (int i=0; i< this.fileList.Length;i++)
                        {
                            if (check_if_valid_extension(this.fileList[i]) == true)
                                validList.Add(this.fileList[i]);
                        }
                        MessageBox.Show("Found: " + validList.Count.ToString()+ " image files", "Opening the files");
                        this.currentIndex = 0;
                        if (loadImage(this.currentIndex) == true)
                        {
                            load_images_list();
                            this.fileListBox.SelectedItem = validList[this.currentIndex];
                        }
                    }
                }
            }
        }

        private void load_images_list()
        {
            for (int i = 0; i < validList.Count; i++)
            {
                this.fileListBox.Items.Add(validList[i]);
            }

        }

        #endregion

        #region Documents control methods
        /// <summary>
        /// This method allows to select the image from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.currentIndex = this.fileListBox.SelectedIndex;
            loadImage(currentIndex);
            
        }

        private void nextDocumentButton_Click(object sender, EventArgs e)
        {
            load_next_document();
        }

        private void previousDocumentButton_Click(object sender, EventArgs e)
        {
            load_previous_document();
        }

        private void nextDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            load_next_document();
        }

        private void previousDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            load_previous_document();
        }

        private void load_previous_document()
        {
            this.currentIndex -= 1;
            //this.pictureBox1.Image = null;
            loadImage(this.currentIndex);
        }

        private void load_next_document()
        {
            this.currentIndex += 1;
            //this.pictureBox1.Image = null;
            loadImage(this.currentIndex);
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            //Field1
            if(this.indexField1.TextLength>0)
                this.indexedText[this.currentIndex].indexedText[0] = this.indexField1.Text;
            else
            {
                MessageBox.Show("Index field 1 cannot be left empty as it is a required field", "Validation error");
                return;
            }
            //Field2
            if (this.requiredBox2.Checked==true & this.indexField2.TextLength==0)
            {
                MessageBox.Show("Index field 2 cannot be left empty as it is a required field", "Validation error");
                return;
            }
            else
                this.indexedText[this.currentIndex].indexedText[1] = this.indexField2.Text;
            //Field3
            if (this.requiredBox3.Checked == true & this.indexField3.TextLength == 0)
            {
                MessageBox.Show("Index field 3 cannot be left empty as it is a required field", "Validation error");
                return;
            }
            else
                this.indexedText[this.currentIndex].indexedText[2] = this.indexField3.Text;
            //Field4
            if (this.requiredBox4.Checked == true & this.indexField4.TextLength == 0)
            {
                MessageBox.Show("Index field 4 cannot be left empty as it is a required field", "Validation error");
                return;
            }
            else
                this.indexedText[this.currentIndex].indexedText[3] = this.indexField4.Text;
            //Field5
            if (this.requiredBox5.Checked == true & this.indexField5.TextLength == 0)
            {
                MessageBox.Show("Index field 5 cannot be left empty as it is a required field", "Validation error");
                return;
            }
            else
                this.indexedText[this.currentIndex].indexedText[4] = this.indexField5.Text;
            //Field6
            if (this.requiredBox6.Checked == true & this.indexField6.TextLength == 0)
            {
                MessageBox.Show("Index field 6 cannot be left empty as it is a required field", "Validation error");
                return;
            }
            else
                this.indexedText[this.currentIndex].indexedText[5] = this.indexField6.Text;
            //Field7
            if (this.requiredBox7.Checked == true & this.indexField7.TextLength == 0)
            {
                MessageBox.Show("Index field 7 cannot be left empty as it is a required field", "Validation error");
                return;
            }
            else
                this.indexedText[this.currentIndex].indexedText[6] = this.indexField7.Text;
            //Field8
            if (this.requiredBox7.Checked == true & this.indexField7.TextLength == 0)
            {
                MessageBox.Show("Index field 8 cannot be left empty as it is a required field", "Validation error");
                return;
            }
            else
                this.indexedText[this.currentIndex].indexedText[7] = this.indexField8.Text;

            //Creating a new filename

            this.indexedText[this.currentIndex].newFilename = this.indexField1.Text;
            if(this.filenameBox2.Checked == true)
                this.indexedText[this.currentIndex].newFilename += ("-" + this.indexField2.Text);
            if(this.filenameBox3.Checked == true)
                this.indexedText[this.currentIndex].newFilename += ("-" + this.indexField3.Text);
            if (this.filenameBox4.Checked == true)
                this.indexedText[this.currentIndex].newFilename += ("-" + this.indexField4.Text);
            if (this.filenameBox5.Checked == true)
                this.indexedText[this.currentIndex].newFilename += ("-" + this.indexField5.Text);
            if (this.filenameBox6.Checked == true)
                this.indexedText[this.currentIndex].newFilename += ("-" + this.indexField6.Text);
            if (this.filenameBox7.Checked == true)
                this.indexedText[this.currentIndex].newFilename += ("-" + this.indexField7.Text);
            if (this.filenameBox8.Checked == true)
                this.indexedText[this.currentIndex].newFilename += ("-" + this.indexField8.Text);

            return;
        }
        #endregion

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
            this.pictureBox1.Location = new Point(this.pictureBox1.Location.X, this.pictureBox1.Location.Y + 70);
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
                return;
            this.pictureBox1.Location = new Point(this.pictureBox1.Location.X - 70, this.pictureBox1.Location.Y);

        }
        private void leftButton_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
                return;
            this.pictureBox1.Location = new Point(this.pictureBox1.Location.X + 70, this.pictureBox1.Location.Y);
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image == null)
                return;
            this.pictureBox1.Location = new Point(this.pictureBox1.Location.X, this.pictureBox1.Location.Y - 70);
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
            this.stickyBox1.Enabled = true;
            this.fieldnameField1.ReadOnly = false;
            if (this.enableBox2.Checked == true)
            {
                this.fieldnameField2.ReadOnly = false;
                this.indexField2.ReadOnly = false;
                this.stickyBox2.Enabled = true;
                this.requiredBox2.Enabled = true;
                this.filenameBox2.Enabled = true;
                this.indexFieldsCount = 2;
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
                this.indexFieldsCount = 1;
            }
            if (this.enableBox3.Checked == true &
                this.enableBox2.Checked == true)
            {
                this.fieldnameField3.ReadOnly = false;
                this.indexField3.ReadOnly = false;
                this.stickyBox3.Enabled = true;
                this.requiredBox3.Enabled = true;
                this.filenameBox3.Enabled = true;
                this.indexFieldsCount = 3;
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
                this.indexFieldsCount = 2;
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
                this.indexFieldsCount = 4;
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
                this.indexFieldsCount = 3;
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
                this.indexFieldsCount = 5;
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
                this.indexFieldsCount = 4;
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
                this.indexFieldsCount = 6;
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
                this.indexFieldsCount = 5;
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
                this.indexFieldsCount = 7;
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
                this.indexFieldsCount = 6;
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
                this.indexFieldsCount = 8;
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
                this.indexFieldsCount = 7;
            }

        }

        private void propertiesLockButton_Click(object sender, EventArgs e)
        {
            this.requiredBox1.Enabled = false;
            this.stickyBox1.Enabled = false;
            this.filenameBox1.Enabled = false;
            this.fieldnameField1.ReadOnly = true;

            this.requiredBox2.Enabled = false;
            this.stickyBox2.Enabled = false;
            this.filenameBox2.Enabled = false;
            this.fieldnameField2.ReadOnly = true;

            this.requiredBox3.Enabled = false;
            this.stickyBox3.Enabled = false;
            this.filenameBox3.Enabled = false;
            this.fieldnameField3.ReadOnly = true;

            this.requiredBox4.Enabled = false;
            this.stickyBox4.Enabled = false;
            this.filenameBox4.Enabled = false;
            this.fieldnameField4.ReadOnly = true;

            this.requiredBox5.Enabled = false;
            this.stickyBox5.Enabled = false;
            this.filenameBox5.Enabled = false;
            this.fieldnameField5.ReadOnly = true;

            this.requiredBox6.Enabled = false;
            this.stickyBox6.Enabled = false;
            this.filenameBox6.Enabled = false;
            this.fieldnameField6.ReadOnly = true;

            this.requiredBox7.Enabled = false;
            this.stickyBox7.Enabled = false;
            this.filenameBox7.Enabled = false;
            this.fieldnameField7.ReadOnly = true;

            this.requiredBox8.Enabled = false;
            this.stickyBox8.Enabled = false;
            this.filenameBox8.Enabled = false;
            this.fieldnameField8.ReadOnly = true;
        }


        #endregion

    }
}
