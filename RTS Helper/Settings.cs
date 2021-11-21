using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.IO;



namespace RTSHelper {



    public class Settings {


        #region "Constantes"

        public static string AOE2Name = "Age of Empires II";

        public static string AOE4Name = "Age of Empires IV";

        #endregion


        #region "Generales"

        public string Game { get; set; } = AOE2Name; // Es una pseudopreferencia porque no se usa por si sola si no para establecer otros valores.

        public string ScreenResolution { get; set; } = "1920x1080"; // Es una pseudopreferencia porque no se usa por si sola si no para establecer otros valores.

        public double GameSpeed { get; set; } = 1;

        public double ExecutionSpeed { get; set; } = 1; // 1 es la máxima velocidad. En esta velocidad cada conjunto de comandos corresponde a un minuto en el juego.

        public string CurrentBuildOrder { get; set; } = "Default";

        public bool ShowNextStep { get; set; }

        public string? BuildOrderDirectory { get; set; }

        public bool MinimizeOnComplete { get; set; } = true;

        #endregion 


        #region "Sonidos"

        public string StepStartSound { get; set; } = "Ticket Machine 2.mp3";

        public string StepEndSound { get; set; } = "Stopwatch Shorter.mp3";

        public int StepStartSoundVolume { get; set; } = 50;

        public int StepEndSoundVolume { get; set; } = 20;

        public int StepEndSoundDuration { get; set; } = 0; // Se establece automáticamente al cambiar el sonido. El valor

        public bool UnmuteAtStartup { get; set; } = true; // Permite que no se mantenga muted si la última vez que se usó la aplicación se cerró muted.

        public bool Muted { get; set; } = false; // Se establece desde la interface principal.

        public bool MuteOnComplete { get; set; } = true;

        #endregion


        #region "Colores"

        public string BackColor { get; set; } = Color.FromRgb(0, 0, 0).ToString();

        public string FontColor { get; set; } = Color.FromRgb(150, 150, 150).ToString();

        public double Opacity { get; set; } = 0.6;

        public string CurrentStepFontColor { get; set; } = Color.FromRgb(220, 220, 220).ToString();

        public string NextStepFontColor { get; set; } = Color.FromRgb(150, 150, 150).ToString();

        public bool FlashOnStepChange { get; set; } = true;

        public string FlashingColor { get; set; } = Color.FromRgb(255, 255, 255).ToString();

        public double FlashingOpacity { get; set; } = 1;

        public bool StopFlashingOnComplete { get; set; } = true;

        #endregion


        #region "Tamaños" 
        // Son establecidos automáticamente al cambiar el juego y la resolución.

        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public double CurrentStepFontSize { get; set; }

        public double NextStepFontSize { get; set; }

        public double ButtonsSize { get; set; }

        public double ButtonsMargin { get; set; }

        public double ButtonsPadding { get; set; }

        public double LargeFontSize { get; set; }

        public double MediumFontSize { get; set; }

        public double LeftMarginCurrentStep { get; set; }

        public double TopMarginCurrentStep { get; set; }

        public double RightMarginNextStep { get; set; }

        public double TopMarginNextStep { get; set; }

        public double BuildOrderSelectorWidth { get; set; }

        public double ExecutionSpeedSelectorWidth { get; set; }

        #endregion


        public string ObtenerGameSpeedText(string game) {

            if (game == AOE2Name) {
                return (GameSpeed == 1.7 ? "Normal" : (GameSpeed == 1 ? "Slow" : (GameSpeed == 1.5 ? "Casual" : (GameSpeed == 2 ? "Fast" : "Other"))));
            } else {
                return "Normal";
            }

        } // ObtenerGameSpeedText>


        public void EstablecerGameSpeed(string gameSpeedText, string game) {

            if (game == AOE2Name) {
                GameSpeed = (gameSpeedText == "Normal" ? 1.7 : (gameSpeedText == "Slow" ? 1 : (gameSpeedText == "Casual" ? 1.5 : (gameSpeedText == "Fast" ? 2 : 1))));
            } else {
                GameSpeed = 1;
            }

        } // EstablecerGameSpeed>


