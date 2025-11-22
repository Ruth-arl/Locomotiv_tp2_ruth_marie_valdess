using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Locomotiv.Utils.Converters
{
    /// <summary>
    /// Convertisseur WPF qui transforme un booléen en couleur de bordure.
    /// True = bleu (#2196F3), False = gris (#CCCCCC).
    /// Utilisé pour indiquer visuellement l'état actif d'un élément.
    /// </summary>
    public class BoolToActiveBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive && isActive)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"));
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
