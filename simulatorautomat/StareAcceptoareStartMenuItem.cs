using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulatorAutomat
{
    class StareAcceptoareStartMenuItem : ToolStripControlHost
    {
        public StareAcceptoareStartMenuItem() : this(new StareAcceptoareStart()) { }
        public StareAcceptoareStartMenuItem(Control c) : base(c) { }
    }
}
