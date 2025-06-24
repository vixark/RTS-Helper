using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Interop;
using WForms = System.Windows.Forms;
using System.IO;
using System.Text.Json;
using Vixark;
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
using Controles = System.Windows.Controls;



namespace RTSHelper {



    public static class Global {


        #region Variables

        public static Settings Preferencias = new Settings();

        public static bool ModoDesarrollo = true;

        public const string AOE2Name = "Age of Empires II";

        public const string AOE4Name = "Age of Empires IV";

        public const string AOMName = "Age of Mythology";

        public const double AOMNormalSpeed = 1 * 1.0018785 * 1.0008; // 1200 segundos en juego dieron 19:57.75 en RTS Helper. El juego estaba más rápido por 1200/1197,75 = 1,00187852222918.

        public static List<string> NombresVentanasJuegos = new List<string>() { "Age of Empires II: Definitive Edition", "Age of Empires IV", 
            "Age of Empires II: HD Edition", "Age of Mythology: Retold" };

        public static Dictionary<string, List<string>> TextosPausa = new Dictionary<string, List<string>> { { AOE2Name,  new List<string> 
            { "Game Paused", "Jogo Pausado", "Spiel pausiert", "Partida en pausa", "Partie mise en pause", "Partita sospesa",
            "Permainan Dijeda", "Partida en pausa", "Gra wstrzymana", "Oyun Duraklatildi", "Van choi bi tam dung", "(F3)" } }, // No se agregan "गेम रोका गया",  "一時停止", "게임 일시 중지", "遊戲暫停", "Пауза" "游戏暂停" porque Tesseract no reconoce estos carácteres. "Oyun Duraklatıldı", "Ván chơi bị tạm dừng" se llevan a alfabeto latino que es el reconocido por Tesseract. Para AOE2 no se incluye el paréntesis y la tecla usada para pausar (F3) para no incluirla en las verificaciones iniciales del texto con Contains.
            { AOMName, new List<string> { "Game Paused", "Hra pozastavena", "Spillet er sat på pause", "Spel gepauzeerd", "Peli keskeytetty", 
                "Partie en pause", "Spiel unterbrochen", "A játék szünetel", "Partita sospesa", "Permainan Dijeda", "Spillet er satt på pause", 
                "Gra wstrzymana", "Jogo pausado", "Partida en pausa", "Spelet är pausat", "Oyun duraklatıldı",  } } // No se agregan "Παιχνίδι σε παύση", "खेल रोका गया", "ポーズ中", "게임 일시정지", "Игра приостановлена", "游戏已暂停", "เกมหยุดชั่วคราว", "遊戲暫停", "Ván chơi tạm dừng" porque Tesseract no reconoce estos carácteres.
        }; 

        public static string OtherName = "Other";

        public static List<string> Juegos = new List<string>() { AOE2Name, AOMName, AOE4Name, OtherName }; // Al cambiar o agregar un elemento aquí también se debe agregar en SettingsWindows.xaml.

        public static string DirectorioAOE2 = @"D:\Juegos\Steam\steamapps\common\AoE2DE";

        public static string DirectorioAplicaciónReal = @"D:\Programas\RTS Helper";

        public static string DirectorioCompilación = AppDomain.CurrentDomain.BaseDirectory ?? @"C:\"; // No veo en que situación podría ser null BaseDirectory.

        public static string DirectorioAplicación = ModoDesarrollo ? DirectorioAplicaciónReal : DirectorioCompilación; 

        public static string RutaPreferencias = Path.Combine(DirectorioAplicación, "Settings.json");

        public static string NoneSoundString = "None";

        public static string NombreFuentePredeterminada = "Tahoma";

        public static string NombreTimesNewRoman = "Times New Roman";

        public static string NombreFuenteSecundariaPredeterminada = NombreTimesNewRoman;

        public static double FactorTamañoTextoAPixelesFuentePredeterminada = 136D / 113; // Es un factor experimental para la fuente actual predeterminada (Tahoma) que permite convertir el tamaño de la fuente al tamaño de la imagen para que ambos sean del mismo alto. Se hace para la fuente predeterminada aunque no debería ser muy diferente con otras fuentes.

        public static string DirectorioSonidosCortos = Path.Combine(DirectorioAplicación, "Sounds", "Short");

        public static string DirectorioSonidosLargos = Path.Combine(DirectorioAplicación, "Sounds", "Long");

        public static string DirectorioNombres = Path.Combine(DirectorioAplicación, "Names");

        public static string DirectorioTemporal = Path.Combine(DirectorioAplicación, "Temp");

        public static string DirectorioEstrategiasCompilación = Path.Combine(DirectorioCompilación, "Build Orders");

        public static string DirectorioEstrategiasCódigo = @"D:\Programas\RTS Helper\Código\RTS Helper\RTS Helper\Build Orders";

        public static string DirectorioEstrategias = DirectorioEstrategiasCompilación; // Tanto en desarrollo como en producción es la misma carpeta porque las estrategias se almacenan en el repositorio y se copian al directorio en la compilación.

        public static string DirectorioImágenes = Path.Combine(DirectorioAplicación, "Images");

        public static string DirectorioPruebasOCR = Path.Combine(DirectorioAplicación, "OCR Tests");

        public static string AlinearInferiormenteId = "\f";

        public static string NuevaLíneaId = "\n";

        public static double CorrecciónEscala = 1.25 / (WForms.Screen.PrimaryScreen!.Bounds.Width / SystemParameters.PrimaryScreenWidth); // Todos los valores son calculados experimentalmente en mi computador que tiene una escala de 125, entonces para ser usado en computadores de otra escala se debe ajustar todos los valores con este factor.

        public static string SeparadorDecimales = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public static InformaciónÚltimasVersiones? InformaciónÚltimasVersiones;
        
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

        public enum TamañosFuente { Indeterminado, XXXSMINUS, XXXS, XXXSPLUS, XXSMINUS, XXS, XXSPLUS, XSMINUS, XS, XSPLUS, SMINUS, S, SPLUS, MMINUS, M,
            MPLUS, LMINUS, L, LPLUS, XLMINUS, XL, XLPLUS, XXLMINUS, XXL, XXLPLUS, XXXLMINUS, XXXL, XXXLPLUS }

        public enum TipoFuente { Sans, SansNegrita, Serif, SerifCuadrada, Caligráfica, Símbolos }

        public enum FuenteEstrategia { Archivo, Memoria }

        public enum TipoArchivoActualización { Completo, ActualizaciónBinarios, Imágenes, Sonidos, Estrategias, InformaciónÚltimasVersiones, 
            CambiosImágenes, CambiosSonidos, CambiosEstrategias }

        public static Dictionary<string, Nombre> Nombres = new Dictionary<string, Nombre>(); // Los nombres no repetidos. La utilidad de este diccionario es principalmente para identificar nombres entre [] y reemplazarlo por la entidad correspondiente. La primera vez que aparezca un nombre sin importar en que idioma esté se agregará a este diccionario con ese idioma, los demás se ignoran. La clave son todos los nombres posibles.

        public static List<(string, Nombre)> TodosLosNombres = new List<(string, Nombre)>(); // Todos los nombres. Su uso es obtener el valor de cierta entidad en cierto tipo/idioma. La clave son todos los nombres posibles.

        public static Dictionary<string, Entidad> Entidades = new Dictionary<string, Entidad>(); // La clave es el ID de la entidad.

        public static Dictionary<string, string> Imágenes = new Dictionary<string, string>(); // La clave es el nombre único de la imagen (nombre del archivo) y el valor es la ruta. No se deben repetir nombres de imágenes, incluso si están en diferentes carpetas.

        public static List<string> Estilos { get; set; } = new List<string> { "b", "i", "u", "nb" };

        public static List<string> Tamaños { get; set; } = new List<string> { "xxxsminus", "xxxs", "xxxsplus", "xxsminus", "xxs", "xxsplus", 
            "xsminus", "xs", "xsplus", "sminus", "s", "splus", "mminus", "m", "mplus", "lminus", "l", "lplus", "xlminus", "xl", "xlplus", "xxlminus", "xxl",
            "xxlplus", "xxxlminus", "xxxl", "xxxlplus" }; // xxxl x3, xxl x2, xl x1.5, l x1.3, m x1, s x1/1,3, xs x1/1,5, xxs x1/2, xxxs x1/3. 

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

        public static double OpacityPredeterminado = 0.85; // El valor anterior 0.8 es un poco muy transparente. 0.85 es un buen equilibrio. Igual el usuario lo puede personalizar después.

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

        public static ParámetrosExtracción AlfaNumByNL1C2 = new ParámetrosExtracción(soloNúmeros: false,
            permitirUnCarácter: false, negativo: false, blancoYNegro: true, escala: 1, luminosidad: 1, contraste: 2, InterpolationMode.NearestNeighbor,
            DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción AlfaNumNegByNL1_5C2 = new ParámetrosExtracción(soloNúmeros: false,
            permitirUnCarácter: false, negativo: true, blancoYNegro: true, escala: 1, luminosidad: 1.5f, contraste: 2, InterpolationMode.NearestNeighbor,
            DImg.PixelFormat.Format8bppIndexed);

        public static ParámetrosExtracción AlfaNumNegByNL2C4 = new ParámetrosExtracción(soloNúmeros: false,
            permitirUnCarácter: false, negativo: true, blancoYNegro: true, escala: 1, luminosidad: 2f, contraste: 4, InterpolationMode.NearestNeighbor,
            DImg.PixelFormat.Format8bppIndexed);

        public enum ScreenCaptureText : int { // Por facilidad y flexibilidad se agregan todos los posibles textos en la misma enumeración. El prefijo es el nombre del juego usando _ en vez de espacios para poder filtrarlo fácilmente en opciones.
            Age_of_Empires_II_PauseF3XS, Age_of_Empires_II_PauseF3S, Age_of_Empires_II_PauseF3M, Age_of_Empires_II_PauseF3L, Age_of_Empires_II_PauseF3XL,
            Age_of_Empires_II_PauseM, Age_of_Empires_II_PauseL,
            Age_of_Empires_II_Villagers_0_to_9, Age_of_Empires_II_Villagers_10_to_99, Age_of_Empires_II_Villagers_100_to_999,
            Age_of_Empires_II_Wood, Age_of_Empires_II_Food, Age_of_Empires_II_Gold, Age_of_Empires_II_Stone, Age_of_Empires_II_Population,
            Age_of_Empires_II_Maximum_Population, Age_of_Empires_II_Villagers_on_Wood, Age_of_Empires_II_Villagers_on_Food, 
            Age_of_Empires_II_Villagers_on_Gold, Age_of_Empires_II_Villagers_on_Stone, Age_of_Empires_II_Timer, Age_of_Empires_II_Speed, 
            Age_of_Empires_II_Age, Age_of_Empires_II_Game_Start,
            Age_of_Mythology_Pause, Age_of_Mythology_Villagers_0_to_9, Age_of_Mythology_Villagers_10_to_99, Age_of_Mythology_Game_Start
        }

        public enum UIMod : int { // Nunca cambiar el nombre, se almacenan como texto en preferencias. Al agregarlo aquí también se debe agregar a UIMods y a CrearOCompletarScreenCaptureRectangles(). No se usará el nombre del juego en el nombre del mod de manera predeterminada porque es improbable que se repita en otros juegos. De todas maneras si se llegara a repetir, no hay problema porque los valores se manejan diferentes según el juego.
            No_Mod, Anne_HK_Better_Resource_Panel_and_Idle_Villager_Icon, Anne_HK_Better_Resource_Panel_TheViper_Version, 
            KoBHV_Brand_New_Resource_Panel_with_Annoying_Idle_Villager_Button, KoBHV_Brand_New_Resource_Panel_Standard_Version,
            Anne_HK_Better_Resource_Panel_Top_Center_Version, Streamlined_UI, PointiBoi_Minimalistic_UI, AllYourBase_Maximum_Advantage_UI,
            Custom_UI_Centered_Modern_Black_and_White, XavilUI, Villese_UI, Bottom_Side_UI, Resource_Bar_at_Bottom, Bottom_Resource_Panel
        } // Mods probados que no cambian la posición de los elementos: Transparent UI, Transparent Default UI, 2331_middle mini-map and lower score panel for elders, Transparent UI - Centered Bigger Minimap - Raised Control Groups - Slightly Raised Score Panel, 14752_Custom UI - Opaque Anthrazite.
        
        public static List<ScreenCaptureText> RectángulosSuperioresIzquierdosAfectadosPorEscalaUI = new List<ScreenCaptureText> {
            ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9, ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99,
            ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999, ScreenCaptureText.Age_of_Empires_II_Wood, ScreenCaptureText.Age_of_Empires_II_Food,
            ScreenCaptureText.Age_of_Empires_II_Gold, ScreenCaptureText.Age_of_Empires_II_Stone, ScreenCaptureText.Age_of_Empires_II_Population,
            ScreenCaptureText.Age_of_Empires_II_Maximum_Population, ScreenCaptureText.Age_of_Empires_II_Villagers_on_Wood,
            ScreenCaptureText.Age_of_Empires_II_Villagers_on_Food, ScreenCaptureText.Age_of_Empires_II_Villagers_on_Gold,
            ScreenCaptureText.Age_of_Empires_II_Villagers_on_Stone, ScreenCaptureText.Age_of_Empires_II_Timer, ScreenCaptureText.Age_of_Empires_II_Speed,
            ScreenCaptureText.Age_of_Empires_II_Age};

        public static Dictionary<ScreenCaptureText, float> RectángulosSuperioresADerechaDePanelCentradoPorEscalaUI = new Dictionary<ScreenCaptureText, float>() {
            {  ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9, 0.4195f }, { ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99, 0.415f } }; //  El valor es el ancho de un panel central en el que los rectangulos RectángulosSuperioresADerechaDePanelCentradoPorEscalaUI están ubicados a la derecha de este. Se mide iniciando más o menos donde iniciarían los rectángulos de RectángulosSuperioresADerechaDePanelCentradoPorEscalaUI y se termina de ajustar su valor para que se pueda calcular correctamente la nueva X con la escala más pequeña. XRectEscalaMásPequeña = (X100% - ((AnchoFraccionalPanelCentradoAOM * (1 - %Escala))/2))

        public static List<ScreenCaptureText> RectángulosCentradosAfectadosPorEscalaUI = new List<ScreenCaptureText> {
            ScreenCaptureText.Age_of_Empires_II_PauseF3XS, ScreenCaptureText.Age_of_Empires_II_PauseF3S, ScreenCaptureText.Age_of_Empires_II_PauseF3M, 
            ScreenCaptureText.Age_of_Empires_II_PauseF3L, ScreenCaptureText.Age_of_Empires_II_PauseF3XL,
            ScreenCaptureText.Age_of_Empires_II_PauseM, ScreenCaptureText.Age_of_Empires_II_PauseL
        };

        public static List<ScreenCaptureText> RectángulosCentradosHorizontalmenteAfectadosPorEscalaUI = new List<ScreenCaptureText> {
            ScreenCaptureText.Age_of_Mythology_Game_Start
        };

        public static List<ScreenCaptureText> RectángulosActivos = new List<ScreenCaptureText> { ScreenCaptureText.Age_of_Empires_II_PauseF3XS,
            ScreenCaptureText.Age_of_Empires_II_PauseF3S, ScreenCaptureText.Age_of_Empires_II_PauseF3M, ScreenCaptureText.Age_of_Empires_II_PauseF3L,
            ScreenCaptureText.Age_of_Empires_II_PauseF3XL, ScreenCaptureText.Age_of_Empires_II_PauseM, ScreenCaptureText.Age_of_Empires_II_PauseL,
            ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9, ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99, 
            ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999, ScreenCaptureText.Age_of_Empires_II_Game_Start, 
            ScreenCaptureText.Age_of_Mythology_Pause, ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9, 
            ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99, ScreenCaptureText.Age_of_Mythology_Game_Start }; // Los rectángulos que actualmente se están usando en el programa y deben mostrarse en los la sección de preferencias para ser poder ser personalizados por el usuario.

        public static Dictionary<string, List<UIMod>> UIMods = new Dictionary<string, List<UIMod>> { { AOE2Name, new List<UIMod> { UIMod.No_Mod,
            UIMod.Anne_HK_Better_Resource_Panel_and_Idle_Villager_Icon, UIMod.Anne_HK_Better_Resource_Panel_TheViper_Version,
            UIMod.KoBHV_Brand_New_Resource_Panel_with_Annoying_Idle_Villager_Button, UIMod.KoBHV_Brand_New_Resource_Panel_Standard_Version,
            UIMod.Anne_HK_Better_Resource_Panel_Top_Center_Version, UIMod.Streamlined_UI, UIMod.PointiBoi_Minimalistic_UI,
            UIMod.AllYourBase_Maximum_Advantage_UI, UIMod.Custom_UI_Centered_Modern_Black_and_White, UIMod.XavilUI, UIMod.Villese_UI,
            UIMod.Bottom_Side_UI, UIMod.Resource_Bar_at_Bottom, UIMod.Bottom_Resource_Panel} } };

        public static float ConfianzaOCRAceptable = 0.50f;

        public static bool MostrandoPreferenciasOCR = false;

        public static Dictionary<ScreenCaptureText, Controles.Image> RectángulosImágenesPrueba = new Dictionary<ScreenCaptureText, Image>();

        public static string EnlaceDonación = "http://vixark.com/donate";

        public static List<string> ÓrdenesDeEjecuciónAEliminar = new List<string> { "Malian Men-at-Arms and Towers by Vixark", "Constant Scouts into Knights by Vixark" }; // A estas estrategias se les cambió el nombre por otro, y el archivo con este nombre fue sobreescrito con un TXT vacío y se quiere que sea eliminado al iniciar.

        public static List<char> CarácteresComoImágenes = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '+', '=', '%', '>', '<', '&', '$', '#', '@', '*', '_', '^', '!', '¡', '?', '¿', '\\', '/', '|' }; // Algunos carácteres se pueden también escribir como imágenes si se quiere generar una personalización especial de ese carácter. Estas imágenes de carácteres no tendrán el alto de las imágenes sino de la fuente.

        #endregion Variables>


        #region Funciones y Procedimientos

        public static string HtmlDonación(int tamañoFuente) 
            => @$"<div style='font-size: {tamañoFuente}px;'>If you like this application, please consider to <a href='{EnlaceDonación}'>donate</a>.</div>";


        public static string? ObtenerURLArchivo(TipoArchivoActualización tipo, int? versiónActual = null, int? últimaVersión = null, 
            string? versiónActualPrograma = null) {

            var baseUrl = InformaciónÚltimasVersiones?.BaseUrl ?? Preferencias.UpdatesBaseUrl;

            switch (tipo) {
                case TipoArchivoActualización.Completo:
                    return $"{baseUrl}/RTS Helper.zip";
                case TipoArchivoActualización.ActualizaciónBinarios:
                    return $"{baseUrl}/update-{versiónActualPrograma?.TrimEnd('.')}.zip";
                case TipoArchivoActualización.Imágenes:
                    return $"{baseUrl}/images-{versiónActual}-to-{últimaVersión}.zip";
                case TipoArchivoActualización.Sonidos:
                    return $"{baseUrl}/sounds-{versiónActual}-to-{últimaVersión}.zip";
                case TipoArchivoActualización.Estrategias:
                    return $"{baseUrl}/build-orders-{últimaVersión}.zip";
                case TipoArchivoActualización.InformaciónÚltimasVersiones:
                    return $"{baseUrl}/last-versions-info.json";
                case TipoArchivoActualización.CambiosImágenes:
                    return $"{baseUrl}/images-changes-{versiónActual}-to-{últimaVersión}.html";
                case TipoArchivoActualización.CambiosSonidos:
                    return $"{baseUrl}/sounds-changes-{versiónActual}-to-{últimaVersión}.html";
                case TipoArchivoActualización.CambiosEstrategias:
                    return $"{baseUrl}/build-orders-changes-{versiónActual}-to-{últimaVersión}.html";
                default:
                    return null;
            }
            
        } // ObtenerURLArchivo>


