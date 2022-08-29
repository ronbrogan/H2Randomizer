using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace H2Randomizer
{
    public class Preferences
    {
        private const string Filename = "prefs.json";
        private static string Filepath;

        static Preferences()
        {
            Filepath = Path.Combine(Environment.CurrentDirectory, Filename);
            Directory.CreateDirectory(Path.GetDirectoryName(Filepath));

            Current = new Preferences();

            if (File.Exists(Filepath))
            {
                Current = JsonSerializer.Deserialize<Preferences>(File.ReadAllText(Filepath)) ?? new Preferences();
            }
            else
            {
                try
                {
                    File.WriteAllText(Filepath, JsonSerializer.Serialize(Current));
                }
                catch (Exception)
                {
                    Filepath = Path.Combine(Path.GetTempPath(), "H2Randomizer", Filename);
                    Directory.CreateDirectory(Path.GetDirectoryName(Filepath));
                }
            }
        }

        public Preferences() { }

        public string Seed { get; set; } = "";
        public bool UnrandomizedWeapons { get; set; }
        public bool RandomizeAiWeapons { get; set; }
        public bool RandomizeAiWeaponsNaturally { get; set; }

        public static Preferences Current;

        public static void Persist()
        {
            try
            {
                File.WriteAllText(Filepath, JsonSerializer.Serialize(Current));
            }
            catch { }
        }
    }
}
