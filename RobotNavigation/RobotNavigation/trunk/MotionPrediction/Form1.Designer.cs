namespace DiplomaMunka
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.motionDraw1 = new DiplomaMunka.MotionDraw();
            this.SuspendLayout();
            // 
            // motionDraw1
            // 
            this.motionDraw1.Location = new System.Drawing.Point(12, 12);
            this.motionDraw1.Name = "motionDraw1";
            this.motionDraw1.Size = new System.Drawing.Size(572, 582);
            this.motionDraw1.TabIndex = 0;
            this.motionDraw1.Text = "motionDraw1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 606);
            this.Controls.Add(this.motionDraw1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private MotionDraw motionDraw1;
    }
}

