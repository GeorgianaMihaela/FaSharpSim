using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulatorAutomat
{
    class StareAcceptoare : StareNeacceptoare
    {
        public StareAcceptoare()
            : base()
        {
            acceptoare = true;            
            esteDeStart = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.Parent is PictureBox)
                this.toolTipSugestie.SetToolTip(this, "ALT + Click pentru a copia stare");
            else
                this.toolTipSugestie.SetToolTip(this, "Click & Drag pentru a adauga stare acceptoare");
            this.miAcc.Checked = true;
        }
    }
}