        public void EstablecerValoresRecomendadosAOE2(string resolución) {

            ShowNextStep = true;
            GameSpeed = 1.7;

            switch (resolución) {
                case "1920x1080":

                    Height = 146;
                    Width = 495;
                    Top = 718;
                    Left = 722;
                    CurrentStepFontSize = 19;
                    NextStepFontSize = 11.5;
                    ButtonsSize = 24;
                    ButtonsMargin = 2;
                    ButtonsPadding = 2;
                    LargeFontSize = 16;
                    MediumFontSize = 14;
                    LeftMarginCurrentStep = 15;
                    TopMarginCurrentStep = 3;
                    TopMarginNextStep = 27;
                    RightMarginNextStep = 67;
                    BuildOrderSelectorWidth = 145;
                    ExecutionSpeedSelectorWidth = 55;
                    break;

                case "2560x1440":

                    Height = 195;
                    Width = 662; // No cambiar, normalmente uso el minimapa 25% más grande y el valor mi valor sería 635.
                    Top = 957;
                    Left = 962;
                    CurrentStepFontSize = 25;
                    NextStepFontSize = 16;
                    ButtonsSize = 33;
                    ButtonsMargin = 3;
                    ButtonsPadding = 2;
                    LargeFontSize = 24;
                    MediumFontSize = 18;
                    LeftMarginCurrentStep = 17;
                    TopMarginCurrentStep = 5;
                    TopMarginNextStep = 31;
                    RightMarginNextStep = 90;
                    BuildOrderSelectorWidth = 200;
                    ExecutionSpeedSelectorWidth = 75;
                    break;

                case "1366x768":

                    Height = 105;
                    Width = 354;
                    Top = 510;
                    Left = 513;
                    CurrentStepFontSize = 13.5;
                    NextStepFontSize = 8.3;
                    ButtonsSize = 17;
                    ButtonsMargin = 1;
                    ButtonsPadding = 1;
                    LargeFontSize = 12;
                    MediumFontSize = 10;
                    LeftMarginCurrentStep = 8;
                    TopMarginCurrentStep = 2;
                    TopMarginNextStep = 15;
                    RightMarginNextStep = 45;
                    BuildOrderSelectorWidth = 110;
                    ExecutionSpeedSelectorWidth = 40;
                    break;

                default:
                    break;
            }

        } // EstablecerValoresRecomendadosAOE2>


        public void EstablecerValoresRecomendados(string resolución, string juego) {

            Game = juego;
            ScreenResolution = resolución;

            if (juego == AOE2Name) {

                EstablecerValoresRecomendadosAOE2(resolución);

            } else if (juego == AOE4Name) {

                EstablecerValoresRecomendadosAOE2(resolución);
                GameSpeed = 1;
                ShowNextStep = false;

                switch (resolución) {
                    case "1920x1080":

                        Height = 146;
                        Width = 263;
                        Top = 718;
                        Left = 1044;
                        CurrentStepFontSize = 14.5;
                        BuildOrderSelectorWidth = 140;
                        break;

                    case "2560x1440":

                        Width = 355;
                        Left = 1389;
                        Height = 195;
                        Top = 957;
                        CurrentStepFontSize = 19.5;
                        BuildOrderSelectorWidth = 185;
                        break;

                    case "1366x768":

                        Height = 105;
                        Width = 188;
                        Top = 510;
                        Left = 743;
                        CurrentStepFontSize = 10;
                        BuildOrderSelectorWidth = 97;
                        break;

                    default:
                        break;
                }

            }

        } // EstablecerValoresRecomendados>


        public static Settings? Deserializar(string json) 
            => (string.IsNullOrEmpty(json)) ? default : JsonSerializer.Deserialize<Settings>(json);


        public static string Serializar(Settings settings) 
            => JsonSerializer.Serialize(settings);


        public static void Guardar(Settings settings, string ruta) 
            => File.WriteAllText(ruta, Settings.Serializar(settings));


    } // Settings>


} // RTSHelper>
