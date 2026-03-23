using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarryPotterTZ
{
    internal class ListCharacters
    {
        public string fullName { get; set; }
        public string nickname { get; set; }
        public string house { get; set; }
        public string interpretedBy { get; set; }
        public string children { get; set; }
        public string image { get; set; }
        public DateTime birthdate { get; set; }
        public int index { get; set; }
        public string knownSpells { get; set; }

        public ListCharacters(string sorok, DateTime v) 
        {
            string[] characters = sorok.Split(',');
            fullName = characters[0].Replace(";", "").Trim();
            nickname = characters[1].Replace(";", "").Trim();
            house = characters[2].Replace(";", "").Trim();
            interpretedBy = characters[3].Replace(";", "").Trim();
            children = characters[4].Replace(";", "").Trim();
            image = characters[5].Replace(";", "").Trim();
            try
            {
                birthdate = DateTime.Parse(characters[6].Replace(";", "").Trim());
                index = int.Parse(characters[7].Replace(";", "").Trim());
            }
            catch (Exception)
            {
                birthdate = v;
            }
            knownSpells = characters[8].Replace(";", "").Trim();
        }
    }
}
