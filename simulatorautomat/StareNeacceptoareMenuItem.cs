using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulatorAutomat
{
    class StareNeacceptoareMenuItem : ToolStripControlHost
    {
        public StareNeacceptoareMenuItem() : this(new StareNeacceptoare()) { }
        public StareNeacceptoareMenuItem(Control c) : base(c) { }
    }
}
