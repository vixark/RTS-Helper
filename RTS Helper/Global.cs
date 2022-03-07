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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using DImg = System.Drawing.Imaging;
using SDrw = System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;



namespace RTSHelper {



    public static class Global {


        #region Variables

        public static Settings Preferencias = new Settings();

        public static bool ModoDesarrollo = false;

        public const string AOE2Name = "Age of Empires II";

        public const string AOE4Name = "Age of Empires IV";

        public static List<string> NombresVentanasJuegos = new List<string>() { "Age of Empires II: Definitive Edition", "Age of Empires IV", 
            "Age of Empires II: HD Edition" };

        public static Dictionary<string, List<string>> TextosPausa = new Dictionary<string, List<string>> { { AOE2Name,  new List<string> 
            { "Game Paused", "Jogo Pausado", "Spiel pausiert", "Partida en pausa", "Partie mise en pause", "Partita sospesa",
            "Permainan Dijeda", "Partida en pausa", "Gra wstrzymana", "Oyun Duraklatildi", "Van choi bi tam dung", "(F3)" } } }; // No se agregan "गेम रोका गया",  "一時停止", "게임 일시 중지", "遊戲暫停", "Пауза" "游戏暂停" porque Tesseract no reconoce estos carácteres. "Oyun Duraklatıldı", "Ván chơi bị tạm dừng" se llevan a alfabeto latino que es el reconocido por Tesseract. Para AOE2 no se incluye el paréntesis y la tecla usada para pausar (F3) para no incluirla en las verificaciones iniciales del texto con Contains.

        public static string OtherName = "Other";

        public static string DirectorioAOE2 = @"D:\Juegos\Steam\steamapps\common\AoE2DE";

        public static string DirectorioAplicaciónReal = @"D:\Programas\RTS Helper";

        public static string DirectorioCompilación = AppDomain.CurrentDomain.BaseDirectory ?? @"C:\"; // No veo en que situación podría ser null BaseDirectory.

        public static string DirectorioAplicación = ModoDesarrollo ? DirectorioAplicaciónReal : DirectorioCompilación; 

        public static string RutaPreferencias = Path.Combine(DirectorioAplicación, "Settings.json");

        public static string NoneSoundString = "None";

        public static string NombreFuentePredeterminada = "Tahoma";

        public static double FactorTamañoTextoAPixeles = 136D / 113; // Es un factor experimental para la fuente actual predeterminada (Tahoma) que permite convertir el tamaño de la fuente al tamaño de la imagen para que ambos sean del mismo alto. Se hace para la fuente predeterminada aunque no debería ser muy diferente con otras fuentes.

        public static string DirectorioSonidosCortos = Path.Combine(DirectorioAplicación, "Sounds", "Short");

        public static string DirectorioSonidosLargos = Path.Combine(DirectorioAplicación, "Sounds", "Long");

        public static string DirectorioNombres = Path.Combine(DirectorioAplicación, "Names");

        public static string DirectorioÓrdenesDeEjecuciónCompilación = Path.Combine(DirectorioCompilación, "Build Orders");

        public static string DirectorioÓrdenesDeEjecuciónCódigo = @"D:\Programas\RTS Helper\Código\RTS Helper\RTS Helper\Build Orders";

        public static string DirectorioÓrdenesDeEjecución = DirectorioÓrdenesDeEjecuciónCompilación; // Tanto en desarrollo como en producción es la misma carpeta porque las órdenes de ejecución se almacenan en el repositorio y se copian al directorio en la compilación.

        public static string DirectorioImágenes = Path.Combine(DirectorioAplicación, "Images");

        public static string DirectorioPruebasOCR = Path.Combine(DirectorioAplicación, "OCR Tests");

        public static string AlinearInferiormenteId = "\f";

        public static string NuevaLíneaId = "\n";

        public static double CorrecciónEscala = 1.25 / (WForms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth); // Todos los valores son calculados experimentalmente en mi computador que tiene una escala de 125, entonces para ser usado en computadores de otra escala se debe ajustar todos los valores con este factor.

        public static string SeparadorDecimales = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public enum NameType { // Al agregar un valor aquí, se debe agregar a Settings.NamesTypesPriority y en Idiomas si es un idioma.
            [Display(Name = "Complete Name")] Complete, [Display(Name = "Common Name")] Common, Abbreviation, Acronym,
            [Display(Name = "Plural Common Name")] CommonPlural, [Display(Name = "Plural Abbreviation")] AbbreviationPlural, 
            [Display(Name = "Plural Acronym")] AcronymPlural, // Todos los anteriores son normalmente en inglés.
            [Display(Name = "Custom Name")] Custom, Image, BR, DE, ES, FR, HI, IT, JP, KO, MS, MX, PL, RU, TR, TW, VI, ZH // TW es equivalente Chino tradicional según las pruebas que he hecho. ZH es Chino Simplificado
        }

        public static Dictionary<string, string> IdiomasJuego = new Dictionary<string, string> { { "EN", "English" }, { "BR", "Português (Brasil)" }, 
            {"DE", "Deutsch" }, { "ES", "Español" }, { "FR", "Français" }, { "HI", "हिंदी" }, { "IT", "Italiano"}, { "JP", "日本語"}, { "KO", "한국어"}, 
            { "MS", "Bahasa Melayu"}, { "MX", "Español (México)"}, { "PL", "Polski"}, { "RU", "Русский"}, { "TR", "Türkçe"}, { "TW", "繁體中文"},
            { "VI", "Tiếng Việt"}, { "ZH", "简体中文"},
        };

        public static Dictionary<string, string> IdiomasNombres = new Dictionary<string, string> { { "BR", "Português (Brasil)" }, {"DE", "Deutsch" },
            { "ES", "Español" }, { "FR", "Français" }, { "HI", "हिंदी" }, { "IT", "Italiano"}, { "JP", "日本語"}, { "KO", "한국어"}, { "MS", "Bahasa Melayu"}, 
            { "MX", "Español (México)"}, { "PL", "Polski"}, { "RU", "Русский"}, { "TR", "Türkçe"}, { "TW", "繁體中文"}, { "VI", "Tiếng Việt"}, 
            { "ZH", "简体中文"},
        };

        public enum TipoSegmento { Entidad, Texto, Imagen } // Entidad puede convertirse en Texto o Imagen según las preferencias del usuario.

        public enum PosiciónTexto { Indeterminado, Normal, Subíndice, Superíndice }

        public enum TamañosFuente { Indeterminado, XXXS, XXS, XS, S, M, L, XL, XXL, XXXL }

        public enum TipoFuente { Sans, SansNegrita, Serif, SerifCuadrada, Caligráfica, Símbolos }

        public static Dictionary<string, Nombre> Nombres = new Dictionary<string, Nombre>(); // Los nombres no repetidos. La utilidad de este diccionario es principalmente para identificar nombres entre [] y reemplazarlo por la entidad correspondiente. La primera vez que aparezca un nombre sin importar en que idioma esté se agregará a este diccionario con ese idioma, los demás se ignoran. La clave son todos los nombres posibles.

        public static List<(string, Nombre)> TodosLosNombres = new List<(string, Nombre)>(); // Todos los nombres. Su uso es obtener el valor de cierta entidad en cierto tipo/idioma. La clave son todos los nombres posibles.

        public static Dictionary<string, Entidad> Entidades = new Dictionary<string, Entidad>(); // La clave es el ID de la entidad.

        public static Dictionary<string, string> Imágenes = new Dictionary<string, string>(); // La clave es el nombre único de la imagen (nombre del archivo) y el valor es la ruta. No se deben repetir nombres de imágenes, incluso si están en diferentes carpetas.

        public static List<string> Estilos { get; set; } = new List<string> { "b", "i", "u", "nb" };

        public static List<string> Tamaños { get; set; } = new List<string> { "xxxs", "xxs", "xs", "s", "m", "l", "xl", "xxl", "xxxl" }; // xxxl x3, xxl x2, xl x1.5, l x1.3, m x1, s x1/1,3, xs x1/1,5, xxs x1/2, xxxs x1/3. 

        public static List<string> Posiciones { get; set; } = new List<string> { "sup", "sub", "normalpos" };

        public static List<string> Colores { get; set; } = new List<string> { "aliceblue", "antiquewhite", "aqua", "aquamarine", "azure", "beige", 
            "bisque", "black", "blanchedalmond", "blue", "blueviolet", "brown", "burlywood", "cadetblue", "chartreuse", "chocolate", "coral", 
            "cornflowerblue", "cornsilk", "crimson", "cyan", "darkblue", "darkcyan", "darkgoldenrod", "darkgray", "darkgreen", "darkkhaki", 
            "darkmagenta", "darkolivegreen", "darkorange", "darkorchid", "darkred", "darksalmon", "darkseagreen", "darkslateblue", "darkslategray", 
            "darkturquoise", "darkviolet", "deeppink", "deepskyblue", "dimgray", "dodgerblue", "firebrick", "floralwhite", "forestgreen", "fuchsia", 
            "gainsboro", "ghostwhite", "gold", "goldenrod", "gray", "green", "greenyellow", "honeydew", "hotpink", "indianred", "indigo", "ivory", 
            "khaki", "lavender", "lavenderblush", "lawngreen", "lemonchiffon", "lightblue", "lightcoral", "lightcyan", "lightgoldenrodyellow", 
            "lightgray", "lightgreen", "lightpink", "lightsalmon", "lightseagreen", "lightskyblue", "lightslategray", "lightsteelblue", "lightyellow", 
            "lime", "limegreen", "linen", "magenta", "maroon", "mediumaquamarine", "mediumblue", "mediumorchid", "mediumpurple", "mediumseagreen", 
            "mediumslateblue", "mediumspringgreen", "mediumturquoise", "mediumvioletred", "midnightblue", "mintcream", "mistyrose", "moccasin", 
            "navajowhite", "navy", "oldlace", "olive", "olivedrab", "orange", "orangered", "orchid", "palegoldenrod", "palegreen", "paleturquoise", 
            "palevioletred", "papayawhip", "peachpuff", "peru", "pink", "plum", "powderblue", "purple", "red", "rosybrown", "royalblue", "saddlebrown", 
            "salmon", "sandybrown", "seagreen", "seashell", "sienna", "silver", "skyblue", "slateblue", "slategray", "snow", "springgreen", "steelblue", 
            "tan", "teal", "thistle", "tomato", "transparent", "turquoise", "violet", "wheat", "white", "whitesmoke", "yellow", "yellowgreen" };

        public static Dictionary<string, Fuente> Fuentes = new Dictionary<string, Fuente> {
            {"arial", new Fuente("Arial", TipoFuente.Sans, false, false, false, false)},
            {"arialblack", new Fuente("Arial Black", TipoFuente.SansNegrita, false, false, false, false)},
            {"calibri", new Fuente("Calibri", TipoFuente.Sans, true, true, true, false)},
            {"cambria", new Fuente("Cambria", TipoFuente.Serif, true, true, true, false)},
            {"cambria math", new Fuente("Cambria Math", TipoFuente.Serif, true, true, true, false)},
            {"candara", new Fuente("Candara", TipoFuente.Sans, true, true, false, false)},
            {"comicsansms", new Fuente("Comic Sans MS", TipoFuente.Sans, false, false, false, false)},
            {"consolas", new Fuente("Consolas", TipoFuente.Sans, false, false, false, false)},
            {"constantia", new Fuente("Constantia", TipoFuente.Serif, true, false, false, false)},
            {"corbel", new Fuente("Corbel", TipoFuente.Sans, true, true, false, false)},
            {"couriernew", new Fuente("Courier New", TipoFuente.SerifCuadrada, false, false, false, false)},
            {"ebrima", new Fuente("Ebrima", TipoFuente.Sans, true, false, false, false)},
            {"franklingothicmedium", new Fuente("Franklin Gothic Medium", TipoFuente.SansNegrita, false, false, false, false)},
            {"gabriola", new Fuente("Gabriola", TipoFuente.Caligráfica, true, true, false, false)},
            {"georgia", new Fuente("Georgia", TipoFuente.Serif, true, false, false, false)},
            {"impact", new Fuente("Impact", TipoFuente.SansNegrita, true, false, false, false)},
            {"lucidaconsole", new Fuente("Lucida Console", TipoFuente.Sans, false, false, false, false)},
            {"lucidasansunicode", new Fuente("Lucida Sans Unicode", TipoFuente.Sans, false, false, false, false)},
            {"malgungothic", new Fuente("Malgun Gothic", TipoFuente.Sans, false, false, false, false)},
            {"microsofthimalaya", new Fuente("Microsoft Himalaya", TipoFuente.Serif, false, false, false, false)},
            {"microsoftjhenghei", new Fuente("Microsoft JhengHei", TipoFuente.Sans, false, false, false, false)},
            {"microsoftnewtai lue", new Fuente("Microsoft New Tai Lue", TipoFuente.Sans, false, false, false, false)},
            {"microsoftphagspa", new Fuente("Microsoft PhagsPa", TipoFuente.Sans, false, false, false, false)},
            {"microsoftsansserif", new Fuente("Microsoft Sans Serif", TipoFuente.Sans, false, false, false, false)},
            {"microsofttaile", new Fuente("Microsoft Tai Le", TipoFuente.Sans, false, false, false, false)},
            {"microsoftyahei", new Fuente("Microsoft YaHei", TipoFuente.Sans, false, false, false, false)},
            {"microsoftyibaiti", new Fuente("Microsoft Yi Baiti", TipoFuente.Sans, false, false, false, false)},
            {"mongolianbaiti", new Fuente("Mongolian Baiti", TipoFuente.Serif, false, false, false, false)},
            {"msgothic", new Fuente("MS Gothic", TipoFuente.SansNegrita, false, false, false, false)},
            {"mvboli", new Fuente("MV Boli", TipoFuente.Caligráfica, false, false, false, false)},
            {"palatinolinotype", new Fuente("Palatino Linotype", TipoFuente.Serif, true, true, true, true)},
            {"segoeprint", new Fuente("Segoe Print", TipoFuente.Caligráfica, false, false, false, false)},
            {"segoescript", new Fuente("Segoe Script", TipoFuente.Caligráfica, false, false, false, false)},
            {"segoeui", new Fuente("Segoe UI", TipoFuente.Sans, true, true, false, false)},
            {"segoeuisymbol", new Fuente("Segoe UI Symbol", TipoFuente.Sans, false, false, false, false)},
            {"simsun", new Fuente("SimSun", TipoFuente.Serif, false, false, false, false)},
            {"sylfaen", new Fuente("Sylfaen", TipoFuente.Serif, false, false, false, false)},
            {"symbol", new Fuente("Symbol", TipoFuente.Serif, false, false, false, false)},
            {"tahoma", new Fuente("Tahoma", TipoFuente.Sans, false, false, false, false)},
            {"timesnewroman", new Fuente("Times New Roman", TipoFuente.Serif, false, false, false, false)},
            {"trebuchetms", new Fuente("Trebuchet MS", TipoFuente.Sans, true, false, false, false)},
            {"verdana", new Fuente("Verdana", TipoFuente.Sans, true, false, false, false)},
            {"webdings", new Fuente("Webdings", TipoFuente.Símbolos, false, false, false, false)},
            {"wingdings", new Fuente("Wingdings", TipoFuente.Símbolos, false, false, false, false)},
        };

        public static Dictionary<string, SolidColorBrush> BrochasDeColorSólido = new Dictionary<string, SolidColorBrush>();

        public static Dictionary<string, BitmapImage> Bitmaps = new Dictionary<string, BitmapImage>();

        public static string BackColorPredeterminado = Color.FromRgb(0, 0, 0).ToString();

        public static string FontColorPredeterminado = Color.FromRgb(150, 150, 150).ToString();

        public static double OpacityPredeterminado = 0.7;

        public static string CurrentStepFontColorPredeterminado = Color.FromRgb(220, 220, 220).ToString();

        public static string NextPreviousStepFontColorPredeterminado = Color.FromRgb(150, 150, 150).ToString();

        public static bool FlashOnStepChangePredeterminado = true;

        public static string FlashingColorPredeterminado = "#498205";

        public static double FlashingOpacityPredeterminado = 1;

        public static bool StopFlashingOnCompletePredeterminado = true;

        public static string StepStartSoundPredeterminado = NoneSoundString; // Antes se tenía "Ticket Machine 2.mp3", pero se consideró que por defecto el programa funciona suficientemente bien con el color flash sin sonidos, aunque se podrían especificar un sonido por defecto según el juego/aplicación que se le vaya a dar.

        public static string StepEndSoundPredeterminado = NoneSoundString; // Antes se tenía "Stopwatch.mp3", ver explicación de decisión en StepStartSoundPredeterminado.

        public static int StepStartSoundVolumePredeterminado = 50;

        public static int StepEndSoundVolumePredeterminado = 20;

        public static double StepDurationPredeterminado = 25; // 50 es la duración de la creación de 2 aldeanos en Age of Empires II, es un valor adecuado para generar instrucciones de a dos aldeanos. También se puede usar 25 que es el tiempo de creación de 1 aldeano.

        public static double ImageSizePredeterminado = 138; // 138 permite tener 4 líneas de imágenes.

        public static double SubscriptAndSuperscriptImagesSizePredeterminado = 50;

        public static bool CurrentStepFontBoldPredeterminado = false;

        public static bool ShowNextStepPredeterminado = false;

        public static bool ShowPreviousStepPredeterminado = false;

        public static ParámetrosExtracción NNL2C2x1 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true, 
            blancoYNegro: true, escala: 1, luminosidad: 2, contraste: 2, InterpolationMode.NearestNeighbor, DImg.PixelFormat.Format8bppIndexed); // Por alguna razón desconocida es necesario que el bitmap sea de 8 bits para que Tesseract lo reconozca adecuadamente. Se usa NearestNeighbor porque lo exige el constructor, pero al tener escala 1, no se aplica ningún modo de interpolación.

