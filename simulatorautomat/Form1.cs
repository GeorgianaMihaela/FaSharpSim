using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Text.RegularExpressions;
using System.Reflection;
using SimulatorAutomat.Properties;

namespace SimulatorAutomat
{
    public partial class FormHome : Form
    {
        // starile puse in meniul din stanga 
        private StareNeacceptoareMenuItem stareNeaccItem;
        private StareAcceptoareMenuItem stareAccMenuItem;
        private StareAcceptoareStartMenuItem stareAccStartItem;
        private StareNeacceptoareStartMenuItem stareNeaccStartItem;

        // daca acest checkbox e bifat, putem incepe sa construim o legatura intre 2 stari
        private CheckBoxEnableLegatura cbEnableLegatura;
        // sau sa stergem o legatura intre stari
        private CheckBoxDisableLegatura cbDisableLegatura;

        private int contorStari; // numaram cate stari am adaugat in panoul de desen
        private int numarClicks; // daca numarClicks=2, putem sa facem o legatura intre 2 stari
        private int nrClicksSim; // index prin string in simularea pas cu pas

        // intr-o legatura, starea de start e data de idxStart;
        // starea de stop -- idxStop
        // iar idxStareSelectata ne arata pe care stare am dat ultimul click
        private int idxStart, idxStop, idxStareSelectata;

        // lista starilor adaugate pe panza mare de desen
        private List<StareNeacceptoare> listaStari;
        // lista de legaturi
        private List<Tranzitie> listaTranzitii;
        // alfabetul automatului
        private List<char> alfabet;

        private int razaStare;
        private bool vedemConsola; // folosita pt a face toggle intre vedem consola si nu vedem consola
        private bool oData; // ajuta la executarea unui cod o data atunci cand incepem simularea

        private bool[] suntemAici; // arata starea curenta prin true -- suntem in acea stare, 0 -- nu
        private bool[] vomFiAici; // arata starea viitoare 

        private List<Label> listaCulori;
        private Color culoareCurenta; // daca am apasat pe vreun label de culoare, preluam culoarea

        private bool amImportatXML; // daca am importat un automat din xml, asta e true
        // se face false daca facem drag and drop la o stare pe panzaAutomat

        private string cale; // unde salvam automatul si de unde importam

        private bool suntemStart; // ne ajuta ca doar o data sa cautam starea de start in simulare
        private bool simGata = false; // preluam rezultatul la functia SimuleazaAutomat

        private Label labelEpsilon; // cu ajutorul acestui label, copiem in clipboard litera epsilon
        private bool areDoarAlfa; // pastram daca af-ul are doar simboluri din alfabet

        List<Color> listaCuloriStare; // culorile inainte de simulare ale starilor

        private static Random r = new Random(); // folosit pt a genera aleator culori pt simulare
        private Color culoareSimulare; // culoarea random folosita in simulare pt tranzitii
        private Color culoareSimNeg; // inversul culorii random 

        private string statusStare;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem unelteToolStripMenuItem;

        private bool amSalvatXML; // true cand am salvat ca xml automatul
        private bool vedemButoanele; // true daca butoanele de jos sunt vizibile 

        private List<char> simbStare; // folosita pentru a colecta simbolurile de pe fiecare tranzitie care pleaca dintr-o stare listaStari[t] in functia EsteDeterminist()

        // variabile folosite pentru selectarea mai multor stari in acelasi timp
        private Point inceputSelectie; // inceputul dreptunghiului de selectie pa panzaAutomat
        private Point sfarsitSelectie; // sfarsitul dreptunghiului de selectie
        private Rectangle dreptunghiSelectie; // dreptunghiul in care sunt incadrate starile selectate

        List<StareNeacceptoare> listaStariSelectate; // starile pe care le selectam cu mouse-ul
        private bool mouseDownPanza; // daca avem mouse-ul apasat pe panzaAutomat
        private Cursor cursEps;
        private string extensie; // folosita pt a verifica faptul ca fisierul importat este xml

        private Bitmap bmpOriginal;
        private Bitmap bmpCrop;

        public FormHome()
        {
            InitializeComponent();

            this.menuStrip1.Items.Add(fisierToolStripMenuItem);
            this.menuStrip1.Items.Add(automatToolStripMenuItem);
            this.menuStrip1.Items.Add(vizualizareToolStripMenuItem);

            this.helpToolStripMenuItem = new ToolStripMenuItem("Ajutor"); // facem dinamic, ca mi-a pocat designerul
            this.helpToolStripMenuItem.ForeColor = SystemColors.HotTrack;
            this.helpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.F1));
            this.helpToolStripMenuItem.Click += new EventHandler(helpToolStripMenuItem_Click);
            this.helpToolStripMenuItem.Image = global::SimulatorAutomat.Properties.Resources.help;

            this.consolaToolStripMenuItem.Image = global::SimulatorAutomat.Properties.Resources.consola;
            this.consolaToolStripMenuItem.ShortcutKeys = Keys.F11;
            this.statusStrip1.Items.Add(statusLabel);

            this.unelteToolStripMenuItem = new ToolStripMenuItem("Bara butoane");
            this.unelteToolStripMenuItem.ForeColor = SystemColors.HotTrack;
            this.unelteToolStripMenuItem.ShortcutKeys = Keys.F8;
            this.unelteToolStripMenuItem.Click += new EventHandler(unelteToolStripMenuItem_Click);

            this.vizualizareToolStripMenuItem.DropDownItems.Add(helpToolStripMenuItem);
            this.vizualizareToolStripMenuItem.DropDownItems.Add(unelteToolStripMenuItem);

            this.stareNeaccItem = new StareNeacceptoareMenuItem();
            this.stareAccMenuItem = new StareAcceptoareMenuItem();
            this.stareAccStartItem = new StareAcceptoareStartMenuItem();
            this.stareNeaccStartItem = new StareNeacceptoareStartMenuItem();

            contorStari = 0;
            numarClicks = 0;
            nrClicksSim = 0;
            idxStart = idxStop = -1;
            idxStareSelectata = -1;

            razaStare = 0;
            vedemConsola = false;
            oData = true;
            areDoarAlfa = false;
            mouseDownPanza = false;

            listaStari = new List<StareNeacceptoare>();
            listaTranzitii = new List<Tranzitie>();
            alfabet = new List<char>();
            listaCuloriStare = new List<Color>();
            simbStare = new List<char>();
            listaStariSelectate = new List<StareNeacceptoare>();

            cbEnableLegatura = new CheckBoxEnableLegatura();
            cbDisableLegatura = new CheckBoxDisableLegatura();
            cbEnableLegatura.CheckedChanged += cbEnableLegatura_CheckedChanged;
            cbDisableLegatura.CheckedChanged += cbDisableLegatura_CheckedChanged;

            toolTip1.SetToolTip(butCompilare, "Compilare (F5)");
            toolTip1.SetToolTip(butSimulare, "Simulare (F6)");
            toolTip1.SetToolTip(butPas, "Simulare pas cu pas (F7)");
            toolTip1.SetToolTip(butColorPicker, "Alege culoarea preferata");
            toolTip1.SetToolTip(butSaveImg, "Exporta imagine (CTRL+E)");
            toolTip1.SetToolTip(butSaveXML, "Salveaza automatul (CTRL+S)");
            toolTip1.SetToolTip(butStergeBox, "Sterge panoul (ALT+C)");
            toolTip1.SetToolTip(butOpen, "Importa un automat (CTRL+O)");

            AdaugaTextColoratConsola("[+] Aplicatie pornita\n", rtbConsola.ForeColor);
            // etichetele de culoare din partea dreapta jos le vom adauga in lista de culori
            listaCulori = new List<Label>();
            listaCulori.Add(label1);
            listaCulori.Add(label2);
            listaCulori.Add(label3);
            listaCulori.Add(label4);
            listaCulori.Add(label5);
            listaCulori.Add(label6);
            listaCulori.Add(label7);
            listaCulori.Add(label9);
            listaCulori.Add(label8);
            listaCulori.Add(label10);
            listaCulori.Add(label11);
            listaCulori.Add(label12);
            listaCulori.Add(label13);
            listaCulori.Add(label14);
            listaCulori.Add(label15);
            listaCulori.Add(label16);

            // adaugam functia care trateaza evenimentul Click pe etichete
            for (int i = 0; i < listaCulori.Count; i++)
                listaCulori[i].Click += LabelCuloare_Click;

            // facem cursorul in forma de color picker atunci cand suntem cu mouse-ul pe panel-ul cu etichete
            using (MemoryStream m = new MemoryStream(global::SimulatorAutomat.Properties.Resources.colorpicker))
            {
                this.tableLayoutPanel3.Cursor = new Cursor(m);
            }
            // facem cursorul Epsilon cand facem drag&drop Epsilon
            using (MemoryStream m = new MemoryStream(global::SimulatorAutomat.Properties.Resources.epsilon))
            {
                cursEps = new Cursor(m);
            }

            culoareCurenta = Color.RoyalBlue; // default acest albastru dragut :)
            amImportatXML = false; // initial nu am importat automatul din xml
            suntemStart = true; // trebuie initial sa gasim starea de start pt a incepe simularea

            //cale = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            cale = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog1.InitialDirectory = cale;
            openFileDialog1.InitialDirectory = cale;

            butSimulare.Enabled = false;
            butPas.Enabled = false;
            simulareToolStripMenuItem.Enabled = false;

            this.panel1.AutoScrollMinSize = panzaAutomat.Size;

            statusStare = "";
            CreeazaLabelEpsilon();

            amSalvatXML = false;
            vedemButoanele = true;

