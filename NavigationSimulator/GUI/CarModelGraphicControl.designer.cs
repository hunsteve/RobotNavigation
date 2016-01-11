namespace OnlabNeuralis
{
    partial class CarModelGraphicControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CarModelGraphicControl
            //             
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.CarModelGraphicControl_Layout);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CarModelGraphicControl_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CarModelGraphicControl_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CarModelGraphicControl_MouseUp);
            this.MouseEnter += new System.EventHandler(this.CarModelGraphicControl_MouseEnter);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
