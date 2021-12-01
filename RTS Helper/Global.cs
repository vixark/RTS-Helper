using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Interop;
using WForms = System.Windows.Forms;
using System.IO;
using System.Text.Json;
using Vixark;
using System.Text.Json.Serialization;
using System.Linq;
using System.Windows;
using static Vixark.General;
using System.Text.RegularExpressions;

namespace RTSHelper {



    public static class Global {


        public static Settings Preferencias = new Settings();

        public static bool ModoDesarrollo = true;

        public static string AOE2Name = "Age of Empires II";

        public static string AOE4Name = "Age of Empires IV";

        public static string OtherName = "Other";

        public static string DirectorioAOE2 = @"D:\Juegos\Steam\steamapps\common\AoE2DE";

        public static string DirectorioAplicaciónReal = @"D:\Programas\RTS Helper";

        public static string DirectorioAplicación = ModoDesarrollo ? DirectorioAplicaciónReal : (AppDomain.CurrentDomain.BaseDirectory ?? @"C:\"); // En realidad no veo en que situación podría ser null BaseDirectory.

        public static string RutaPreferencias = Path.Combine(DirectorioAplicación, "Settings.json");

        public static string NoneSoundString = "None";

        public static string DirectorioBuildOrdersPredeterminado = Path.Combine(DirectorioAplicación, "Build Orders");

        public static string DirectorioSonidosCortos = Path.Combine(DirectorioAplicación, "Sounds", "Short");

        public static string DirectorioSonidosLargos = Path.Combine(DirectorioAplicación, "Sounds", "Long");

        public static string DirectorioNombres = Path.Combine(DirectorioAplicación, "Names");

        public static string DirectorioImágenes = Path.Combine(DirectorioAplicación, "Images");

        public static string DirectorioBuildOrdersEfectivo => Preferencias.BuildOrderDirectory ?? DirectorioBuildOrdersPredeterminado;

        public enum NameType {
            Complete, Common, Abbreviation, Acronym, CommonPlural, AbbreviationPlural, AcronymPlural, // Todos estos son en inglés.
            Custom, BR, DE, ES, FR, HI, IT, JP, KO, MS, MX, PL, RU, TR, TW, VI, ZH
        }

        public static string ObtenerSeleccionadoEnCombobox(SelectionChangedEventArgs e) {

            var cbi = e.AddedItems[0] as ComboBoxItem;
            if (cbi != null) {
                if (cbi.Content is null) {
                    throw new Exception("No se encontró el elemento seleccionado en cbi.Content."); // Nunca debería pasar.
                } else {
                    return cbi.Content?.ToString() ?? throw new Exception("No se encontró el elemento seleccionado en cbi.Content?.ToString().");
                }              
            } else {
                var str = e.AddedItems[0] as string;
                return str ?? throw new Exception("No se encontró el elemento seleccionado en str.");
            }

        } // ObtenerSeleccionadoEnCombobox>


        public static string ObtenerResoluciónRecomendada() {

            var interopHelper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            var pantallaActual = WForms.Screen.FromHandle(interopHelper.Handle);
            var anchoPantalla = pantallaActual.Bounds.Width;
            var altoPantalla = pantallaActual.Bounds.Height;
            var resolución = "1920x1080";
            if (anchoPantalla == 1920 && altoPantalla == 1080) {
                resolución = "1920x1080";
            } else if (anchoPantalla == 2560 && altoPantalla == 1440) {
                resolución = "2560x1440";
            } else if (anchoPantalla == 1366 && altoPantalla == 768) {
                resolución = "1366x768";
            } else if (altoPantalla >= 1440) {
                resolución = "2560x1440";
            } else if (altoPantalla >= 1080) {
                resolución = "1920x1080";
            } else {
                resolución = "1366x768";
            }
            return resolución;

        } // ObtenerResoluciónRecomendada>


        public static int ObtenerDuraciónEndStepSound() 
            => (int)Math.Round(1000 * MediaPlayer.GetDuration(Path.Combine(DirectorioSonidosLargos, Preferencias.StepEndSound)), 0);


        public static void CrearArchivosNombres() {

            if (!File.Exists(Preferencias.EnglishNamesPath)) CrearArchivoNombresInglés();
            if (!File.Exists(Preferencias.NamesPath)) CrearArchivoNombres();

        } // CrearArchivosNombres>


        public static void CrearArchivoNombres() {

            if (!File.Exists(Preferencias.EnglishNamesPath)) throw new FileNotFoundException($"{Preferencias.EnglishNamesPath} not found.");
            var names = JsonSerializer.Deserialize<Dictionary<NameType, Dictionary<string, string>>>(File.ReadAllText(Preferencias.EnglishNamesPath), 
                ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración | Serialización.EnumeraciónEnTexto | Serialización.UTF8));

            if (Preferencias.Game == AOE2Name) {

                foreach (var tipo in Vixark.General.ObtenerValores<NameType>()) {

                    var nombreTipo = tipo.ToString();
                    if (nombreTipo.Length == 2) { // Es un idioma.

                        names.Add(tipo, new Dictionary<string, string>());
                        var rutaNombresIdioma = Path.Combine(DirectorioAOE2, "resources", nombreTipo, "strings", "key-value", "key-value-strings-utf8.txt");
                        var líneas = File.ReadAllLines(rutaNombresIdioma);
                        foreach (var línea in líneas) {

                            var m = Regex.Match(línea, @"([0-9]+)[ 	]+""(.+)""");
                            if (m.Success) {

                                var código = m.Groups[1].Value;         
                                if (names[NameType.Complete].ContainsKey(código) && !names[tipo].ContainsKey(código)) {

                                    var nombre = m.Groups[2].Value;
                                    if (nombre.EndsWith(": ")) { // Elimina el : y espacio que tienen algunos nombres.
                                        names[tipo].Add(código, nombre[0..^2]);
                                    } else if (nombre.Contains(" (")) {
                                        var índiceFinal = nombre.IndexOf(" (");
                                        names[tipo].Add(código, nombre[0..índiceFinal]); // Para quitar el contenido dentro de los paréntesis de Fish (Perch), Tree (Oak), etc.
                                    } else {
                                        names[tipo].Add(código, nombre);
                                    }
                                    
                                }

                            }

                        }

                    }

                }

            }

            File.WriteAllText(Preferencias.NamesPath, JsonSerializer.Serialize(names, 
                ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración | Serialización.EnumeraciónEnTexto | Serialización.UTF8)));

        } // CrearArchivoNombres>


