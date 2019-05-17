using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace NewAlbumDemo
{
    public class ModelAlbum
    {
        public ModelAlbum()
        {
            labels = new List<LabelItem>();
        }
        public int photoID { get; set; }
        public string photoCaption { get; set; }
        public Uri photoURL { get; set; }
        public string photoPath { get; set; }
        public string photoDescription { get; set; }
        public List<LabelItem> labels { get; set; }
        public BitmapImage imageSource { get; set; }
        public EnumStatus SqlStatu { get; set; }
    }
    public class LabelItem
    {
        public LabelItem(string str)
        {
            value = str;
        }
        public string value { get; set; }
    }
    public enum EnumStatus
    {
        Insert = 0,
        Delete = 1,
        Update = 2,
        Query = 3,
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
        
        public string iconName { get; set; }
    }

    public sealed class ScenarioConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Scenario)
            {
                return (value as Scenario).Title;
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
                throw new NotImplementedException("Converting from string is not supported.");
        }
    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
}
