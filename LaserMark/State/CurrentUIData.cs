using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserMark.State
{
    public class CurrentUIData
    {
        public static PanelControl RightLayoutControl { get; set; }

        public static TextEdit BibText { get; set; }

        public static Size WindowSize { get; set; }

        public static Size RightPanelSize { get; set; }
    }
}
