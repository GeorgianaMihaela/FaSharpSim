using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulatorAutomat
{
    class StareAcceptoareStart : StareAcceptoare
    {
        public StareAcceptoareStart()
            : base()
        {
            esteDeStart = true;           
            acceptoare = true;           
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!(this.Parent is PictureBox))
                this.toolTipSugestie.SetToolTip(this, "Click & Drag pentru a adauga stare de start acceptoare ");
            else
                this.toolTipSugestie.SetToolTip(this, "ALT + Click pentru a copia stare");
            this.miStart.Checked = true;
            this.miAcc.Checked = true;
        }
    }
}