            panzaAutomat.MouseClick += new MouseEventHandler(panzaAutomat_MouseClick);
            panzaAutomat.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        // se apeleeaza cand dam click pe meniul de vizualizare
        void unelteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActiuneVizualizareButoane();
        }

        private void ActiuneVizualizareButoane()
        {
            if (!vedemButoanele)
            {
                this.butColorPicker.Visible = true;
                this.butCompilare.Visible = true;
                this.butOpen.Visible = true;
                this.butPas.Visible = true;
                this.butSaveImg.Visible = true;
                this.butSaveXML.Visible = true;
                this.butSimulare.Visible = true;
                this.butStergeBox.Visible = true;
                this.butStop.Visible = true;
                if (rtbInput.Enabled)
                    this.rtbInput.Visible = true;

                this.label1.Visible = true;
                this.label2.Visible = true;
                this.label3.Visible = true;
                this.label4.Visible = true;
                this.label5.Visible = true;
                this.label6.Visible = true;
                this.label7.Visible = true;
                this.label8.Visible = true;
                this.label9.Visible = true;
                this.label10.Visible = true;
                this.label11.Visible = true;
                this.label12.Visible = true;
                this.label13.Visible = true;
                this.label14.Visible = true;
                this.label15.Visible = true;
                this.label16.Visible = true;

                this.labelAlfabet.Visible = true;
                this.tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Percent;
                this.tableLayoutPanel1.RowStyles[1].Height = 24;
                this.panzaAutomat.Height = this.splitContainer1.Height;
                this.panel1.Height = this.panzaAutomat.Height - 5;
            }
            else
            {
                this.butColorPicker.Visible = false;
                this.butCompilare.Visible = false;
                this.butOpen.Visible = false;
                this.butPas.Visible = false;
                this.butSaveImg.Visible = false;
                this.butSaveXML.Visible = false;
                this.butSimulare.Visible = false;
                this.butStergeBox.Visible = false;
                this.butStop.Visible = false;
                this.rtbInput.Visible = false;

                this.label1.Visible = false;
                this.label2.Visible = false;
                this.label3.Visible = false;
                this.label4.Visible = false;
                this.label5.Visible = false;
                this.label6.Visible = false;
                this.label7.Visible = false;
                this.label8.Visible = false;
                this.label9.Visible = false;
                this.label10.Visible = false;
                this.label11.Visible = false;
                this.label12.Visible = false;
                this.label13.Visible = false;
                this.label14.Visible = false;
                this.label15.Visible = false;
                this.label16.Visible = false;

                this.labelAlfabet.Visible = false;
                this.tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Percent;
                this.tableLayoutPanel1.RowStyles[1].Height = 4;
                this.panzaAutomat.Height = this.splitContainer1.Height;
                this.panel1.Height = this.panzaAutomat.Height - 5;
            }
            vedemButoanele = !vedemButoanele;
        }

        // se apeleaza cand dam click pe picture box
        void panzaAutomat_MouseClick(object sender, MouseEventArgs e)
        {
            if (!panzaAutomat.ContainsFocus)
                panel1.Focus();
            panzaAutomat.Cursor = Cursors.Default;

            if (listaStari.Count > 0)
                for (int i = 0; i < listaStari.Count; i++)
                    listaStari[i].BackColor = Color.FromArgb(204, 229, 255);

            idxStareSelectata = -1; // cand dam click pe plansa de lucru, sa nu mai fie selectata nicio stare
            if (listaStariSelectate.Count > 0) listaStariSelectate.Clear();

            if (cbEnableLegatura.Checked || cbDisableLegatura.Checked)
                ActiuneEscape();
        }

        // se apeleaza cand dam click pe meniul Help
        void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "help.chm");
        }

        private void CreeazaLabelEpsilon()
        {
            labelEpsilon = new Label();
            labelEpsilon.Text = "ε";
            labelEpsilon.Font = new Font("Microsoft Sans Serif", 30.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelEpsilon.TextAlign = ContentAlignment.MiddleCenter;
            labelEpsilon.ForeColor = Color.RoyalBlue;
            toolTip1.SetToolTip(labelEpsilon, "Apasa pentru a copia Epsilon (ALT+E)");
            // atasam evenimentele la labelul Epsilon
            labelEpsilon.MouseEnter += new EventHandler(labelEpsilon_MouseEnter);
            labelEpsilon.MouseLeave += new EventHandler(labelEpsilon_MouseLeave);
            labelEpsilon.MouseClick += new MouseEventHandler(labelEpsilon_MouseClick);
            labelEpsilon.MouseMove += new MouseEventHandler(labelEpsilon_MouseMove);
            labelEpsilon.GiveFeedback += new GiveFeedbackEventHandler(labelEpsilon_GiveFeedback);
            // schimbam cursorul sa fie manuta cand suntem deasupra labelului Epsilon
            labelEpsilon.Cursor = Cursors.Hand;
        }

        // se apeleaza in timpul operatiunii de grad&drop Epsilon
        void labelEpsilon_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            this.panzaAutomat.Cursor = cursEps;
        }

        // se apeleaza cand miscam mouse-ul pe label Epsilon
        void labelEpsilon_MouseMove(object sender, MouseEventArgs e)
        {
            // facem dragNdrop la epsilon daca exista macar o tranzitie
            if (e.Button == MouseButtons.Left && listaTranzitii.Count > 0)
            {
                labelEpsilon.DoDragDrop(this.labelEpsilon.Text, DragDropEffects.Copy);
            }
        }

        // se apeleaza cand dam click pe label Epsilon
        void labelEpsilon_MouseClick(object sender, MouseEventArgs e)
        {
            ActiuneEpsilon();
        }

        private void ActiuneEpsilon()
        {
            Clipboard.SetText(labelEpsilon.Text);
            statusLabel.Text = "Epsilon copiat in clipboard";
            // punem epsilon pe tranzitie cand dam click pe label epsilon
            if (listaTranzitii.Count > 0)
                for (int i = 0; i < listaTranzitii.Count; i++)
                    if (listaTranzitii[i].GetTbTranz.Focused)
                    {
                        if (listaTranzitii[i].GetTbTranz.Text != "")
                            if (listaTranzitii[i].GetTbTranz.Text.EndsWith(","))
                                listaTranzitii[i].GetTbTranz.Text += labelEpsilon.Text;
                            else listaTranzitii[i].GetTbTranz.Text += "," + labelEpsilon.Text;
                        else
                            listaTranzitii[i].GetTbTranz.Text = labelEpsilon.Text;
                    }
        }

        // se apeleaza cand mouse-ul paraseste conturul labelului Epsilon
        void labelEpsilon_MouseLeave(object sender, EventArgs e)
        {
            labelEpsilon.Font = new Font(labelEpsilon.Font.Name, labelEpsilon.Font.SizeInPoints, FontStyle.Regular);
            labelEpsilon.BackColor = this.BackColor;
        }

        // se apeleaza cand mouse-ul intra pe terioriul labelului Epsilon
        void labelEpsilon_MouseEnter(object sender, EventArgs e)
        {
            labelEpsilon.Font = new Font(labelEpsilon.Font.Name, labelEpsilon.Font.SizeInPoints, FontStyle.Underline | FontStyle.Bold);
            labelEpsilon.BackColor = Color.Gold;
        }

        private Color GetRandomColor()
        {
            return Color.FromArgb(r.Next(0, 230), r.Next(0, 230), r.Next(0, 230));
        }

        private Color GetCuloareContrast(Color crandom)
        {
            int c = 0;
            // calculam luminanta
            double a = 1 - (0.299 * crandom.R + 0.587 * crandom.G + 0.114 * crandom.B) / 255;
            if (a < 0.5) // culori deschise, font negru
                c = 0;
            else
                c = 255; // culori inchise, font alb
            return Color.FromArgb(c, c, c);
        }

        //  se apeleaza cand dam click pe labelurile din dreapta jos (colorate)
        private void LabelCuloare_Click(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            // daca avem vreo stare selectata
            if (idxStareSelectata >= 0)
            {
                listaStari[idxStareSelectata].Culoare = lbl.BackColor;
                listaStari[idxStareSelectata].Invalidate();
            }
            if (listaStariSelectate.Count > 0)
            {
                for (int t = 0; t < listaStariSelectate.Count; t++)
                {
                    listaStariSelectate[t].Culoare = lbl.BackColor;
                    listaStariSelectate[t].Invalidate();
                }

            }
            culoareCurenta = lbl.BackColor;
            statusLabel.ForeColor = culoareCurenta;
            statusLabel.Text = "Culoare selectata: " + culoareCurenta.ToString();
        }

        // se apeleaza cand se invoca metoda Invalidate() sau oricand se face redesenare la pictureBox
        // aici se deseneaza curbele Bezier ale tranzitiilor si se pun labelurile tranzitiilor 
        private void panzaAutomat_Paint(object sender, PaintEventArgs e)
        {
            // selectia mai multor stari, deseneaza dreptunghi albastru punctat de selectie
            if (mouseDownPanza)
            {
                using (Pen pen = new Pen(Color.RoyalBlue, 1))
                {
                    pen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, dreptunghiSelectie);
                }
            }

            try
            {
                // daca avem macar o legatura
                if (listaTranzitii.Count > 0)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    // distanta dintre 2 stari
                    double distantaDouaStari = 0;
                    // puncte utile pentru trasarea legaturilor
                    Point pLegStareStart, pLegStareStop, pControl;
                    pLegStareStart = Point.Empty;
                    pLegStareStop = Point.Empty;
                    pControl = Point.Empty;

                    int centruStartX = 0, centruStartY = 0, centruStopX = 0, centruStopY = 0;
                    int mijlocX = 0, mijlocY = 0;
                    double razaDistX = 0;
                    int distCentruX = 0, distCentruY = 0;
                    double ddist = 0;

                    Point p2Control = Point.Empty;
                    Point pRotStart = Point.Empty, pRotStop = Point.Empty;

                    for (int t = 0; t < listaTranzitii.Count; t++)
                    {
                        // daca starea de start coincide cu cea de stop
                        if (listaTranzitii[t].IndexStareStart == listaTranzitii[t].IndexStareStop)
                        {
                            pLegStareStart.X = listaStari[listaTranzitii[t].IndexStareStart].Location.X + 15;
                            pLegStareStart.Y = listaStari[listaTranzitii[t].IndexStareStart].Location.Y + listaStari[listaTranzitii[t].IndexStareStart].Height / 4 - 10;
                            pLegStareStop.X = listaStari[listaTranzitii[t].IndexStareStart].Location.X + listaStari[listaTranzitii[t].IndexStareStart].Width - 5;
                            pLegStareStop.Y = listaStari[listaTranzitii[t].IndexStareStart].Location.Y + listaStari[listaTranzitii[t].IndexStareStart].Height / 4;

                            pControl.X = pLegStareStart.X - 30;
                            pControl.Y = pLegStareStart.Y - 60;
                            p2Control.X = pLegStareStop.X + 30;
                            p2Control.Y = pLegStareStart.Y - 60;

                            listaTranzitii[t].Left = pLegStareStart.X;
                            listaTranzitii[t].Top = pControl.Y - 15;

                            e.Graphics.DrawBezier(listaTranzitii[t].PenTranzitie, pLegStareStart, pControl, p2Control, pLegStareStop);
                        }
                        else
                        {
                            // centrele starilor de start si de stop
                            centruStartX = listaStari[listaTranzitii[t].IndexStareStart].Location.X + razaStare;
                            centruStartY = listaStari[listaTranzitii[t].IndexStareStart].Location.Y + razaStare;
                            centruStopX = listaStari[listaTranzitii[t].IndexStareStop].Location.X + razaStare;
                            centruStopY = listaStari[listaTranzitii[t].IndexStareStop].Location.Y + razaStare;

                            // punctul de start al tranzitiei
                            pLegStareStart.X = centruStartX;
                            pLegStareStart.Y = centruStartY;

                            // distantele pe Ox si Oy dintre centrele starilor de start si stop
                            distCentruX = centruStopX - centruStartX;
                            distCentruY = centruStopY - centruStartY;
                            distantaDouaStari = Math.Sqrt(distCentruX * distCentruX + distCentruY * distCentruY); ;

                            razaDistX = 1 - razaStare / distantaDouaStari;
                            // punctul de stop al unei tranzitii
                            pLegStareStop.X = (int)(centruStartX + razaDistX * distCentruX);
                            pLegStareStop.Y = (int)(centruStartY + razaDistX * distCentruY);

                            mijlocX = (centruStartX + centruStopX) / 2;
                            mijlocY = (centruStartY + centruStopY) / 2;

                            distCentruX = centruStopX - mijlocX;
                            distCentruY = centruStopY - mijlocY;

                            distantaDouaStari = Math.Sqrt(distCentruX * distCentruX + distCentruY * distCentruY);
                            ddist = razaStare / distantaDouaStari;
                            pControl.X = (int)(mijlocX + ddist * distCentruY);
                            pControl.Y = (int)(mijlocY - ddist * distCentruX);

                            ddist = (razaStare + listaTranzitii[t].Width / 2) / distantaDouaStari;
                            listaTranzitii[t].Left = (int)(mijlocX + ddist * distCentruY - listaTranzitii[t].Width / 2);
                            if (listaStari[listaTranzitii[t].IndexStareStart].Location.X <= listaStari[listaTranzitii[t].IndexStareStop].Location.X)
                                listaTranzitii[t].Top = (int)(mijlocY - ddist * distCentruX);
                            else
                                listaTranzitii[t].Top = (int)(mijlocY - ddist * distCentruX - listaTranzitii[t].Height);

                            e.Graphics.DrawBezier(listaTranzitii[t].PenTranzitie, pLegStareStart, pControl, pControl, pLegStareStop);
                        }
                    }
                }
            }
            catch (OverflowException ex)
            {
                // apare la urmatorul scenariu: avem 2 stari cu o tranzitie intre ele si le suprapunem complet
                Console.WriteLine(ex.ToString());
            }
        }

        // se apeleaza imediat dupa crearea formului
        private void FormHome_Load(object sender, EventArgs e)
        {
            // adaugam toate uneltele din meniul din stanga
            toolStrip_baraMeniu.Items.Add(stareNeaccStartItem);
            toolStrip_baraMeniu.Items.Add(stareNeaccItem);
            toolStrip_baraMeniu.Items.Add(stareAccStartItem);
            toolStrip_baraMeniu.Items.Add(stareAccMenuItem);
            toolStrip_baraMeniu.Items.Add(new ToolStripControlHost(labelEpsilon));
            toolStrip_baraMeniu.Items.Add(new ToolStripControlHost(cbEnableLegatura));
            toolStrip_baraMeniu.Items.Add(new ToolStripControlHost(cbDisableLegatura));
        }

        // se apeleaza cand facem drag&drop la stari pe picture box si starea 
        // a intrat pe terioriul picture box-ului
        private void panzaAutomat_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        // se apeleaza cand facem drag&drop la stari pe picture box
        private void panzaAutomat_DragDrop(object sender, DragEventArgs e)
        {
            CreeazaStare(e, "", -1, false, false, Point.Empty, Color.Empty);
            statusLabel.Text = "Construire automat";
            amImportatXML = false; // s-a dus efectul importului, deja prelucram "manual" automatul
            Cursor.Clip = Rectangle.Empty;
        }

        // se apeleaza cand miscam mouse-ul deasupra unei stari
        void stareAdaugata_MouseMove(object sender, MouseEventArgs e)
        {
            // facem refresh la picture box numai daca butonul mouse-ului este apasat
            if ((Control.MouseButtons & MouseButtons.Left) != 0)
            {
                this.panzaAutomat.Invalidate();
            }
        }

        // se apeleaza cand suntem cu mouse-ul apasat deasupra unei stari
        void stareAdaugata_MouseDown(object sender, MouseEventArgs e)
        {
            // vad ce tip de stare este 
            StareNeacceptoare stn = null;
            if (sender is StareAcceptoare)
                stn = sender as StareAcceptoare;
            else if (sender is StareNeacceptoare)
                stn = sender as StareNeacceptoare;
            else if (sender is StareAcceptoareStart)
                stn = sender as StareAcceptoareStart;
            else stn = sender as StareNeacceptoareStart;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        stn.Tag = "Apasata";
                        idxStareSelectata = listaStari.IndexOf(stn);
                        listaStari[idxStareSelectata].BringToFront();
                        if (rtbConsola.Visible)
                            if (listaStari[idxStareSelectata].NumeStare != "tux" && listaStari[idxStareSelectata].NumeStare != "")
                                AdaugaTextColoratConsola("[+] Am selectat starea " + listaStari[idxStareSelectata].NumeStare + "\n", Color.Gold);
                            else
                                AdaugaTextColoratConsola("[+] Construire automat\n", rtbConsola.ForeColor);
                        if (listaStari[idxStareSelectata].NumeStare != "tux" && listaStari[idxStareSelectata].NumeStare != "")
                            statusLabel.Text = "Stare selectata: " + listaStari[idxStareSelectata].NumeStare;
                        else
                            statusLabel.Text = "Construire automat";

                        if (cbEnableLegatura.Checked)
                        {
                            numarClicks++;
                            if (numarClicks == 1)
                            {
                                idxStart = listaStari.IndexOf(stn);
                            }
                            else if (numarClicks == 2)
                            {
                                idxStop = listaStari.IndexOf(stn);

                                CreeazaTranzitie(this.idxStart, this.idxStop, this.culoareCurenta, "");

                                cbEnableLegatura.Checked = false;
                                numarClicks = 0;
                            }
                        }
                        else if (cbDisableLegatura.Checked)
                        {
                            numarClicks++;
                            if (numarClicks == 1)
                            {
                                idxStart = listaStari.IndexOf(stn);
                            }
                            else if (numarClicks == 2)
                            {
                                idxStop = listaStari.IndexOf(stn);

                                for (int t = 0; t < listaTranzitii.Count; t++)
                                {
                                    if (listaTranzitii[t].IndexStareStart == idxStart && listaTranzitii[t].IndexStareStop == idxStop)
                                    {
                                        if (rtbConsola.Visible) AdaugaTextColoratConsola("[+] Am sters tranzitia " + listaTranzitii[t].ToString() + "\n", rtbConsola.ForeColor);
                                        statusLabel.Text = "Tranzitie stearsa: " + listaTranzitii[t].ToString();
                                        listaTranzitii.Remove(listaTranzitii[t]);
                                    }
                                }
                                // daca am sters o tranzitie, trebuie sa o stergem si de picture box
                                var toateTranzitiile = panzaAutomat.Controls.OfType<Tranzitie>().ToList();
                                foreach (Tranzitie t in toateTranzitiile)
                                    if (!listaTranzitii.Contains(t))
                                        panzaAutomat.Controls.Remove(t);
                                cbDisableLegatura.Checked = false;
                                numarClicks = 0;
                            }
                        }

                        // facem starea selectata galbena
                        for (int t = 0; t < listaStari.Count; t++)
                            if (t != idxStareSelectata)
                                listaStari[t].BackColor = Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
                            else
                                listaStari[t].BackColor = Color.Gold;

                        if (stn.MutamStarea)
                        {
                            stn.PozitieX = e.X;
                            stn.PozitieY = e.Y;
                            stn.MutamStarea = false;
                        }
                        break;
                    }
                case MouseButtons.Right:
                    break;
            }
        }

        // se apeleaza cand dam click pe o tranzitie
        void tranz_Click(object sender, EventArgs e)
        {
            Tranzitie t = sender as Tranzitie;
            t.SchimbaCuloarea(culoareCurenta, 2);
            panzaAutomat.Invalidate();
        }

        // se apeleaza cand apasam o tasta si suntem cu mouse-ul pe stare
        void stareAdaugata_KeyUp(object sender, KeyEventArgs e)
        {
            StareNeacceptoare stn = null;
            if (sender is StareAcceptoare)
                stn = sender as StareAcceptoare;
            else if (sender is StareNeacceptoare)
                stn = sender as StareNeacceptoare;
            else if (sender is StareAcceptoareStart)
                stn = sender as StareAcceptoareStart;
            else stn = sender as StareNeacceptoareStart;

            if (!butOpen.Enabled) e.Handled = true; // nu avem voie sa stergem stari in timpul simularii si stim ca butonul de open e disabled atunci :)
            else
                // stergem o stare cu tasta DELETE daca am dat click pe stare si daca starea se afla in panou, nu in meniu
                if (e.KeyCode == Keys.Delete && stn.Parent == panzaAutomat && panzaAutomat.ClientRectangle.Contains(panzaAutomat.PointToClient(Cursor.Position)))
                {
                    ActiuneStergeStare(stn);
                }
        }

        public void ActiuneStergeStare(StareNeacceptoare stn)
        {
            // daca avem macar o tranzitie
            if (listaTranzitii.Count > 0)
            {
                // le stergem pe cele care vin si pleaca din starea pe care o stergem: stn
                listaTranzitii.RemoveAll(s => s.IndexStareStart == listaStari.IndexOf(stn) || s.IndexStareStop == listaStari.IndexOf(stn));
                // le stergem si de pe pictureBox
                var toateTranzitiile = panzaAutomat.Controls.OfType<Tranzitie>().ToList();
                foreach (Tranzitie t in toateTranzitiile)
                    if (!listaTranzitii.Contains(t))
                        panzaAutomat.Controls.Remove(t);

                // trebuie sa devansam cu 1 indecsii de dupa starea stearsa
                for (int t = 0; t < listaTranzitii.Count; t++)
                {
                    if (listaTranzitii[t].IndexStareStart > listaStari.IndexOf(stn))
                        listaTranzitii[t].IndexStareStart -= 1;
                    if (listaTranzitii[t].IndexStareStop > listaStari.IndexOf(stn))
                        listaTranzitii[t].IndexStareStop -= 1;
                }
            }
            if (rtbConsola.Visible)
                AdaugaTextColoratConsola((stn.NumeStare == "tux" || stn.NumeStare == "" ? "" : "[+] Am sters starea " + stn.NumeStare) + "\n", rtbConsola.ForeColor);
            statusLabel.Text = (stn.NumeStare == "tux" || stn.NumeStare == "" ? "Ready" : "Am sters starea " + stn.NumeStare);

            listaStari.Remove(stn);
            panzaAutomat.Controls.Remove(stn);
            stn.Dispose();
            // chemam repaint pe pictureBox
            panzaAutomat.Invalidate();
        }

        // se apeleaza cand bifam/debifam checkbox-ul DisableLegatura
        void cbDisableLegatura_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Checked) cbEnableLegatura.Enabled = false;
            else cbEnableLegatura.Enabled = true;
            amImportatXML = false;
        }

        // se apeleaza cand bifam/debifam checkbox-ul EnableLegatura
        void cbEnableLegatura_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Checked) cbDisableLegatura.Enabled = false;
            else cbDisableLegatura.Enabled = true;
            amImportatXML = false;
        }

        // funtie ce compileaza automatul
        private string CompilareAutomat()
        {
            string s = "";
            int cntStart = 0;
            bool okNume = true;

            // daca avem stari si macar o tranzitie intre ele
            if (listaStari.Count > 0 && listaTranzitii.Count > 0)
            {
                for (int t = 0; t < listaStari.Count; t++)
                {
                    if (listaStari[t].EsteDeStart)
                        cntStart++;
                    if (listaStari[t].NumeStare == "tux" || listaStari[t].NumeStare == "")
                        okNume = false;
                }

                // trebuie sa avem nume dat fiecarei stari
                if (!okNume)
                {
                    s = "Nu ati dat nume starilor!";
                    return s;
                }

                // trebuie sa avem o singura stare de start
                if (cntStart != 1)
                {
                    s = "Nicio stare de start sau prea multe stari de start pentru automatul construit";
                    return s;
                }

                // trebuie sa avem macar o stare acceptoare
                if (!listaStari.Any(st => st.Acceptoare))
                {
                    s = "Nu exista nicio stare acceptoare pentru automatul construit!";
                    return s;
                }
                s = "Compilare reusita!";
                return s;
            }
            s = "Atentie! Automat neconstruit!";
            return s;

        }

        // metoda ce parcurge lista de tranzitii si preia simbolurile automatului
        private void PreiaAlfabet()
        {
            char c = '\0';
            List<char> alfTemp = new List<char>();

            int j = 0;

            for (int t = 0; t < listaTranzitii.Count; t++)
            {
                for (j = 0; j < listaTranzitii[t].GetSimboluriTranzitie().Count; j++)
                {
                    c = listaTranzitii[t].GetSimboluriTranzitie()[j];
                    alfTemp.Add(c);
                }
            }
            alfabet = alfTemp.Distinct().ToList();
            alfabet.Sort();

            alfTemp = null;
        }

        // metoda pe care o apelam cand apasam pe butonul de Compilare sau pe meniul de Compilare
        private bool ActiuneCompilare()
        {
            string s = CompilareAutomat();
            if (s != "Compilare reusita!")
            {
                rtbInput.Enabled = false;
                rtbInput.Visible = false;

                simulareToolStripMenuItem.Enabled = false;
                butSimulare.Enabled = false;
                butPas.Enabled = false;
                butStop.Enabled = false;

                nrClicksSim = 0;

                rtbInput.Clear();
                if (rtbConsola.Visible)
                    AdaugaTextColoratConsola("[+] " + s + "\n", Color.Red);
                statusLabel.Text = s;
                MessageBox.Show(s, "Eroare!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // aici preluam alfabetul, dupa compilarea reusita, la inceput de simulare
            PreiaAlfabet();

            labelAlfabet.Text = "Alfabetul automatului: ";
            for (int i = 0; i < alfabet.Count; i++)
                labelAlfabet.Text += alfabet[i] + " ";
            labelAlfabet.Visible = true;

            // si facem enabled butoanele de simulare si input box
            butPas.Enabled = true;
            butSimulare.Enabled = true;
            butStop.Enabled = true;
            butSaveImg.Enabled = true;
            butSaveXML.Enabled = true;

            simulareToolStripMenuItem.Enabled = true;
            exportXMLToolStripMenuItem.Enabled = true;
            exportToolStripMenuItem.Enabled = true;

            rtbInput.Enabled = true;
            rtbInput.Visible = true;

            bool esteDet = EsteDeterminist();

            if (rtbConsola.Visible) AdaugaTextColoratConsola("[+]" + s + " Automatul construit este " + (esteDet ? "determinist" : "nedeterminist") + Environment.NewLine, Color.AliceBlue);
            statusLabel.Text = s + " Automatul construit este " + (esteDet ? "determinist" : "nedeterminist");
            MessageBox.Show(s + " Automatul construit este " + (esteDet ? "determinist" : "nedeterminist"), "Succes!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }

        // se apeleaza cand dam click pe butonul de compilare
        private void butCompilare_Click(object sender, EventArgs e)
        {
            ActiuneCompilare();
        }
        // se apeleaza cand dam click pe meniul compilare
        private void compilareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActiuneCompilare();
        }

        private void ActiuneSimulare()
        {
            if (oData)
            {
                toolStrip_baraMeniu.Enabled = false; // nu putem adauga stari in timpul simularii
                rtbInput.ReadOnly = true; // nu putem modifica stringul in timpul simularii

                // aici cream vectorul stareCurenta
                suntemAici = new bool[listaStari.Count];
                for (int t = 0; t < suntemAici.Length; t++) suntemAici[t] = false;

                // si vectorul stare viitoare
                vomFiAici = new bool[listaStari.Count];
                for (int t = 0; t < vomFiAici.Length; t++) vomFiAici[t] = false;

                // si vectorul de culori in care stocam culorile initiale ale fiecarei stari
                for (int t = 0; t < listaStari.Count; t++)
                    listaCuloriStare.Insert(t, listaStari[t].Culoare);

                if (rtbConsola.Visible)
                    AdaugaTextColoratConsola("[+] Incepem simularea\n", Color.Yellow);
                statusLabel.Text = "Incepem simularea";

                areDoarAlfa = AreSimboluriDoarDinAlfabet();
                oData = false;
            }

            if (rtbInput.Text != "")
            {
                // vedem daca stringul dat ca intrare are numai simboluri din alfabet
                if (areDoarAlfa)
                    simGata = SimuleazaAutomat(rtbInput.Text, nrClicksSim);
                else
                {
                    timer1.Stop();

                    MessageBox.Show("Stringul nu respecta alfabetul automatului", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (rtbConsola.Visible)
                        AdaugaTextColoratConsola("[+] Stringul nu respecta alfabetul automatului\n", Color.Red);
                    statusLabel.Text = "Stringul nu respecta alfabetul automatului";
                    importaToolStripMenuItem.Enabled = true;
                    butOpen.Enabled = true;

                    rtbInput.ReadOnly = false;
                    nrClicksSim = -1; // daca stringul introdus nu corespunde cu alfabetul, 
                    // resetam nr de click-uri date pe butonul de simulare
                    oData = true; // trebuie sa reluam testarea daca are doar simboluri din alfabet
                    ReseteazaInterfata();
                }
                if (nrClicksSim == rtbInput.Text.Length)
                {
                    timer1.Stop();
                    if (simGata)
                    {
                        if (rtbConsola.Visible)
                            AdaugaTextColoratConsola("[+] String acceptat: " + rtbInput.Text + Environment.NewLine, Color.White);
                        statusLabel.Text = "String acceptat: " + rtbInput.Text;
                        MessageBox.Show("String acceptat", "Felicitari", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (rtbConsola.Visible)
                            AdaugaTextColoratConsola("[+] String neacceptat: " + rtbInput.Text + Environment.NewLine, Color.White);
                        statusLabel.Text = "String neacceptat: " + rtbInput.Text;
                        MessageBox.Show("String neacceptat", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    ReseteazaInterfata();
                }
            }
            else
            {
                MessageBox.Show("Intrarea este nula", "Atentie!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                statusLabel.Text = "Intrarea este nula";
                butSimulare.Enabled = true;
                butStop.Enabled = true;
                rtbInput.ReadOnly = false;
                nrClicksSim = -1;
                oData = true;
                ReseteazaInterfata();
            }
            nrClicksSim++;
        }

        // se intampla daca apasam butonul de simulare pas cu pas
        private void butPas_Click(object sender, EventArgs e)
        {
            butSimulare.Enabled = false;
            butOpen.Enabled = false;
            importaToolStripMenuItem.Enabled = false;
            simulareToolStripMenuItem.Enabled = false;
            butStop.Enabled = false;
            butStergeBox.Enabled = false;
            ActiuneSimulare();
        }

        // functie ce simuleaza automatul
        private bool SimuleazaAutomat(string input, int index)
        {
            if (!listaStari.Any(st => st.Acceptoare))
            {
                MessageBox.Show("Nicio stare acceptoare pentru automat!", "Eroare!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ReseteazaInterfata();
                return false;
            }
            else
            {
                List<char> simbTranz = null;
                for (int t = 0; t < vomFiAici.Length; t++)
                    vomFiAici[t] = false;
                statusStare = "Stare curenta: ";

                if (index < input.Length)
                {
                    // pornim din starea de start
                    int t = 0, j = 0, k = 0;
                    int start = -1;
                    if (suntemStart)
                    {
                        start = GasesteStareDeStart();
                        if (start != -1)
                        {
                            suntemAici[start] = true;
                            listaStari[start].Culoare = Color.Red;

                            // calculam inchiderea epsilon a starii de start astfel:
                            // daca gasim vreo tranzitie care incepe din starea de start,
                            // preluam simbolurile de pe ea
                            // daca lista de simboluri contine epsilon,
                            // inseamna ca si celelalte stari sunt de start
                            for (t = 0; t < listaTranzitii.Count; t++)
                                if (listaTranzitii[t].IndexStareStart == start)
                                {
                                    simbTranz = listaTranzitii[t].GetSimboluriTranzitie();
                                    if (simbTranz.Contains('\u03B5')) // epsilon unicode
                                    {
                                        suntemAici[listaTranzitii[t].IndexStareStop] = true;
                                        listaStari[listaTranzitii[t].IndexStareStop].AjungemPrinEpsilon = true; // ajungem aici prin epsilon
                                    }
                                }

                            if (rtbConsola.Visible && suntemAici[start]) // suntem la start numai la inceputul simularii, apoi suntemAici[start] devine fals
                                AdaugaTextColoratConsola("[+] Suntem in starea " + listaStari[start].NumeStare + "\n", Color.Yellow);
                            statusLabel.Text = "Stare: " + listaStari[start].NumeStare + " ";
                            suntemStart = false;
                        }
                        else
                        {
                            MessageBox.Show("Eroare! Nu exista stare de start", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ReseteazaInterfata();
                            return false;
                        }
                    }

                    // parcurgem tranzitiile si facem inchiderea epsilon pt fiecare stare
                    for (t = 0; t < listaTranzitii.Count; t++)
                        // vedem daca suntem intr-o stare de start a unei tranzitii
                        if (suntemAici[listaTranzitii[t].IndexStareStart])
                        {
                            // preluam simbolurile de pe arc (tranzitie)
                            simbTranz = listaTranzitii[t].GetSimboluriTranzitie();
                            // parcurgem lista de simboluri de pe tranzitie
                            for (j = 0; j < simbTranz.Count; j++)
                            {
                                // facem inchiderea epsilon pt stare
                                if (simbTranz[j] == '\u03B5')
                                {
                                    panzaAutomat.Invalidate();
                                    // starea viitoare va fi starea de stop a tranzitiei
                                    suntemAici[listaTranzitii[t].IndexStareStop] = true;
                                    //statusStare += listaStari[listaTranzitii[t].IndexStareStop].NumeStare + " ";
                                    listaStari[listaTranzitii[t].IndexStareStop].AjungemPrinEpsilon = true;
                                }
                            }
                        }

                    for (t = 0; t < listaTranzitii.Count; t++)
                        // vedem daca suntem intr-o stare de start a unei tranzitii
                        if (suntemAici[listaTranzitii[t].IndexStareStart])
                        {
                            simbTranz = listaTranzitii[t].GetSimboluriTranzitie();
                            for (j = 0; j < simbTranz.Count; j++)
                                // daca simbolul curent din cuvant se afla pe tranzitie 
                                if (input[index] == simbTranz[j])
                                {
                                    panzaAutomat.Invalidate();
                                    // starea viitoare va fi starea de stop a tranzitiei
                                    vomFiAici[listaTranzitii[t].IndexStareStop] = true;
                                    //  statusStare += listaStari[listaTranzitii[t].IndexStareStop].NumeStare + " ";
                                    listaStari[listaTranzitii[t].IndexStareStop].AjungemPrinEpsilon = false;
                                }
                        }

                    for (t = 0; t < listaStari.Count; t++)
                        if (vomFiAici[t] || (suntemAici[t] && listaStari[t].AjungemPrinEpsilon))
                        {
                            listaStari[t].Culoare = Color.Red;
                            listaStari[t].Invalidate();

                            // vedem ce tranzitii ajung in starea listaStari[t] si le coloram daca au pe ele epsilon sau simbolul curent
                            for (k = 0; k < listaTranzitii.Count; k++)
                            {
                                // daca tranzitia ajunge in starea listaStari[t]
                                if (listaTranzitii[k].IndexStareStop == t && suntemAici[listaTranzitii[k].IndexStareStart])

                                    if (listaTranzitii[k].GetSimboluriTranzitie().Contains(input[index]) || listaTranzitii[k].GetSimboluriTranzitie().Contains('\u03B5'))
                                    {
                                        culoareSimulare = GetRandomColor();
                                        // culoareSimNeg = Color.FromArgb(255 - culoareSimulare.R, 255 - culoareSimulare.G, 255 - culoareSimulare.B);
                                        culoareSimNeg = GetCuloareContrast(culoareSimulare);
                                        listaTranzitii[k].SchimbaCuloarea(culoareSimulare, 2);
                                        panzaAutomat.Invalidate();
                                    }
                            }

                            if (rtbConsola.Visible)
                                AdaugaTextColoratConsola("[+] Suntem in starea " + listaStari[t].NumeStare + "\n", culoareSimulare);
                            statusStare += listaStari[t].NumeStare + " ";
                            statusStrip1.BackColor = culoareSimulare;
                            statusLabel.ForeColor = culoareSimNeg;
                            statusLabel.Text = statusStare;
                        }
                        else
                        {
                            listaStari[t].Culoare = listaCuloriStare[t]; // pictam starea in culoarea initiala
                            listaStari[t].Invalidate();
                        }

                    rtbInput.Select(index, 1);
                    rtbInput.SelectionColor = Color.Red;

                    // starea viitoare devine apoi stare curenta
                    Array.Copy(vomFiAici, suntemAici, vomFiAici.Length);

                    // daca suntem intr-o stare acceptoare, stringul nu s-a terminat si nu mai avem tranzitii
                    // care pleaca din starea acceptoare
                    // stringul nu este acceptat
                    for (int idxSim = 0; idxSim < listaStari.Count; idxSim++)
                        if (listaStari[idxSim].Acceptoare)
                            if (vomFiAici[idxSim])
                                if (!listaTranzitii.Exists(tr => tr.IndexStareStart == idxSim))
                                {
                                    //listaStari[idxSim].Culoare = listaCuloriStare[idxSim];
                                    //listaStari[idxSim].Invalidate();
                                    return false;
                                }
                }
                else
                {
                    int idxSim = 0;
                    int it = 0;

                    // daca s-a terminat stringul, facem inchiderea epsilon a ultimei stari
                    for (idxSim = 0; idxSim < listaStari.Count; idxSim++)
                        if (suntemAici[idxSim])
                            for (it = 0; it < listaTranzitii.Count; it++)
                                if (listaTranzitii[it].IndexStareStart == idxSim)
                                {
                                    simbTranz = listaTranzitii[it].GetSimboluriTranzitie();
                                    if (simbTranz.Contains('\u03B5'))
                                    {
                                        suntemAici[listaTranzitii[it].IndexStareStop] = true;
                                        listaTranzitii[it].SchimbaCuloarea(culoareSimulare, 2);
                                        listaStari[listaTranzitii[it].IndexStareStop].Culoare = Color.Red;
                                        listaStari[listaTranzitii[it].IndexStareStop].Invalidate();
                                        listaStari[listaTranzitii[it].IndexStareStop].AjungemPrinEpsilon = true;
                                        if (rtbConsola.Visible)
                                            AdaugaTextColoratConsola("[+] Suntem in starea " + listaStari[listaTranzitii[it].IndexStareStop].NumeStare + "\n", culoareSimulare);
                                        statusStare += " " + listaStari[listaTranzitii[it].IndexStareStop].NumeStare;
                                        panzaAutomat.Invalidate();
                                    }
                                }
                    statusLabel.Text = statusStare;
                    // vedem daca suntem intr-o stare acceptoare
                    for (idxSim = 0; idxSim < listaStari.Count; idxSim++)
                        if (!suntemAici[idxSim])
                        {
                            listaStari[idxSim].Culoare = listaCuloriStare[idxSim];
                            //listaStari[idxSim].AjungemPrinEpsilon = false;
                            listaStari[idxSim].Invalidate();
                        }
                        else if (listaStari[idxSim].Acceptoare)
                        {
                            if (!suntemAici[idxSim])
                            {
                                listaStari[idxSim].Culoare = listaCuloriStare[idxSim];
                                listaStari[idxSim].AjungemPrinEpsilon = false;
                                listaStari[idxSim].Invalidate();
                            }
                            else
                            {
                                return true;
                            }
                        }
                }
                return false;
            }
        }

        private bool AreSimboluriDoarDinAlfabet()
        {
            int t = 0;
            for (t = 0; t < rtbInput.Text.Length; t++)
                if (!alfabet.Contains(rtbInput.Text[t])) return false;
            return true;
        }

        // se apeleaza cand dam click pe meniul iesire
        private void iesireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int GasesteStareDeStart()
        {
            for (int t = 0; t < listaStari.Count; t++)
                if (listaStari[t].EsteDeStart) return t;
            return -1;
        }

        // seteaza lucrurile pt o noua simulare
        private void ReseteazaInterfata()
        {
            if (!butOpen.Enabled) butOpen.Enabled = true;
            if (!butPas.Enabled) butPas.Enabled = true;
            if (!butSimulare.Enabled) butSimulare.Enabled = true;
            if (!butStop.Enabled) butStop.Enabled = true;
            if (!butSaveImg.Enabled) butSaveImg.Enabled = true;
            if (!butSaveXML.Enabled) butSaveXML.Enabled = true;
            if (!butStergeBox.Enabled) butStergeBox.Enabled = true;
            if (rtbInput.ReadOnly) rtbInput.ReadOnly = false;
            if (!simulareToolStripMenuItem.Enabled) simulareToolStripMenuItem.Enabled = true;
            if (!exportXMLToolStripMenuItem.Enabled) exportXMLToolStripMenuItem.Enabled = true;
            if (!exportToolStripMenuItem.Enabled) exportToolStripMenuItem.Enabled = true;
            if (!importaToolStripMenuItem.Enabled) importaToolStripMenuItem.Enabled = true;

            toolStrip_baraMeniu.Enabled = true;

            statusStrip1.BackColor = Color.FromArgb(204, 229, 255);
            statusLabel.ForeColor = Color.RoyalBlue;
            rtbInput.SelectAll();
            rtbInput.SelectionColor = System.Drawing.SystemColors.MenuHighlight;
            rtbInput.Invalidate();

            oData = true;
            nrClicksSim = -1;
            suntemStart = true; // trebuie sa cautam iar stare de start
            statusLabel.Text = "Ready";

            for (int t = 0; t < listaStari.Count; t++)
            {
                listaStari[t].BackColor = Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
                listaStari[t].Culoare = listaCuloriStare[t];
                listaStari[t].Invalidate();
            }
            for (int t = 0; t < listaTranzitii.Count; t++)
            {
                listaTranzitii[t].SchimbaCuloarea(Color.RoyalBlue, 2);
                panzaAutomat.Invalidate();
            }

            // reinitializam si vectorii de stare
            for (int t = 0; t < suntemAici.Length; t++)
            {
                suntemAici[t] = false;
                vomFiAici[t] = false;
            }
        }

        // asociata la nivel de form pt evenimentele legate de taste
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // daca apasam pe F2
            if (keyData == Keys.F2)
            {
                // bifeaza/debifeaza CheckBox-ul pentru construirea tranzitiei
                cbEnableLegatura.Checked = !cbEnableLegatura.Checked;
                return true;
            }
            // daca apasam SHIFT si F2
            if (keyData == (Keys.Shift | Keys.F2))
            {
                // bifeaza/debifeaza CheckBox-ul pentru stergerea tranzitiei
                cbDisableLegatura.Checked = !cbDisableLegatura.Checked;
                return true;
            }
            // daca apasam pe F7
            if (keyData == Keys.F7)
            {
                // simuleaza automatul pas cu pas
                ActiuneSimulare();
                return true;
            }
            // daca apasam ALT si C
            if (keyData == (Keys.Alt | Keys.C))
            {
                // sterge tot de pe PictureBox
                ActiuneStergeBox();
                return true;
            }
            // daca apasam ALT si E
            if (keyData == (Keys.Alt | Keys.E))
            {
                // copiaza Epsilon in clipboard
                ActiuneEpsilon();
                return true;
            }
            // daca apsam Escape
            if (keyData == Keys.Escape)
            {
                // anulam crearea sau stergerea unei tranzitii
                ActiuneEscape();
                return true;
            }
            // daca apasam Delete
            if (keyData == Keys.Delete)
            {
                // sterge starile selectate
                if (listaStariSelectate.Count > 0)
                    for (int t = 0; t < listaStariSelectate.Count; t++)
                        ActiuneStergeStare(listaStariSelectate[t]);
                return true;
            }
            // apeleaza functia de baza
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ActiuneEscape()
        {
            numarClicks = 0;
            if (cbEnableLegatura.Checked)
                cbEnableLegatura.Checked = false;
            if (cbDisableLegatura.Checked)
                cbDisableLegatura.Checked = false;
        }

        // export imagine
        private void ActiuneExportImagine()
        {
            if (panzaAutomat.Controls.Count > 0) // daca avem macar o stare sau o tranzitie pe panou
            {
                saveFileDialog1.Filter = "Imagini (*.png, *.jpg, *.bmp) | *.png; *.jpg; *.bmp";
                saveFileDialog1.DefaultExt = "png";
                saveFileDialog1.RestoreDirectory = false;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (listaStariSelectate.Count > 0)
                        SalveazaImgineCropped();
                    else
                        ExportFromPictureBox.ExportAsPicture(saveFileDialog1.FileName, panzaAutomat);

                    if (rtbConsola.Visible)
                        AdaugaTextColoratConsola("[+] Export reusit\n", Color.Green);
                    MessageBox.Show("Export reusit", "Succes!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (saveFileDialog1.FileName != "")
                {
                    saveFileDialog1.InitialDirectory = Path.GetDirectoryName(saveFileDialog1.FileName);
                    saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName);
                }
            }
            else MessageBox.Show("Nu este nimic de exportat!", "Atentie!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        // se apeleaza cand dam click pe butonul de export
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActiuneExportImagine();
        }

        // export xml
        private void ActiuneExportXML()
        {
            if (panzaAutomat.Controls.Count > 0) // daca avem macar o stare sau o tranzitie pe panou
            {
                saveFileDialog1.Filter = "Fisiere XML (*.xml)|*.xml";
                saveFileDialog1.DefaultExt = "xml";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ExportaAutomatXML(saveFileDialog1.FileName);
                    this.Text = "FASharpSim " + saveFileDialog1.FileName;
                    if (rtbConsola.Visible)
                        AdaugaTextColoratConsola("[+] Export reusit\n", Color.Green);
                    MessageBox.Show("Automat salvat cu succes", "Succes!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    amSalvatXML = true;
                }
                if (saveFileDialog1.FileName != "")
                {
                    saveFileDialog1.InitialDirectory = Path.GetDirectoryName(saveFileDialog1.FileName);
                    saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName);
                }
            }
            else MessageBox.Show("Nu este nimic de salvat!", "Atentie!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        // se apeleaza cand dam click pe meniul de export
        private void exportXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActiuneExportXML();
        }

        // efectueaza efectiv exportul automatului
        private void ExportaAutomatXML(string numeXML)
        {
            // cream fisierul xml, care contine automatul
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                // radacina documentului va fi tagul <automat>
                new XElement("automat", new XElement("stari", // multimea starilor
                    from stare in listaStari
                    select new XElement("stare",  // vom avea cate un tag <stare> pt fiecare stare din AF
                        new XElement("nume", stare.NumeStare), // cu elementele nume
                        new XElement("idx", listaStari.IndexOf(stare)), // idx
                        new XElement("start", stare.EsteDeStart), // start
                        new XElement("acceptoare", stare.Acceptoare), // acceptoare
                        new XElement("locatie", stare.Location), // locatie
                        new XElement("culoare", stare.Culoare.ToArgb()) // culoare
                        )
                    ),
                    new XElement("tranzitii", // multimea tranzitiilor
                    from tranz in listaTranzitii
                    select new XElement("tranzitie", // vom avea cate un tag <tranzitie> pt fiecare tranzitie din AF
                        new XElement("idxStart", tranz.IndexStareStart), // cu elementele idxStart
                        new XElement("idxStop", tranz.IndexStareStop), // idxStop
                        new XElement("culoare", tranz.PenTranzitie.Color.ToArgb()), // culoare
                        new XElement("simboluri", tranz.TextTranzitie) // simboluri
                        )
                    )
                )
            );
            xdoc.Save(saveFileDialog1.FileName, SaveOptions.None); // salvam efectiv fisierul, fara formatare
        }

        private void ActiuneDeschide()
        {
            butSimulare.Enabled = false;
            butPas.Enabled = false;
            butStop.Enabled = false;
            simulareToolStripMenuItem.Enabled = false;

            if (panzaAutomat.Controls.Count > 0) ActiuneStergeBox();

            openFileDialog1.Filter = "Fisiere XML (*.xml)|*.xml";
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.RestoreDirectory = false; // tine minte folderul deschis ultima data

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                extensie = Path.GetExtension(openFileDialog1.FileName);
                if (extensie == ".xml")
                {
                    try
                    {
                        amImportatXML = true;
                        ImportaAutomat(openFileDialog1.FileName);
                        openFileDialog1.InitialDirectory = Path.GetDirectoryName(openFileDialog1.FileName); // tine minte sa deschida ultimul folder la care am fost
                        saveFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory;
                        saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);

                        if (rtbConsola.Visible)
                            AdaugaTextColoratConsola("[+] Import reusit\n", Color.Green);
                        this.Text = "FASharpSim " + openFileDialog1.FileName;
                    }
                    catch (XmlException ex)
                    {
                        MessageBox.Show("Eroare!", ex.ToString(), MessageBoxButtons.OK);
                        amImportatXML = false;
                    }
                }
            }
            else
                amImportatXML = false;

        }

        // se apeleza cand dam click pe meniul deschide
        private void importaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActiuneDeschide();
        }

        // face efectiv importul automatului
        private void ImportaAutomat(string numeXML)
        {
            // citim fisierul xml exportat care contine automatul
            XDocument xdoc = XDocument.Load(numeXML);
            // parcurgem elementele "stare" si facem clasa anonima cu ele
            var stariTmpList = from s in xdoc.Descendants("stare")
                               select new
                               {
                                   Nume = s.Element("nume").Value,
                                   Idx = Convert.ToInt16(s.Element("idx").Value),
                                   Start = Convert.ToBoolean(s.Element("start").Value),
                                   Acceptoare = Convert.ToBoolean(s.Element("acceptoare").Value),
                                   Locatie = s.Element("locatie").Value,
                                   Culoare = s.Element("culoare").Value
                               };
            string[] punct = null; // ne ajuta sa preluam locatia starii din xml, ca ea e de forma {X=437,Y=157}

            Color argbCuloare = Color.Empty;
            // parcurgem lista de IEnumerable stariTmpList (in ea avem ce am citit din tagul stare din xml)
            foreach (var s in stariTmpList)
            {
                // inlocuim acoladele, orice litere si = cu stringul vid, apoi il "spargem" dupa , (virgula)
                // acest array va avea doar doua elemente: x si y de la locatia pe ecran a starii
                punct = Regex.Replace(s.Locatie, @"[\{\}a-zA-Z=]", "").Split(',');
                // preluam culoarea
                argbCuloare = Color.FromArgb(Int32.Parse(s.Culoare));
                // cream starea (null pt ca nu facem drag&drop)
                CreeazaStare(null, s.Nume, s.Idx, s.Start, s.Acceptoare, new Point(int.Parse(punct[0]), int.Parse(punct[1])), argbCuloare);
            }

            // parcurgem elementele tranzitie si vom face tot o clasa anonima cu ele
            var tranzTmpList = from t in xdoc.Descendants("tranzitie")
                               select new
                               {
                                   IdxStart = Convert.ToInt16(t.Element("idxStart").Value),
                                   IdxStop = Convert.ToInt16(t.Element("idxStop").Value),
                                   Culoare = t.Element("culoare").Value,
                                   Simboluri = t.Element("simboluri").Value
                               };

            foreach (var t in tranzTmpList)
            {
                argbCuloare = Color.FromArgb(Int32.Parse(t.Culoare));
                CreeazaTranzitie(t.IdxStart, t.IdxStop, argbCuloare, t.Simboluri);
            }
        }

        private void CreeazaStare(DragEventArgs e, string nume, int idx, bool start, bool acceptoare, Point locatie, Color culoare)
        {
            StareNeacceptoare stareAdaugata = null;
            StareNeacceptoare stareDragged = null;
            // daca facem drag and drop, e nu va fi null; 
            // daca luam af-ul din xml, e este null
            if (e != null)
            {
                // vedem ce tip de stare cream 
                switch (e.Data.GetData(e.Data.GetFormats()[0]).ToString())
                {
                    case "SimulatorAutomat.StareNeacceptoare":
                    case "SimulatorAutomat.StareAcceptoare":
                    case "SimulatorAutomat.StareAcceptoareStart":
                    case "SimulatorAutomat.StareNeacceptoareStart":
                        stareDragged = (StareNeacceptoare)e.Data.GetData(e.Data.GetFormats()[0]);
                        break;
                }

                if (stareDragged != null)
                {
                    if (!stareDragged.Acceptoare && !stareDragged.EsteDeStart)
                    {
                        stareAdaugata = new StareNeacceptoare();
                        stareAdaugata.Name = "StareNeacceptoare" + contorStari;
                    }
                    else if (!stareDragged.Acceptoare && stareDragged.EsteDeStart)
                    {
                        stareAdaugata = new StareNeacceptoareStart();
                        stareAdaugata.Name = "StareNeacceptoareStart" + contorStari;
                    }
                    else if (stareDragged.Acceptoare && !stareDragged.EsteDeStart)
                    {
                        stareAdaugata = new StareAcceptoare();
                        stareAdaugata.Name = "StareAcceptoare" + contorStari;
                    }
                    else
                    {
                        stareAdaugata = new StareAcceptoareStart();
                        stareAdaugata.Name = "StareAcceptoareStart" + contorStari;
                    }
                }
                // pun conditia asta pt ca e.Data ala poate sa fie epsilonul tras din greseala pe panza
                if (stareAdaugata != null)
                {
                    stareAdaugata.Location = this.panzaAutomat.PointToClient(new Point(e.X, e.Y));
                    stareAdaugata.Culoare = culoareCurenta;
                    listaStari.Add(stareAdaugata);
                    if (rtbConsola.Visible) AdaugaTextColoratConsola("[+] Am adaugat " + stareAdaugata.Name + "\n", rtbConsola.ForeColor);
                }
            }
            else  // luam automatul din xml, deci nu facem drag and drop, si transmitem null pe e (EventArgs la DragNDrop)
            {
                if (start && acceptoare) stareAdaugata = new StareAcceptoareStart();
                else if (start && !acceptoare) stareAdaugata = new StareNeacceptoareStart();
                else if (!start && acceptoare) stareAdaugata = new StareAcceptoare();
                else stareAdaugata = new StareNeacceptoare();

                if (nume != "tux")
                    stareAdaugata.NumeStare = nume;
                else
                    stareAdaugata.NumeStare = "?";
                listaStari.Insert(idx, stareAdaugata);
                stareAdaugata.Location = locatie;
                stareAdaugata.Culoare = culoare;
                stareAdaugata.FaVizibilaLabelStare();
            }
            if (stareAdaugata != null)
            {
                stareAdaugata.Parent = this.panzaAutomat;
                stareAdaugata.Tag = "Neapasata";
                razaStare = stareAdaugata.Width / 2;
                contorStari++;
                stareAdaugata.MouseDown += stareAdaugata_MouseDown;
                stareAdaugata.MouseMove += stareAdaugata_MouseMove;
                stareAdaugata.KeyUp += stareAdaugata_KeyUp;
                stareAdaugata.HandlerStergeStare += this.StergeStare;

                panzaAutomat.Controls.Add(stareAdaugata);
                stareAdaugata.BringToFront();
            }
        }

        public void StergeStare(object sender, EventArgs e)
        {
            StareNeacceptoare stn = sender as StareNeacceptoare;
            ActiuneStergeStare(stn);
        }

        private void CreeazaTranzitie(int start, int stop, Color culoare, string simb)
        {
            Tranzitie tranz = new Tranzitie();
            tranz.IndexStareStart = start;
            tranz.IndexStareStop = stop;

            // verificam daca tranzitia este deja in lista de tranzitii
            // daca da, perimitem utilizatorului sa o editeze, nu sa o puna peste
            int t = listaTranzitii.FindIndex(tr => (tr.IndexStareStart == tranz.IndexStareStart && tr.IndexStareStop == tranz.IndexStareStop));
            if (t >= 0)
            {
                listaTranzitii[t].FaInvizibilaLabelTranz();
                listaTranzitii[t].FaVizibilTextBoxTranz();
                if (rtbConsola.Visible) AdaugaTextColoratConsola("[+] Tranzitie editata: " + listaTranzitii[t].ToString() + Environment.NewLine, rtbConsola.ForeColor);
                statusLabel.Text = "Editam tranzitia " + listaTranzitii[t].ToString();
                tranz = null;
            }
            else
            {
                if (amImportatXML)
                {
                    tranz.FaVizibilaLabelTranz();
                    tranz.SetSimboluri(simb);
                }
                else
                {
                    tranz.SetSimboluri("?");
                }
                tranz.NumeStart = (listaStari[start].NumeStare == "tux" ? listaStari[start].Name : listaStari[start].NumeStare);
                tranz.NumeStop = (listaStari[stop].NumeStare == "tux" ? listaStari[stop].Name : listaStari[stop].NumeStare);
                tranz.SchimbaCuloarea(culoare, 2);

                tranz.Click += tranz_Click;
                tranz.MouseDoubleClick += new MouseEventHandler(tranz_MouseDoubleClick);
                tranz.DragDrop += new DragEventHandler(tranz_DragDrop);
                tranz.DragEnter += new DragEventHandler(tranz_DragEnter);
                listaTranzitii.Add(tranz);
                panzaAutomat.Controls.Add(tranz);
                panzaAutomat.Invalidate();

                if (rtbConsola.Visible)
                    AdaugaTextColoratConsola("[+] Am adaugat tranzitia " + tranz.ToString() + "\n", rtbConsola.ForeColor);
                statusLabel.Text = (amImportatXML ? "Import reusit" : "Tranzitie adaugata: " + tranz.ToString());
            }

        }
        // se apeleaza cand dam dublu click pe tranzitie
        void tranz_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            butSimulare.Enabled = false;
            butPas.Enabled = false;
            butStop.Enabled = false;
            rtbInput.Visible = false;
            simulareToolStripMenuItem.Enabled = false;
        }

        // se apeleaza cand Epsilon intra pe teritoriul tranzitiei la operatiune drag&drop
        void tranz_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            butSimulare.Enabled = false;
            butPas.Enabled = false;
            butStop.Enabled = false;
            rtbInput.Visible = false;
            simulareToolStripMenuItem.Enabled = false;
        }

        // se apeleaza cand incepe operatiunea de drag&drop a lui Epsilon
        void tranz_DragDrop(object sender, DragEventArgs e)
        {
            Tranzitie t = sender as Tranzitie;
            t.SetSimboluri(labelEpsilon.Text);
            this.panzaAutomat.Cursor = Cursors.Default;
        }

        // se apeleaza cand dam click pe meniul consola
        private void consolaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!vedemConsola)
            {
                splitContainer1.SplitterDistance = this.Width - this.Width / 2 + (1 / 5) * this.Width;
                this.rtbConsola.Visible = true;
            }
            else
            {
                splitContainer1.SplitterDistance = this.Width - 10;
                this.rtbConsola.Visible = false;
            }
            vedemConsola = !vedemConsola;
        }

        // se apeleaza cand consola isi schimba vizibilitatea
        private void rtbConsola_VisibleChanged(object sender, EventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            if (!rtb.Visible) rtb.Text = "";
        }

        private void AdaugaTextColoratConsola(string text, Color culoare)
        {
            rtbConsola.SelectionColor = culoare;
            rtbConsola.SelectedText = text;
        }

        // se apeleaza cand se modifica textul din consola
        private void rtbConsola_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            rtb.Select(rtb.Text.Length, 0);
            rtb.ScrollToCaret();
        }

        // se apeleaza cand dam click pe butonul color picker
        private void butColorPicker_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                culoareCurenta = colorDialog1.Color;
                b.FlatAppearance.BorderColor = culoareCurenta;
                b.ForeColor = culoareCurenta;
                b.BackColor = culoareCurenta;

                if (idxStareSelectata >= 0)
                {
                    listaStari[idxStareSelectata].Culoare = culoareCurenta;
                    listaStari[idxStareSelectata].Invalidate();
                }
                if (listaStariSelectate.Count > 0)
                    for (int t = 0; t < listaStariSelectate.Count; t++)
                    {
                        listaStariSelectate[t].Culoare = culoareCurenta;
                        listaStariSelectate[t].Invalidate();
                    }
            }
        }

        // se apeleaza cand dam click pe butonul de simulare
        private void butSimulare_Click(object sender, EventArgs e)
        {
            ActiuneStartTimer();
        }

        private void ActiuneStartTimer()
        {
            if (rtbInput.Text != "")
            {
                butPas.Enabled = false;
                butOpen.Enabled = false;
                importaToolStripMenuItem.Enabled = false;
                butStergeBox.Enabled = false;
                timer1.Start();
            }
            else
            {
                butOpen.Enabled = true;
                importaToolStripMenuItem.Enabled = true;
                butStergeBox.Enabled = true;
                butPas.Enabled = true;
                MessageBox.Show("String de intrare nul :(", "Atentie!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        // se apeleaza cand dam click pe meniul simulare
        private void simulareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActiuneStartTimer();
        }

        // se apeleaza o data la o secunda, caci atunci "ticaie" timerul
        private void timer1_Tick(object sender, EventArgs e)
        {
            ActiuneSimulare();
        }

        // daca audagam sau scoatem stari, cerem recompilare
        private void panzaAutomat_ControlRemoved(object sender, ControlEventArgs e)
        {
            butPas.Enabled = false;
            butSimulare.Enabled = false;
            butStop.Enabled = false;
            rtbInput.Visible = false;
            amSalvatXML = false;
            labelAlfabet.Visible = false;
            rtbInput.Clear();
        }

        // se apeleaza cand adaugam ceva pe picture box
        private void panzaAutomat_ControlAdded(object sender, ControlEventArgs e)
        {
            butPas.Enabled = false;
            butSimulare.Enabled = false;
            butStop.Enabled = false;
            butCompilare.Enabled = true;
            rtbInput.Visible = false;
            amSalvatXML = false;
            labelAlfabet.Visible = false;
            rtbInput.Clear();
        }

        // se apeleaza cand apasam pe butonul stop
        private void butStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (rtbConsola.Visible)
                AdaugaTextColoratConsola("Simulare oprita\n", Color.Yellow);
            statusLabel.Text = "Simulare oprita";
        }

        // se apeleaza cand dam click pe butonul X rosu
        private void butStergeBox_Click(object sender, EventArgs e)
        {
            ActiuneStergeBox();
        }

        private void ActiuneStergeBox()
        {
            // intrebam userul daca inainte de stergere vrea sa salveze
            if (panzaAutomat.Controls.Count > 0 && !amSalvatXML)
            {
                DialogResult res = MessageBox.Show("Salvati plansa de lucru ?", "Atentie!", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes) ActiuneExportXML();
            }
            // facem tranzitiile nule (daca avem)
            if (listaTranzitii.Count > 0)
            {
                for (int i = 0; i < listaTranzitii.Count; i++)
                    listaTranzitii[i] = null;
                // stergem elementele din lista
                listaTranzitii.Clear();
            }
            // facem starile nule
            if (listaStari.Count > 0)
            {
                for (int i = 0; i < listaStari.Count; i++)
                    listaStari[i] = null;
                // stergem elementele din lista
                listaStari.Clear();
            }

            // scriem in bara de stare ca a fost curatata plansa 
            statusLabel.Text = "Am curatat plansa de lucru";
            // daca e vizibila consola, anuntam si acolo despre stergerea plansei
            if (rtbConsola.Visible)
                AdaugaTextColoratConsola("[+] Am curatat plansa de lucru\n", Color.DarkOrange);

            // dezactivam butonul de compilare
            butCompilare.Enabled = false;
            // stergem tot de pe PictureBox
            panzaAutomat.Controls.Clear();
            // chemam metoda Paint
            panzaAutomat.Invalidate();

            this.Text = "FASharpSim";
        }

        // se apeleaza cand dam click pe butonul deschidere
        private void butOpen_Click(object sender, EventArgs e)
        {
            ActiuneDeschide();
        }
        // se apeleaza cand dam click pe butonul save
        private void butSaveXML_Click(object sender, EventArgs e)
        {
            ActiuneExportXML();
        }
        // se apeleaza la click pe butonul save
        private void butSaveImg_Click(object sender, EventArgs e)
        {
            ActiuneExportImagine();
        }
        // se apeleaza cand se redimensioneaza picture boxul
        private void panzaAutomat_Resize(object sender, EventArgs e)
        {
            panel1.AutoScrollMinSize = panzaAutomat.Size;
        }
        // se apeleaza cand prorietatea ReadOnly a rich text boxului pentru 
        // input trece din true in false, sau din false in true
        private void rtbInput_ReadOnlyChanged(object sender, EventArgs e)
        {
            if (rtbInput.ReadOnly)
            {
                rtbInput.BackColor = Color.FromArgb(204, 204, 255);
                rtbInput.Cursor = Cursors.No;
            }
            else
            {
                rtbInput.BackColor = Color.FromArgb(204, 229, 255);
                rtbInput.Cursor = Cursors.IBeam;
            }
        }
        // se apeleaza inainte de a se inchide formul, cand vrem sa iesim din aplicatie
        private void FormHome_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!amSalvatXML && panzaAutomat.Controls.Count > 0)
            {
                DialogResult res = MessageBox.Show("Salvezi automatul ? Daca nu salvezi, schimbarile facute vor fi pierdute!", "Salveaza!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Yes) ActiuneExportXML();
                else if (res == DialogResult.Cancel) e.Cancel = true;
            }
        }

        private bool EsteDeterminist()
        {
            // daca are vreo tranzitie epsilon nu este
            int t = 0, k = 0;
            for (t = 0; t < listaTranzitii.Count; t++)
                if (listaTranzitii[t].GetSimboluriTranzitie().Contains('\u03B5')) return false;
            //  tinem o lista de simboluri totala pt fiecare stare. In ea memoram simbolurile de pe fiecare
            // tranzitie care pleaca dn starea listaStari[t]
            int dimAlfabet = alfabet.Count;

            for (t = 0; t < listaStari.Count; t++)
            {
                simbStare.Clear();
                for (k = 0; k < listaTranzitii.Count; k++)
                    if (listaTranzitii[k].IndexStareStart == t)
                        simbStare.AddRange(listaTranzitii[k].GetSimboluriTranzitie());
                simbStare.Sort();
                if (simbStare.Count != dimAlfabet)
                    return false;
                if (!Enumerable.SequenceEqual(simbStare, alfabet))
                    return false;
            }
            return true;
        }
        // se apeleaza cand se redimensioneaza formul
        private void FormHome_Resize(object sender, EventArgs e)
        {
            this.panzaAutomat.Size = this.tableLayoutPanel1.Size;
            this.panel1.AutoScrollMinSize = panzaAutomat.Size;
        }

        // metode utilizate pentru selectarea mai multor stari
        private void GetStariSelectate()
        {
            foreach (Control c in panzaAutomat.Controls)
                if (c is StareNeacceptoare)
                    if (dreptunghiSelectie.IntersectsWith(c.Bounds))
                        listaStariSelectate.Add((StareNeacceptoare)c);

            for (int t = 0; t < listaStariSelectate.Count; t++)
                listaStariSelectate[t].BackColor = Color.Gold;
            if (listaStariSelectate.Count > 1)
                statusLabel.Text = "Ai selectat " + listaStariSelectate.Count + " stari";
        }
        // se apeleaza cand suntem cu mouseul apasat deasupra picture boxului
        private void panzaAutomat_MouseDown(object sender, MouseEventArgs e)
        {
            statusLabel.Text = "Ready";
            for (int t = 0; t < listaStari.Count; t++)
                listaStari[t].BackColor = Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            Cursor.Clip = Rectangle.Empty;

            if (e.Button == MouseButtons.Left)
            {
                listaStariSelectate.Clear();
                inceputSelectie = PointToClient(MousePosition);
                mouseDownPanza = true;
            }
        }
        // se apeleaza cand luam mouse-ul de pe picture box
        private void panzaAutomat_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDownPanza = false;
            SeteazaDreptunghiSelectie();
            panzaAutomat.Invalidate();
            Cursor.Clip = Rectangle.Empty;
            GetStariSelectate();
        }

        // daca avem un dreptunghi de selectie, salveaza imaginea cropped doar din dreptunghiul ala
        private void SalveazaImgineCropped()
        {
            bmpOriginal = new Bitmap(panzaAutomat.Width, panzaAutomat.Height);
            panzaAutomat.DrawToBitmap(bmpOriginal, panzaAutomat.ClientRectangle);
            bmpCrop = new Bitmap(dreptunghiSelectie.Width, dreptunghiSelectie.Height);
            Graphics g = Graphics.FromImage(bmpCrop);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.DrawImage(bmpOriginal, 0, 0, dreptunghiSelectie, GraphicsUnit.Pixel);
            panzaAutomat.Invalidate();
            bmpCrop.Save(saveFileDialog1.FileName);
            bmpOriginal.Dispose();
            bmpCrop.Dispose();
            g.Dispose();
        }

        private void SeteazaDreptunghiSelectie()
        {
            int x, y;
            int lungime, latime;
            x = inceputSelectie.X > sfarsitSelectie.X ? sfarsitSelectie.X : inceputSelectie.X;
            y = inceputSelectie.Y > sfarsitSelectie.Y ? sfarsitSelectie.Y : inceputSelectie.Y;
            lungime = inceputSelectie.X > sfarsitSelectie.X ? inceputSelectie.X - sfarsitSelectie.X : sfarsitSelectie.X - inceputSelectie.X;
            latime = inceputSelectie.Y > sfarsitSelectie.Y ? inceputSelectie.Y - sfarsitSelectie.Y : sfarsitSelectie.Y - inceputSelectie.Y;
            dreptunghiSelectie = new Rectangle(x, y, lungime, latime);
        }
        // se apeleaza cand miscam mouseul deasupra picture boxlui
        private void panzaAutomat_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!mouseDownPanza) return;
                sfarsitSelectie = PointToClient(MousePosition);
                SeteazaDreptunghiSelectie();
                panzaAutomat.Invalidate();
            }
        }
        // se apeleaza cans se schimba vizibilitatea rich text boxului input
        private void rtbInput_VisibleChanged(object sender, EventArgs e)
        {
            if (!rtbInput.Visible) rtbInput.Clear();
        }
        // se apeleaza cand se schimba vizibilitatea labelului alfabetului
        private void labelAlfabet_VisibleChanged(object sender, EventArgs e)
        {
            if (!labelAlfabet.Visible) labelAlfabet.Text = "Alfabetul automatului: ";
        }
    }
}
