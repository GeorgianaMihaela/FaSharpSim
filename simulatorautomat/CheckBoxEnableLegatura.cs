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
    class CheckBoxEnableLegatura : CheckBox
    {
        protected ToolTip tip;
        public CheckBoxEnableLegatura()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.MinimumSize = new Size(40, 40);
            tip = new ToolTip();
            tip.SetToolTip(this, "Bifeaza pentru a trasa tranzitie (F2)");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen p = new Pen(Color.RoyalBlue, 6);
            p.EndCap = LineCap.ArrowAnchor;
            Point pStart = new Point(30, 40);
            Point pStop = new Point(45, 0);
            Point pCtrl1 = new Point(10, 10);
            Point pCtrl2 = new Point(35, 10);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawBezier(p, pStart, pCtrl1, pCtrl2, pStop);
            p.Dispose();
        }
    }
}
