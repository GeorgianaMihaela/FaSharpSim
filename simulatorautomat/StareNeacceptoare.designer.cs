namespace SimulatorAutomat
{
    partial class StareNeacceptoare
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
            this.components = new System.ComponentModel.Container();
            this.label_stare = new System.Windows.Forms.Label();
            this.textBox_stare = new System.Windows.Forms.TextBox();
            this.toolTipSugestie = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label_stare
            // 
            this.label_stare.AutoSize = true;
            this.label_stare.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_stare.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_stare.Location = new System.Drawing.Point(29, 26);
            this.label_stare.Name = "label_stare";
            this.label_stare.Size = new System.Drawing.Size(27, 16);
            this.label_stare.TabIndex = 0;
            this.label_stare.Text = "tux";
            this.label_stare.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_stare.Visible = false;
            this.label_stare.SizeChanged += new System.EventHandler(this.label_stare_SizeChanged);
            this.label_stare.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.label_stare_MouseDoubleClick);
            this.label_stare.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_stare_MouseDown);
            this.label_stare.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_stare_MouseMove);
            this.label_stare.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_stare_MouseUp);
            // 
            // textBox_stare
            // 
            this.textBox_stare.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_stare.ForeColor = System.Drawing.Color.RoyalBlue;
            this.textBox_stare.Location = new System.Drawing.Point(9, 25);
            this.textBox_stare.Name = "textBox_stare";
            this.textBox_stare.Size = new System.Drawing.Size(52, 22);
            this.textBox_stare.TabIndex = 1;
            this.textBox_stare.Visible = false;
            this.textBox_stare.Text = "";
            this.textBox_stare.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_stare_KeyUp);
            // 
            // StareNeacceptoare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.textBox_stare);
            this.Controls.Add(this.label_stare);
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(50, 50);
            this.Name = "StareNeacceptoare";
            this.Size = new System.Drawing.Size(70, 70);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        protected System.Windows.Forms.Label label_stare;
        protected System.Windows.Forms.TextBox textBox_stare;
        protected System.Windows.Forms.ToolTip toolTipSugestie;


    }
}
