using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HarryPotterTZ
{
    internal class ListSpells
    {
        public string spell { get; set; }
        public string use { get; set; }
        public int index { get; set; }

        public ListSpells(string sor)
        {
            string[] spells = sor.Split(',');
            spell = spells[0].Trim();
            use = spells[1].Trim();
            try
            {
                index = int.Parse(spells[2].Trim());
            }
            catch (Exception)
            {
                index = 0;
            }
        }
    }
}
