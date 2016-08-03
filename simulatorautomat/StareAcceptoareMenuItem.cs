using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulatorAutomat
{
    class StareAcceptoareMenuItem : ToolStripControlHost
    {
        public StareAcceptoareMenuItem() : this(new StareAcceptoare()) { }
        public StareAcceptoareMenuItem(Control c) : base(c) { }
    }
}
