using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



namespace RTSHelper {



    public class HoverButton : Button { // https://stackoverflow.com/questions/20073294/change-color-of-button-when-mouse-is-over.


        public static readonly DependencyProperty hoverColorProperty = DependencyProperty.Register("hoverColor", typeof(SolidColorBrush), 
            typeof(HoverButton), new PropertyMetadata(new BrushConverter().ConvertFrom("#5D5D5D")));


        public SolidColorBrush hoverColor {
            get => (SolidColorBrush)GetValue(hoverColorProperty);
            set => SetValue(hoverColorProperty, value);
        }


        public static readonly DependencyProperty bgColorProperty = DependencyProperty.Register("bgColor", typeof(SolidColorBrush), typeof(HoverButton),
            new PropertyMetadata(new SolidColorBrush(Colors.Red)));


        public SolidColorBrush bgColor {
            get => (SolidColorBrush)GetValue(bgColorProperty);
            set => SetValue(bgColorProperty, value);
        }


        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), 
            typeof(HoverButton), new PropertyMetadata(new CornerRadius()));


        public CornerRadius CornerRadius {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }


    } // HoverButton>



} // RTSHelper>