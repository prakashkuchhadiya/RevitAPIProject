using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIProject.Utiles
{
    public static class DebugUtiles
    {
        public static void ShowInDebug(this List<string> strings)
        {
            strings.ForEach(s => Debug.WriteLine(s));
        }
    }
}
