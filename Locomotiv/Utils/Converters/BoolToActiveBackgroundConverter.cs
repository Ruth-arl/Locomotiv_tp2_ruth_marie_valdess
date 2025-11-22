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
    /// Convertisseur WPF qui transforme un booléen en couleur de fond.
    /// True = couleur bleue claire (#E3F2FD), False = blanc.
    /// Utilisé pour indiquer visuellement l'état actif d'un élément.
    /// </summary>
    public class BoolToActiveBackgroundConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en SolidColorBrush pour le fond.
        /// </summary>
        /// <param name="value">Valeur booléenne indiquant si l'élément est actif.</param>
        /// <returns>Brush bleu clair si actif, blanc sinon.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive && isActive)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD"));
            }
            return new SolidColorBrush(Colors.White);
        }

        /// <summary>
        /// Conversion inverse non implémentée.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
