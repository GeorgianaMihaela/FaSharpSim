using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulatorAutomat
{
    // clasa ce implementeaza checkbox-ul care trebuie bifat pt a sterge o tranzitie
    class CheckBoxDisableLegatura : CheckBoxEnableLegatura
    {
        public CheckBoxDisableLegatura()
            : base()
        {
            tip.SetToolTip(this, "Bifeaza pentru a sterge tranzitie (SHIFT+F2)");
        }
        // se apeleaza de fiecare data cand se redeseneaza check boxul
        protected override void OnPaint(PaintEventArgs e)
        {
            // desenam un x rosu pe check box
            base.OnPaint(e);
            Pen p = new Pen(Color.Red, 6);

            Point pStart = new Point(20, 0);
            Point pStop = new Point(45, 40);
            e.Graphics.DrawLine(p, pStart, pStop);
            pStart = new Point(20, 40);
            pStop = new Point(45, 10);
            e.Graphics.DrawLine(p, pStart, pStop);
            p.Dispose();
        }
    }
}
