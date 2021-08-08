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
    /// <summary>
    /// Main class
    /// </summary>
    public partial class imageIndexerMainWindow : Form
    {
        private string binPath { get; set; }
        private Image imgOriginal { get; set; }
        private string folder_location { get; set; }
        private string[] fileList { get; set; }
        private int currentIndex { get; set; }

        private List<string[]> indexedValuesList = new List<string[]>();

        private List<string> newFileNames = new List<string>();

        private List<string> validList = new List<string>();

        private int indexFieldsCount = 1;

        private char[] FileNameIlligalCharacters = { '<', '>', ':', '\"', '/','\\','|','?','*'};

        public imageIndexerMainWindow()
        {
            InitializeComponent();
            if (System.Diagnostics.Debugger.IsAttached == true)
                this.versionLabel.Text = "Development version: " + typeof(imageIndexerMainWindow).Assembly.GetName().Version;
            else
                this.versionLabel.Text = "Version: " + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            lockIncompleteFunctions();
            currentIndex = -1;
        }

        private void lockIncompleteFunctions()
        {
            this.rotateClockwiseToolStripMenuItem.Enabled = false;
            this.rotateCounterwiseToolStripMenuItem.Enabled = false;
            this.saveConfigFileToolStripMenuItem.Enabled = false;
            this.helpToolStripMenuItem.Enabled = false;
            this.aboutToolStripMenuItem.Enabled = false;
            this.nextPageToolStripMenuItem1.Enabled = false;
            this.previousPageToolStripMenuItem1.Enabled = false;
            this.pdfBrowser1.Visible = false;
            this.enterLicenseKeyToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// This method checks if the file extension is either jpg jpeg tiff tif png or pdf (PDF will open in build in webbrowser
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool check_if_valid_extension(string file)
        {
            if (file.Contains(".jpg") == true ||
               file.Contains(".jpeg") == true ||
               file.Contains(".tiff") == true ||
               file.Contains(".tif") == true ||
               file.Contains(".png") == true ||
               file.Contains(".pdf") == true)
                return true;
            else
                return false;
        }

        private void reloadPictureBox()
        {
            this.pictureBox1.Location = new System.Drawing.Point(6, 11);
            this.pictureBox1.Size = new System.Drawing.Size(867, 815);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.Image = Properties.Resources.imageIndexer;
            this.pdfBrowser1.Visible = false;
        }

        /// <summary>
        /// This method loads the image on the screen and sets its zoom to fit on the screen
        /// </summary>
        /// <param name="indexNumber">position in the validList List</param>
        /// <returns>true for success, false for fail - fail due to going over the range or file exception</returns>
        private bool loadImage(int indexNumber, bool ifValidateButtonUsed = false)
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

                    if (this.pictureBox1.Image != null)
                    {
                        this.pictureBox1.Image.Dispose();
                        reloadPictureBox();
                    }
                    if (validList[indexNumber].Contains(".pdf"))
                    {
                        this.pictureBox1.Visible = false;
                        this.pdfBrowser1.Visible = true;
                        this.pdfBrowser1.Navigate(validList[indexNumber]);
                        loadMetadata(this.currentIndex, ifValidateButtonUsed);
                        return true;
                    }
                    else
                    {
                        this.pdfBrowser1.DocumentText = "";
                        this.pdfBrowser1.Visible = false;
                        this.pictureBox1.Visible = true;
                        this.pictureBox1.Image = Properties.Resources.imageIndexer;
                        this.imgOriginal = Image.FromFile(validList[indexNumber]);
                        this.pictureBox1.Image = imgOriginal;
                        this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        // reduce flickering
                        this.DoubleBuffered = true;
                        fitTheImage();
                        this.fileListBox.SelectedItem = validList[indexNumber];

                        loadMetadata(this.currentIndex, ifValidateButtonUsed);
                        return true;
                    }

                }
                catch (Exception e)
                {
                    if (validList[indexNumber].Contains(".pdf"))
                    {
                        MessageBox.Show("Error 0x1002 - unable open the PDF file:\n" + validList[indexNumber] + "\n\n" + e.Message);
                        return false;
                    }
                    else
                    {
                        MessageBox.Show("Error 0x1001 - Error while opening file. File is potentially corrupted. \n\n" + e.ToString(), "Error!");
                        if (this.pictureBox1.Image != null)
                        {
                            this.pictureBox1.Image.Dispose();
                            reloadPictureBox();
                        }
                        return false;
                    }
                }
            }
            else
            {
                this.currentIndex = validList.Count - 1;
                MessageBox.Show("No more valid files to open", "File opening");
                return false;
            }

        }



        private void loadMetadata(int currentIndex, bool ifValidateButtonUsed)
        {
            //field1
            if (this.indexedValuesList[currentIndex][0] != null)
                if (ifValidateButtonUsed == true)
                {
                    if (this.stickyBox1.Checked == true & currentIndex > 0)
                        this.indexField1.Text = this.indexedValuesList[currentIndex - 1][0];
                    else
                        this.indexField1.Text = this.indexedValuesList[currentIndex][0];
                }
                else
                    this.indexField1.Text = this.indexedValuesList[currentIndex][0];
            //field2
            if (this.indexedValuesList[currentIndex][1] != null)
                if (ifValidateButtonUsed == true)
                {
                    if (this.stickyBox2.Checked == true & currentIndex > 0)
                        this.indexField2.Text = this.indexedValuesList[currentIndex - 1][1];
                    else
                        this.indexField2.Text = this.indexedValuesList[currentIndex][1];
                }
                else
                    this.indexField2.Text = this.indexedValuesList[currentIndex][1];
            //field3
            if (this.indexedValuesList[currentIndex][2] != null)
                if (ifValidateButtonUsed == true)
                {
                    if (this.stickyBox3.Checked == true & currentIndex > 0)
                        this.indexField3.Text = this.indexedValuesList[currentIndex - 1][2];
                    else
                        this.indexField3.Text = this.indexedValuesList[currentIndex][2];
                }
                else
                    this.indexField3.Text = this.indexedValuesList[currentIndex][2];
            //field4
            if (this.indexedValuesList[currentIndex][3] != null)
                if (ifValidateButtonUsed == true)
                {
                    if (this.stickyBox4.Checked == true & currentIndex > 0)
                        this.indexField4.Text = this.indexedValuesList[currentIndex - 1][3];
                    else
                        this.indexField4.Text = this.indexedValuesList[currentIndex][3];
                }
                else
                    this.indexField4.Text = this.indexedValuesList[currentIndex][3];
            //field5
            if (this.indexedValuesList[currentIndex][4] != null)
                if (ifValidateButtonUsed == true)
                {
                    if (this.stickyBox5.Checked == true & currentIndex > 0)
                        this.indexField5.Text = this.indexedValuesList[currentIndex - 1][4];
                    else
                        this.indexField5.Text = this.indexedValuesList[currentIndex][4];
                }
                else
                    this.indexField5.Text = this.indexedValuesList[currentIndex][4];
            //field6
            if (this.indexedValuesList[currentIndex][5] != null)
                if (ifValidateButtonUsed == true)
                {
                    if (this.stickyBox6.Checked == true & currentIndex > 0)
                        this.indexField6.Text = this.indexedValuesList[currentIndex - 1][5];
                    else
                        this.indexField6.Text = this.indexedValuesList[currentIndex][5];
                }
                else
                    this.indexField6.Text = this.indexedValuesList[currentIndex][5];
            //field7
            if (this.indexedValuesList[currentIndex][6] != null)
                if (ifValidateButtonUsed == true)
                {
                    if (this.stickyBox7.Checked == true & currentIndex > 0)
                        this.indexField7.Text = this.indexedValuesList[currentIndex - 1][6];
                    else
                        this.indexField7.Text = this.indexedValuesList[currentIndex][6];
                }
                else
                    this.indexField7.Text = this.indexedValuesList[currentIndex][6];
            //field8
            if (this.indexedValuesList[currentIndex][7] != null)
                if (ifValidateButtonUsed == true)
                {
                    if (this.stickyBox8.Checked == true & currentIndex > 0)
                        this.indexField8.Text = this.indexedValuesList[currentIndex - 1][7];
                    else
                        this.indexField8.Text = this.indexedValuesList[currentIndex][7];
                }
                else
                    this.indexField8.Text = this.indexedValuesList[currentIndex][7];
        }

        #region loadingFolder
        /// <summary>
        /// This method allows to select the folder with the images loads up images into the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectTheFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.indexedValuesList.Clear();
            this.newFileNames.Clear();
            this.currentIndex = 0;
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
                        for (int i = 0; i < this.fileList.Length; i++)
                        {
                            if (check_if_valid_extension(this.fileList[i]) == true)
                                validList.Add(this.fileList[i]);
                        }
                        MessageBox.Show("Found: " + validList.Count.ToString() + " image files", "Opening the files");
                        this.currentIndex = 0;

                        for (int i = 0; i < validList.Count; i++)
                        {
                            this.indexedValuesList.Add(new string[8]);
                        }
                        for (int i = 0; i < indexedValuesList.Count; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                this.indexedValuesList[i][j] = "";
                            }
                        }

                        for (int i = 0; i < validList.Count; i++)
                            this.newFileNames.Add("");

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
            //clears previously added values
            this.fileListBox.Items.Clear();
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
            loadImage(this.currentIndex);
        }

        private void load_next_document()
        {
            this.currentIndex += 1;
            loadImage(this.currentIndex);
        }

        /// <summary>
        /// This method checks if the value used for filename contains illigal characters for windows filename
        /// </summary>
        /// <param name="value">string value of the index field</param>
        /// <param name="position">index field position to set focus back on that field</param>
        /// <returns>boolean true for found, false for not found</returns>
        private bool checkIfContainsIlligalCharacters(string value, int position,string fieldName)
        {
            for (int character = 0; character < this.FileNameIlligalCharacters.Length; character++)
            {
                if (value.Contains(this.FileNameIlligalCharacters[character]))
                {
                    MessageBox.Show($"Index field {position} - {fieldName} - lands in the filename\n\nIt cannot contain illigal characters such as: < > : \" / \\ | ? * ", "Validation error - illigal characters for filename");
                    focusOn(position);
                    return true ;
                }
            }
            return false;
        }

        /// <summary>
        /// This method displays error that field cannot be blank as it's required.
        /// </summary>
        /// <param name="position">To indicate which field it is related to</param>
        private void emptyFieldErrorMessage(int position, string fieldName)
        {
            MessageBox.Show($"Index field {position} - {fieldName} - cannot be left empty as it is a required field", "Validation error - empty field");
            focusOn(position);
            return;
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            if (this.currentIndex == -1 || this.pictureBox1.Image == null)
            {
                MessageBox.Show("No images loaded up", "Validation");
                return;
            }
            //Field1
            if (this.indexField1.TextLength > 0)
                this.indexedValuesList[this.currentIndex][0] = this.indexField1.Text;
            else
            {
                emptyFieldErrorMessage(1,this.fieldnameField1.Text);
                return;
            }
            //Field2
            if (this.requiredBox2.Checked == true & this.indexField2.TextLength == 0)
            {
                emptyFieldErrorMessage(2, this.fieldnameField2.Text);
                return;
            }
            else
            {
                if (this.filenameBox2.Checked == true)
                    if (checkIfContainsIlligalCharacters(this.indexField2.Text, 2, this.fieldnameField2.Text) == true)
                        return;
                this.indexedValuesList[this.currentIndex][1] = this.indexField2.Text;
            }
            //Field3
            if (this.requiredBox3.Checked == true & this.indexField3.TextLength == 0)
            {
                emptyFieldErrorMessage(3, this.fieldnameField3.Text);
                return;
            }
            else
            {
                if (this.filenameBox3.Checked == true)
                    if (checkIfContainsIlligalCharacters(this.indexField3.Text, 3, this.fieldnameField3.Text) == true)
                        return;
                this.indexedValuesList[this.currentIndex][2] = this.indexField3.Text;
            }
            //Field4
            if (this.requiredBox4.Checked == true & this.indexField4.TextLength == 0)
            {
                emptyFieldErrorMessage(4, this.fieldnameField4.Text);
                return;
            }
            else
            {
                if (this.filenameBox4.Checked == true)
                    if (checkIfContainsIlligalCharacters(this.indexField4.Text, 4, this.fieldnameField4.Text) == true)
                        return;
                this.indexedValuesList[this.currentIndex][3] = this.indexField4.Text;
            }
                
            //Field5
            if (this.requiredBox5.Checked == true & this.indexField5.TextLength == 0)
            {
                emptyFieldErrorMessage(5, this.fieldnameField5.Text);
                return;
            }
            else
            {
                if (this.filenameBox5.Checked == true)
                    if (checkIfContainsIlligalCharacters(this.indexField5.Text, 5, this.fieldnameField5.Text) == true)
                        return;
                this.indexedValuesList[this.currentIndex][4] = this.indexField5.Text;
            }
                
            //Field6
            if (this.requiredBox6.Checked == true & this.indexField6.TextLength == 0)
            {
                emptyFieldErrorMessage(6, this.fieldnameField6.Text);
                return;
            }
            else
            {
                if (this.filenameBox6.Checked == true)
                    if (checkIfContainsIlligalCharacters(this.indexField6.Text, 6, this.fieldnameField6.Text) == true)
                        return;
                this.indexedValuesList[this.currentIndex][5] = this.indexField6.Text;
            }
            //Field7
            if (this.requiredBox7.Checked == true & this.indexField7.TextLength == 0)
            {
                emptyFieldErrorMessage(7, this.fieldnameField7.Text);
                return;
            }
            else
            {
                if (this.filenameBox7.Checked == true)
                    if (checkIfContainsIlligalCharacters(this.indexField7.Text, 7, this.fieldnameField7.Text) == true)
                        return;
                this.indexedValuesList[this.currentIndex][6] = this.indexField7.Text;
            }
            //Field8
            if (this.requiredBox7.Checked == true & this.indexField7.TextLength == 0)
            {
                emptyFieldErrorMessage(8, this.fieldnameField8.Text);
                return;
            }
            else
            {
                if (this.filenameBox8.Checked == true)
                    if (checkIfContainsIlligalCharacters(this.indexField8.Text, 7, this.fieldnameField8.Text) == true)
                        return;
                this.indexedValuesList[this.currentIndex][7] = this.indexField8.Text;
            }
            //Creating a new filename if file renaming is checked
            if (this.filesRenamingBox.Checked == true)
                if (assignFileNames() == false)
                    return;


            this.currentIndex += 1;
            loadImage(this.currentIndex, true);
            focusOn(1);
            return;
        }

        /// <summary>
        /// This method creates a new filenames
        /// </summary>
        /// <returns>true for OK - false for missing data</returns>
        private bool assignFileNames()
        {
            this.newFileNames[this.currentIndex] = this.indexField1.Text;
            if (this.filenameBox2.Checked == true)
            {
                this.requiredBox2.Checked = true;
                if (this.indexField2.TextLength == 0)
                {
                    emptyFieldErrorMessage(2, this.fieldnameField2.Text);
                    return false;
                }
                this.newFileNames[this.currentIndex] += ("-" + this.indexField2.Text);
            }

            if (this.filenameBox3.Checked == true) 
            {
                this.requiredBox3.Checked = true;
                if (this.indexField3.TextLength == 0)
                {
                    emptyFieldErrorMessage(3, this.fieldnameField3.Text);
                    return false;
                }
                this.newFileNames[this.currentIndex] += ("-" + this.indexField3.Text); 
            }

            if (this.filenameBox4.Checked == true)
            {
                this.requiredBox4.Checked = true;
                if (this.indexField4.TextLength == 0)
                {
                    emptyFieldErrorMessage(4, this.fieldnameField4.Text);
                    return false;
                }
                this.newFileNames[this.currentIndex] += ("-" + this.indexField4.Text);
            }

            if (this.filenameBox5.Checked == true)
            {
                this.requiredBox5.Checked = true;
                if (this.indexField5.TextLength == 0)
                {
                    emptyFieldErrorMessage(5, this.fieldnameField5.Text);
                    return false;
                }
                this.newFileNames[this.currentIndex] += ("-" + this.indexField5.Text);
            }

            if (this.filenameBox6.Checked == true)
            {
                this.requiredBox6.Checked = true;
                if (this.indexField6.TextLength == 0)
                {
                    emptyFieldErrorMessage(6, this.fieldnameField6.Text);
                    return false;
                }
                this.newFileNames[this.currentIndex] += ("-" + this.indexField6.Text);
            }
              
            if (this.filenameBox7.Checked == true)
            {
                this.requiredBox7.Checked = true;
                if (this.indexField7.TextLength == 0)
                {
                    emptyFieldErrorMessage(7, this.fieldnameField7.Text);
                    return false;
                }
                this.newFileNames[this.currentIndex] += ("-" + this.indexField7.Text);
            }
                    
            if (this.filenameBox8.Checked == true)
            {
                this.requiredBox8.Checked = true;
                if (this.indexField8.TextLength == 0)
                {
                    emptyFieldErrorMessage(8, this.fieldnameField8.Text);
                    return false;
                }
                this.newFileNames[this.currentIndex] += ("-" + this.indexField8.Text);
            }
                   
            return true;
        }
       

        private void focusOn(int fieldNumber)
        {
            if (fieldNumber == 1)
            {
                this.indexField1.Focus();
                this.indexField1.SelectionLength = 0;
            }
            if (fieldNumber == 2)
            {
                this.indexField2.Focus();
                this.indexField2.SelectionLength = 0;
            }
            if (fieldNumber == 3)
            {
                this.indexField3.Focus();
                this.indexField3.SelectionLength = 0;
            }
            if (fieldNumber == 4)
            {
                this.indexField4.Focus();
                this.indexField4.SelectionLength = 0;
            }
            if (fieldNumber == 5)
            {
                this.indexField5.Focus();
                this.indexField5.SelectionLength = 0;
            }
            if (fieldNumber == 6)
            {
                this.indexField6.Focus();
                this.indexField6.SelectionLength = 0;
            }
            if (fieldNumber == 7)
            {
                this.indexField7.Focus();
                this.indexField7.SelectionLength = 0;
            }
            if (fieldNumber == 8)
            {
                this.indexField8.Focus();
                this.indexField8.SelectionLength = 0;
            }

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
            if (this.pictureBox1.Image == null)
                return;
            this.pictureBox1.Location = new System.Drawing.Point(6, 11);
            this.pictureBox1.Size = new System.Drawing.Size(867, 815);
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
            int zoomStep = 200;
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
            refreshSettings();
        }

        private void refreshSettings()
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
            lockProperties();
        }

        private void lockProperties()
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

            focusOn(1);
        }

        #endregion

        #region Config loading and saving
        private void loadConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog settingsSelection = new OpenFileDialog();
            settingsSelection.Title = "Select BIN file with settings";
            settingsSelection.Filter = "BIN files (*.bin)|*.bin";
            settingsSelection.FilterIndex = 1;
            settingsSelection.RestoreDirectory = true;

            if(settingsSelection.ShowDialog() == DialogResult.OK)
            {
                this.binPath = settingsSelection.FileName;
            }
            else
            {
                MessageBox.Show("No bin file selected, please try again", "Settings file selection error");
                return;
            }

            List<string> settingsData = new List<string>();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(this.binPath);
                var size = lines.Length;
                for (int i = 0; i < size; i++)
                {
                    //splits the columns by comma
                    string[] columns = lines[i].Split(',');
                    foreach (string columnsItem in columns)
                    {
                        settingsData.Add(columnsItem);
                    }
                }
                
            }
            catch (Exception IOException)
            {
                MessageBox.Show("Unable to open the settings file\n" + IOException.ToString(), "Settings opening error");
                return;
            }

            if(settingsData.Count!=51)
            {
                MessageBox.Show("Error 0x1003 - Unable to open the settings file\n File is corrupted.", "Settings opening error");
                return;
            }
            settingItems loadedSettings = new settingItems(settingsData);
            applySettings(loadedSettings);
            lockProperties();

            }

        private void applySettings(settingItems settings)
        {
            if (settings.Title == null) 
            {
                MessageBox.Show("Settings file is corrupted");
                return; 
            }
            if (settings.FileRenaming == true)
                this.filesRenamingBox.Checked = true;
            else
                this.filesRenamingBox.Checked = false;

            refreshSettings();
            if (settings.Fields[0, 1] == "true")
            {
                this.enableBox1.Checked = true;
                this.fieldnameField1.Text = settings.Fields[0, 2];
                this.stickyBox1.Checked = Convert.ToBoolean(settings.Fields[0, 3]);
                this.requiredBox1.Checked = Convert.ToBoolean(settings.Fields[0, 4]);
                this.filenameBox1.Checked = Convert.ToBoolean(settings.Fields[0, 5]);
            }

            if (settings.Fields[1, 1] == "true")
            {
                this.enableBox2.Checked = true;
                this.fieldnameField2.Text = settings.Fields[1, 2];
                this.stickyBox2.Checked = Convert.ToBoolean(settings.Fields[1, 3]);
                this.requiredBox2.Checked = Convert.ToBoolean(settings.Fields[1, 4]);
                this.filenameBox2.Checked = Convert.ToBoolean(settings.Fields[1, 5]);
            }

            if (settings.Fields[2, 1] == "true")
            {
                this.enableBox3.Checked = true;
                this.fieldnameField3.Text = settings.Fields[2, 2];
                this.stickyBox3.Checked = Convert.ToBoolean(settings.Fields[2, 3]);
                this.requiredBox3.Checked = Convert.ToBoolean(settings.Fields[2, 4]);
                this.filenameBox3.Checked = Convert.ToBoolean(settings.Fields[2, 5]);
            }

            if (settings.Fields[3, 1] == "true")
            {
                this.enableBox4.Checked = true;
                this.fieldnameField4.Text = settings.Fields[3, 2];
                this.stickyBox4.Checked = Convert.ToBoolean(settings.Fields[3, 3]);
                this.requiredBox4.Checked = Convert.ToBoolean(settings.Fields[3, 4]);
                this.filenameBox4.Checked = Convert.ToBoolean(settings.Fields[3, 5]);
            }

            if (settings.Fields[4, 1] == "true")
            {
                this.enableBox5.Checked = true;
                this.fieldnameField5.Text = settings.Fields[4, 2];
                this.stickyBox5.Checked = Convert.ToBoolean(settings.Fields[4, 3]);
                this.requiredBox5.Checked = Convert.ToBoolean(settings.Fields[4, 4]);
                this.filenameBox5.Checked = Convert.ToBoolean(settings.Fields[4, 5]);
            }

            if (settings.Fields[5, 1] == "true")
            {
                this.enableBox6.Checked = true;
                this.fieldnameField6.Text = settings.Fields[5, 2];
                this.stickyBox6.Checked = Convert.ToBoolean(settings.Fields[5, 3]);
                this.requiredBox6.Checked = Convert.ToBoolean(settings.Fields[5, 4]);
                this.filenameBox6.Checked = Convert.ToBoolean(settings.Fields[5, 5]);
            }

            if (settings.Fields[6, 1] == "true")
            {
                this.enableBox7.Checked = true;
                this.fieldnameField7.Text = settings.Fields[6, 2];
                this.stickyBox7.Checked = Convert.ToBoolean(settings.Fields[6, 3]);
                this.requiredBox7.Checked = Convert.ToBoolean(settings.Fields[6, 4]);
                this.filenameBox7.Checked = Convert.ToBoolean(settings.Fields[6, 5]);
            }

            if (settings.Fields[7, 1] == "true")
            {
                this.enableBox8.Checked = true;
                this.fieldnameField8.Text = settings.Fields[7, 2];
                this.stickyBox8.Checked = Convert.ToBoolean(settings.Fields[7, 3]);
                this.requiredBox8.Checked = Convert.ToBoolean(settings.Fields[7, 4]);
                this.filenameBox8.Checked = Convert.ToBoolean(settings.Fields[7, 5]);
            }
            refreshSettings();
        }

        #endregion
    }

    /// <summary>
    /// This class contains information about settings.
    /// </summary>
    public class settingItems 
    {
        public string Title { get; set; }
        public string ProjectID { get; set; }
        public bool FileRenaming { get; set; }
        public string[,] Fields { get; set; }

        public settingItems(List<string> data)
        {
            //If the list with settings contains more or less than 51 items returns error.
            if(data.Count !=51)
            {
                MessageBox.Show("Settings file is corrupted", "Loading settings");
                return;
            }
            Fields = new string[8, 6];
            this.Title = data[0];
            this.ProjectID = data[1];
            this.FileRenaming = Convert.ToBoolean(data[2]);

            MessageBox.Show(data.Count.ToString());

            int index = 3;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    //Console.WriteLine(data[index]);
                    this.Fields[i, j] = data[index];
                    index = index + 1;
                }
            }
        }
    }
}
