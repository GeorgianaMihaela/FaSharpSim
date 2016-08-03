using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Drawing2D;

namespace SimulatorAutomat
{
    public partial class Tranzitie : UserControl
    {
        private int xPoz, yPoz;
        private int indexStareStart;
        private int indexStareStop;
        private string numeStart;
        private string numeStop;
        private Pen pen;
        private AdjustableArrowCap arrowCap;
        private bool amDatEnter;

        public Tranzitie()
        {
            pen = new Pen(Color.RoyalBlue, 2);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Custom;
            arrowCap = new AdjustableArrowCap(6, 6, true);
            pen.CustomEndCap = arrowCap;
            amDatEnter = false;

            InitializeComponent();
            this.labelTranz.Text = "?";
            this.labelTranz.Visible = true;

            if (labelTranz.Text != "?") tbTranz.Text = labelTranz.Text;
            labelTranz.MinimumSize = new Size(50, 20);
            labelTranz.AutoSize = false;
            labelTranz.AutoEllipsis = true;
            this.tbTranz.LostFocus += new EventHandler(tbTranz_LostFocus);
        }

        void tbTranz_LostFocus(object sender, EventArgs e)
        {
            if (!amDatEnter) ActiuneEnterSimboluri();
        }

        public int PozitieX { get { return xPoz; } set { xPoz = value; } }
        public int PozitieY { get { return yPoz; } set { yPoz = value; } }

        public int IndexStareStart { get { return indexStareStart; } set { indexStareStart = value; } }
        public int IndexStareStop { get { return indexStareStop; } set { indexStareStop = value; } }

        public string NumeStart { get { return numeStart; } set { numeStart = value; } }
        public string NumeStop { get { return numeStop; } set { numeStop = value; } }

        public Pen PenTranzitie { get { return pen; } }
        public string TextTranzitie { get { return this.labelTranz.Text; } }
        public TextBox GetTbTranz { get { return this.tbTranz; } }

        public void SchimbaCuloarea(Color culoare, int grosime)
        {
            pen.Color = culoare;
            pen.Width = grosime;

            arrowCap.Width = grosime + 4;
            arrowCap.Height = grosime + 4;
            this.labelTranz.ForeColor = culoare;
        }

        public List<char> GetSimboluriTranzitie()
        {
            List<char> ctmp = new List<char>();

            if (this.labelTranz.Text != "")
            {
                // trebuie sa facem trim, ca textul sa nu aiba space intre caractere si virgula
                this.labelTranz.Text = this.labelTranz.Text.Replace(" ", String.Empty);
                if (this.labelTranz.Text.IndexOf(',') >= 0)
                {
                    string[] stmp = this.labelTranz.Text.Split(',');
                    for (int t = 0; t < stmp.Length; t++)
                        ctmp.Add(Convert.ToChar(stmp[t]));
                }
                else ctmp.Add(Convert.ToChar(this.labelTranz.Text));
            }
            return ctmp;
        }

        public void SetSimboluri(string s)
        {
            if (this.labelTranz.Text == "" || this.labelTranz.Text == "?")
                this.labelTranz.Text = s;
            else // vedem daca se termina cu virgula, daca nu, adaugam noi virgula si simbolurile
                if (this.labelTranz.Text.EndsWith(","))
                    this.labelTranz.Text += s;
                else 
                    this.labelTranz.Text += "," + s;
           
            this.labelTranz.Visible = true;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left)
            {
                if (labelTranz.Text != "?") tbTranz.Text = labelTranz.Text;
                tbTranz.Visible = true;
                tbTranz.Focus();
                labelTranz.Visible = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                this.Top += (e.Y - yPoz);
                this.Left += (e.X - xPoz);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                xPoz = e.X;
                yPoz = e.Y;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            amDatEnter = false;
        }

        private void ActiuneEnterSimboluri()
        {
            bool okVirgula = true; // presupunem ca virgulele sunt la locul lor
            // eliminam spatiile
            string textTranz = tbTranz.Text.Replace(" ", string.Empty);

            if (tbTranz.Text != ",")
            {
                // trebuie sa avem virgula pe pozitii impare din stringul de pe tranzitie
                // de exemplu pt a,b,c - lungime=6, virgula se afla pe pozitiile: 1,3

                // daca incepe cu virgula sau se termina cu virgula, o eliminam
                // dar poate fi si virgula caracter admis pe tranzitie
                if (textTranz.StartsWith(",")) textTranz = textTranz.Substring(1);
                if (textTranz.EndsWith(",")) textTranz = textTranz.Substring(0, textTranz.Length - 1);
                if (textTranz.Length != 1) // daca avem mai mult de un singur caracter
                {
                    for (int i = 1; i < textTranz.Length; i += 2)
                        if (textTranz[i] != ',') okVirgula = false; // trebuie sa avem virgula dupa fiecare caracter din sir
                }
            }
            if (okVirgula)
            {
                labelTranz.Text = textTranz;
                tbTranz.Visible = false;
                labelTranz.Visible = true;
            }
            else
            {
                // spunem utilizatorului ca stringul introdus nu e ok
                tbTranz.Visible = true;
                labelTranz.Visible = false;
                MessageBox.Show("Format tranzitie incorect! Trebuie sa fie de forma c,c,c", "Eroare!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbTranz_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                amDatEnter = true;
                ActiuneEnterSimboluri();
            }
        }

        private void labelTranz_SizeChanged(object sender, EventArgs e)
        {
            labelTranz.Left = (this.Width - labelTranz.Size.Width) / 2;
        }

        // propagam evenimentele de dublu click, miscare mouse si mouse down 
        // de la controlul mare (tranzitie) catre label-ul pe care scriem simbolul
        // ca sa raspunda la ele 
        private void labelTranz_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.OnMouseDoubleClick(e);
        }

        private void labelTranz_MouseMove(object sender, MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        private void labelTranz_MouseDown(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        private void labelTranz_TextChanged(object sender, EventArgs e)
        {
            Label l = sender as Label;
            Size size = TextRenderer.MeasureText(tbTranz.Text, tbTranz.Font);
            l.Width = size.Width;
            l.Height = size.Height;
        }

        public void FaVizibilaLabelTranz()
        {
            this.labelTranz.Visible = true;
        }

        public void FaInvizibilaLabelTranz()
        {
            this.labelTranz.Visible = false;
        }

        public void FaVizibilTextBoxTranz()
        {
            if (labelTranz.Text != "?") tbTranz.Text = labelTranz.Text; // cand editam, sa ne pastreze textul de dinainte
            this.tbTranz.Visible = true;
        }
        public void FaInvizibilTextBoxTranz()
        {
            tbTranz.Text = "";
            this.tbTranz.Visible = false;
        }

        public override string ToString()
        {
            return "(" + this.numeStart + ", " + this.numeStop + ")";
        }
    }
}
