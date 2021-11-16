using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;



namespace RTSHelper {



    public class Settings {


        public static string AOE2Name = "Age of Empires II";

        public static string AOE4Name = "Age of Empires IV";

        public string Game { get; set; } = AOE2Name; // Es una pseudopreferencia porque no se usa por si sola si no para establecer otros valores.

        public string ScreenResolution { get; set; } = "1920x1080"; // Es una pseudopreferencia porque no se usa por si sola si no para establecer otros valores.

        public double GameSpeed { get; set; } = 1.7;

        public double ExecutionSpeed { get; set; } = 1; // 1 es la máxima velocidad. En esta velocidad cada set de instrucciones corresponde a un minuto en el juego.

        public string CurrentBuildOrder { get; set; } = "Default";

        public double CurrentStepFontSize { get; set; } = 19; // 1920x1080.

        public double NextStepFontSize { get; set; } = 12.5; // 1920x1080.

        public string BackColor { get; set; } = Color.FromRgb(0, 0, 0).ToString();

        public string FontColor { get; set; } = Color.FromRgb(150, 150, 150).ToString();

        public double Opacity { get; set; } = 0.6;

        public string CurrentStepFontColor { get; set; } = Color.FromRgb(220, 220, 220).ToString();

        public string NextStepFontColor { get; set; } = Color.FromRgb(150, 150, 150).ToString();

        public double Width { get; set; } = 472; // 1920x1080.

        public double Height { get; set; } = 146; // 1920x1080.

        public double Top { get; set; } = 718; // 1920x1080.

        public double Left { get; set; } = 722; // 1920x1080.

        public bool ShowNextStep { get; set; } = true; // AOE2.

        public string BuildOrderDirectory { get; set; } = null;

        public int ButtonsMargin { get; set; } = 2; // 1920x1080.

        public int ButtonsPadding { get; set; } = 2; // 1920x1080.

        public double LargeFontSize { get; set; } = 17; // 1920x1080.

        public double MediumFontSize { get; set; } = 15; // 1920x1080.

        public double SmallFontSize { get; set; } = 11; // 1920x1080.

        public double LeftMarginCurrentStep { get; set; } = 14; // 1920x1080.

        public double TopMarginCurrentStep { get; set; } = 6; // 1920x1080.

        public double RightMarginNextStep { get; set; } = 65; // 1920x1080.

        public double TopMarginNextStep { get; set; } = 4; // 1920x1080.

        public bool PlaySoundEachStep { get; set; } = false;


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


        public void EstablecerValoresRecomendados(string resolución, string juego) {

            Game = juego;
            ScreenResolution = resolución;

            if (juego == AOE2Name) {

                ShowNextStep = true;
                GameSpeed = 1.7;

                switch (resolución) {
                    case "1920x1080":

                        Height = 146;
                        Width = 495;
                        Top = 718;
                        Left = 722;
                        SmallFontSize = 11;
                        MediumFontSize = 15;
                        LargeFontSize = 17;
                        CurrentStepFontSize = 19;
                        NextStepFontSize = 12.5;
                        ButtonsMargin = 2;
                        ButtonsPadding = 2;
                        LeftMarginCurrentStep = 14;
                        TopMarginCurrentStep = 6;
                        TopMarginNextStep = 4;
                        RightMarginNextStep = 65;
                        break;

                    case "2560x1440":

                        Height = 195;
                        Width = 662; // No cambiar, normalmente uso el minimapa 25% más grande y el valor mi valor sería 635.
                        Top = 957;
                        Left = 962;
                        SmallFontSize = 13;
                        MediumFontSize = 16;
                        LargeFontSize = 22;
                        CurrentStepFontSize = 24;
                        NextStepFontSize = 16;
                        ButtonsMargin = 4;
                        ButtonsPadding = 5;
                        LeftMarginCurrentStep = 20;
                        TopMarginCurrentStep = 10;
                        TopMarginNextStep = 8;
                        RightMarginNextStep = 90;
                        break;

                    case "1366x768":

                        Height = 105;
                        Width = 354;
                        Top = 510;
                        Left = 513;
                        SmallFontSize = 8;
                        MediumFontSize = 9;
                        LargeFontSize = 11;
                        CurrentStepFontSize = 13;
                        NextStepFontSize = 8.5;
                        ButtonsMargin = 1;
                        ButtonsPadding = 2;
                        LeftMarginCurrentStep = 11;
                        TopMarginCurrentStep = 4;
                        TopMarginNextStep = 3;
                        RightMarginNextStep = 50;
                        break;

                    default:
                        break;
                }

            } else if (juego == AOE4Name) {

                GameSpeed = 1;
                ShowNextStep = false;

                switch (resolución) {
                    case "1920x1080":

                        Height = 146;
                        Width = 263;
                        Top = 718;
                        Left = 1044;
                        SmallFontSize = 11;
                        MediumFontSize = 13;
                        LargeFontSize = 16;
                        CurrentStepFontSize = 15;
                        NextStepFontSize = 9;
                        ButtonsMargin = 3;
                        ButtonsPadding = 2;
                        LeftMarginCurrentStep = 15;
                        TopMarginCurrentStep = 6;
                        TopMarginNextStep = 3;
                        RightMarginNextStep = 50;
                        break;

                    case "2560x1440":

                        Width = 355;
                        Left = 1389;
                        Height = 195;
                        Top = 957;
                        SmallFontSize = 12;
                        MediumFontSize = 12;
                        LargeFontSize = 16;
                        CurrentStepFontSize = 20;
                        NextStepFontSize = 16;
                        ButtonsMargin = 3;
                        ButtonsPadding = 3;
                        LeftMarginCurrentStep = 20;
                        TopMarginCurrentStep = 10;
                        TopMarginNextStep = 8;
                        RightMarginNextStep = 90;
                        break;

                    case "1366x768":

                        SmallFontSize = 8;
                        MediumFontSize = 9;
                        LargeFontSize = 11;
                        CurrentStepFontSize = 10;
                        NextStepFontSize = 8.5;
                        ButtonsMargin = 1;
                        ButtonsPadding = 2;
                        LeftMarginCurrentStep = 11;
                        TopMarginCurrentStep = 4;
                        TopMarginNextStep = 3;
                        RightMarginNextStep = 50;
                        Height = 105;
                        Width = 188;
                        Top = 510;
                        Left = 743;
                        break;

                    default:
                        break;
                }

            }

        } // EstablecerValoresRecomendados>


        public static Settings Deserializar(string json) 
            => (string.IsNullOrEmpty(json)) ? default : JsonSerializer.Deserialize<Settings>(json);


        public static string Serializar(Settings settings) 
            => JsonSerializer.Serialize(settings);


        public static void Guardar(Settings settings, string ruta) 
            => System.IO.File.WriteAllText(ruta, Settings.Serializar(settings));


    } // Settings>


} // RTSHelper>
