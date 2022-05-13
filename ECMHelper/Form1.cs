using ECMBase;

namespace ECMHelper
{
    public partial class Form1 : Form
    {

        ECMProject currentProject = ECMProject.Create();
        string currentProjectPath;

        public Form1()
        {
            InitializeComponent();
        }




        public void UpdateImage()
        {
            if (PictureBox.Image!=null) PictureBox.Image.Dispose();
            PictureBox.Image = currentProject.option.GetImage();

            PictureBox.Update();
        }

        private void ButtonImageLoad_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ButtonLoad_Click(null, null);
            RefreshButtons();
            UpdateImage();
        }

        void RefreshButtons()
        {
            DataText1B.Text = currentProject.option.divider.topgap.ToString();
            DataText2B.Text = currentProject.option.divider.leftgap.ToString();
            DataText3B.Text = currentProject.option.divider.boxheight.ToString();
            DataText4B.Text = currentProject.option.divider.boxgap.ToString();
        }


        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }


        //嬪難 除問
        private void DataText1B_TextChanged(object sender, EventArgs e)
        {
            string str = (sender as TextBox).Text;
            if (int.TryParse(str,out int val))
            {

                currentProject.option.divider.topgap = val;
            }
        }

        //謝難 除問
        private void DataText2B_TextChanged(object sender, EventArgs e)
        {
            string str = (sender as TextBox).Text;
            if (int.TryParse(str, out int val))
            {
                currentProject.option.divider.leftgap = val;
            }
        }


        //蘊 堪檜
        private void DataText3B_TextChanged(object sender, EventArgs e)
        {
            string str = (sender as TextBox).Text;
            if (int.TryParse(str, out int val))
            {
                currentProject.option.divider.boxheight = val;
            }
        }


        //蘊 除問
        private void DataText4B_TextChanged(object sender, EventArgs e)
        {
            string str = (sender as TextBox).Text;
            if (int.TryParse(str, out int val))
            {
                currentProject.option.divider.boxgap = val;
            }
        }

        private void ButtonImageUpdate_Click(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void ButtonLoadData_Click(object sender, EventArgs e)
        {
            string str = (sender as TextBox).Text;
            
            
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            string str = OpenFolderDialog();
            if (str != "")
            {
                this.currentProject = ECMDataLoader.LoadProject(str);
                
                RefreshButtons();
            }
            UpdateImage();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if(currentProjectPath==null)
                currentProjectPath = OpenFolderDialog();

            ECMDataLoader.SaveProjectOption(this.currentProject.option, currentProjectPath);

        }

        string OpenFolderDialog()
        {
            string path;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path = fbd.SelectedPath;
                }
                else return "";
            }
            if (path == null) return "";

            return path;

        }
    }
}