        public static void CrearArchivoNombresInglés() {

            var names = new Dictionary<NameType, Dictionary<string, string>>();
            names.Add(NameType.Complete, new Dictionary<string, string>());
            names.Add(NameType.Common, new Dictionary<string, string>());
            names.Add(NameType.Abbreviation, new Dictionary<string, string>());
            names.Add(NameType.Acronym, new Dictionary<string, string>());
            names.Add(NameType.CommonPlural, new Dictionary<string, string>());
            names.Add(NameType.AbbreviationPlural, new Dictionary<string, string>());
            names.Add(NameType.AcronymPlural, new Dictionary<string, string>());

            if (Preferencias.Game == AOE2Name) {

                names[NameType.Complete].Add("4201", "Dark Age");
                names[NameType.Complete].Add("5083", "Archer"); names[NameType.Abbreviation].Add("5083", "Arch"); names[NameType.CommonPlural].Add("5083", "Archers"); names[NameType.AbbreviationPlural].Add("5083", "Archs");
                names[NameType.Complete].Add("5084", "Crossbowman"); names[NameType.Abbreviation].Add("5084", "Crossbow"); names[NameType.Acronym].Add("5084", "Xbow"); names[NameType.CommonPlural].Add("5084", "Crossbowmen"); names[NameType.AbbreviationPlural].Add("5084", "Crossbows"); names[NameType.AcronymPlural].Add("5084", "Xbows");
                names[NameType.Complete].Add("5418", "Arbalester"); names[NameType.Abbreviation].Add("5418", "Arb"); names[NameType.CommonPlural].Add("5418", "Arbalesters"); names[NameType.AbbreviationPlural].Add("5418", "Arbs");
                names[NameType.Complete].Add("5088", "Skirmisher"); names[NameType.Abbreviation].Add("5088", "Skirm"); names[NameType.CommonPlural].Add("5088", "Skirmishers"); names[NameType.AbbreviationPlural].Add("5088", "Skirms");
                names[NameType.Complete].Add("5087", "Elite Skirmisher");
                names[NameType.Complete].Add("5190", "Imperial Skirmisher"); names[NameType.Abbreviation].Add("5190", "Imp Skirm"); names[NameType.CommonPlural].Add("5190", "Imperial Skirmishers"); names[NameType.AbbreviationPlural].Add("5190", "Imp Skirms");
                names[NameType.Complete].Add("5690", "Slinger"); names[NameType.CommonPlural].Add("5690", "Slingers");
                names[NameType.Complete].Add("5086", "Hand Cannoneer"); names[NameType.Acronym].Add("5086", "HC"); names[NameType.CommonPlural].Add("5086", "Hand Cannoneers");
                names[NameType.Complete].Add("5085", "Cavalry Archer"); names[NameType.Abbreviation].Add("5085", "Cav Archer"); names[NameType.Acronym].Add("5085", "CA"); names[NameType.CommonPlural].Add("5085", "Cavalry Archers"); names[NameType.AbbreviationPlural].Add("5085", "Cav Archers");
                names[NameType.Complete].Add("5412", "Heavy Cavalry Archer"); names[NameType.Acronym].Add("5412", "HCA"); names[NameType.CommonPlural].Add("5412", "Heavy Cavalry Archers");
                names[NameType.Complete].Add("5137", "Genitour"); names[NameType.Abbreviation].Add("5137", "Geni"); names[NameType.CommonPlural].Add("5137", "Genitours"); names[NameType.AbbreviationPlural].Add("5137", "Genis");
                names[NameType.Complete].Add("5139", "Elite Genitour");
                names[NameType.Complete].Add("7411", "Thumb Ring");
                names[NameType.Complete].Add("7415", "Parthian Tactics");
                names[NameType.Complete].Add("5128", "Archery Range"); names[NameType.Common].Add("5128", "Archery|Range"); names[NameType.CommonPlural].Add("5128", "Archeries|Ranges");
                names[NameType.Complete].Add("5079", "Militia"); names[NameType.CommonPlural].Add("5079", "Militias");
                names[NameType.Complete].Add("5080", "Man-at-Arms"); names[NameType.Acronym].Add("5080", "MAA|M@A"); names[NameType.CommonPlural].Add("5080", "Men-at-Arms");
                names[NameType.Complete].Add("5081", "Long Swordsman"); names[NameType.Abbreviation].Add("5081", "Longsword"); names[NameType.Acronym].Add("5081", "LS"); names[NameType.CommonPlural].Add("5081", "Long Swordsmen"); names[NameType.AbbreviationPlural].Add("5081", "Longswords");
                names[NameType.Complete].Add("5411", "Two-Handed Swordsman"); names[NameType.Acronym].Add("5411", "2HS"); names[NameType.CommonPlural].Add("5411", "Two-Handed Swordsmen");
                names[NameType.Complete].Add("5469", "Champion"); names[NameType.Abbreviation].Add("5469", "Champ"); names[NameType.CommonPlural].Add("5469", "Champions"); names[NameType.AbbreviationPlural].Add("5469", "Champs");
                names[NameType.Complete].Add("5078", "Spearman"); names[NameType.Abbreviation].Add("5078", "Spear"); names[NameType.CommonPlural].Add("5078", "Spearmen"); names[NameType.AbbreviationPlural].Add("5078", "Spears");
                names[NameType.Complete].Add("5408", "Pikeman"); names[NameType.Abbreviation].Add("5408", "Pike"); names[NameType.CommonPlural].Add("5408", "Pikemen"); names[NameType.AbbreviationPlural].Add("5408", "Pikes");
                names[NameType.Complete].Add("5409", "Halberdier"); names[NameType.Abbreviation].Add("5409", "Halb"); names[NameType.CommonPlural].Add("5409", "Halberdier"); names[NameType.AbbreviationPlural].Add("5409", "Halbs");
                names[NameType.Complete].Add("5672", "Eagle Scout"); names[NameType.CommonPlural].Add("5672", "Eagle Scouts");
                names[NameType.Complete].Add("5671", "Eagle Warrior"); names[NameType.Common].Add("5671", "Eagle"); names[NameType.Acronym].Add("5671", "EW"); names[NameType.CommonPlural].Add("5671", "Eagles");
                names[NameType.Complete].Add("5673", "Elite Eagle Warrior");
                names[NameType.Complete].Add("5114", "Condottiero"); names[NameType.Abbreviation].Add("5114", "Condo"); names[NameType.CommonPlural].Add("5114", "Condottieros"); names[NameType.AbbreviationPlural].Add("5114", "Condos");
                names[NameType.Complete].Add("7403", "Supplies");
                names[NameType.Complete].Add("7210", "Squires");
                names[NameType.Complete].Add("7258", "Arson");
                names[NameType.Complete].Add("5135", "Barracks"); names[NameType.Abbreviation].Add("5135", "Rax"); names[NameType.AbbreviationPlural].Add("5135", "Raxes");
                names[NameType.Complete].Add("5326", "Scout Cavalry"); names[NameType.Common].Add("5326", "Scout"); names[NameType.Acronym].Add("5326", "SC"); names[NameType.CommonPlural].Add("5326", "Scouts");
                names[NameType.Complete].Add("5069", "Light Cavalry"); names[NameType.Abbreviation].Add("5069", "Light Cav"); names[NameType.Acronym].Add("5069", "LCav"); names[NameType.CommonPlural].Add("5069", "Light Cavalries"); names[NameType.AbbreviationPlural].Add("5069", "Light Cavs"); names[NameType.AcronymPlural].Add("5069", "LCavs");
                names[NameType.Complete].Add("5661", "Hussar"); names[NameType.CommonPlural].Add("5661", "Hussars");
                names[NameType.Complete].Add("5577", "Winged Hussar"); names[NameType.CommonPlural].Add("5577", "Winged Hussars");
                names[NameType.Complete].Add("7409", "Bloodlines");
                names[NameType.Complete].Add("5068", "Knight"); names[NameType.Acronym].Add("5068", "KT|KNT|KTS"); names[NameType.CommonPlural].Add("5068", "Knights");
                names[NameType.Complete].Add("5070", "Cavalier"); names[NameType.Abbreviation].Add("5070", "Cav|Cava"); names[NameType.CommonPlural].Add("5070", "Cavaliers"); names[NameType.AbbreviationPlural].Add("5070", "Cavs|Cavas");
                names[NameType.Complete].Add("5471", "Paladin"); names[NameType.Abbreviation].Add("5471", "Pala|Pal"); names[NameType.CommonPlural].Add("5471", "Paladins"); names[NameType.AbbreviationPlural].Add("5471", "Palas|Pals");
                names[NameType.Complete].Add("5416", "Camel Rider"); names[NameType.Common].Add("5416", "Camel"); names[NameType.CommonPlural].Add("5416", "Camels");
                names[NameType.Complete].Add("5417", "Heavy Camel Rider"); names[NameType.Common].Add("5417", "Heavy Camel"); names[NameType.CommonPlural].Add("5417", "Heavy Camels");
                names[NameType.Complete].Add("5419", "Imperial Camel Rider"); names[NameType.Common].Add("5419", "Imperial Camel"); names[NameType.Abbreviation].Add("5419", "Imp Camel"); names[NameType.CommonPlural].Add("5419", "Imperial Camels"); names[NameType.AbbreviationPlural].Add("5419", "Imp Camels");
                names[NameType.Complete].Add("19033", "Battle Elephant"); names[NameType.Abbreviation].Add("19033", "Battle Ele"); names[NameType.CommonPlural].Add("19033", "Battle Elephants"); names[NameType.AbbreviationPlural].Add("19033", "Battle Eles");
                names[NameType.Complete].Add("5168", "Elite Battle Elephant");
                names[NameType.Complete].Add("19127", "Steppe Lancer"); names[NameType.Common].Add("19127", "Lancer"); names[NameType.CommonPlural].Add("19127", "Lancers");
                names[NameType.Complete].Add("5010", "Elite Steppe Lancer");
                names[NameType.Complete].Add("5040", "Xolotl Warrior"); names[NameType.Common].Add("5040", "Xolotl"); names[NameType.CommonPlural].Add("5040", "Xolotls");
                names[NameType.Complete].Add("7039", "Husbandry");
                names[NameType.Complete].Add("5171", "Stable"); names[NameType.Acronym].Add("5171", "STBL|STB"); names[NameType.CommonPlural].Add("5171", "Stables");
                names[NameType.Complete].Add("5169", "Siege Workshop"); names[NameType.Acronym].Add("5169", "SW"); names[NameType.CommonPlural].Add("5169", "Siege Workshops");
                names[NameType.Complete].Add("5094", "Battering Ram"); names[NameType.Common].Add("5094", "Ram"); names[NameType.CommonPlural].Add("5094", "Rams");
                names[NameType.Complete].Add("5289", "Capped Ram"); names[NameType.CommonPlural].Add("5289", "Capped Rams");
                names[NameType.Complete].Add("5446", "Siege Ram"); names[NameType.CommonPlural].Add("5446", "Siege Rams");
                names[NameType.Complete].Add("5095", "Mangonel"); names[NameType.Abbreviation].Add("5095", "Mango"); names[NameType.CommonPlural].Add("5095", "Mangonels"); names[NameType.AbbreviationPlural].Add("5095", "Mangos");
                names[NameType.Complete].Add("5448", "Onager"); names[NameType.CommonPlural].Add("5448", "Onagers");
                names[NameType.Complete].Add("5493", "Siege Onager"); names[NameType.Acronym].Add("5493", "SO"); names[NameType.CommonPlural].Add("5493", "Siege Onagers");
                names[NameType.Complete].Add("5096", "Scorpion"); names[NameType.Abbreviation].Add("5096", "Scorp"); names[NameType.CommonPlural].Add("5096", "Scorpions"); names[NameType.AbbreviationPlural].Add("5096", "Scorps");
                names[NameType.Complete].Add("5439", "Heavy Scorpion"); names[NameType.Abbreviation].Add("5439", "Heavy Scorp"); names[NameType.Acronym].Add("5439", "HScorp|HS"); names[NameType.CommonPlural].Add("5439", "Heavy Scorpions"); names[NameType.AbbreviationPlural].Add("5439", "Heavy Scorps"); names[NameType.AcronymPlural].Add("5439", "HScorps");
                names[NameType.Complete].Add("5445", "Siege Tower"); names[NameType.CommonPlural].Add("5445", "Siege Towers");
                names[NameType.Complete].Add("5093", "Bombard Cannon"); names[NameType.Acronym].Add("5093", "BBC"); names[NameType.CommonPlural].Add("5093", "Bombard Cannons");
                names[NameType.Complete].Add("5579", "Houfnice"); names[NameType.CommonPlural].Add("5579", "Houfnices");
                names[NameType.Complete].Add("5131", "Blacksmith"); names[NameType.Acronym].Add("5131", "BS");
                names[NameType.Complete].Add("7208", "Padded Archer Armor");
                names[NameType.Complete].Add("7172", "Fletching");
                names[NameType.Complete].Add("7067", "Forging");
                names[NameType.Complete].Add("7081", "Scale Barding Armor");
                names[NameType.Complete].Add("7074", "Scale Mail Armor");
                names[NameType.Complete].Add("7209", "Leather Archer Armor");
                names[NameType.Complete].Add("7150", "Bodkin Arrow"); names[NameType.Common].Add("7150", "Bodkin");
                names[NameType.Complete].Add("7068", "Iron Casting");
                names[NameType.Complete].Add("7082", "Chain Barding Armor");
                names[NameType.Complete].Add("7076", "Chain Mail Armor");
                names[NameType.Complete].Add("7216", "Ring Archer Armor");
                names[NameType.Complete].Add("7151", "Bracer");
                names[NameType.Complete].Add("7075", "Blast Furnace");
                names[NameType.Complete].Add("7080", "Plate Barding Armor");
                names[NameType.Complete].Add("7077", "Plate Mail Armor");
                names[NameType.Complete].Add("5144", "Dock"); names[NameType.CommonPlural].Add("5144", "Docks");
                names[NameType.Complete].Add("5090", "Fishing Ship"); names[NameType.CommonPlural].Add("5090", "Fishing Ships");
                names[NameType.Complete].Add("5443", "Transport Ship"); names[NameType.Abbreviation].Add("5443", "Xport"); names[NameType.CommonPlural].Add("5443", "Transport Ships"); names[NameType.AbbreviationPlural].Add("5443", "Xports");
                names[NameType.Complete].Add("5160", "Fire Galley"); names[NameType.CommonPlural].Add("5160", "Fire Galleys");
                names[NameType.Complete].Add("5089", "Trade Cog"); names[NameType.CommonPlural].Add("5089", "Trade Cogs");
                names[NameType.Complete].Add("5162", "Demolition Raft"); names[NameType.Abbreviation].Add("5162", "Demo Raft"); names[NameType.CommonPlural].Add("5162", "Demolition Rafts"); names[NameType.AbbreviationPlural].Add("5162", "Demo Rafts");
                names[NameType.Complete].Add("5436", "Galley"); names[NameType.CommonPlural].Add("5436", "Galleys");
                names[NameType.Complete].Add("5426", "Fire Ship"); names[NameType.Abbreviation].Add("5426", "Fire"); names[NameType.CommonPlural].Add("5426", "Fire Ships"); names[NameType.AbbreviationPlural].Add("5426", "Fires");
                names[NameType.Complete].Add("7314", "Gillnets");
                names[NameType.Complete].Add("5424", "Demolition Ship"); names[NameType.Abbreviation].Add("5424", "Demo"); names[NameType.CommonPlural].Add("5424", "Demolition Ships"); names[NameType.AbbreviationPlural].Add("5424", "Demos");
                names[NameType.Complete].Add("5091", "War Galley"); names[NameType.CommonPlural].Add("5091", "War Galleys");
                names[NameType.Complete].Add("5429", "Fast Fire Ship"); names[NameType.Abbreviation].Add("5429", "Fast Fire"); names[NameType.Acronym].Add("5429", "FFS"); names[NameType.CommonPlural].Add("5429", "Fast Fire Ships"); names[NameType.AbbreviationPlural].Add("5429", "Fast Fires");
                names[NameType.Complete].Add("5287", "Cannon Galleon"); names[NameType.CommonPlural].Add("5287", "Cannon Galleons");
                names[NameType.Complete].Add("5425", "Heavy Demolition Ship"); names[NameType.Abbreviation].Add("5425", "Heavy Demo"); names[NameType.CommonPlural].Add("5425", "Heavy Demolition Ships"); names[NameType.AbbreviationPlural].Add("5425", "Heavy Demos");
                names[NameType.Complete].Add("5309", "Galleon"); names[NameType.CommonPlural].Add("5309", "Galleons");
                names[NameType.Complete].Add("5573", "Elite Cannon Galleon");
                names[NameType.Complete].Add("5731", "Turtle Ship"); names[NameType.Acronym].Add("5731", "TS"); names[NameType.CommonPlural].Add("5731", "Turtle Ships");
                names[NameType.Complete].Add("5732", "Elite Turtle Ship");
                names[NameType.Complete].Add("5106", "Longboat"); names[NameType.CommonPlural].Add("5106", "Longboats");
                names[NameType.Complete].Add("5457", "Elite Longboat");
                names[NameType.Complete].Add("5132", "Caravel"); names[NameType.CommonPlural].Add("5132", "Caravels");
                names[NameType.Complete].Add("5133", "Elite Caravel");
                names[NameType.Complete].Add("7372", "Careening");
                names[NameType.Complete].Add("7373", "Dry Dock");
                names[NameType.Complete].Add("7377", "Shipwright");
                names[NameType.Complete].Add("5495", "Fish Trap"); names[NameType.CommonPlural].Add("5495", "Fish Traps");
                names[NameType.Complete].Add("5176", "University"); names[NameType.Abbreviation].Add("5176", "Uni"); names[NameType.CommonPlural].Add("5176", "Universities"); names[NameType.AbbreviationPlural].Add("5176", "Unis");
                names[NameType.Complete].Add("7050", "Masonry");
                names[NameType.Complete].Add("7051", "Architecture");
                names[NameType.Complete].Add("7047", "Chemistry");
                names[NameType.Complete].Add("7093", "Ballistics");
                names[NameType.Complete].Add("7378", "Siege Engineers");
                names[NameType.Complete].Add("7374", "Heated Shot");
                names[NameType.Complete].Add("7278", "Arrowslits");
                names[NameType.Complete].Add("7321", "Murder Holes");
                names[NameType.Complete].Add("7054", "Treadmill Crane");
                names[NameType.Complete].Add("5504", "Outpost"); names[NameType.CommonPlural].Add("5504", "Outposts");
                names[NameType.Complete].Add("5178", "Watch Tower"); names[NameType.Common].Add("5178", "Tower"); names[NameType.CommonPlural].Add("5178", "Towers");
                names[NameType.Complete].Add("5154", "Guard Tower"); names[NameType.CommonPlural].Add("5154", "Guard Towers");
                names[NameType.Complete].Add("5155", "Keep"); names[NameType.CommonPlural].Add("5155", "Keeps");
                names[NameType.Complete].Add("5156", "Bombard Tower"); names[NameType.Acronym].Add("5156", "BBT"); names[NameType.CommonPlural].Add("5156", "Bombard Towers");
                names[NameType.Complete].Add("5202", "Palisade Wall"); names[NameType.Common].Add("5202", "Palisade"); names[NameType.CommonPlural].Add("5202", "Palisades");
                names[NameType.Complete].Add("5186", "Palisade Gate"); names[NameType.CommonPlural].Add("5186", "Palisade Gates");
                names[NameType.Complete].Add("5203", "Stone Wall"); names[NameType.CommonPlural].Add("5203", "Stone Walls");
                names[NameType.Complete].Add("5204", "Fortified Wall"); names[NameType.CommonPlural].Add("5204", "Fortified Walls");
                names[NameType.Complete].Add("5185", "Gate"); names[NameType.Common].Add("5185", "Stone Gate"); names[NameType.CommonPlural].Add("5185", "Stone Gates");
                names[NameType.Complete].Add("5142", "Castle"); names[NameType.Acronym].Add("5142", "CST"); names[NameType.CommonPlural].Add("5142", "Castles");
                names[NameType.Complete].Add("5660", "Petard"); names[NameType.Abbreviation].Add("5660", "Pet"); names[NameType.CommonPlural].Add("5660", "Petards"); names[NameType.AbbreviationPlural].Add("5660", "Pets");
                names[NameType.Complete].Add("5097", "Trebuchet"); names[NameType.Abbreviation].Add("5097", "Treb"); names[NameType.CommonPlural].Add("5097", "Trebuchets"); names[NameType.AbbreviationPlural].Add("5097", "Trebs");
                names[NameType.Complete].Add("7376", "Hoardings");
                names[NameType.Complete].Add("7322", "Sappers");
                names[NameType.Complete].Add("7319", "Conscription");
                names[NameType.Complete].Add("7408", "Spies/Treason");
                names[NameType.Complete].Add("19329", "Krepost"); names[NameType.CommonPlural].Add("19329", "Kreposts");
                names[NameType.Complete].Add("19138", "Donjon"); names[NameType.CommonPlural].Add("19138", "Donjons");
                names[NameType.Complete].Add("5138", "Monastery"); names[NameType.CommonPlural].Add("5138", "Monasteries");
                names[NameType.Complete].Add("5099", "Monk"); names[NameType.CommonPlural].Add("5099", "Monks");
                names[NameType.Complete].Add("5691", "Missionary"); names[NameType.CommonPlural].Add("5691", "Missionaries");
                names[NameType.Complete].Add("7315", "Redemption");
                names[NameType.Complete].Add("7316", "Atonement");
                names[NameType.Complete].Add("7435", "Herbal Medicine");
                names[NameType.Complete].Add("7412", "Heresy");
                names[NameType.Complete].Add("7221", "Sanctity");
                names[NameType.Complete].Add("7249", "Fervor");
                names[NameType.Complete].Add("7045", "Faith");
                names[NameType.Complete].Add("7220", "Illumination");
                names[NameType.Complete].Add("7222", "Block Printing");
                names[NameType.Complete].Add("7416", "Theocracy");
                names[NameType.Complete].Add("5344", "House"); names[NameType.CommonPlural].Add("5344", "Houses");
                names[NameType.Complete].Add("5164", "Town Center"); names[NameType.Acronym].Add("5164", "TC"); names[NameType.CommonPlural].Add("5164", "Town Centers");
                names[NameType.Complete].Add("14121", "Villager"); names[NameType.Abbreviation].Add("14121", "Vil|Vill"); names[NameType.Acronym].Add("14121", "V"); names[NameType.CommonPlural].Add("14121", "Villagers"); names[NameType.AbbreviationPlural].Add("14121", "Vils|Vills");
                names[NameType.Complete].Add("4202", "Feudal Age");
                names[NameType.Complete].Add("7022", "Loom");
                names[NameType.Complete].Add("7008", "Town Watch");
                names[NameType.Complete].Add("4203", "Castle Age");
                names[NameType.Complete].Add("7211", "Wheelbarrow"); names[NameType.Acronym].Add("7211", "WB");
                names[NameType.Complete].Add("7282", "Town Patrol");
                names[NameType.Complete].Add("4204", "Imperial Age"); names[NameType.Abbreviation].Add("4204", "Imp");
                names[NameType.Complete].Add("7246", "Hand Cart"); names[NameType.Acronym].Add("7246", "HC");
                names[NameType.Complete].Add("19141", "Flemish Militia"); names[NameType.CommonPlural].Add("19141", "Flemish Militias");
                names[NameType.Complete].Add("5182", "Wonder"); names[NameType.CommonPlural].Add("5182", "Wonders");
                names[NameType.Complete].Add("5159", "Feitoria");
                names[NameType.Complete].Add("5487", "Mining Camp"); names[NameType.Acronym].Add("5487", "MC"); names[NameType.CommonPlural].Add("5487", "Mining Camps");
                names[NameType.Complete].Add("7055", "Gold Mining");
                names[NameType.Complete].Add("7276", "Stone Mining");
                names[NameType.Complete].Add("7180", "Gold Shaft Mining");
                names[NameType.Complete].Add("7277", "Stone Shaft Mining");
                names[NameType.Complete].Add("5464", "Lumber Camp"); names[NameType.Acronym].Add("5464", "LC"); names[NameType.CommonPlural].Add("5464", "Lumber Camps");
                names[NameType.Complete].Add("7189", "Double-Bit Axe");
                names[NameType.Complete].Add("7190", "Bow Saw");
                names[NameType.Complete].Add("7231", "Two-Man Saw");
                names[NameType.Complete].Add("5581", "Folwark"); names[NameType.CommonPlural].Add("5581", "Folwarks");
                names[NameType.Complete].Add("5157", "Mill"); names[NameType.CommonPlural].Add("5157", "Mills");
                names[NameType.Complete].Add("5149", "Farm"); names[NameType.CommonPlural].Add("5149", "Farms");
                names[NameType.Complete].Add("5161", "Market"); names[NameType.Acronym].Add("5161", "Mkt"); names[NameType.CommonPlural].Add("5161", "Markets");
                names[NameType.Complete].Add("7014", "Horse Collar");
                names[NameType.Complete].Add("13319", "Trade Cart"); names[NameType.CommonPlural].Add("13319", "Trade Carts");
                names[NameType.Complete].Add("7023", "Coinage");
                names[NameType.Complete].Add("7410", "Caravan");
                names[NameType.Complete].Add("7017", "Banking");
                names[NameType.Complete].Add("7015", "Guilds");
                names[NameType.Complete].Add("7013", "Heavy Plow");
                names[NameType.Complete].Add("7012", "Crop Rotation");
                names[NameType.Complete].Add("5667", "Jaguar Warrior"); names[NameType.Common].Add("5667", "Jaguar"); names[NameType.Abbreviation].Add("5667", "Jag"); names[NameType.CommonPlural].Add("5667", "Jaguars"); names[NameType.AbbreviationPlural].Add("5667", "Jags");
                names[NameType.Complete].Add("5134", "Camel Archer"); names[NameType.CommonPlural].Add("5134", "Camel Archers");
                names[NameType.Complete].Add("5561", "Hussite Wagon"); names[NameType.CommonPlural].Add("5561", "Hussite Wagons");
                names[NameType.Complete].Add("5107", "Longbowman"); names[NameType.Acronym].Add("5107", "LBow|LB"); names[NameType.CommonPlural].Add("5107", "Longbowmen"); names[NameType.AcronymPlural].Add("5107", "LBows");
                names[NameType.Complete].Add("5288", "Konnik"); names[NameType.CommonPlural].Add("5288", "Konniks");
                names[NameType.Complete].Add("5534", "Coustillier"); names[NameType.CommonPlural].Add("5534", "Coustilliers");
                names[NameType.Complete].Add("5151", "Arambai"); names[NameType.CommonPlural].Add("5151", "Arambais");
                names[NameType.Complete].Add("5101", "Cataphract"); names[NameType.Abbreviation].Add("5101", "Cata"); names[NameType.CommonPlural].Add("5101", "Cataphracts"); names[NameType.AbbreviationPlural].Add("5101", "Catas");
                names[NameType.Complete].Add("5113", "Woad Raider"); names[NameType.Abbreviation].Add("5113", "Woad"); names[NameType.CommonPlural].Add("5113", "Woad Raiders"); names[NameType.AbbreviationPlural].Add("5113", "Woads");
                names[NameType.Complete].Add("5102", "Chu Ko Nu"); names[NameType.Abbreviation].Add("5102", "Chuck"); names[NameType.Acronym].Add("5102", "CKN"); names[NameType.CommonPlural].Add("5102", "Chu Ko Nus"); names[NameType.AbbreviationPlural].Add("5102", "Chucks");
                names[NameType.Complete].Add("5315", "Kipchak"); names[NameType.CommonPlural].Add("5315", "Kipchaks");
                names[NameType.Complete].Add("5143", "Shotel Warrior"); names[NameType.Common].Add("5143", "Shotel"); names[NameType.CommonPlural].Add("5143", "Shotels");
                names[NameType.Complete].Add("5111", "Throwing Axeman"); names[NameType.Abbreviation].Add("5111", "Axe"); names[NameType.Acronym].Add("5111", "TA"); names[NameType.CommonPlural].Add("5111", "Throwing Axemen"); names[NameType.AbbreviationPlural].Add("5111", "Axes");
                names[NameType.Complete].Add("5104", "Huskarl"); names[NameType.Abbreviation].Add("5104", "Husk|Husky"); names[NameType.CommonPlural].Add("5104", "Huskarls"); names[NameType.AbbreviationPlural].Add("5104", "Husks|Huskies");
                names[NameType.Complete].Add("5675", "Tarkan"); names[NameType.CommonPlural].Add("5675", "Tarkans");
                names[NameType.Complete].Add("5686", "Kamayuk"); names[NameType.Abbreviation].Add("5686", "Kama"); names[NameType.CommonPlural].Add("5686", "Kamayuks"); names[NameType.AbbreviationPlural].Add("5686", "Kamas");
                names[NameType.Complete].Add("5656", "Elephant Archer"); names[NameType.Abbreviation].Add("5656", "Ele Archer|Ele Arch"); names[NameType.CommonPlural].Add("5656", "Elephant Archers"); names[NameType.AbbreviationPlural].Add("5656", "Ele Archers|Ele Archs");
                names[NameType.Complete].Add("5723", "Genoese Crossbowman"); names[NameType.Abbreviation].Add("5723", "Geno|Genbow"); names[NameType.Acronym].Add("5723", "GXbow|GenXBow"); names[NameType.CommonPlural].Add("5723", "Genoese Crossbowmen"); names[NameType.AbbreviationPlural].Add("5723", "Genbows|Genos"); names[NameType.AcronymPlural].Add("5723", "GXbows|GenXBows");
                names[NameType.Complete].Add("5110", "Samurai"); names[NameType.Abbreviation].Add("5110", "Sam"); names[NameType.CommonPlural].Add("5110", "Samurais"); names[NameType.AbbreviationPlural].Add("5110", "Sams");
                names[NameType.Complete].Add("5146", "Ballista Elephant"); names[NameType.Abbreviation].Add("5146", "Ballista Ele"); names[NameType.CommonPlural].Add("5146", "Ballista Elephants"); names[NameType.AbbreviationPlural].Add("5146", "Ballista Eles");
                names[NameType.Complete].Add("5727", "War Wagon"); names[NameType.Acronym].Add("5727", "WW"); names[NameType.CommonPlural].Add("5727", "War Wagons");
                names[NameType.Complete].Add("5328", "Leitis"); names[NameType.CommonPlural].Add("5328", "Leitises");
                names[NameType.Complete].Add("5728", "Magyar Huszar"); names[NameType.Common].Add("5728", "Huszar"); names[NameType.Abbreviation].Add("5728", "Mag Hus"); names[NameType.CommonPlural].Add("5728", "Huszars");
                names[NameType.Complete].Add("5148", "Karambit Warrior"); names[NameType.Common].Add("5148", "Karambit"); names[NameType.CommonPlural].Add("5148", "Karambits");
                names[NameType.Complete].Add("5140", "Gbeto"); names[NameType.CommonPlural].Add("5140", "Gbetos");
                names[NameType.Complete].Add("5683", "Plumed Archer"); names[NameType.Abbreviation].Add("5683", "Plume|Plum"); names[NameType.CommonPlural].Add("5683", "Plumed Archers"); names[NameType.AbbreviationPlural].Add("5683", "Plumes|Plums");
                names[NameType.Complete].Add("5108", "Mangudai"); names[NameType.Abbreviation].Add("5108", "Mangu"); names[NameType.CommonPlural].Add("5108", "Mangudais"); names[NameType.AbbreviationPlural].Add("5108", "Mangus");
                names[NameType.Complete].Add("5109", "War Elephant"); names[NameType.Abbreviation].Add("5109", "War Ele"); names[NameType.Acronym].Add("5109", "WE"); names[NameType.CommonPlural].Add("5109", "War Elephants"); names[NameType.AbbreviationPlural].Add("5109", "War Eles");
                names[NameType.Complete].Add("5558", "Obuch"); names[NameType.CommonPlural].Add("5558", "Obuchs");
                names[NameType.Complete].Add("5129", "Organ Gun"); names[NameType.CommonPlural].Add("5129", "Organ Guns");
                names[NameType.Complete].Add("5103", "Mameluke"); names[NameType.Abbreviation].Add("5103", "Mam"); names[NameType.CommonPlural].Add("5103", "Mamelukes"); names[NameType.AbbreviationPlural].Add("5103", "Mams");
                names[NameType.Complete].Add("5538", "Serjeant"); names[NameType.CommonPlural].Add("5538", "Serjeants");
                names[NameType.Complete].Add("5447", "Boyar"); names[NameType.CommonPlural].Add("5447", "Boyars");
                names[NameType.Complete].Add("5687", "Conquistador"); names[NameType.Abbreviation].Add("5687", "Conq"); names[NameType.CommonPlural].Add("5687", "Conquistadores"); names[NameType.AbbreviationPlural].Add("5687", "Conqs");
                names[NameType.Complete].Add("5313", "Keshik"); names[NameType.CommonPlural].Add("5313", "Keshik");
                names[NameType.Complete].Add("19134", "Flaming Camel"); names[NameType.CommonPlural].Add("19134", "Flaming Camels");
                names[NameType.Complete].Add("5112", "Teutonic Knight"); names[NameType.Acronym].Add("5112", "TK"); names[NameType.CommonPlural].Add("5112", "Teutonic Knights");
                names[NameType.Complete].Add("5105", "Janissary"); names[NameType.Abbreviation].Add("5105", "Jan|Janny"); names[NameType.CommonPlural].Add("5105", "Janissaries"); names[NameType.AbbreviationPlural].Add("5105", "Jans|Jannies");
                names[NameType.Complete].Add("5165", "Rattan Archer"); names[NameType.Common].Add("5165", "Rattan"); names[NameType.CommonPlural].Add("5165", "Rattans");
                names[NameType.Complete].Add("5076", "Berserk"); names[NameType.Abbreviation].Add("5076", "Zerk"); names[NameType.CommonPlural].Add("5076", "Berserks"); names[NameType.AbbreviationPlural].Add("5076", "Zerks");
                names[NameType.Complete].Add("5669", "Elite Jaguar Warrior");
                names[NameType.Complete].Add("5136", "Elite Camel Archer");
                names[NameType.Complete].Add("5563", "Elite Hussite Wagon");
                names[NameType.Complete].Add("5456", "Elite Longbowman");
                names[NameType.Complete].Add("5290", "Elite Konnik");
                names[NameType.Complete].Add("5536", "Elite Coustillier");
                names[NameType.Complete].Add("5152", "Elite Arambai");
                names[NameType.Complete].Add("5451", "Elite Cataphract");
                names[NameType.Complete].Add("5463", "Elite Woad Raider");
                names[NameType.Complete].Add("5452", "Elite Chu Ko Nu");
                names[NameType.Complete].Add("5327", "Elite Kipchak");
                names[NameType.Complete].Add("5145", "Elite Shotel Warrior");
                names[NameType.Complete].Add("5461", "Elite Throwing Axeman");
                names[NameType.Complete].Add("5454", "Elite Huskarl");
                names[NameType.Complete].Add("5677", "Elite Tarkan");
                names[NameType.Complete].Add("5688", "Elite Kamayuk");
                names[NameType.Complete].Add("5657", "Elite Elephant Archer");
                names[NameType.Complete].Add("5725", "Elite Genoese Crossbowman");
                names[NameType.Complete].Add("5460", "Elite Samurai");
                names[NameType.Complete].Add("5147", "Elite Ballista Elephant");
                names[NameType.Complete].Add("5729", "Elite War Wagon");
                names[NameType.Complete].Add("5329", "Elite Leitis");
                names[NameType.Complete].Add("5730", "Elite Magyar Huszar");
                names[NameType.Complete].Add("5150", "Elite Karambit Warrior");
                names[NameType.Complete].Add("5141", "Elite Gbeto");
                names[NameType.Complete].Add("5685", "Elite Plumed Archer");
                names[NameType.Complete].Add("5458", "Elite Mangudai");
                names[NameType.Complete].Add("5459", "Elite War Elephant");
                names[NameType.Complete].Add("5559", "Elite Obuch");
                names[NameType.Complete].Add("5130", "Elite Organ Gun");
                names[NameType.Complete].Add("5453", "Elite Mameluke");
                names[NameType.Complete].Add("5540", "Elite Serjeant");
                names[NameType.Complete].Add("5449", "Elite Boyar");
                names[NameType.Complete].Add("5689", "Elite Conquistador");
                names[NameType.Complete].Add("5314", "Elite Keshik");
                names[NameType.Complete].Add("5462", "Elite Teutonic Knight");
                names[NameType.Complete].Add("5455", "Elite Janissary");
                names[NameType.Complete].Add("5166", "Elite Rattan Archer");
                names[NameType.Complete].Add("5576", "Elite Berserk");
                names[NameType.Complete].Add("7326", "Atlatl");
                names[NameType.Complete].Add("7429", "Garland Wars");
                names[NameType.Complete].Add("7256", "Kasbah");
                names[NameType.Complete].Add("7257", "Maghrebi Camels");
                names[NameType.Complete].Add("7350", "Wagenburg Tactics");
                names[NameType.Complete].Add("7351", "Hussite Reforms");
                names[NameType.Complete].Add("7419", "Yeomen");
                names[NameType.Complete].Add("7327", "Warwolf");
                names[NameType.Complete].Add("7307", "Stirrups");
                names[NameType.Complete].Add("7308", "Bagains");
                names[NameType.Complete].Add("7342", "Burgundian Vineyards");
                names[NameType.Complete].Add("7343", "Flemish Revolution"); names[NameType.Abbreviation].Add("7343", "FlemRev");
                names[NameType.Complete].Add("7295", "Howdah");
                names[NameType.Complete].Add("7296", "Manipur Cavalry");
                names[NameType.Complete].Add("7313", "Greek Fire");
                names[NameType.Complete].Add("7318", "Logistica");
                names[NameType.Complete].Add("7369", "Stronghold");
                names[NameType.Complete].Add("7421", "Furor Celtica");
                names[NameType.Complete].Add("7368", "Great Wall");
                names[NameType.Complete].Add("7432", "Rocketry");
                names[NameType.Complete].Add("7311", "Steppe Husbandry");
                names[NameType.Complete].Add("7398", "Cuman Mercenaries");
                names[NameType.Complete].Add("7252", "Royal Heirs");
                names[NameType.Complete].Add("7253", "Torsion Engines");
                names[NameType.Complete].Add("7324", "Bearded Axe");
                names[NameType.Complete].Add("7287", "Chivalry");
                names[NameType.Complete].Add("7427", "Anarchy");
                names[NameType.Complete].Add("7439", "Perfusion");
                names[NameType.Complete].Add("7370", "Marauders");
                names[NameType.Complete].Add("7428", "Atheism");
                names[NameType.Complete].Add("7266", "Andean Sling");
                names[NameType.Complete].Add("7267", "Fabric Shields");
                names[NameType.Complete].Add("7270", "Sultans");
                names[NameType.Complete].Add("7271", "Shatagni");
                names[NameType.Complete].Add("7272", "Pavise");
                names[NameType.Complete].Add("7273", "Silk Road");
                names[NameType.Complete].Add("7371", "Yasama");
                names[NameType.Complete].Add("7059", "Kataparuto");
                names[NameType.Complete].Add("7291", "Tusk Swords");
                names[NameType.Complete].Add("7292", "Double Crossbow");
                names[NameType.Complete].Add("7380", "Eupseong");
                names[NameType.Complete].Add("7438", "Shinkichon");
                names[NameType.Complete].Add("7399", "Hill Forts");
                names[NameType.Complete].Add("7400", "Tower Shields");
                names[NameType.Complete].Add("7275", "Corvinian Army");
                names[NameType.Complete].Add("7274", "Recurve Bow");
                names[NameType.Complete].Add("7293", "Thalassocracy");
                names[NameType.Complete].Add("7294", "Forced Levy");
                names[NameType.Complete].Add("7254", "Tigui");
                names[NameType.Complete].Add("7255", "Farimba");
                names[NameType.Complete].Add("7379", "Hul'che Javelineers");
                names[NameType.Complete].Add("7420", "El Dorado");
                names[NameType.Complete].Add("7280", "Nomads");
                names[NameType.Complete].Add("7422", "Drill");
                names[NameType.Complete].Add("7281", "Kamandaran");
                names[NameType.Complete].Add("7423", "Mahouts");
                names[NameType.Complete].Add("7348", "Szlachta Privileges");
                names[NameType.Complete].Add("7349", "Lechitic Legacy");
                names[NameType.Complete].Add("7250", "Carrack");
                names[NameType.Complete].Add("7251", "Arquebus");
                names[NameType.Complete].Add("7284", "Madrasah");
                names[NameType.Complete].Add("7424", "Zealotry");
                names[NameType.Complete].Add("7344", "First Crusade");
                names[NameType.Complete].Add("7345", "Hauberk");
                names[NameType.Complete].Add("7268", "Orthodoxy");
                names[NameType.Complete].Add("7269", "Druzhina");
                names[NameType.Complete].Add("7286", "Inquisition");
                names[NameType.Complete].Add("7325", "Supremacy");
                names[NameType.Complete].Add("7309", "Silk Armor");
                names[NameType.Complete].Add("7310", "Timurid Siegecraft");
                names[NameType.Complete].Add("7283", "Ironclad");
                names[NameType.Complete].Add("7426", "Crenellations");
                names[NameType.Complete].Add("7285", "Sipahi");
                names[NameType.Complete].Add("7425", "Artillery");
                names[NameType.Complete].Add("7297", "Chatras");
                names[NameType.Complete].Add("7298", "Paper Money");
                names[NameType.Complete].Add("7312", "Chieftains");
                names[NameType.Complete].Add("7431", "Berserkergang");
                names[NameType.Complete].Add("8100", "Upgrade to Crossbowman");
                names[NameType.Complete].Add("8233", "Upgrade to Arbalester");
                names[NameType.Complete].Add("8098", "Upgrade to Elite Skirmisher");
                names[NameType.Complete].Add("8300", "Upgrade to Imperial Skirmisher");
                names[NameType.Complete].Add("8218", "Upgrade to Heavy Cavalry Archer");
                names[NameType.Complete].Add("8239", "Upgrade to Elite Genitour");
                names[NameType.Complete].Add("8230", "Upgrade to Man-at-Arms");
                names[NameType.Complete].Add("8204", "Upgrade to Long Swordsman");
                names[NameType.Complete].Add("8217", "Upgrade to Two-Handed Swordsman");
                names[NameType.Complete].Add("8260", "Upgrade to Champion");
                names[NameType.Complete].Add("8176", "Upgrade to Pikeman");
                names[NameType.Complete].Add("8414", "Upgrade to Halberdier");
                names[NameType.Complete].Add("8413", "Upgrade to Eagle Warrior");
                names[NameType.Complete].Add("8433", "Upgrade to Elite Eagle Warrior");
                names[NameType.Complete].Add("8261", "Upgrade to Light Cavalry");
                names[NameType.Complete].Add("8417", "Upgrade to Hussar");
                names[NameType.Complete].Add("8352", "Upgrade to Winged Hussar");
                names[NameType.Complete].Add("8207", "Upgrade to Cavalier");
                names[NameType.Complete].Add("8259", "Upgrade to Paladin");
                names[NameType.Complete].Add("8235", "Upgrade to Heavy Camel Rider");
                names[NameType.Complete].Add("8236", "Upgrade to Imperial Camel Rider");
                names[NameType.Complete].Add("8299", "Upgrade to Elite Battle Elephant");
                names[NameType.Complete].Add("8402", "Upgrade to Elite Steppe Lancer");
                names[NameType.Complete].Add("8096", "Upgrade to Capped Ram");
                names[NameType.Complete].Add("8263", "Upgrade to Siege Ram");
                names[NameType.Complete].Add("8262", "Upgrade to Onager");
                names[NameType.Complete].Add("8323", "Upgrade to Siege Onager");
                names[NameType.Complete].Add("8244", "Upgrade to Heavy Scorpion");
                names[NameType.Complete].Add("8353", "Upgrade to Houfnice");
                names[NameType.Complete].Add("8034", "Upgrade to War Galley, Fire Ships and Demolition Ships");
                names[NameType.Complete].Add("19347", "Tech: Elite Cannon Galleon");
                names[NameType.Complete].Add("8242", "Upgrade to Heavy Demolition Ship");
                names[NameType.Complete].Add("8243", "Upgrade to Fast Fire Ship");
                names[NameType.Complete].Add("8035", "Upgrade to Galleon");
                names[NameType.Complete].Add("8437", "Upgrade to Elite Turtle Ship");
                names[NameType.Complete].Add("8393", "Upgrade to Elite Longboat");
                names[NameType.Complete].Add("8240", "Upgrade to Elite Caravel");
                names[NameType.Complete].Add("8163", "Upgrade to Fortified Wall");
                names[NameType.Complete].Add("8186", "Upgrade to Guard Tower");
                names[NameType.Complete].Add("8063", "Upgrade to Keep");
                names[NameType.Complete].Add("19498", "Tech: Bombard Tower");
                names[NameType.Complete].Add("8434", "Upgrade to Elite Jaguar Warrior");
                names[NameType.Complete].Add("8238", "Upgrade to Elite Camel Archer");
                names[NameType.Complete].Add("8347", "Upgrade to Elite Hussite Wagon");
                names[NameType.Complete].Add("8381", "Upgrade to Elite Longbowman");
                names[NameType.Complete].Add("8303", "Upgrade to Elite Konnik");
                names[NameType.Complete].Add("8340", "Upgrade to Elite Coustillier");
                names[NameType.Complete].Add("8289", "Upgrade to Elite Arambai");
                names[NameType.Complete].Add("8382", "Upgrade to Elite Cataphract");
                names[NameType.Complete].Add("8383", "Upgrade to Elite Woad Raider");
                names[NameType.Complete].Add("8384", "Upgrade to Elite Chu Ko Nu");
                names[NameType.Complete].Add("8305", "Upgrade to Elite Kipchak");
                names[NameType.Complete].Add("8245", "Upgrade to Elite Shotel Warrior");
                names[NameType.Complete].Add("8394", "Upgrade to Elite Throwing Axeman");
                names[NameType.Complete].Add("8386", "Upgrade to Elite Huskarl");
                names[NameType.Complete].Add("8418", "Upgrade to Elite Tarkan");
                names[NameType.Complete].Add("8265", "Upgrade to Elite Kamayuk");
                names[NameType.Complete].Add("8397", "Upgrade to Elite Elephant Archer");
                names[NameType.Complete].Add("8396", "Upgrade to Elite Genoese Crossbowman");
                names[NameType.Complete].Add("8387", "Upgrade to Elite Samurai");
                names[NameType.Complete].Add("8247", "Upgrade to Elite Ballista Elephant");
                names[NameType.Complete].Add("8436", "Upgrade to Elite War Wagon");
                names[NameType.Complete].Add("8306", "Upgrade to Elite Leitis");
                names[NameType.Complete].Add("8395", "Upgrade to Elite Magyar Huszar");
                names[NameType.Complete].Add("8248", "Upgrade to Elite Karambit Warrior");
                names[NameType.Complete].Add("8241", "Upgrade to Elite Gbeto");
                names[NameType.Complete].Add("8430", "Upgrade to Elite Plumed Archer");
                names[NameType.Complete].Add("8388", "Upgrade to Elite Mangudai");
                names[NameType.Complete].Add("8389", "Upgrade to Elite War Elephant");
                names[NameType.Complete].Add("8346", "Upgrade to Elite Obuch");
                names[NameType.Complete].Add("8237", "Upgrade to Elite Organ Gun");
                names[NameType.Complete].Add("8390", "Upgrade to Elite Mameluke");
                names[NameType.Complete].Add("8341", "Upgrade to Elite Serjeant");
                names[NameType.Complete].Add("8264", "Upgrade to Elite Boyar");
                names[NameType.Complete].Add("8317", "Upgrade to Elite Conquistador");
                names[NameType.Complete].Add("8304", "Upgrade to Elite Keshik");
                names[NameType.Complete].Add("8391", "Upgrade to Elite Teutonic Knight");
                names[NameType.Complete].Add("8392", "Upgrade to Elite Janissary");
                names[NameType.Complete].Add("8290", "Upgrade to Elite Rattan Archer");
                names[NameType.Complete].Add("8401", "Upgrade to Elite Berserk");
                names[NameType.Complete].Add("10271", "Britons"); names[NameType.Abbreviation].Add("10271", "Brits"); names[NameType.Acronym].Add("10271", "Bri|Br");
                names[NameType.Complete].Add("10272", "Franks"); names[NameType.Acronym].Add("10272", "Fr");
                names[NameType.Complete].Add("10273", "Goths"); names[NameType.Acronym].Add("10273", "Go");
                names[NameType.Complete].Add("10274", "Teutons"); names[NameType.Abbreviation].Add("10274", "Teut"); names[NameType.Acronym].Add("10274", "Te");
                names[NameType.Complete].Add("10275", "Japanese"); names[NameType.Abbreviation].Add("10275", "Jap"); names[NameType.Acronym].Add("10275", "Ja");
                names[NameType.Complete].Add("10276", "Chinese"); names[NameType.Acronym].Add("10276", "Chi|Ch");
                names[NameType.Complete].Add("10277", "Byzantines"); names[NameType.Abbreviation].Add("10277", "Byz"); names[NameType.Acronym].Add("10277", "Bz");
                names[NameType.Complete].Add("10278", "Persians"); names[NameType.Acronym].Add("10278", "Per|Pe");
                names[NameType.Complete].Add("10279", "Saracens"); names[NameType.Abbreviation].Add("10279", "Sara"); names[NameType.Acronym].Add("10279", "Sar|Sa");
                names[NameType.Complete].Add("10280", "Turks"); names[NameType.Acronym].Add("10280", "Tur|Tu");
                names[NameType.Complete].Add("10281", "Vikings"); names[NameType.Abbreviation].Add("10281", "Vik"); names[NameType.Acronym].Add("10281", "Vk");
                names[NameType.Complete].Add("10282", "Mongols"); names[NameType.Acronym].Add("10282", "Mon|Mo");
                names[NameType.Complete].Add("10283", "Celts"); names[NameType.Acronym].Add("10283", "Cel|Ce");
                names[NameType.Complete].Add("10284", "Spanish"); names[NameType.Abbreviation].Add("10284", "Span"); names[NameType.Acronym].Add("10284", "Sp|Spa");
                names[NameType.Complete].Add("10285", "Aztecs"); names[NameType.Acronym].Add("10285", "Az|Azt");
                names[NameType.Complete].Add("10286", "Mayans"); names[NameType.Abbreviation].Add("10286", "Maya"); names[NameType.Acronym].Add("10286", "May");
                names[NameType.Complete].Add("10287", "Huns"); names[NameType.Acronym].Add("10287", "Hun|Hu");
                names[NameType.Complete].Add("10288", "Koreans"); names[NameType.Acronym].Add("10288", "Kor|Ko");
                names[NameType.Complete].Add("10289", "Italians"); names[NameType.Acronym].Add("10289", "Ital|It");
                names[NameType.Complete].Add("10290", "Indians"); names[NameType.Abbreviation].Add("10290", "Indi"); names[NameType.Acronym].Add("10290", "Id");
                names[NameType.Complete].Add("10291", "Incas"); names[NameType.Acronym].Add("10291", "Inc|Ic");
                names[NameType.Complete].Add("10292", "Magyars"); names[NameType.Abbreviation].Add("10292", "Magy"); names[NameType.Acronym].Add("10292", "Mg");
                names[NameType.Complete].Add("10293", "Slavs"); names[NameType.Acronym].Add("10293", "Sl");
                names[NameType.Complete].Add("10294", "Portuguese"); names[NameType.Abbreviation].Add("10294", "Port"); names[NameType.Acronym].Add("10294", "Por");
                names[NameType.Complete].Add("10295", "Ethiopians"); names[NameType.Abbreviation].Add("10295", "Ethi"); names[NameType.Acronym].Add("10295", "Et");
                names[NameType.Complete].Add("10296", "Malians"); names[NameType.Abbreviation].Add("10296", "Mali"); names[NameType.Acronym].Add("10296", "Mli");
                names[NameType.Complete].Add("10297", "Berbers"); names[NameType.Abbreviation].Add("10297", "Berb"); names[NameType.Acronym].Add("10297", "Be");
                names[NameType.Complete].Add("10298", "Khmer"); names[NameType.Acronym].Add("10298", "Kh");
                names[NameType.Complete].Add("10299", "Malay"); names[NameType.Acronym].Add("10299", "Mly");
                names[NameType.Complete].Add("10300", "Burmese"); names[NameType.Abbreviation].Add("10300", "Burm"); names[NameType.Acronym].Add("10300", "Bm");
                names[NameType.Complete].Add("10301", "Vietnamese"); names[NameType.Abbreviation].Add("10301", "Viet"); names[NameType.Acronym].Add("10301", "Vt");
                names[NameType.Complete].Add("10302", "Bulgarians"); names[NameType.Acronym].Add("10302", "Bul");
                names[NameType.Complete].Add("10303", "Tatars"); names[NameType.Acronym].Add("10303", "Ta");
                names[NameType.Complete].Add("10304", "Cumans"); names[NameType.Acronym].Add("10304", "Cu");
                names[NameType.Complete].Add("10305", "Lithuanians"); names[NameType.Acronym].Add("10305", "Li");
                names[NameType.Complete].Add("10306", "Burgundians"); names[NameType.Acronym].Add("10306", "Bgd");
                names[NameType.Complete].Add("10307", "Sicilians"); names[NameType.Acronym].Add("10307", "Si");
                names[NameType.Complete].Add("10308", "Poles"); names[NameType.Acronym].Add("10308", "Pol");
                names[NameType.Complete].Add("10309", "Bohemians"); names[NameType.Acronym].Add("10309", "Bo");
                names[NameType.Complete].Add("1001", "Age of Empires II"); names[NameType.Common].Add("1001", "Age of Empires 2"); names[NameType.Acronym].Add("1001", "AOE2");
                names[NameType.Complete].Add("4105", "Stop");
                names[NameType.Complete].Add("4107", "Unload");
                names[NameType.Complete].Add("4131", "Automatically Reseed Farms"); names[NameType.Common].Add("4131", "Autofarm");
                names[NameType.Complete].Add("4121", "Automatically Rebuild Fish Traps");
                names[NameType.Complete].Add("5056", "Zebra"); names[NameType.CommonPlural].Add("5056", "Zebras");
                names[NameType.Complete].Add("5057", "Ostrich"); names[NameType.CommonPlural].Add("5057", "Ostriches");
                names[NameType.Complete].Add("5059", "Lion"); names[NameType.CommonPlural].Add("5059", "Lions");
                names[NameType.Complete].Add("5060", "Crocodile"); names[NameType.CommonPlural].Add("5060", "Crocodiles");
                names[NameType.Complete].Add("5061", "Goat"); names[NameType.CommonPlural].Add("5061", "Goats");
                names[NameType.Complete].Add("5067", "Arrow"); names[NameType.CommonPlural].Add("5067", "Arrows");
                names[NameType.Complete].Add("5071", "Deer"); names[NameType.CommonPlural].Add("5071", "Deers");
                names[NameType.Complete].Add("5075", "Wolf"); names[NameType.CommonPlural].Add("5075", "Wolves");
                names[NameType.Complete].Add("5072", "Fish (Perch)"); names[NameType.Common].Add("5072", "Fish");
                names[NameType.Complete].Add("5350", "Relic"); names[NameType.Abbreviation].Add("5350", "Rel"); names[NameType.CommonPlural].Add("5350", "Relics"); names[NameType.AbbreviationPlural].Add("5350", "Rels");
                names[NameType.Complete].Add("5397", "Tree (Oak)"); names[NameType.Common].Add("5397", "Tree"); names[NameType.CommonPlural].Add("5397", "Trees");
                names[NameType.Complete].Add("5401", "Forage Bush"); names[NameType.CommonPlural].Add("5401", "Forage Bushes");
                names[NameType.Complete].Add("5406", "Wild Boar"); names[NameType.Common].Add("5406", "Boar"); names[NameType.CommonPlural].Add("5406", "Boars");
                names[NameType.Complete].Add("5498", "Sheep"); names[NameType.CommonPlural].Add("5498", "Sheeps");
                names[NameType.Complete].Add("5502", "Cow"); names[NameType.CommonPlural].Add("5502", "Cows");
                names[NameType.Complete].Add("5503", "Llama"); names[NameType.CommonPlural].Add("5503", "Llamas");
                names[NameType.Complete].Add("6039", "Civilization"); names[NameType.Abbreviation].Add("6039", "Civ"); names[NameType.CommonPlural].Add("6039", "Civilizations"); names[NameType.AbbreviationPlural].Add("6039", "Civs");
                names[NameType.Complete].Add("4122", "Stand Ground");
                names[NameType.Complete].Add("4123", "Attack Ground");
                names[NameType.Complete].Add("4124", "Heal");
                names[NameType.Complete].Add("4125", "Convert");
                names[NameType.Complete].Add("4133", "Aggressive Stance"); names[NameType.Common].Add("4133", "Aggressive");
                names[NameType.Complete].Add("4134", "Defensive Stance"); names[NameType.Common].Add("4134", "Defensive");
                names[NameType.Complete].Add("4135", "No Attack Stance"); names[NameType.Common].Add("4135", "No Attack");
                names[NameType.Complete].Add("4136", "Guard");
                names[NameType.Complete].Add("4137", "Follow");
                names[NameType.Complete].Add("4138", "Patrol");
                names[NameType.Complete].Add("4144", "Set Gather Point"); names[NameType.Common].Add("4144", "Gather Point");
                names[NameType.Complete].Add("4301", "Food"); names[NameType.Acronym].Add("4301", "F");
                names[NameType.Complete].Add("4302", "Wood"); names[NameType.Acronym].Add("4302", "W");
                names[NameType.Complete].Add("4303", "Stone"); names[NameType.Acronym].Add("4303", "S");
                names[NameType.Complete].Add("4304", "Gold"); names[NameType.Acronym].Add("4304", "G");
                names[NameType.Complete].Add("4305", "Health"); names[NameType.Acronym].Add("4305", "HP");
                names[NameType.Complete].Add("4306", "Armor");
                names[NameType.Complete].Add("4307", "Attack"); names[NameType.Abbreviation].Add("4307", "Att");
                names[NameType.Complete].Add("4313", "Population"); names[NameType.Abbreviation].Add("4313", "Pop");
                names[NameType.Complete].Add("4314", "Range");
                names[NameType.Complete].Add("4316", "Speed");
                names[NameType.Complete].Add("5170", "Tiger"); names[NameType.CommonPlural].Add("5170", "Tigers");
                names[NameType.Complete].Add("5172", "Rhinoceros"); names[NameType.Abbreviation].Add("5172", "Rhino"); names[NameType.CommonPlural].Add("5172", "Rhinoceroses"); names[NameType.AbbreviationPlural].Add("5172", "Rhinos");
                names[NameType.Complete].Add("5173", "Box Turtles"); names[NameType.Common].Add("5173", "Turtles");
                names[NameType.Complete].Add("5175", "Water Buffalo"); names[NameType.Common].Add("5175", "Buffalo"); names[NameType.CommonPlural].Add("5175", "Buffalos");
                names[NameType.Complete].Add("5743", "Elephant"); names[NameType.Abbreviation].Add("5743", "Ele"); names[NameType.CommonPlural].Add("5743", "Elephants"); names[NameType.AbbreviationPlural].Add("5743", "Eles");
                names[NameType.Complete].Add("12201", "Line of Sight"); names[NameType.Acronym].Add("12201", "LoS");
                names[NameType.Complete].Add("19322", "Unique Unit"); names[NameType.Acronym].Add("19322", "UU"); names[NameType.CommonPlural].Add("19322", "Unique Units");
                names[NameType.Complete].Add("10878", "Black Forest"); names[NameType.Acronym].Add("10878", "BF");
                names[NameType.Complete].Add("10875", "Arabia");
                names[NameType.Complete].Add("10895", "Arena");
                names[NameType.Complete].Add("10887", "Migration"); names[NameType.Abbreviation].Add("10887", "Migra");
                names[NameType.Complete].Add("10608", "Mediterranean"); names[NameType.Abbreviation].Add("10608", "Medi");
                names[NameType.Complete].Add("9653", "Random Map"); names[NameType.Acronym].Add("9653", "RM");
                names[NameType.Complete].Add("9751", "Death Match"); names[NameType.Acronym].Add("9751", "DM");
                names[NameType.Complete].Add("9762", "King of the Hill"); names[NameType.Acronym].Add("9762", "KotH");
                names[NameType.Complete].Add("9228", "Regicide"); names[NameType.Abbreviation].Add("9228", "Regi");
                names[NameType.Complete].Add("10945", "Michi");
                names[NameType.Complete].Add("10921", "Lombardia");
                names[NameType.Complete].Add("10958", "Runestones");
                names[NameType.Complete].Add("10885", "Islands");
                names[NameType.Complete].Add("10897", "Oasis");
                names[NameType.Complete].Add("5187", "Fortress");
                names[NameType.Complete].Add("4206", "Nomad");
                names[NameType.Complete].Add("10924", "MegaRandom");
                names[NameType.Complete].Add("10919", "Hideout");
                names[NameType.Complete].Add("10883", "Gold Rush");
                names[NameType.Complete].Add("10923", "Valley");
                names[NameType.Complete].Add("10891", "Scandinavia");
                names[NameType.Complete].Add("10918", "Golden Pit");
                names[NameType.Complete].Add("10894", "Yucatan");
                names[NameType.Complete].Add("10920", "Hill Fort");
                names[NameType.Complete].Add("10889", "Team Islands");
                names[NameType.Complete].Add("10922", "Steppe");
                names[NameType.Complete].Add("10898", "Ghost Lake");
                names[NameType.Complete].Add("10877", "Baltic");
                names[NameType.Complete].Add("400010", "Socotra");
                names[NameType.Complete].Add("19350", "Auto Scout");
                names[NameType.Complete].Add("10026", "Artificial Intelligence"); names[NameType.Acronym].Add("10026", "AI");
                names[NameType.Complete].Add("20205", "Pierce Armor: "); names[NameType.Common].Add("20205", "Pierce Armor"); names[NameType.Acronym].Add("20205", "PA");
                names[NameType.Complete].Add("20204", "Armor: "); names[NameType.Common].Add("20204", "Melee Armor"); names[NameType.Acronym].Add("20204", "MA");
                names[NameType.Complete].Add("400011", "Straggler Trees");
                names[NameType.Complete].Add("400012", "Long Distance");
                names[NameType.Complete].Add("400013", "Time");

                string elite(string original) => "Elite " + original.Replace("|", $"|Elite ");

                string e(string original) => "E" + original.Replace("|", $"|E");

                string upgradeElite(string original) => "Elite " + original.Replace("|", $" Upgrade|Elite ") + " Upgrade";

                string uE(string original) => "E" + original.Replace("|", $" U|E") + " U";

                string upgrade(string original) => original.Replace("|", $" Upgrade|") + " Upgrade";

                string u(string original) => original.Replace("|", $" U|") + " U";


                void autogeneración(string textoClave, Func<string, string> conversiónAcrónimos, Func<string, string> conversiónNormal, 
                    List<string> códigosAIgnorar, out List<string> códigosCoincidentes) {

                    var códigosTC = names[NameType.Complete].Where(kv => kv.Value.StartsWith(textoClave)).Select(kv => kv.Key).ToList();
                    foreach (var códigoAIgnorar in códigosAIgnorar) {
                        códigosTC.Remove(códigoAIgnorar);
                    }

                    foreach (var códigoTC in códigosTC) {

                        var nombreCompletoSinTextoClave = names[NameType.Complete][códigoTC].Replace($"{textoClave} ", "");
                        var códigoSinTC = names[NameType.Complete].FirstOrDefault(kv => kv.Value == nombreCompletoSinTextoClave).Key;
                        if (códigoSinTC != null) {

                            var tipos = new List<NameType> { NameType.Common, NameType.CommonPlural, NameType.Abbreviation, NameType.AbbreviationPlural,
                                NameType.Acronym, NameType.AcronymPlural };

                            foreach (var tipo in tipos) {

                                var nombreSinTC = names[tipo].FirstOrDefault(kv => kv.Key == códigoSinTC).Value;
                                if (nombreSinTC != null) {

                                    if (names[tipo].ContainsKey(códigoTC)) {
                                        MessageBox.Show($"{nombreCompletoSinTextoClave} already has an {textoClave} value in {tipo}. " + 
                                            $"Don't add {textoClave} values manually");
                                    } else {

                                        if (tipo == NameType.Acronym || tipo == NameType.AcronymPlural) {
                                            names[tipo].Add(códigoTC, conversiónAcrónimos(nombreSinTC));
                                        } else {
                                            names[tipo].Add(códigoTC, conversiónNormal(nombreSinTC));
                                        }

                                    }

                                } else if (tipo == NameType.Common && (textoClave == "Upgrade to" || textoClave == "Tech:" 
                                    || textoClave == "Upgrade to Elite" || textoClave == "Tech: Elite")) {

                                    names[tipo].Add(códigoTC, conversiónNormal(nombreCompletoSinTextoClave)); // Principalmente para formar los Upgrade to con el Upgrade al final para todos los textos.

                                } // Si no se tiene nombre común, abreviación o acrónimo, no se debe hacer nada.

                            }

                        } else {

                            var excepciones = new List<string> { "War Galley, Fire Ships and Demolition Ships" };
                            if (!excepciones.Contains(nombreCompletoSinTextoClave)) 
                                MessageBox.Show($"{nombreCompletoSinTextoClave} doesn't have it's non {textoClave} counterpart.");

                        }

                    }

                    códigosCoincidentes = new List<string>(códigosTC);

                } // autogeneración>

                autogeneración("Elite", e, elite, new List<string>(), out _); // Autogeneración de los Elite.
                autogeneración("Upgrade to Elite", uE, upgradeElite, new List<string>(), out List<string> códigosUpgradeToElite); // Autogeneración de los Upgrade to Elite.
                autogeneración("Tech: Elite", uE, upgradeElite, new List<string>(), out List<string> códigosTechToElite); // Autogeneración de los Tech: Elite.                                                                                                       // 
                autogeneración("Upgrade to", u, upgrade, códigosUpgradeToElite, out _); // Autogeneración de los Upgrade.
                autogeneración("Tech:", u, upgrade, códigosTechToElite, out _); // Autogeneración de los Tech:.

            } // AOE2>

            File.WriteAllText(Preferencias.EnglishNamesPath, JsonSerializer.Serialize(names, 
                ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración | Serialización.EnumeraciónEnTexto | Serialización.UTF8)));

        } // CrearArchivoNombresInglés>

        

    } // Global>



} // RTSHelper>
