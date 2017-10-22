using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvgToMpa
{
    public class BlockMenu : Block
    {
        public string Text { get; set; }
        public MenuType MenuType { get; set; }
        public List<BlockMenuOption> OptionList { get; set; } = new List<BlockMenuOption>();
    }
}
