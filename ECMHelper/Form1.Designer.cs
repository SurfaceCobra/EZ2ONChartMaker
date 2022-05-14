namespace ECMHelper
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Button ButtonImageLoad;
            System.Windows.Forms.Button ButtonImageDelete;
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.DataText1B = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ButtonSave = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.DataText2B = new System.Windows.Forms.TextBox();
            this.DataText3B = new System.Windows.Forms.TextBox();
            this.DataText4B = new System.Windows.Forms.TextBox();
            this.DataText1A = new System.Windows.Forms.TextBox();
            this.DataText2A = new System.Windows.Forms.TextBox();
            this.DataText3A = new System.Windows.Forms.TextBox();
            this.ButtonImageUpdate = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            ButtonImageLoad = new System.Windows.Forms.Button();
            ButtonImageDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonImageLoad
            // 
            ButtonImageLoad.Location = new System.Drawing.Point(6, 6);
            ButtonImageLoad.Name = "ButtonImageLoad";
            ButtonImageLoad.Size = new System.Drawing.Size(111, 23);
            ButtonImageLoad.TabIndex = 1;
            ButtonImageLoad.Text = "이미지 불러오기";
            ButtonImageLoad.UseVisualStyleBackColor = true;
            ButtonImageLoad.Click += new System.EventHandler(this.ButtonImageLoad_Click);
            // 
            // ButtonImageDelete
            // 
            ButtonImageDelete.Location = new System.Drawing.Point(6, 278);
            ButtonImageDelete.Name = "ButtonImageDelete";
            ButtonImageDelete.Size = new System.Drawing.Size(122, 23);
            ButtonImageDelete.TabIndex = 20;
            ButtonImageDelete.Text = "이미지 삭제";
            ButtonImageDelete.UseVisualStyleBackColor = true;
            // 
            // PictureBox
            // 
            this.PictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PictureBox.Location = new System.Drawing.Point(12, 12);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(604, 496);
            this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox.TabIndex = 0;
            this.PictureBox.TabStop = false;
            // 
            // DataText1B
            // 
            this.DataText1B.Location = new System.Drawing.Point(86, 35);
            this.DataText1B.Name = "DataText1B";
            this.DataText1B.Size = new System.Drawing.Size(100, 23);
            this.DataText1B.TabIndex = 6;
            this.DataText1B.TextChanged += new System.EventHandler(this.DataText1B_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ButtonSave,
            this.ButtonLoad});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // ButtonSave
            // 
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(122, 22);
            this.ButtonSave.Text = "저장";
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // ButtonLoad
            // 
            this.ButtonLoad.Name = "ButtonLoad";
            this.ButtonLoad.Size = new System.Drawing.Size(122, 22);
            this.ButtonLoad.Text = "불러오기";
            this.ButtonLoad.Click += new System.EventHandler(this.ButtonLoad_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(6, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(75, 23);
            this.textBox2.TabIndex = 8;
            this.textBox2.Text = "위측 간격";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(6, 93);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(75, 23);
            this.textBox3.TabIndex = 9;
            this.textBox3.Text = "칸 높이";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(6, 122);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(75, 23);
            this.textBox4.TabIndex = 10;
            this.textBox4.Text = "칸 간격";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(6, 6);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(180, 23);
            this.textBox6.TabIndex = 12;
            this.textBox6.Text = "구분선 설정";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(6, 64);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(75, 23);
            this.textBox7.TabIndex = 13;
            this.textBox7.Text = "좌측 간격";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(6, 35);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(66, 23);
            this.textBox5.TabIndex = 14;
            this.textBox5.Text = "x좌표";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(6, 64);
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(66, 23);
            this.textBox8.TabIndex = 15;
            this.textBox8.Text = "y좌표";
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(6, 93);
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.Size = new System.Drawing.Size(66, 23);
            this.textBox9.TabIndex = 16;
            this.textBox9.Text = "높이";
            // 
            // DataText2B
            // 
            this.DataText2B.Location = new System.Drawing.Point(86, 64);
            this.DataText2B.Name = "DataText2B";
            this.DataText2B.Size = new System.Drawing.Size(100, 23);
            this.DataText2B.TabIndex = 17;
            this.DataText2B.TextChanged += new System.EventHandler(this.DataText2B_TextChanged);
            // 
            // DataText3B
            // 
            this.DataText3B.Location = new System.Drawing.Point(86, 93);
            this.DataText3B.Name = "DataText3B";
            this.DataText3B.Size = new System.Drawing.Size(100, 23);
            this.DataText3B.TabIndex = 18;
            this.DataText3B.TextChanged += new System.EventHandler(this.DataText3B_TextChanged);
            // 
            // DataText4B
            // 
            this.DataText4B.Location = new System.Drawing.Point(86, 122);
            this.DataText4B.Name = "DataText4B";
            this.DataText4B.Size = new System.Drawing.Size(100, 23);
            this.DataText4B.TabIndex = 19;
            this.DataText4B.TextChanged += new System.EventHandler(this.DataText4B_TextChanged);
            // 
            // DataText1A
            // 
            this.DataText1A.Location = new System.Drawing.Point(78, 35);
            this.DataText1A.Name = "DataText1A";
            this.DataText1A.Size = new System.Drawing.Size(108, 23);
            this.DataText1A.TabIndex = 21;
            // 
            // DataText2A
            // 
            this.DataText2A.Location = new System.Drawing.Point(78, 64);
            this.DataText2A.Name = "DataText2A";
            this.DataText2A.Size = new System.Drawing.Size(108, 23);
            this.DataText2A.TabIndex = 22;
            // 
            // DataText3A
            // 
            this.DataText3A.Location = new System.Drawing.Point(78, 93);
            this.DataText3A.Name = "DataText3A";
            this.DataText3A.Size = new System.Drawing.Size(108, 23);
            this.DataText3A.TabIndex = 23;
            // 
            // ButtonImageUpdate
            // 
            this.ButtonImageUpdate.Location = new System.Drawing.Point(622, 485);
            this.ButtonImageUpdate.Name = "ButtonImageUpdate";
            this.ButtonImageUpdate.Size = new System.Drawing.Size(125, 23);
            this.ButtonImageUpdate.TabIndex = 24;
            this.ButtonImageUpdate.Text = "이미지 새로고침";
            this.ButtonImageUpdate.UseVisualStyleBackColor = true;
            this.ButtonImageUpdate.Click += new System.EventHandler(this.ButtonImageUpdate_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(625, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(253, 467);
            this.tabControl1.TabIndex = 25;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.textBox6);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Controls.Add(this.textBox7);
            this.tabPage1.Controls.Add(this.textBox3);
            this.tabPage1.Controls.Add(this.DataText4B);
            this.tabPage1.Controls.Add(this.textBox4);
            this.tabPage1.Controls.Add(this.DataText3B);
            this.tabPage1.Controls.Add(this.DataText1B);
            this.tabPage1.Controls.Add(this.DataText2B);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(245, 439);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "구분선";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(ButtonImageLoad);
            this.tabPage2.Controls.Add(this.textBox5);
            this.tabPage2.Controls.Add(this.textBox8);
            this.tabPage2.Controls.Add(ButtonImageDelete);
            this.tabPage2.Controls.Add(this.DataText3A);
            this.tabPage2.Controls.Add(this.textBox9);
            this.tabPage2.Controls.Add(this.DataText2A);
            this.tabPage2.Controls.Add(this.DataText1A);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(245, 439);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "이미지";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(245, 439);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "텍스트";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 520);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.ButtonImageUpdate);
            this.Controls.Add(this.PictureBox);
            this.Name = "Form1";
            this.Text = "EZ2ON 서열표 제작기";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox PictureBox;
        private Button ButtonImageLoad;
        private TextBox DataText1B;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem ButtonSave;
        private ToolStripMenuItem ButtonLoad;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox textBox6;
        private TextBox textBox7;
        private TextBox textBox5;
        private TextBox textBox8;
        private TextBox textBox9;
        private TextBox DataText2B;
        private TextBox DataText3B;
        private TextBox DataText4B;
        private TextBox DataText1A;
        private TextBox DataText2A;
        private TextBox DataText3A;
        private Button ButtonImageUpdate;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
    }
}