        public static double? ObtenerFactorTamañoFuente(TamañosFuente tamañoFuente) => tamañoFuente switch {
            TamañosFuente.XXXLPLUS => 2.3D,
            TamañosFuente.XXXL => 2D,
            TamañosFuente.XXXLMINUS => 1.85D,
            TamañosFuente.XXLPLUS => 1.65D,
            TamañosFuente.XXL => 1.5D,
            TamañosFuente.XXLMINUS => 1.44D,
            TamañosFuente.XLPLUS => 1.36D,
            TamañosFuente.XL => 1.3D,
            TamañosFuente.XLMINUS => 1.25D,
            TamañosFuente.LPLUS => 1.2D,
            TamañosFuente.L => 1.15D,
            TamañosFuente.LMINUS => 1.1D,
            TamañosFuente.MPLUS => 1.05D,
            TamañosFuente.M => 1,
            TamañosFuente.MMINUS => (1D / 1.06),
            TamañosFuente.SPLUS => (1D / 1.14),
            TamañosFuente.S => (1D / 1.2),
            TamañosFuente.SMINUS => (1D / 1.3),
            TamañosFuente.XSPLUS => (1D / 1.4),
            TamañosFuente.XS => (1D / 1.5),
            TamañosFuente.XSMINUS => (1D / 1.66),
            TamañosFuente.XXSPLUS => (1D / 1.84),
            TamañosFuente.XXS => (1D / 2),
            TamañosFuente.XXSMINUS => (1D / 2.33),
            TamañosFuente.XXXSPLUS => (1D / 2.66),
            TamañosFuente.XXXS => (1D / 3),
            TamañosFuente.XXXSMINUS => (1D / 3.3),
            TamañosFuente.Indeterminado => null
        };


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
            } else if(anchoPantalla == 1680 && altoPantalla == 1050) {
                resolución = "1680x1050";
            } else if(anchoPantalla == 1920 && altoPantalla == 1200) {
                resolución = "1920x1200";
            } else if (anchoPantalla == 2560 && altoPantalla == 1440) {
                resolución = "2560x1440";
            } else if (anchoPantalla == 1366 && altoPantalla == 768) {
                resolución = "1366x768";
            } else if (anchoPantalla == 1280 && altoPantalla == 800) {
                resolución = "1280x800";
            } else if (anchoPantalla == 1280 && altoPantalla == 720) {
                resolución = "1280x720";
            } else if (anchoPantalla == 3840 && altoPantalla == 2160) {
                resolución = "3840x2160";
            } else if (anchoPantalla == 1600 && altoPantalla == 900) {
                resolución = "1600x900";
            } else if (anchoPantalla == 1440 && altoPantalla == 900) {
                resolución = "1440x900";
            } else if (anchoPantalla == 1360 && altoPantalla == 768) {
                resolución = "1360x768";
            } else if (altoPantalla >= 2160) {
                resolución = "3840x2160";
            } else if (altoPantalla >= 1440) {
                resolución = "2560x1440";
            } else if (altoPantalla >= 1080) {
                resolución = "1920x1080";
            } else if (altoPantalla >= 900) {
                resolución = "1600x900";
            } else if (altoPantalla >= 768) {
                resolución = "1366x768";
            } else {
                resolución = "1280x720";                 
            }
            return resolución;

        } // ObtenerResoluciónRecomendada>


        public static int ObtenerDuraciónEndStepSound(string soundName) 
            => (int)Math.Round(1000 * MediaPlayer.GetDuration(Path.Combine(DirectorioSonidosLargos, soundName)), 0);


        public static Dictionary<NameType, Dictionary<string, string>> DeserializarNombres(string ruta)
            => JsonSerializer.Deserialize<Dictionary<NameType, Dictionary<string, string>>>(File.ReadAllText(ruta),
                ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración | Serialización.EnumeraciónEnTexto | Serialización.UTF8))!;


        public static Dictionary<string, Dictionary<string, string>> DeserializarTipos(string ruta)
            => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(ruta),
                ObtenerOpcionesSerialización(Serialización.DiccionarioClaveEnumeración | Serialización.EnumeraciónEnTexto | Serialización.UTF8))!;


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
                    MostrarError($"Type can't be null for {nombreCompleto} ID = {kv.Key}. Entity wasn't created for {nombreCompleto}.");
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
                                    if (!Entidades.ContainsKey(idAnterior)) {
                                        MostrarError($"Entity not found for {idAnterior}"); // No debería suceder. Se ignora.
                                    } else {

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
                types["Type"].Add("Fish (Action)", "Action");
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
                types["Type"].Add("Straggler Tree", "Resource");
                types["Type"].Add("Rhinoceros", "Resource");
                types["Type"].Add("Box Turtles", "Resource");
                types["Type"].Add("Water Buffalo", "Resource");
                types["Type"].Add("Elephant", "Resource");
                types["Type"].Add("Fruit Bush", "Resource");
                types["Type"].Add("Stone Mine", "Resource");
                types["Type"].Add("Gold Mine", "Resource");
                types["Type"].Add("Food to Gold", "Resource");
                types["Type"].Add("Food to Stone", "Resource");
                types["Type"].Add("Food to Wood", "Resource");
                types["Type"].Add("Gold to Food", "Resource");
                types["Type"].Add("Gold to Stone", "Resource");
                types["Type"].Add("Gold to Wood", "Resource");
                types["Type"].Add("Stone to Food", "Resource");
                types["Type"].Add("Stone to Gold", "Resource");
                types["Type"].Add("Stone to Wood", "Resource");
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
                types["Type"].Add("Explore", "Action");
                types["Type"].Add("Flag", "Other");
                types["Type"].Add("0s", "Other");
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
                types["Type"].Add("Go Back to Work", "Action");
                types["Type"].Add("Woodline", "Resource");
                types["Type"].Add("Chicken", "Resource");
                types["Type"].Add("Drop Off Resources", "Action");

            } else if (Preferencias.Game == AOMName) {

                types["Type"].Add("Attack Move", "Actions");
                types["Type"].Add("Back To Work", "Actions");
                types["Type"].Add("Build Dock", "Actions");
                types["Type"].Add("Build Farm", "Actions");
                types["Type"].Add("Build Granary", "Actions");
                types["Type"].Add("Build Lumber Camp", "Actions");
                types["Type"].Add("Build Mining Camp", "Actions");
                types["Type"].Add("Build Storehouse", "Actions");
                types["Type"].Add("Delete", "Actions");
                types["Type"].Add("Eco Nearby Gather Point", "Actions");
                types["Type"].Add("Eject Back To Work", "Actions");
                types["Type"].Add("Flare", "Actions");
                types["Type"].Add("Flare Selection", "Actions");
                types["Type"].Add("Garrison", "Actions");
                types["Type"].Add("Gather Point", "Actions");
                types["Type"].Add("Guard", "Actions");
                types["Type"].Add("Livestock Return", "Actions");
                types["Type"].Add("Mil Nearby Gather Point", "Actions");
                types["Type"].Add("Patrol", "Actions");
                types["Type"].Add("Repair", "Actions");
                types["Type"].Add("Return Resources", "Actions");
                types["Type"].Add("Seek Shelter", "Actions");
                types["Type"].Add("Send Fishing Ship", "Actions");
                types["Type"].Add("Send Fishing Ship To Build", "Actions");
                types["Type"].Add("Send Heroes", "Actions");
                types["Type"].Add("Send Military", "Actions");
                types["Type"].Add("Send Navy", "Actions");
                types["Type"].Add("Send Priest", "Actions");
                types["Type"].Add("Send Priest To Build", "Actions");
                types["Type"].Add("Send Siege", "Actions");
                types["Type"].Add("Send Transport Ship", "Actions");
                types["Type"].Add("Send Villagers", "Actions");
                types["Type"].Add("Send Villagers To Build", "Actions");
                types["Type"].Add("Stop", "Actions");
                types["Type"].Add("Town Bell", "Actions");
                types["Type"].Add("Ungarrison", "Actions");
                types["Type"].Add("Attac Building", "Actions");
                types["Type"].Add("Attack", "Actions");
                types["Type"].Add("Attack Forbidden", "Actions");
                types["Type"].Add("Attack Move Cursor", "Actions");
                types["Type"].Add("Auto Assign On Dropsite", "Actions");
                types["Type"].Add("Board", "Actions");
                types["Type"].Add("Build", "Actions");
                types["Type"].Add("Favor", "Actions");
                types["Type"].Add("Fish Cursor", "Actions");
                types["Type"].Add("Food Aggro Huntable", "Actions");
                types["Type"].Add("Food Berryfarm", "Actions");
                types["Type"].Add("Food Herdable", "Actions");
                types["Type"].Add("Food Huntable", "Actions");
                types["Type"].Add("Garrison Cursor", "Actions");
                types["Type"].Add("Godpower", "Actions");
                types["Type"].Add("Godpower Forbidden", "Actions");
                types["Type"].Add("Gold", "Actions");
                types["Type"].Add("Gold Trade", "Actions");
                types["Type"].Add("Heal", "Actions");
                types["Type"].Add("Herdable", "Actions");
                types["Type"].Add("Priest Empower", "Actions");
                types["Type"].Add("Rally", "Actions");
                types["Type"].Add("Relic Dropoff", "Actions");
                types["Type"].Add("Relic Pickup", "Actions");
                types["Type"].Add("Repair Cursor", "Actions");
                types["Type"].Add("Res Drop All", "Actions");
                types["Type"].Add("Res Drop Food", "Actions");
                types["Type"].Add("Res Drop Gold", "Actions");
                types["Type"].Add("Res Drop Other", "Actions");
                types["Type"].Add("Res Drop Wood", "Actions");
                types["Type"].Add("Standard", "Actions");
                types["Type"].Add("Unboard", "Actions");
                types["Type"].Add("Walk", "Actions");
                types["Type"].Add("Wood Gathering", "Actions");
                types["Type"].Add("Market Buy Food", "Actions");
                types["Type"].Add("Market Buy Wood", "Actions");
                types["Type"].Add("Market Sell Food", "Actions");
                types["Type"].Add("Market Sell Wood", "Actions");
                types["Type"].Add("Mode Box", "Actions");
                types["Type"].Add("Mode Box On", "Actions");
                types["Type"].Add("Mode Line", "Actions");
                types["Type"].Add("Mode Line On", "Actions");
                types["Type"].Add("Mode Spread", "Actions");
                types["Type"].Add("Mode Spread On", "Actions");
                types["Type"].Add("Mode Ui Economic", "Actions");
                types["Type"].Add("Mode Ui Military", "Actions");
                types["Type"].Add("Mode Villager Build", "Actions");
                types["Type"].Add("Mode Wedge", "Actions");
                types["Type"].Add("Mode Wedge On", "Actions");
                types["Type"].Add("Reticule Attack", "Actions");
                types["Type"].Add("Reticule Null", "Actions");
                types["Type"].Add("Reticule Select A", "Actions");
                types["Type"].Add("Reticule Select B", "Actions");
                types["Type"].Add("Stance Aggressive", "Actions");
                types["Type"].Add("Stance Aggressive On", "Actions");
                types["Type"].Add("Stance Defensive", "Actions");
                types["Type"].Add("Stance Defensive On", "Actions");
                types["Type"].Add("Stance Passive", "Actions");
                types["Type"].Add("Stance Passive On", "Actions");
                types["Type"].Add("Stance Stand Ground", "Actions");
                types["Type"].Add("Stance Stand Ground On", "Actions");
                types["Type"].Add("Temple Spawnpoint", "Actions");
                types["Type"].Add("Wall To Gate", "Actions");
                types["Type"].Add("Classical Age", "Ages");
                types["Type"].Add("Heroic Age", "Ages");
                types["Type"].Add("Mythic Age", "Ages");
                types["Type"].Add("Score Age 1", "Ages");
                types["Type"].Add("Score Age 2", "Ages");
                types["Type"].Add("Score Age 3", "Ages");
                types["Type"].Add("Score Age 4", "Ages");
                types["Type"].Add("Score Age 5", "Ages");
                types["Type"].Add("Wonder Age", "Ages");
                types["Type"].Add("Archery Range", "Buildings");
                types["Type"].Add("Armory", "Buildings");
                types["Type"].Add("Asgardian Hill Fort", "Buildings");
                types["Type"].Add("Barracks", "Buildings");
                types["Type"].Add("Citadel Center Atlantean", "Buildings");
                types["Type"].Add("Citadel Center Egyptian", "Buildings");
                types["Type"].Add("Citadel Center Greek", "Buildings");
                types["Type"].Add("Citadel Center Norse", "Buildings");
                types["Type"].Add("Columns", "Buildings");
                types["Type"].Add("Counter Barracks", "Buildings");
                types["Type"].Add("Dock", "Buildings");
                types["Type"].Add("Dwarven Armory", "Buildings");
                types["Type"].Add("Economic Guild", "Buildings");
                types["Type"].Add("Farm", "Buildings");
                types["Type"].Add("Fortress", "Buildings");
                types["Type"].Add("Fountain", "Buildings");
                types["Type"].Add("Gate", "Buildings");
                types["Type"].Add("Granary", "Buildings");
                types["Type"].Add("Great Hall", "Buildings");
                types["Type"].Add("Healing Spring Building", "Buildings");
                types["Type"].Add("Hesperides Building", "Buildings");
                types["Type"].Add("Hill Fort", "Buildings");
                types["Type"].Add("House", "Buildings");
                types["Type"].Add("Lighthouse", "Buildings");
                types["Type"].Add("Longhouse", "Buildings");
                types["Type"].Add("Lumber Camp", "Buildings");
                types["Type"].Add("Lure Building", "Buildings");
                types["Type"].Add("Manor", "Buildings");
                types["Type"].Add("Market", "Buildings");
                types["Type"].Add("Migdol Stronghold", "Buildings");
                types["Type"].Add("Military Academy", "Buildings");
                types["Type"].Add("Military Barracks", "Buildings");
                types["Type"].Add("Mining Camp", "Buildings");
                types["Type"].Add("Mirror Tower", "Buildings");
                types["Type"].Add("Monument To Gods", "Buildings");
                types["Type"].Add("Monument To Pharaohs", "Buildings");
                types["Type"].Add("Monument To Priests", "Buildings");
                types["Type"].Add("Monument To Soldiers", "Buildings");
                types["Type"].Add("Monument To Villagers", "Buildings");
                types["Type"].Add("Obelisk", "Buildings");
                types["Type"].Add("Palace", "Buildings");
                types["Type"].Add("Ruins", "Buildings");
                types["Type"].Add("Sentry Tower", "Buildings");
                types["Type"].Add("Siege Works", "Buildings");
                types["Type"].Add("Sign", "Buildings");
                types["Type"].Add("Sky Passage", "Buildings");
                types["Type"].Add("Stable", "Buildings");
                types["Type"].Add("Storehouse", "Buildings");
                types["Type"].Add("Tartarian Gate Building", "Buildings");
                types["Type"].Add("Temple", "Buildings");
                types["Type"].Add("Titan Gate", "Buildings");
                types["Type"].Add("Town Center Atlantean", "Buildings");
                types["Type"].Add("Town Center Egyptian", "Buildings");
                types["Type"].Add("Town Center Greek", "Buildings");
                types["Type"].Add("Town Center Norse", "Buildings");
                types["Type"].Add("Village Center Atlantean", "Buildings");
                types["Type"].Add("Village Center Egyptian", "Buildings");
                types["Type"].Add("Village Center Greek", "Buildings");
                types["Type"].Add("Village Center Norse", "Buildings");
                types["Type"].Add("Wonder", "Buildings");
                types["Type"].Add("Wood Pile", "Buildings");
                types["Type"].Add("Wooden Wall", "Buildings");
                types["Type"].Add("Ancestors", "God Powers");
                types["Type"].Add("Asgardian Bastion", "God Powers");
                types["Type"].Add("Bolt", "God Powers");
                types["Type"].Add("Bronze", "God Powers");
                types["Type"].Add("Carnivora", "God Powers");
                types["Type"].Add("Ceasefire", "God Powers");
                types["Type"].Add("Chaos", "God Powers");
                types["Type"].Add("Citadel", "God Powers");
                types["Type"].Add("Curse", "God Powers");
                types["Type"].Add("Deconstruction", "God Powers");
                types["Type"].Add("Dwarven Mine", "God Powers");
                types["Type"].Add("Earthquake", "God Powers");
                types["Type"].Add("Eclipse", "God Powers");
                types["Type"].Add("Fimbulwinter", "God Powers");
                types["Type"].Add("Flaming Weapons", "God Powers");
                types["Type"].Add("Forest Fire", "God Powers");
                types["Type"].Add("Frost", "God Powers");
                types["Type"].Add("Gaia Forest", "God Powers");
                types["Type"].Add("Great Hunt", "God Powers");
                types["Type"].Add("Gullinbursti", "God Powers");
                types["Type"].Add("Healing Spring", "God Powers");
                types["Type"].Add("Hesperides", "God Powers");
                types["Type"].Add("Implode", "God Powers");
                types["Type"].Add("Inferno", "God Powers");
                types["Type"].Add("Lightning Storm", "God Powers");
                types["Type"].Add("Locust Swarm", "God Powers");
                types["Type"].Add("Lure", "God Powers");
                types["Type"].Add("Meteor", "God Powers");
                types["Type"].Add("Nidhogg", "God Powers");
                types["Type"].Add("Pestilence", "God Powers");
                types["Type"].Add("Plague Of Serpents", "God Powers");
                types["Type"].Add("Plenty Vault", "God Powers");
                types["Type"].Add("Prosperity", "God Powers");
                types["Type"].Add("Ragnarok", "God Powers");
                types["Type"].Add("Rain", "God Powers");
                types["Type"].Add("Restoration", "God Powers");
                types["Type"].Add("Sentinel", "God Powers");
                types["Type"].Add("Shifting Sands", "God Powers");
                types["Type"].Add("Shockwave", "God Powers");
                types["Type"].Add("Son Of Osiris", "God Powers");
                types["Type"].Add("Spider Lair", "God Powers");
                types["Type"].Add("Spy", "God Powers");
                types["Type"].Add("Tartarian Gate", "God Powers");
                types["Type"].Add("Tempest", "God Powers");
                types["Type"].Add("Tornado", "God Powers");
                types["Type"].Add("Traitor", "God Powers");
                types["Type"].Add("Undermine", "God Powers");
                types["Type"].Add("Underworld Passage", "God Powers");
                types["Type"].Add("Valor", "God Powers");
                types["Type"].Add("Vision", "God Powers");
                types["Type"].Add("Vortex", "God Powers");
                types["Type"].Add("Walking Woods", "God Powers");
                types["Type"].Add("Aegir", "Gods");
                types["Type"].Add("Anubis", "Gods");
                types["Type"].Add("Aphrodite Beta", "Gods");
                types["Type"].Add("Aphrodite", "Gods");
                types["Type"].Add("Apollo Beta", "Gods");
                types["Type"].Add("Apollo", "Gods");
                types["Type"].Add("Ares", "Gods");
                types["Type"].Add("Artemis", "Gods");
                types["Type"].Add("Athena Beta", "Gods");
                types["Type"].Add("Athena", "Gods");
                types["Type"].Add("Atlas", "Gods");
                types["Type"].Add("Baldr Beta", "Gods");
                types["Type"].Add("Baldr", "Gods");
                types["Type"].Add("Bast", "Gods");
                types["Type"].Add("Bragi", "Gods");
                types["Type"].Add("Dionysus Beta", "Gods");
                types["Type"].Add("Dionysus", "Gods");
                types["Type"].Add("Forseti", "Gods");
                types["Type"].Add("Freyja Beta", "Gods");
                types["Type"].Add("Freyja", "Gods");
                types["Type"].Add("Freyr", "Gods");
                types["Type"].Add("Gaia Custom", "Gods");
                types["Type"].Add("Gaia", "Gods");
                types["Type"].Add("Hades", "Gods");
                types["Type"].Add("Heimdall", "Gods");
                types["Type"].Add("Hekate", "Gods");
                types["Type"].Add("Hel", "Gods");
                types["Type"].Add("Helios", "Gods");
                types["Type"].Add("Hephaestus", "Gods");
                types["Type"].Add("Hera", "Gods");
                types["Type"].Add("Hermes Beta", "Gods");
                types["Type"].Add("Hermes", "Gods");
                types["Type"].Add("Horus", "Gods");
                types["Type"].Add("Hyperion", "Gods");
                types["Type"].Add("Isis", "Gods");
                types["Type"].Add("Kronos", "Gods");
                types["Type"].Add("Leto Beta", "Gods");
                types["Type"].Add("Leto", "Gods");
                types["Type"].Add("Loki Beta", "Gods");
                types["Type"].Add("Loki", "Gods");
                types["Type"].Add("Nephthys Beta", "Gods");
                types["Type"].Add("Nephthys", "Gods");
                types["Type"].Add("Njord", "Gods");
                types["Type"].Add("Oceanus Beta", "Gods");
                types["Type"].Add("Oceanus", "Gods");
                types["Type"].Add("Odin", "Gods");
                types["Type"].Add("Oranos Beta", "Gods");
                types["Type"].Add("Oranos", "Gods");
                types["Type"].Add("Osiris", "Gods");
                types["Type"].Add("Poseidon Beta", "Gods");
                types["Type"].Add("Poseidon", "Gods");
                types["Type"].Add("Prometheus Beta", "Gods");
                types["Type"].Add("Prometheus", "Gods");
                types["Type"].Add("Ptah", "Gods");
                types["Type"].Add("Ra", "Gods");
                types["Type"].Add("Rheia Beta", "Gods");
                types["Type"].Add("Rheia", "Gods");
                types["Type"].Add("Sekhmet", "Gods");
                types["Type"].Add("Set Beta", "Gods");
                types["Type"].Add("Set", "Gods");
                types["Type"].Add("Skadi", "Gods");
                types["Type"].Add("Sobek", "Gods");
                types["Type"].Add("Theia Beta", "Gods");
                types["Type"].Add("Theia", "Gods");
                types["Type"].Add("Thor Beta", "Gods");
                types["Type"].Add("Thor", "Gods");
                types["Type"].Add("Thoth", "Gods");
                types["Type"].Add("Tyr", "Gods");
                types["Type"].Add("Ullr", "Gods");
                types["Type"].Add("Vidar", "Gods");
                types["Type"].Add("Zeus Beta", "Gods");
                types["Type"].Add("Zeus", "Gods");
                types["Type"].Add("Acropolis", "Maps");
                types["Type"].Add("Air", "Maps");
                types["Type"].Add("Alfheim", "Maps");
                types["Type"].Add("All Maps", "Maps");
                types["Type"].Add("Anatolia", "Maps");
                types["Type"].Add("Archipelago", "Maps");
                types["Type"].Add("Arena", "Maps");
                types["Type"].Add("Black Sea", "Maps");
                types["Type"].Add("Blue Lagoon", "Maps");
                types["Type"].Add("Elysium", "Maps");
                types["Type"].Add("Erebus", "Maps");
                types["Type"].Add("Ghost Lake", "Maps");
                types["Type"].Add("Giza", "Maps");
                types["Type"].Add("Gold Rush", "Maps");
                types["Type"].Add("Highland", "Maps");
                types["Type"].Add("Ironwood", "Maps");
                types["Type"].Add("Islands", "Maps");
                types["Type"].Add("Jotunheim", "Maps");
                types["Type"].Add("Kerlaugar", "Maps");
                types["Type"].Add("Land Unknown", "Maps");
                types["Type"].Add("Mapthumb Land", "Maps");
                types["Type"].Add("Mapthumb Navy", "Maps");
                types["Type"].Add("Mapthumb Standard", "Maps");
                types["Type"].Add("Marsh", "Maps");
                types["Type"].Add("Mediterranean", "Maps");
                types["Type"].Add("Megalopolis", "Maps");
                types["Type"].Add("Midgard", "Maps");
                types["Type"].Add("Mirage", "Maps");
                types["Type"].Add("Mirkwood", "Maps");
                types["Type"].Add("Mount Olympus", "Maps");
                types["Type"].Add("Muspellheim", "Maps");
                types["Type"].Add("Nile Shallows", "Maps");
                types["Type"].Add("Nomad", "Maps");
                types["Type"].Add("Oasis", "Maps");
                types["Type"].Add("River Nile", "Maps");
                types["Type"].Add("River Styx", "Maps");
                types["Type"].Add("Savannah", "Maps");
                types["Type"].Add("Sea Of Worms", "Maps");
                types["Type"].Add("Team Migration", "Maps");
                types["Type"].Add("The Unknown", "Maps");
                types["Type"].Add("Tiny", "Maps");
                types["Type"].Add("Treasury Black Map", "Maps");
                types["Type"].Add("Tundra", "Maps");
                types["Type"].Add("Valley Of Kings", "Maps");
                types["Type"].Add("Vinlandsaga", "Maps");
                types["Type"].Add("Watering Hole", "Maps");
                types["Type"].Add("Standard Difficulty", "Others");
                types["Type"].Add("Moderate Difficulty", "Others");
                types["Type"].Add("Hard Difficulty", "Others");
                types["Type"].Add("Titan Difficulty", "Others");
                types["Type"].Add("Area", "Others");
                types["Type"].Add("Crush Armor", "Others");
                types["Type"].Add("Crush Dmg", "Others");
                types["Type"].Add("Divine Dmg", "Others");
                types["Type"].Add("Garrison Stat", "Others");
                types["Type"].Add("Hack Armor", "Others");
                types["Type"].Add("Hack Dmg", "Others");
                types["Type"].Add("Hp", "Others");
                types["Type"].Add("Hp Regen", "Others");
                types["Type"].Add("Pierce Armor", "Others");
                types["Type"].Add("Pierce Dmg", "Others");
                types["Type"].Add("Projectile", "Others");
                types["Type"].Add("Range", "Others");
                types["Type"].Add("Rof", "Others");
                types["Type"].Add("Speed", "Others");
                types["Type"].Add("Time", "Others");
                types["Type"].Add("Arctic Wolf", "Resources");
                types["Type"].Add("Aurochs", "Resources");
                types["Type"].Add("Baboon", "Resources");
                types["Type"].Add("Bear", "Resources");
                types["Type"].Add("Berry Bush", "Resources");
                types["Type"].Add("Boar", "Resources");
                types["Type"].Add("Caribou", "Resources");
                types["Type"].Add("Chicken", "Resources");
                types["Type"].Add("Cow", "Resources");
                types["Type"].Add("Crocodile", "Resources");
                types["Type"].Add("Crowned Crane", "Resources");
                types["Type"].Add("Deer", "Resources");
                types["Type"].Add("Elephant", "Resources");
                types["Type"].Add("Elk", "Resources");
                types["Type"].Add("Fish", "Resources");
                types["Type"].Add("Gazelle", "Resources");
                types["Type"].Add("Giraffe", "Resources");
                types["Type"].Add("Goat", "Resources");
                types["Type"].Add("Gold Mine", "Resources");
                types["Type"].Add("Hippopotamus", "Resources");
                types["Type"].Add("Hyena", "Resources");
                types["Type"].Add("Lion", "Resources");
                types["Type"].Add("Monkey", "Resources");
                types["Type"].Add("Monkey Raft", "Resources");
                types["Type"].Add("Pig", "Resources");
                types["Type"].Add("Polar Bear", "Resources");
                types["Type"].Add("Res Favor", "Resources");
                types["Type"].Add("Res Food", "Resources");
                types["Type"].Add("Res Gold", "Resources");
                types["Type"].Add("Res Military", "Resources");
                types["Type"].Add("Res Pop", "Resources");
                types["Type"].Add("Res Population", "Resources");
                types["Type"].Add("Res Wood", "Resources");
                types["Type"].Add("Rhinoceros", "Resources");
                types["Type"].Add("Settlement", "Resources");
                types["Type"].Add("Summoning", "Resources");
                types["Type"].Add("Tamarisk", "Resources");
                types["Type"].Add("Cypress", "Resources");
                types["Type"].Add("Cypress Snow", "Resources");
                types["Type"].Add("Gaia Tree", "Resources");
                types["Type"].Add("Hades Tree", "Resources");
                types["Type"].Add("Marsh Tree", "Resources");
                types["Type"].Add("Oak Autumn", "Resources");
                types["Type"].Add("Oak", "Resources");
                types["Type"].Add("Olive", "Resources");
                types["Type"].Add("Palm", "Resources");
                types["Type"].Add("Straggler", "Resources");
                types["Type"].Add("Pine Dead", "Resources");
                types["Type"].Add("Pine", "Resources");
                types["Type"].Add("Pine Snow", "Resources");
                types["Type"].Add("Savannah Tree", "Resources");
                types["Type"].Add("Tundra Tree", "Resources");
                types["Type"].Add("Tundra Snow", "Resources");
                types["Type"].Add("Walrus", "Resources");
                types["Type"].Add("Water Buffalo", "Resources");
                types["Type"].Add("Wolf", "Resources");
                types["Type"].Add("Zebra", "Resources");
                types["Type"].Add("Adze Of Wepwawet", "Technologies");
                types["Type"].Add("Aegis Shield", "Technologies");
                types["Type"].Add("Alluvial Clay", "Technologies");
                types["Type"].Add("Ambassadors", "Technologies");
                types["Type"].Add("Anastrophe", "Technologies");
                types["Type"].Add("Archaic Age", "Technologies");
                types["Type"].Add("Architects", "Technologies");
                types["Type"].Add("Arctic Gale", "Technologies");
                types["Type"].Add("Arctic Winds", "Technologies");
                types["Type"].Add("Argive Patronage", "Technologies");
                types["Type"].Add("Argonauts", "Technologies");
                types["Type"].Add("Asper Blood", "Technologies");
                types["Type"].Add("Atef Crown", "Technologies");
                types["Type"].Add("Avenging Spirit", "Technologies");
                types["Type"].Add("Axe Of Vengeance", "Technologies");
                types["Type"].Add("Ballista Tower", "Technologies");
                types["Type"].Add("Ballistics", "Technologies");
                types["Type"].Add("Beast Slayer", "Technologies");
                types["Type"].Add("Berserkergang", "Technologies");
                types["Type"].Add("Bite Of The Shark", "Technologies");
                types["Type"].Add("Boiling Oil", "Technologies");
                types["Type"].Add("Bone Bow", "Technologies");
                types["Type"].Add("Book Of Thoth", "Technologies");
                types["Type"].Add("Bow Saw", "Technologies");
                types["Type"].Add("Bravery", "Technologies");
                types["Type"].Add("Bronze Armor", "Technologies");
                types["Type"].Add("Bronze Shields", "Technologies");
                types["Type"].Add("Bronze Wall", "Technologies");
                types["Type"].Add("Bronze Weapons", "Technologies");
                types["Type"].Add("Burning Pitch", "Technologies");
                types["Type"].Add("Call Of Valhalla", "Technologies");
                types["Type"].Add("Carpenters", "Technologies");
                types["Type"].Add("Carrier Pigeons", "Technologies");
                types["Type"].Add("Cave Troll", "Technologies");
                types["Type"].Add("Celerity", "Technologies");
                types["Type"].Add("Champion Archers", "Technologies");
                types["Type"].Add("Champion Axemen", "Technologies");
                types["Type"].Add("Champion Camel Riders", "Technologies");
                types["Type"].Add("Champion Cavalry", "Technologies");
                types["Type"].Add("Champion Chariot Archers", "Technologies");
                types["Type"].Add("Champion Infantry", "Technologies");
                types["Type"].Add("Champion Infantry Norse", "Technologies");
                types["Type"].Add("Champion Slingers", "Technologies");
                types["Type"].Add("Champion Spearmen", "Technologies");
                types["Type"].Add("Champion War Elephants", "Technologies");
                types["Type"].Add("Champion Warships", "Technologies");
                types["Type"].Add("Channels", "Technologies");
                types["Type"].Add("Chthonic Rites", "Technologies");
                types["Type"].Add("Citadel Wall", "Technologies");
                types["Type"].Add("Clairvoyance", "Technologies");
                types["Type"].Add("Coinage", "Technologies");
                types["Type"].Add("Conscript Barracks Soldiers", "Technologies");
                types["Type"].Add("Conscript Cavalry", "Technologies");
                types["Type"].Add("Conscript Counter Soldiers", "Technologies");
                types["Type"].Add("Conscript Great Hall Soldiers", "Technologies");
                types["Type"].Add("Conscript Hill Fort Soldiers", "Technologies");
                types["Type"].Add("Conscript Infantry", "Technologies");
                types["Type"].Add("Conscript Longhouse Soldiers", "Technologies");
                types["Type"].Add("Conscript Mainline Soldiers", "Technologies");
                types["Type"].Add("Conscript Migdol Soldiers", "Technologies");
                types["Type"].Add("Conscript Palace Soldiers", "Technologies");
                types["Type"].Add("Conscript Ranged Soldiers", "Technologies");
                types["Type"].Add("Conscript Sailors", "Technologies");
                types["Type"].Add("Copper Armor", "Technologies");
                types["Type"].Add("Copper Shields", "Technologies");
                types["Type"].Add("Copper Weapons", "Technologies");
                types["Type"].Add("Crenellations", "Technologies");
                types["Type"].Add("Crimson Linen", "Technologies");
                types["Type"].Add("Criosphinx", "Technologies");
                types["Type"].Add("Crocodilopolis", "Technologies");
                types["Type"].Add("Daktyloi", "Technologies");
                types["Type"].Add("Dark Water", "Technologies");
                types["Type"].Add("Daughters Of The Sea", "Technologies");
                types["Type"].Add("Deimos Sword Of Dread", "Technologies");
                types["Type"].Add("Desert Wind", "Technologies");
                types["Type"].Add("Devotees Of Atlas", "Technologies");
                types["Type"].Add("Dionysia", "Technologies");
                types["Type"].Add("Disablot", "Technologies");
                types["Type"].Add("Divine Blood", "Technologies");
                types["Type"].Add("Draft Horses", "Technologies");
                types["Type"].Add("Dragonscale Shields", "Technologies");
                types["Type"].Add("Dwarven Auger", "Technologies");
                types["Type"].Add("Dwarven Breastplate", "Technologies");
                types["Type"].Add("Dwarven Weapons", "Technologies");
                types["Type"].Add("Electrum Bullets", "Technologies");
                types["Type"].Add("Empyrean Speed", "Technologies");
                types["Type"].Add("Enclosed Deck", "Technologies");
                types["Type"].Add("Engineers", "Technologies");
                types["Type"].Add("Enyos Bow Of Horror", "Technologies");
                types["Type"].Add("Eyes In The Forest", "Technologies");
                types["Type"].Add("Face Of The Gorgon", "Technologies");
                types["Type"].Add("Feasts Of Renown", "Technologies");
                types["Type"].Add("Feet Of The Jackal", "Technologies");
                types["Type"].Add("Flames Of Typhon", "Technologies");
                types["Type"].Add("Flood Control", "Technologies");
                types["Type"].Add("Flood Of The Nile", "Technologies");
                types["Type"].Add("Force Of The West Wind", "Technologies");
                types["Type"].Add("Forge Of Olympus", "Technologies");
                types["Type"].Add("Fortified Town Center", "Technologies");
                types["Type"].Add("Fortified Wall Egyptian", "Technologies");
                types["Type"].Add("Fortified Wall Greek", "Technologies");
                types["Type"].Add("Freyrs Gift", "Technologies");
                types["Type"].Add("Frontline Heroics", "Technologies");
                types["Type"].Add("Funeral Barge", "Technologies");
                types["Type"].Add("Funeral Rites", "Technologies");
                types["Type"].Add("Fury Of The Fallen", "Technologies");
                types["Type"].Add("Gemini", "Technologies");
                types["Type"].Add("Gjallarhorn", "Technologies");
                types["Type"].Add("Golden Apples", "Technologies");
                types["Type"].Add("Granite Blood", "Technologies");
                types["Type"].Add("Granite Maw", "Technologies");
                types["Type"].Add("Grasp Of Ran", "Technologies");
                types["Type"].Add("Greatest Of Fifty", "Technologies");
                types["Type"].Add("Guard Tower Atlantean", "Technologies");
                types["Type"].Add("Guard Tower Egyptian", "Technologies");
                types["Type"].Add("Guard Tower Greek", "Technologies");
                types["Type"].Add("Guardian Of Io", "Technologies");
                types["Type"].Add("Hall Of Thanes", "Technologies");
                types["Type"].Add("Halo Of The Sun", "Technologies");
                types["Type"].Add("Hamask", "Technologies");
                types["Type"].Add("Hammer Of Thunder", "Technologies");
                types["Type"].Add("Hand Axe", "Technologies");
                types["Type"].Add("Hand Of Talos", "Technologies");
                types["Type"].Add("Hands Of The Pharaoh", "Technologies");
                types["Type"].Add("Heart Of The Titans", "Technologies");
                types["Type"].Add("Heavy Archers", "Technologies");
                types["Type"].Add("Heavy Axemen", "Technologies");
                types["Type"].Add("Heavy Camel Riders", "Technologies");
                types["Type"].Add("Heavy Cavalry", "Technologies");
                types["Type"].Add("Heavy Chariot Archers", "Technologies");
                types["Type"].Add("Heavy Infantry", "Technologies");
                types["Type"].Add("Heavy Infantry Norse", "Technologies");
                types["Type"].Add("Heavy Slingers", "Technologies");
                types["Type"].Add("Heavy Spearmen", "Technologies");
                types["Type"].Add("Heavy War Elephants", "Technologies");
                types["Type"].Add("Heavy Warships", "Technologies");
                types["Type"].Add("Hephaestus Revenge", "Technologies");
                types["Type"].Add("Heroic Fleet", "Technologies");
                types["Type"].Add("Heroic Renewal", "Technologies");
                types["Type"].Add("Hieracosphinx", "Technologies");
                types["Type"].Add("Horns Of Consecration", "Technologies");
                types["Type"].Add("Hunting Equipment", "Technologies");
                types["Type"].Add("Huntress Axe", "Technologies");
                types["Type"].Add("Husbandry", "Technologies");
                types["Type"].Add("Iron Armor", "Technologies");
                types["Type"].Add("Iron Shields", "Technologies");
                types["Type"].Add("Iron Wall", "Technologies");
                types["Type"].Add("Iron Weapons", "Technologies");
                types["Type"].Add("Irrigation", "Technologies");
                types["Type"].Add("Jotuns", "Technologies");
                types["Type"].Add("Labyrinth Of Minos", "Technologies");
                types["Type"].Add("Lance Of Stone", "Technologies");
                types["Type"].Add("Leather Frame Shield", "Technologies");
                types["Type"].Add("Levy Barracks Soldiers", "Technologies");
                types["Type"].Add("Levy Cavalry", "Technologies");
                types["Type"].Add("Levy Counter Soldiers", "Technologies");
                types["Type"].Add("Levy Great Hall Soldiers", "Technologies");
                types["Type"].Add("Levy Hill Fort Soldiers", "Technologies");
                types["Type"].Add("Levy Infantry", "Technologies");
                types["Type"].Add("Levy Longhouse Soldiers", "Technologies");
                types["Type"].Add("Levy Mainline Soldiers", "Technologies");
                types["Type"].Add("Levy Migdol Soldiers", "Technologies");
                types["Type"].Add("Levy Palace Soldiers", "Technologies");
                types["Type"].Add("Levy Ranged Soldiers", "Technologies");
                types["Type"].Add("Long Serpent", "Technologies");
                types["Type"].Add("Lord Of Horses", "Technologies");
                types["Type"].Add("Masons", "Technologies");
                types["Type"].Add("Medium Archers", "Technologies");
                types["Type"].Add("Medium Axemen", "Technologies");
                types["Type"].Add("Medium Cavalry", "Technologies");
                types["Type"].Add("Medium Infantry", "Technologies");
                types["Type"].Add("Medium Infantry Norse", "Technologies");
                types["Type"].Add("Medium Slingers", "Technologies");
                types["Type"].Add("Medium Spearmen", "Technologies");
                types["Type"].Add("Medjay", "Technologies");
                types["Type"].Add("Meteoric Iron Armor", "Technologies");
                types["Type"].Add("Monstrous Rage", "Technologies");
                types["Type"].Add("Mythic Rejuvenation", "Technologies");
                types["Type"].Add("Nebty", "Technologies");
                types["Type"].Add("Necropolis", "Technologies");
                types["Type"].Add("New Kingdom", "Technologies");
                types["Type"].Add("Nine Waves", "Technologies");
                types["Type"].Add("Olympian Parentage", "Technologies");
                types["Type"].Add("Olympian Weapons", "Technologies");
                types["Type"].Add("Omniscience", "Technologies");
                types["Type"].Add("Oracle", "Technologies");
                types["Type"].Add("Orichalcum Mail", "Technologies");
                types["Type"].Add("Orichalcum Wall", "Technologies");
                types["Type"].Add("Perception", "Technologies");
                types["Type"].Add("Petrification", "Technologies");
                types["Type"].Add("Phobos Spear Of Panic", "Technologies");
                types["Type"].Add("Pickaxe", "Technologies");
                types["Type"].Add("Pioneer Of The Skies", "Technologies");
                types["Type"].Add("Plow", "Technologies");
                types["Type"].Add("Poseidons Secret", "Technologies");
                types["Type"].Add("Prophetic Sight", "Technologies");
                types["Type"].Add("Purse Seine", "Technologies");
                types["Type"].Add("Quarry", "Technologies");
                types["Type"].Add("Rampage", "Technologies");
                types["Type"].Add("Rheias Gift", "Technologies");
                types["Type"].Add("Rigsthula", "Technologies");
                types["Type"].Add("Rime", "Technologies");
                types["Type"].Add("Ring Giver", "Technologies");
                types["Type"].Add("Ring Oath", "Technologies");
                types["Type"].Add("Roar Of Orthus", "Technologies");
                types["Type"].Add("Sacred Cats", "Technologies");
                types["Type"].Add("Safeguard", "Technologies");
                types["Type"].Add("Salt Amphora", "Technologies");
                types["Type"].Add("Sarissa", "Technologies");
                types["Type"].Add("Scalloped Axe", "Technologies");
                types["Type"].Add("Secrets Of The Titans", "Technologies");
                types["Type"].Add("Serpent Spear", "Technologies");
                types["Type"].Add("Servants Of Glory", "Technologies");
                types["Type"].Add("Sessrumnir", "Technologies");
                types["Type"].Add("Shaduf", "Technologies");
                types["Type"].Add("Shaft Mine", "Technologies");
                types["Type"].Add("Shafts Of Plague", "Technologies");
                types["Type"].Add("Shoulder Of Talos", "Technologies");
                types["Type"].Add("Signal Fires", "Technologies");
                types["Type"].Add("Silent Resolve", "Technologies");
                types["Type"].Add("Skin Of The Rhino", "Technologies");
                types["Type"].Add("Slings Of The Sun", "Technologies");
                types["Type"].Add("Solar Barque", "Technologies");
                types["Type"].Add("Sons Of Sleipnir", "Technologies");
                types["Type"].Add("Sons Of The Sun", "Technologies");
                types["Type"].Add("Spear Of Horus", "Technologies");
                types["Type"].Add("Spirit Of Maat", "Technologies");
                types["Type"].Add("Spirited Charge", "Technologies");
                types["Type"].Add("Stone Wall Atlantean", "Technologies");
                types["Type"].Add("Stone Wall Egyptian", "Technologies");
                types["Type"].Add("Stone Wall Greek", "Technologies");
                types["Type"].Add("Stone Wall Norse", "Technologies");
                types["Type"].Add("Sun Ray", "Technologies");
                types["Type"].Add("Sundried Mud Brick", "Technologies");
                types["Type"].Add("Swine Array", "Technologies");
                types["Type"].Add("Sylvan Lore", "Technologies");
                types["Type"].Add("Tax Collectors", "Technologies");
                types["Type"].Add("Temple Of Healing", "Technologies");
                types["Type"].Add("Temporal Chaos", "Technologies");
                types["Type"].Add("Theft Of Fire", "Technologies");
                types["Type"].Add("Thracian Horses", "Technologies");
                types["Type"].Add("Thundering Hooves", "Technologies");
                types["Type"].Add("Thurisaz Rune", "Technologies");
                types["Type"].Add("Titan Shield", "Technologies");
                types["Type"].Add("Titanomachy", "Technologies");
                types["Type"].Add("Tusks Of Apedemak", "Technologies");
                types["Type"].Add("Twilight Of The Gods", "Technologies");
                types["Type"].Add("Valgaldr", "Technologies");
                types["Type"].Add("Valley Of The Kings", "Technologies");
                types["Type"].Add("Vaults Of Erebus", "Technologies");
                types["Type"].Add("Vikings", "Technologies");
                types["Type"].Add("Volcanic Forge", "Technologies");
                types["Type"].Add("Watch Tower Atlantean", "Technologies");
                types["Type"].Add("Watch Tower Egyptian", "Technologies");
                types["Type"].Add("Watch Tower Greek", "Technologies");
                types["Type"].Add("Watch Tower Norse", "Technologies");
                types["Type"].Add("Weightless Mace", "Technologies");
                types["Type"].Add("Will Of Kronos", "Technologies");
                types["Type"].Add("Winged Messenger", "Technologies");
                types["Type"].Add("Winter Harvest", "Technologies");
                types["Type"].Add("Wrath Of The Deep", "Technologies");
                types["Type"].Add("Ydalir", "Technologies");
                types["Type"].Add("Achilles", "Units");
                types["Type"].Add("Ajax", "Units");
                types["Type"].Add("Anubite", "Units");
                types["Type"].Add("Arctic Wolf Of Set", "Units");
                types["Type"].Add("Arcus Hero", "Units");
                types["Type"].Add("Arcus", "Units");
                types["Type"].Add("Argus", "Units");
                types["Type"].Add("Atalanta", "Units");
                types["Type"].Add("Aurochs Of Set", "Units");
                types["Type"].Add("Automaton", "Units");
                types["Type"].Add("Avenger", "Units");
                types["Type"].Add("Axeman", "Units");
                types["Type"].Add("Baboon Of Set", "Units");
                types["Type"].Add("Ballista", "Units");
                types["Type"].Add("Battle Boar", "Units");
                types["Type"].Add("Bear Of Set", "Units");
                types["Type"].Add("Behemoth", "Units");
                types["Type"].Add("Bellerophon", "Units");
                types["Type"].Add("Berserk", "Units");
                types["Type"].Add("Bireme", "Units");
                types["Type"].Add("Boar Of Set", "Units");
                types["Type"].Add("Caladria", "Units");
                types["Type"].Add("Camel Rider", "Units");
                types["Type"].Add("Caravan Atlantean", "Units");
                types["Type"].Add("Caravan Egyptian", "Units");
                types["Type"].Add("Caravan Greek", "Units");
                types["Type"].Add("Caravan Norse", "Units");
                types["Type"].Add("Carcinos", "Units");
                types["Type"].Add("Caribou Of Set", "Units");
                types["Type"].Add("Carnivora Unit", "Units");
                types["Type"].Add("Catapult", "Units");
                types["Type"].Add("Centaur", "Units");
                types["Type"].Add("Centimanus", "Units");
                types["Type"].Add("Chariot Archer", "Units");
                types["Type"].Add("Cheiroballista Hero", "Units");
                types["Type"].Add("Cheiroballista", "Units");
                types["Type"].Add("Chicken Of Set", "Units");
                types["Type"].Add("Chimera", "Units");
                types["Type"].Add("Chiron", "Units");
                types["Type"].Add("Colossus", "Units");
                types["Type"].Add("Contarius Hero", "Units");
                types["Type"].Add("Contarius", "Units");
                types["Type"].Add("Crocodile Of Set", "Units");
                types["Type"].Add("Crowned Crane Of Set", "Units");
                types["Type"].Add("Cyclops", "Units");
                types["Type"].Add("Deer Of Set", "Units");
                types["Type"].Add("Destroyer Hero", "Units");
                types["Type"].Add("Destroyer", "Units");
                types["Type"].Add("Dragon Ship", "Units");
                types["Type"].Add("Draugr", "Units");
                types["Type"].Add("Dreki", "Units");
                types["Type"].Add("Dryad", "Units");
                types["Type"].Add("Einheri", "Units");
                types["Type"].Add("Elephant Of Set", "Units");
                types["Type"].Add("Elk Of Set", "Units");
                types["Type"].Add("Fafnir", "Units");
                types["Type"].Add("Fanatic Hero", "Units");
                types["Type"].Add("Fanatic", "Units");
                types["Type"].Add("Fenris Wolf Brood", "Units");
                types["Type"].Add("Fimbulwinter Wolf", "Units");
                types["Type"].Add("Fire Giant", "Units");
                types["Type"].Add("Fire Ship", "Units");
                types["Type"].Add("Fire Siphon", "Units");
                types["Type"].Add("Fishing Ship Atlantean", "Units");
                types["Type"].Add("Fishing Ship Egyptian", "Units");
                types["Type"].Add("Fishing Ship Greek", "Units");
                types["Type"].Add("Fishing Ship Norse", "Units");
                types["Type"].Add("Frost Giant", "Units");
                types["Type"].Add("Gastraphetoros", "Units");
                types["Type"].Add("Gazelle Of Set", "Units");
                types["Type"].Add("Giraffe Of Set", "Units");
                types["Type"].Add("Godi", "Units");
                types["Type"].Add("Gullinbursti Unit", "Units");
                types["Type"].Add("Hades Shade", "Units");
                types["Type"].Add("Helepolis", "Units");
                types["Type"].Add("Heracles", "Units");
                types["Type"].Add("Hero Of Ragnarok Dwarf", "Units");
                types["Type"].Add("Hero Of Ragnarok", "Units");
                types["Type"].Add("Hersir", "Units");
                types["Type"].Add("Hetairos", "Units");
                types["Type"].Add("Hippeus", "Units");
                types["Type"].Add("Hippocampus", "Units");
                types["Type"].Add("Hippolyta", "Units");
                types["Type"].Add("Hippopotamus Of Set", "Units");
                types["Type"].Add("Hirdman", "Units");
                types["Type"].Add("Hoplite", "Units");
                types["Type"].Add("Huskarl", "Units");
                types["Type"].Add("Hydra", "Units");
                types["Type"].Add("Hyena Of Set", "Units");
                types["Type"].Add("Hypaspist", "Units");
                types["Type"].Add("Jarl", "Units");
                types["Type"].Add("Jason", "Units");
                types["Type"].Add("Jormun Elver", "Units");
                types["Type"].Add("Juggernaut", "Units");
                types["Type"].Add("Katapeltes Hero", "Units");
                types["Type"].Add("Katapeltes", "Units");
                types["Type"].Add("Kataskopos", "Units");
                types["Type"].Add("Kebenit", "Units");
                types["Type"].Add("Kraken", "Units");
                types["Type"].Add("Lampades", "Units");
                types["Type"].Add("Leviathan", "Units");
                types["Type"].Add("Lion Of Set", "Units");
                types["Type"].Add("Longboat", "Units");
                types["Type"].Add("Lost Ship", "Units");
                types["Type"].Add("Man O War", "Units");
                types["Type"].Add("Manticore", "Units");
                types["Type"].Add("Medusa", "Units");
                types["Type"].Add("Mercenary Cavalry", "Units");
                types["Type"].Add("Mercenary", "Units");
                types["Type"].Add("Militia", "Units");
                types["Type"].Add("Minion", "Units");
                types["Type"].Add("Minotaur", "Units");
                types["Type"].Add("Monkey Of Set", "Units");
                types["Type"].Add("Mountain Giant", "Units");
                types["Type"].Add("Mummy", "Units");
                types["Type"].Add("Murmillo Hero", "Units");
                types["Type"].Add("Murmillo", "Units");
                types["Type"].Add("Myrmidon", "Units");
                types["Type"].Add("Nemean Lion", "Units");
                types["Type"].Add("Nereid", "Units");
                types["Type"].Add("Nidhogg Unit", "Units");
                types["Type"].Add("Odysseus", "Units");
                types["Type"].Add("Oracle Hero", "Units");
                types["Type"].Add("Oracle Unit", "Units");
                types["Type"].Add("Ox Cart", "Units");
                types["Type"].Add("Pegasus", "Units");
                types["Type"].Add("Peltast", "Units");
                types["Type"].Add("Pentekonter", "Units");
                types["Type"].Add("Perseus", "Units");
                types["Type"].Add("Petrobolos", "Units");
                types["Type"].Add("Petsuchos", "Units");
                types["Type"].Add("Pharaoh", "Units");
                types["Type"].Add("Phoenix Egg", "Units");
                types["Type"].Add("Phoenix", "Units");
                types["Type"].Add("Polar Bear Of Set", "Units");
                types["Type"].Add("Polyphemus", "Units");
                types["Type"].Add("Portable Ram", "Units");
                types["Type"].Add("Priest", "Units");
                types["Type"].Add("Prodromos", "Units");
                types["Type"].Add("Promethean", "Units");
                types["Type"].Add("Promethean Offspring", "Units");
                types["Type"].Add("Raiding Cavalry", "Units");
                types["Type"].Add("Ramming Galley", "Units");
                types["Type"].Add("Raven", "Units");
                types["Type"].Add("Rhinoceros Of Set", "Units");
                types["Type"].Add("Roc", "Units");
                types["Type"].Add("Rock Giant", "Units");
                types["Type"].Add("Satyr", "Units");
                types["Type"].Add("Scarab", "Units");
                types["Type"].Add("Scorpion Man", "Units");
                types["Type"].Add("Scylla", "Units");
                types["Type"].Add("Sea Snake", "Units");
                types["Type"].Add("Sentinel Unit", "Units");
                types["Type"].Add("Serpent", "Units");
                types["Type"].Add("Servant", "Units");
                types["Type"].Add("Siege Bireme", "Units");
                types["Type"].Add("Siege Tower", "Units");
                types["Type"].Add("Slinger", "Units");
                types["Type"].Add("Son Of Osiris Unit", "Units");
                types["Type"].Add("Spearman", "Units");
                types["Type"].Add("Sphinx", "Units");
                types["Type"].Add("Spider Egg", "Units");
                types["Type"].Add("Stymphalian Bird", "Units");
                types["Type"].Add("Tartarian Spawn", "Units");
                types["Type"].Add("The Argo", "Units");
                types["Type"].Add("Theseus", "Units");
                types["Type"].Add("Throwing Axeman", "Units");
                types["Type"].Add("Titan Atlantean", "Units");
                types["Type"].Add("Titan Egyptian", "Units");
                types["Type"].Add("Titan Greek", "Units");
                types["Type"].Add("Titan Norse", "Units");
                types["Type"].Add("Toxotes", "Units");
                types["Type"].Add("Transport Ship Atlantean", "Units");
                types["Type"].Add("Transport Ship Egyptian", "Units");
                types["Type"].Add("Transport Ship Greek", "Units");
                types["Type"].Add("Transport Ship Norse", "Units");
                types["Type"].Add("Trireme", "Units");
                types["Type"].Add("Troll", "Units");
                types["Type"].Add("Turma Hero", "Units");
                types["Type"].Add("Turma", "Units");
                types["Type"].Add("Archer", "Units");
                types["Type"].Add("Archer Ship", "Units");
                types["Type"].Add("Building", "Units");
                types["Type"].Add("Cavalry", "Units");
                types["Type"].Add("Close Combat Ship", "Units");
                types["Type"].Add("Flying Unit", "Units");
                types["Type"].Add("Hero", "Units");
                types["Type"].Add("Human Soldier", "Units");
                types["Type"].Add("Infantry", "Units");
                types["Type"].Add("Myth Unit", "Units");
                types["Type"].Add("Ship", "Units");
                types["Type"].Add("Siege Ship", "Units");
                types["Type"].Add("Siege Weapon", "Units");
                types["Type"].Add("Titan", "Units");
                types["Type"].Add("Tower", "Units");
                types["Type"].Add("Villager", "Units");
                types["Type"].Add("Wall", "Units");
                types["Type"].Add("Valkyrie", "Units");
                types["Type"].Add("Villager Atlantean Hero", "Units");
                types["Type"].Add("Villager Atlantean", "Units");
                types["Type"].Add("Villager Dwarf", "Units");
                types["Type"].Add("Villager Egyptian", "Units");
                types["Type"].Add("Villager Greek", "Units");
                types["Type"].Add("Villager Norse", "Units");
                types["Type"].Add("Wadjet", "Units");
                types["Type"].Add("Walking Woods Unit", "Units");
                types["Type"].Add("Walrus Of Set", "Units");
                types["Type"].Add("War Barge", "Units");
                types["Type"].Add("War Elephant", "Units");
                types["Type"].Add("War Turtle", "Units");
                types["Type"].Add("Water Buffalo Of Set", "Units");
                types["Type"].Add("Water Carnivora", "Units");
                types["Type"].Add("Wolf Of Set", "Units");
                types["Type"].Add("Zebra Of Set", "Units");
                types["Type"].Add("Auto Scout Ability", "Abilities");
                types["Type"].Add("Idle", "Others");
                types["Type"].Add("1S", "Other");
                types["Type"].Add("2S", "Other");
                types["Type"].Add("3S", "Other");
                types["Type"].Add("4S", "Other");
                types["Type"].Add("5S", "Other");
                types["Type"].Add("6S", "Other");
                types["Type"].Add("7S", "Other");
                types["Type"].Add("8S", "Other");
                types["Type"].Add("9S", "Other");
                types["Type"].Add("10S", "Other");
                types["Type"].Add("Less Than", "Other");
                types["Type"].Add("Open Square Bracket", "Other");
                types["Type"].Add("Close Square Bracket", "Other");
                types["Type"].Add("0S", "Other");
                types["Type"].Add("Up", "Other");
                types["Type"].Add("Right", "Other");
                types["Type"].Add("Left", "Other");
                types["Type"].Add("Down", "Other");
                types["Type"].Add("Question", "Other");
                types["Type"].Add("Atlanteans", "Civilizations");
                types["Type"].Add("Egyptians", "Civilizations");
                types["Type"].Add("Greeks", "Civilizations");
                types["Type"].Add("Norse", "Civilizations");
                types["Type"].Add("Atlanteans Circular", "Civilizations");
                types["Type"].Add("Egyptians Circular", "Civilizations");
                types["Type"].Add("Greeks Circular", "Civilizations");
                types["Type"].Add("Norse Circular", "Civilizations");
                types["Type"].Add("Woodline", "Resources");
                types["Type"].Add("Pray", "Actions");
                types["Type"].Add("Explore", "Actions");
                types["Type"].Add("Autoqueue", "Actions");
                types["Type"].Add("Relic", "Relics");
                types["Type"].Add("Axe Cart", "Units");
                types["Type"].Add("Baihu", "Units");
                types["Type"].Add("Caravan Chinese", "Units");
                types["Type"].Add("Chiwen", "Units");
                types["Type"].Add("Chu Ko Nu", "Units");
                types["Type"].Add("Dao Swordsman", "Units");
                types["Type"].Add("Doujian", "Units");
                types["Type"].Add("Enchanted", "Units");
                types["Type"].Add("Fei", "Units");
                types["Type"].Add("Fire Archer", "Units");
                types["Type"].Add("Fishing Ship Chinese", "Units");
                types["Type"].Add("Ge Halberdier", "Units");
                types["Type"].Add("Gebing Double", "Units");
                types["Type"].Add("Gongqi", "Units");
                types["Type"].Add("Hundun", "Units");
                types["Type"].Add("Jiang Ziya", "Units");
                types["Type"].Add("Kuafu Hero", "Units");
                types["Type"].Add("Kuafu", "Units");
                types["Type"].Add("Li Jing", "Units");
                types["Type"].Add("Louchuan", "Units");
                types["Type"].Add("Mengchong", "Units");
                types["Type"].Add("Nezha Child", "Units");
                types["Type"].Add("Nezha", "Units");
                types["Type"].Add("Nezha Youth", "Units");
                types["Type"].Add("Pioneer", "Units");
                types["Type"].Add("Pixiu", "Units");
                types["Type"].Add("Qilin", "Units");
                types["Type"].Add("Qinglong", "Units");
                types["Type"].Add("Qingqi Double", "Units");
                types["Type"].Add("Qingqi", "Units");
                types["Type"].Add("Qiongqi", "Units");
                types["Type"].Add("Sage", "Units");
                types["Type"].Add("Siege Crossbow", "Units");
                types["Type"].Add("Sky Lantern", "Units");
                types["Type"].Add("Taotie", "Units");
                types["Type"].Add("Taowu", "Units");
                types["Type"].Add("Terracotta Rider", "Units");
                types["Type"].Add("Tiger Cavalry Dismounted", "Units");
                types["Type"].Add("Tiger Cavalry", "Units");
                types["Type"].Add("Titan Chinese", "Units");
                types["Type"].Add("Transport Ship Chinese", "Units");
                types["Type"].Add("Villager Chinese Clay", "Units");
                types["Type"].Add("Villager Chinese Female", "Units");
                types["Type"].Add("Villager Chinese", "Units");
                types["Type"].Add("Villager Chinese Male", "Units");
                types["Type"].Add("Wen Zhong", "Units");
                types["Type"].Add("White Horse Cavalry", "Units");
                types["Type"].Add("Wuzu Javelineer", "Units");
                types["Type"].Add("Xuanwu", "Units");
                types["Type"].Add("Yang Jian", "Units");
                types["Type"].Add("Yazi", "Units");
                types["Type"].Add("Yinglong", "Units");
                types["Type"].Add("Zhuque", "Units");
                types["Type"].Add("Archery Training Module", "Buildings");
                types["Type"].Add("Baolei", "Buildings");
                types["Type"].Add("Cavalry Training", "Buildings");
                types["Type"].Add("Citadel Center Chinese", "Buildings");
                types["Type"].Add("Earth Wall", "Buildings");
                types["Type"].Add("Elite Training Module", "Buildings");
                types["Type"].Add("Farm Rice", "Buildings");
                types["Type"].Add("Farm Shennong", "Buildings");
                types["Type"].Add("Imperial Academy", "Buildings");
                types["Type"].Add("Infantry Training Module", "Buildings");
                types["Type"].Add("Machine Workshop", "Buildings");
                types["Type"].Add("Military Camp", "Buildings");
                types["Type"].Add("Silo", "Buildings");
                types["Type"].Add("The Peach Blossom Spring", "Buildings");
                types["Type"].Add("Tower Camp", "Buildings");
                types["Type"].Add("Town Center Chinese", "Buildings");
                types["Type"].Add("Training Camp", "Buildings");
                types["Type"].Add("Village Center Chinese", "Buildings");
                types["Type"].Add("Blazing Prairie", "God Powers");
                types["Type"].Add("Creation", "God Powers");
                types["Type"].Add("Drought Land", "God Powers");
                types["Type"].Add("Earth Wall Power", "God Powers");
                types["Type"].Add("Forest Protection", "God Powers");
                types["Type"].Add("Great Flood", "God Powers");
                types["Type"].Add("Lightning Weapons", "God Powers");
                types["Type"].Add("Prosperous Seeds", "God Powers");
                types["Type"].Add("Shennong Gift All", "God Powers");
                types["Type"].Add("Shennong Gift Classical", "God Powers");
                types["Type"].Add("Shennong Gift Heroic", "God Powers");
                types["Type"].Add("Shennong Gift Mythic", "God Powers");
                types["Type"].Add("The Peach Blossom Spring Power", "God Powers");
                types["Type"].Add("Vanish", "God Powers");
                types["Type"].Add("Venom Beast", "God Powers");
                types["Type"].Add("Yinglongs Wrath", "God Powers");
                types["Type"].Add("A Bumper Grain Harvest", "Technologies");
                types["Type"].Add("Abundance", "Technologies");
                types["Type"].Add("Advanced Defenses", "Technologies");
                types["Type"].Add("Autumn Of Abundance", "Technologies");
                types["Type"].Add("Bottomless Stomach", "Technologies");
                types["Type"].Add("Celestial Weapons", "Technologies");
                types["Type"].Add("Champion Baimayicong", "Technologies");
                types["Type"].Add("Champion Chukonu", "Technologies");
                types["Type"].Add("Champion Gebing", "Technologies");
                types["Type"].Add("Champion Gongqi", "Technologies");
                types["Type"].Add("Champion Gongshou", "Technologies");
                types["Type"].Add("Champion Hubaoqi", "Technologies");
                types["Type"].Add("Champion Infantry Chinese", "Technologies");
                types["Type"].Add("Champion Jinwei", "Technologies");
                types["Type"].Add("Champion Qingqi", "Technologies");
                types["Type"].Add("Champion Wuzu", "Technologies");
                types["Type"].Add("Chasing The Sun", "Technologies");
                types["Type"].Add("Conscript Baolei Soldiers", "Technologies");
                types["Type"].Add("Divine Books", "Technologies");
                types["Type"].Add("Divine Judgement", "Technologies");
                types["Type"].Add("Divine Light", "Technologies");
                types["Type"].Add("Drought Ships", "Technologies");
                types["Type"].Add("East Wind", "Technologies");
                types["Type"].Add("Fish Basket", "Technologies");
                types["Type"].Add("Flaming Blood", "Technologies");
                types["Type"].Add("Fortified Wall Chinese", "Technologies");
                types["Type"].Add("Frenzied Dash", "Technologies");
                types["Type"].Add("Fusillade Tower", "Technologies");
                types["Type"].Add("Giants Favor", "Technologies");
                types["Type"].Add("Gilded Shields", "Technologies");
                types["Type"].Add("Great Wall", "Technologies");
                types["Type"].Add("Guard Tower Chinese", "Technologies");
                types["Type"].Add("Heavy Gebing", "Technologies");
                types["Type"].Add("Heavy Gongqi", "Technologies");
                types["Type"].Add("Heavy Gongshou", "Technologies");
                types["Type"].Add("Heavy Hubaoqi", "Technologies");
                types["Type"].Add("Heavy Infantry Chinese", "Technologies");
                types["Type"].Add("Heavy Qingqi", "Technologies");
                types["Type"].Add("Heavy Wuzu", "Technologies");
                types["Type"].Add("Herbal Medicine", "Technologies");
                types["Type"].Add("Hooves Of The Wind", "Technologies");
                types["Type"].Add("Imperial Order", "Technologies");
                types["Type"].Add("Kuafu Chieftain", "Technologies");
                types["Type"].Add("Land Consolidation", "Technologies");
                types["Type"].Add("Last Stand", "Technologies");
                types["Type"].Add("Leizus Silk", "Technologies");
                types["Type"].Add("Levy Baolei Soldiers", "Technologies");
                types["Type"].Add("Longevity Blessing", "Technologies");
                types["Type"].Add("Maelstrom", "Technologies");
                types["Type"].Add("Master Of Weaponry", "Technologies");
                types["Type"].Add("Medium Gebing", "Technologies");
                types["Type"].Add("Medium Gongshou", "Technologies");
                types["Type"].Add("Medium Infantry Chinese", "Technologies");
                types["Type"].Add("Medium Qingqi", "Technologies");
                types["Type"].Add("Might Of Destruction", "Technologies");
                types["Type"].Add("Opportune Time", "Technologies");
                types["Type"].Add("Peach Of Immortality", "Technologies");
                types["Type"].Add("Power Of Chaos", "Technologies");
                types["Type"].Add("Qilins Blessing", "Technologies");
                types["Type"].Add("Rage Of Slaughter", "Technologies");
                types["Type"].Add("Red Cliffs Fleet", "Technologies");
                types["Type"].Add("Reincarnation", "Technologies");
                types["Type"].Add("Rising Tide", "Technologies");
                types["Type"].Add("Rock Solid", "Technologies");
                types["Type"].Add("Scorching Feathers", "Technologies");
                types["Type"].Add("Shaker Of Heaven", "Technologies");
                types["Type"].Add("Shield Blessing", "Technologies");
                types["Type"].Add("Silk Road", "Technologies");
                types["Type"].Add("Sinister Defiance", "Technologies");
                types["Type"].Add("Sky Fire", "Technologies");
                types["Type"].Add("Slash And Burn", "Technologies");
                types["Type"].Add("Son Of Loong", "Technologies");
                types["Type"].Add("Song Of Midsummer", "Technologies");
                types["Type"].Add("Southern Fire", "Technologies");
                types["Type"].Add("Spoils Of War", "Technologies");
                types["Type"].Add("Stone Wall Chinese", "Technologies");
                types["Type"].Add("Tai Chi", "Technologies");
                types["Type"].Add("Tempestuous Storm", "Technologies");
                types["Type"].Add("Temple Of Heaven", "Technologies");
                types["Type"].Add("Terracotta Riders", "Technologies");
                types["Type"].Add("Trading Season", "Technologies");
                types["Type"].Add("Vibrant Land", "Technologies");
                types["Type"].Add("Watch Tower Chinese", "Technologies");
                types["Type"].Add("Xuanyuans Bloodline", "Technologies");
                types["Type"].Add("Yang", "Technologies");
                types["Type"].Add("Yin", "Technologies");
                types["Type"].Add("Yin Yang", "Technologies");
                types["Type"].Add("Fuxi", "Gods");
                types["Type"].Add("Nuwa", "Gods");
                types["Type"].Add("Shennong", "Gods");
                types["Type"].Add("Chiyou", "Gods");
                types["Type"].Add("Gonggong", "Gods");
                types["Type"].Add("Goumang", "Gods");
                types["Type"].Add("Houtu", "Gods");
                types["Type"].Add("Huangdi", "Gods");
                types["Type"].Add("Nuba", "Gods");
                types["Type"].Add("Rushou", "Gods");
                types["Type"].Add("Xuannu", "Gods");
                types["Type"].Add("Zhurong", "Gods");
                types["Type"].Add("Bamboo", "Resources");
                types["Type"].Add("Chinese Pine Dead", "Resources");
                types["Type"].Add("Chinese Pine", "Resources");
                types["Type"].Add("Ginkgo Autumn", "Resources");
                types["Type"].Add("Ginkgo", "Resources");
                types["Type"].Add("Metasequoia Autumn", "Resources");
                types["Type"].Add("Metasequoia", "Resources");
                types["Type"].Add("Peach", "Resources");
                types["Type"].Add("Pear", "Resources");
                types["Type"].Add("Willow", "Resources");
                types["Type"].Add("Black Bear", "Resources");
                types["Type"].Add("Golden Pheasant", "Resources");
                types["Type"].Add("Panda", "Resources");
                types["Type"].Add("Red Crowned Crane", "Resources");
                types["Type"].Add("Spotted Deer", "Resources");
                types["Type"].Add("Turkey", "Resources");
                types["Type"].Add("Bamboo Grove", "Maps");
                types["Type"].Add("Great Wall Map", "Maps");
                types["Type"].Add("Peach Blossom Land", "Maps");
                types["Type"].Add("Qinghai Lake", "Maps");
                types["Type"].Add("Silk Road Map", "Maps");
                types["Type"].Add("Steppe", "Maps");
                types["Type"].Add("Yellow River", "Maps");
                types["Type"].Add("Chinese", "Civilizations");

            } // AOM >

            File.WriteAllText(Preferencias.TypesDefaultPath, SerializarNombres(types));

        } // CrearArchivoTipos>


        public static void CrearArchivoImágenesPersonalizadas() {

            var customImages = new Dictionary<NameType, Dictionary<string, string>>();
            customImages.Add(NameType.Image, new Dictionary<string, string>());
            if (Preferencias.Game == AOE2Name) {

                customImages[NameType.Image].Add("Fish (Action)", "Fish");
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

                customNames[NameType.Custom].Add("Food", "[Image]");
                customNames[NameType.Custom].Add("Wood", "[Image]");
                customNames[NameType.Custom].Add("Stone", "[Image]");
                customNames[NameType.Custom].Add("Gold", "[Image]");
                customNames[NameType.Custom].Add("Pikeman", "Pointy Bois");
                customNames[NameType.Custom].Add("Flemish Revolution", "The Button");

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

                names[NameType.Complete].Add("4201", "Dark Age"); names[NameType.Acronym].Add("4201", "DRK");
                names[NameType.Complete].Add("4202", "Feudal Age"); names[NameType.Acronym].Add("4202", "FDL");
                names[NameType.Complete].Add("4203", "Castle Age"); names[NameType.Acronym].Add("4203", "CST");
                names[NameType.Complete].Add("4204", "Imperial Age"); names[NameType.Abbreviation].Add("4204", "IMP");
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
                names[NameType.Complete].Add("4144", "Set Gather Point"); names[NameType.Acronym].Add("4144", "SET GP");
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
                names[NameType.Complete].Add("400024", "Fish (Action)");
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
                names[NameType.Complete].Add("400011", "Straggler Tree"); names[NameType.Acronym].Add("400011", "STR");
                names[NameType.Complete].Add("5172", "Rhinoceros"); names[NameType.Abbreviation].Add("5172", "Rhino"); names[NameType.CommonPlural].Add("5172", "Rhinoceroses"); names[NameType.AbbreviationPlural].Add("5172", "Rhinos");
                names[NameType.Complete].Add("5173", "Box Turtles"); names[NameType.Common].Add("5173", "Turtles");
                names[NameType.Complete].Add("5175", "Water Buffalo"); names[NameType.Common].Add("5175", "Buffalo"); names[NameType.CommonPlural].Add("5175", "Buffalos");
                names[NameType.Complete].Add("5743", "Elephant"); names[NameType.Abbreviation].Add("5743", "Ele"); names[NameType.CommonPlural].Add("5743", "Elephants"); names[NameType.AbbreviationPlural].Add("5743", "Eles");
                names[NameType.Complete].Add("5796", "Fruit Bush");
                names[NameType.Complete].Add("5252", "Stone Mine"); names[NameType.Acronym].Add("5252", "SM");
                names[NameType.Complete].Add("5400", "Gold Mine"); names[NameType.Acronym].Add("5400", "GM");
                names[NameType.Complete].Add("400029", "Food to Gold");
                names[NameType.Complete].Add("400030", "Food to Stone");
                names[NameType.Complete].Add("400031", "Food to Wood");
                names[NameType.Complete].Add("400032", "Gold to Food");
                names[NameType.Complete].Add("400033", "Gold to Stone");
                names[NameType.Complete].Add("400034", "Gold to Wood");
                names[NameType.Complete].Add("400035", "Stone to Food");
                names[NameType.Complete].Add("400036", "Stone to Gold");
                names[NameType.Complete].Add("400037", "Stone to Wood");
                names[NameType.Complete].Add("400038", "Wood to Food");
                names[NameType.Complete].Add("400039", "Wood to Gold");
                names[NameType.Complete].Add("400040", "Wood to Stone");
                names[NameType.Complete].Add("5083", "Archer"); names[NameType.Abbreviation].Add("5083", "Arch"); names[NameType.CommonPlural].Add("5083", "Archers"); names[NameType.AbbreviationPlural].Add("5083", "Archs");
                names[NameType.Complete].Add("5084", "Crossbowman"); names[NameType.Abbreviation].Add("5084", "Crossbow"); names[NameType.Acronym].Add("5084", "Xbow"); names[NameType.CommonPlural].Add("5084", "Crossbowmen"); names[NameType.AbbreviationPlural].Add("5084", "Crossbows"); names[NameType.AcronymPlural].Add("5084", "Xbows");
                names[NameType.Complete].Add("5418", "Arbalester"); names[NameType.Abbreviation].Add("5418", "Arb"); names[NameType.CommonPlural].Add("5418", "Arbalesters"); names[NameType.AbbreviationPlural].Add("5418", "Arbs");
                names[NameType.Complete].Add("5088", "Skirmisher"); names[NameType.Abbreviation].Add("5088", "Skirm"); names[NameType.Acronym].Add("5088", "SKR"); names[NameType.CommonPlural].Add("5088", "Skirmishers"); names[NameType.AbbreviationPlural].Add("5088", "Skirms");
                names[NameType.Complete].Add("5087", "Elite Skirmisher");
                names[NameType.Complete].Add("5190", "Imperial Skirmisher"); names[NameType.Abbreviation].Add("5190", "Imp Skirm"); names[NameType.CommonPlural].Add("5190", "Imperial Skirmishers"); names[NameType.AbbreviationPlural].Add("5190", "Imp Skirms");
                names[NameType.Complete].Add("5690", "Slinger"); names[NameType.CommonPlural].Add("5690", "Slingers");
                names[NameType.Complete].Add("5086", "Hand Cannoneer"); names[NameType.Acronym].Add("5086", "HCAN"); names[NameType.CommonPlural].Add("5086", "Hand Cannoneers");
                names[NameType.Complete].Add("5085", "Cavalry Archer"); names[NameType.Abbreviation].Add("5085", "Cav Archer"); names[NameType.Acronym].Add("5085", "CA"); names[NameType.CommonPlural].Add("5085", "Cavalry Archers"); names[NameType.AbbreviationPlural].Add("5085", "Cav Archers");
                names[NameType.Complete].Add("5412", "Heavy Cavalry Archer"); names[NameType.Acronym].Add("5412", "HCA"); names[NameType.CommonPlural].Add("5412", "Heavy Cavalry Archers");
                names[NameType.Complete].Add("5137", "Genitour"); names[NameType.Abbreviation].Add("5137", "Geni"); names[NameType.CommonPlural].Add("5137", "Genitours"); names[NameType.AbbreviationPlural].Add("5137", "Genis");
                names[NameType.Complete].Add("5139", "Elite Genitour");
                names[NameType.Complete].Add("5079", "Militia"); names[NameType.Abbreviation].Add("5079", "Mil"); names[NameType.CommonPlural].Add("5079", "Militias");
                names[NameType.Complete].Add("5080", "Man-at-Arms"); names[NameType.Acronym].Add("5080", "MAA|M@A"); names[NameType.CommonPlural].Add("5080", "Men-at-Arms");
                names[NameType.Complete].Add("5081", "Long Swordsman"); names[NameType.Abbreviation].Add("5081", "Longsword"); names[NameType.Acronym].Add("5081", "LS"); names[NameType.CommonPlural].Add("5081", "Long Swordsmen"); names[NameType.AbbreviationPlural].Add("5081", "Longswords");
                names[NameType.Complete].Add("5411", "Two-Handed Swordsman"); names[NameType.Acronym].Add("5411", "2HS"); names[NameType.CommonPlural].Add("5411", "Two-Handed Swordsmen");
                names[NameType.Complete].Add("5469", "Champion"); names[NameType.Abbreviation].Add("5469", "Champ"); names[NameType.Acronym].Add("5469", "CHMP"); names[NameType.CommonPlural].Add("5469", "Champions"); names[NameType.AbbreviationPlural].Add("5469", "Champs");
                names[NameType.Complete].Add("5078", "Spearman"); names[NameType.Abbreviation].Add("5078", "Spear"); names[NameType.Acronym].Add("5078", "SPR"); names[NameType.CommonPlural].Add("5078", "Spearmen"); names[NameType.AbbreviationPlural].Add("5078", "Spears");
                names[NameType.Complete].Add("5408", "Pikeman"); names[NameType.Abbreviation].Add("5408", "Pike"); names[NameType.Acronym].Add("5408", "PK"); names[NameType.CommonPlural].Add("5408", "Pikemen"); names[NameType.AbbreviationPlural].Add("5408", "Pikes");
                names[NameType.Complete].Add("5409", "Halberdier"); names[NameType.Abbreviation].Add("5409", "Halb"); names[NameType.CommonPlural].Add("5409", "Halberdiers"); names[NameType.AbbreviationPlural].Add("5409", "Halbs");
                names[NameType.Complete].Add("5672", "Eagle Scout"); names[NameType.Acronym].Add("5672", "ESC"); names[NameType.CommonPlural].Add("5672", "Eagle Scouts");
                names[NameType.Complete].Add("5671", "Eagle Warrior"); names[NameType.Common].Add("5671", "Eagle"); names[NameType.Acronym].Add("5671", "EW"); names[NameType.CommonPlural].Add("5671", "Eagles");
                names[NameType.Complete].Add("5673", "Elite Eagle Warrior");
                names[NameType.Complete].Add("5114", "Condottiero"); names[NameType.Abbreviation].Add("5114", "Condo"); names[NameType.CommonPlural].Add("5114", "Condottieros"); names[NameType.AbbreviationPlural].Add("5114", "Condos");
                names[NameType.Complete].Add("5326", "Scout Cavalry"); names[NameType.Common].Add("5326", "Scout"); names[NameType.Acronym].Add("5326", "SC"); names[NameType.CommonPlural].Add("5326", "Scouts");
                names[NameType.Complete].Add("5069", "Light Cavalry"); names[NameType.Abbreviation].Add("5069", "Light Cav"); names[NameType.Acronym].Add("5069", "Lcav|LCV"); names[NameType.CommonPlural].Add("5069", "Light Cavalries"); names[NameType.AbbreviationPlural].Add("5069", "Light Cavs"); names[NameType.AcronymPlural].Add("5069", "LCavs");
                names[NameType.Complete].Add("5661", "Hussar"); names[NameType.Acronym].Add("5661", "HSS"); names[NameType.CommonPlural].Add("5661", "Hussars");
                names[NameType.Complete].Add("5577", "Winged Hussar"); names[NameType.CommonPlural].Add("5577", "Winged Hussars");
                names[NameType.Complete].Add("5068", "Knight"); names[NameType.Acronym].Add("5068", "KT|KNT|KTS"); names[NameType.CommonPlural].Add("5068", "Knights");
                names[NameType.Complete].Add("5070", "Cavalier"); names[NameType.Abbreviation].Add("5070", "Cav|Cava"); names[NameType.CommonPlural].Add("5070", "Cavaliers"); names[NameType.AbbreviationPlural].Add("5070", "Cavs|Cavas");
                names[NameType.Complete].Add("5471", "Paladin"); names[NameType.Abbreviation].Add("5471", "Pala|Pal"); names[NameType.CommonPlural].Add("5471", "Paladins"); names[NameType.AbbreviationPlural].Add("5471", "Palas|Pals");
                names[NameType.Complete].Add("5416", "Camel Rider"); names[NameType.Common].Add("5416", "Camel"); names[NameType.Acronym].Add("5416", "CML"); names[NameType.CommonPlural].Add("5416", "Camels");
                names[NameType.Complete].Add("5417", "Heavy Camel Rider"); names[NameType.Common].Add("5417", "Heavy Camel"); names[NameType.Acronym].Add("5417", "HCML"); names[NameType.CommonPlural].Add("5417", "Heavy Camels");
                names[NameType.Complete].Add("5419", "Imperial Camel Rider"); names[NameType.Common].Add("5419", "Imperial Camel"); names[NameType.Abbreviation].Add("5419", "Imp Camel"); names[NameType.CommonPlural].Add("5419", "Imperial Camels"); names[NameType.AbbreviationPlural].Add("5419", "Imp Camels");
                names[NameType.Complete].Add("19033", "Battle Elephant"); names[NameType.Abbreviation].Add("19033", "Battle Ele"); names[NameType.Acronym].Add("19033", "BE"); names[NameType.CommonPlural].Add("19033", "Battle Elephants"); names[NameType.AbbreviationPlural].Add("19033", "Battle Eles");
                names[NameType.Complete].Add("5168", "Elite Battle Elephant");
                names[NameType.Complete].Add("19127", "Steppe Lancer"); names[NameType.Common].Add("19127", "Lancer"); names[NameType.Acronym].Add("19127", "SL"); names[NameType.CommonPlural].Add("19127", "Lancers");
                names[NameType.Complete].Add("5010", "Elite Steppe Lancer");
                names[NameType.Complete].Add("5040", "Xolotl Warrior"); names[NameType.Common].Add("5040", "Xolotl"); names[NameType.CommonPlural].Add("5040", "Xolotls");
                names[NameType.Complete].Add("5094", "Battering Ram"); names[NameType.Common].Add("5094", "Ram"); names[NameType.CommonPlural].Add("5094", "Rams");
                names[NameType.Complete].Add("5289", "Capped Ram"); names[NameType.Acronym].Add("5289", "CRAM"); names[NameType.CommonPlural].Add("5289", "Capped Rams");
                names[NameType.Complete].Add("5446", "Siege Ram"); names[NameType.Acronym].Add("5446", "SRAM"); names[NameType.CommonPlural].Add("5446", "Siege Rams");
                names[NameType.Complete].Add("5095", "Mangonel"); names[NameType.Abbreviation].Add("5095", "Mango"); names[NameType.Acronym].Add("5095", "MNG"); names[NameType.CommonPlural].Add("5095", "Mangonels"); names[NameType.AbbreviationPlural].Add("5095", "Mangos");
                names[NameType.Complete].Add("5448", "Onager"); names[NameType.Acronym].Add("5448", "ONG"); names[NameType.CommonPlural].Add("5448", "Onagers");
                names[NameType.Complete].Add("5493", "Siege Onager"); names[NameType.Acronym].Add("5493", "SO"); names[NameType.CommonPlural].Add("5493", "Siege Onagers");
                names[NameType.Complete].Add("5096", "Scorpion"); names[NameType.Abbreviation].Add("5096", "Scorp"); names[NameType.Acronym].Add("5096", "SCR"); names[NameType.CommonPlural].Add("5096", "Scorpions"); names[NameType.AbbreviationPlural].Add("5096", "Scorps");
                names[NameType.Complete].Add("5439", "Heavy Scorpion"); names[NameType.Abbreviation].Add("5439", "Heavy Scorp"); names[NameType.Acronym].Add("5439", "Hscorp|HSCR"); names[NameType.CommonPlural].Add("5439", "Heavy Scorpions"); names[NameType.AbbreviationPlural].Add("5439", "Heavy Scorps"); names[NameType.AcronymPlural].Add("5439", "HScorps");
                names[NameType.Complete].Add("5445", "Siege Tower"); names[NameType.Acronym].Add("5445", "STWR"); names[NameType.CommonPlural].Add("5445", "Siege Towers");
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
                names[NameType.Complete].Add("5099", "Monk"); names[NameType.Acronym].Add("5099", "MNK"); names[NameType.CommonPlural].Add("5099", "Monks");
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
                names[NameType.Complete].Add("5128", "Archery Range"); names[NameType.Common].Add("5128", "Archery|Range"); names[NameType.Acronym].Add("5128", "AR"); names[NameType.CommonPlural].Add("5128", "Archeries|Ranges");
                names[NameType.Complete].Add("5135", "Barracks"); names[NameType.Abbreviation].Add("5135", "Rax"); names[NameType.AbbreviationPlural].Add("5135", "Raxes");
                names[NameType.Complete].Add("5171", "Stable"); names[NameType.Acronym].Add("5171", "STBL|STB"); names[NameType.CommonPlural].Add("5171", "Stables");
                names[NameType.Complete].Add("5169", "Siege Workshop"); names[NameType.Acronym].Add("5169", "SWS"); names[NameType.CommonPlural].Add("5169", "Siege Workshops");
                names[NameType.Complete].Add("5131", "Blacksmith"); names[NameType.Acronym].Add("5131", "BLK");
                names[NameType.Complete].Add("5495", "Fish Trap"); names[NameType.CommonPlural].Add("5495", "Fish Traps");
                names[NameType.Complete].Add("5176", "University"); names[NameType.Abbreviation].Add("5176", "Uni"); names[NameType.CommonPlural].Add("5176", "Universities"); names[NameType.AbbreviationPlural].Add("5176", "Unis");
                names[NameType.Complete].Add("5144", "Dock"); names[NameType.CommonPlural].Add("5144", "Docks");
                names[NameType.Complete].Add("5504", "Outpost"); names[NameType.CommonPlural].Add("5504", "Outposts");
                names[NameType.Complete].Add("5178", "Watch Tower"); names[NameType.Common].Add("5178", "Tower"); names[NameType.Acronym].Add("5178", "TWR"); names[NameType.CommonPlural].Add("5178", "Towers");
                names[NameType.Complete].Add("5154", "Guard Tower"); names[NameType.CommonPlural].Add("5154", "Guard Towers");
                names[NameType.Complete].Add("5155", "Keep"); names[NameType.CommonPlural].Add("5155", "Keeps");
                names[NameType.Complete].Add("5156", "Bombard Tower"); names[NameType.Acronym].Add("5156", "BBT"); names[NameType.CommonPlural].Add("5156", "Bombard Towers");
                names[NameType.Complete].Add("5202", "Palisade Wall"); names[NameType.Common].Add("5202", "Palisade"); names[NameType.Acronym].Add("5202", "PW"); names[NameType.CommonPlural].Add("5202", "Palisades");
                names[NameType.Complete].Add("5186", "Palisade Gate"); names[NameType.CommonPlural].Add("5186", "Palisade Gates"); names[NameType.Acronym].Add("5186", "PG");
                names[NameType.Complete].Add("5203", "Stone Wall"); names[NameType.Acronym].Add("5203", "SW"); names[NameType.CommonPlural].Add("5203", "Stone Walls"); names[NameType.Acronym].Add("5185", "SG");
                names[NameType.Complete].Add("5204", "Fortified Wall"); names[NameType.CommonPlural].Add("5204", "Fortified Walls");
                names[NameType.Complete].Add("5185", "Gate"); names[NameType.Common].Add("5185", "Stone Gate"); names[NameType.CommonPlural].Add("5185", "Stone Gates");
                names[NameType.Complete].Add("5142", "Castle"); names[NameType.Acronym].Add("5142", "CSTL"); names[NameType.CommonPlural].Add("5142", "Castles");
                names[NameType.Complete].Add("19329", "Krepost"); names[NameType.Acronym].Add("19329", "KRP"); names[NameType.CommonPlural].Add("19329", "Kreposts");
                names[NameType.Complete].Add("19138", "Donjon"); names[NameType.Acronym].Add("19138", "DONJ"); names[NameType.CommonPlural].Add("19138", "Donjons");
                names[NameType.Complete].Add("5138", "Monastery"); names[NameType.Acronym].Add("5138", "MST"); names[NameType.CommonPlural].Add("5138", "Monasteries");
                names[NameType.Complete].Add("5344", "House"); names[NameType.Acronym].Add("5344", "H|HS"); names[NameType.CommonPlural].Add("5344", "Houses");
                names[NameType.Complete].Add("5164", "Town Center"); names[NameType.Acronym].Add("5164", "TC"); names[NameType.CommonPlural].Add("5164", "Town Centers");
                names[NameType.Complete].Add("5159", "Feitoria"); names[NameType.Acronym].Add("5159", "FEIT");
                names[NameType.Complete].Add("5487", "Mining Camp"); names[NameType.Acronym].Add("5487", "MC"); names[NameType.CommonPlural].Add("5487", "Mining Camps");
                names[NameType.Complete].Add("5464", "Lumber Camp"); names[NameType.Acronym].Add("5464", "LC"); names[NameType.CommonPlural].Add("5464", "Lumber Camps");
                names[NameType.Complete].Add("5581", "Folwark"); names[NameType.Acronym].Add("5581", "FWK"); names[NameType.CommonPlural].Add("5581", "Folwarks");
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
                names[NameType.Complete].Add("7403", "Supplies"); names[NameType.Acronym].Add("7403", "SPL");
                names[NameType.Complete].Add("7210", "Squires");
                names[NameType.Complete].Add("7258", "Arson");
                names[NameType.Complete].Add("7409", "Bloodlines"); names[NameType.Acronym].Add("7409", "BLDL");
                names[NameType.Complete].Add("7039", "Husbandry");
                names[NameType.Complete].Add("7208", "Padded Archer Armor"); names[NameType.Acronym].Add("7208", "PAA");
                names[NameType.Complete].Add("7172", "Fletching"); names[NameType.Acronym].Add("7172", "FCH");
                names[NameType.Complete].Add("7067", "Forging"); names[NameType.Acronym].Add("7067", "FRG");
                names[NameType.Complete].Add("7081", "Scale Barding Armor"); names[NameType.Acronym].Add("7081", "SBA");
                names[NameType.Complete].Add("7074", "Scale Mail Armor"); names[NameType.Acronym].Add("7074", "SMA");
                names[NameType.Complete].Add("7209", "Leather Archer Armor"); names[NameType.Acronym].Add("7209", "LAA");
                names[NameType.Complete].Add("7150", "Bodkin Arrow"); names[NameType.Common].Add("7150", "Bodkin"); names[NameType.Acronym].Add("7150", "BA");
                names[NameType.Complete].Add("7068", "Iron Casting"); names[NameType.Acronym].Add("7068", "IC");
                names[NameType.Complete].Add("7082", "Chain Barding Armor"); names[NameType.Acronym].Add("7082", "CBA");
                names[NameType.Complete].Add("7076", "Chain Mail Armor"); names[NameType.Acronym].Add("7076", "CMA");
                names[NameType.Complete].Add("7216", "Ring Archer Armor"); names[NameType.Acronym].Add("7216", "RAA");
                names[NameType.Complete].Add("7151", "Bracer"); names[NameType.Acronym].Add("7151", "BRC");
                names[NameType.Complete].Add("7075", "Blast Furnace"); names[NameType.Acronym].Add("7075", "BF");
                names[NameType.Complete].Add("7080", "Plate Barding Armor"); names[NameType.Acronym].Add("7080", "PBA");
                names[NameType.Complete].Add("7077", "Plate Mail Armor"); names[NameType.Acronym].Add("7077", "PMA");
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
                names[NameType.Complete].Add("7054", "Treadmill Crane"); names[NameType.Acronym].Add("7054", "TMC");
                names[NameType.Complete].Add("7376", "Hoardings");
                names[NameType.Complete].Add("7322", "Sappers");
                names[NameType.Complete].Add("7319", "Conscription");
                names[NameType.Complete].Add("7408", "Spies/Treason");
                names[NameType.Complete].Add("7315", "Redemption");
                names[NameType.Complete].Add("7316", "Atonement");
                names[NameType.Complete].Add("7435", "Herbal Medicine");
                names[NameType.Complete].Add("7412", "Heresy");
                names[NameType.Complete].Add("7221", "Sanctity");
                names[NameType.Complete].Add("7249", "Fervor"); names[NameType.Acronym].Add("7249", "FRV");
                names[NameType.Complete].Add("7045", "Faith");
                names[NameType.Complete].Add("7220", "Illumination");
                names[NameType.Complete].Add("7222", "Block Printing");
                names[NameType.Complete].Add("7416", "Theocracy");
                names[NameType.Complete].Add("7022", "Loom");
                names[NameType.Complete].Add("7008", "Town Watch");
                names[NameType.Complete].Add("7211", "Wheelbarrow"); names[NameType.Acronym].Add("7211", "WB");
                names[NameType.Complete].Add("7282", "Town Patrol");
                names[NameType.Complete].Add("7246", "Hand Cart"); names[NameType.Acronym].Add("7246", "HC");
                names[NameType.Complete].Add("7055", "Gold Mining"); names[NameType.Acronym].Add("7055", "GMG");
                names[NameType.Complete].Add("7276", "Stone Mining"); names[NameType.Acronym].Add("7276", "SMG");
                names[NameType.Complete].Add("7180", "Gold Shaft Mining"); names[NameType.Acronym].Add("7180", "GSM");
                names[NameType.Complete].Add("7277", "Stone Shaft Mining"); names[NameType.Acronym].Add("7277", "SSM");
                names[NameType.Complete].Add("7189", "Double-Bit Axe"); names[NameType.Acronym].Add("7189", "DBA");
                names[NameType.Complete].Add("7190", "Bow Saw"); names[NameType.Acronym].Add("7190", "BS");
                names[NameType.Complete].Add("7231", "Two-Man Saw"); names[NameType.Acronym].Add("7231", "TMS");
                names[NameType.Complete].Add("7014", "Horse Collar"); names[NameType.Acronym].Add("7014", "HRC");
                names[NameType.Complete].Add("7023", "Coinage");
                names[NameType.Complete].Add("7410", "Caravan");
                names[NameType.Complete].Add("7017", "Banking");
                names[NameType.Complete].Add("7015", "Guilds");
                names[NameType.Complete].Add("7013", "Heavy Plow"); names[NameType.Acronym].Add("7013", "HP");
                names[NameType.Complete].Add("7012", "Crop Rotation"); names[NameType.Acronym].Add("7012", "CR");
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
                names[NameType.Complete].Add("4313", "Population"); names[NameType.Abbreviation].Add("4313", "Pop|VS");
                names[NameType.Complete].Add("4314", "Range*");
                names[NameType.Complete].Add("4316", "Speed");
                names[NameType.Complete].Add("12201", "Line of Sight"); names[NameType.Acronym].Add("12201", "LoS");
                names[NameType.Complete].Add("19322", "Unique Unit"); names[NameType.Acronym].Add("19322", "UU"); names[NameType.CommonPlural].Add("19322", "Unique Units");
                names[NameType.Complete].Add("13073", "Idle");
                names[NameType.Complete].Add("13140", "Gather Point"); names[NameType.Acronym].Add("13140", "GP");
                names[NameType.Complete].Add("10026", "Artificial Intelligence"); names[NameType.Acronym].Add("10026", "AI");
                names[NameType.Complete].Add("20205", "Pierce Armor: "); names[NameType.Common].Add("20205", "Pierce Armor"); names[NameType.Acronym].Add("20205", "PA");
                names[NameType.Complete].Add("400012", "Long Distance"); names[NameType.Acronym].Add("400012", "LD");
                names[NameType.Complete].Add("400013", "Time"); names[NameType.Acronym].Add("400013", "t");
                names[NameType.Complete].Add("7019", "Cartography");
                names[NameType.Complete].Add("7090", "Tracking");
                names[NameType.Complete].Add("400041", "Cursor");
                names[NameType.Complete].Add("400042", "Arrow Up");
                names[NameType.Complete].Add("13055", "Explore");
                names[NameType.Complete].Add("13330", "Flag");
                names[NameType.Complete].Add("400064", "0s"); names[NameType.Common].Add("400064", "");
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
                names[NameType.Complete].Add("19324", "Go Back to Work"); names[NameType.Common].Add("19324", "Back to Work");
                names[NameType.Complete].Add("400065", "Woodline"); names[NameType.Acronym].Add("400065", "WL");
                names[NameType.Complete].Add("400066", "Chicken"); names[NameType.Acronym].Add("400066", "CHK");
                names[NameType.Complete].Add("400067", "Drop Off Resources"); names[NameType.Acronym].Add("400067", "DR");

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

            } else if (Preferencias.Game == AOMName) {

                names[NameType.Complete].Add("1", "Attack Move");
                names[NameType.Complete].Add("2", "Back To Work");
                names[NameType.Complete].Add("3", "Build Dock");
                names[NameType.Complete].Add("4", "Build Farm");
                names[NameType.Complete].Add("5", "Build Granary");
                names[NameType.Complete].Add("6", "Build Lumber Camp");
                names[NameType.Complete].Add("7", "Build Mining Camp");
                names[NameType.Complete].Add("8", "Build Storehouse");
                names[NameType.Complete].Add("9", "Delete");
                names[NameType.Complete].Add("10", "Eco Nearby Gather Point");
                names[NameType.Complete].Add("11", "Eject Back To Work");
                names[NameType.Complete].Add("12", "Flare");
                names[NameType.Complete].Add("13", "Flare Selection");
                names[NameType.Complete].Add("14", "Garrison"); names[NameType.Abbreviation].Add("14", "GRR");
                names[NameType.Complete].Add("15", "Gather Point"); names[NameType.Abbreviation].Add("15", "gp");
                names[NameType.Complete].Add("16", "Guard");
                names[NameType.Complete].Add("17", "Livestock Return");
                names[NameType.Complete].Add("18", "Mil Nearby Gather Point");
                names[NameType.Complete].Add("19", "Patrol");
                names[NameType.Complete].Add("20", "Repair");
                names[NameType.Complete].Add("21", "Return Resources");
                names[NameType.Complete].Add("22", "Seek Shelter"); names[NameType.Abbreviation].Add("22", "Shelter");
                names[NameType.Complete].Add("23", "Send Fishing Ship");
                names[NameType.Complete].Add("24", "Send Fishing Ship To Build");
                names[NameType.Complete].Add("25", "Send Heroes");
                names[NameType.Complete].Add("26", "Send Military");
                names[NameType.Complete].Add("27", "Send Navy");
                names[NameType.Complete].Add("28", "Send Priest");
                names[NameType.Complete].Add("29", "Send Priest To Build");
                names[NameType.Complete].Add("30", "Send Siege");
                names[NameType.Complete].Add("31", "Send Transport Ship");
                names[NameType.Complete].Add("32", "Send Villagers");
                names[NameType.Complete].Add("33", "Send Villagers To Build");
                names[NameType.Complete].Add("34", "Stop");
                names[NameType.Complete].Add("35", "Town Bell");
                names[NameType.Complete].Add("36", "Ungarrison"); names[NameType.Abbreviation].Add("36", "UG");
                names[NameType.Complete].Add("37", "Attac Building");
                names[NameType.Complete].Add("38", "Attack"); names[NameType.Abbreviation].Add("38", "ATK");
                names[NameType.Complete].Add("39", "Attack Forbidden");
                names[NameType.Complete].Add("40", "Attack Move Cursor");
                names[NameType.Complete].Add("41", "Auto Assign On Dropsite");
                names[NameType.Complete].Add("42", "Board");
                names[NameType.Complete].Add("43", "Build");
                names[NameType.Complete].Add("44", "Favor");
                names[NameType.Complete].Add("45", "Fish Cursor");
                names[NameType.Complete].Add("46", "Food Aggro Huntable");
                names[NameType.Complete].Add("47", "Food Berryfarm");
                names[NameType.Complete].Add("48", "Food Herdable");
                names[NameType.Complete].Add("49", "Food Huntable");
                names[NameType.Complete].Add("50", "Garrison Cursor");
                names[NameType.Complete].Add("51", "Godpower");
                names[NameType.Complete].Add("52", "Godpower Forbidden");
                names[NameType.Complete].Add("53", "Gold");
                names[NameType.Complete].Add("54", "Gold Trade");
                names[NameType.Complete].Add("55", "Heal");
                names[NameType.Complete].Add("56", "Herdable");
                names[NameType.Complete].Add("57", "Priest Empower"); names[NameType.Abbreviation].Add("57", "Empower|E");
                names[NameType.Complete].Add("58", "Rally"); names[NameType.Abbreviation].Add("58", "RLL");
                names[NameType.Complete].Add("59", "Relic Dropoff");
                names[NameType.Complete].Add("60", "Relic Pickup");
                names[NameType.Complete].Add("61", "Repair Cursor");
                names[NameType.Complete].Add("62", "Res Drop All");
                names[NameType.Complete].Add("63", "Res Drop Food"); names[NameType.Abbreviation].Add("63", "DF");
                names[NameType.Complete].Add("64", "Res Drop Gold"); names[NameType.Abbreviation].Add("64", "DG");
                names[NameType.Complete].Add("65", "Res Drop Other");
                names[NameType.Complete].Add("66", "Res Drop Wood"); names[NameType.Abbreviation].Add("66", "DW");
                names[NameType.Complete].Add("67", "Standard");
                names[NameType.Complete].Add("68", "Unboard");
                names[NameType.Complete].Add("69", "Walk");
                names[NameType.Complete].Add("70", "Wood Gathering");
                names[NameType.Complete].Add("71", "Market Buy Food");
                names[NameType.Complete].Add("72", "Market Buy Wood");
                names[NameType.Complete].Add("73", "Market Sell Food");
                names[NameType.Complete].Add("74", "Market Sell Wood");
                names[NameType.Complete].Add("75", "Mode Box");
                names[NameType.Complete].Add("76", "Mode Box On");
                names[NameType.Complete].Add("77", "Mode Line");
                names[NameType.Complete].Add("78", "Mode Line On");
                names[NameType.Complete].Add("79", "Mode Spread");
                names[NameType.Complete].Add("80", "Mode Spread On");
                names[NameType.Complete].Add("81", "Mode Ui Economic");
                names[NameType.Complete].Add("82", "Mode Ui Military");
                names[NameType.Complete].Add("83", "Mode Villager Build");
                names[NameType.Complete].Add("84", "Mode Wedge");
                names[NameType.Complete].Add("85", "Mode Wedge On");
                names[NameType.Complete].Add("86", "Reticule Attack");
                names[NameType.Complete].Add("87", "Reticule Null");
                names[NameType.Complete].Add("88", "Reticule Select A");
                names[NameType.Complete].Add("89", "Reticule Select B");
                names[NameType.Complete].Add("90", "Stance Aggressive");
                names[NameType.Complete].Add("91", "Stance Aggressive On");
                names[NameType.Complete].Add("92", "Stance Defensive");
                names[NameType.Complete].Add("93", "Stance Defensive On");
                names[NameType.Complete].Add("94", "Stance Passive");
                names[NameType.Complete].Add("95", "Stance Passive On");
                names[NameType.Complete].Add("96", "Stance Stand Ground");
                names[NameType.Complete].Add("97", "Stance Stand Ground On");
                names[NameType.Complete].Add("98", "Temple Spawnpoint");
                names[NameType.Complete].Add("99", "Wall To Gate");
                names[NameType.Complete].Add("100", "Classical Age");
                names[NameType.Complete].Add("101", "Heroic Age");
                names[NameType.Complete].Add("102", "Mythic Age");
                names[NameType.Complete].Add("103", "Score Age 1"); names[NameType.Abbreviation].Add("103", "AGE1");
                names[NameType.Complete].Add("104", "Score Age 2"); names[NameType.Abbreviation].Add("104", "AGE2");
                names[NameType.Complete].Add("105", "Score Age 3"); names[NameType.Abbreviation].Add("105", "AGE3");
                names[NameType.Complete].Add("106", "Score Age 4"); names[NameType.Abbreviation].Add("106", "AGE4");
                names[NameType.Complete].Add("107", "Score Age 5"); names[NameType.Abbreviation].Add("107", "AGE5");
                names[NameType.Complete].Add("108", "Wonder Age");
                names[NameType.Complete].Add("109", "Archery Range"); names[NameType.Abbreviation].Add("109", "Range|R|AR");
                names[NameType.Complete].Add("110", "Armory"); names[NameType.Abbreviation].Add("110", "A");
                names[NameType.Complete].Add("111", "Asgardian Hill Fort"); names[NameType.Abbreviation].Add("111", "ASGHF");
                names[NameType.Complete].Add("112", "Barracks"); names[NameType.Abbreviation].Add("112", "RAX");
                names[NameType.Complete].Add("113", "Citadel Center Atlantean");
                names[NameType.Complete].Add("114", "Citadel Center Egyptian");
                names[NameType.Complete].Add("115", "Citadel Center Greek");
                names[NameType.Complete].Add("116", "Citadel Center Norse");
                names[NameType.Complete].Add("117", "Columns");
                names[NameType.Complete].Add("118", "Counter Barracks"); names[NameType.Abbreviation].Add("118", "CB|CRAX");
                names[NameType.Complete].Add("119", "Dock"); names[NameType.Abbreviation].Add("119", "DK");
                names[NameType.Complete].Add("120", "Dwarven Armory"); names[NameType.Abbreviation].Add("120", "DA");
                names[NameType.Complete].Add("121", "Economic Guild"); names[NameType.Abbreviation].Add("121", "EG");
                names[NameType.Complete].Add("122", "Farm");
                names[NameType.Complete].Add("123", "Fortress"); names[NameType.Abbreviation].Add("123", "FRT");
                names[NameType.Complete].Add("124", "Fountain");
                names[NameType.Complete].Add("125", "Gate");
                names[NameType.Complete].Add("126", "Granary"); names[NameType.Abbreviation].Add("126", "GR");
                names[NameType.Complete].Add("127", "Great Hall"); names[NameType.Abbreviation].Add("127", "GH");
                names[NameType.Complete].Add("128", "Healing Spring Building");
                names[NameType.Complete].Add("129", "Hesperides Building");
                names[NameType.Complete].Add("130", "Hill Fort"); names[NameType.Abbreviation].Add("130", "HF");
                names[NameType.Complete].Add("131", "House"); names[NameType.Abbreviation].Add("131", "H");
                names[NameType.Complete].Add("132", "Lighthouse");
                names[NameType.Complete].Add("133", "Longhouse"); names[NameType.Abbreviation].Add("133", "LH");
                names[NameType.Complete].Add("134", "Lumber Camp"); names[NameType.Abbreviation].Add("134", "LC");
                names[NameType.Complete].Add("135", "Lure Building");
                names[NameType.Complete].Add("136", "Manor"); names[NameType.Abbreviation].Add("136", "M");
                names[NameType.Complete].Add("137", "Market"); names[NameType.Abbreviation].Add("137", "MKT");
                names[NameType.Complete].Add("138", "Migdol Stronghold"); names[NameType.Abbreviation].Add("138", "MS");
                names[NameType.Complete].Add("139", "Military Academy"); names[NameType.Abbreviation].Add("139", "MA");
                names[NameType.Complete].Add("140", "Military Barracks"); names[NameType.Abbreviation].Add("140", "MRAX");
                names[NameType.Complete].Add("141", "Mining Camp"); names[NameType.Abbreviation].Add("141", "MC");
                names[NameType.Complete].Add("142", "Mirror Tower"); names[NameType.Abbreviation].Add("142", "MTWR");
                names[NameType.Complete].Add("143", "Monument To Gods"); names[NameType.Abbreviation].Add("143", "MG");
                names[NameType.Complete].Add("144", "Monument To Pharaohs"); names[NameType.Abbreviation].Add("144", "MPH");
                names[NameType.Complete].Add("145", "Monument To Priests"); names[NameType.Abbreviation].Add("145", "MPR");
                names[NameType.Complete].Add("146", "Monument To Soldiers"); names[NameType.Abbreviation].Add("146", "MSO");
                names[NameType.Complete].Add("147", "Monument To Villagers"); names[NameType.Abbreviation].Add("147", "MV");
                names[NameType.Complete].Add("148", "Obelisk"); names[NameType.Abbreviation].Add("148", "OBK");
                names[NameType.Complete].Add("149", "Palace"); names[NameType.Abbreviation].Add("149", "PLC");
                names[NameType.Complete].Add("150", "Ruins");
                names[NameType.Complete].Add("151", "Sentry Tower"); names[NameType.Abbreviation].Add("151", "ST");
                names[NameType.Complete].Add("152", "Siege Works"); names[NameType.Abbreviation].Add("152", "SW");
                names[NameType.Complete].Add("153", "Sign");
                names[NameType.Complete].Add("154", "Sky Passage"); names[NameType.Abbreviation].Add("154", "SP");
                names[NameType.Complete].Add("155", "Stable"); names[NameType.Abbreviation].Add("155", "S|STB|STBL");
                names[NameType.Complete].Add("156", "Storehouse"); names[NameType.Abbreviation].Add("156", "SH");
                names[NameType.Complete].Add("157", "Tartarian Gate Building");
                names[NameType.Complete].Add("158", "Temple"); names[NameType.Abbreviation].Add("158", "T");
                names[NameType.Complete].Add("159", "Titan Gate");
                names[NameType.Complete].Add("160", "Town Center Atlantean"); names[NameType.Abbreviation].Add("160", "TCA");
                names[NameType.Complete].Add("161", "Town Center Egyptian"); names[NameType.Abbreviation].Add("161", "TCE");
                names[NameType.Complete].Add("162", "Town Center Greek"); names[NameType.Abbreviation].Add("162", "TCG");
                names[NameType.Complete].Add("163", "Town Center Norse"); names[NameType.Abbreviation].Add("163", "TCN");
                names[NameType.Complete].Add("164", "Village Center Atlantean"); names[NameType.Abbreviation].Add("164", "VCA");
                names[NameType.Complete].Add("165", "Village Center Egyptian"); names[NameType.Abbreviation].Add("165", "VCE");
                names[NameType.Complete].Add("166", "Village Center Greek"); names[NameType.Abbreviation].Add("166", "VCG");
                names[NameType.Complete].Add("167", "Village Center Norse"); names[NameType.Abbreviation].Add("167", "VCN");
                names[NameType.Complete].Add("168", "Wonder");
                names[NameType.Complete].Add("169", "Wood Pile");
                names[NameType.Complete].Add("170", "Wooden Wall"); names[NameType.Abbreviation].Add("170", "WW");
                names[NameType.Complete].Add("171", "Ancestors");
                names[NameType.Complete].Add("172", "Asgardian Bastion");
                names[NameType.Complete].Add("173", "Bolt");
                names[NameType.Complete].Add("174", "Bronze");
                names[NameType.Complete].Add("175", "Carnivora");
                names[NameType.Complete].Add("176", "Ceasefire");
                names[NameType.Complete].Add("177", "Chaos");
                names[NameType.Complete].Add("178", "Citadel");
                names[NameType.Complete].Add("179", "Curse");
                names[NameType.Complete].Add("180", "Deconstruction");
                names[NameType.Complete].Add("181", "Dwarven Mine");
                names[NameType.Complete].Add("182", "Earthquake");
                names[NameType.Complete].Add("183", "Eclipse");
                names[NameType.Complete].Add("184", "Fimbulwinter");
                names[NameType.Complete].Add("185", "Flaming Weapons");
                names[NameType.Complete].Add("186", "Forest Fire");
                names[NameType.Complete].Add("187", "Frost");
                names[NameType.Complete].Add("188", "Gaia Forest");
                names[NameType.Complete].Add("189", "Great Hunt");
                names[NameType.Complete].Add("190", "Gullinbursti");
                names[NameType.Complete].Add("191", "Healing Spring");
                names[NameType.Complete].Add("192", "Hesperides");
                names[NameType.Complete].Add("193", "Implode");
                names[NameType.Complete].Add("194", "Inferno");
                names[NameType.Complete].Add("195", "Lightning Storm");
                names[NameType.Complete].Add("196", "Locust Swarm");
                names[NameType.Complete].Add("197", "Lure");
                names[NameType.Complete].Add("198", "Meteor");
                names[NameType.Complete].Add("199", "Nidhogg");
                names[NameType.Complete].Add("200", "Pestilence");
                names[NameType.Complete].Add("201", "Plague Of Serpents");
                names[NameType.Complete].Add("202", "Plenty Vault");
                names[NameType.Complete].Add("203", "Prosperity");
                names[NameType.Complete].Add("204", "Ragnarok");
                names[NameType.Complete].Add("205", "Rain");
                names[NameType.Complete].Add("206", "Restoration");
                names[NameType.Complete].Add("207", "Sentinel");
                names[NameType.Complete].Add("208", "Shifting Sands");
                names[NameType.Complete].Add("209", "Shockwave");
                names[NameType.Complete].Add("210", "Son Of Osiris");
                names[NameType.Complete].Add("211", "Spider Lair");
                names[NameType.Complete].Add("212", "Spy");
                names[NameType.Complete].Add("213", "Tartarian Gate");
                names[NameType.Complete].Add("214", "Tempest");
                names[NameType.Complete].Add("215", "Tornado");
                names[NameType.Complete].Add("216", "Traitor");
                names[NameType.Complete].Add("217", "Undermine");
                names[NameType.Complete].Add("218", "Underworld Passage");
                names[NameType.Complete].Add("219", "Valor");
                names[NameType.Complete].Add("220", "Vision");
                names[NameType.Complete].Add("221", "Vortex");
                names[NameType.Complete].Add("222", "Walking Woods");
                names[NameType.Complete].Add("223", "Aegir");
                names[NameType.Complete].Add("224", "Anubis");
                names[NameType.Complete].Add("225", "Aphrodite Beta");
                names[NameType.Complete].Add("226", "Aphrodite");
                names[NameType.Complete].Add("227", "Apollo Beta");
                names[NameType.Complete].Add("228", "Apollo");
                names[NameType.Complete].Add("229", "Ares");
                names[NameType.Complete].Add("230", "Artemis");
                names[NameType.Complete].Add("231", "Athena Beta");
                names[NameType.Complete].Add("232", "Athena");
                names[NameType.Complete].Add("233", "Atlas");
                names[NameType.Complete].Add("234", "Baldr Beta");
                names[NameType.Complete].Add("235", "Baldr");
                names[NameType.Complete].Add("236", "Bast");
                names[NameType.Complete].Add("237", "Bragi");
                names[NameType.Complete].Add("238", "Dionysus Beta");
                names[NameType.Complete].Add("239", "Dionysus");
                names[NameType.Complete].Add("240", "Forseti");
                names[NameType.Complete].Add("241", "Freyja Beta");
                names[NameType.Complete].Add("242", "Freyja");
                names[NameType.Complete].Add("243", "Freyr");
                names[NameType.Complete].Add("244", "Gaia Custom");
                names[NameType.Complete].Add("245", "Gaia");
                names[NameType.Complete].Add("246", "Hades");
                names[NameType.Complete].Add("247", "Heimdall");
                names[NameType.Complete].Add("248", "Hekate");
                names[NameType.Complete].Add("249", "Hel");
                names[NameType.Complete].Add("250", "Helios");
                names[NameType.Complete].Add("251", "Hephaestus");
                names[NameType.Complete].Add("252", "Hera");
                names[NameType.Complete].Add("253", "Hermes Beta");
                names[NameType.Complete].Add("254", "Hermes");
                names[NameType.Complete].Add("255", "Horus");
                names[NameType.Complete].Add("256", "Hyperion");
                names[NameType.Complete].Add("257", "Isis");
                names[NameType.Complete].Add("258", "Kronos");
                names[NameType.Complete].Add("259", "Leto Beta");
                names[NameType.Complete].Add("260", "Leto");
                names[NameType.Complete].Add("261", "Loki Beta");
                names[NameType.Complete].Add("262", "Loki");
                names[NameType.Complete].Add("263", "Nephthys Beta");
                names[NameType.Complete].Add("264", "Nephthys");
                names[NameType.Complete].Add("265", "Njord");
                names[NameType.Complete].Add("266", "Oceanus Beta");
                names[NameType.Complete].Add("267", "Oceanus");
                names[NameType.Complete].Add("268", "Odin");
                names[NameType.Complete].Add("269", "Oranos Beta");
                names[NameType.Complete].Add("270", "Oranos");
                names[NameType.Complete].Add("271", "Osiris");
                names[NameType.Complete].Add("272", "Poseidon Beta");
                names[NameType.Complete].Add("273", "Poseidon");
                names[NameType.Complete].Add("274", "Prometheus Beta");
                names[NameType.Complete].Add("275", "Prometheus");
                names[NameType.Complete].Add("276", "Ptah");
                names[NameType.Complete].Add("277", "Ra");
                names[NameType.Complete].Add("278", "Rheia Beta");
                names[NameType.Complete].Add("279", "Rheia");
                names[NameType.Complete].Add("280", "Sekhmet");
                names[NameType.Complete].Add("281", "Set Beta");
                names[NameType.Complete].Add("282", "Set");
                names[NameType.Complete].Add("283", "Skadi");
                names[NameType.Complete].Add("284", "Sobek");
                names[NameType.Complete].Add("285", "Theia Beta");
                names[NameType.Complete].Add("286", "Theia");
                names[NameType.Complete].Add("287", "Thor Beta");
                names[NameType.Complete].Add("288", "Thor");
                names[NameType.Complete].Add("289", "Thoth");
                names[NameType.Complete].Add("290", "Tyr");
                names[NameType.Complete].Add("291", "Ullr");
                names[NameType.Complete].Add("292", "Vidar");
                names[NameType.Complete].Add("293", "Zeus Beta");
                names[NameType.Complete].Add("294", "Zeus");
                names[NameType.Complete].Add("295", "Acropolis");
                names[NameType.Complete].Add("296", "Air");
                names[NameType.Complete].Add("297", "Alfheim");
                names[NameType.Complete].Add("298", "All Maps");
                names[NameType.Complete].Add("299", "Anatolia");
                names[NameType.Complete].Add("300", "Archipelago");
                names[NameType.Complete].Add("301", "Arena");
                names[NameType.Complete].Add("302", "Black Sea");
                names[NameType.Complete].Add("303", "Blue Lagoon");
                names[NameType.Complete].Add("304", "Elysium");
                names[NameType.Complete].Add("305", "Erebus");
                names[NameType.Complete].Add("306", "Ghost Lake");
                names[NameType.Complete].Add("307", "Giza");
                names[NameType.Complete].Add("308", "Gold Rush");
                names[NameType.Complete].Add("309", "Highland");
                names[NameType.Complete].Add("310", "Ironwood");
                names[NameType.Complete].Add("311", "Islands");
                names[NameType.Complete].Add("312", "Jotunheim");
                names[NameType.Complete].Add("313", "Kerlaugar");
                names[NameType.Complete].Add("314", "Land Unknown");
                names[NameType.Complete].Add("315", "Mapthumb Land"); names[NameType.Abbreviation].Add("315", "Land Map");
                names[NameType.Complete].Add("316", "Mapthumb Navy"); names[NameType.Abbreviation].Add("316", "Water Map");
                names[NameType.Complete].Add("317", "Mapthumb Standard"); names[NameType.Abbreviation].Add("317", "Standard Map");
                names[NameType.Complete].Add("318", "Marsh");
                names[NameType.Complete].Add("319", "Mediterranean");
                names[NameType.Complete].Add("320", "Megalopolis");
                names[NameType.Complete].Add("321", "Midgard");
                names[NameType.Complete].Add("322", "Mirage");
                names[NameType.Complete].Add("323", "Mirkwood");
                names[NameType.Complete].Add("324", "Mount Olympus");
                names[NameType.Complete].Add("325", "Muspellheim");
                names[NameType.Complete].Add("326", "Nile Shallows");
                names[NameType.Complete].Add("327", "Nomad");
                names[NameType.Complete].Add("328", "Oasis");
                names[NameType.Complete].Add("329", "River Nile");
                names[NameType.Complete].Add("330", "River Styx");
                names[NameType.Complete].Add("331", "Savannah");
                names[NameType.Complete].Add("332", "Sea Of Worms");
                names[NameType.Complete].Add("333", "Team Migration");
                names[NameType.Complete].Add("334", "The Unknown");
                names[NameType.Complete].Add("335", "Tiny");
                names[NameType.Complete].Add("336", "Treasury Black Map");
                names[NameType.Complete].Add("337", "Tundra");
                names[NameType.Complete].Add("338", "Valley Of Kings");
                names[NameType.Complete].Add("339", "Vinlandsaga");
                names[NameType.Complete].Add("340", "Watering Hole");
                names[NameType.Complete].Add("341", "Standard Difficulty");
                names[NameType.Complete].Add("342", "Moderate Difficulty");
                names[NameType.Complete].Add("343", "Hard Difficulty");
                names[NameType.Complete].Add("344", "Titan Difficulty");
                names[NameType.Complete].Add("345", "Area");
                names[NameType.Complete].Add("346", "Crush Armor");
                names[NameType.Complete].Add("347", "Crush Dmg");
                names[NameType.Complete].Add("348", "Divine Dmg");
                names[NameType.Complete].Add("349", "Garrison Stat");
                names[NameType.Complete].Add("350", "Hack Armor");
                names[NameType.Complete].Add("351", "Hack Dmg");
                names[NameType.Complete].Add("352", "Hp");
                names[NameType.Complete].Add("353", "Hp Regen");
                names[NameType.Complete].Add("354", "Pierce Armor");
                names[NameType.Complete].Add("355", "Pierce Dmg");
                names[NameType.Complete].Add("356", "Projectile");
                names[NameType.Complete].Add("357", "Range");
                names[NameType.Complete].Add("358", "Rof");
                names[NameType.Complete].Add("359", "Speed");
                names[NameType.Complete].Add("360", "Time");
                names[NameType.Complete].Add("361", "Arctic Wolf");
                names[NameType.Complete].Add("362", "Aurochs");
                names[NameType.Complete].Add("363", "Baboon");
                names[NameType.Complete].Add("364", "Bear");
                names[NameType.Complete].Add("365", "Berry Bush"); names[NameType.Abbreviation].Add("365", "Berry|B");
                names[NameType.Complete].Add("366", "Boar");
                names[NameType.Complete].Add("367", "Caribou");
                names[NameType.Complete].Add("368", "Chicken"); names[NameType.Abbreviation].Add("368", "CHK");
                names[NameType.Complete].Add("369", "Cow");
                names[NameType.Complete].Add("370", "Crocodile");
                names[NameType.Complete].Add("371", "Crowned Crane");
                names[NameType.Complete].Add("372", "Deer");
                names[NameType.Complete].Add("373", "Elephant"); names[NameType.Abbreviation].Add("373", "Ele");
                names[NameType.Complete].Add("374", "Elk");
                names[NameType.Complete].Add("375", "Fish");
                names[NameType.Complete].Add("376", "Gazelle");
                names[NameType.Complete].Add("377", "Giraffe");
                names[NameType.Complete].Add("378", "Goat");
                names[NameType.Complete].Add("379", "Gold Mine"); names[NameType.Abbreviation].Add("379", "GM");
                names[NameType.Complete].Add("380", "Hippopotamus"); names[NameType.Abbreviation].Add("380", "Hippo");
                names[NameType.Complete].Add("381", "Hyena");
                names[NameType.Complete].Add("382", "Lion");
                names[NameType.Complete].Add("383", "Monkey");
                names[NameType.Complete].Add("384", "Monkey Raft");
                names[NameType.Complete].Add("385", "Pig");
                names[NameType.Complete].Add("386", "Polar Bear");
                names[NameType.Complete].Add("387", "Res Favor"); names[NameType.Abbreviation].Add("387", "FV");
                names[NameType.Complete].Add("388", "Res Food"); names[NameType.Abbreviation].Add("388", "F");
                names[NameType.Complete].Add("389", "Res Gold"); names[NameType.Abbreviation].Add("389", "G");
                names[NameType.Complete].Add("390", "Res Military");
                names[NameType.Complete].Add("391", "Res Pop");
                names[NameType.Complete].Add("392", "Res Population"); names[NameType.Abbreviation].Add("392", "P");
                names[NameType.Complete].Add("393", "Res Wood"); names[NameType.Abbreviation].Add("393", "W");
                names[NameType.Complete].Add("394", "Rhinoceros"); names[NameType.Abbreviation].Add("394", "Rhino");
                names[NameType.Complete].Add("395", "Settlement");
                names[NameType.Complete].Add("396", "Summoning");
                names[NameType.Complete].Add("397", "Tamarisk");
                names[NameType.Complete].Add("398", "Cypress");
                names[NameType.Complete].Add("399", "Cypress Snow");
                names[NameType.Complete].Add("400", "Gaia Tree");
                names[NameType.Complete].Add("401", "Hades Tree");
                names[NameType.Complete].Add("402", "Marsh Tree");
                names[NameType.Complete].Add("403", "Oak Autumn");
                names[NameType.Complete].Add("404", "Oak");
                names[NameType.Complete].Add("405", "Olive");
                names[NameType.Complete].Add("406", "Palm");
                names[NameType.Complete].Add("407", "Straggler"); names[NameType.Abbreviation].Add("407", "STR");
                names[NameType.Complete].Add("408", "Pine Dead");
                names[NameType.Complete].Add("409", "Pine");
                names[NameType.Complete].Add("410", "Pine Snow");
                names[NameType.Complete].Add("411", "Savannah Tree");
                names[NameType.Complete].Add("412", "Tundra Tree");
                names[NameType.Complete].Add("413", "Tundra Snow");
                names[NameType.Complete].Add("414", "Walrus");
                names[NameType.Complete].Add("415", "Water Buffalo");
                names[NameType.Complete].Add("416", "Wolf");
                names[NameType.Complete].Add("417", "Zebra");
                names[NameType.Complete].Add("418", "Adze Of Wepwawet");
                names[NameType.Complete].Add("419", "Aegis Shield");
                names[NameType.Complete].Add("420", "Alluvial Clay");
                names[NameType.Complete].Add("421", "Ambassadors");
                names[NameType.Complete].Add("422", "Anastrophe");
                names[NameType.Complete].Add("423", "Archaic Age");
                names[NameType.Complete].Add("424", "Architects");
                names[NameType.Complete].Add("425", "Arctic Gale");
                names[NameType.Complete].Add("426", "Arctic Winds");
                names[NameType.Complete].Add("427", "Argive Patronage");
                names[NameType.Complete].Add("428", "Argonauts");
                names[NameType.Complete].Add("429", "Asper Blood");
                names[NameType.Complete].Add("430", "Atef Crown");
                names[NameType.Complete].Add("431", "Avenging Spirit");
                names[NameType.Complete].Add("432", "Axe Of Vengeance");
                names[NameType.Complete].Add("433", "Ballista Tower");
                names[NameType.Complete].Add("434", "Ballistics");
                names[NameType.Complete].Add("435", "Beast Slayer");
                names[NameType.Complete].Add("436", "Berserkergang");
                names[NameType.Complete].Add("437", "Bite Of The Shark");
                names[NameType.Complete].Add("438", "Boiling Oil");
                names[NameType.Complete].Add("439", "Bone Bow");
                names[NameType.Complete].Add("440", "Book Of Thoth");
                names[NameType.Complete].Add("441", "Bow Saw");
                names[NameType.Complete].Add("442", "Bravery");
                names[NameType.Complete].Add("443", "Bronze Armor");
                names[NameType.Complete].Add("444", "Bronze Shields");
                names[NameType.Complete].Add("445", "Bronze Wall");
                names[NameType.Complete].Add("446", "Bronze Weapons");
                names[NameType.Complete].Add("447", "Burning Pitch");
                names[NameType.Complete].Add("448", "Call Of Valhalla");
                names[NameType.Complete].Add("449", "Carpenters");
                names[NameType.Complete].Add("450", "Carrier Pigeons");
                names[NameType.Complete].Add("451", "Cave Troll");
                names[NameType.Complete].Add("452", "Celerity");
                names[NameType.Complete].Add("453", "Champion Archers");
                names[NameType.Complete].Add("454", "Champion Axemen");
                names[NameType.Complete].Add("455", "Champion Camel Riders");
                names[NameType.Complete].Add("456", "Champion Cavalry");
                names[NameType.Complete].Add("457", "Champion Chariot Archers");
                names[NameType.Complete].Add("458", "Champion Infantry");
                names[NameType.Complete].Add("459", "Champion Infantry Norse");
                names[NameType.Complete].Add("460", "Champion Slingers");
                names[NameType.Complete].Add("461", "Champion Spearmen");
                names[NameType.Complete].Add("462", "Champion War Elephants");
                names[NameType.Complete].Add("463", "Champion Warships");
                names[NameType.Complete].Add("464", "Channels");
                names[NameType.Complete].Add("465", "Chthonic Rites");
                names[NameType.Complete].Add("466", "Citadel Wall");
                names[NameType.Complete].Add("467", "Clairvoyance");
                names[NameType.Complete].Add("468", "Coinage");
                names[NameType.Complete].Add("469", "Conscript Barracks Soldiers");
                names[NameType.Complete].Add("470", "Conscript Cavalry");
                names[NameType.Complete].Add("471", "Conscript Counter Soldiers");
                names[NameType.Complete].Add("472", "Conscript Great Hall Soldiers");
                names[NameType.Complete].Add("473", "Conscript Hill Fort Soldiers");
                names[NameType.Complete].Add("474", "Conscript Infantry");
                names[NameType.Complete].Add("475", "Conscript Longhouse Soldiers");
                names[NameType.Complete].Add("476", "Conscript Mainline Soldiers");
                names[NameType.Complete].Add("477", "Conscript Migdol Soldiers");
                names[NameType.Complete].Add("478", "Conscript Palace Soldiers");
                names[NameType.Complete].Add("479", "Conscript Ranged Soldiers");
                names[NameType.Complete].Add("480", "Conscript Sailors");
                names[NameType.Complete].Add("481", "Copper Armor");
                names[NameType.Complete].Add("482", "Copper Shields");
                names[NameType.Complete].Add("483", "Copper Weapons");
                names[NameType.Complete].Add("484", "Crenellations");
                names[NameType.Complete].Add("485", "Crimson Linen");
                names[NameType.Complete].Add("486", "Criosphinx");
                names[NameType.Complete].Add("487", "Crocodilopolis");
                names[NameType.Complete].Add("488", "Daktyloi");
                names[NameType.Complete].Add("489", "Dark Water");
                names[NameType.Complete].Add("490", "Daughters Of The Sea");
                names[NameType.Complete].Add("491", "Deimos Sword Of Dread");
                names[NameType.Complete].Add("492", "Desert Wind");
                names[NameType.Complete].Add("493", "Devotees Of Atlas");
                names[NameType.Complete].Add("494", "Dionysia");
                names[NameType.Complete].Add("495", "Disablot");
                names[NameType.Complete].Add("496", "Divine Blood");
                names[NameType.Complete].Add("497", "Draft Horses");
                names[NameType.Complete].Add("498", "Dragonscale Shields");
                names[NameType.Complete].Add("499", "Dwarven Auger");
                names[NameType.Complete].Add("500", "Dwarven Breastplate");
                names[NameType.Complete].Add("501", "Dwarven Weapons");
                names[NameType.Complete].Add("502", "Electrum Bullets");
                names[NameType.Complete].Add("503", "Empyrean Speed");
                names[NameType.Complete].Add("504", "Enclosed Deck");
                names[NameType.Complete].Add("505", "Engineers");
                names[NameType.Complete].Add("506", "Enyos Bow Of Horror");
                names[NameType.Complete].Add("507", "Eyes In The Forest");
                names[NameType.Complete].Add("508", "Face Of The Gorgon");
                names[NameType.Complete].Add("509", "Feasts Of Renown");
                names[NameType.Complete].Add("510", "Feet Of The Jackal");
                names[NameType.Complete].Add("511", "Flames Of Typhon");
                names[NameType.Complete].Add("512", "Flood Control");
                names[NameType.Complete].Add("513", "Flood Of The Nile");
                names[NameType.Complete].Add("514", "Force Of The West Wind");
                names[NameType.Complete].Add("515", "Forge Of Olympus");
                names[NameType.Complete].Add("516", "Fortified Town Center");
                names[NameType.Complete].Add("517", "Fortified Wall Egyptian");
                names[NameType.Complete].Add("518", "Fortified Wall Greek");
                names[NameType.Complete].Add("519", "Freyrs Gift");
                names[NameType.Complete].Add("520", "Frontline Heroics");
                names[NameType.Complete].Add("521", "Funeral Barge");
                names[NameType.Complete].Add("522", "Funeral Rites");
                names[NameType.Complete].Add("523", "Fury Of The Fallen");
                names[NameType.Complete].Add("524", "Gemini");
                names[NameType.Complete].Add("525", "Gjallarhorn");
                names[NameType.Complete].Add("526", "Golden Apples");
                names[NameType.Complete].Add("527", "Granite Blood");
                names[NameType.Complete].Add("528", "Granite Maw");
                names[NameType.Complete].Add("529", "Grasp Of Ran");
                names[NameType.Complete].Add("530", "Greatest Of Fifty");
                names[NameType.Complete].Add("531", "Guard Tower Atlantean");
                names[NameType.Complete].Add("532", "Guard Tower Egyptian");
                names[NameType.Complete].Add("533", "Guard Tower Greek");
                names[NameType.Complete].Add("534", "Guardian Of Io");
                names[NameType.Complete].Add("535", "Hall Of Thanes");
                names[NameType.Complete].Add("536", "Halo Of The Sun");
                names[NameType.Complete].Add("537", "Hamask");
                names[NameType.Complete].Add("538", "Hammer Of Thunder");
                names[NameType.Complete].Add("539", "Hand Axe");
                names[NameType.Complete].Add("540", "Hand Of Talos");
                names[NameType.Complete].Add("541", "Hands Of The Pharaoh");
                names[NameType.Complete].Add("542", "Heart Of The Titans");
                names[NameType.Complete].Add("543", "Heavy Archers");
                names[NameType.Complete].Add("544", "Heavy Axemen");
                names[NameType.Complete].Add("545", "Heavy Camel Riders");
                names[NameType.Complete].Add("546", "Heavy Cavalry");
                names[NameType.Complete].Add("547", "Heavy Chariot Archers");
                names[NameType.Complete].Add("548", "Heavy Infantry");
                names[NameType.Complete].Add("549", "Heavy Infantry Norse");
                names[NameType.Complete].Add("550", "Heavy Slingers");
                names[NameType.Complete].Add("551", "Heavy Spearmen");
                names[NameType.Complete].Add("552", "Heavy War Elephants");
                names[NameType.Complete].Add("553", "Heavy Warships");
                names[NameType.Complete].Add("554", "Hephaestus Revenge");
                names[NameType.Complete].Add("555", "Heroic Fleet");
                names[NameType.Complete].Add("556", "Heroic Renewal");
                names[NameType.Complete].Add("557", "Hieracosphinx");
                names[NameType.Complete].Add("558", "Horns Of Consecration");
                names[NameType.Complete].Add("559", "Hunting Equipment");
                names[NameType.Complete].Add("560", "Huntress Axe");
                names[NameType.Complete].Add("561", "Husbandry");
                names[NameType.Complete].Add("562", "Iron Armor");
                names[NameType.Complete].Add("563", "Iron Shields");
                names[NameType.Complete].Add("564", "Iron Wall");
                names[NameType.Complete].Add("565", "Iron Weapons");
                names[NameType.Complete].Add("566", "Irrigation");
                names[NameType.Complete].Add("567", "Jotuns");
                names[NameType.Complete].Add("568", "Labyrinth Of Minos");
                names[NameType.Complete].Add("569", "Lance Of Stone");
                names[NameType.Complete].Add("570", "Leather Frame Shield");
                names[NameType.Complete].Add("571", "Levy Barracks Soldiers");
                names[NameType.Complete].Add("572", "Levy Cavalry");
                names[NameType.Complete].Add("573", "Levy Counter Soldiers");
                names[NameType.Complete].Add("574", "Levy Great Hall Soldiers");
                names[NameType.Complete].Add("575", "Levy Hill Fort Soldiers");
                names[NameType.Complete].Add("576", "Levy Infantry");
                names[NameType.Complete].Add("577", "Levy Longhouse Soldiers");
                names[NameType.Complete].Add("578", "Levy Mainline Soldiers");
                names[NameType.Complete].Add("579", "Levy Migdol Soldiers");
                names[NameType.Complete].Add("580", "Levy Palace Soldiers");
                names[NameType.Complete].Add("581", "Levy Ranged Soldiers");
                names[NameType.Complete].Add("582", "Long Serpent");
                names[NameType.Complete].Add("583", "Lord Of Horses");
                names[NameType.Complete].Add("584", "Masons");
                names[NameType.Complete].Add("585", "Medium Archers");
                names[NameType.Complete].Add("586", "Medium Axemen");
                names[NameType.Complete].Add("587", "Medium Cavalry");
                names[NameType.Complete].Add("588", "Medium Infantry");
                names[NameType.Complete].Add("589", "Medium Infantry Norse");
                names[NameType.Complete].Add("590", "Medium Slingers");
                names[NameType.Complete].Add("591", "Medium Spearmen");
                names[NameType.Complete].Add("592", "Medjay");
                names[NameType.Complete].Add("593", "Meteoric Iron Armor");
                names[NameType.Complete].Add("594", "Monstrous Rage");
                names[NameType.Complete].Add("595", "Mythic Rejuvenation");
                names[NameType.Complete].Add("596", "Nebty");
                names[NameType.Complete].Add("597", "Necropolis");
                names[NameType.Complete].Add("598", "New Kingdom");
                names[NameType.Complete].Add("599", "Nine Waves");
                names[NameType.Complete].Add("600", "Olympian Parentage");
                names[NameType.Complete].Add("601", "Olympian Weapons");
                names[NameType.Complete].Add("602", "Omniscience");
                names[NameType.Complete].Add("603", "Oracle");
                names[NameType.Complete].Add("604", "Orichalcum Mail");
                names[NameType.Complete].Add("605", "Orichalcum Wall");
                names[NameType.Complete].Add("606", "Perception");
                names[NameType.Complete].Add("607", "Petrification");
                names[NameType.Complete].Add("608", "Phobos Spear Of Panic");
                names[NameType.Complete].Add("609", "Pickaxe");
                names[NameType.Complete].Add("610", "Pioneer Of The Skies");
                names[NameType.Complete].Add("611", "Plow");
                names[NameType.Complete].Add("612", "Poseidons Secret");
                names[NameType.Complete].Add("613", "Prophetic Sight");
                names[NameType.Complete].Add("614", "Purse Seine");
                names[NameType.Complete].Add("615", "Quarry");
                names[NameType.Complete].Add("616", "Rampage");
                names[NameType.Complete].Add("617", "Rheias Gift");
                names[NameType.Complete].Add("618", "Rigsthula");
                names[NameType.Complete].Add("619", "Rime");
                names[NameType.Complete].Add("620", "Ring Giver");
                names[NameType.Complete].Add("621", "Ring Oath");
                names[NameType.Complete].Add("622", "Roar Of Orthus");
                names[NameType.Complete].Add("623", "Sacred Cats");
                names[NameType.Complete].Add("624", "Safeguard");
                names[NameType.Complete].Add("625", "Salt Amphora");
                names[NameType.Complete].Add("626", "Sarissa");
                names[NameType.Complete].Add("627", "Scalloped Axe");
                names[NameType.Complete].Add("628", "Secrets Of The Titans");
                names[NameType.Complete].Add("629", "Serpent Spear");
                names[NameType.Complete].Add("630", "Servants Of Glory");
                names[NameType.Complete].Add("631", "Sessrumnir");
                names[NameType.Complete].Add("632", "Shaduf");
                names[NameType.Complete].Add("633", "Shaft Mine");
                names[NameType.Complete].Add("634", "Shafts Of Plague");
                names[NameType.Complete].Add("635", "Shoulder Of Talos");
                names[NameType.Complete].Add("636", "Signal Fires");
                names[NameType.Complete].Add("637", "Silent Resolve");
                names[NameType.Complete].Add("638", "Skin Of The Rhino");
                names[NameType.Complete].Add("639", "Slings Of The Sun");
                names[NameType.Complete].Add("640", "Solar Barque");
                names[NameType.Complete].Add("641", "Sons Of Sleipnir");
                names[NameType.Complete].Add("642", "Sons Of The Sun");
                names[NameType.Complete].Add("643", "Spear Of Horus");
                names[NameType.Complete].Add("644", "Spirit Of Maat");
                names[NameType.Complete].Add("645", "Spirited Charge");
                names[NameType.Complete].Add("646", "Stone Wall Atlantean"); names[NameType.Abbreviation].Add("646", "SWA");
                names[NameType.Complete].Add("647", "Stone Wall Egyptian"); names[NameType.Abbreviation].Add("647", "SWE");
                names[NameType.Complete].Add("648", "Stone Wall Greek"); names[NameType.Abbreviation].Add("648", "SWG");
                names[NameType.Complete].Add("649", "Stone Wall Norse"); names[NameType.Abbreviation].Add("649", "SWN");
                names[NameType.Complete].Add("650", "Sun Ray");
                names[NameType.Complete].Add("651", "Sundried Mud Brick");
                names[NameType.Complete].Add("652", "Swine Array");
                names[NameType.Complete].Add("653", "Sylvan Lore");
                names[NameType.Complete].Add("654", "Tax Collectors");
                names[NameType.Complete].Add("655", "Temple Of Healing");
                names[NameType.Complete].Add("656", "Temporal Chaos");
                names[NameType.Complete].Add("657", "Theft Of Fire");
                names[NameType.Complete].Add("658", "Thracian Horses");
                names[NameType.Complete].Add("659", "Thundering Hooves");
                names[NameType.Complete].Add("660", "Thurisaz Rune");
                names[NameType.Complete].Add("661", "Titan Shield");
                names[NameType.Complete].Add("662", "Titanomachy");
                names[NameType.Complete].Add("663", "Tusks Of Apedemak");
                names[NameType.Complete].Add("664", "Twilight Of The Gods");
                names[NameType.Complete].Add("665", "Valgaldr");
                names[NameType.Complete].Add("666", "Valley Of The Kings");
                names[NameType.Complete].Add("667", "Vaults Of Erebus");
                names[NameType.Complete].Add("668", "Vikings");
                names[NameType.Complete].Add("669", "Volcanic Forge");
                names[NameType.Complete].Add("670", "Watch Tower Atlantean"); names[NameType.Abbreviation].Add("670", "WTA");
                names[NameType.Complete].Add("671", "Watch Tower Egyptian"); names[NameType.Abbreviation].Add("671", "WTE");
                names[NameType.Complete].Add("672", "Watch Tower Greek"); names[NameType.Abbreviation].Add("672", "WTG");
                names[NameType.Complete].Add("673", "Watch Tower Norse"); names[NameType.Abbreviation].Add("673", "WTN");
                names[NameType.Complete].Add("674", "Weightless Mace");
                names[NameType.Complete].Add("675", "Will Of Kronos");
                names[NameType.Complete].Add("676", "Winged Messenger");
                names[NameType.Complete].Add("677", "Winter Harvest");
                names[NameType.Complete].Add("678", "Wrath Of The Deep");
                names[NameType.Complete].Add("679", "Ydalir");
                names[NameType.Complete].Add("680", "Achilles");
                names[NameType.Complete].Add("681", "Ajax");
                names[NameType.Complete].Add("682", "Anubite");
                names[NameType.Complete].Add("683", "Arctic Wolf Of Set");
                names[NameType.Complete].Add("684", "Arcus Hero");
                names[NameType.Complete].Add("685", "Arcus");
                names[NameType.Complete].Add("686", "Argus");
                names[NameType.Complete].Add("687", "Atalanta");
                names[NameType.Complete].Add("688", "Aurochs Of Set");
                names[NameType.Complete].Add("689", "Automaton");
                names[NameType.Complete].Add("690", "Avenger");
                names[NameType.Complete].Add("691", "Axeman");
                names[NameType.Complete].Add("692", "Baboon Of Set");
                names[NameType.Complete].Add("693", "Ballista");
                names[NameType.Complete].Add("694", "Battle Boar");
                names[NameType.Complete].Add("695", "Bear Of Set");
                names[NameType.Complete].Add("696", "Behemoth");
                names[NameType.Complete].Add("697", "Bellerophon");
                names[NameType.Complete].Add("698", "Berserk"); names[NameType.Abbreviation].Add("698", "BK");
                names[NameType.Complete].Add("699", "Bireme");
                names[NameType.Complete].Add("700", "Boar Of Set");
                names[NameType.Complete].Add("701", "Caladria");
                names[NameType.Complete].Add("702", "Camel Rider");
                names[NameType.Complete].Add("703", "Caravan Atlantean");
                names[NameType.Complete].Add("704", "Caravan Egyptian");
                names[NameType.Complete].Add("705", "Caravan Greek");
                names[NameType.Complete].Add("706", "Caravan Norse");
                names[NameType.Complete].Add("707", "Carcinos");
                names[NameType.Complete].Add("708", "Caribou Of Set");
                names[NameType.Complete].Add("709", "Carnivora Unit");
                names[NameType.Complete].Add("710", "Catapult");
                names[NameType.Complete].Add("711", "Centaur");
                names[NameType.Complete].Add("712", "Centimanus");
                names[NameType.Complete].Add("713", "Chariot Archer");
                names[NameType.Complete].Add("714", "Cheiroballista Hero");
                names[NameType.Complete].Add("715", "Cheiroballista");
                names[NameType.Complete].Add("716", "Chicken Of Set");
                names[NameType.Complete].Add("717", "Chimera");
                names[NameType.Complete].Add("718", "Chiron");
                names[NameType.Complete].Add("719", "Colossus");
                names[NameType.Complete].Add("720", "Contarius Hero");
                names[NameType.Complete].Add("721", "Contarius");
                names[NameType.Complete].Add("722", "Crocodile Of Set");
                names[NameType.Complete].Add("723", "Crowned Crane Of Set");
                names[NameType.Complete].Add("724", "Cyclops");
                names[NameType.Complete].Add("725", "Deer Of Set");
                names[NameType.Complete].Add("726", "Destroyer Hero");
                names[NameType.Complete].Add("727", "Destroyer");
                names[NameType.Complete].Add("728", "Dragon Ship");
                names[NameType.Complete].Add("729", "Draugr");
                names[NameType.Complete].Add("730", "Dreki");
                names[NameType.Complete].Add("731", "Dryad");
                names[NameType.Complete].Add("732", "Einheri");
                names[NameType.Complete].Add("733", "Elephant Of Set");
                names[NameType.Complete].Add("734", "Elk Of Set");
                names[NameType.Complete].Add("735", "Fafnir");
                names[NameType.Complete].Add("736", "Fanatic Hero");
                names[NameType.Complete].Add("737", "Fanatic");
                names[NameType.Complete].Add("738", "Fenris Wolf Brood");
                names[NameType.Complete].Add("739", "Fimbulwinter Wolf");
                names[NameType.Complete].Add("740", "Fire Giant");
                names[NameType.Complete].Add("741", "Fire Ship");
                names[NameType.Complete].Add("742", "Fire Siphon");
                names[NameType.Complete].Add("743", "Fishing Ship Atlantean"); names[NameType.Abbreviation].Add("743", "FSA");
                names[NameType.Complete].Add("744", "Fishing Ship Egyptian"); names[NameType.Abbreviation].Add("744", "FSE");
                names[NameType.Complete].Add("745", "Fishing Ship Greek"); names[NameType.Abbreviation].Add("745", "FSG");
                names[NameType.Complete].Add("746", "Fishing Ship Norse"); names[NameType.Abbreviation].Add("746", "FSN");
                names[NameType.Complete].Add("747", "Frost Giant");
                names[NameType.Complete].Add("748", "Gastraphetoros");
                names[NameType.Complete].Add("749", "Gazelle Of Set");
                names[NameType.Complete].Add("750", "Giraffe Of Set");
                names[NameType.Complete].Add("751", "Godi");
                names[NameType.Complete].Add("752", "Gullinbursti Unit");
                names[NameType.Complete].Add("753", "Hades Shade");
                names[NameType.Complete].Add("754", "Helepolis");
                names[NameType.Complete].Add("755", "Heracles");
                names[NameType.Complete].Add("756", "Hero Of Ragnarok Dwarf");
                names[NameType.Complete].Add("757", "Hero Of Ragnarok");
                names[NameType.Complete].Add("758", "Hersir");
                names[NameType.Complete].Add("759", "Hetairos");
                names[NameType.Complete].Add("760", "Hippeus");
                names[NameType.Complete].Add("761", "Hippocampus");
                names[NameType.Complete].Add("762", "Hippolyta");
                names[NameType.Complete].Add("763", "Hippopotamus Of Set");
                names[NameType.Complete].Add("764", "Hirdman");
                names[NameType.Complete].Add("765", "Hoplite");
                names[NameType.Complete].Add("766", "Huskarl");
                names[NameType.Complete].Add("767", "Hydra");
                names[NameType.Complete].Add("768", "Hyena Of Set");
                names[NameType.Complete].Add("769", "Hypaspist");
                names[NameType.Complete].Add("770", "Jarl");
                names[NameType.Complete].Add("771", "Jason");
                names[NameType.Complete].Add("772", "Jormun Elver");
                names[NameType.Complete].Add("773", "Juggernaut");
                names[NameType.Complete].Add("774", "Katapeltes Hero");
                names[NameType.Complete].Add("775", "Katapeltes");
                names[NameType.Complete].Add("776", "Kataskopos"); names[NameType.Abbreviation].Add("776", "KK");
                names[NameType.Complete].Add("777", "Kebenit");
                names[NameType.Complete].Add("778", "Kraken");
                names[NameType.Complete].Add("779", "Lampades");
                names[NameType.Complete].Add("780", "Leviathan");
                names[NameType.Complete].Add("781", "Lion Of Set");
                names[NameType.Complete].Add("782", "Longboat");
                names[NameType.Complete].Add("783", "Lost Ship");
                names[NameType.Complete].Add("784", "Man O War");
                names[NameType.Complete].Add("785", "Manticore");
                names[NameType.Complete].Add("786", "Medusa");
                names[NameType.Complete].Add("787", "Mercenary Cavalry");
                names[NameType.Complete].Add("788", "Mercenary");
                names[NameType.Complete].Add("789", "Militia");
                names[NameType.Complete].Add("790", "Minion");
                names[NameType.Complete].Add("791", "Minotaur"); names[NameType.Abbreviation].Add("791", "MT");
                names[NameType.Complete].Add("792", "Monkey Of Set");
                names[NameType.Complete].Add("793", "Mountain Giant");
                names[NameType.Complete].Add("794", "Mummy");
                names[NameType.Complete].Add("795", "Murmillo Hero");
                names[NameType.Complete].Add("796", "Murmillo");
                names[NameType.Complete].Add("797", "Myrmidon");
                names[NameType.Complete].Add("798", "Nemean Lion");
                names[NameType.Complete].Add("799", "Nereid");
                names[NameType.Complete].Add("800", "Nidhogg Unit");
                names[NameType.Complete].Add("801", "Odysseus");
                names[NameType.Complete].Add("802", "Oracle Hero");
                names[NameType.Complete].Add("803", "Oracle Unit"); names[NameType.Abbreviation].Add("803", "O");
                names[NameType.Complete].Add("804", "Ox Cart"); names[NameType.Abbreviation].Add("804", "Ox");
                names[NameType.Complete].Add("805", "Pegasus");
                names[NameType.Complete].Add("806", "Peltast");
                names[NameType.Complete].Add("807", "Pentekonter");
                names[NameType.Complete].Add("808", "Perseus");
                names[NameType.Complete].Add("809", "Petrobolos");
                names[NameType.Complete].Add("810", "Petsuchos");
                names[NameType.Complete].Add("811", "Pharaoh"); names[NameType.Abbreviation].Add("811", "PH");
                names[NameType.Complete].Add("812", "Phoenix Egg");
                names[NameType.Complete].Add("813", "Phoenix");
                names[NameType.Complete].Add("814", "Polar Bear Of Set");
                names[NameType.Complete].Add("815", "Polyphemus");
                names[NameType.Complete].Add("816", "Portable Ram");
                names[NameType.Complete].Add("817", "Priest"); names[NameType.Abbreviation].Add("817", "PR");
                names[NameType.Complete].Add("818", "Prodromos");
                names[NameType.Complete].Add("819", "Promethean");
                names[NameType.Complete].Add("820", "Promethean Offspring");
                names[NameType.Complete].Add("821", "Raiding Cavalry");
                names[NameType.Complete].Add("822", "Ramming Galley");
                names[NameType.Complete].Add("823", "Raven");
                names[NameType.Complete].Add("824", "Rhinoceros Of Set");
                names[NameType.Complete].Add("825", "Roc");
                names[NameType.Complete].Add("826", "Rock Giant");
                names[NameType.Complete].Add("827", "Satyr");
                names[NameType.Complete].Add("828", "Scarab");
                names[NameType.Complete].Add("829", "Scorpion Man");
                names[NameType.Complete].Add("830", "Scylla");
                names[NameType.Complete].Add("831", "Sea Snake");
                names[NameType.Complete].Add("832", "Sentinel Unit");
                names[NameType.Complete].Add("833", "Serpent");
                names[NameType.Complete].Add("834", "Servant");
                names[NameType.Complete].Add("835", "Siege Bireme");
                names[NameType.Complete].Add("836", "Siege Tower");
                names[NameType.Complete].Add("837", "Slinger");
                names[NameType.Complete].Add("838", "Son Of Osiris Unit");
                names[NameType.Complete].Add("839", "Spearman");
                names[NameType.Complete].Add("840", "Sphinx");
                names[NameType.Complete].Add("841", "Spider Egg");
                names[NameType.Complete].Add("842", "Stymphalian Bird");
                names[NameType.Complete].Add("843", "Tartarian Spawn");
                names[NameType.Complete].Add("844", "The Argo");
                names[NameType.Complete].Add("845", "Theseus");
                names[NameType.Complete].Add("846", "Throwing Axeman"); names[NameType.Abbreviation].Add("846", "TA");
                names[NameType.Complete].Add("847", "Titan Atlantean");
                names[NameType.Complete].Add("848", "Titan Egyptian");
                names[NameType.Complete].Add("849", "Titan Greek");
                names[NameType.Complete].Add("850", "Titan Norse");
                names[NameType.Complete].Add("851", "Toxotes"); names[NameType.Abbreviation].Add("851", "tx");
                names[NameType.Complete].Add("852", "Transport Ship Atlantean");
                names[NameType.Complete].Add("853", "Transport Ship Egyptian");
                names[NameType.Complete].Add("854", "Transport Ship Greek");
                names[NameType.Complete].Add("855", "Transport Ship Norse");
                names[NameType.Complete].Add("856", "Trireme");
                names[NameType.Complete].Add("857", "Troll");
                names[NameType.Complete].Add("858", "Turma Hero");
                names[NameType.Complete].Add("859", "Turma");
                names[NameType.Complete].Add("860", "Archer");
                names[NameType.Complete].Add("861", "Archer Ship");
                names[NameType.Complete].Add("862", "Building");
                names[NameType.Complete].Add("863", "Cavalry");
                names[NameType.Complete].Add("864", "Close Combat Ship");
                names[NameType.Complete].Add("865", "Flying Unit");
                names[NameType.Complete].Add("866", "Hero");
                names[NameType.Complete].Add("867", "Human Soldier");
                names[NameType.Complete].Add("868", "Infantry");
                names[NameType.Complete].Add("869", "Myth Unit");
                names[NameType.Complete].Add("870", "Ship");
                names[NameType.Complete].Add("871", "Siege Ship");
                names[NameType.Complete].Add("872", "Siege Weapon");
                names[NameType.Complete].Add("873", "Titan");
                names[NameType.Complete].Add("874", "Tower");
                names[NameType.Complete].Add("875", "Villager");
                names[NameType.Complete].Add("876", "Wall");
                names[NameType.Complete].Add("877", "Valkyrie");
                names[NameType.Complete].Add("878", "Villager Atlantean Hero"); names[NameType.Abbreviation].Add("878", "Citizen Hero|CH");
                names[NameType.Complete].Add("879", "Villager Atlantean"); names[NameType.Abbreviation].Add("879", "Citizen|C");
                names[NameType.Complete].Add("880", "Villager Dwarf"); names[NameType.Abbreviation].Add("880", "Dwarf|D");
                names[NameType.Complete].Add("881", "Villager Egyptian"); names[NameType.Abbreviation].Add("881", "Laborer|L");
                names[NameType.Complete].Add("882", "Villager Greek"); names[NameType.Abbreviation].Add("882", "Villager|V");
                names[NameType.Complete].Add("883", "Villager Norse"); names[NameType.Abbreviation].Add("883", "Gatherer|GT");
                names[NameType.Complete].Add("884", "Wadjet");
                names[NameType.Complete].Add("885", "Walking Woods Unit");
                names[NameType.Complete].Add("886", "Walrus Of Set");
                names[NameType.Complete].Add("887", "War Barge");
                names[NameType.Complete].Add("888", "War Elephant");
                names[NameType.Complete].Add("889", "War Turtle");
                names[NameType.Complete].Add("890", "Water Buffalo Of Set");
                names[NameType.Complete].Add("891", "Water Carnivora");
                names[NameType.Complete].Add("892", "Wolf Of Set");
                names[NameType.Complete].Add("893", "Zebra Of Set");
                names[NameType.Complete].Add("894", "Auto Scout Ability"); names[NameType.Abbreviation].Add("894", "Auto Scout|as");
                names[NameType.Complete].Add("895", "Idle");
                names[NameType.Complete].Add("896", "1S"); names[NameType.Abbreviation].Add("896", " ");
                names[NameType.Complete].Add("897", "2S"); names[NameType.Abbreviation].Add("897", "  ");
                names[NameType.Complete].Add("898", "3S"); names[NameType.Abbreviation].Add("898", "   ");
                names[NameType.Complete].Add("899", "4S"); names[NameType.Abbreviation].Add("899", "    ");
                names[NameType.Complete].Add("900", "5S"); names[NameType.Abbreviation].Add("900", "     ");
                names[NameType.Complete].Add("901", "6S"); names[NameType.Abbreviation].Add("901", "      ");
                names[NameType.Complete].Add("902", "7S"); names[NameType.Abbreviation].Add("902", "       ");
                names[NameType.Complete].Add("903", "8S"); names[NameType.Abbreviation].Add("903", "        ");
                names[NameType.Complete].Add("904", "9S"); names[NameType.Abbreviation].Add("904", "         ");
                names[NameType.Complete].Add("905", "10S"); names[NameType.Abbreviation].Add("905", "          ");
                names[NameType.Complete].Add("906", "Less Than"); names[NameType.Abbreviation].Add("906", "<");
                names[NameType.Complete].Add("907", "Open Square Bracket"); names[NameType.Abbreviation].Add("907", "[");
                names[NameType.Complete].Add("908", "Close Square Bracket"); names[NameType.Abbreviation].Add("908", "]");
                names[NameType.Complete].Add("909", "0S"); names[NameType.Abbreviation].Add("909", "");
                names[NameType.Complete].Add("910", "Up");
                names[NameType.Complete].Add("911", "Right");
                names[NameType.Complete].Add("912", "Left");
                names[NameType.Complete].Add("913", "Down");
                names[NameType.Complete].Add("914", "Question");
                names[NameType.Complete].Add("915", "Atlanteans");
                names[NameType.Complete].Add("916", "Egyptians");
                names[NameType.Complete].Add("917", "Greeks");
                names[NameType.Complete].Add("918", "Norse");
                names[NameType.Complete].Add("919", "Atlanteans Circular");
                names[NameType.Complete].Add("920", "Egyptians Circular");
                names[NameType.Complete].Add("921", "Greeks Circular");
                names[NameType.Complete].Add("922", "Norse Circular");
                names[NameType.Complete].Add("923", "Woodline"); names[NameType.Abbreviation].Add("923", "wl");
                names[NameType.Complete].Add("924", "Pray");
                names[NameType.Complete].Add("925", "Explore"); names[NameType.Abbreviation].Add("925", "EXP");
                names[NameType.Complete].Add("926", "Autoqueue"); names[NameType.Abbreviation].Add("926", "aq");
                names[NameType.Complete].Add("927", "Relic");
                names[NameType.Complete].Add("928", "Axe Cart");
                names[NameType.Complete].Add("929", "Baihu");
                names[NameType.Complete].Add("930", "Caravan Chinese");
                names[NameType.Complete].Add("931", "Chiwen");
                names[NameType.Complete].Add("932", "Chu Ko Nu");
                names[NameType.Complete].Add("933", "Dao Swordsman");
                names[NameType.Complete].Add("934", "Doujian");
                names[NameType.Complete].Add("935", "Enchanted");
                names[NameType.Complete].Add("936", "Fei");
                names[NameType.Complete].Add("937", "Fire Archer");
                names[NameType.Complete].Add("938", "Fishing Ship Chinese"); names[NameType.Abbreviation].Add("938", "FSCH");
                names[NameType.Complete].Add("939", "Ge Halberdier");
                names[NameType.Complete].Add("940", "Gebing Double");
                names[NameType.Complete].Add("941", "Gongqi");
                names[NameType.Complete].Add("942", "Hundun");
                names[NameType.Complete].Add("943", "Jiang Ziya");
                names[NameType.Complete].Add("944", "Kuafu Hero"); names[NameType.Abbreviation].Add("944", "KH");
                names[NameType.Complete].Add("945", "Kuafu"); names[NameType.Abbreviation].Add("945", "K");
                names[NameType.Complete].Add("946", "Li Jing");
                names[NameType.Complete].Add("947", "Louchuan");
                names[NameType.Complete].Add("948", "Mengchong");
                names[NameType.Complete].Add("949", "Nezha Child");
                names[NameType.Complete].Add("950", "Nezha");
                names[NameType.Complete].Add("951", "Nezha Youth");
                names[NameType.Complete].Add("952", "Pioneer");
                names[NameType.Complete].Add("953", "Pixiu");
                names[NameType.Complete].Add("954", "Qilin");
                names[NameType.Complete].Add("955", "Qinglong");
                names[NameType.Complete].Add("956", "Qingqi Double");
                names[NameType.Complete].Add("957", "Qingqi");
                names[NameType.Complete].Add("958", "Qiongqi");
                names[NameType.Complete].Add("959", "Sage");
                names[NameType.Complete].Add("960", "Siege Crossbow");
                names[NameType.Complete].Add("961", "Sky Lantern");
                names[NameType.Complete].Add("962", "Taotie");
                names[NameType.Complete].Add("963", "Taowu");
                names[NameType.Complete].Add("964", "Terracotta Rider");
                names[NameType.Complete].Add("965", "Tiger Cavalry Dismounted");
                names[NameType.Complete].Add("966", "Tiger Cavalry");
                names[NameType.Complete].Add("967", "Titan Chinese");
                names[NameType.Complete].Add("968", "Transport Ship Chinese");
                names[NameType.Complete].Add("969", "Villager Chinese Clay");
                names[NameType.Complete].Add("970", "Villager Chinese Female");
                names[NameType.Complete].Add("971", "Villager Chinese"); names[NameType.Abbreviation].Add("971", "Peasant|PS");
                names[NameType.Complete].Add("972", "Villager Chinese Male");
                names[NameType.Complete].Add("973", "Wen Zhong");
                names[NameType.Complete].Add("974", "White Horse Cavalry");
                names[NameType.Complete].Add("975", "Wuzu Javelineer");
                names[NameType.Complete].Add("976", "Xuanwu");
                names[NameType.Complete].Add("977", "Yang Jian");
                names[NameType.Complete].Add("978", "Yazi");
                names[NameType.Complete].Add("979", "Yinglong");
                names[NameType.Complete].Add("980", "Zhuque");
                names[NameType.Complete].Add("981", "Archery Training Module"); names[NameType.Abbreviation].Add("981", "ATM");
                names[NameType.Complete].Add("982", "Baolei");
                names[NameType.Complete].Add("983", "Cavalry Training"); names[NameType.Abbreviation].Add("983", "CT");
                names[NameType.Complete].Add("984", "Citadel Center Chinese"); names[NameType.Abbreviation].Add("984", "CCCH");
                names[NameType.Complete].Add("985", "Earth Wall");
                names[NameType.Complete].Add("986", "Elite Training Module"); names[NameType.Abbreviation].Add("986", "ETM");
                names[NameType.Complete].Add("987", "Farm Rice"); names[NameType.Abbreviation].Add("987", "FR");
                names[NameType.Complete].Add("988", "Farm Shennong"); names[NameType.Abbreviation].Add("988", "FS");
                names[NameType.Complete].Add("989", "Imperial Academy"); names[NameType.Abbreviation].Add("989", "IA");
                names[NameType.Complete].Add("990", "Infantry Training Module"); names[NameType.Abbreviation].Add("990", "ITM");
                names[NameType.Complete].Add("991", "Machine Workshop"); names[NameType.Abbreviation].Add("991", "MW");
                names[NameType.Complete].Add("992", "Military Camp"); names[NameType.Abbreviation].Add("992", "MILC");
                names[NameType.Complete].Add("993", "Silo");
                names[NameType.Complete].Add("994", "The Peach Blossom Spring");
                names[NameType.Complete].Add("995", "Tower Camp"); names[NameType.Abbreviation].Add("995", "TWRC");
                names[NameType.Complete].Add("996", "Town Center Chinese"); names[NameType.Abbreviation].Add("996", "TCCH");
                names[NameType.Complete].Add("997", "Training Camp"); names[NameType.Abbreviation].Add("997", "TRC");
                names[NameType.Complete].Add("998", "Village Center Chinese"); names[NameType.Abbreviation].Add("998", "VCCH");
                names[NameType.Complete].Add("999", "Blazing Prairie");
                names[NameType.Complete].Add("1000", "Creation");
                names[NameType.Complete].Add("1001", "Drought Land");
                names[NameType.Complete].Add("1002", "Earth Wall Power");
                names[NameType.Complete].Add("1003", "Forest Protection");
                names[NameType.Complete].Add("1004", "Great Flood");
                names[NameType.Complete].Add("1005", "Lightning Weapons");
                names[NameType.Complete].Add("1006", "Prosperous Seeds");
                names[NameType.Complete].Add("1007", "Shennong Gift All");
                names[NameType.Complete].Add("1008", "Shennong Gift Classical");
                names[NameType.Complete].Add("1009", "Shennong Gift Heroic");
                names[NameType.Complete].Add("1010", "Shennong Gift Mythic");
                names[NameType.Complete].Add("1011", "The Peach Blossom Spring Power");
                names[NameType.Complete].Add("1012", "Vanish");
                names[NameType.Complete].Add("1013", "Venom Beast");
                names[NameType.Complete].Add("1014", "Yinglongs Wrath");
                names[NameType.Complete].Add("1015", "A Bumper Grain Harvest");
                names[NameType.Complete].Add("1016", "Abundance");
                names[NameType.Complete].Add("1017", "Advanced Defenses");
                names[NameType.Complete].Add("1018", "Autumn Of Abundance");
                names[NameType.Complete].Add("1019", "Bottomless Stomach");
                names[NameType.Complete].Add("1020", "Celestial Weapons");
                names[NameType.Complete].Add("1021", "Champion Baimayicong");
                names[NameType.Complete].Add("1022", "Champion Chukonu");
                names[NameType.Complete].Add("1023", "Champion Gebing");
                names[NameType.Complete].Add("1024", "Champion Gongqi");
                names[NameType.Complete].Add("1025", "Champion Gongshou");
                names[NameType.Complete].Add("1026", "Champion Hubaoqi");
                names[NameType.Complete].Add("1027", "Champion Infantry Chinese");
                names[NameType.Complete].Add("1028", "Champion Jinwei");
                names[NameType.Complete].Add("1029", "Champion Qingqi");
                names[NameType.Complete].Add("1030", "Champion Wuzu");
                names[NameType.Complete].Add("1031", "Chasing The Sun");
                names[NameType.Complete].Add("1032", "Conscript Baolei Soldiers");
                names[NameType.Complete].Add("1033", "Divine Books");
                names[NameType.Complete].Add("1034", "Divine Judgement");
                names[NameType.Complete].Add("1035", "Divine Light");
                names[NameType.Complete].Add("1036", "Drought Ships");
                names[NameType.Complete].Add("1037", "East Wind");
                names[NameType.Complete].Add("1038", "Fish Basket");
                names[NameType.Complete].Add("1039", "Flaming Blood");
                names[NameType.Complete].Add("1040", "Fortified Wall Chinese");
                names[NameType.Complete].Add("1041", "Frenzied Dash");
                names[NameType.Complete].Add("1042", "Fusillade Tower");
                names[NameType.Complete].Add("1043", "Giants Favor");
                names[NameType.Complete].Add("1044", "Gilded Shields");
                names[NameType.Complete].Add("1045", "Great Wall");
                names[NameType.Complete].Add("1046", "Guard Tower Chinese");
                names[NameType.Complete].Add("1047", "Heavy Gebing");
                names[NameType.Complete].Add("1048", "Heavy Gongqi");
                names[NameType.Complete].Add("1049", "Heavy Gongshou");
                names[NameType.Complete].Add("1050", "Heavy Hubaoqi");
                names[NameType.Complete].Add("1051", "Heavy Infantry Chinese");
                names[NameType.Complete].Add("1052", "Heavy Qingqi");
                names[NameType.Complete].Add("1053", "Heavy Wuzu");
                names[NameType.Complete].Add("1054", "Herbal Medicine");
                names[NameType.Complete].Add("1055", "Hooves Of The Wind");
                names[NameType.Complete].Add("1056", "Imperial Order");
                names[NameType.Complete].Add("1057", "Kuafu Chieftain");
                names[NameType.Complete].Add("1058", "Land Consolidation");
                names[NameType.Complete].Add("1059", "Last Stand");
                names[NameType.Complete].Add("1060", "Leizus Silk");
                names[NameType.Complete].Add("1061", "Levy Baolei Soldiers");
                names[NameType.Complete].Add("1062", "Longevity Blessing");
                names[NameType.Complete].Add("1063", "Maelstrom");
                names[NameType.Complete].Add("1064", "Master Of Weaponry");
                names[NameType.Complete].Add("1065", "Medium Gebing");
                names[NameType.Complete].Add("1066", "Medium Gongshou");
                names[NameType.Complete].Add("1067", "Medium Infantry Chinese");
                names[NameType.Complete].Add("1068", "Medium Qingqi");
                names[NameType.Complete].Add("1069", "Might Of Destruction");
                names[NameType.Complete].Add("1070", "Opportune Time");
                names[NameType.Complete].Add("1071", "Peach Of Immortality");
                names[NameType.Complete].Add("1072", "Power Of Chaos");
                names[NameType.Complete].Add("1073", "Qilins Blessing");
                names[NameType.Complete].Add("1074", "Rage Of Slaughter");
                names[NameType.Complete].Add("1075", "Red Cliffs Fleet");
                names[NameType.Complete].Add("1076", "Reincarnation");
                names[NameType.Complete].Add("1077", "Rising Tide");
                names[NameType.Complete].Add("1078", "Rock Solid");
                names[NameType.Complete].Add("1079", "Scorching Feathers");
                names[NameType.Complete].Add("1080", "Shaker Of Heaven");
                names[NameType.Complete].Add("1081", "Shield Blessing");
                names[NameType.Complete].Add("1082", "Silk Road");
                names[NameType.Complete].Add("1083", "Sinister Defiance");
                names[NameType.Complete].Add("1084", "Sky Fire");
                names[NameType.Complete].Add("1085", "Slash And Burn");
                names[NameType.Complete].Add("1086", "Son Of Loong");
                names[NameType.Complete].Add("1087", "Song Of Midsummer");
                names[NameType.Complete].Add("1088", "Southern Fire");
                names[NameType.Complete].Add("1089", "Spoils Of War");
                names[NameType.Complete].Add("1090", "Stone Wall Chinese"); names[NameType.Abbreviation].Add("1090", "SWCH");
                names[NameType.Complete].Add("1091", "Tai Chi");
                names[NameType.Complete].Add("1092", "Tempestuous Storm");
                names[NameType.Complete].Add("1093", "Temple Of Heaven");
                names[NameType.Complete].Add("1094", "Terracotta Riders");
                names[NameType.Complete].Add("1095", "Trading Season");
                names[NameType.Complete].Add("1096", "Vibrant Land");
                names[NameType.Complete].Add("1097", "Watch Tower Chinese"); names[NameType.Abbreviation].Add("1097", "WTCH");
                names[NameType.Complete].Add("1098", "Xuanyuans Bloodline");
                names[NameType.Complete].Add("1099", "Yang");
                names[NameType.Complete].Add("1100", "Yin");
                names[NameType.Complete].Add("1101", "Yin Yang");
                names[NameType.Complete].Add("1102", "Fuxi");
                names[NameType.Complete].Add("1103", "Nuwa");
                names[NameType.Complete].Add("1104", "Shennong");
                names[NameType.Complete].Add("1105", "Chiyou");
                names[NameType.Complete].Add("1106", "Gonggong");
                names[NameType.Complete].Add("1107", "Goumang");
                names[NameType.Complete].Add("1108", "Houtu");
                names[NameType.Complete].Add("1109", "Huangdi");
                names[NameType.Complete].Add("1110", "Nuba");
                names[NameType.Complete].Add("1111", "Rushou");
                names[NameType.Complete].Add("1112", "Xuannu");
                names[NameType.Complete].Add("1113", "Zhurong");
                names[NameType.Complete].Add("1114", "Bamboo");
                names[NameType.Complete].Add("1115", "Chinese Pine Dead");
                names[NameType.Complete].Add("1116", "Chinese Pine");
                names[NameType.Complete].Add("1117", "Ginkgo Autumn");
                names[NameType.Complete].Add("1118", "Ginkgo");
                names[NameType.Complete].Add("1119", "Metasequoia Autumn");
                names[NameType.Complete].Add("1120", "Metasequoia");
                names[NameType.Complete].Add("1121", "Peach");
                names[NameType.Complete].Add("1122", "Pear");
                names[NameType.Complete].Add("1123", "Willow");
                names[NameType.Complete].Add("1124", "Black Bear");
                names[NameType.Complete].Add("1125", "Golden Pheasant");
                names[NameType.Complete].Add("1126", "Panda");
                names[NameType.Complete].Add("1127", "Red Crowned Crane");
                names[NameType.Complete].Add("1128", "Spotted Deer");
                names[NameType.Complete].Add("1129", "Turkey");
                names[NameType.Complete].Add("1130", "Bamboo Grove");
                names[NameType.Complete].Add("1131", "Great Wall Map");
                names[NameType.Complete].Add("1132", "Peach Blossom Land");
                names[NameType.Complete].Add("1133", "Qinghai Lake");
                names[NameType.Complete].Add("1134", "Silk Road Map");
                names[NameType.Complete].Add("1135", "Steppe");
                names[NameType.Complete].Add("1136", "Yellow River");
                names[NameType.Complete].Add("1137", "Chinese");

            } // AOM>

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
        public static Segmento ObtenerSegmentoEfectivo(Entidad entidad, out string? errores, int? númeroPaso) {

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

                        var segmento = new Segmento(imagen, null, TipoSegmento.Imagen, null, out string? erroresInternos, númeroPaso);
                        AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
                        return segmento;

                    }

                } else {

                    if (entidad.Nombres.ContainsKey(tipoNombre)) {

                        var textoSegmento = entidad.Nombres[tipoNombre];
                        var segmento = new Segmento(Preferencias.CapitalizeNames ? textoSegmento.ToUpper() : textoSegmento , null, TipoSegmento.Texto, 
                            null, out string? erroresInternos, númeroPaso);
                        AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
                        return segmento;

                    }
                        
                }

            }

            AgregarErrores(ref errores, $"To Developer: ObtenerSegmentoEfectivo() didn't found any match for {entidad.NombreCompleto}.", númeroPaso);
            var segmento2 = new Segmento("", null, TipoSegmento.Texto, null, out string? erroresInternos2, númeroPaso);
            AgregarErrores(ref errores, erroresInternos2, númeroPaso: null);
            return segmento2;

        } //  ObtenerSegmentoEfectivo>


        public static string ObtenerTextoNúmeroLocal(string textoNúmero) 
            => textoNúmero.Replace(".", SeparadorDecimales).Replace(",", SeparadorDecimales);


        public static MessageBoxResult MostrarError(string mensaje) => MessageBox.Show(mensaje, "Error");


        public static void AgregarErrores(ref string? errores, string? nuevosErrores, int? númeroPaso) { // Solo se puede usar this ref para estructuras https://stackoverflow.com/questions/2618597/impossible-to-use-ref-and-out-for-first-this-parameter-in-extension-methods y https://stackoverflow.com/questions/46748334/extension-method-not-setting-value.
            if (!string.IsNullOrEmpty(nuevosErrores)) 
                errores += (númeroPaso != null ? $"Step {númeroPaso}: " : "") + nuevosErrores + (nuevosErrores.EndsWith(Environment.NewLine) ? "" : Environment.NewLine);
        } // AgregarErrores>


        public static MessageBoxResult MostrarInformación(string mensaje) => MessageBox.Show(mensaje, "Info");


        public static string? ExtraerTextoDePantalla(ScreenCaptureText tipo, ParámetrosExtracción parámetros, out float confianza) {

            using var bmp = CapturaDePantalla.ObtenerBitmap(ObtenerRectángulo(tipo), parámetros.Negativo, parámetros.BlancoYNegro,
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

            using var bmp = CapturaDePantalla.ObtenerBitmap(ObtenerRectángulo(tipo), negativo: false, blancoYNegro: false, escala: 1, 
                luminosidad: 1, contraste: 1, InterpolationMode.Low, DImg.PixelFormat.Format32bppArgb);
            var coloresEsquinas = new List<SDrw.Color>();
            coloresEsquinas.Add(bmp.GetPixel(0, 0));
            coloresEsquinas.Add(bmp.GetPixel(0, bmp.Height - 1));
            coloresEsquinas.Add(bmp.GetPixel(bmp.Width - 1, bmp.Height - 1));
            coloresEsquinas.Add(bmp.GetPixel(bmp.Width - 1, 0));
            return ObtenerColorPromedio(coloresEsquinas);

        } // ExtraerColorFondo>


        public static ScreenCaptureText ObtenerTipoPausa() {

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

            } else if (Preferencias.Game == AOMName) {

                switch (Preferencias.GameLanguage) {

                    case "EN":
                    case "IT":
                    case "DE":
                    case "BR":
                        tipo = ScreenCaptureText.Age_of_Mythology_Pause;
                        break;
                    case "ES":
                    case "MX":
                    case "PL":
                    case "MS":
                    case "FR":
                    case "TR":
                        tipo = ScreenCaptureText.Age_of_Mythology_Pause;
                        break;
                    case "ZH":
                    case "TW":
                    case "JP":
                        tipo = ScreenCaptureText.Age_of_Mythology_Pause;
                        break;
                    case "RU":
                        tipo = ScreenCaptureText.Age_of_Mythology_Pause;
                        break;
                    case "HI":
                        tipo = ScreenCaptureText.Age_of_Mythology_Pause;
                        break;
                    case "KO":
                        tipo = ScreenCaptureText.Age_of_Mythology_Pause;
                        break;
                    case "VI":
                        tipo = ScreenCaptureText.Age_of_Mythology_Pause;
                        break;
                    default:
                        break;
                }

            }
            return tipo;

        } // ObtenerTipoPausa>


        public static string? LeerPausa(out float confianza) {
      
            var textoPausa = "";
            var tipo = ObtenerTipoPausa();
            if (Preferencias.Game == Global.AOE2Name) {
         
                textoPausa = ExtraerTextoDePantalla(tipo, new List<string>(), out confianza);
                if (textoPausa != null && textoPausa.Contains("(")) textoPausa = textoPausa.Substring(0, textoPausa.IndexOf("(")).TrimEnd(); // Se elimina el texto leído que esté después de un paréntesis de apertura porque ese texto puede ser diferente para cada usuario dependiendo de la tecla que tenga asignada para pausa.

            } else {
                textoPausa = ExtraerTextoDePantalla(tipo, new List<string>(), out confianza);
            }

            return textoPausa;

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
                    if (progresoActual >= 90 && progresoActual <= 99 && 
                        (Preferencias.ScreenResolution == "1920x1080" || Preferencias.ScreenResolution == "1680x1050" 
                        || Preferencias.ScreenResolution == "1920x1200")) 
                            extraConfianzaRequerida = 0.2F; // Con la nueva fuente Smooth Serif de 2022 para la resolución 1920x1080 el intervalo de 90 a 99 es muy inexacto y puede producir números consecutivos incorrectos en 98 y 99 que pueden ser leídos como 8 y 9. Para evitar, este problema se sube la confianza para que el 98 que se lee como 8 con confianza 0,61, no se lea.
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

            } else if (Preferencias.Game == AOMName) {

                if (progresoActual < 9) {

                    texto = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9, new List<string>(), out confianza); 
                    if (confianza != -1) confianza += 1; // Se suma 1 a la confianza para evitar que en el intervalo de 0 a 9 se consideren lecturas no confiables por no proveer la lista de valores esperados y se retrase el ajuste innecesariamente un paso más.

                } else if (progresoActual == 9 || progresoActual == 10) {

                    var texto2n = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99, valoresEsperados, out float c2n);
                    var leído2n = false;
                    if (c2n > ConfianzaOCRAceptable) {

                        var éxito2 = int.TryParse(texto2n, out int progreso2);
                        if (progreso2 < 15) { // Leer comentario en AOE2, se deja igual la misma lógica.
                            leído2n = true;
                            texto = texto2n;
                            confianza = c2n;
                        }

                    }

                    if (!leído2n) {

                        var texto1n = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9, new List<string>(), out float c1n,
                            extraConfianzaRequerida: 0.3f);
                        texto = texto1n;
                        confianza = c1n + (confianza == -1 ? 0 : 1); // Ver confianza += 1;

                    }

                } else if (progresoActual > 10 && progresoActual < 99) {
                    texto = ExtraerTextoDePantalla(ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99, valoresEsperados, out confianza);
                }

            }

            var éxito = int.TryParse(texto, out int progreso);
            return éxito ? progreso : (int?)null;

        } // LeerProgreso>


        public static SDrw.RectangleF? ObtenerRectánguloSinAjustes(ScreenCaptureText tipo) { // El único objetivo de esta función es comparar con el rectángulo recomendado para detectar si se ha cambiado manualmente. No se usa en ninguna otra parte.

            if (Preferencias.ScreenCaptureRectangles != null && Preferencias.ScreenCaptureRectangles.ContainsKey(tipo)) { 
                return Preferencias.ScreenCaptureRectangles[tipo];
            } else {
                return null;
            }

        } // ObtenerRectánguloSinAjustes>


        public static SDrw.RectangleF ObtenerRectángulo(ScreenCaptureText tipo) {

            if (Preferencias.ScreenCaptureRectangles == null) CrearOCompletarScreenCaptureRectangles(cambióResolución: false, cambióUIMod: false);
            if (Preferencias.ScreenCaptureRectangles!.ContainsKey(tipo)) { // Después de CrearOCompletarScreenCaptureRectangles() se asegura que Preferencias.ScreenCaptureRectangles no es nulo.
               
                var r = Preferencias.ScreenCaptureRectangles![tipo];
                var e = Preferencias.GameInterfaceScale / 100;

                if (RectángulosCentradosAfectadosPorEscalaUI.Contains(tipo)) {
                    return new SDrw.RectangleF(r.X - r.Width * (e - 1) / 2, r.Y - r.Height * (e - 1) / 2, r.Width * e, r.Height * e);
                } else if (RectángulosSuperioresIzquierdosAfectadosPorEscalaUI.Contains(tipo)) {

                    var altura16a9 = Preferencias.WidthScreenResolution * 9 / 16; // Los rectángulos superiores izquierdos de la interface en el caso de Age of Empires II y por el momento se asumirá que en el caso de otros juegos también se mantienen inalterados al cambiar de resolución de proporción 16:9 (1,77) a otras proporciones (típicamente 1.66). Entonces todos los cálculos de los rectángulos superiores se hacen con base en la altura que tendría una pantalla si tuviera una relación 16:9 que es con la que se crean inicialmente los rectángulos. 
                    var factorAltura = (float)altura16a9 / (float)Preferencias.HeightScreenResolution; // Este factor es 1 para todas las pantallas con proporción 16:9 (1.77) que es con las que se crearon originalmente los rectángulos.
                    return new SDrw.RectangleF(r.X * e, r.Y * e * factorAltura, r.Width * e, r.Height * e * factorAltura);

                } else if (RectángulosSuperioresADerechaDePanelCentradoPorEscalaUI.ContainsKey(tipo)) {
                    return new SDrw.RectangleF((r.X - ((RectángulosSuperioresADerechaDePanelCentradoPorEscalaUI[tipo] * (1 - e)) / 2)), 
                        r.Y * e, r.Width * e, r.Height * e);
                } else if (RectángulosCentradosHorizontalmenteAfectadosPorEscalaUI.Contains(tipo)) {
                    return new SDrw.RectangleF(r.X - r.Width * (e - 1) / 2, r.Y * e, r.Width * e, r.Height * e);
                } else {
                    return r;
                }

            } else {
                return new SDrw.RectangleF(0, 0, 0.1f, 0.1f); // Un rectángulo cualquiera para que no saque error.
            }

        } // ObtenerRectángulo>


        public static void CrearOCompletarScreenCaptureRectangles(bool cambióResolución, bool cambióUIMod) {

            void actualizarRectánguloAOE2(ScreenCaptureText tipo) {

                if (Preferencias.ScreenCaptureRectangles == null) return;
                var rectángulo = ObtenerRectánguloRecomendado(tipo, Preferencias.ScreenResolution, Preferencias.UIMod);
                if (!Preferencias.ScreenCaptureRectangles.ContainsKey(tipo)) {
                    Preferencias.ScreenCaptureRectangles.Add(tipo, rectángulo);
                } else {
                    Preferencias.ScreenCaptureRectangles[tipo] = rectángulo;
                }

            } // actualizarRectánguloAOE2>

            void agregarRectángulo(ScreenCaptureText tipo) {

                if (Preferencias.ScreenCaptureRectangles == null) return;
                Preferencias.ScreenCaptureRectangles.Add(tipo, ObtenerRectánguloRecomendado(tipo, Preferencias.ScreenResolution));

            } // agregarRectángulo>

            var cambió = false;
            if (Preferencias.ScreenCaptureRectangles == null) {
                cambió = true;
                Preferencias.ScreenCaptureRectangles = new Dictionary<ScreenCaptureText, SDrw.RectangleF>();    
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_Game_Start)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Empires_II_Game_Start);
            }

            if (cambióUIMod || !Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9)) {
                cambió = true;        
                actualizarRectánguloAOE2(ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9);
            }

            if (cambióUIMod || cambióResolución || !Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99)) {
                cambió = true;
                actualizarRectánguloAOE2(ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99);
            }          

            if (cambióUIMod || cambióResolución || !Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999)) {   
                cambió = true;
                actualizarRectánguloAOE2(ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999);
            }
            
            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseM)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Empires_II_PauseM);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseL)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Empires_II_PauseL);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3XS)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Empires_II_PauseF3XS);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3S)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Empires_II_PauseF3S);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3M)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Empires_II_PauseF3M);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3L)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Empires_II_PauseF3L);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Empires_II_PauseF3XL)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Empires_II_PauseF3XL);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Mythology_Pause)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Mythology_Pause);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99);
            }

            if (!Preferencias.ScreenCaptureRectangles.ContainsKey(ScreenCaptureText.Age_of_Mythology_Game_Start)) {
                cambió = true;
                agregarRectángulo(ScreenCaptureText.Age_of_Mythology_Game_Start);
            }

            if (cambió) Settings.Guardar(Preferencias, RutaPreferencias);

        } // CrearOCompletarScreenCaptureRectangles>


        public static SDrw.RectangleF ObtenerRectánguloRecomendado(ScreenCaptureText tipo, string resolución, string uiModStr = "No_Mod") {

            var xDesfaceAOE2V = (resolución.Contains("x1080") || resolución.Contains("x1050") || resolución.Contains("x1024") || 
                resolución.Contains("x960") || resolución.Contains("x900") || resolución.Contains("x864") || resolución.Contains("x800") || 
                resolución.Contains("x768") || resolución.Contains("x720") || resolución.Contains("x600") || resolución.Contains("x1200")) 
                ? (571.5F - 573F) / 2560 : 0; // La resolución 1920x1080 o menor fuerza la fuente Smooth Serif, entonces se requiere un recorte diferente.
            if (xDesfaceAOE2V != 0 && tipo == ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999) xDesfaceAOE2V += 1F / 2560; // Un pequeño ajuste números más grandes.
            if (xDesfaceAOE2V != 0 && tipo == ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9) xDesfaceAOE2V = 0; // Para un solo carácter no se realiza el ajuste con xDesfaceAOE2V.

            var rectángulo = new SDrw.RectangleF();
            var uiMod = uiModStr.AEnumeración<UIMod>();
    
            switch (tipo) {
                case ScreenCaptureText.Age_of_Empires_II_PauseF3XS:
                    rectángulo = new SDrw.RectangleF(1310F / 2560, 682F / 1440, 79F / 2560, 71F / 1440);
                    break;
                case ScreenCaptureText.Age_of_Empires_II_PauseF3S:
                    rectángulo = new SDrw.RectangleF(1295F / 2560, 682F / 1440, 79F / 2560, 71F / 1440);
                    break;
                case ScreenCaptureText.Age_of_Empires_II_PauseF3M:
                    rectángulo = new SDrw.RectangleF(1322F / 2560, 682F / 1440, 79F / 2560, 71F / 1440);
                    break;
                case ScreenCaptureText.Age_of_Empires_II_PauseF3L:
                    rectángulo = new SDrw.RectangleF(1351F / 2560, 682F / 1440, 79F / 2560, 71F / 1440);
                    break;
                case ScreenCaptureText.Age_of_Empires_II_PauseF3XL:
                    rectángulo = new SDrw.RectangleF(1417F / 2560, 682F / 1440, 79F / 2560, 71F / 1440);
                    break;
                case ScreenCaptureText.Age_of_Empires_II_PauseM:
                    rectángulo = new SDrw.RectangleF(1113F / 2560, 675F / 1440, 335F / 2560, 91F / 1440);
                    break;
                case ScreenCaptureText.Age_of_Empires_II_PauseL:
                    rectángulo = new SDrw.RectangleF(1084F / 2560, 672F / 1440, 393F / 2560, 95F / 1440);
                    break;
                case ScreenCaptureText.Age_of_Empires_II_Villagers_0_to_9:

                    rectángulo = new SDrw.RectangleF(582F / 2560, 49F / 1440, 13F / 2560, 17F / 1440); // El algoritmo de OCR es inestable, cambia con el recorte realizado. Se debe procurar hacer siempre el mismo recorte. Si se cambia el rectángulo de recorte, se deben verificar la estabilidad del algoritmo con todos los números del rango aplicable.
                    switch (uiMod) {
                        case UIMod.No_Mod:
                            break;
                        case UIMod.Anne_HK_Better_Resource_Panel_and_Idle_Villager_Icon:
                        case UIMod.Anne_HK_Better_Resource_Panel_TheViper_Version:
                        case UIMod.KoBHV_Brand_New_Resource_Panel_with_Annoying_Idle_Villager_Button:
                        case UIMod.KoBHV_Brand_New_Resource_Panel_Standard_Version:
                            rectángulo = new SDrw.RectangleF(0.2255F - xDesfaceAOE2V, 0.029F, 0.008F, 0.014F);
                            break;
                        case UIMod.Anne_HK_Better_Resource_Panel_Top_Center_Version:
                            rectángulo = new SDrw.RectangleF(0.4755F - xDesfaceAOE2V, 0.029F, 0.008F, 0.014F);
                            break;
                        case UIMod.Streamlined_UI:
                            rectángulo = new SDrw.RectangleF(0.2257F - xDesfaceAOE2V, 0.034F, 0.008F, 0.013F);
                            break;
                        case UIMod.PointiBoi_Minimalistic_UI:
                            rectángulo = new SDrw.RectangleF(0.2263F - xDesfaceAOE2V, 0.032F, 0.008F, 0.013F);
                            break;
                        case UIMod.AllYourBase_Maximum_Advantage_UI:
                            MostrarError("The mod AllYourBase Maximum Advantage UI is not compatible with RTS Helper. Please ask the mod author to make the font of villager count bigger."); // Solo se muestra aquí para no repetir el error varias veces.
                            rectángulo.X = 0.3208F; rectángulo.Y = 0.816F;
                            break;
                        case UIMod.Custom_UI_Centered_Modern_Black_and_White:
                            rectángulo = new SDrw.RectangleF(0.391F - xDesfaceAOE2V, 0.8325F, 0.008F, 0.012F);
                            break;
                        case UIMod.XavilUI:
                            rectángulo.X = 0.488F; rectángulo.Y = 0.8135F;
                            break;
                        case UIMod.Villese_UI:
                            rectángulo.X = 0.264F; rectángulo.Y = 0.814F;
                            break;
                        case UIMod.Bottom_Side_UI:
                            rectángulo.X = 0.468F; rectángulo.Y = 0.821F;
                            break;
                        case UIMod.Resource_Bar_at_Bottom:
                            rectángulo.Y = 0.812F;
                            break;
                        case UIMod.Bottom_Resource_Panel:
                            rectángulo.X = 0.264F; rectángulo.Y = 0.814F;
                            break;
                        default:
                            break;
                    }
                    rectángulo.X = rectángulo.X + xDesfaceAOE2V;
                    break;

                case ScreenCaptureText.Age_of_Empires_II_Villagers_10_to_99:

                    rectángulo = new SDrw.RectangleF(573F / 2560, 48F / 1440, 23F / 2560, 17F / 1440); // Desface de x comparado con 0-9: -0,0035 a -0,0038 para el panel estandar sin cambio de letra ni tamaño.
                    switch (uiMod) {
                        case UIMod.No_Mod:
                            break;
                        case UIMod.Anne_HK_Better_Resource_Panel_and_Idle_Villager_Icon:
                        case UIMod.Anne_HK_Better_Resource_Panel_TheViper_Version:
                        case UIMod.KoBHV_Brand_New_Resource_Panel_with_Annoying_Idle_Villager_Button:
                        case UIMod.KoBHV_Brand_New_Resource_Panel_Standard_Version:
                            rectángulo = new SDrw.RectangleF(0.221F - xDesfaceAOE2V, 0.029F, 0.012F, 0.014F); // Los recuadors deben ser con amplitud a los lados para que no se descuadre en otras resoluciones y con poca altura par que no confunda el 7 con el 1. Locuras de la librería de OCR...
                            break;
                        case UIMod.Anne_HK_Better_Resource_Panel_Top_Center_Version:
                            rectángulo = new SDrw.RectangleF(0.471F - xDesfaceAOE2V, 0.029F, 0.012F, 0.014F); // Los recuadors deben ser con amplitud a los lados para que no se descuadre en otras resoluciones y con poca altura par que no confunda el 7 con el 1. Locuras de la librería de OCR...
                            break;
                        case UIMod.Streamlined_UI:
                            rectángulo = new SDrw.RectangleF(0.2215F - xDesfaceAOE2V, 0.034F, 0.012F, 0.013F);
                            break;
                        case UIMod.PointiBoi_Minimalistic_UI:
                            rectángulo = new SDrw.RectangleF(0.222F - xDesfaceAOE2V, 0.032F, 0.012F, 0.013F);
                            break;
                        case UIMod.AllYourBase_Maximum_Advantage_UI:
                            rectángulo.X = 0.3177F; rectángulo.Y = 0.816F;
                            break;
                        case UIMod.Custom_UI_Centered_Modern_Black_and_White:
                            rectángulo = new SDrw.RectangleF(0.3907F - xDesfaceAOE2V, 0.8325F, 0.012F, 0.012F);
                            break;
                        case UIMod.XavilUI:
                            rectángulo.X = 0.48448437F; rectángulo.Y = 0.8135F;
                            break;
                        case UIMod.Villese_UI:
                            rectángulo.X = 0.2603F; rectángulo.Y = 0.814F;
                            break;
                        case UIMod.Bottom_Side_UI:
                            rectángulo.X = 0.4642F; rectángulo.Y = 0.821F;
                            break;
                        case UIMod.Resource_Bar_at_Bottom:
                            rectángulo.Y = 0.812F;
                            break;
                        case UIMod.Bottom_Resource_Panel:
                            rectángulo.X = 0.2602F; rectángulo.Y = 0.814F;
                            break;
                        default:
                            break;
                    }
                    rectángulo.X = rectángulo.X + xDesfaceAOE2V;
                    break;

                case ScreenCaptureText.Age_of_Empires_II_Villagers_100_to_999:

                    rectángulo = new SDrw.RectangleF(565F / 2560, 49F / 1440, 30F / 2560, 17F / 1440); // Desface de x comparado con -0,0066 para el panel estandar sin cambio de letra ni tamaño.
                    switch (uiMod) {
                        case UIMod.No_Mod:
                            break;
                        case UIMod.Anne_HK_Better_Resource_Panel_and_Idle_Villager_Icon:
                        case UIMod.Anne_HK_Better_Resource_Panel_TheViper_Version:
                        case UIMod.KoBHV_Brand_New_Resource_Panel_with_Annoying_Idle_Villager_Button:
                        case UIMod.KoBHV_Brand_New_Resource_Panel_Standard_Version:
                            rectángulo = new SDrw.RectangleF(0.2165F - xDesfaceAOE2V, 0.029F, 0.016F, 0.014F);
                            break;
                        case UIMod.Anne_HK_Better_Resource_Panel_Top_Center_Version:
                            rectángulo = new SDrw.RectangleF(0.4665F - xDesfaceAOE2V, 0.029F, 0.016F, 0.014F);
                            break;
                        case UIMod.Streamlined_UI:
                            rectángulo = new SDrw.RectangleF(0.217F - xDesfaceAOE2V, 0.034F, 0.016F, 0.013F);
                            break;
                        case UIMod.PointiBoi_Minimalistic_UI:
                            rectángulo = new SDrw.RectangleF(0.2175F - xDesfaceAOE2V, 0.032F, 0.016F, 0.013F);
                            break;
                        case UIMod.AllYourBase_Maximum_Advantage_UI:
                            rectángulo.X = 0.3146F; rectángulo.Y = 0.816F;
                            break;
                        case UIMod.Custom_UI_Centered_Modern_Black_and_White:
                            rectángulo = new SDrw.RectangleF(0.3915F - xDesfaceAOE2V, 0.8325F, 0.015F, 0.012F);
                            break;
                        case UIMod.XavilUI:
                            rectángulo.X = 0.481F; rectángulo.Y = 0.8135F;
                            break;
                        case UIMod.Villese_UI:
                            rectángulo.X = 0.2572F; rectángulo.Y = 0.814F;
                            break;
                        case UIMod.Bottom_Side_UI:
                            rectángulo.X = 0.4608F; rectángulo.Y = 0.821F;
                            break;
                        case UIMod.Resource_Bar_at_Bottom:
                            rectángulo.Y = 0.812F;
                            break;
                        case UIMod.Bottom_Resource_Panel:
                            rectángulo.X = 0.257F; rectángulo.Y = 0.814F;
                            break;
                        default:
                            break;
                    }
                    rectángulo.X = rectángulo.X + xDesfaceAOE2V;
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
                case ScreenCaptureText.Age_of_Empires_II_Game_Start:
                    rectángulo = new SDrw.RectangleF(2F / 2560, 1416F / 1440, 22F / 2560, 22F / 1440);
                    break;
                case ScreenCaptureText.Age_of_Mythology_Pause:
                    rectángulo = new SDrw.RectangleF(0.423F, 0.48F, 0.152F, 0.04F);
                    break;
                case ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9:
                    rectángulo = new SDrw.RectangleF(0.71F, 0.01F, 0.006F, 0.015F);
                    break;
                case ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99:
                    rectángulo = new SDrw.RectangleF(0.7075F, 0.01F, 0.0115F, 0.015F);
                    break;
                case ScreenCaptureText.Age_of_Mythology_Game_Start:
                    rectángulo = new SDrw.RectangleF(0.49306F, 0.06714F, 0.01429F, 0.01286F);
                    break;
                default:
                    break;
            }

            return rectángulo;

        } // ObtenerRectánguloRecomendado>


        public static ParámetrosExtracción ObtenerParámetrosOCR(ScreenCaptureText tipo, int númeroRecomendado) {

            switch (tipo) {
                case ScreenCaptureText.Age_of_Mythology_Pause:
                    return AlfaNumNegByNL2C4;
                case ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9:
                    return HQBCL1_5C2x16; 
                case ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99:
                    return HQBCL1_5C2x16; 
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
                                case "1680x1050":
                                case "1920x1200":
                                    return HQBCL1_5C2x16; // Smooth Serif.
                                case "1366x768":
                                case "1600x900":
                                case "1280x720":
                                case "1360x768":
                                case "1280x800":
                                case "1440x900":
                                    return NNL1C2x2; // Para Smooth Serif: HQBCL1_5C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return NNL2C2x2; // Para Smooth Serif: NNL2C2x2.
                            }

                        case 2:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                case "1680x1050":
                                case "1920x1200":
                                    return HQBCL2C2x16; // Smooth Serif.
                                case "1366x768":
                                case "1600x900":
                                case "1280x720":
                                case "1360x768":
                                case "1280x800":
                                case "1440x900":
                                    return HQBCL1C2x4; // Para Smooth Serif: HQBCL2C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return HQBCL2C2x4; // Para Smooth Serif: HQBCL2C2x4.
                            }

                        case 3:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                case "1680x1050":
                                case "1920x1200":
                                    return NNL1C2x1; // Smooth Serif.
                                case "1366x768":
                                case "1600x900":
                                case "1280x720":
                                case "1360x768":
                                case "1280x800":
                                case "1440x900":
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
                                case "1680x1050":
                                case "1920x1200":
                                    return HQBCL1_5C2x16; // Smooth Serif.
                                case "1366x768":
                                case "1600x900":
                                case "1280x720":
                                case "1360x768":
                                case "1280x800":
                                case "1440x900":
                                    return HQBCL1C2x16; // Para Smooth Serif: HQBCL1_5C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return HQBCL2C2x16; // Para Smooth Serif: HQBCL1_5C2x16. Extrañamente se requiere establecer la variable unCarácter en verdadero para que coincida adecuadamente números de 2 cifras. Parece ser un bug del algorítmo de Tesseract.
                            }

                        case 2:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                case "1680x1050":
                                case "1920x1200":
                                    return HQBCL2C2x16; // Smooth Serif.
                                case "1366x768":
                                case "1600x900":
                                case "1280x720":
                                case "1360x768":
                                case "1280x800":
                                case "1440x900":
                                    return HQBCL2C6x4; // Para Smooth Serif: HQBCL2C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return HQBCL4C6x4; // Para Smooth Serif: HQBCL4C6x4.
                            }

                        case 3:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                case "1680x1050":
                                case "1920x1200":
                                    return NNL1C2x1; // Smooth Serif.
                                case "1366x768":
                                case "1600x900":
                                case "1280x720":
                                case "1360x768":
                                case "1280x800":
                                case "1440x900":
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
                                case "1680x1050":
                                case "1920x1200":
                                    return HQBCL1_5C2x16; // Smooth Serif.
                                case "1366x768":
                                case "1600x900":
                                case "1280x720":
                                case "1360x768":
                                case "1280x800":
                                case "1440x900":
                                    return HQBCL2C6x4; // Para Smooth Serif: HQBCL1_5C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return HQBCL4C6x4; // Para Smooth Serif: HQBCL1_5C2x16.
                            }

                        case 2:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                case "1680x1050":
                                case "1920x1200":
                                    return HQBCL2C2x16; // Smooth Serif.
                                case "1366x768":
                                case "1600x900":
                                case "1280x720":
                                case "1360x768":
                                case "1280x800":
                                case "1440x900":
                                    return NNL1C2x1; // Para Smooth Serif: HQBCL2C2x16.
                                case "2560x1440":
                                case "3840x2160":
                                default:
                                    return NNL2C2x1; // Para Smooth Serif: HQBCL4C6x4.
                            }

                        case 3:

                            switch (Preferencias.ScreenResolution) {
                                case "1920x1080":
                                case "1680x1050":
                                case "1920x1200":
                                    return NNL1C2x1; // Smooth Serif.
                                case "1366x768":
                                case "1600x900":
                                case "1280x720":
                                case "1360x768":
                                case "1280x800":
                                case "1440x900":
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
                case ScreenCaptureText.Age_of_Mythology_Villagers_0_to_9:
                case ScreenCaptureText.Age_of_Mythology_Villagers_10_to_99:
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
