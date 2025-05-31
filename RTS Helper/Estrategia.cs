using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static RTSHelper.Global;
using System.Windows.Media;
using static Vixark.General;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RTSHelper {



    public class Estrategia {



        #region Propiedades

        public List<Paso> Pasos { get; set; } = new List<Paso>();

        public Paso? Introducción { get; set; } = null;

        private int _NúmeroPaso = 0;
        public int NúmeroPaso {

            get { return _NúmeroPaso; }
            set {

                var hacerCambioNúmeroPaso = _NúmeroPaso != value;
                _NúmeroPaso = value;
                if (hacerCambioNúmeroPaso && EnCambioNúmeroPaso != null) EnCambioNúmeroPaso(); // Se hace aquí para que los procedimientos de la acción ya tengan del nuevo valor del paso.

            } 

        }

        public Dictionary<string, Comportamiento> ClasesDeComportamientos { get; set; } = new Dictionary<string, Comportamiento>();

        public Dictionary<string, Formato> ClasesDeFormatos { get; set; } = new Dictionary<string, Formato>();

        public Action? EnCambioNúmeroPaso;

        public string[] LíneasCódigo { get; set; } = new string[0];

        #endregion Propiedades>



        #region Propiedades Autocalculadas

        public bool EsÚltimoPaso => NúmeroPaso == Pasos.Count - 1;

        public bool EsPasoDespuésDeÚltimo => NúmeroPaso - 1 == Pasos.Count - 1;

        public bool EsDespuésDeÚltimoPaso => NúmeroPaso > Pasos.Count - 1;

        #endregion Propiedades Autocalculadas>



        #region Funciones y Procedimientos


        public bool VerificarEstiloCódigo(out string? inconsistencias) {

            StringBuilder inconsistenciasEstilo = new StringBuilder();
            var cuentaLínea = 0;
            var cuentaPaso = 0;
            var cuentaProgreso = 0;
            var correcto = true;
            var últimaLíneaVacía = false;
            var tieneLíneaInformativa = false;
            var yaPasóPorLíneaInformativa = false;
            int? cuentaAsignaciónUnidadesEconómicas = -1;

            void agregarInconsistencia(string inconsistencia) {
                inconsistenciasEstilo.AppendLine(inconsistencia);
                correcto = false;
            } // agregarInconsistencia>

            foreach (var línea in LíneasCódigo) {

                cuentaLínea++;
                if (cuentaLínea > 1) { // La primera línea típicamente es un comentario o cualquier otra cosa. Se ignorará.

                    var descripciónLínea = $"Line {cuentaLínea}";
                    if (string.IsNullOrEmpty(línea)) {
                        if (últimaLíneaVacía && yaPasóPorLíneaInformativa) agregarInconsistencia($"{descripciónLínea} is an empty line and previous line was also empty.");
                        últimaLíneaVacía = true;
                    } else {
                        últimaLíneaVacía = false;
                    }
                  
                    if (línea.StartsWith(" ")) { // Ninguna línea debería empezar por espacio vacío. Esto también ayuda a detectar líneas vacías de separación que por error tengan espacio.
                        agregarInconsistencia($"{descripciónLínea} starts with space.");
                    } else if (!(línea.StartsWith("{{") || línea.StartsWith("[[") || línea.StartsWith("<<") || línea.StartsWith("((") 
                        || string.IsNullOrEmpty(línea))) { // A estas secciones no se les aplica control de estilo. Ni a las líneas vacías.

                        if (línea.StartsWith("||")) {
                            
                            var cNúmeroPaso = Regex.Matches(línea, @"\|\| ([0-9]+) \|\|"); // Verificación de existencia de comentario con número de paso y que sea consecutivo.
                            if (cNúmeroPaso.Count > 0) {

                                var númeroPaso = int.Parse(cNúmeroPaso.First().Groups[1].Value);
                                var descripciónPaso = $"Step {númeroPaso}";
                                if (númeroPaso != cuentaPaso)
                                    agregarInconsistencia($"{descripciónPaso} doesn't have a consecutive step number. {cuentaPaso} was expected.");

                                if (línea.Contains("}   /f"))
                                    agregarInconsistencia($"{descripciónPaso} is empty and has more than two white spaces between }} and /f.");

                                var cEspaciosIncorrectos = Regex.Matches(línea, "\\[[0-9]s]\\[|]\\[[0-9]s]");
                                if (cEspaciosIncorrectos.Count > 0) 
                                    agregarInconsistencia($"{descripciónPaso} has spaces elements ([#s]) next to other elements. " +
                                        $"Put a white space around space elements.");

                                var cComportamientos = Regex.Matches(línea, "  {p=([0-9]+).*}  ", RegexOptions.IgnoreCase);
                                var progreso = -1;
                                if (cComportamientos.Count > 0) {

                                    progreso = int.Parse(cComportamientos.First().Groups[1].Value);
                                    if ((progreso != cuentaProgreso && progreso != cuentaProgreso + 1) && númeroPaso != 0)
                                        agregarInconsistencia($"{descripciónPaso} doesn't have a consecutive progress value. " +
                                            $"{cuentaProgreso} or {cuentaProgreso + 1} was expected."); // Aunque es posible que el progreso cambie en más de 1 por paso en algunas situaciones (por ejemplo en estragia Fast Castle en Age of Empires II), se prefiere establecer esta verificación de estilo porque la mayoría de las veces solo debe aumentar de a uno.
                                    cuentaProgreso = progreso;

                                    if (cuentaLínea >= LíneasCódigo.Count() - 1)
                                        agregarInconsistencia($"{descripciónPaso} is the last step and has a behavior section starting with progress. " +
                                            $"Last step shouldn't have progress.");

                                } else {

                                    if (cuentaLínea < LíneasCódigo.Count() - 1) 
                                        agregarInconsistencia($"{descripciónPaso} doesn't have a behavior section starting with progress and with two " +
                                            $"white spaces in each side. Example: '  {{p=40 t=50}}  '.");

                                }

                                var palabraTotal = "";
                                var textoOrdenTareas = "";
                                var prioridadTareas = new List<string>();
                                var patrónUnidadEconómica = "";
                                var barcosPescadores = new List<string> { "fsg", "fsa", "fsch", "fsn", "fse", "fsch" };
                                var barcosPescadoresStr = ", " + string.Join(", ", barcosPescadores);

                                if (Preferencias.Game == AOE2Name) {

                                    palabraTotal = "vs";
                                    textoOrdenTareas = "w, f, g, s, h, tc, town center, farm, lc, mill, mc, barracks, rax, range, archery, ar, stable, stb, stbl, blacksmith, blk, mkt, market, uni, university, " +
                                        "monastery, mst, tower, twr, pw, sw, arrow, idle, ?";
                                    prioridadTareas.AddRange(textoOrdenTareas.Split(", "));
                                    patrónUnidadEconómica = "(v)";

                                } else if (Preferencias.Game == AOMName) {

                                    palabraTotal = "p";
                                    textoOrdenTareas = "f, w, g, fv, str, olive, b, ch, pig, goat, deer, cow, boar, rhino, ele, h, m, tca, tce, tcg, " +
                                        "tcn, tcc, tcch, farm, fr, fs, dk, mv, mso, mpr, mph, mg, eg, lc, gr, silo, sh, ox, dock, eg, mc, t, atm, b, ct, etm, ia, itm, mw, milc, baolei, rax, crax, range, ar, ma, mrax, stb, stbl, s, a, " +
                                        "da, gh, hf, lh, ms, plc, mkt, sp, stwr, twr, ww, sw, up, down, left, right, idle, question, ?" + barcosPescadoresStr;
                                    prioridadTareas.AddRange(textoOrdenTareas.Split(", "));
                                    patrónUnidadEconómica = "(v|l|c|ch|g|d|ps|k|kh)";

                                }

                                var cPanelInferior = Regex.Match(línea, $".+  \\/f  <r>(([0-9]+)\\+*\\[([a-z0-9\\?]+)\\]  )+([0-9]+)\\+*\\[{palabraTotal}\\].*", 
                                    RegexOptions.IgnoreCase);
                                if (cPanelInferior.Success && cPanelInferior.Groups.Count == 5) {

                                    var total = int.Parse(cPanelInferior.Groups[4].Captures[0].Value);
                                    var suma = 0;
                                    for (int idCaptura = 0; idCaptura < cPanelInferior.Groups[2].Captures.Count; idCaptura++) {

                                        var cantidad = int.Parse(cPanelInferior.Groups[2].Captures[idCaptura].Value);
                                        var actividad = cPanelInferior.Groups[3].Captures[idCaptura].Value;
                                        if (cantidad == 0)
                                            agregarInconsistencia($"In {descripciónPaso} there's a task with 0 economic units. If a task doesn't have " +
                                                $"economic units asigned, don't add it.");
                                        if (!barcosPescadores.Contains(actividad)) suma += cantidad; // Los barcos pescadores no suman al total de población de aldeanos en Age of Mythology.

                                    }

                                    if (total != suma)
                                        agregarInconsistencia($"In {descripciónPaso} the sum of the economic units in each task is not equal to " +
                                            $"the total economic units.");

                                    if (progreso != -1 && total != progreso)
                                        agregarInconsistencia($"In {descripciónPaso} progress (p=#) should be equal to {palabraTotal} (#[{palabraTotal}]).");

                                    var prioridadAnteriorTarea = -1;
                                    for (int idTarea = 0; idTarea < cPanelInferior.Groups[3].Captures.Count; idTarea++) {

                                        var tarea = cPanelInferior.Groups[3].Captures[idTarea].Value.ToLower();
                                        var prioridad = prioridadTareas.IndexOf(tarea);
                                        if (prioridad == -1) {
                                            agregarInconsistencia($"{descripciónPaso} has [{tarea}] task which is not recognized. " +
                                                $"Recognized tasks: {textoOrdenTareas}.");
                                        } else {

                                            if (prioridad < prioridadAnteriorTarea)
                                                agregarInconsistencia($"{descripciónPaso} has [{tarea}] task in a incorrect position. " +
                                                    $"The tasks' correct order is: {textoOrdenTareas}.");
                                            prioridadAnteriorTarea = prioridad;

                                        }

                                    }

                                } else {

                                    if (cuentaLínea < LíneasCódigo.Count() - 1)
                                        agregarInconsistencia($"{descripciónPaso} doesn't have a tasks detail section with tasks separated by two " +
                                            $"white spaces and starting with /f surrounded by two white spaces. Example: '  /f  <r>2[w]  14[f]  1[mc]  17[vs]'.");

                                }


                                var cUnos = Regex.Matches(línea, "\\[1\\].+<r>");
                                if (cUnos.Count > 0)
                                    agregarInconsistencia($"{descripciónPaso} has [1]. Avoid using [1] to specify one entity.");

                                var cContenido = Regex.Matches(línea, "}  (.+)  \\/f");
                                if (cContenido.Count > 0) {

                                    var contenido = cContenido[0].Groups[1].Value;
                                    var cDoblesEspacios = Regex.Matches(contenido, "([^ ])  [^ ]");
                                    if (cDoblesEspacios.Count > 0) {

                                        foreach (Match? c in cDoblesEspacios) {
                                            if (c != null && c.Groups[1].Value != ":" && c.Groups[1].Value != "|")
                                                agregarInconsistencia($"{descripciónPaso} has an unneeded double space. " +
                                                    $"If you really need the double space, you can force it with ' [0s] '.");
                                        }

                                    }

                                }

                                if (línea.Contains("    ") || línea.Contains("     ") || línea.Contains("      ") || línea.Contains("       "))
                                    agregarInconsistencia($"{descripciónPaso} has a white space longer than 3 characters. To create longer spaces use [#s].");
              
                                var cAsignaciónUnidadesEconómicas = Regex.Matches(línea, 
                                    $"  \\[{patrónUnidadEconómica}\\]<xt?>([0-9][0-9]?[0-9]?)-?([0-9.]?[0-9.]?[0-9.]?)<>", RegexOptions.IgnoreCase);
                                if (cAsignaciónUnidadesEconómicas.Count > 0) {

                                    var númeroInicial = int.Parse(cAsignaciónUnidadesEconómicas[0].Groups[2].Value);
                                    var textoNúmeroFinal = cAsignaciónUnidadesEconómicas[0].Groups[3].Value;
                                    var númeroFinal = textoNúmeroFinal == "..." ? (int?)null : 
                                        (string.IsNullOrEmpty(textoNúmeroFinal) ? númeroInicial : int.Parse(textoNúmeroFinal));

                                    if (cuentaAsignaciónUnidadesEconómicas == null) { // Si ya estaba en el final, no se esperaban más rangos.
                                        agregarInconsistencia($"{descripciónPaso} has an economic unit assignment instruction, but an earlier step had already " +
                                            $"finshed the assignment like this '[{patrónUnidadEconómica}]<xt>[##]-...<>'.");
                                    } else {

                                        if (cuentaAsignaciónUnidadesEconómicas != -1 && númeroInicial != cuentaAsignaciónUnidadesEconómicas + 1) 
                                            agregarInconsistencia($"{descripciónPaso} has an economic unit assignment instruction that doesn't match " +
                                                $"with an early assignment. The range was expected to start with {cuentaAsignaciónUnidadesEconómicas + 1}.");

                                        if (númeroInicial != progreso + 1) 
                                            agregarInconsistencia($"{descripciónPaso} has an economic unit asignment instruction that doesn't match " +
                                                $"with the current progress. The range was expected to start with {progreso + 1}.");
 
                                    }
                                    cuentaAsignaciónUnidadesEconómicas = númeroFinal;

                                }

                                cuentaPaso++;

                            } else {
                                if (yaPasóPorLíneaInformativa) agregarInconsistencia($"{descripciónLínea} doesn't have step number in initial comment.");
                            }
                            
                        } else {
                            agregarInconsistencia($"{descripciónLínea} doesn't start with ||."); // Todas las líneas de pasos deben empezar con el comentario con el número de paso. 
                        }

                    } else {

                        if (línea.StartsWith("((")) {
                            tieneLíneaInformativa = true;
                            yaPasóPorLíneaInformativa = true;
                        }

                    }

                }
     
            }

            if (!tieneLíneaInformativa) agregarInconsistencia($"Doesn't have build order information section starting with ((.");
            inconsistencias = inconsistenciasEstilo.ToString();
            return correcto;

        } // VerificarEstiloCódigo>


        public bool EscribirCódigoEnArchivo(string directorioEstrategias, string nombreEstrategia, bool modoDesarrollo, 
            out string? rutaEstrategia) {

            if (modoDesarrollo) {
                rutaEstrategia = ObtenerRutaEstrategias(Settings.ObtenerDirectorioEstrategias(Global.DirectorioEstrategiasCódigo, 
                    Preferencias.Game), nombreEstrategia);
            } else {
                rutaEstrategia = ObtenerRutaEstrategias(directorioEstrategias, nombreEstrategia);
            }

            if (!File.Exists(rutaEstrategia)) return false;
            if (!ObtenerArchivoLibre(rutaEstrategia)) return false;
            try {

                var líneasCódigoArchivo = new List<string>();
                foreach (var líneaCódigo in LíneasCódigo) {

                    var corchetes = líneaCódigo.StartsWith("[[ ") && líneaCódigo.EndsWith(" ]]");
                    var llaves = líneaCódigo.StartsWith("{{ ") && líneaCódigo.EndsWith(" }}");
                    var angulares = líneaCódigo.StartsWith("<< ") && líneaCódigo.EndsWith(" >>");
                    var aperturas = "[{<";
                    var cierres = "]}>";

                    if (corchetes || llaves || angulares) {

                        var apertura = corchetes ? '[' : (llaves ? '{' : (angulares ? '<' : '?'));
                        var cierre = corchetes ? ']' : (llaves ? '}' : (angulares ? '>' : '?'));
                        var líneaCódigoArchivo = new StringBuilder();
                        var profundidadSignos = 0;

                        líneasCódigoArchivo.Add($"{apertura}{apertura}");
                        var índice = 3;
                        foreach (char c in líneaCódigo[3..^2]) {

                            if (aperturas.Contains(c)) {
                                profundidadSignos++;
                                líneaCódigoArchivo.Append(c);
                            } else if (cierres.Contains(c)) {
                                if (profundidadSignos > 0) profundidadSignos--;
                                líneaCódigoArchivo.Append(c);
                            } else if (c == ' ' && profundidadSignos == 0) {

                                if (líneaCódigo[índice + 1] == '[') { // Un caso especial en el que se está usando un espacio el valor del grupo. En teoría debería tener una lógica más compleja para omitir agregar salto de línea incluso si después hay una letra o número, pero en ese caso sería difícil diferenciarlo de un correcto fin del grupo e inicio del nombre del siguiente grupo. Por el momento, para el uso que se le quiere dar que es usar un espacio entre grupos ya existentes, funciona aceptable.
                                    líneaCódigoArchivo.Append(c);
                                } else {
                                    líneasCódigoArchivo.Add(líneaCódigoArchivo.ToString());
                                    líneaCódigoArchivo.Clear();
                                }

                            } else {
                                líneaCódigoArchivo.Append(c);
                            }

                            índice++;

                        }
                        líneasCódigoArchivo.Add($"{cierre}{cierre}");

                    } else {
                        líneasCódigoArchivo.Add(líneaCódigo);
                    }

                }

                File.WriteAllLines(rutaEstrategia, líneasCódigoArchivo);
                return true;

            } catch (Exception) {
                return false;
                throw;
            }

        } // EscribirCódigoEnArchivo>


        public string ObtenerRutaEstrategias(string directorioEstrategias, string nombreEstrategia) {

            var ruta = Path.Combine(directorioEstrategias, $"{nombreEstrategia}.txt");
            if (!Directory.Exists(directorioEstrategias)) Directory.CreateDirectory(directorioEstrategias);
            return ruta;

        } // ObtenerRutaEstrategias>


        public bool CargarPasos(FuenteEstrategia fuente, string directorioEstrategias, string nombreEstrategia, out string? errores, 
            out string? rutaEstrategia) {

            errores = null;
            var pasos = new List<Paso>();
            rutaEstrategia = ObtenerRutaEstrategias(directorioEstrategias, nombreEstrategia);
            if (!File.Exists(rutaEstrategia)) return false;
            if (!ObtenerArchivoLibre(rutaEstrategia)) return true; // Si pasado el tiempo máximo permitido, el archivo no está libre, entonces no carga los pasos.

            Formato? formatoGlobal = null;
            Comportamiento? comportamientoGlobal = null;
            var grupos = new Dictionary<string, string>(); // Almacena el nombre de un nombre grupo y su contenido. El contenido será reemplazado directamente en el lugar que se use el nombre del grupo.
            var númeroPaso = 0;
            var númeroLínea = 0;

            if (fuente == FuenteEstrategia.Archivo) {

                var líneasCódigoArchivo = File.ReadAllLines(rutaEstrategia).ToList();
                var líneasCódigo = new List<string>();
                var consolidandoAngulares = false;
                var consolidandoCorchetes = false;
                var consolidandoLlaves = false;
                var líneaCorchetesConsolidadada = new StringBuilder();
                var líneaLlavesConsolidadada = new StringBuilder();
                var líneaAngularesConsolidadada = new StringBuilder();

                foreach (var líneaCódigoDirecta in líneasCódigoArchivo) { // Consolida las líneas separadas con << >>, [[ ]] y {{ }} en una sola línea.

                    var consolidandoSección = consolidandoCorchetes || consolidandoLlaves || consolidandoAngulares;
                    var líneaCódigoDirectaLimpia = líneaCódigoDirecta.Trim();
                    if (líneaCódigoDirectaLimpia == "<<" && !consolidandoSección) consolidandoAngulares = true;
                    if (líneaCódigoDirectaLimpia == "[[" && !consolidandoSección) consolidandoCorchetes = true;
                    if (líneaCódigoDirectaLimpia == "{{" && !consolidandoSección) consolidandoLlaves = true;
                    consolidandoSección = consolidandoCorchetes || consolidandoLlaves || consolidandoAngulares;

                    if (consolidandoSección) {

                        if (consolidandoCorchetes) líneaCorchetesConsolidadada.Append($"{líneaCódigoDirecta} ");
                        if (consolidandoAngulares) líneaAngularesConsolidadada.Append($"{líneaCódigoDirecta} ");
                        if (consolidandoLlaves) líneaLlavesConsolidadada.Append($"{líneaCódigoDirecta} ");

                    } else {
                        líneasCódigo.Add(líneaCódigoDirecta);
                    }

                    if (líneaCódigoDirectaLimpia == ">>" && consolidandoAngulares) {
                        líneasCódigo.Add(líneaAngularesConsolidadada.ToString().TrimEnd());
                        consolidandoAngulares = false;
                        líneaAngularesConsolidadada.Clear();
                    }

                    if (líneaCódigoDirectaLimpia == "]]" && consolidandoCorchetes) {
                        líneasCódigo.Add(líneaCorchetesConsolidadada.ToString().TrimEnd());
                        consolidandoCorchetes = false;
                        líneaCorchetesConsolidadada.Clear();
                    }

                    if (líneaCódigoDirectaLimpia == "}}" && consolidandoLlaves) {
                        líneasCódigo.Add(líneaLlavesConsolidadada.ToString().TrimEnd());
                        consolidandoLlaves = false;
                        líneaLlavesConsolidadada.Clear();
                    }

                }

                LíneasCódigo = líneasCódigo.ToArray();

            } // fuente == FuenteEstrategia.Archivo>

            foreach (var códigoLínea in LíneasCódigo) {

                string? comentarioFinal = null;
                var códigoSinComentarios = códigoLínea.StartsWith("((") ? códigoLínea : (EliminarComentarios(códigoLínea, out comentarioFinal) ?? ""); // No se extraen comentarios del paso de introducción porque si tiene comentarios se eliminan los paréntesis de cierre y ya no se reconoce como paso de introducción.
                var códigoRecortado = códigoSinComentarios.Trim();

                if (códigoRecortado.StartsWith("<<") && códigoRecortado.EndsWith(">>")) { // Es necesario hacerlo así y no con Regex por facilidad y para que no entre en conflicto con las clases de comportamientos que van entre <>.

                    formatoGlobal = new Formato(códigoRecortado[2..^2], out Dictionary<string, Formato> clasesLeídas, null, out string? erroresInternos
                        , -4);
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
                    ClasesDeFormatos = clasesLeídas;

                } else if (códigoRecortado.StartsWith("{{") && códigoRecortado.EndsWith("}}")) { // Es necesario hacerlo así y no con Regex por facilidad y para que no entre en conflicto con las clases de comportamientos que van entre {}.

                    comportamientoGlobal = new Comportamiento(códigoRecortado[2..^2], out Dictionary<string, Comportamiento> clasesLeídas, null, 
                        out string? erroresInternos, -3);
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
                    ClasesDeComportamientos = clasesLeídas;

                } else if (códigoRecortado.StartsWith("((") && códigoRecortado.EndsWith("))")) { // Es necesario hacerlo así y no con Regex por facilidad.

                    var códigoIntroSinComentarios = EliminarComentarios(códigoRecortado[2..^2], out comentarioFinal) ?? "";
                    var códigoIntroSinComentariosRecortado = códigoIntroSinComentarios.Trim();
                    Introducción = new Paso(númeroLínea, códigoIntroSinComentariosRecortado, comportamientoGlobal, formatoGlobal, ClasesDeFormatos, 
                        ClasesDeComportamientos, grupos, out string ? erroresInternos, -2, comentarioFinal);
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

                } else if (códigoRecortado.StartsWith("[[") && códigoRecortado.EndsWith("]]")) { // Es necesario hacerlo así y no con Regex por facilidad y para que no entre en conflicto con las clases de comportamientos que van entre [].

                    var códigoGrupos = códigoRecortado[2..^1]; // No recorta el último corchete porque este se ignorará cuando se agregue el contenido del último grupo.
                    var estado = "n"; // Puede ser n: en nombre, c: contenido.
                    var profundidadCorchetes = 0;
                    var nombreGrupoActual = new StringBuilder();
                    var contenidoGrupoActual = new StringBuilder();
                    var cuentaCarácteres = 0;

                    foreach (var carácter in códigoGrupos) { // Se hace de esta manera y no con regex porque regex no es capaz de controlar adecuadamente los múltiples [] dentro de estos textos.

                        if (carácter == '=') {
                            estado = "c";
                        } else if ((carácter == ' ' && profundidadCorchetes == 0) || cuentaCarácteres == códigoGrupos.Length - 1) {

                            estado = "n";
                            var nombre = nombreGrupoActual.ToString();
                            if (!string.IsNullOrWhiteSpace(nombre)) {

                                if (ObtenerEntidad(nombre) != null) {
                                    AgregarErrores(ref errores, $"{nombre} can't be used as a group name because it's already an entity name.", -1);
                                } else {

                                    if (grupos.ContainsKey(nombre)) {
                                        AgregarErrores(ref errores, $"You can't add two groups with the same name: {nombre}.", -1);
                                    } else {
                                        grupos.Add(nombre, contenidoGrupoActual.ToString());
                                    }
                                
                                }

                            }

                            nombreGrupoActual = new StringBuilder();
                            contenidoGrupoActual = new StringBuilder();

                        } else {

                            var agregarContenido = true; // Evita que se agreguen los corchetes de inicio y del final.
                            if (carácter == '[') {
                                agregarContenido = profundidadCorchetes != 0;
                                profundidadCorchetes++;
                            } else if (carácter == ']') {
                                profundidadCorchetes--;
                                agregarContenido = profundidadCorchetes != 0;
                            }

                            if (estado == "c" && agregarContenido) {
                                contenidoGrupoActual.Append(carácter);
                            } else if (estado == "n") {
                                nombreGrupoActual.Append(carácter);
                            }

                        }
                        cuentaCarácteres++;

                    }

                } else if (!string.IsNullOrWhiteSpace(códigoRecortado)) { // No se agrega el paso si es un espacio en la build order. Estos espacios son útiles para tener más orden de edición.

                    pasos.Add(new Paso(númeroLínea, códigoRecortado, comportamientoGlobal, formatoGlobal, ClasesDeFormatos, ClasesDeComportamientos, grupos, 
                        out string? erroresInternos, númeroPaso, comentarioFinal));
                    AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
                    númeroPaso++;

                }

                númeroLínea++;

            }

            if (pasos.Count == 0) {
                pasos.Add(new Paso(0, "", null, null, null, null, new Dictionary<string, string>(), out string? erroresInternos, 0, null)); // Para poder almacenar la duración de los pasos, debe haber como mínimo un paso.
                AgregarErrores(ref errores, erroresInternos, númeroPaso: null);
            }
                
            if (NúmeroPaso > 0) { // Si se carga una build order en la mitad de la ejecución, debe copiar las duraciones de los pasos de la ejecución actual.
                for (int i = 0; i < NúmeroPaso; i++) {
                    if (i <= pasos.Count - 1 && i <= Pasos.Count - 1) pasos[i].DuraciónEnJuego = Pasos[i].DuraciónEnJuego;
                }
            }
            Pasos = pasos;

            return true;

        } // CargarPasos>


        public string? EliminarComentarios(string? texto, out string? comentarioFinal) {

            comentarioFinal = null;
            if (texto is null) return null;
            if (!texto.Contains("||")) return texto;
            var textoSinComentarios = new StringBuilder();
            var índiceActual = 0;
            var índiceAnterior = 0;
            var esComentario = texto.StartsWith("||");
            var inicioTexto = true;
            string? comentarioActual = null;

            do {

                índiceAnterior = índiceActual;
                índiceActual = texto.IndexOf("||", índiceAnterior + 2);
                var índiceInicial = inicioTexto ? 0 : índiceAnterior + 2;
                var textoSegmento = índiceActual == -1 ? texto[índiceInicial..^0] : texto[índiceInicial..índiceActual];
                if (!esComentario) {
                    textoSinComentarios.Append(textoSegmento);
                    comentarioActual = null; // Si se encuentra cualquier texto adicional después del último comentario, ese comentario no es un comentario de final de línea.
                } else {
                    comentarioActual = textoSegmento;
                }
                esComentario = !esComentario;
                inicioTexto = false;

            } while (índiceActual != -1);

            if (comentarioActual != null) comentarioFinal = comentarioActual.Trim();
            return textoSinComentarios.ToString();

        } // EliminarComentarios>


        public void MostrarPaso(int? númeroPaso, Formato formatoPredeterminado, StackPanel contenedor, bool mostrarSiempreÚltimoPaso, double altoMáximo,
            HorizontalAlignment alineaciónHorizontal, double margenInferior, TextBox? editorCódigo, bool simulación, out bool superóAlto, out string? errores) {

            superóAlto = false;
            var númeroPasoAMostrar = númeroPaso;
            errores = null;
            if (númeroPaso >= Pasos.Count) {

                if (mostrarSiempreÚltimoPaso) {
                    númeroPasoAMostrar = Pasos.Count > 0 ? Pasos.Count - 1 : (int?)null;
                } else {
                    númeroPasoAMostrar = null;
                }

            }

            Paso? paso;
            if (númeroPaso == null || númeroPaso < 0) {
                paso = Introducción;
            } else if (númeroPasoAMostrar == null) {
                paso = null; // No muestra nada. Es el el siguiente paso después del último.     
            } else {
                paso = Pasos[(int)númeroPasoAMostrar];
            }

            if (paso != null) {

                if (!simulación && editorCódigo != null) {
                    editorCódigo.Tag = "cambiando-texto-desde-código";
                    editorCódigo.Text = LíneasCódigo[paso.LíneaCódigo];
                    editorCódigo.Tag = paso.LíneaCódigo;
                }

                var sumaAlto = 0D;
                foreach (var instrucción in paso.Instrucciones) {

                    var mayorEspaciadoVertical = 0D;
                    var altoInstrucción = 0D;
                    var spnHorizontal = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = alineaciónHorizontal };

                    foreach (var segmento in instrucción.Segmentos) {

                        var formato = Formato.ObtenerFormatoEfectivo(segmento.Formato, formatoPredeterminado, out string? erroresInternos, númeroPaso, 
                            forzarValores: true);
                        AgregarErrores(ref errores, erroresInternos, númeroPaso: null);

                        if (formato.Negrita == null || formato.Cursiva == null || formato.NombreFuente == null || formato.Subrayado == null
                            || formato.TamañoFuenteEfectiva == null || formato.Color == null || formato.ColorHexadecimal == null ||
                            formato.TamañoImagen == null || formato.ObtenerTamañoImagenEfectiva((double)formato.TamañoImagen) == null) {
                            AgregarErrores(ref errores, "To Developer: Unexpected null value in formato in MostrarPaso().", númeroPaso);
                            continue;
                        }

                        var altoSegmentoTexto = (double)formato.TamañoFuenteEfectiva
                            * ((formato.NombreFuente == NombreTimesNewRoman ? 0.82 : 1) * FactorTamañoTextoAPixelesFuentePredeterminada);
                        var altoSegmentoImagen = 0D;
                        var altoSegmento = 0D;

                        var espaciadoVertical = altoSegmentoTexto * (Preferencias.LineSpacing / 100);
                        if (espaciadoVertical > mayorEspaciadoVertical) mayorEspaciadoVertical = espaciadoVertical;

                        var segmentosEfectivos = new List<Segmento>();
                        var margenEnTexto = false;
                        if (segmento.Tipo == TipoSegmento.Entidad) {

                            var entidad = ObtenerEntidad(segmento.Texto);
                            if (entidad == null) {

                                AgregarErrores(ref errores, $"The name {segmento.Texto} wasn't found.", númeroPaso);
                                var segmentoEfectivo = segmento.Clonar(out string? erroresInternos2, númeroPaso);
                                AgregarErrores(ref errores, erroresInternos2, númeroPaso: null);
                                segmentoEfectivo.Tipo = TipoSegmento.Texto;
                                segmentosEfectivos.Add(segmentoEfectivo);
                                margenEnTexto = false;

                            } else {

                                var segmentoEfectivo = ObtenerSegmentoEfectivo(entidad, out string? erroresInternos2, númeroPaso);
                                margenEnTexto = segmentoEfectivo.Tipo != TipoSegmento.Imagen; // Cuando se usa el marcado con corchetes cuadrados por ejemplo, [attack][town center] se espera que las entidades no sean adjuntas si están en texto, es decir, se espera que no se muestre ATTKTC, si no ATT TC. Por esto se debe agregar este margen adicional cuando la entidad no se vaya a mostrar como imagen.
                                AgregarErrores(ref errores, erroresInternos2, númeroPaso: null);
                                segmentosEfectivos.Add(segmentoEfectivo);

                            }

                        } else { // TipoSegmento.Texto.

                            var textos = segmento.Texto.Split('-');
                            if (segmento.Texto.Contains("http") || formato.Posición == PosiciónTexto.Normal) textos = new string[] { segmento.Texto };

                            for (var i = 0; i < textos.Length; i++) {

                                var segmentoEfectivo = segmento.Clonar(out string? erroresInternos2, númeroPaso);
                                AgregarErrores(ref errores, erroresInternos2, númeroPaso: null);
                                margenEnTexto = false; // El texto normal no lleva margen.
                                segmentoEfectivo.Texto = textos[i];
                                segmentosEfectivos.Add(segmentoEfectivo);

                                if (i < textos.Length - 1) {

                                    var segmentoEfectivoGuión = segmento.Clonar(out string? erroresInternosGuión, númeroPaso);
                                    AgregarErrores(ref errores, erroresInternosGuión, númeroPaso: null);
                                    margenEnTexto = false; // El texto normal no lleva margen.
                                    segmentoEfectivoGuión.Texto = "-";
                                    segmentosEfectivos.Add(segmentoEfectivoGuión);

                                }

                            }

                        }

                        if (simulación) continue;

                        var tamañoImagen = (double)formato.ObtenerTamañoImagenEfectiva((double)formato.TamañoImagen)!;
                        altoSegmentoImagen = tamañoImagen;
                        var margen = tamañoImagen * (Preferencias.EntityHorizontalMargin / 200); // Se divide por 200 porque el margen se aplica a ambos lados.
                        var margenDerecha = margen;
                        var margenIzquierda = margen;
                        var margenAdicionalSubYSuperíndices = (formatoPredeterminado.TamañoFuenteEfectiva ?? 25) * 10 / 60;
                        var alineación = VerticalAlignment.Center;
                        var radioEsquinas = tamañoImagen * (Preferencias.ImageBackgroundRoundedCornersRadius / 100);
                        if (radioEsquinas < 5) radioEsquinas = 5; // Cuando es menos de 5 no se siente redondeado. Esta línea se agregó principalmente para mejorar la apariencia en las imágenes compuestas más compactas que usan la funcionalidad de desface vertical y horizontal.
                        var radioEsquinasIzquierdas = radioEsquinas;
                        var radioEsquinasDerechas = radioEsquinas;
                        var ambosÍndices = false;

                        if (formato.Posición != PosiciónTexto.Normal) {

                            margenIzquierda = -margen;
                            margenDerecha = margen;
                            tamañoImagen = (Preferencias.SubscriptAndSuperscriptImagesSize / 100) * tamañoImagen;
                            radioEsquinasIzquierdas = 0;

                            if (formato.Posición == PosiciónTexto.Subíndice) {
                                alineación = VerticalAlignment.Bottom;
                            } else if (formato.Posición == PosiciónTexto.Superíndice) {
                                alineación = VerticalAlignment.Top;
                            }

                            if (spnHorizontal.Children.Count > 0 && spnHorizontal.Children[spnHorizontal.Children.Count - 1] is Grid gridAnterior
                                && gridAnterior.Children[0] is Border borderAnterior) { // Ajusta las esquinas redondeadas de la imagen anterior para que sea integrada con la imagen subíndice o superíndice.
                                    
                                var radioAnterior = borderAnterior.CornerRadius.TopLeft;
                                switch (gridAnterior.VerticalAlignment) {
                                    case VerticalAlignment.Top:

                                        if (formato.Posición == PosiciónTexto.Superíndice)
                                            borderAnterior.CornerRadius = new CornerRadius(radioAnterior, 0, 0, radioAnterior);
                                        if (formato.Posición == PosiciónTexto.Subíndice) ambosÍndices = true;

                                        break;

                                    case VerticalAlignment.Bottom:

                                        if (formato.Posición == PosiciónTexto.Subíndice)
                                            borderAnterior.CornerRadius = new CornerRadius(radioAnterior, 0, 0, radioAnterior);
                                        if (formato.Posición == PosiciónTexto.Superíndice) ambosÍndices = true;
                                        break;

                                    case VerticalAlignment.Center:
                                    case VerticalAlignment.Stretch:

                                        if (formato.Posición == PosiciónTexto.Subíndice) {
                                            borderAnterior.CornerRadius = new CornerRadius(radioAnterior, radioAnterior, 0, radioAnterior);
                                        } else if (formato.Posición == PosiciónTexto.Superíndice) {
                                            borderAnterior.CornerRadius = new CornerRadius(radioAnterior, 0, radioAnterior, radioAnterior);
                                        }
                                        break;

                                }

                                if (ambosÍndices) { // Se está usando tanto el superíndice como el subíndice. Se debe verificar un objeto aún más atrás para corregir los bordes del original.

                                    margenIzquierda = -margen - (double)(gridAnterior.Tag ?? tamañoImagen) - margenAdicionalSubYSuperíndices; // Para que quede justo abajo/arriba del otro super/subíndice.
                                    if (spnHorizontal.Children.Count > 1
                                        && spnHorizontal.Children[spnHorizontal.Children.Count - 2] is Grid gridAnterior2
                                        && gridAnterior2.Children[0] is Border borderAnterior2 &&
                                        gridAnterior2.VerticalAlignment == VerticalAlignment.Center) { // Solo se analiza el caso de imagen grande anterior a la anterior. Los otros no son casos usuales.

                                        var radioAnterior2 = borderAnterior2.CornerRadius.TopLeft;
                                        borderAnterior2.CornerRadius = new CornerRadius(radioAnterior2, 0, 0, radioAnterior2);

                                    }

                                }

                            }

                        } // formato.Posición != PosiciónTexto.Normal>

                        if (formato.DesfaceHorizontal != null && Math.Abs((double)formato.DesfaceHorizontal) > 0) {

                            if (formato.DesfaceHorizontal > 0) {
                                margenIzquierda = margenIzquierda - tamañoImagen * ((double)formato.DesfaceHorizontal! / 100);
                            } else {
                                margenDerecha = margenDerecha + tamañoImagen * ((double)formato.DesfaceHorizontal! / 100);
                            }                    

                        }
                            
                        var margenSuperior = 0D;
                        if (formato.DesfaceVertical != null && Math.Abs((double)formato.DesfaceVertical) > 0)
                            margenSuperior = tamañoImagen * (((double)formato.DesfaceVertical! + 2) / 100); // El + 2 es un pequeño ajuste para un caso de uso común para generar subíndices compactos, donde estaba quedando un espacio final sin rellenar en la parte inferior. Por ejemplo para [v]<xxs ho100  vo100>[boar]<>.
                                                    
                        var anchoSegmentosEfectivos = 0.0;

                        var grilla = new Grid { Margin = new Thickness(0), VerticalAlignment = alineación };
                        var borde = new Border {
                            Height = tamañoImagen, CornerRadius = new CornerRadius(radioEsquinasIzquierdas,
                            radioEsquinasDerechas, radioEsquinasDerechas, radioEsquinasIzquierdas),
                            Opacity = formato.OpacidadFondoImagen ?? Preferencias.ImageBackgroundOpacity, Background = ObtenerBrocha(Preferencias.ImageBackgroundColor),
                            Margin = new Thickness(margenIzquierda, margenSuperior, margenDerecha, 0)
                        };

                        var cuentaSegmentosEfectivos = 0;
                        foreach (var segmentoEfectivo in segmentosEfectivos) {

                            cuentaSegmentosEfectivos++;

                            if (segmentoEfectivo.Tipo == TipoSegmento.Imagen) {

                                var imagen = new Image {
                                    Source = ObtenerImagen(segmentoEfectivo.Texto), Height = tamañoImagen,
                                    Margin = new Thickness(margenIzquierda, margenSuperior, margenDerecha, 0)
                                }; // El texto del segmentoEfectivo es el nombre de la imagen.

                                if (formato.Posición == PosiciónTexto.Normal && CarácteresComoImágenes.Contains(segmentoEfectivo.Texto[0])) {
                                    imagen.Height = (double)formato.TamañoFuenteEfectiva;
                                    imagen.Margin = new Thickness(0, 0, 0, 0);
                                }

                                if (formato.Posición != PosiciónTexto.Normal) {
                                    imagen.Margin = new Thickness(imagen.Margin.Left + margenAdicionalSubYSuperíndices, imagen.Margin.Top, imagen.Margin.Right, imagen.Margin.Bottom);
                                    imagen.Width = imagen.Height; // Para corregir algunas imagenes que no son cuadradas como el ciervo.
                                }

                                if (instrucción.AlinearInferiormente) imagen.Margin = new Thickness(imagen.Margin.Left + imagen.Height * 0.15, imagen.Margin.Top, imagen.Margin.Right + imagen.Height * 0.15, imagen.Margin.Bottom); // Una pequeña margen adicional a estas imágenes de la parte inferior que normalmente son pequeñas para poder usarlas en el código con texto adjunto a ellas. De esta manera se evita tener que escribir el espacio en el código, que además si se escribiera, suele resultar demasiado grande para la separación entre el texto adjunto y la imagen en esa área.

                                RenderOptions.SetBitmapScalingMode(imagen, BitmapScalingMode.HighQuality);
                                grilla.Children.Add(borde);
                                grilla.Children.Add(imagen);
                                anchoSegmentosEfectivos += tamañoImagen; 
                                grilla.Tag = anchoSegmentosEfectivos;
                                altoSegmento = altoSegmentoImagen; // Así solo tenga una imagen en subíndice o superíndice se toma todo el espacio vertical como si tuviera una imagen normal.
                                //borde.Background = Brushes.Yellow;

                            } else if (segmentoEfectivo.Tipo == TipoSegmento.Texto) {

                                var textBlock = ObtenerTextBlock(segmentoEfectivo.Texto);
                                textBlock.FontStyle = (bool)formato.Cursiva ? FontStyles.Italic : FontStyles.Normal;
                                textBlock.FontWeight = (bool)formato.Negrita ? FontWeights.Bold : FontWeights.Normal;
                                textBlock.FontFamily = new FontFamily((string)formato.NombreFuente);
                                textBlock.Foreground = ObtenerBrocha(formato.ColorHexadecimal);
                                var margenTexto = (margenEnTexto ? formato.TamañoTextoEfectivo * (Preferencias.EntityHorizontalMargin / 200) : 0) ?? 0;
                                if ((bool)formato.Subrayado) textBlock.TextDecorations = TextDecorations.Underline;
                                var desfaceAnchoPorFuente = (formato.NombreFuente == NombreTimesNewRoman ? tamañoImagen * 9 / 99 : 0);
                                var desfaceIzquierdaPorFuente = (formato.NombreFuente == NombreTimesNewRoman ? tamañoImagen * 19.5 / 150 : 0);

                                static void espaciarGuión(TextBlock textBlock) {

                                    if (textBlock.Text == "-") {
                                        textBlock.Text = " - "; // https://www.compart.com/en/unicode/U+2009.
                                        textBlock.FontWeight = FontWeights.Bold;
                                        textBlock.FontFamily = new FontFamily("Tahoma");
                                    }

                                } // espaciarGuión>

                                if (formato.Posición != PosiciónTexto.Normal) {

                                    textBlock.FontFamily = Formato.ObtenerFuentePosiciónEspecial(PosiciónTexto.Normal, segmentoEfectivo.Texto,
                                        formato.NombreFuente, out string? erroresInternos2, númeroPaso); // La función ObtenerFuentePosiciónEspecial es muy flexible porque permite seleccionar la fuente correcta para subíndice o superíndice que tenga los carácteres que se requieren para el texto que se quiere mostrar. Sin embargo, posteriormente se decidió implementar los subíndices y superíndices manualmente para permitir compartir más código con el procedimiento de imágenes en subíndices y superíndices, entonces la función ya no se usa para el objetivo que fue diseñada si no como un creador del objeto fuente que siempre pasa la posición normal en el primer parámetro. Originalmente se pasaba formato.Posición a esta función y se establecía la variante de la fuente correcta en textBlock.Typography.Variants.
                                    AgregarErrores(ref errores, erroresInternos2, númeroPaso: null);

                                    textBlock.Margin = new Thickness(margenAdicionalSubYSuperíndices + desfaceAnchoPorFuente * 2 + margenIzquierda + anchoSegmentosEfectivos - desfaceIzquierdaPorFuente,
                                        0, margenDerecha,
                                        formato.Posición == PosiciónTexto.Subíndice ? -tamañoImagen / (formato.NombreFuente == NombreTimesNewRoman ? 6 : 7) : 0); // El margen inferior es para eliminar el espacio que suelen tener las fuentes en la parte inferior que no se alcanza a eliminar con BlockLineHeight.
                                    textBlock.FontSize = tamañoImagen; // Se establece el texto superíndice y subíndice del mismo tamaño de las imágenes superíndices y subíndices.
                                    textBlock.LineStackingStrategy = LineStackingStrategy.BlockLineHeight; // Esta y la siguiente línea son para forzar el textBlock a que sea del mismo tamaño de la fuente. Ver https://stackoverflow.com/questions/7543846/how-to-remove-additional-padding-from-a-wpf-textblock.
                                    textBlock.LineHeight = textBlock.FontSize * (formato.NombreFuente == NombreTimesNewRoman ? 0.85 : 1.095);
                                    borde.Height = textBlock.LineHeight * 0.905;

                                    if (formato.Posición == PosiciónTexto.Subíndice) {
                                        textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                                    } else if (formato.Posición == PosiciónTexto.Superíndice) {
                                        textBlock.VerticalAlignment = VerticalAlignment.Top;
                                    }

                                    espaciarGuión(textBlock);
                                    anchoSegmentosEfectivos += textBlock.Ancho();
                                    grilla.Tag = anchoSegmentosEfectivos; // No funciona bien cuando el ancho es muy largo y se está combinando con un super/subíndice imagen y la imagen se puso segunda en el código, así: [explore]<x>Opponent<p>[sc]<>, la solución por el momento es escribir el código así [explore]<x>Opponent<p>[sc]<>. Cuando son dos textos uno en subíndice y otro en superíndice si funciona bien.
                                    //textBlock.Background = Brushes.Fuchsia;

                                } else {

                                    textBlock.FontSize = (double)formato.TamañoFuenteEfectiva;
                                    textBlock.Margin = new Thickness(margenTexto, 0, margenTexto, 0);
                                    textBlock.VerticalAlignment = VerticalAlignment.Center; // Es preferible el alineamiento central para facilitar el posicionamiento de las imágenes. Además, el alineamiento inferior no coincide exactamente con la línea base de los textos cuando se usan diferentes tamaños de fuente, entonces tampoco es muy útil. 
                                    //textBlock.Background = Brushes.Red;

                                }

                                if (segmentosEfectivos.Count() > 1) {
                                    borde.Margin = new Thickness(borde.Margin.Left + 0.17 * desfaceAnchoPorFuente, borde.Margin.Top, borde.Margin.Right, borde.Margin.Bottom);
                                } else {
                                    borde.Margin = new Thickness(borde.Margin.Left + desfaceAnchoPorFuente / 2, borde.Margin.Top, borde.Margin.Right, borde.Margin.Bottom);
                                }

                                borde.Width = anchoSegmentosEfectivos + desfaceAnchoPorFuente + margenAdicionalSubYSuperíndices;
                                if (borde.Width < tamañoImagen && ambosÍndices) borde.Width = borde.Height + margenAdicionalSubYSuperíndices; // Se corrige el ancho del borde para los casos en los que el texto es muy pequeño y se tiene otro sub/superíndice para que el ancho del borde del texto quede igual al del otro sub/superíndice que suele ser una imagen. No se corrije en los casos que está solo para no generar espacios innecesarios.

                                if (formato.Posición == PosiciónTexto.Superíndice) {
                                    borde.Margin = new Thickness(borde.Margin.Left, borde.Margin.Top - tamañoImagen * (segmentosEfectivos.Count() > 1 ? 2.5 : 7.65) / 97, borde.Margin.Right, borde.Margin.Bottom);
                                }

                                if (formato.Posición != PosiciónTexto.Normal && !grilla.Children.Contains(borde)) grilla.Children.Add(borde); // Solo agrega los bordes si es superíndice o subíndice. Principalmente se usan al lado de imágenes entonces tiene sentido que el borde su color de fondo lo comparta con la imagen adjunta. Si requiriera un uso común al lado de texto, se tendría que agregar la excepción.
                                grilla.Children.Add(textBlock);
                                altoSegmento = altoSegmentoTexto;

                            }

                            if (altoSegmento > altoInstrucción) altoInstrucción = altoSegmento;

                        } // foreach segmentoEfectivo>

                        spnHorizontal.Children.Add(grilla);

                    } // foreach segmentos>

                    if (simulación) continue;

                    altoInstrucción += mayorEspaciadoVertical;
                    sumaAlto += altoInstrucción;
                    if (sumaAlto > (altoMáximo + margenInferior)) superóAlto = true; // Para efectos de la alerta por contenido muy largo se tiene en cuenta el margen para que quede la alerta justo en el límite de ocultarse la primera parte inferior del último paso.
                    if (superóAlto) { foreach (var child in spnHorizontal.Children) { } } //if (child is TextBlock textblock) textblock.Foreground = ObtenerBrocha("#F00");  No borrar. Se usa para el desarrollo.
                    spnHorizontal.Margin = new Thickness(0, 0, 0, mayorEspaciadoVertical);

                    if (instrucción.AlinearInferiormente) {

                        var altoRestante = altoMáximo - sumaAlto;
                        if (altoRestante > 0) {
                            var spnRelleno = new StackPanel() { Height = altoRestante };
                            contenedor.Children.Add(spnRelleno);
                        }
                        //spnHorizontal.Background = Brushes.Fuchsia;

                    }
                    contenedor.Children.Add(spnHorizontal);

                } // foreach instrucción>

            } else { // paso != null>

                if (!!simulación && editorCódigo != null) {
                    editorCódigo.Tag = "cambiando-texto-desde-código";
                    editorCódigo.Text = null;
                    editorCódigo.Tag = null;
                }

            }

        } // MostrarPaso>


        public SolidColorBrush ObtenerBrocha(string colorHexadecimal) {

            if (!BrochasDeColorSólido.ContainsKey(colorHexadecimal))
                BrochasDeColorSólido.Add(colorHexadecimal, new SolidColorBrush((Color)ObtenerMediaColor(colorHexadecimal)!)); // Se almacena en diccionario para no duplicar el objeto en memoria.
            return BrochasDeColorSólido[colorHexadecimal];

        } // ObtenerBrocha>


        public BitmapImage? ObtenerImagen(string nombreImagen) {

            if (!Imágenes.ContainsKey(nombreImagen))
                return null;

            if (!Bitmaps.ContainsKey(nombreImagen))
                Bitmaps.Add(nombreImagen, new BitmapImage(new Uri(Imágenes[nombreImagen]))); // Se almacena en diccionario para no duplicar el objeto en memoria y para no generarlo cada vez pues se puede demorar alrededor de 50 ms.
            return Bitmaps[nombreImagen];

        } // ObtenerImagen>


        public TextBlock ObtenerTextBlock(string texto) {

            if (texto.Contains("http", StringComparison.InvariantCultureIgnoreCase)) {

                var textBlock = new TextBlock();
                var matches = Regex.Matches(texto, @"(https?:\/\/[^ :]+):*([^ :]*)");
                var índiceActual = 0;
                foreach (Match? match in matches) {
                    if (match != null) {

                        var índiceMatch = match.Index;
                        var enlace = new Hyperlink();
                        var url = match.Groups[1].Value;

                        if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)) {

                            enlace.NavigateUri = new Uri(url);
                            var textoEnlace = match.Groups[2].Value.Replace("_", " ");
                            enlace.Inlines.Add(string.IsNullOrWhiteSpace(textoEnlace) ? url : textoEnlace);
                            enlace.RequestNavigate += Enlace_RequestNavigate;
                            var textoPlano = texto.Substring(índiceActual, índiceMatch - índiceActual);
                            textBlock.Inlines.Add(new Run(textoPlano));
                            textBlock.Inlines.Add(enlace);
                            índiceActual = índiceMatch + match.Value.Length;

                        }

                    }
                }
                if (índiceActual <= texto.Length) textBlock.Inlines.Add(new Run(texto.Substring(índiceActual)));
                return textBlock;

            } else {
                return new TextBlock { Text = texto };
            }

        } // ObtenerTextBlock>


        static void Enlace_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {

            try {
                Process.Start(new ProcessStartInfo(e.Uri.ToString()) { UseShellExecute = true });
            } catch { }

        } // Enlace_RequestNavigate>


        #endregion Funciones y Procedimientos>



    } // Estrategia>



} // RTSHelper>
