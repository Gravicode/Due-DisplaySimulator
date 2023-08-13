using GHIElectronics.TinyCLR.DUE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DueDisplaySimulator.Helpers
{
    public class StringConsole : IConsole
    {
        StringBuilder sb = new StringBuilder();
        public void Cls()
        {
            sb.Clear();
        }

        public void Print(object text)
        {
            sb.Append(text);
        }

      
        public string GetContent()
        {
            return sb.ToString();
        }
    }
}
