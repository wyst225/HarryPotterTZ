using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace HarryPotterTZ
{
    internal class Program
    {
        static string connectionString = "Server=localhost;Database=harrypotterdb;port=3307;Uid=root;Pwd=;";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            //adatbeolvasas
            var characters = ReadCharactersFromFile();
            var spells = ReadSpellsFromFile();

            // menetes adatbazisba
            SaveToDatabase(characters, spells);

            //konzol
            RunConsoleOperations(characters);

            Console.WriteLine("\nNyomj egy gombot a kilépéshez...");
            Console.ReadKey();
        }

        static void SaveToDatabase(List<ListCharacters> characters, List<ListSpells> spells)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                Console.WriteLine("Adatbázis kapcsolat kész, mentés folyamatban...");

                //spll
                foreach (var s in spells)
                {
                    var cmd = new MySqlCommand("INSERT IGNORE INTO spells (name, `use`) VALUES (@n, @u)", conn);
                    cmd.Parameters.AddWithValue("@n", s.spell);
                    cmd.Parameters.AddWithValue("@u", s.use);
                    cmd.ExecuteNonQuery();
                }

                //karakters
                foreach (var c in characters)
                {
                    var cmdChar = new MySqlCommand(@"INSERT INTO characters (full_name, nickname, hogwarts_house, interpreted_by, image_url, birthdate) 
                                                   VALUES (@fn, @nn, @hh, @ib, @im, @bd); SELECT LAST_INSERT_ID();", conn);
                    cmdChar.Parameters.AddWithValue("@fn", c.fullName);
                    cmdChar.Parameters.AddWithValue("@nn", c.nickname);
                    cmdChar.Parameters.AddWithValue("@hh", c.house);
                    cmdChar.Parameters.AddWithValue("@ib", c.interpretedBy);
                    cmdChar.Parameters.AddWithValue("@im", c.image);
                    cmdChar.Parameters.AddWithValue("@bd", c.birthdate == DateTime.MinValue ? (object)DBNull.Value : c.birthdate);

                    int charId = Convert.ToInt32(cmdChar.ExecuteScalar());

                    //children
                    if (!string.IsNullOrEmpty(c.children))
                    {
                        foreach (var childName in c.children.Split(';'))
                        {
                            var cmdChild = new MySqlCommand("INSERT INTO children (character_id, child_name) VALUES (@cid, @cn)", conn);
                            cmdChild.Parameters.AddWithValue("@cid", charId);
                            cmdChild.Parameters.AddWithValue("@cn", childName.Trim());
                            cmdChild.ExecuteNonQuery();
                        }
                    }

                    // known spells
                    if (!string.IsNullOrEmpty(c.knownSpells))
                    {
                        foreach (var sName in c.knownSpells.Split(';'))
                        {
                            var cmdGetS = new MySqlCommand("SELECT spell_id FROM spells WHERE name = @sn", conn);
                            cmdGetS.Parameters.AddWithValue("@sn", sName.Trim());
                            var sId = cmdGetS.ExecuteScalar();

                            if (sId != null)
                            {
                                var cmdLink = new MySqlCommand("INSERT INTO character_spells (character_id, spell_id) VALUES (@cid, @sid)", conn);
                                cmdLink.Parameters.AddWithValue("@cid", charId);
                                cmdLink.Parameters.AddWithValue("@sid", sId);
                                cmdLink.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        static void RunConsoleOperations(List<ListCharacters> characters)
        {
            //4.1
            Console.WriteLine("\n--- Karakterek listája (születés szerint csökkenő) ---");
            var sorted = characters.OrderByDescending(c => c.birthdate).ToList();
            foreach (var c in sorted)
            {
                string bd = c.birthdate == DateTime.MinValue ? "Ismeretlen" : c.birthdate.ToShortDateString();
                Console.WriteLine($"{c.fullName} ({bd})");
                Console.WriteLine($"   Varázslatok: {c.knownSpells?.Replace(";", ", ")}");
                Console.WriteLine($"   Gyermekek: {c.children?.Replace(";", ", ")}");
            }

            // 4.2 
            var validDates = characters.Where(c => c.birthdate != DateTime.MinValue).ToList();
            if (validDates.Any())
            {
                var oldest = validDates.OrderBy(c => c.birthdate).First();
                var youngest = validDates.OrderByDescending(c => c.birthdate).First();
                Console.WriteLine($"\nLegidősebb: {oldest.fullName}");
                Console.WriteLine($"Legfiatalabb: {youngest.fullName}");
            }

            // 4.3
            Console.Write("\nKérlek, adj meg egy becenevet: ");
            string nick = Console.ReadLine();
            var found = characters.FirstOrDefault(c => c.nickname.Equals(nick, StringComparison.OrdinalIgnoreCase));

            if (found != null)
            {
                string v = string.IsNullOrEmpty(found.knownSpells) ? "Nincs" : found.knownSpells.Replace(";", ", ");
                string gy = string.IsNullOrEmpty(found.children) ? "Nincs" : found.children.Replace(";", ", ");
                Console.WriteLine($"Varázslatai: {v}, Gyermekei: {gy}");
            }
            else
            {
                Console.WriteLine("Nincs ilyen becenevű karakter.");
            }
        }

        static List<ListCharacters> ReadCharactersFromFile()
        {
            List<ListCharacters> characters = new List<ListCharacters>();
            if (File.Exists("HP_characters.csv"))
            {
                var lines = File.ReadAllLines("HP_characters.csv", Encoding.UTF8).Skip(1);
                foreach (var line in lines) characters.Add(new ListCharacters(line, DateTime.MinValue));
            }
            return characters;
        }

        static List<ListSpells> ReadSpellsFromFile()
        {
            List<ListSpells> spells = new List<ListSpells>();
            if (File.Exists("HP_spells.csv"))
            {
                var lines = File.ReadAllLines("HP_spells.csv", Encoding.UTF8).Skip(1);
                foreach (var line in lines) spells.Add(new ListSpells(line));
            }
            return spells;
        }
    }
}