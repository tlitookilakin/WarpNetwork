using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarpNetwork
{
    class CustomLocationHandler
    {
        public Action<string> Warp { get; set; }
        public Func<string, bool> GetEnabled { get; set; }
        public Func<string, string> GetLabel { get; set; }
        public CustomLocationHandler(Action<string> w, Func<string, bool> e, Func<string, string> l)
        {
            Warp = w;
            GetEnabled = e;
            GetLabel = l;
        }
    }
}
