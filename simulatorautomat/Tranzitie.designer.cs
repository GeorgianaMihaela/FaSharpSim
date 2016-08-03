namespace SimulatorAutomat
{
    partial class Tranzitie
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
            this.tbTranz = new System.Windows.Forms.TextBox();
            this.labelTranz = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbTranz
            // 
            this.tbTranz.Location = new System.Drawing.Point(7, 6);
            this.tbTranz.Name = "tbTranz";
            this.tbTranz.Size = new System.Drawing.Size(39, 20);
            this.tbTranz.TabIndex = 0;
            this.tbTranz.Visible = false;
            this.tbTranz.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbTranz_KeyUp);
            // 
            // labelTranz
            // 
            this.labelTranz.AutoEllipsis = true;
            this.labelTranz.AutoSize = true;
            this.labelTranz.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTranz.ForeColor = System.Drawing.Color.RoyalBlue;
            this.labelTranz.Location = new System.Drawing.Point(17, 7);
            this.labelTranz.Name = "labelTranz";
            this.labelTranz.Size = new System.Drawing.Size(16, 16);
            this.labelTranz.TabIndex = 1;
            this.labelTranz.Text = "?";
            this.labelTranz.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTranz.SizeChanged += new System.EventHandler(this.labelTranz_SizeChanged);
            this.labelTranz.TextChanged += new System.EventHandler(this.labelTranz_TextChanged);
            this.labelTranz.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.labelTranz_MouseDoubleClick);
            this.labelTranz.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelTranz_MouseDown);
            this.labelTranz.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labelTranz_MouseMove);
            // 
            // Tranzitie
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.labelTranz);
            this.Controls.Add(this.tbTranz);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "Tranzitie";
            this.Size = new System.Drawing.Size(60, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbTranz;
        private System.Windows.Forms.Label labelTranz;
    }
}
