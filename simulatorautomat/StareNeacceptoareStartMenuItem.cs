using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulatorAutomat
{
    class StareNeacceptoareStartMenuItem : ToolStripControlHost
    {
        public StareNeacceptoareStartMenuItem() : this(new StareNeacceptoareStart()) { }
        public StareNeacceptoareStartMenuItem(Control c) : base(c) { }
    }
}
