using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SimulatorAutomat
{
    public partial class StareNeacceptoare : UserControl
    {
        protected int xPoz, yPoz; // pozitia curenta a starii pe ecran
        protected bool dragDrop; // daca miscam starea pe ecran
        protected bool acceptoare; // starea este sau nu acceptoare
        protected bool esteDeStart; // starea este sau nu de start
        protected Color culoare; // culoarea starii
        protected int grosime; // grosimea cu care este desenata starea
        protected bool ajungemPrinEpsilon; // daca in aceasta stare se ajunge prin simbolul epsilon
        protected bool amDatEnter; // folosita pentru a controla afisarea numelui starii

        protected ContextMenu cm;
        protected MenuItem miAcc;
        protected MenuItem miStart;
        protected MenuItem miSterge;

        public EventHandler HandlerStergeStare;

        public StareNeacceptoare()
        {
            acceptoare = false;
            esteDeStart = false;
            culoare = Color.RoyalBlue; // implicit albastru
            grosime = 2; // default 2 px
            ajungemPrinEpsilon = false;
            amDatEnter = false;

            InitializeComponent();
            this.textBox_stare.LostFocus += new EventHandler(textBox_stare_LostFocus);
            cm = new ContextMenu();
        }

        void textBox_stare_LostFocus(object sender, EventArgs e)
        {
            if (!amDatEnter)
                ActiuneEnter();
        }

        public String NumeStare { get { return this.label_stare.Text; } set { this.label_stare.Text = value; } }
        public bool Acceptoare { get { return acceptoare; } }
        public bool EsteDeStart { get { return esteDeStart; } }
        public bool AjungemPrinEpsilon { get { return this.ajungemPrinEpsilon; } set { this.ajungemPrinEpsilon = value; } }
        public int PozitieX { get { return this.xPoz; } set { this.xPoz = value; } }
        public int PozitieY { get { return this.yPoz; } set { this.yPoz = value; } }
        public bool MutamStarea { get { return dragDrop; } set { dragDrop = value; } }
        public Color Culoare { get { return culoare; } set { culoare = value; } }
        public int Grosime { get { return grosime; } set { grosime = value; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            miAcc = new MenuItem("Acceptoare");
            miStart = new MenuItem("De start");
            miSterge = new MenuItem("Sterge");
            // miAcc.RadioCheck = true;
            // miStart.RadioCheck = true;
            miAcc.Click += new EventHandler(miAcc_Click);
            miStart.Click += new EventHandler(miStart_Click);
            miSterge.Click += new EventHandler(miSterge_Click);
            cm.MenuItems.Add(miAcc);
            cm.MenuItems.Add(miStart);
            cm.MenuItems.Add(miSterge);

            if (this.Parent is PictureBox)
            {
                this.ContextMenu = cm;
                toolTipSugestie.SetToolTip(this, "ALT + Click pentru a copia stare");
            }
            else
                toolTipSugestie.SetToolTip(this, "Click & Drag pentru a adauga stare neacceptoare");
        }

        void miSterge_Click(object sender, EventArgs e)
        {
            this.OnActiuneStergeStare();
        }

        public void OnActiuneStergeStare()
        {
            if (this.HandlerStergeStare != null)
                this.HandlerStergeStare(this, new EventArgs());
        }

        public virtual void miStart_Click(object sender, EventArgs e)
        {
            this.miStart.Checked = !this.miStart.Checked;
            if (this.miStart.Checked)
                this.esteDeStart = true;
            else this.esteDeStart = false;
            this.Invalidate();
        }

        public virtual void miAcc_Click(object sender, EventArgs e)
        {
            this.miAcc.Checked = !this.miAcc.Checked;
            if (this.miAcc.Checked) this.acceptoare = true;
            else this.acceptoare = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Capture = true;
            if (this.Parent is PictureBox && e.Button == MouseButtons.Left)
            {
                Cursor.Clip = this.Parent.RectangleToScreen(new Rectangle(e.X, e.Y, this.Parent.ClientSize.Width - this.Width, this.Parent.ClientSize.Height - this.Height));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            dragDrop = true;
            if (e.Button == MouseButtons.Left)
            {
                // daca butonul stanga este apasat
                if ((Control.MouseButtons & MouseButtons.Left) != 0)
                {
                    // daca starea e in interiorul pictureBox-ului
                    if (this.Parent is PictureBox)
                    {
                        // duplicam starea la ALT + Click
                        if (Control.ModifierKeys == Keys.Alt && dragDrop == true)
                            this.DoDragDrop(this, DragDropEffects.Copy);
                        // daca nu e apasat ALT, pur si simplu mutam starea
                        else if (this.Parent.ClientRectangle.Contains(this.Parent.PointToClient(Cursor.Position)))
                        {
                            // mouse-ul este in limitele panoului 
                            this.Top += (e.Y - yPoz);
                            this.Left += (e.X - xPoz);
                        }
                    }
                    // daca starea e in bara de meniu si nu in pictureBox, facem D&D la click de acolo
                    else if (dragDrop == true)
                        this.DoDragDrop(this, DragDropEffects.Copy);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            dragDrop = false;
            amDatEnter = false;
            Cursor.Clip = Rectangle.Empty;
            this.Capture = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left && this.Parent is PictureBox)
            {
                ActiuneDubluClick();
            }
        }

        protected void ActiuneDubluClick()
        {
            textBox_stare.Visible = true;
            textBox_stare.Focus();
            label_stare.Visible = false;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Pen p = new Pen(culoare, grosime);
            p.Color = culoare;
            Graphics g = e.Graphics;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle r = new Rectangle(2, 2, this.Width - 4, this.Height - 4);
            g.DrawEllipse(p, r);

            // daca dam click dreapta, o facem acceptoare in functie de meniul ala contextual
            if (this.acceptoare)
            {
                // mai desenam un cerculet
                r = new Rectangle(5, 5, this.Width - 10, this.Height - 10);
                g.DrawEllipse(p, r);
            }

            // daca e de start, desenam sageata sus
            if (this.esteDeStart)
            {
                p.Width = 8;
                p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                g.DrawLine(p, new Point(0, 0), new Point(20, 20));
            }

            p.Dispose();
            var grafPath = new System.Drawing.Drawing2D.GraphicsPath();
            grafPath.AddEllipse(0, 0, this.Width, this.Height);
            this.Region = new System.Drawing.Region(grafPath);
            grafPath.Dispose();

            this.label_stare.ForeColor = culoare;
        }

        protected virtual void textBox_stare_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ActiuneEnter();
                amDatEnter = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                this.OnKeyUp(e);
            }
        }

        protected void ActiuneEnter()
        {
            label_stare.Text = textBox_stare.Text;
            textBox_stare.Visible = false;
            label_stare.Visible = true;
        }

        protected virtual void label_stare_SizeChanged(object sender, EventArgs e)
        {
            label_stare.Left = (this.Width - label_stare.Size.Width) / 2;
        }

        protected void label_stare_MouseDown(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        protected void label_stare_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.OnMouseDoubleClick(e);
        }

        protected void label_stare_MouseMove(object sender, MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        protected void label_stare_MouseUp(object sender, MouseEventArgs e)
        {
            this.OnMouseUp(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Brush b = new SolidBrush(this.BackColor);
            e.Graphics.FillEllipse(b, 0, 0, this.Width, this.Height);
            b.Dispose();
        }

        public void FaVizibilaLabelStare()
        {
            this.label_stare.Visible = true;
        }

        public MenuItem getMeniuAcceptoare()
        {
            return this.miAcc;
        }

        public MenuItem getMeniuStart()
        {
            return this.miStart;
        }

        public MenuItem getMeniuSterge()
        {
            return this.miSterge;
        }
        public ContextMenu getContextMeniu()
        {
            return this.cm;
        }
    }
}