        public static ParámetrosExtracción NNL1C2x1 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true,
            blancoYNegro: true, escala: 1, luminosidad: 1, contraste: 2, InterpolationMode.NearestNeighbor, DImg.PixelFormat.Format8bppIndexed); // Por alguna razón desconocida es necesario que el bitmap sea de 8 bits para que Tesseract lo reconozca adecuadamente. Se usa NearestNeighbor porque lo exige el constructor, pero al tener escala 1, no se aplica ningún modo de interpolación.

        public static ParámetrosExtracción NNL1_5C2x1 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true,
            blancoYNegro: true, escala: 1, luminosidad: 1.5f, contraste: 2, InterpolationMode.NearestNeighbor, DImg.PixelFormat.Format8bppIndexed); // Por alguna razón desconocida es necesario que el bitmap sea de 8 bits para que Tesseract lo reconozca adecuadamente. Se usa NearestNeighbor porque lo exige el constructor, pero al tener escala 1, no se aplica ningún modo de interpolación.

        public static ParámetrosExtracción NNL2C2x2 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true, 
            blancoYNegro: true, escala: 2, luminosidad: 2, contraste: 2, InterpolationMode.NearestNeighbor, DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción NNL1C2x2 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true,
            blancoYNegro: true, escala: 2, luminosidad: 1, contraste: 2, InterpolationMode.NearestNeighbor, DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción HQBCL2C2x4 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true, 
            blancoYNegro: true, escala: 4, luminosidad: 2, contraste: 2, InterpolationMode.HighQualityBicubic, DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción HQBCL1C2x4 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true,
            blancoYNegro: true, escala: 4, luminosidad: 1, contraste: 2, InterpolationMode.HighQualityBicubic, DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción HQBCL2C2x16 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true,
            blancoYNegro: true, escala: 16, luminosidad: 2, contraste: 2, InterpolationMode.HighQualityBicubic, DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción HQBCL1_5C2x16 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true,
            blancoYNegro: true, escala: 16, luminosidad: 1.5f, contraste: 2, InterpolationMode.HighQualityBicubic, DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción HQBCL1C2x16 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true,
            blancoYNegro: true, escala: 16, luminosidad: 1, contraste: 2, InterpolationMode.HighQualityBicubic, DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción HQBCL4C6x4 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true,
            blancoYNegro: true, escala: 4, luminosidad: 4, contraste: 6, InterpolationMode.HighQualityBicubic, DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción HQBCL2C6x4 = new ParámetrosExtracción(soloNúmeros: true, permitirUnCarácter: true, negativo: true,
            blancoYNegro: true, escala: 4, luminosidad: 2, contraste: 6, InterpolationMode.HighQualityBicubic, DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción AlfanuméricoSinProcesamiento = new ParámetrosExtracción(soloNúmeros: false, 
            permitirUnCarácter: false, negativo: false, blancoYNegro: false, escala: 1, luminosidad: 1, contraste: 1, InterpolationMode.NearestNeighbor, 
            DImg.PixelFormat.Format32bppArgb); // Se usa NearestNeighbor porque lo exige el constructor, pero al tener escala 1, no se aplica ningún modo de interpolación.

        public static ParámetrosExtracción AlfaNumNegByNL2C2 = new ParámetrosExtracción(soloNúmeros: false,
            permitirUnCarácter: false, negativo: true, blancoYNegro: true, escala: 1, luminosidad: 2, contraste: 2, InterpolationMode.NearestNeighbor,
            DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción AlfaNumNegByNL1C2 = new ParámetrosExtracción(soloNúmeros: false,
            permitirUnCarácter: false, negativo: true, blancoYNegro: true, escala: 1, luminosidad: 1, contraste: 2, InterpolationMode.NearestNeighbor,
            DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción AlfaNumNegByNL1_5C2 = new ParámetrosExtracción(soloNúmeros: false,
            permitirUnCarácter: false, negativo: true, blancoYNegro: true, escala: 1, luminosidad: 1.5f, contraste: 2, InterpolationMode.NearestNeighbor,
            DImg.PixelFormat.Format8bppIndexed);

        public enum ScreenCaptureText : int { // Por facilidad y flexibilidad se agregan todos los posibles textos en la misma enumeración. El prefijo es el nombre del juego usando _ en vez de espacios para poder filtrarlo fácilmente en opciones.
            Age_of_Empires_II_PauseF3XS, Age_of_Empires_II_PauseF3S, Age_of_Empires_II_PauseF3M, Age_of_Empires_II_PauseF3L, Age_of_Empires_II_PauseF3XL,
            Age_of_Empires_II_PauseM, Age_of_Empires_II_PauseL,
            Age_of_Empires_II_Villagers_0_to_9, Age_of_Empires_II_Villagers_10_to_99, Age_of_Empires_II_Villagers_100_to_999,
            Age_of_Empires_II_Wood, Age_of_Empires_II_Food, Age_of_Empires_II_Gold, Age_of_Empires_II_Stone, Age_of_Empires_II_Population,
            Age_of_Empires_II_Maximum_Population, Age_of_Empires_II_Villagers_on_Wood, Age_of_Empires_II_Villagers_on_Food, 
            Age_of_Empires_II_Villagers_on_Gold, Age_of_Empires_II_Villagers_on_Stone, Age_of_Empires_II_Timer, Age_of_Empires_II_Speed, 
            Age_of_Empires_II_Age, Age_of_Empires_II_InicioJuego
        }

        public static List<ScreenCaptureText> RectángulosAfectadosPorEscalaInterface = new List<ScreenCaptureText> {
            ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9, ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99,
            ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999, ScreenCaptureText.Age_of_Empires_II_Wood, ScreenCaptureText.Age_of_Empires_II_Food,
            ScreenCaptureText.Age_of_Empires_II_Gold, ScreenCaptureText.Age_of_Empires_II_Stone, ScreenCaptureText.Age_of_Empires_II_Population,
            ScreenCaptureText.Age_of_Empires_II_Maximum_Population, ScreenCaptureText.Age_of_Empires_II_Villagers_on_Wood,
            ScreenCaptureText.Age_of_Empires_II_Villagers_on_Food, ScreenCaptureText.Age_of_Empires_II_Villagers_on_Gold,
            ScreenCaptureText.Age_of_Empires_II_Villagers_on_Stone, ScreenCaptureText.Age_of_Empires_II_Timer, ScreenCaptureText.Age_of_Empires_II_Speed,
            ScreenCaptureText.Age_of_Empires_II_Age, ScreenCaptureText.Age_of_Empires_II_InicioJuego };

        public static float ConfianzaOCRAceptable = 0.50f;

        #endregion Variables>


        #region Funciones y Procedimientos

        public static string ObtenerSeleccionadoEnCombobox(SelectionChangedEventArgs e, bool tag = false) {

            var cbi = e.AddedItems[0] as ComboBoxItem;
            if (cbi != null) {

                if (tag) {

                    if (cbi.Tag is null) {
                        throw new Exception("No se encontró el elemento seleccionado en cbi.Tag."); // Nunca debería pasar.
                    } else {
                        return cbi.Tag?.ToString() ?? throw new Exception("No se encontró el elemento seleccionado en cbi.Tag?.ToString().");
                    }

                } else {

                    if (cbi.Content is null) {
                        throw new Exception("No se encontró el elemento seleccionado en cbi.Content."); // Nunca debería pasar.
                    } else {
                        return cbi.Content?.ToString() ?? throw new Exception("No se encontró el elemento seleccionado en cbi.Content?.ToString().");
                    }

                }

            } else {
                var str = e.AddedItems[0] as string;
                return str ?? throw new Exception("No se encontró el elemento seleccionado en str.");
            }
             
        } // ObtenerSeleccionadoEnCombobox>


        public static SDrw.Rectangle ObtenerRectánguloPantallaActual(bool ajustadoEscala) {

            if (ajustadoEscala) {
                return new SDrw.Rectangle(0, 0, (int)Math.Round(SystemParameters.PrimaryScreenWidth),
                    (int)Math.Round(SystemParameters.PrimaryScreenHeight));
            } else {

                var interopHelper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
                var pantallaActual = WForms.Screen.FromHandle(interopHelper.Handle);
                return pantallaActual.Bounds;

            }

        } // ObtenerRectánguloPantallaActual>


        public static string ObtenerResoluciónRecomendada() {

            var rectánguloPantalla = ObtenerRectánguloPantallaActual(ajustadoEscala: false);
            var anchoPantalla = rectánguloPantalla.Width;
            var altoPantalla = rectánguloPantalla.Height;
            var resolución = "1920x1080";
            if (anchoPantalla == 1920 && altoPantalla == 1080) {
                resolución = "1920x1080";
            } else if (anchoPantalla == 2560 && altoPantalla == 1440) {
                resolución = "2560x1440";
            } else if (anchoPantalla == 1366 && altoPantalla == 768) {
                resolución = "1366x768";
            } else if (anchoPantalla == 3840 && altoPantalla == 2160) {
                resolución = "3840x2160";
            } else if (altoPantalla >= 2160) {
                resolución = "3840x2160";
            } else if (altoPantalla >= 1440) {
                resolución = "2560x1440";
            } else if (altoPantalla >= 1080) {
                resolución = "1920x1080";
            } else {
                resolución = "1366x768";
            }
            return resolución;

        } // ObtenerResoluciónRecomendada>


        public static int ObtenerDuraciónEndStepSound(string soundName) 
            => (int)Math.Round(1000 * MediaPlayer.GetDuration(Path.Combine(DirectorioSonidosLargos, soundName)), 0);


        public static Dictionary<NameType, Dictionary<string, string>> DeserializarNombres(string ruta)
            => JsonSerializer.Deserialize<Dictionary<NameType, Dictionary<string, string>>>(File.ReadAllText(ruta),
                ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración | Serialización.EnumeraciónEnTexto | Serialización.UTF8));


        public static Dictionary<string, Dictionary<string, string>> DeserializarTipos(string ruta)
            => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(ruta),
                ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración | Serialización.EnumeraciónEnTexto | Serialización.UTF8));


        public static string SerializarNombres<T>(T nombres)
            => JsonSerializer.Serialize(nombres, ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración
                | Serialización.EnumeraciónEnTexto | Serialización.UTF8));


        public static void CrearEntidadesYNombres() {

            if (!File.Exists(Preferencias.NamesPath) && !File.Exists(Preferencias.EnglishNamesPath)) CrearArchivoNombresInglés(); // El archivo de nombres en inglés, solo es necesario si no existe el archivo de nombres.
            if (!File.Exists(Preferencias.NamesPath)) CrearArchivoNombres();
            if (!File.Exists(Preferencias.CustomNamesDefaultPath)) CrearArchivoNombresPersonalizadosDefault();
            if (!File.Exists(Preferencias.CustomImagesDefaultPath)) CrearArchivoImágenesPersonalizadas();
            if (!File.Exists(Preferencias.TypesDefaultPath)) CrearArchivoTipos();
            if (!File.Exists(Preferencias.CustomNamesPath)) File.Copy(Preferencias.CustomNamesDefaultPath, Preferencias.CustomNamesPath);
            if (!File.Exists(Preferencias.CustomImagesPath)) File.Copy(Preferencias.CustomImagesDefaultPath, Preferencias.CustomImagesPath);
            if (!File.Exists(Preferencias.TypesPath)) File.Copy(Preferencias.TypesDefaultPath, Preferencias.TypesPath);
            if (File.Exists(Preferencias.EnglishNamesPath) && File.Exists(Preferencias.NamesPath)) File.Delete(Preferencias.EnglishNamesPath); // El archivo EnglishNamesPath es innecesario, solo es temporal para la creación de NamesPath.
            CrearEntidades();
            CrearNombresYAgregarAEntidades();
            CrearImágenes();

        } // CrearEntidadesYNombres>


        public static void CrearNombresYAgregarAEntidades() {
            CrearNombres();
            AgregarNombresAEntidades();
        } // CrearNombresYAgregarAEntidades>


        public static void CrearImágenes() {

            Imágenes.Clear();
            var imágenesHuérfanas = ""; // Se usa internamente para fines de revisión de las imágenes.
            var directorioImágenesGame = Path.Combine(DirectorioImágenes, Preferencias.Game);
            var carpetas = Directory.GetDirectories(directorioImágenesGame, "*", SearchOption.AllDirectories).ToList();
            carpetas.Add(directorioImágenesGame);

            foreach (var carpeta in carpetas) {

                var imágenes = Directory.GetFiles(carpeta);
                foreach (var imagen in imágenes) {

                    var nombreImagen = Path.GetFileNameWithoutExtension(imagen).ToLowerInvariant();
                    if (ExtensionesImágenes.Contains(ObtenerExtensión(imagen))) {

                        if (Imágenes.ContainsKey(nombreImagen)) {
                            MostrarInformación($"A new image with the same name '{nombreImagen}' was found in {imagen}. Only the first image will be used. " + 
                                "You can't use images with the same, not even in different directories.");
                        } else {
                            Imágenes.Add(nombreImagen, imagen);
                        }

                    }
                        
                    if (Entidades.FirstOrDefault(e => e.Value.ObtenerImagenEfectiva() == nombreImagen).Value == null) // Solo verifica por imágenes húerfanas usando la primera imágen efectiva.
                        imágenesHuérfanas += nombreImagen + Environment.NewLine;

                }

            }

        } // CrearImágenes>


        public static void AgregarNombresAEntidades() {

            foreach (var entidad in Entidades) {

                entidad.Value.Nombres.Clear(); // Siempre se limpian para permitir la actualización desde settings después de cambiar los nombres personalizados. En la primera carga no tiene ningún efecto.

                var nombres = TodosLosNombres.Where(tupla => tupla.Item2.ID == entidad.Key).ToList();
                foreach (var kv in nombres) {
                    if (!entidad.Value.Nombres.ContainsKey(kv.Item2.Tipo)) entidad.Value.Nombres.Add(kv.Item2.Tipo, kv.Item2.Valor); // Solo se agrega el primer nombre. El objetivo de estos nombres es mostrarlos cuando se elija ver la build order con ese tipo, por lo tanto cuando una entidad tenga un nombre que para el mismo tipo tiene varias opciones, por ejemplo la entidad Knight tiene opción en acrónimos de KT|KNT|KTS cuando se muestre la build order con acrónimos para Knight aparecerá KT en vez de KT|KNT|KTS.
                }

            }

        } // AgregarNombresAEntidades>


        public static void CrearEntidades() {

            Entidades.Clear();
            var names = DeserializarNombres(Preferencias.NamesPath);
            var types = DeserializarTipos(Preferencias.TypesPath);
            var customImages = DeserializarNombres(Preferencias.CustomImagesPath);         

            foreach (var kv in names[NameType.Complete]) {

                var nombreCompleto = kv.Value;
                var tipo = types["Type"].FirstOrDefault(kv2 => kv2.Key == nombreCompleto).Value;
                var imagenPersonalizada = customImages[NameType.Image].FirstOrDefault(kv2 => kv2.Key == nombreCompleto).Value;
                if (tipo == null) {
                    throw new Exception($"Type can't be null for {nombreCompleto} ID = {kv.Key}.");
                } else {
                    Entidades.Add(kv.Key, new Entidad(kv.Key, nombreCompleto, tipo, imagenPersonalizada));
                }
                
            }

        } // CrearEntidades>


        public static void CrearNombres() {

            TodosLosNombres.Clear();
            Nombres.Clear();

            var names = DeserializarNombres(Preferencias.NamesPath);
            var customNames = DeserializarNombres(Preferencias.CustomNamesPath);

            names.Add(NameType.Custom, new Dictionary<string, string>());
            foreach (var kv in customNames[NameType.Custom]) {

                var nombreCompleto = kv.Key;
                var nombrePersonalizado = kv.Value;
                var id = Entidades.FirstOrDefault(e => e.Value.NombreCompleto == nombreCompleto).Key;
                if (id != null) names[NameType.Custom].Add(id, nombrePersonalizado);

            }

            var nombresYTipoNombre = new Dictionary<string, NameType>(); // Para detectar colisiones de términos entre idiomas.
            var conflictosEncontrados = new Dictionary<string, NameType>();

            foreach (var tipoNombre in ObtenerValores<NameType>()) {

                if (names.ContainsKey(tipoNombre)) {
                    foreach (var kv in names[tipoNombre]) {

                        var id = kv.Key;
                        var nombres = kv.Value.Split("|");
                        foreach (var nombreIt in nombres) {

                            var nombre = nombreIt;                      
                            reintentar:
                            var nombreMinúscula = nombre.ToLowerInvariant(); // Se usa esta función para construir el diccionario de nombres que la conversión no dependa de la cultura actual del usuario y el diccionario siempre sea el mismo idependiente de esta. Leer más en https://docs.microsoft.com/en-us/dotnet/api/system.string.tolowerinvariant?view=net-6.0.

                            if (Nombres.ContainsKey(nombreMinúscula)) {

                                var idAnterior = Nombres[nombreMinúscula].ID;
                                if (idAnterior == id) {

                                    if (!EsIdioma(tipoNombre) && tipoNombre != NameType.Custom) 
                                        MostrarInformación($"The name type {tipoNombre} of {nombre} ID = {id} is already used by other name type."); // Se permite que se repita el mismo nombre entre diferentes idiomas para la misma clave o para los personalizados (custom). Se alerta para los tipos de nombre que no son idiomas porque posiblemente se trate de un error en el ingreso de los datos.

                                } else { // Ya está el nombre para otro ID distinto. Se debe solucionar el conflicto.

                                    var tipo = Entidades[id].Tipo;
                                    var tipoAnterior = Entidades[idAnterior].Tipo;
                                    if (tipo == tipoAnterior) { // Es un conflicto más grave porque el juego tiene el mismo nombre para diferentes IDs del mismo tipo. Aún se puede solucionar en caso que sea colisión de idiomas.

                                        var tipoNombreAnterior = nombresYTipoNombre[nombreMinúscula];
                                        if (tipoNombre == tipoNombreAnterior) { // Es un conflicto grave porque es el mismo nombre para unidades diferentes del mismo tipo en el mismo idioma. Es un error de traducción del juego que fue reportado aquí https://forums.ageofempires.com/t/units-buildings-tech-names-collisions-in-various-languages/187437. Por el momento lo único que se puede hacer es agregar un asterisco para diferenciarlos. 

                                            nombre += "*";
                                            conflictosEncontrados.Agregar(nombre, tipoNombre);
                                            goto reintentar; // Se usa goto para reiniciar el procedimiento de chequeo completo, podría suceder que se repita varias veces y se tenga que hacer esta corrección varias veces.
                                            //MessageBox.Show($"The name {nombre} of type {tipoNombre} with ID = {id} is already used by entity " + $"ID = {idAnterior}.", "Warning");

                                        } else { // Es una colisión entre nombres de entidades en diferentes idiomas. Se agrega al nombre un identificador del idioma.

                                            nombre += "*";
                                            conflictosEncontrados.Agregar(nombre, tipoNombre);
                                            goto reintentar; // Se usa goto para reiniciar el procedimiento de chequeo completo, podría suceder que se repita varias veces y se tenga que hacer esta corrección varias veces.

                                        }

                                    } else { // Es una colisión entre entidades de diferente tipo. Puede suceder por ejemplo, con cosas como Mercado (edificio) y Mercado (mapa). Se soluciona agregando el tipo al siguiente a agregar.

                                        nombre += "*";
                                        conflictosEncontrados.Agregar(nombre, tipoNombre);
                                        goto reintentar; // Se usa goto para reiniciar el procedimiento de chequeo completo, podría suceder que se repita varias veces y se tenga que hacer esta corrección varias veces.

                                    }
                                     
                                }
                            
                            } else { // !Nombres.ContainsKey(nombre).

                                Nombres.Add(nombreMinúscula, new Nombre(nombre, tipoNombre, id));
                                nombresYTipoNombre.Add(nombreMinúscula, tipoNombre);

                            }
                            TodosLosNombres.Add((nombreMinúscula, new Nombre(nombre, tipoNombre, id)));

                        }

                    } // foreach names[tipoNombre]>
                }

            } // foreach NameType>

            var textoConflictos = ""; // Se podría presentar al usuario en los settings o simplemente para uso interno. El cálculo de este valor no agrega mayor tiempo de cálculo.
            var conflictosAgrupados = conflictosEncontrados.GroupBy(kv => kv.Value).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var kv in conflictosAgrupados) {

                textoConflictos += kv.Key.ToString() + Environment.NewLine;
                foreach (var kv2 in kv.Value) {
                    textoConflictos += kv2.Key.ToString() + Environment.NewLine;
                }
                textoConflictos += Environment.NewLine;

            }

        } // CrearNombres>


        public static void CrearArchivoTipos() {

            var types = new Dictionary<string, Dictionary<string, string>>();
            types.Add("Type", new Dictionary<string, string>()); // Así no sea necesario se maneja la misma estructura de diccionario de diccionario para mantener la misma estructura de los otros archivos.

            if (Preferencias.Game == AOE2Name) {

                types["Type"].Add("Dark Age", "Age");
                types["Type"].Add("Feudal Age", "Age");
                types["Type"].Add("Castle Age", "Age");
                types["Type"].Add("Imperial Age", "Age");
                types["Type"].Add("Stop", "Action");
                types["Type"].Add("Unload", "Action");
                types["Type"].Add("Automatically Reseed Farms", "Action");
                types["Type"].Add("Automatically Rebuild Fish Traps", "Action");
                types["Type"].Add("Attack Ground", "Action");
                types["Type"].Add("Heal", "Action");
                types["Type"].Add("Convert", "Action");
                types["Type"].Add("Aggressive Stance", "Action");
                types["Type"].Add("Auto Scout", "Action");
                types["Type"].Add("Defensive Stance", "Action");
                types["Type"].Add("No Attack Stance", "Action");
                types["Type"].Add("Guard", "Action");
                types["Type"].Add("Follow", "Action");
                types["Type"].Add("Patrol", "Action");
                types["Type"].Add("Set Gather Point", "Action");
                types["Type"].Add("Attack Move", "Action");
                types["Type"].Add("Garrison", "Action");
                types["Type"].Add("Defend", "Action");
                types["Type"].Add("Build", "Action");
                types["Type"].Add("Flare", "Action");
                types["Type"].Add("Ring Town Bell", "Action");
                types["Type"].Add("Delete", "Action");
                types["Type"].Add("Stand Ground", "Action");
                types["Type"].Add("Attack", "Action");
                types["Type"].Add("Drop Food", "Action");
                types["Type"].Add("Drop Gold", "Action");
                types["Type"].Add("Drop Stone", "Action");
                types["Type"].Add("Drop Wood", "Action");
                types["Type"].Add("Gather Berries", "Action");
                types["Type"].Add("Gather Meat", "Action");
                types["Type"].Add("Hunt", "Action");
                types["Type"].Add("Fish", "Action");
                types["Type"].Add("Chop", "Action");
                types["Type"].Add("Mine Gold", "Action");
                types["Type"].Add("Mine Stone", "Action");
                types["Type"].Add("Deer", "Resource");
                types["Type"].Add("Fish (Tuna)", "Resource");
                types["Type"].Add("Relic", "Resource");
                types["Type"].Add("Tree (Oak)", "Resource");
                types["Type"].Add("Forage Bush", "Resource");
                types["Type"].Add("Wild Boar", "Resource");
                types["Type"].Add("Sheep", "Resource");
                types["Type"].Add("Cow", "Resource");
                types["Type"].Add("Llama", "Resource");
                types["Type"].Add("Food", "Resource");
                types["Type"].Add("Wood", "Resource");
                types["Type"].Add("Stone", "Resource");
                types["Type"].Add("Gold", "Resource");
                types["Type"].Add("Zebra", "Resource");
                types["Type"].Add("Ostrich", "Resource");
                types["Type"].Add("Goat", "Resource");
                types["Type"].Add("Turkey", "Resource");
                types["Type"].Add("Ibex", "Resource");
                types["Type"].Add("Pig", "Resource");
                types["Type"].Add("Goose", "Resource");
                types["Type"].Add("Straggler Trees", "Resource");
                types["Type"].Add("Rhinoceros", "Resource");
                types["Type"].Add("Box Turtles", "Resource");
                types["Type"].Add("Water Buffalo", "Resource");
                types["Type"].Add("Elephant", "Resource");
                types["Type"].Add("Fruit Bush", "Resource");
                types["Type"].Add("Stone Mine", "Resource");
                types["Type"].Add("Gold Mine", "Resource");
                types["Type"].Add("Food to Gold", "Resource");
                types["Type"].Add("Food to Stone", "Resource");
                types["Type"].Add("Food to wood", "Resource");
                types["Type"].Add("Gold to Food", "Resource");
                types["Type"].Add("Gold to Stone", "Resource");
                types["Type"].Add("Gold to wood", "Resource");
                types["Type"].Add("Stone to Food", "Resource");
                types["Type"].Add("Stone to Gold", "Resource");
                types["Type"].Add("Stone to wood", "Resource");
                types["Type"].Add("Wood to Food", "Resource");
                types["Type"].Add("Wood to Gold", "Resource");
                types["Type"].Add("Wood to Stone", "Resource");
                types["Type"].Add("Archer", "Unit");
                types["Type"].Add("Crossbowman", "Unit");
                types["Type"].Add("Arbalester", "Unit");
                types["Type"].Add("Skirmisher", "Unit");
                types["Type"].Add("Elite Skirmisher", "Unit");
                types["Type"].Add("Imperial Skirmisher", "Unit");
                types["Type"].Add("Slinger", "Unit");
                types["Type"].Add("Hand Cannoneer", "Unit");
                types["Type"].Add("Cavalry Archer", "Unit");
                types["Type"].Add("Heavy Cavalry Archer", "Unit");
                types["Type"].Add("Genitour", "Unit");
                types["Type"].Add("Elite Genitour", "Unit");
                types["Type"].Add("Militia", "Unit");  
                types["Type"].Add("Man-at-Arms", "Unit");
                types["Type"].Add("Long Swordsman", "Unit");
                types["Type"].Add("Two-Handed Swordsman", "Unit");
                types["Type"].Add("Champion", "Unit");
                types["Type"].Add("Spearman", "Unit");
                types["Type"].Add("Pikeman", "Unit");
                types["Type"].Add("Halberdier", "Unit");
                types["Type"].Add("Eagle Scout", "Unit");
                types["Type"].Add("Eagle Warrior", "Unit");
                types["Type"].Add("Elite Eagle Warrior", "Unit");
                types["Type"].Add("Condottiero", "Unit");
                types["Type"].Add("Scout Cavalry", "Unit");
                types["Type"].Add("Light Cavalry", "Unit");
                types["Type"].Add("Hussar", "Unit");
                types["Type"].Add("Winged Hussar", "Unit");
                types["Type"].Add("Knight", "Unit");
                types["Type"].Add("Cavalier", "Unit");
                types["Type"].Add("Paladin", "Unit");
                types["Type"].Add("Camel Rider", "Unit");
                types["Type"].Add("Heavy Camel Rider", "Unit");
                types["Type"].Add("Imperial Camel Rider", "Unit");
                types["Type"].Add("Battle Elephant", "Unit");
                types["Type"].Add("Elite Battle Elephant", "Unit");
                types["Type"].Add("Steppe Lancer", "Unit");
                types["Type"].Add("Elite Steppe Lancer", "Unit");
                types["Type"].Add("Xolotl Warrior", "Unit");
                types["Type"].Add("Battering Ram", "Unit");
                types["Type"].Add("Capped Ram", "Unit");
                types["Type"].Add("Siege Ram", "Unit");
                types["Type"].Add("Mangonel", "Unit");
                types["Type"].Add("Onager", "Unit");
                types["Type"].Add("Siege Onager", "Unit");
                types["Type"].Add("Scorpion", "Unit");
                types["Type"].Add("Heavy Scorpion", "Unit");
                types["Type"].Add("Siege Tower", "Unit");
                types["Type"].Add("Bombard Cannon", "Unit");
                types["Type"].Add("Houfnice", "Unit");
                types["Type"].Add("Fishing Ship", "Unit");
                types["Type"].Add("Transport Ship", "Unit");
                types["Type"].Add("Fire Galley", "Unit");
                types["Type"].Add("Trade Cog", "Unit");
                types["Type"].Add("Demolition Raft", "Unit");
                types["Type"].Add("Galley", "Unit");
                types["Type"].Add("Fire Ship", "Unit");
                types["Type"].Add("Demolition Ship", "Unit");
                types["Type"].Add("War Galley", "Unit");
                types["Type"].Add("Fast Fire Ship", "Unit");
                types["Type"].Add("Cannon Galleon", "Unit");
                types["Type"].Add("Heavy Demolition Ship", "Unit");
                types["Type"].Add("Galleon", "Unit");
                types["Type"].Add("Elite Cannon Galleon", "Unit");
                types["Type"].Add("Turtle Ship", "Unit");
                types["Type"].Add("Elite Turtle Ship", "Unit");
                types["Type"].Add("Longboat", "Unit");
                types["Type"].Add("Elite Longboat", "Unit");
                types["Type"].Add("Caravel", "Unit");
                types["Type"].Add("Elite Caravel", "Unit");
                types["Type"].Add("Petard", "Unit");
                types["Type"].Add("Trebuchet", "Unit");
                types["Type"].Add("Monk", "Unit");
                types["Type"].Add("Missionary", "Unit");
                types["Type"].Add("Villager", "Unit");
                types["Type"].Add("Trade Cart", "Unit");
                types["Type"].Add("Flemish Militia", "Unit");
                types["Type"].Add("Jaguar Warrior", "Unit");
                types["Type"].Add("Camel Archer", "Unit");
                types["Type"].Add("Hussite Wagon", "Unit");
                types["Type"].Add("Longbowman", "Unit");
                types["Type"].Add("Konnik", "Unit");
                types["Type"].Add("Coustillier", "Unit");
                types["Type"].Add("Arambai", "Unit");
                types["Type"].Add("Cataphract", "Unit");
                types["Type"].Add("Woad Raider", "Unit");
                types["Type"].Add("Chu Ko Nu", "Unit");
                types["Type"].Add("Kipchak", "Unit");
                types["Type"].Add("Shotel Warrior", "Unit");
                types["Type"].Add("Throwing Axeman", "Unit");
                types["Type"].Add("Huskarl", "Unit");
                types["Type"].Add("Tarkan", "Unit");
                types["Type"].Add("Kamayuk", "Unit");
                types["Type"].Add("Elephant Archer", "Unit");
                types["Type"].Add("Genoese Crossbowman", "Unit");
                types["Type"].Add("Samurai", "Unit");
                types["Type"].Add("Ballista Elephant", "Unit");
                types["Type"].Add("War Wagon", "Unit");
                types["Type"].Add("Leitis", "Unit");
                types["Type"].Add("Magyar Huszar", "Unit");
                types["Type"].Add("Karambit Warrior", "Unit");
                types["Type"].Add("Gbeto", "Unit");
                types["Type"].Add("Plumed Archer", "Unit");
                types["Type"].Add("Mangudai", "Unit");
                types["Type"].Add("War Elephant", "Unit");
                types["Type"].Add("Obuch", "Unit");
                types["Type"].Add("Organ Gun", "Unit");
                types["Type"].Add("Mameluke", "Unit");
                types["Type"].Add("Serjeant", "Unit");
                types["Type"].Add("Boyar", "Unit");
                types["Type"].Add("Conquistador", "Unit");
                types["Type"].Add("Keshik", "Unit");
                types["Type"].Add("Flaming Camel", "Unit");
                types["Type"].Add("Teutonic Knight", "Unit");
                types["Type"].Add("Janissary", "Unit");
                types["Type"].Add("Rattan Archer", "Unit");
                types["Type"].Add("Berserk", "Unit");
                types["Type"].Add("Elite Jaguar Warrior", "Unit");
                types["Type"].Add("Elite Camel Archer", "Unit");
                types["Type"].Add("Elite Hussite Wagon", "Unit");
                types["Type"].Add("Elite Longbowman", "Unit");
                types["Type"].Add("Elite Konnik", "Unit");
                types["Type"].Add("Elite Coustillier", "Unit");
                types["Type"].Add("Elite Arambai", "Unit");
                types["Type"].Add("Elite Cataphract", "Unit");
                types["Type"].Add("Elite Woad Raider", "Unit");
                types["Type"].Add("Elite Chu Ko Nu", "Unit");
                types["Type"].Add("Elite Kipchak", "Unit");
                types["Type"].Add("Elite Shotel Warrior", "Unit");
                types["Type"].Add("Elite Throwing Axeman", "Unit");
                types["Type"].Add("Elite Huskarl", "Unit");
                types["Type"].Add("Elite Tarkan", "Unit");
                types["Type"].Add("Elite Kamayuk", "Unit");
                types["Type"].Add("Elite Elephant Archer", "Unit");
                types["Type"].Add("Elite Genoese Crossbowman", "Unit");
                types["Type"].Add("Elite Samurai", "Unit");
                types["Type"].Add("Elite Ballista Elephant", "Unit");
                types["Type"].Add("Elite War Wagon", "Unit");
                types["Type"].Add("Elite Leitis", "Unit");
                types["Type"].Add("Elite Magyar Huszar", "Unit");
                types["Type"].Add("Elite Karambit Warrior", "Unit");
                types["Type"].Add("Elite Gbeto", "Unit");
                types["Type"].Add("Elite Plumed Archer", "Unit");
                types["Type"].Add("Elite Mangudai", "Unit");
                types["Type"].Add("Elite War Elephant", "Unit");
                types["Type"].Add("Elite Obuch", "Unit");
                types["Type"].Add("Elite Organ Gun", "Unit");
                types["Type"].Add("Elite Mameluke", "Unit");
                types["Type"].Add("Elite Serjeant", "Unit");
                types["Type"].Add("Elite Boyar", "Unit");
                types["Type"].Add("Elite Conquistador", "Unit");
                types["Type"].Add("Elite Keshik", "Unit");
                types["Type"].Add("Elite Teutonic Knight", "Unit");
                types["Type"].Add("Elite Janissary", "Unit");
                types["Type"].Add("Elite Rattan Archer", "Unit");
                types["Type"].Add("Elite Berserk", "Unit");
                types["Type"].Add("Lion", "Unit");
                types["Type"].Add("Crocodile", "Unit");
                types["Type"].Add("Tiger", "Unit");
                types["Type"].Add("Wolf", "Unit");
                types["Type"].Add("Komodo Dragon", "Unit");
                types["Type"].Add("Dismounted Konnik", "Unit");
                types["Type"].Add("American Monk", "Unit");
                types["Type"].Add("Jaguar (Animal)", "Unit");
                types["Type"].Add("Wonder", "Building");
                types["Type"].Add("Archery Range", "Building");
                types["Type"].Add("Barracks", "Building");
                types["Type"].Add("Stable", "Building");
                types["Type"].Add("Siege Workshop", "Building");
                types["Type"].Add("Blacksmith", "Building");
                types["Type"].Add("Fish Trap", "Building");
                types["Type"].Add("University", "Building");
                types["Type"].Add("Dock", "Building");
                types["Type"].Add("Outpost", "Building");
                types["Type"].Add("Watch Tower", "Building");
                types["Type"].Add("Guard Tower", "Building");
                types["Type"].Add("Keep", "Building");
                types["Type"].Add("Bombard Tower", "Building");
                types["Type"].Add("Palisade Wall", "Building");
                types["Type"].Add("Palisade Gate", "Building");
                types["Type"].Add("Stone Wall", "Building");
                types["Type"].Add("Fortified Wall", "Building");
                types["Type"].Add("Gate", "Building");
                types["Type"].Add("Castle", "Building");
                types["Type"].Add("Krepost", "Building");
                types["Type"].Add("Donjon", "Building");
                types["Type"].Add("Monastery", "Building");
                types["Type"].Add("House", "Building");
                types["Type"].Add("Town Center", "Building");
                types["Type"].Add("Feitoria", "Building");
                types["Type"].Add("Mining Camp", "Building");
                types["Type"].Add("Lumber Camp", "Building");
                types["Type"].Add("Folwark", "Building");
                types["Type"].Add("Mill", "Building");
                types["Type"].Add("Farm", "Building");
                types["Type"].Add("Market", "Building");
                types["Type"].Add("Harbor", "Building");
                types["Type"].Add("Britons", "Civilization");
                types["Type"].Add("Franks", "Civilization");
                types["Type"].Add("Goths", "Civilization");
                types["Type"].Add("Teutons", "Civilization");
                types["Type"].Add("Japanese", "Civilization");
                types["Type"].Add("Chinese", "Civilization");
                types["Type"].Add("Byzantines", "Civilization");
                types["Type"].Add("Persians", "Civilization");
                types["Type"].Add("Saracens", "Civilization");
                types["Type"].Add("Turks", "Civilization");
                types["Type"].Add("Vikings", "Civilization");
                types["Type"].Add("Mongols", "Civilization");
                types["Type"].Add("Celts", "Civilization");
                types["Type"].Add("Spanish", "Civilization");
                types["Type"].Add("Aztecs", "Civilization");
                types["Type"].Add("Mayans", "Civilization");
                types["Type"].Add("Huns", "Civilization");
                types["Type"].Add("Koreans", "Civilization");
                types["Type"].Add("Italians", "Civilization");
                types["Type"].Add("Indians", "Civilization");
                types["Type"].Add("Incas", "Civilization");
                types["Type"].Add("Magyars", "Civilization");
                types["Type"].Add("Slavs", "Civilization");
                types["Type"].Add("Portuguese", "Civilization");
                types["Type"].Add("Ethiopians", "Civilization");
                types["Type"].Add("Malians", "Civilization");
                types["Type"].Add("Berbers", "Civilization");
                types["Type"].Add("Khmer", "Civilization");
                types["Type"].Add("Malay", "Civilization");
                types["Type"].Add("Burmese", "Civilization");
                types["Type"].Add("Vietnamese", "Civilization");
                types["Type"].Add("Bulgarians", "Civilization");
                types["Type"].Add("Tatars", "Civilization");
                types["Type"].Add("Cumans", "Civilization");
                types["Type"].Add("Lithuanians", "Civilization");
                types["Type"].Add("Burgundians", "Civilization");
                types["Type"].Add("Sicilians", "Civilization");
                types["Type"].Add("Poles", "Civilization");
                types["Type"].Add("Bohemians", "Civilization");
                types["Type"].Add("Thumb Ring", "Technology");
                types["Type"].Add("Parthian Tactics", "Technology");
                types["Type"].Add("Supplies", "Technology");
                types["Type"].Add("Squires", "Technology");
                types["Type"].Add("Arson", "Technology");
                types["Type"].Add("Bloodlines", "Technology");
                types["Type"].Add("Husbandry", "Technology");
                types["Type"].Add("Padded Archer Armor", "Technology");
                types["Type"].Add("Fletching", "Technology");
                types["Type"].Add("Forging", "Technology");
                types["Type"].Add("Scale Barding Armor", "Technology");
                types["Type"].Add("Scale Mail Armor", "Technology");
                types["Type"].Add("Leather Archer Armor", "Technology");
                types["Type"].Add("Bodkin Arrow", "Technology");
                types["Type"].Add("Iron Casting", "Technology");
                types["Type"].Add("Chain Barding Armor", "Technology");
                types["Type"].Add("Chain Mail Armor", "Technology");
                types["Type"].Add("Ring Archer Armor", "Technology");
                types["Type"].Add("Bracer", "Technology");
                types["Type"].Add("Blast Furnace", "Technology");
                types["Type"].Add("Plate Barding Armor", "Technology");
                types["Type"].Add("Plate Mail Armor", "Technology");
                types["Type"].Add("Gillnets", "Technology");
                types["Type"].Add("Careening", "Technology");
                types["Type"].Add("Dry Dock", "Technology");
                types["Type"].Add("Shipwright", "Technology");
                types["Type"].Add("Masonry", "Technology");
                types["Type"].Add("Architecture", "Technology");
                types["Type"].Add("Chemistry", "Technology");
                types["Type"].Add("Ballistics", "Technology");
                types["Type"].Add("Siege Engineers", "Technology");
                types["Type"].Add("Heated Shot", "Technology");
                types["Type"].Add("Arrowslits", "Technology");
                types["Type"].Add("Murder Holes", "Technology");
                types["Type"].Add("Treadmill Crane", "Technology");
                types["Type"].Add("Hoardings", "Technology");
                types["Type"].Add("Sappers", "Technology");
                types["Type"].Add("Conscription", "Technology");
                types["Type"].Add("Spies/Treason", "Technology");
                types["Type"].Add("Redemption", "Technology");
                types["Type"].Add("Atonement", "Technology");
                types["Type"].Add("Herbal Medicine", "Technology");
                types["Type"].Add("Heresy", "Technology");
                types["Type"].Add("Sanctity", "Technology");
                types["Type"].Add("Fervor", "Technology");
                types["Type"].Add("Faith", "Technology");
                types["Type"].Add("Illumination", "Technology");
                types["Type"].Add("Block Printing", "Technology");
                types["Type"].Add("Theocracy", "Technology");
                types["Type"].Add("Loom", "Technology");
                types["Type"].Add("Town Watch", "Technology");
                types["Type"].Add("Wheelbarrow", "Technology");
                types["Type"].Add("Town Patrol", "Technology");
                types["Type"].Add("Hand Cart", "Technology");
                types["Type"].Add("Gold Mining", "Technology");
                types["Type"].Add("Stone Mining", "Technology");
                types["Type"].Add("Gold Shaft Mining", "Technology");
                types["Type"].Add("Stone Shaft Mining", "Technology");
                types["Type"].Add("Double-Bit Axe", "Technology");
                types["Type"].Add("Bow Saw", "Technology");
                types["Type"].Add("Two-Man Saw", "Technology");
                types["Type"].Add("Horse Collar", "Technology");
                types["Type"].Add("Coinage", "Technology");
                types["Type"].Add("Caravan", "Technology");
                types["Type"].Add("Banking", "Technology");
                types["Type"].Add("Guilds", "Technology");
                types["Type"].Add("Heavy Plow", "Technology");
                types["Type"].Add("Crop Rotation", "Technology");
                types["Type"].Add("Atlatl", "Technology");
                types["Type"].Add("Garland Wars", "Technology");
                types["Type"].Add("Kasbah", "Technology");
                types["Type"].Add("Maghrebi Camels", "Technology");
                types["Type"].Add("Wagenburg Tactics", "Technology");
                types["Type"].Add("Hussite Reforms", "Technology");
                types["Type"].Add("Yeomen", "Technology");
                types["Type"].Add("Warwolf", "Technology");
                types["Type"].Add("Stirrups", "Technology");
                types["Type"].Add("Bagains", "Technology");
                types["Type"].Add("Burgundian Vineyards", "Technology");
                types["Type"].Add("Flemish Revolution", "Technology");
                types["Type"].Add("Howdah", "Technology");
                types["Type"].Add("Manipur Cavalry", "Technology");
                types["Type"].Add("Greek Fire", "Technology");
                types["Type"].Add("Logistica", "Technology");
                types["Type"].Add("Stronghold", "Technology");
                types["Type"].Add("Furor Celtica", "Technology");
                types["Type"].Add("Great Wall", "Technology");
                types["Type"].Add("Rocketry", "Technology");
                types["Type"].Add("Steppe Husbandry", "Technology");
                types["Type"].Add("Cuman Mercenaries", "Technology");
                types["Type"].Add("Royal Heirs", "Technology");
                types["Type"].Add("Torsion Engines", "Technology");
                types["Type"].Add("Bearded Axe", "Technology");
                types["Type"].Add("Chivalry", "Technology");
                types["Type"].Add("Anarchy", "Technology");
                types["Type"].Add("Perfusion", "Technology");
                types["Type"].Add("Marauders", "Technology");
                types["Type"].Add("Atheism", "Technology");
                types["Type"].Add("Andean Sling", "Technology");
                types["Type"].Add("Fabric Shields", "Technology");
                types["Type"].Add("Sultans", "Technology");
                types["Type"].Add("Shatagni", "Technology");
                types["Type"].Add("Pavise", "Technology");
                types["Type"].Add("Silk Road", "Technology");
                types["Type"].Add("Yasama", "Technology");
                types["Type"].Add("Kataparuto", "Technology");
                types["Type"].Add("Tusk Swords", "Technology");
                types["Type"].Add("Double Crossbow", "Technology");
                types["Type"].Add("Eupseong", "Technology");
                types["Type"].Add("Shinkichon", "Technology");
                types["Type"].Add("Hill Forts", "Technology");
                types["Type"].Add("Tower Shields", "Technology");
                types["Type"].Add("Corvinian Army", "Technology");
                types["Type"].Add("Recurve Bow", "Technology");
                types["Type"].Add("Thalassocracy", "Technology");
                types["Type"].Add("Forced Levy", "Technology");
                types["Type"].Add("Tigui", "Technology");
                types["Type"].Add("Farimba", "Technology");
                types["Type"].Add("Hul'che Javelineers", "Technology");
                types["Type"].Add("El Dorado", "Technology");
                types["Type"].Add("Nomads", "Technology");
                types["Type"].Add("Drill", "Technology");
                types["Type"].Add("Kamandaran", "Technology");
                types["Type"].Add("Mahouts", "Technology");
                types["Type"].Add("Szlachta Privileges", "Technology");
                types["Type"].Add("Lechitic Legacy", "Technology");
                types["Type"].Add("Carrack", "Technology");
                types["Type"].Add("Arquebus", "Technology");
                types["Type"].Add("Madrasah", "Technology");
                types["Type"].Add("Zealotry", "Technology");
                types["Type"].Add("First Crusade", "Technology");
                types["Type"].Add("Hauberk", "Technology");
                types["Type"].Add("Orthodoxy", "Technology");
                types["Type"].Add("Druzhina", "Technology");
                types["Type"].Add("Inquisition", "Technology");
                types["Type"].Add("Supremacy", "Technology");
                types["Type"].Add("Silk Armor", "Technology");
                types["Type"].Add("Timurid Siegecraft", "Technology");
                types["Type"].Add("Ironclad", "Technology");
                types["Type"].Add("Crenellations", "Technology");
                types["Type"].Add("Sipahi", "Technology");
                types["Type"].Add("Artillery", "Technology");
                types["Type"].Add("Chatras", "Technology");
                types["Type"].Add("Paper Money", "Technology");
                types["Type"].Add("Chieftains", "Technology");
                types["Type"].Add("Berserkergang", "Technology");
                types["Type"].Add("Tech: Bombard Tower", "Technology");
                types["Type"].Add("Castle Age Unique Technology", "Technology");
                types["Type"].Add("Imperial Age Unique Technology", "Technology");
                types["Type"].Add("Research Cannon Galleon (requires Chemistry)", "Technology");
                types["Type"].Add("Upgrade to Elite Unique Unit", "Upgrade");
                types["Type"].Add("Upgrade to Crossbowman", "Upgrade");
                types["Type"].Add("Upgrade to Arbalester", "Upgrade");
                types["Type"].Add("Upgrade to Elite Skirmisher", "Upgrade");
                types["Type"].Add("Upgrade to Imperial Skirmisher", "Upgrade");
                types["Type"].Add("Upgrade to Heavy Cavalry Archer", "Upgrade");
                types["Type"].Add("Upgrade to Elite Genitour", "Upgrade");
                types["Type"].Add("Upgrade to Man-at-Arms", "Upgrade");
                types["Type"].Add("Upgrade to Long Swordsman", "Upgrade");
                types["Type"].Add("Upgrade to Two-Handed Swordsman", "Upgrade");
                types["Type"].Add("Upgrade to Champion", "Upgrade");
                types["Type"].Add("Upgrade to Pikeman", "Upgrade");
                types["Type"].Add("Upgrade to Halberdier", "Upgrade");
                types["Type"].Add("Upgrade to Eagle Warrior", "Upgrade");
                types["Type"].Add("Upgrade to Elite Eagle Warrior", "Upgrade");
                types["Type"].Add("Upgrade to Light Cavalry", "Upgrade");
                types["Type"].Add("Upgrade to Hussar", "Upgrade");
                types["Type"].Add("Upgrade to Winged Hussar", "Upgrade");
                types["Type"].Add("Upgrade to Cavalier", "Upgrade");
                types["Type"].Add("Upgrade to Paladin", "Upgrade");
                types["Type"].Add("Upgrade to Heavy Camel Rider", "Upgrade");
                types["Type"].Add("Upgrade to Imperial Camel Rider", "Upgrade");
                types["Type"].Add("Upgrade to Elite Battle Elephant", "Upgrade");
                types["Type"].Add("Upgrade to Elite Steppe Lancer", "Upgrade");
                types["Type"].Add("Upgrade to Capped Ram", "Upgrade");
                types["Type"].Add("Upgrade to Siege Ram", "Upgrade");
                types["Type"].Add("Upgrade to Onager", "Upgrade");
                types["Type"].Add("Upgrade to Siege Onager", "Upgrade");
                types["Type"].Add("Upgrade to Heavy Scorpion", "Upgrade");
                types["Type"].Add("Upgrade to Houfnice", "Upgrade");
                types["Type"].Add("Upgrade to War Galley, Fire Ships and Demolition Ships", "Upgrade");
                types["Type"].Add("Tech: Elite Cannon Galleon", "Upgrade");
                types["Type"].Add("Upgrade to Heavy Demolition Ship", "Upgrade");
                types["Type"].Add("Upgrade to Fast Fire Ship", "Upgrade");
                types["Type"].Add("Upgrade to Galleon", "Upgrade");
                types["Type"].Add("Upgrade to Elite Turtle Ship", "Upgrade");
                types["Type"].Add("Upgrade to Elite Longboat", "Upgrade");
                types["Type"].Add("Upgrade to Elite Caravel", "Upgrade");
                types["Type"].Add("Upgrade to Fortified Wall", "Upgrade");
                types["Type"].Add("Upgrade to Guard Tower", "Upgrade");
                types["Type"].Add("Upgrade to Keep", "Upgrade");
                types["Type"].Add("Upgrade to Elite Jaguar Warrior", "Upgrade");
                types["Type"].Add("Upgrade to Elite Camel Archer", "Upgrade");
                types["Type"].Add("Upgrade to Elite Hussite Wagon", "Upgrade");
                types["Type"].Add("Upgrade to Elite Longbowman", "Upgrade");
                types["Type"].Add("Upgrade to Elite Konnik", "Upgrade");
                types["Type"].Add("Upgrade to Elite Coustillier", "Upgrade");
                types["Type"].Add("Upgrade to Elite Arambai", "Upgrade");
                types["Type"].Add("Upgrade to Elite Cataphract", "Upgrade");
                types["Type"].Add("Upgrade to Elite Woad Raider", "Upgrade");
                types["Type"].Add("Upgrade to Elite Chu Ko Nu", "Upgrade");
                types["Type"].Add("Upgrade to Elite Kipchak", "Upgrade");
                types["Type"].Add("Upgrade to Elite Shotel Warrior", "Upgrade");
                types["Type"].Add("Upgrade to Elite Throwing Axeman", "Upgrade");
                types["Type"].Add("Upgrade to Elite Huskarl", "Upgrade");
                types["Type"].Add("Upgrade to Elite Tarkan", "Upgrade");
                types["Type"].Add("Upgrade to Elite Kamayuk", "Upgrade");
                types["Type"].Add("Upgrade to Elite Elephant Archer", "Upgrade");
                types["Type"].Add("Upgrade to Elite Genoese Crossbowman", "Upgrade");
                types["Type"].Add("Upgrade to Elite Samurai", "Upgrade");
                types["Type"].Add("Upgrade to Elite Ballista Elephant", "Upgrade");
                types["Type"].Add("Upgrade to Elite War Wagon", "Upgrade");
                types["Type"].Add("Upgrade to Elite Leitis", "Upgrade");
                types["Type"].Add("Upgrade to Elite Magyar Huszar", "Upgrade");
                types["Type"].Add("Upgrade to Elite Karambit Warrior", "Upgrade");
                types["Type"].Add("Upgrade to Elite Gbeto", "Upgrade");
                types["Type"].Add("Upgrade to Elite Plumed Archer", "Upgrade");
                types["Type"].Add("Upgrade to Elite Mangudai", "Upgrade");
                types["Type"].Add("Upgrade to Elite War Elephant", "Upgrade");
                types["Type"].Add("Upgrade to Elite Obuch", "Upgrade");
                types["Type"].Add("Upgrade to Elite Organ Gun", "Upgrade");
                types["Type"].Add("Upgrade to Elite Mameluke", "Upgrade");
                types["Type"].Add("Upgrade to Elite Serjeant", "Upgrade");
                types["Type"].Add("Upgrade to Elite Boyar", "Upgrade");
                types["Type"].Add("Upgrade to Elite Conquistador", "Upgrade");
                types["Type"].Add("Upgrade to Elite Keshik", "Upgrade");
                types["Type"].Add("Upgrade to Elite Teutonic Knight", "Upgrade");
                types["Type"].Add("Upgrade to Elite Janissary", "Upgrade");
                types["Type"].Add("Upgrade to Elite Rattan Archer", "Upgrade");
                types["Type"].Add("Upgrade to Elite Berserk", "Upgrade");
                types["Type"].Add("Black Forest", "Map");
                types["Type"].Add("Arabia", "Map");
                types["Type"].Add("Arena", "Map");
                types["Type"].Add("Migration", "Map");
                types["Type"].Add("Mediterranean", "Map");
                types["Type"].Add("Random Map", "Map");
                types["Type"].Add("Death Match", "Map");
                types["Type"].Add("King of the Hill", "Map");
                types["Type"].Add("Regicide", "Map");
                types["Type"].Add("Michi", "Map");
                types["Type"].Add("Lombardia", "Map");
                types["Type"].Add("Runestones", "Map");
                types["Type"].Add("Islands", "Map");
                types["Type"].Add("Oasis", "Map");
                types["Type"].Add("Fortress", "Map");
                types["Type"].Add("Nomad", "Map");
                types["Type"].Add("MegaRandom", "Map");
                types["Type"].Add("Hideout", "Map");
                types["Type"].Add("Gold Rush", "Map");
                types["Type"].Add("Valley", "Map");
                types["Type"].Add("Scandinavia", "Map");
                types["Type"].Add("Golden Pit", "Map");
                types["Type"].Add("Yucatan", "Map");
                types["Type"].Add("Hill Fort", "Map");
                types["Type"].Add("Team Islands", "Map");
                types["Type"].Add("Steppe", "Map");
                types["Type"].Add("Ghost Lake", "Map");
                types["Type"].Add("Baltic", "Map");
                types["Type"].Add("Socotra", "Map");
                types["Type"].Add("African Clearing", "Map");
                types["Type"].Add("Acclivity", "Map");
                types["Type"].Add("Acropolis", "Map");
                types["Type"].Add("Aftermath", "Map");
                types["Type"].Add("Amazon Tunnel", "Map");
                types["Type"].Add("Archipelago", "Map");
                types["Type"].Add("Atacama", "Map");
                types["Type"].Add("Coastal", "Map");
                types["Type"].Add("Coastal Forest", "Map");
                types["Type"].Add("Continental", "Map");
                types["Type"].Add("Marketplace", "Map");
                types["Type"].Add("Mongolia", "Map");
                types["Type"].Add("Random Land Map", "Map");
                types["Type"].Add("Sacred Springs", "Map");
                types["Type"].Add("Salt Marsh", "Map");
                types["Type"].Add("Age of Empires II", "Other");
                types["Type"].Add("Arrow", "Other");
                types["Type"].Add("Health", "Other");
                types["Type"].Add("Armor", "Other");
                types["Type"].Add("Civilization", "Other");
                types["Type"].Add("Population", "Other");
                types["Type"].Add("Range*", "Other");
                types["Type"].Add("Speed", "Other");
                types["Type"].Add("Line of Sight", "Other");
                types["Type"].Add("Unique Unit", "Other");
                types["Type"].Add("Idle", "Other");
                types["Type"].Add("Gather Point", "Other");
                types["Type"].Add("Artificial Intelligence", "Other");
                types["Type"].Add("Pierce Armor: ", "Other");
                types["Type"].Add("Long Distance", "Other");
                types["Type"].Add("Time", "Other");
                types["Type"].Add("Cartography", "Other");
                types["Type"].Add("Tracking", "Other");
                types["Type"].Add("Cursor", "Other");
                types["Type"].Add("Arrow Up", "Other");
                types["Type"].Add("->", "Other");
                types["Type"].Add("1", "Other");
                types["Type"].Add("2", "Other");
                types["Type"].Add("3", "Other");
                types["Type"].Add("4", "Other");
                types["Type"].Add("5", "Other");
                types["Type"].Add("6", "Other");
                types["Type"].Add("7", "Other");
                types["Type"].Add("8", "Other");
                types["Type"].Add("9", "Other");
                types["Type"].Add("0", "Other");
                types["Type"].Add("-", "Other");
                types["Type"].Add("Explore", "Action");
                types["Type"].Add("Flag", "Other");
                types["Type"].Add("1s", "Other");
                types["Type"].Add("2s", "Other");
                types["Type"].Add("3s", "Other");
                types["Type"].Add("4s", "Other");
                types["Type"].Add("5s", "Other");
                types["Type"].Add("6s", "Other");
                types["Type"].Add("7s", "Other");
                types["Type"].Add("8s", "Other");
                types["Type"].Add("9s", "Other");
                types["Type"].Add("10s", "Other");
                types["Type"].Add("Green Arabia", "Map");
                types["Type"].Add("Opponent Villager", "Unit");
                types["Type"].Add("Less Than", "Other");
                types["Type"].Add("Open Square Bracket", "Other");
                types["Type"].Add("Close Square Bracket", "Other");
                types["Type"].Add("Random Civilization", "Civilization");

            }

            File.WriteAllText(Preferencias.TypesDefaultPath, SerializarNombres(types));

        } // CrearArchivoTipos>


        public static void CrearArchivoImágenesPersonalizadas() {

            var customImages = new Dictionary<NameType, Dictionary<string, string>>();
            customImages.Add(NameType.Image, new Dictionary<string, string>());
            if (Preferencias.Game == AOE2Name) {

                customImages[NameType.Image].Add("Villager", "Male Villager|Female Villager");
                customImages[NameType.Image].Add("Jaguar (Animal)", "Jaguar");
                customImages[NameType.Image].Add("Watch Tower", "Guard Tower");
                customImages[NameType.Image].Add("Spies/Treason", "Spies-Treason");
                customImages[NameType.Image].Add("Atlatl", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Garland Wars", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Kasbah", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Maghrebi Camels", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Wagenburg Tactics", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Hussite Reforms", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Yeomen", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Warwolf", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Stirrups", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Bagains", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Burgundian Vineyards", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Flemish Revolution", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Howdah", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Manipur Cavalry", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Greek Fire", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Logistica", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Stronghold", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Furor Celtica", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Great Wall", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Rocketry", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Steppe Husbandry", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Cuman Mercenaries", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Royal Heirs", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Torsion Engines", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Bearded Axe", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Chivalry", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Anarchy", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Perfusion", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Marauders", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Atheism", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Andean Sling", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Fabric Shields", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Sultans", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Shatagni", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Pavise", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Silk Road", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Yasama", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Kataparuto", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Tusk Swords", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Double Crossbow", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Eupseong", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Shinkichon", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Hill Forts", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Tower Shields", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Corvinian Army", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Recurve Bow", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Thalassocracy", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Forced Levy", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Tigui", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Farimba", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Hul'che Javelineers", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("El Dorado", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Nomads", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Drill", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Kamandaran", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Mahouts", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Szlachta Privileges", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Lechitic Legacy", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Carrack", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Arquebus", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Madrasah", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Zealotry", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("First Crusade", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Hauberk", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Orthodoxy", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Druzhina", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Inquisition", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Supremacy", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Silk Armor", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Timurid Siegecraft", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Ironclad", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Crenellations", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Sipahi", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Artillery", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Chatras", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Paper Money", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Chieftains", "Castle Age Unique Technology");
                customImages[NameType.Image].Add("Berserkergang", "Imperial Age Unique Technology");
                customImages[NameType.Image].Add("Tech: Bombard Tower", "Research Bombard Tower");
                customImages[NameType.Image].Add("Research Cannon Galleon (requires Chemistry)", "Research Cannon Galleon");
                customImages[NameType.Image].Add("Tech: Elite Cannon Galleon", "Upgrade to Elite Cannon Galleon");
                customImages[NameType.Image].Add("Range*", "Range");
                customImages[NameType.Image].Add("Pierce Armor: ", "Pierce Armor");
                customImages[NameType.Image].Add("Opponent Villager", "Opponent Male Villager|Opponent Female Villager");


            }

            File.WriteAllText(Preferencias.CustomImagesDefaultPath, SerializarNombres(customImages));

        } // CrearArchivoImágenesPersonalizadas>


        public static bool EsIdioma(NameType tipo) => tipo.ToString().Length == 2;


        public static void CrearArchivoNombres() {

            var names = DeserializarNombres(Preferencias.EnglishNamesPath);

            if (Preferencias.Game == AOE2Name) {

                foreach (var tipo in Vixark.General.ObtenerValores<NameType>()) {

                    var nombreTipo = tipo.ToString();
                    if (EsIdioma(tipo)) { 

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

            File.WriteAllText(Preferencias.NamesPath, SerializarNombres(names));

        } // CrearArchivoNombres>


        public static void CrearArchivoNombresPersonalizadosDefault() {

            var customNames = new Dictionary<NameType, Dictionary<string, string>>();
            customNames.Add(NameType.Custom, new Dictionary<string, string>());
            if (Preferencias.Game == AOE2Name) {

                customNames[NameType.Custom].Add("Dark Age", "DRK");
                customNames[NameType.Custom].Add("Feudal Age", "FDL");
                customNames[NameType.Custom].Add("Castle Age", "CST");
                customNames[NameType.Custom].Add("Imperial Age", "IMP");
                customNames[NameType.Custom].Add("Food", "[Image]");
                customNames[NameType.Custom].Add("Wood", "[Image]");
                customNames[NameType.Custom].Add("Stone", "[Image]");
                customNames[NameType.Custom].Add("Gold", "[Image]");
                customNames[NameType.Custom].Add("Straggler Trees", "STR");
                customNames[NameType.Custom].Add("Archer", "ARCH");
                customNames[NameType.Custom].Add("Crossbowman", "[Acronym]");
                customNames[NameType.Custom].Add("Arbalester", "ARB");
                customNames[NameType.Custom].Add("Skirmisher", "SKR");
                customNames[NameType.Custom].Add("Elite Skirmisher", "ESKR");
                customNames[NameType.Custom].Add("Hand Cannoneer", "HCAN");
                customNames[NameType.Custom].Add("Cavalry Archer", "CA");
                customNames[NameType.Custom].Add("Heavy Cavalry Archer", "HCA");
                customNames[NameType.Custom].Add("Militia", "MIL");
                customNames[NameType.Custom].Add("Man-at-Arms", "MAA");
                customNames[NameType.Custom].Add("Long Swordsman", "LS");
                customNames[NameType.Custom].Add("Two-Handed Swordsman", "2HS");
                customNames[NameType.Custom].Add("Champion", "CHMP");
                customNames[NameType.Custom].Add("Spearman", "SPR");
                customNames[NameType.Custom].Add("Pikeman", "PK");
                customNames[NameType.Custom].Add("Halberdier", "HALB");
                customNames[NameType.Custom].Add("Eagle Scout", "ESC");
                customNames[NameType.Custom].Add("Eagle Warrior", "EW");
                customNames[NameType.Custom].Add("Elite Eagle Warrior", "EEW");
                customNames[NameType.Custom].Add("Scout Cavalry", "SC");
                customNames[NameType.Custom].Add("Light Cavalry", "LCV");
                customNames[NameType.Custom].Add("Hussar", "HSS");
                customNames[NameType.Custom].Add("Cavalier", "CAV");
                customNames[NameType.Custom].Add("Paladin", "PAL");
                customNames[NameType.Custom].Add("Camel Rider", "CML");
                customNames[NameType.Custom].Add("Heavy Camel Rider", "HCML");
                customNames[NameType.Custom].Add("Battle Elephant", "BE");
                customNames[NameType.Custom].Add("Elite Battle Elephant", "EBE");
                customNames[NameType.Custom].Add("Steppe Lancer", "SL");
                customNames[NameType.Custom].Add("Elite Steppe Lancer", "ESL");
                customNames[NameType.Custom].Add("Battering Ram", "RAM");
                customNames[NameType.Custom].Add("Capped Ram", "CRAM");
                customNames[NameType.Custom].Add("Siege Ram", "SRAM");
                customNames[NameType.Custom].Add("Mangonel", "MNG");
                customNames[NameType.Custom].Add("Onager", "ONG");
                customNames[NameType.Custom].Add("Siege Onager", "SO");
                customNames[NameType.Custom].Add("Scorpion", "SCR");
                customNames[NameType.Custom].Add("Heavy Scorpion", "HSCR");
                customNames[NameType.Custom].Add("Siege Tower", "STWR");
                customNames[NameType.Custom].Add("Bombard Cannon", "BBC");
                customNames[NameType.Custom].Add("Petard", "PET");
                customNames[NameType.Custom].Add("Trebuchet", "TREB");
                customNames[NameType.Custom].Add("Monk", "MNK");
                customNames[NameType.Custom].Add("Archery Range", "AR");
                customNames[NameType.Custom].Add("Barracks", "RAX");
                customNames[NameType.Custom].Add("Stable", "STB");
                customNames[NameType.Custom].Add("Siege Workshop", "SW");
                customNames[NameType.Custom].Add("Blacksmith", "BLK");
                customNames[NameType.Custom].Add("University", "UNI");
                customNames[NameType.Custom].Add("Dock", "DOCK");
                customNames[NameType.Custom].Add("Watch Tower", "TWR");
                customNames[NameType.Custom].Add("Bombard Tower", "BBT");
                customNames[NameType.Custom].Add("Palisade Wall", "PWLL");
                customNames[NameType.Custom].Add("Stone Wall", "SWLL");
                customNames[NameType.Custom].Add("Castle", "CSTL");
                customNames[NameType.Custom].Add("Krepost", "KRP");
                customNames[NameType.Custom].Add("Donjon", "DONJ");
                customNames[NameType.Custom].Add("Monastery", "MST");
                customNames[NameType.Custom].Add("House", "HS");
                customNames[NameType.Custom].Add("Town Center", "TC");
                customNames[NameType.Custom].Add("Feitoria", "FEIT");
                customNames[NameType.Custom].Add("Mining Camp", "MC");
                customNames[NameType.Custom].Add("Lumber Camp", "LC");
                customNames[NameType.Custom].Add("Folwark", "FWK");
                customNames[NameType.Custom].Add("Mill", "MILL");
                customNames[NameType.Custom].Add("Farm", "FARM");
                customNames[NameType.Custom].Add("Market", "MKT");
                customNames[NameType.Custom].Add("Supplies", "SPL");
                customNames[NameType.Custom].Add("Bloodlines", "BLDL");
                customNames[NameType.Custom].Add("Padded Archer Armor", "PAA");
                customNames[NameType.Custom].Add("Fletching", "FCH");
                customNames[NameType.Custom].Add("Forging", "FRG");
                customNames[NameType.Custom].Add("Scale Barding Armor", "SBA");
                customNames[NameType.Custom].Add("Scale Mail Armor", "SMA");
                customNames[NameType.Custom].Add("Leather Archer Armor", "LAA");
                customNames[NameType.Custom].Add("Bodkin Arrow", "BA");
                customNames[NameType.Custom].Add("Iron Casting", "IC");
                customNames[NameType.Custom].Add("Chain Barding Armor", "CBA");
                customNames[NameType.Custom].Add("Chain Mail Armor", "CMA");
                customNames[NameType.Custom].Add("Ring Archer Armor", "RAA");
                customNames[NameType.Custom].Add("Bracer", "BRC");
                customNames[NameType.Custom].Add("Blast Furnace", "BF");
                customNames[NameType.Custom].Add("Plate Barding Armor", "PBA");
                customNames[NameType.Custom].Add("Plate Mail Armor", "PMA");
                customNames[NameType.Custom].Add("Treadmill Crane", "TMC");
                customNames[NameType.Custom].Add("Fervor", "FRV");
                customNames[NameType.Custom].Add("Wheelbarrow", "WB");
                customNames[NameType.Custom].Add("Hand Cart", "HC");
                customNames[NameType.Custom].Add("Gold Mining", "GM");
                customNames[NameType.Custom].Add("Stone Mining", "SM");
                customNames[NameType.Custom].Add("Gold Shaft Mining", "GSM");
                customNames[NameType.Custom].Add("Stone Shaft Mining", "SSM");
                customNames[NameType.Custom].Add("Double-Bit Axe", "DBA");
                customNames[NameType.Custom].Add("Bow Saw", "BS");
                customNames[NameType.Custom].Add("Two-Man Saw", "TMS");
                customNames[NameType.Custom].Add("Horse Collar", "HRC");
                customNames[NameType.Custom].Add("Heavy Plow", "HP");
                customNames[NameType.Custom].Add("Crop Rotation", "CR");
                customNames[NameType.Custom].Add("Civilization", "CIV");
                customNames[NameType.Custom].Add("Population", "Pop");
                customNames[NameType.Custom].Add("Long Distance", "LD");
                customNames[NameType.Custom].Add("Time", "t");

            }

            File.WriteAllText(Preferencias.CustomNamesDefaultPath, SerializarNombres(customNames));

        } // CrearArchivoNombresPersonalizados>


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
                names[NameType.Complete].Add("4202", "Feudal Age");
                names[NameType.Complete].Add("4203", "Castle Age");
                names[NameType.Complete].Add("4204", "Imperial Age"); names[NameType.Abbreviation].Add("4204", "Imp");
                names[NameType.Complete].Add("4105", "Stop");
                names[NameType.Complete].Add("4107", "Unload");
                names[NameType.Complete].Add("4131", "Automatically Reseed Farms"); names[NameType.Common].Add("4131", "Autofarm");
                names[NameType.Complete].Add("4121", "Automatically Rebuild Fish Traps");
                names[NameType.Complete].Add("4123", "Attack Ground");
                names[NameType.Complete].Add("4124", "Heal");
                names[NameType.Complete].Add("4125", "Convert");
                names[NameType.Complete].Add("4133", "Aggressive Stance"); names[NameType.Common].Add("4133", "Aggressive");
                names[NameType.Complete].Add("19350", "Auto Scout");
                names[NameType.Complete].Add("4134", "Defensive Stance"); names[NameType.Common].Add("4134", "Defensive");
                names[NameType.Complete].Add("4135", "No Attack Stance"); names[NameType.Common].Add("4135", "No Attack");
                names[NameType.Complete].Add("4136", "Guard");
                names[NameType.Complete].Add("4137", "Follow");
                names[NameType.Complete].Add("4138", "Patrol");
                names[NameType.Complete].Add("4144", "Set Gather Point");
                names[NameType.Complete].Add("11580", "Attack Move");
                names[NameType.Complete].Add("10109", "Garrison");
                names[NameType.Complete].Add("13051", "Defend");
                names[NameType.Complete].Add("13052", "Build");
                names[NameType.Complete].Add("5316", "Flare");
                names[NameType.Complete].Add("19319", "Ring Town Bell");
                names[NameType.Complete].Add("4013", "Delete");
                names[NameType.Complete].Add("4122", "Stand Ground");
                names[NameType.Complete].Add("4307", "Attack"); names[NameType.Abbreviation].Add("4307", "Att");
                names[NameType.Complete].Add("400018", "Drop Food");
                names[NameType.Complete].Add("400019", "Drop Gold");
                names[NameType.Complete].Add("400020", "Drop Stone");
                names[NameType.Complete].Add("400021", "Drop Wood");
                names[NameType.Complete].Add("400022", "Gather Berries");
                names[NameType.Complete].Add("400023", "Gather Meat");
                names[NameType.Complete].Add("13063", "Hunt");
                names[NameType.Complete].Add("400024", "Fish");
                names[NameType.Complete].Add("400025", "Chop");
                names[NameType.Complete].Add("400027", "Mine Gold");
                names[NameType.Complete].Add("400028", "Mine Stone");
                names[NameType.Complete].Add("5071", "Deer"); names[NameType.CommonPlural].Add("5071", "Deers");
                names[NameType.Complete].Add("5337", "Fish (Tuna)"); names[NameType.Common].Add("5337", "Fish");
                names[NameType.Complete].Add("5350", "Relic"); names[NameType.Abbreviation].Add("5350", "Rel"); names[NameType.CommonPlural].Add("5350", "Relics"); names[NameType.AbbreviationPlural].Add("5350", "Rels");
                names[NameType.Complete].Add("5397", "Tree (Oak)"); names[NameType.Common].Add("5397", "Tree"); names[NameType.CommonPlural].Add("5397", "Trees");
                names[NameType.Complete].Add("5401", "Forage Bush"); names[NameType.Common].Add("5401", "Berry Bush"); names[NameType.Abbreviation].Add("5401", "Berry"); names[NameType.CommonPlural].Add("5401", "Forage Bushes|Berry Bushes"); names[NameType.AbbreviationPlural].Add("5401", "Berries");
                names[NameType.Complete].Add("5406", "Wild Boar"); names[NameType.Common].Add("5406", "Boar"); names[NameType.CommonPlural].Add("5406", "Boars");
                names[NameType.Complete].Add("5498", "Sheep"); names[NameType.Acronym].Add("5498", "SHP"); names[NameType.CommonPlural].Add("5498", "Sheeps");
                names[NameType.Complete].Add("5502", "Cow"); names[NameType.CommonPlural].Add("5502", "Cows");
                names[NameType.Complete].Add("5503", "Llama"); names[NameType.CommonPlural].Add("5503", "Llamas");
                names[NameType.Complete].Add("4301", "Food"); names[NameType.Acronym].Add("4301", "F");
                names[NameType.Complete].Add("4302", "Wood"); names[NameType.Acronym].Add("4302", "W");
                names[NameType.Complete].Add("4303", "Stone"); names[NameType.Acronym].Add("4303", "S");
                names[NameType.Complete].Add("4304", "Gold"); names[NameType.Acronym].Add("4304", "G");
                names[NameType.Complete].Add("5056", "Zebra"); names[NameType.CommonPlural].Add("5056", "Zebras");
                names[NameType.Complete].Add("5057", "Ostrich"); names[NameType.CommonPlural].Add("5057", "Ostriches");
                names[NameType.Complete].Add("5061", "Goat"); names[NameType.CommonPlural].Add("5061", "Goats");
                names[NameType.Complete].Add("5733", "Turkey");
                names[NameType.Complete].Add("5340", "Ibex");
                names[NameType.Complete].Add("5346", "Pig");
                names[NameType.Complete].Add("5345", "Goose");
                names[NameType.Complete].Add("400011", "Straggler Trees");
                names[NameType.Complete].Add("5172", "Rhinoceros"); names[NameType.Abbreviation].Add("5172", "Rhino"); names[NameType.CommonPlural].Add("5172", "Rhinoceroses"); names[NameType.AbbreviationPlural].Add("5172", "Rhinos");
                names[NameType.Complete].Add("5173", "Box Turtles"); names[NameType.Common].Add("5173", "Turtles");
                names[NameType.Complete].Add("5175", "Water Buffalo"); names[NameType.Common].Add("5175", "Buffalo"); names[NameType.CommonPlural].Add("5175", "Buffalos");
                names[NameType.Complete].Add("5743", "Elephant"); names[NameType.Abbreviation].Add("5743", "Ele"); names[NameType.CommonPlural].Add("5743", "Elephants"); names[NameType.AbbreviationPlural].Add("5743", "Eles");
                names[NameType.Complete].Add("5796", "Fruit Bush");
                names[NameType.Complete].Add("5252", "Stone Mine");
                names[NameType.Complete].Add("5400", "Gold Mine");
                names[NameType.Complete].Add("400029", "Food to Gold");
                names[NameType.Complete].Add("400030", "Food to Stone");
                names[NameType.Complete].Add("400031", "Food to wood");
                names[NameType.Complete].Add("400032", "Gold to Food");
                names[NameType.Complete].Add("400033", "Gold to Stone");
                names[NameType.Complete].Add("400034", "Gold to wood");
                names[NameType.Complete].Add("400035", "Stone to Food");
                names[NameType.Complete].Add("400036", "Stone to Gold");
                names[NameType.Complete].Add("400037", "Stone to wood");
                names[NameType.Complete].Add("400038", "Wood to Food");
                names[NameType.Complete].Add("400039", "Wood to Gold");
                names[NameType.Complete].Add("400040", "Wood to Stone");
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
                names[NameType.Complete].Add("5079", "Militia"); names[NameType.Abbreviation].Add("5079", "Mil"); names[NameType.CommonPlural].Add("5079", "Militias");
                names[NameType.Complete].Add("5080", "Man-at-Arms"); names[NameType.Acronym].Add("5080", "MAA|M@A"); names[NameType.CommonPlural].Add("5080", "Men-at-Arms");
                names[NameType.Complete].Add("5081", "Long Swordsman"); names[NameType.Abbreviation].Add("5081", "Longsword"); names[NameType.Acronym].Add("5081", "LS"); names[NameType.CommonPlural].Add("5081", "Long Swordsmen"); names[NameType.AbbreviationPlural].Add("5081", "Longswords");
                names[NameType.Complete].Add("5411", "Two-Handed Swordsman"); names[NameType.Acronym].Add("5411", "2HS"); names[NameType.CommonPlural].Add("5411", "Two-Handed Swordsmen");
                names[NameType.Complete].Add("5469", "Champion"); names[NameType.Abbreviation].Add("5469", "Champ"); names[NameType.CommonPlural].Add("5469", "Champions"); names[NameType.AbbreviationPlural].Add("5469", "Champs");
                names[NameType.Complete].Add("5078", "Spearman"); names[NameType.Abbreviation].Add("5078", "Spear"); names[NameType.CommonPlural].Add("5078", "Spearmen"); names[NameType.AbbreviationPlural].Add("5078", "Spears");
                names[NameType.Complete].Add("5408", "Pikeman"); names[NameType.Abbreviation].Add("5408", "Pike"); names[NameType.CommonPlural].Add("5408", "Pikemen"); names[NameType.AbbreviationPlural].Add("5408", "Pikes");
                names[NameType.Complete].Add("5409", "Halberdier"); names[NameType.Abbreviation].Add("5409", "Halb"); names[NameType.CommonPlural].Add("5409", "Halberdiers"); names[NameType.AbbreviationPlural].Add("5409", "Halbs");
                names[NameType.Complete].Add("5672", "Eagle Scout"); names[NameType.CommonPlural].Add("5672", "Eagle Scouts");
                names[NameType.Complete].Add("5671", "Eagle Warrior"); names[NameType.Common].Add("5671", "Eagle"); names[NameType.Acronym].Add("5671", "EW"); names[NameType.CommonPlural].Add("5671", "Eagles");
                names[NameType.Complete].Add("5673", "Elite Eagle Warrior");
                names[NameType.Complete].Add("5114", "Condottiero"); names[NameType.Abbreviation].Add("5114", "Condo"); names[NameType.CommonPlural].Add("5114", "Condottieros"); names[NameType.AbbreviationPlural].Add("5114", "Condos");
                names[NameType.Complete].Add("5326", "Scout Cavalry"); names[NameType.Common].Add("5326", "Scout"); names[NameType.Acronym].Add("5326", "SC"); names[NameType.CommonPlural].Add("5326", "Scouts");
                names[NameType.Complete].Add("5069", "Light Cavalry"); names[NameType.Abbreviation].Add("5069", "Light Cav"); names[NameType.Acronym].Add("5069", "LCav"); names[NameType.CommonPlural].Add("5069", "Light Cavalries"); names[NameType.AbbreviationPlural].Add("5069", "Light Cavs"); names[NameType.AcronymPlural].Add("5069", "LCavs");
                names[NameType.Complete].Add("5661", "Hussar"); names[NameType.CommonPlural].Add("5661", "Hussars");
                names[NameType.Complete].Add("5577", "Winged Hussar"); names[NameType.CommonPlural].Add("5577", "Winged Hussars");
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
                names[NameType.Complete].Add("5094", "Battering Ram"); names[NameType.Common].Add("5094", "Ram"); names[NameType.CommonPlural].Add("5094", "Rams");
                names[NameType.Complete].Add("5289", "Capped Ram"); names[NameType.CommonPlural].Add("5289", "Capped Rams");
                names[NameType.Complete].Add("5446", "Siege Ram"); names[NameType.CommonPlural].Add("5446", "Siege Rams");
                names[NameType.Complete].Add("5095", "Mangonel"); names[NameType.Abbreviation].Add("5095", "Mango"); names[NameType.CommonPlural].Add("5095", "Mangonels"); names[NameType.AbbreviationPlural].Add("5095", "Mangos");
                names[NameType.Complete].Add("5448", "Onager"); names[NameType.CommonPlural].Add("5448", "Onagers");
                names[NameType.Complete].Add("5493", "Siege Onager"); names[NameType.Acronym].Add("5493", "SO"); names[NameType.CommonPlural].Add("5493", "Siege Onagers");
                names[NameType.Complete].Add("5096", "Scorpion"); names[NameType.Abbreviation].Add("5096", "Scorp"); names[NameType.CommonPlural].Add("5096", "Scorpions"); names[NameType.AbbreviationPlural].Add("5096", "Scorps");
                names[NameType.Complete].Add("5439", "Heavy Scorpion"); names[NameType.Abbreviation].Add("5439", "Heavy Scorp"); names[NameType.Acronym].Add("5439", "HScorp"); names[NameType.CommonPlural].Add("5439", "Heavy Scorpions"); names[NameType.AbbreviationPlural].Add("5439", "Heavy Scorps"); names[NameType.AcronymPlural].Add("5439", "HScorps");
                names[NameType.Complete].Add("5445", "Siege Tower"); names[NameType.CommonPlural].Add("5445", "Siege Towers");
                names[NameType.Complete].Add("5093", "Bombard Cannon"); names[NameType.Acronym].Add("5093", "BBC"); names[NameType.CommonPlural].Add("5093", "Bombard Cannons");
                names[NameType.Complete].Add("5579", "Houfnice"); names[NameType.CommonPlural].Add("5579", "Houfnices");
                names[NameType.Complete].Add("5090", "Fishing Ship"); names[NameType.CommonPlural].Add("5090", "Fishing Ships");
                names[NameType.Complete].Add("5443", "Transport Ship"); names[NameType.Abbreviation].Add("5443", "Xport"); names[NameType.CommonPlural].Add("5443", "Transport Ships"); names[NameType.AbbreviationPlural].Add("5443", "Xports");
                names[NameType.Complete].Add("5160", "Fire Galley"); names[NameType.CommonPlural].Add("5160", "Fire Galleys");
                names[NameType.Complete].Add("5089", "Trade Cog"); names[NameType.CommonPlural].Add("5089", "Trade Cogs");
                names[NameType.Complete].Add("5162", "Demolition Raft"); names[NameType.Abbreviation].Add("5162", "Demo Raft"); names[NameType.CommonPlural].Add("5162", "Demolition Rafts"); names[NameType.AbbreviationPlural].Add("5162", "Demo Rafts");
                names[NameType.Complete].Add("5436", "Galley"); names[NameType.CommonPlural].Add("5436", "Galleys");
                names[NameType.Complete].Add("5426", "Fire Ship"); names[NameType.Abbreviation].Add("5426", "Fire"); names[NameType.CommonPlural].Add("5426", "Fire Ships"); names[NameType.AbbreviationPlural].Add("5426", "Fires");
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
                names[NameType.Complete].Add("5660", "Petard"); names[NameType.Abbreviation].Add("5660", "Pet"); names[NameType.CommonPlural].Add("5660", "Petards"); names[NameType.AbbreviationPlural].Add("5660", "Pets");
                names[NameType.Complete].Add("5097", "Trebuchet"); names[NameType.Abbreviation].Add("5097", "Treb"); names[NameType.CommonPlural].Add("5097", "Trebuchets"); names[NameType.AbbreviationPlural].Add("5097", "Trebs");
                names[NameType.Complete].Add("5099", "Monk"); names[NameType.CommonPlural].Add("5099", "Monks");
                names[NameType.Complete].Add("5691", "Missionary"); names[NameType.Abbreviation].Add("5691", "Donk"); names[NameType.CommonPlural].Add("5691", "Missionaries"); names[NameType.AbbreviationPlural].Add("5691", "Donks");
                names[NameType.Complete].Add("14121", "Villager"); names[NameType.Abbreviation].Add("14121", "Vil|Vill"); names[NameType.Acronym].Add("14121", "V"); names[NameType.CommonPlural].Add("14121", "Villagers"); names[NameType.AbbreviationPlural].Add("14121", "Vils|Vills");
                names[NameType.Complete].Add("13319", "Trade Cart"); names[NameType.CommonPlural].Add("13319", "Trade Carts");
                names[NameType.Complete].Add("19141", "Flemish Militia"); names[NameType.CommonPlural].Add("19141", "Flemish Militias");
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
                names[NameType.Complete].Add("5313", "Keshik"); names[NameType.CommonPlural].Add("5313", "Keshiks");
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
                names[NameType.Complete].Add("5059", "Lion"); names[NameType.CommonPlural].Add("5059", "Lions");
                names[NameType.Complete].Add("5060", "Crocodile"); names[NameType.CommonPlural].Add("5060", "Crocodiles");
                names[NameType.Complete].Add("5170", "Tiger"); names[NameType.CommonPlural].Add("5170", "Tigers");
                names[NameType.Complete].Add("5075", "Wolf"); names[NameType.CommonPlural].Add("5075", "Wolves");
                names[NameType.Complete].Add("5163", "Komodo Dragon");
                names[NameType.Complete].Add("5654", "Dismounted Konnik");
                names[NameType.Complete].Add("400026", "American Monk");
                names[NameType.Complete].Add("400017", "Jaguar (Animal)");
                names[NameType.Complete].Add("5182", "Wonder"); names[NameType.CommonPlural].Add("5182", "Wonders");
                names[NameType.Complete].Add("5128", "Archery Range"); names[NameType.Common].Add("5128", "Archery|Range"); names[NameType.CommonPlural].Add("5128", "Archeries|Ranges");
                names[NameType.Complete].Add("5135", "Barracks"); names[NameType.Abbreviation].Add("5135", "Rax"); names[NameType.AbbreviationPlural].Add("5135", "Raxes");
                names[NameType.Complete].Add("5171", "Stable"); names[NameType.Acronym].Add("5171", "STBL|STB"); names[NameType.CommonPlural].Add("5171", "Stables");
                names[NameType.Complete].Add("5169", "Siege Workshop"); names[NameType.Acronym].Add("5169", "SW"); names[NameType.CommonPlural].Add("5169", "Siege Workshops");
                names[NameType.Complete].Add("5131", "Blacksmith");
                names[NameType.Complete].Add("5495", "Fish Trap"); names[NameType.CommonPlural].Add("5495", "Fish Traps");
                names[NameType.Complete].Add("5176", "University"); names[NameType.Abbreviation].Add("5176", "Uni"); names[NameType.CommonPlural].Add("5176", "Universities"); names[NameType.AbbreviationPlural].Add("5176", "Unis");
                names[NameType.Complete].Add("5144", "Dock"); names[NameType.CommonPlural].Add("5144", "Docks");
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
                names[NameType.Complete].Add("5142", "Castle"); names[NameType.CommonPlural].Add("5142", "Castles");
                names[NameType.Complete].Add("19329", "Krepost"); names[NameType.CommonPlural].Add("19329", "Kreposts");
                names[NameType.Complete].Add("19138", "Donjon"); names[NameType.CommonPlural].Add("19138", "Donjons");
                names[NameType.Complete].Add("5138", "Monastery"); names[NameType.CommonPlural].Add("5138", "Monasteries");
                names[NameType.Complete].Add("5344", "House"); names[NameType.Acronym].Add("5344", "H"); names[NameType.CommonPlural].Add("5344", "Houses");
                names[NameType.Complete].Add("5164", "Town Center"); names[NameType.Acronym].Add("5164", "TC"); names[NameType.CommonPlural].Add("5164", "Town Centers");
                names[NameType.Complete].Add("5159", "Feitoria");
                names[NameType.Complete].Add("5487", "Mining Camp"); names[NameType.Acronym].Add("5487", "MC"); names[NameType.CommonPlural].Add("5487", "Mining Camps");
                names[NameType.Complete].Add("5464", "Lumber Camp"); names[NameType.Acronym].Add("5464", "LC"); names[NameType.CommonPlural].Add("5464", "Lumber Camps");
                names[NameType.Complete].Add("5581", "Folwark"); names[NameType.CommonPlural].Add("5581", "Folwarks");
                names[NameType.Complete].Add("5157", "Mill"); names[NameType.CommonPlural].Add("5157", "Mills");
                names[NameType.Complete].Add("5149", "Farm"); names[NameType.CommonPlural].Add("5149", "Farms");
                names[NameType.Complete].Add("5161", "Market"); names[NameType.Acronym].Add("5161", "Mkt"); names[NameType.CommonPlural].Add("5161", "Markets");
                names[NameType.Complete].Add("5249", "Harbor");
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
                names[NameType.Complete].Add("10291", "Incas"); names[NameType.Acronym].Add("10291", "Inc");
                names[NameType.Complete].Add("10292", "Magyars"); names[NameType.Abbreviation].Add("10292", "Magy"); names[NameType.Acronym].Add("10292", "Mg");
                names[NameType.Complete].Add("10293", "Slavs"); names[NameType.Acronym].Add("10293", "SV");
                names[NameType.Complete].Add("10294", "Portuguese"); names[NameType.Abbreviation].Add("10294", "Port"); names[NameType.Acronym].Add("10294", "Por");
                names[NameType.Complete].Add("10295", "Ethiopians"); names[NameType.Abbreviation].Add("10295", "Ethi"); names[NameType.Acronym].Add("10295", "Et");
                names[NameType.Complete].Add("10296", "Malians"); names[NameType.Abbreviation].Add("10296", "Mali"); names[NameType.Acronym].Add("10296", "Mli");
                names[NameType.Complete].Add("10297", "Berbers"); names[NameType.Abbreviation].Add("10297", "Berb"); names[NameType.Acronym].Add("10297", "BB");
                names[NameType.Complete].Add("10298", "Khmer"); names[NameType.Acronym].Add("10298", "Kh");
                names[NameType.Complete].Add("10299", "Malay"); names[NameType.Acronym].Add("10299", "Mly");
                names[NameType.Complete].Add("10300", "Burmese"); names[NameType.Abbreviation].Add("10300", "Burm"); names[NameType.Acronym].Add("10300", "Bm");
                names[NameType.Complete].Add("10301", "Vietnamese"); names[NameType.Abbreviation].Add("10301", "Viet"); names[NameType.Acronym].Add("10301", "Vt");
                names[NameType.Complete].Add("10302", "Bulgarians"); names[NameType.Acronym].Add("10302", "Bul");
                names[NameType.Complete].Add("10303", "Tatars"); names[NameType.Acronym].Add("10303", "TT");
                names[NameType.Complete].Add("10304", "Cumans"); names[NameType.Acronym].Add("10304", "Cu");
                names[NameType.Complete].Add("10305", "Lithuanians"); names[NameType.Acronym].Add("10305", "Li");
                names[NameType.Complete].Add("10306", "Burgundians"); names[NameType.Acronym].Add("10306", "Bgd");
                names[NameType.Complete].Add("10307", "Sicilians"); names[NameType.Acronym].Add("10307", "Si");
                names[NameType.Complete].Add("10308", "Poles"); names[NameType.Acronym].Add("10308", "Pol");
                names[NameType.Complete].Add("10309", "Bohemians"); names[NameType.Acronym].Add("10309", "Bo");
                names[NameType.Complete].Add("7411", "Thumb Ring");
                names[NameType.Complete].Add("7415", "Parthian Tactics");
                names[NameType.Complete].Add("7403", "Supplies");
                names[NameType.Complete].Add("7210", "Squires");
                names[NameType.Complete].Add("7258", "Arson");
                names[NameType.Complete].Add("7409", "Bloodlines");
                names[NameType.Complete].Add("7039", "Husbandry");
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
                names[NameType.Complete].Add("7314", "Gillnets");
                names[NameType.Complete].Add("7372", "Careening");
                names[NameType.Complete].Add("7373", "Dry Dock");
                names[NameType.Complete].Add("7377", "Shipwright");
                names[NameType.Complete].Add("7050", "Masonry");
                names[NameType.Complete].Add("7051", "Architecture");
                names[NameType.Complete].Add("7047", "Chemistry");
                names[NameType.Complete].Add("7093", "Ballistics");
                names[NameType.Complete].Add("7378", "Siege Engineers");
                names[NameType.Complete].Add("7374", "Heated Shot");
                names[NameType.Complete].Add("7278", "Arrowslits");
                names[NameType.Complete].Add("7321", "Murder Holes");
                names[NameType.Complete].Add("7054", "Treadmill Crane");
                names[NameType.Complete].Add("7376", "Hoardings");
                names[NameType.Complete].Add("7322", "Sappers");
                names[NameType.Complete].Add("7319", "Conscription");
                names[NameType.Complete].Add("7408", "Spies/Treason");
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
                names[NameType.Complete].Add("7022", "Loom");
                names[NameType.Complete].Add("7008", "Town Watch");
                names[NameType.Complete].Add("7211", "Wheelbarrow"); names[NameType.Acronym].Add("7211", "WB");
                names[NameType.Complete].Add("7282", "Town Patrol");
                names[NameType.Complete].Add("7246", "Hand Cart");
                names[NameType.Complete].Add("7055", "Gold Mining");
                names[NameType.Complete].Add("7276", "Stone Mining");
                names[NameType.Complete].Add("7180", "Gold Shaft Mining");
                names[NameType.Complete].Add("7277", "Stone Shaft Mining");
                names[NameType.Complete].Add("7189", "Double-Bit Axe");
                names[NameType.Complete].Add("7190", "Bow Saw");
                names[NameType.Complete].Add("7231", "Two-Man Saw");
                names[NameType.Complete].Add("7014", "Horse Collar");
                names[NameType.Complete].Add("7023", "Coinage");
                names[NameType.Complete].Add("7410", "Caravan");
                names[NameType.Complete].Add("7017", "Banking");
                names[NameType.Complete].Add("7015", "Guilds");
                names[NameType.Complete].Add("7013", "Heavy Plow");
                names[NameType.Complete].Add("7012", "Crop Rotation");
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
                names[NameType.Complete].Add("19498", "Tech: Bombard Tower"); names[NameType.Common].Add("19498", "Research Bombard Tower");
                names[NameType.Complete].Add("400014", "Castle Age Unique Technology");
                names[NameType.Complete].Add("400015", "Imperial Age Unique Technology");
                names[NameType.Complete].Add("8037", "Research Cannon Galleon (requires Chemistry)"); names[NameType.Common].Add("8037", "Research Cannon Galleon");
                names[NameType.Complete].Add("400016", "Upgrade to Elite Unique Unit");
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
                names[NameType.Complete].Add("10878", "Black Forest");
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
                names[NameType.Complete].Add("10940", "African Clearing");
                names[NameType.Complete].Add("10948", "Acclivity");
                names[NameType.Complete].Add("10914", "Acropolis");
                names[NameType.Complete].Add("10959", "Aftermath");
                names[NameType.Complete].Add("10938", "Amazon Tunnel");
                names[NameType.Complete].Add("10876", "Archipelago");
                names[NameType.Complete].Add("10941", "Atacama");
                names[NameType.Complete].Add("10604", "Coastal");
                names[NameType.Complete].Add("10939", "Coastal Forest");
                names[NameType.Complete].Add("10607", "Continental");
                names[NameType.Complete].Add("10953", "Marketplace");
                names[NameType.Complete].Add("10892", "Mongolia");
                names[NameType.Complete].Add("10899", "Random Land Map");
                names[NameType.Complete].Add("10964", "Sacred Springs");
                names[NameType.Complete].Add("10893", "Salt Marsh");
                names[NameType.Complete].Add("1001", "Age of Empires II"); names[NameType.Common].Add("1001", "Age of Empires 2"); names[NameType.Acronym].Add("1001", "AOE2");
                names[NameType.Complete].Add("5067", "Arrow"); names[NameType.CommonPlural].Add("5067", "Arrows");
                names[NameType.Complete].Add("4305", "Health");
                names[NameType.Complete].Add("4306", "Armor");
                names[NameType.Complete].Add("6039", "Civilization"); names[NameType.Abbreviation].Add("6039", "Civ"); names[NameType.CommonPlural].Add("6039", "Civilizations"); names[NameType.AbbreviationPlural].Add("6039", "Civs");
                names[NameType.Complete].Add("4313", "Population"); names[NameType.Abbreviation].Add("4313", "Pop");
                names[NameType.Complete].Add("4314", "Range*");
                names[NameType.Complete].Add("4316", "Speed");
                names[NameType.Complete].Add("12201", "Line of Sight"); names[NameType.Acronym].Add("12201", "LoS");
                names[NameType.Complete].Add("19322", "Unique Unit"); names[NameType.Acronym].Add("19322", "UU"); names[NameType.CommonPlural].Add("19322", "Unique Units");
                names[NameType.Complete].Add("13073", "Idle");
                names[NameType.Complete].Add("13140", "Gather Point");
                names[NameType.Complete].Add("10026", "Artificial Intelligence"); names[NameType.Acronym].Add("10026", "AI");
                names[NameType.Complete].Add("20205", "Pierce Armor: "); names[NameType.Common].Add("20205", "Pierce Armor"); names[NameType.Acronym].Add("20205", "PA");
                names[NameType.Complete].Add("400012", "Long Distance");
                names[NameType.Complete].Add("400013", "Time");
                names[NameType.Complete].Add("7019", "Cartography");
                names[NameType.Complete].Add("7090", "Tracking");
                names[NameType.Complete].Add("400041", "Cursor");
                names[NameType.Complete].Add("400042", "Arrow Up");
                names[NameType.Complete].Add("400043", "->"); names[NameType.Common].Add("400043", "→");
                names[NameType.Complete].Add("98", "1");
                names[NameType.Complete].Add("22121", "2");
                names[NameType.Complete].Add("400044", "3");
                names[NameType.Complete].Add("400045", "4");
                names[NameType.Complete].Add("400046", "5");
                names[NameType.Complete].Add("400047", "6");
                names[NameType.Complete].Add("22126", "7");
                names[NameType.Complete].Add("22127", "8");
                names[NameType.Complete].Add("22128", "9");
                names[NameType.Complete].Add("99", "0");
                names[NameType.Complete].Add("9648", "-");
                names[NameType.Complete].Add("13055", "Explore");
                names[NameType.Complete].Add("13330", "Flag");
                names[NameType.Complete].Add("400048", "1s"); names[NameType.Common].Add("400048", " ");
                names[NameType.Complete].Add("400049", "2s"); names[NameType.Common].Add("400049", "  ");
                names[NameType.Complete].Add("400050", "3s"); names[NameType.Common].Add("400050", "   ");
                names[NameType.Complete].Add("400051", "4s"); names[NameType.Common].Add("400051", "    ");
                names[NameType.Complete].Add("400052", "5s"); names[NameType.Common].Add("400052", "     ");
                names[NameType.Complete].Add("400053", "6s"); names[NameType.Common].Add("400053", "      ");
                names[NameType.Complete].Add("400054", "7s"); names[NameType.Common].Add("400054", "       ");
                names[NameType.Complete].Add("400055", "8s"); names[NameType.Common].Add("400055", "        ");
                names[NameType.Complete].Add("400056", "9s"); names[NameType.Common].Add("400056", "         ");
                names[NameType.Complete].Add("400057", "10s"); names[NameType.Common].Add("400057", "          ");
                names[NameType.Complete].Add("400058", "Green Arabia");
                names[NameType.Complete].Add("400059", "Opponent Villager"); names[NameType.Abbreviation].Add("400059", "Opponent Vil|Opponent Vill"); names[NameType.Acronym].Add("400059", "Opponent V"); names[NameType.CommonPlural].Add("400059", "Opponent Villagers"); names[NameType.AbbreviationPlural].Add("400059", "Opponent Vils|Opponent Vills");
                names[NameType.Complete].Add("400060", "Less Than"); names[NameType.Common].Add("400060", "<");
                names[NameType.Complete].Add("400061", "Open Square Bracket"); names[NameType.Common].Add("400061", "[");
                names[NameType.Complete].Add("400062", "Close Square Bracket"); names[NameType.Common].Add("400062", "]");
                names[NameType.Complete].Add("400063", "Random Civilization");

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
                                        MostrarInformación($"{nombreCompletoSinTextoClave} already has an {textoClave} value in {tipo}. " + 
                                            $"Don't add {textoClave} values manually");

                                    } else {

                                        if (tipo == NameType.Acronym || tipo == NameType.AcronymPlural) {
                                            names[tipo].Add(códigoTC, conversiónAcrónimos(nombreSinTC));
                                        } else {
                                            names[tipo].Add(códigoTC, conversiónNormal(nombreSinTC));
                                        }

                                    }

                                } else if (tipo == NameType.Common && (textoClave == "Upgrade to" || textoClave == "Upgrade to Elite" 
                                    || textoClave == "Tech: Elite")) {

                                    names[tipo].Add(códigoTC, conversiónNormal(nombreCompletoSinTextoClave)); // Principalmente para formar los Upgrade to con el Upgrade al final para todos los textos.

                                } // Si no se tiene nombre común, abreviación o acrónimo, no se debe hacer nada.

                            }

                        } else {

                            var excepciones = new List<string> { "War Galley, Fire Ships and Demolition Ships" };
                            if (!excepciones.Contains(nombreCompletoSinTextoClave)) 
                                MostrarInformación($"{nombreCompletoSinTextoClave} doesn't have it's non {textoClave} counterpart.");

                        }

                    }

                    códigosCoincidentes = new List<string>(códigosTC);

                } // autogeneración>

                autogeneración("Elite", e, elite, new List<string>(), out _); // Autogeneración de los Elite.
                autogeneración("Upgrade to Elite", uE, upgradeElite, new List<string>(), out List<string> códigosUpgradeToElite); // Autogeneración de los Upgrade to Elite.
                autogeneración("Tech: Elite", uE, upgradeElite, new List<string>(), out List<string> códigosTechToElite); // Autogeneración de los Tech: Elite.                                                                                                       // 
                autogeneración("Upgrade to", u, upgrade, códigosUpgradeToElite, out _); // Autogeneración de los Upgrade.

            } // AOE2>

            File.WriteAllText(Preferencias.EnglishNamesPath, SerializarNombres(names));

        } // CrearArchivoNombresInglés>


        public static Entidad? ObtenerEntidad(string nombre) {

            if (!Nombres.ContainsKey(nombre)) return null;
            return Entidades[Nombres[nombre].ID];

        } // ObtenerEntidad>


        public static string? ObtenerRutaImagen(Entidad entidad, out string imagen) {

            imagen = entidad.ObtenerImagenEfectiva(Preferencias.RandomImageForMultipleImages);
            if (Imágenes.ContainsKey(imagen)) {
                return Imágenes[imagen];
            } else {
                return null;
            }
                
        } // ObtenerRutaImagen>


        /// <summary>
        /// Obtiene el texto o imagen que se mostrará para esta entidad. Usa las prioridades establecidas en DisplayPriority.
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        public static Segmento ObtenerSegmentoEfectivo(Entidad entidad, out string? errores) {

            errores = null;
            foreach (var prioridad in Preferencias.ObtenerDisplayPriorityOrdenadas()) {

                var tipoNombre = prioridad.Key;

                if (tipoNombre == NameType.Custom && entidad.Nombres.ContainsKey(NameType.Custom) && entidad.Nombres[NameType.Custom].StartsWith("[") &&
                    entidad.Nombres[NameType.Custom].Contains("]")) {

                    var tipoNombreTexto = entidad.Nombres[NameType.Custom].TrimEnd('*');
                    tipoNombreTexto = tipoNombreTexto[1..(tipoNombreTexto.IndexOf(']'))];
                    Enum.TryParse(typeof(NameType), tipoNombreTexto, ignoreCase: true, out object? resultado);
                    if (resultado != null) tipoNombre = (NameType)resultado;

                }

                if (tipoNombre == NameType.Image) {

                    var rutaImagen = ObtenerRutaImagen(entidad, out string imagen);
                    if (rutaImagen != null) {

                        var segmento = new Segmento(imagen, null, TipoSegmento.Imagen, null, out string? erroresInternos);
                        AgregarErrores(ref errores, erroresInternos);
                        return segmento;

                    }

                } else {

                    if (entidad.Nombres.ContainsKey(tipoNombre)) {

                        var textoSegmento = entidad.Nombres[tipoNombre];
                        var segmento = new Segmento(Preferencias.CapitalizeNames ? textoSegmento.ToUpper() : textoSegmento , null, TipoSegmento.Texto, null, 
                            out string? erroresInternos);
                        AgregarErrores(ref errores, erroresInternos);
                        return segmento;

                    }
                        
                }

            }

            AgregarErrores(ref errores, $"To Developer: ObtenerSegmentoEfectivo() didn't found any match for {entidad.NombreCompleto}.");
            var segmento2 = new Segmento("", null, TipoSegmento.Texto, null, out string? erroresInternos2);
            AgregarErrores(ref errores, erroresInternos2);
            return segmento2;

        } //  ObtenerSegmentoEfectivo>


        public static string ObtenerTextoNúmeroLocal(string textoNúmero) 
            => textoNúmero.Replace(".", SeparadorDecimales).Replace(",", SeparadorDecimales);


        public static MessageBoxResult MostrarError(string mensaje) => MessageBox.Show(mensaje, "Error");

        public static void AgregarErrores(ref string? errores, string? nuevosErrores) { // Solo se puede usar this ref para estructuras https://stackoverflow.com/questions/2618597/impossible-to-use-ref-and-out-for-first-this-parameter-in-extension-methods y https://stackoverflow.com/questions/46748334/extension-method-not-setting-value.
            if (!string.IsNullOrEmpty(nuevosErrores)) 
                errores += nuevosErrores + (nuevosErrores.EndsWith(Environment.NewLine) ? "" : Environment.NewLine);
        } // AgregarErrores>

        public static MessageBoxResult MostrarInformación(string mensaje) => MessageBox.Show(mensaje, "Info");


        public static string? ExtraerTextoDePantalla(ScreenCaptureText tipo, ParámetrosExtracción parámetros, out float confianza) {

            using var bmp = CapturaDePantalla.ObtenerBitmap(ObtenerRectánguloTextoEnPantalla(tipo), parámetros.Negativo, parámetros.BlancoYNegro,
                parámetros.Escala, parámetros.Luminosidad, parámetros.Contraste, parámetros.ModoInterpolación, parámetros.FormatoPixeles);
            var texto = OCR.ObtenerTexto(bmp, parámetros.SoloNúmeros, parámetros.PermitirUnCarácter, out confianza);

            if (Preferencias.OCRTestMode) {

                if (!Directory.Exists(DirectorioPruebasOCR)) Directory.CreateDirectory(DirectorioPruebasOCR);
                try {
                    bmp.Save(Path.Combine(DirectorioPruebasOCR, $"{LimpiarNombreArchivo(texto)}.bmp"), ImageFormat.Bmp);
                } catch (Exception) {
                    bmp.Save(Path.Combine(DirectorioPruebasOCR, $"{DateTime.Now.ToString("yyyyMMddhhmmss")}.bmp"), ImageFormat.Bmp);
                }
                
            }

            return texto;

        } // ExtraerTextoDePantalla>


        public static SDrw.Color ExtraerColorFondo(ScreenCaptureText tipo) {

            using var bmp = CapturaDePantalla.ObtenerBitmap(ObtenerRectánguloTextoEnPantalla(tipo), negativo: false, blancoYNegro: false, escala: 1, 
                luminosidad: 1, contraste: 1, InterpolationMode.Low, DImg.PixelFormat.Format32bppArgb);
            var coloresEsquinas = new List<SDrw.Color>();
            coloresEsquinas.Add(bmp.GetPixel(0, 0));
            coloresEsquinas.Add(bmp.GetPixel(0, bmp.Height - 1));
            coloresEsquinas.Add(bmp.GetPixel(bmp.Width - 1, bmp.Height - 1));
            coloresEsquinas.Add(bmp.GetPixel(bmp.Width - 1, 0));
            return ObtenerColorPromedio(coloresEsquinas);

        } // ExtraerColorPromedioEsquinas>


        public static string? LeerPausa(out float confianza) {

            var tipo = ScreenCaptureText.Age_of_Empires_II_PauseM;
            if (Preferencias.Game == Global.AOE2Name) {

                switch (Preferencias.GameLanguage) {

                    case "EN":
                    case "IT":
                    case "DE":
                    case "BR":
                        tipo = ScreenCaptureText.Age_of_Empires_II_PauseM;
                        break;
                    case "ES":
                    case "MX":
                    case "PL":
                    case "MS":
                    case "FR":
                    case "TR":
                        tipo = ScreenCaptureText.Age_of_Empires_II_PauseL;
                        break;
                    case "ZH":
                    case "TW":
                    case "JP":
                        tipo = ScreenCaptureText.Age_of_Empires_II_PauseF3XS;
                        break;
                    case "RU":
                        tipo = ScreenCaptureText.Age_of_Empires_II_PauseF3S;
                        break;
                    case "HI":
                        tipo = ScreenCaptureText.Age_of_Empires_II_PauseF3M;
                        break;
                    case "KO":
                        tipo = ScreenCaptureText.Age_of_Empires_II_PauseF3L;
                        break;
                    case "VI":
                        tipo = ScreenCaptureText.Age_of_Empires_II_PauseF3XL;
                        break;
                    default:
                        break;
                }

            }

            return ExtraerTextoDePantalla(tipo, new List<string>(), out confianza);

        } // LeerPausa>


        public static int? LeerProgreso(int progresoActual, out float confianza, int? rangoValoresEsperados = null) { // null se usa el valor predeterminado/recomendado para el juego. Por ejemplo para Age of Empires II, 3 es un valor aceptable para el rango de valores esperados cuando hay más de 30 aldeanos (cantidad de aldeanos en una build order de castillos rápida). Sería como si 3 centros de pueblo crearan aldeanos en el mismo segundo y aún así el RTS lo aceptara como correcto. Es improbable por el lado del juego que esto suceda, pero entre más se alargue el tiempo entre extracciones de progreso, más probable sería que suceda que se creen o se mueran 4 aldeanos entre estas verificaciones y el RTS no lo acepte cómo válidas.

            string? texto = null;
            confianza = 0;
            var valoresEsperados = new List<string>();

            if (rangoValoresEsperados == null) {

                if (Preferencias.Game == Global.AOE2Name) {
                    rangoValoresEsperados = progresoActual > 30 ? 3 : 2; // Para menos de 30 aldeanos no es posible que hayan 3 centros de pueblo produciendo aldeanos en condiciones de juego normales. Se prefiere acortar el rango de valores esperados para evitar errores. Aún si hubieran 3 centros de pueblo produciendo aldeanos, sería improbable que los 3 salieran en el mismo segundo, entonces el 2 tampoco funcionaría del todo mal en ese caso.
                } else {
                    rangoValoresEsperados = 3;
                }

            }

            for (int i = progresoActual - (int)rangoValoresEsperados; i <= progresoActual + rangoValoresEsperados; i++) {
                if (i >= 0) valoresEsperados.Add(i.ToString());
            }

            if (Preferencias.Game == Global.AOE2Name) {

                if (progresoActual < 9) {

                    texto = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9, new List<string>(), out confianza, 
                        extraConfianzaRequerida: 0.3f); // Se agrega más confianza requerida porque el 8 se puede confundir con el 3 en 1080. No se usa lista de valores esperados porque puede confundirse el 5 con el 3 que están muy cerca y se prefiere evitar este error.
                    if (confianza != -1) confianza+= 1; // Se suma 1 a la confianza para evitar que en el intervalo de 0 a 9 se consideren lecturas no confiables por no proveer la lista de valores esperados y se retrase el ajuste innecesariamente un paso más.

                } else if (progresoActual == 9 || progresoActual == 10) { 

                    var texto2n = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99, valoresEsperados, out float c2n);
                    var leído2n = false;
                    if (c2n > ConfianzaOCRAceptable) {

                        var éxito2 = int.TryParse(texto2n, out int progreso2);
                        if (progreso2 < 15) { // Podía estar en progreso 9 y aún tener el 8 en el juego, leyendo incorrectamente 78 por leerlo con el recorte de dos cifras, después con el 9 leía 79 y generaba un desface por haber leído dos valores consecutivos en los 70s. Para evitar este problema se limitó en el momento del procesamiento del paso a máximo cambioMáximo pasos, pero aquí también se evitará aceptar un progreso leído claramente incorrecto si este es mayor de 15.
                            leído2n = true;
                            texto = texto2n;
                            confianza = c2n;
                        }

                    } 
                    
                    if (!leído2n) {

                        var texto1n = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9, new List<string>(), out float c1n, 
                            extraConfianzaRequerida: 0.3f);
                        texto = texto1n;
                        confianza = c1n + (confianza == -1 ? 0 : 1); // Ver confianza += 1;

                    }
                    
                } else if (progresoActual > 10 && progresoActual < 99) {

                    var extraConfianzaRequerida = 0F;
                    if (progresoActual >= 74 && progresoActual <= 78 && (Preferencias.ScreenResolution == "2560x1440" 
                        || Preferencias.ScreenResolution == "3840x2160")) extraConfianzaRequerida = 0.25F; // Con el 76 el OCR se enloquece y devuelve 716 sin razón alguna. Esta extra confianza hace que no se acepte ese 716 y se obtenga una lectura correcta con otros parámetros.
                    if (progresoActual >= 90 && progresoActual <= 99 && Preferencias.ScreenResolution == "1920x1080") extraConfianzaRequerida = 0.2F; // Con la nueva fuente Smooth Serif de 2022 para la resolución 1920x1080 el intervalo de 90 a 99 es muy inexacto y puede producir números consecutivos incorrectos en 98 y 99 que pueden ser leídos como 8 y 9. Para evitar, este problema se sube la confianza para que el 98 que se lee como 8 con confianza 0,61, no se lea.
                    texto = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99, valoresEsperados, out confianza, 
                        extraConfianzaRequerida: extraConfianzaRequerida);
            
                } else if (progresoActual == 99 || progresoActual == 100) {

                    var texto3n = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999, valoresEsperados, out float c3n);
                    if (c3n > ConfianzaOCRAceptable) {

                        texto = texto3n;
                        confianza = c3n;

                    } else {

                        var texto2n = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99, valoresEsperados, out float c2n);
                        texto = texto2n;
                        confianza = c2n;

                    }

                } else if (progresoActual > 100) {

                    texto = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999, valoresEsperados, out confianza);

                }

            }

            var éxito = int.TryParse(texto, out int progreso);
            return éxito ? progreso : (int?)null;

        } // LeerProgreso>


        public static SDrw.RectangleF ObtenerRectánguloTextoEnPantalla(ScreenCaptureText tipo) {

            if (Preferencias.ScreenCaptureRectangles == null) CrearOCompletarScreenCaptureRectangles(cambióResolución: false);
            if (Preferencias.ScreenCaptureRectangles!.ContainsKey(tipo)) { // Después de CrearOCompletarScreenCaptureRectangles() se asegura que Preferencias.ScreenCaptureRectangles no es nulo.
               
                var r = Preferencias.ScreenCaptureRectangles![tipo];
                var e = RectángulosAfectadosPorEscalaInterface.Contains(tipo) ? Preferencias.GameInterfaceScale / 100 : 1;
                return new SDrw.RectangleF(r.X * e, r.Y * e, r.Width * e, r.Height * e);

            } else {
                return new SDrw.RectangleF(0, 0, 0.1f, 0.1f); // Un rectángulo cualquiera para que no saque error.
            }

        } // ObtenerRectánguloTextoEnPantalla>


        public static void CrearOCompletarScreenCaptureRectangles(bool cambióResolución) {

            var cambió = false;
            if (Preferencias.ScreenCaptureRectangles == null) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles = new Dictionary<ScreenCaptureText, SDrw.RectangleF>();    
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_InicioJuego)) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_InicioJuego,
                    new SDrw.RectangleF(2F / 2560, 1416F / 1440, 22F / 2560, 22F / 1440));
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9)) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9, 
                    new SDrw.RectangleF(582F / 2560, 49F / 1440, 13F / 2560, 17F / 1440)); // El algoritmo de OCR es inestable, cambia con el recorte realizado. Se debe procurar hacer siempre el mismo recorte. Si se cambia el rectángulo de recorte, se deben verificar la estabilidad del algoritmo con todos los números del rango aplicable.
            }

            if (cambióResolución || !Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99)) {

                cambió = true;
                var xSerif = 573F / 2560;
                var xSmoothSerif = 571F / 2560;
                var x = Preferencias.ScreenResolution == "1920x1080" ? xSmoothSerif : xSerif; // La resolución 1920x1080 fuerza la fuente Smooth Serif, entonces se requiere un recorte diferente.
                var rectángulo = new SDrw.RectangleF(x, 48F / 1440, 23F / 2560, 17F / 1440);

                if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99)) {
                    Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99, rectángulo);
                } else {

                    var xActual = Preferencias.ScreenCaptureRectangles[ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99].X;
                    if (xActual == xSerif || xActual == xSmoothSerif) 
                        Preferencias.ScreenCaptureRectangles[ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99] = rectángulo; // Evita cambiarlo para los usuarios que lo tengan personalizado. La manera más fácil de reconocer que no lo tienen personalizado es que el x sea uno de los predeterminados.

                }
                
            }          

            if (cambióResolución || !Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999)) {
                
                cambió = true;
                var xSerif = 565F / 2560;
                var xSmoothSerif = 563F / 2560;
                var x = Preferencias.ScreenResolution == "1920x1080" ? xSmoothSerif : xSerif; // La resolución 1920x1080 fuerza la fuente Smooth Serif, entonces se requiere un recorte diferente.
                var rectángulo = new SDrw.RectangleF(x, 49F / 1440, 30F / 2560, 17F / 1440);
                if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999)) {
                    Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999, rectángulo);
                } else {

                    var xActual = Preferencias.ScreenCaptureRectangles[ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999].X;
                    if (xActual == xSerif || xActual == xSmoothSerif)
                        Preferencias.ScreenCaptureRectangles[ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999] = rectángulo; // Evita cambiarlo para los usuarios que lo tengan personalizado. La manera más fácil de reconocer que no lo tienen personalizado es que el x sea uno de los predeterminados.

                }

            }
            
            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseM)) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_PauseM,
                    new SDrw.RectangleF(1113F / 2560, 675F / 1440, 335F / 2560, 91F / 1440));
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseL)) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_PauseL,
                    new SDrw.RectangleF(1084F / 2560, 672F / 1440, 393F / 2560, 95F / 1440));
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3XS)) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_PauseF3XS,
                    new SDrw.RectangleF(1310F / 2560, 682F / 1440, 79F / 2560, 71F / 1440));
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3S)) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_PauseF3S,
                    new SDrw.RectangleF(1295F / 2560, 682F / 1440, 79F / 2560, 71F / 1440));
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3M)) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_PauseF3M,
                    new SDrw.RectangleF(1322F / 2560, 682F / 1440, 79F / 2560, 71F / 1440));
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3L)) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_PauseF3L,
                    new SDrw.RectangleF(1351F / 2560, 682F / 1440, 79F / 2560, 71F / 1440));
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3XL)) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles.Add(ScreenCaptureText.Age_of_Empires_II_PauseF3XL,
                    new SDrw.RectangleF(1417F / 2560, 682F / 1440, 79F / 2560, 71F / 1440));
            }

            if (cambió) Settings.Guardar(Preferencias, RutaPreferencias);

        } // CrearOCompletarScreenCaptureRectangles>


        public static ParámetrosExtracción ObtenerParámetrosOCR(ScreenCaptureText tipo, int númeroRecomendado) {

            switch (tipo) {
                case ScreenCaptureText.Age_of_Empires_II_PauseM:
                case ScreenCaptureText.Age_of_Empires_II_PauseL:
                case ScreenCaptureText.Age_of_Empires_II_PauseF3XS:
                case ScreenCaptureText.Age_of_Empires_II_PauseF3S:
                case ScreenCaptureText.Age_of_Empires_II_PauseF3M:
                case ScreenCaptureText.Age_of_Empires_II_PauseF3L:
                case ScreenCaptureText.Age_of_Empires_II_PauseF3XL:
                    return AlfaNumNegByNL2C2; // Para Smooth Serif: AlfaNumNegByNL1_5C2. El cambio de fuente a Smooth Serif en 1920x1080 solo aplica para los números de recursos y la cantidad de aldeanos, no aplica para la fuente del texto de pausa.
                case ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9:

                    switch (númeroRecomendado) {
                        case 1:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                    return HQBCL1_5C2x16; // Smooth Serif.
                                case "1366x768":
                                    return NNL1C2x2; // Para Smooth Serif: HQBCL1_5C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return NNL2C2x2; // Para Smooth Serif: NNL2C2x2.
                            }

                        case 2:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                    return HQBCL2C2x16; // Smooth Serif.
                                case "1366x768":
                                    return HQBCL1C2x4; // Para Smooth Serif: HQBCL2C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return HQBCL2C2x4; // Para Smooth Serif: HQBCL2C2x4.
                            }

                        case 3:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                    return NNL1C2x1; // Smooth Serif.
                                case "1366x768":
                                    return NNL1C2x1; // Para Smooth Serif: NNL1C2x1.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return NNL2C2x1; // Para Smooth Serif: NNL2C2x1.
                            }
                            
                    }
                    break;

                case ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99:

                    switch (númeroRecomendado) {
                        case 1:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                    return HQBCL1_5C2x16; // Smooth Serif.
                                case "1366x768":
                                    return HQBCL1C2x16; // Para Smooth Serif: HQBCL1_5C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return HQBCL2C2x16; // Para Smooth Serif: HQBCL1_5C2x16. Extrañamente se requiere establecer la variable unCarácter en verdadero para que coincida adecuadamente números de 2 cifras. Parece ser un bug del algorítmo de Tesseract.
                            }

                        case 2:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                    return HQBCL2C2x16; // Smooth Serif.
                                case "1366x768":
                                    return HQBCL2C6x4; // Para Smooth Serif: HQBCL2C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return HQBCL4C6x4; // Para Smooth Serif: HQBCL4C6x4.
                            }

                        case 3:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                    return NNL1C2x1; // Smooth Serif.
                                case "1366x768":
                                    return NNL1C2x1; // Para Smooth Serif: NNL1C2x1.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return NNL2C2x1; // Para Smooth Serif: NNL2C2x1.
                            }
                            
                    }
                    break;

                case ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999:

                    switch (númeroRecomendado) {
                        case 1:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                    return HQBCL1_5C2x16; // Smooth Serif.
                                case "1366x768":
                                    return HQBCL2C6x4; // Para Smooth Serif: HQBCL1_5C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return HQBCL4C6x4; // Para Smooth Serif: HQBCL1_5C2x16.
                            }

                        case 2:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                    return HQBCL2C2x16; // Smooth Serif.
                                case "1366x768":
                                    return NNL1C2x1; // Para Smooth Serif: HQBCL2C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return NNL2C2x1; // Para Smooth Serif: HQBCL4C6x4.
                            }

                        case 3:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                    return NNL1C2x1; // Smooth Serif.
                                case "1366x768":
                                    return HQBCL1C2x16; // Para Smooth Serif: NNL1C2x1.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return HQBCL2C2x16; // Para Smooth Serif: NNL2C2x1.
                            }

                    }
                    break;

                case ScreenCaptureText.Age_of_Empires_II_Wood:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Food:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Gold:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Stone:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Population:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Maximum_Population:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Villagers_on_Wood:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Villagers_on_Food:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Villagers_on_Gold:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Villagers_on_Stone:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Timer:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Speed:
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Age:
                    break;
                default:
                    break;
            }

            return AlfanuméricoSinProcesamiento;

        } // ObtenerParámetrosOCR>


        /// <summary>
        /// Si el resultado de la extracción es uno de los valores esperados, se acepta una extracción con baja confianza.
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="máximosParámetrosExtracción"></param>
        /// <param name="valoresEsperados"></param>
        /// <param name="máximaConfianza"></param>
        /// <returns></returns>
        public static string? ExtraerTextoDePantalla(ScreenCaptureText tipo, List<string> valoresEsperados, out float máximaConfianza, 
            float extraConfianzaRequerida = 0) {

            var máximosIntentosDiferentesExtracción = 1;
            switch (tipo) {
                case ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9:
                case ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99:
                case ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999:
                    máximosIntentosDiferentesExtracción = 3;
                    break;
                default:
                    máximosIntentosDiferentesExtracción = 1;
                    break;
            }

            máximaConfianza = 0;

            for (int i = 1; i <= máximosIntentosDiferentesExtracción; i++) {
                
                var texto = ExtraerTextoDePantalla(tipo, ObtenerParámetrosOCR(tipo, i), out float confianza) ?? "";
                if (valoresEsperados.Contains(texto)) {

                    máximaConfianza = 1 + confianza;
                    return texto;

                } else if (confianza >= ConfianzaOCRAceptable + extraConfianzaRequerida) {

                    máximaConfianza = confianza;
                    return texto;

                } else if (i == máximosIntentosDiferentesExtracción) {

                    máximaConfianza = -1f;
                    return texto; // Igual se devuelve el texto encontrado, pero la función que lo llama debe verificar que la confianza no sea -1.

                }

            }

            return "";

        } // ExtraerTextoDePantalla>


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] static extern int GetWindowTextLength(IntPtr hWnd);

        private static string ObtenerNombreVentanaActual() {

            var título = string.Empty;
            var handle = GetForegroundWindow(); 
            var longitud = GetWindowTextLength(handle) + 1; 
            var stringBuilder = new StringBuilder(longitud);
            if (GetWindowText(handle, stringBuilder, longitud) > 0) título = stringBuilder.ToString();
            return título;

        } // ObtenerNombreVentanaActual>


        public static bool Jugando() => NombresVentanasJuegos.Contains(ObtenerNombreVentanaActual());


        public static bool EsFavorita(string juego, string nombreÓrdenDeEjecución) => Preferencias.FavoriteBuildOrders.ContainsKey(Preferencias.Game) 
            && Preferencias.FavoriteBuildOrders[juego].Contains(nombreÓrdenDeEjecución.ToLower());


        public static void AgregarAFavoritos(string juego, string nombreÓrdenDeEjecución) {

            if (!EsFavorita(juego, nombreÓrdenDeEjecución)) { 
                if (!Preferencias.FavoriteBuildOrders.ContainsKey(juego)) Preferencias.FavoriteBuildOrders.Add(juego, new List<string>());
                Preferencias.FavoriteBuildOrders[juego].Add(nombreÓrdenDeEjecución.ToLower()); 
            }
            Settings.Guardar(Preferencias, RutaPreferencias);

        } // AgregarAFavoritos>


        public static void EliminarDeFavoritos(string juego, string nombreÓrdenDeEjecución) {

            if (EsFavorita(juego, nombreÓrdenDeEjecución)) Preferencias.FavoriteBuildOrders[juego].Remove(nombreÓrdenDeEjecución.ToLower());
            Settings.Guardar(Preferencias, RutaPreferencias);

        } // EliminarDeFavoritos>


        public static bool RequiereAgregarÓrdenDeEjecución(string juego, string nombreÓrdenDeEjecución, out bool juegoSinFavoritas) {

            juegoSinFavoritas = !Preferencias.FavoriteBuildOrders.ContainsKey(juego) || Preferencias.FavoriteBuildOrders[juego].Count == 0;
            return juegoSinFavoritas || !Preferencias.ShowOnlyFavoriteBuildOrders || EsFavorita(juego, nombreÓrdenDeEjecución);

        } // RequiereAgregarÓrdenDeEjecución>


        #endregion Procedimientos y Funciones>


    } // Global>



} // RTSHelper>
