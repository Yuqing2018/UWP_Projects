using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NewAlbumDemo
{
    public sealed class SelectedIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int)
                return ((int)value + 1);
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int result;
            if (int.TryParse(value.ToString(), out result))
                return (result - 1);
            else
                throw new NotImplementedException("Converting from string is not supported.");
        }
    }
    public sealed class SelectedIndexToBoolConverter : DependencyObject, IValueConverter
    {
        public int itemCount
        {
            get { return (int)GetValue(itemCountProperty); }
            set { SetValue(itemCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for paramString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty itemCountProperty =
            DependencyProperty.Register("itemCount", typeof(int), typeof(SelectedIndexToBoolConverter), new PropertyMetadata(0));



        public string controlName
        {
            get { return (string)GetValue(controlNameProperty); }
            set { SetValue(controlNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for controlName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty controlNameProperty =
            DependencyProperty.Register("controlName", typeof(string), typeof(SelectedIndexToBoolConverter), new PropertyMetadata(String.Empty));


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int)
            {
                if (controlName == "leftArrow")
                    return ((int)value > 0);
                else if (controlName == "rightArrow")
                    return (int)value < (itemCount - 1);
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is int)
                return value;
            //int result;
            //if (int.TryParse(value.ToString(), out result))
            //    return (result - 1);
            else
                throw new NotImplementedException("Converting from string is not supported.");
        }

    }

    public sealed class VisibleIfTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible);
        }
    }

    public sealed class CollapsedIfTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Collapsed);
        }
    }
}
