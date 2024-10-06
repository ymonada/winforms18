namespace WinFormsApp18
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
            drawingPanel = new Panel();
            btnRun = new Button();
            SuspendLayout();
            // 
            // drawingPanel
            // 
            drawingPanel.Location = new Point(0, 25);
            drawingPanel.Name = "drawingPanel";
            drawingPanel.Size = new Size(1920, 1080);
            drawingPanel.TabIndex = 0;
            // 
            // btnRun
            // 
            btnRun.Location = new Point(0, 0);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(75, 25);
            btnRun.TabIndex = 0;
            btnRun.Text = "Run";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += btnRun_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1307, 756);
            Controls.Add(btnRun);
            Controls.Add(drawingPanel);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Panel drawingPanel;
        private Button btnRun;
    }
}
