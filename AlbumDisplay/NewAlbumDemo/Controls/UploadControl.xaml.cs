using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NewAlbumDemo.Controls
{
    public sealed partial class UploadControl : UserControl
    {
        private List<LabelItem> LabelList { get; set; }
        public UploadControl()
        {
            this.InitializeComponent();
            LabelList = new List<LabelItem>();
        }

        private void imageTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.ShowAttachedFlyout(sender as TextBlock);
        }

        private void AppBarButton_Click_AddLabel(object sender, RoutedEventArgs e)
        {
            LabelItem item = new LabelItem("");
            LabelList = tempLabels.Items.Select(x=>(x as LabelItem)).ToList();
            LabelList.Add(item);
            tempLabels.ItemsSource = LabelList;
        }
        public  async void uploadImage(StorageFile FilePicture)
        {
            MyTextBox.Text = FilePicture.Name;
            using (IRandomAccessStream fileStream = await FilePicture.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                tempImage.Source = bitmapImage;
            }
        }
        public ModelAlbum saveImageInfo(ModelAlbum model)
        {
            model.photoDescription = descriptionTxt.Text;
            //tempLabels.Items.ToList().ForEach((item as LabelItem) =>{
            //})
            model.labels = LabelList.Where(x => !String.IsNullOrEmpty(x.value)).ToList();
            model.photoCaption = imageTitle.Text;
            return model;
        }
    }
}
