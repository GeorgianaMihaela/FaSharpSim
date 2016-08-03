using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulatorAutomat
{
    class StareNeacceptoareStart : StareNeacceptoare
    {
        public StareNeacceptoareStart()
            : base()
        {
            esteDeStart = true;           
            acceptoare = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!(this.Parent is PictureBox))
                toolTipSugestie.SetToolTip(this, "Click & Drag pentru a adauga stare de start neacceptoare");
            else
                toolTipSugestie.SetToolTip(this, "ALT + Click pentru a copia starea");
            this.miStart.Checked = true;
        } 
    }
}
