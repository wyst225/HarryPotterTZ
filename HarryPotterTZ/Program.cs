using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarryPotterTZ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //List<ListCharacters> lines = ReadCharactersFromFile();
            ReadCharactersFromFile();
        }

        static List<ListCharacters> ReadCharactersFromFile()
        {
            List<ListCharacters> characters = new List<ListCharacters>();

            if (System.IO.File.Exists("HP_characters.csv"))
            {
                try
                {
                    string[] lines = System.IO.File.ReadAllLines("HP_characters.csv", Encoding.UTF8);
                    foreach (string line in lines)
                    {
                        characters.Add(new ListCharacters(line, DateTime.MinValue));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading file: " + ex.Message);
                }

                Console.WriteLine("Succes");
            } 
            else {
                Console.WriteLine("File not found: HP_characters.csv");
            }

            return characters;
        }
    }
}
