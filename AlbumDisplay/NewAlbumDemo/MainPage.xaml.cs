using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Data;
using Windows.Storage;
using System.Text;
using Windows.Storage.Provider;
using Windows.Storage.Pickers;
using System.Reflection.Metadata;
using System.Collections.ObjectModel;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using NewAlbumDemo.Controls;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NewAlbumDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        FileOpenPicker openPicker = new FileOpenPicker();
        ObservableCollection<ModelAlbum> m_items = new ObservableCollection<ModelAlbum>();
        MySQLHelper mysql = new MySQLHelper();
        StorageFile FilePicture;
        int allCount;
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (mysql.getInstance() != null)
                mysql.CloseConnection();
            m_items.Clear();
            MySqlConnectionStringBuilder s = new MySqlConnectionStringBuilder();
            s.Server = "localhost";
            s.UserID = "root";
            s.Database = "album";
            s.Password = "mysql2018";
            mysql.Initialize(s.Server, s.Database, s.UserID, s.Password);
            if (mysql.OpenConnection())
            {
                var cmd = mysql.CreateCmd(new ModelAlbum(), EnumStatus.Query);
                var dt = mysql.GetDataTable(cmd.CommandText, "album");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ModelAlbum temp = new ModelAlbum();
                    temp.photoID = int.Parse(dt.Rows[i][0].ToString());
                    temp.photoCaption = dt.Rows[i][1].ToString();
                    temp.photoURL = new Uri(dt.Rows[i][2].ToString());
                    //byte[] bs =((Blob)dt.Rows[i][2]).GetBytes().ToArray();
                    //temp.imageSource =await UrlToImageSource(dt.Rows[i][2].ToString());
                    temp.photoDescription = dt.Rows[i][3].ToString();
                    temp.labels = dt.Rows[i][4].ToString().TrimEnd(',').Split(',').Select(x => new LabelItem(x)).ToList();
                    m_items.Add(temp);
                }
                flipView.ItemsSource = m_items;
               // sumCount.Text
                    allCount = m_items.Count;
            }
            if (e.Parameter != null && (e.Parameter is int))
            {
                flipView.SelectedIndex = (int)e.Parameter;
            }
        }
        private async void AddPicture_Click(object sender, RoutedEventArgs e)
        {
            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");
            FilePicture = await openPicker.PickSingleFileAsync();
            if (FilePicture != null)
            {
                uploadControl.uploadImage(FilePicture);
                //.imageTitle.Text = FilePicture.Name;
                uploadBtn.Visibility = Visibility.Visible;
            }

            //if (FilePicture != null)
            //{
            //    StorageFolder folder = ApplicationData.Current.LocalFolder;
            //    //StorageFolder picturesLibrary = await KnownFolders.GetFolderForUserAsync(null /* current user */, KnownFolderId.PicturesLibrary);
            //    StorageFile fileCopy = await FilePicture.CopyAsync(folder, FilePicture.Name, NameCollisionOption.GenerateUniqueName);
            //    ModelAlbum model = new ModelAlbum();
            //    model.photoCaption = FilePicture.Name;
            //    model.photoURL = new Uri(String.Format("{0}", fileCopy.Path));
            //    model.photoDescription = "It is the photo with name of " + model.photoCaption;

            //    uploadBtn.Visibility = Visibility;
            //    tempImage.Source = model.photoURL;
            //    //model.labels.Add(new LabelItem("lable1"));
            //    //model.labels.Add(new LabelItem("lable2"));
            //    // mysql.getInsert(mysql.CreateCmd(model, EnumStatus.Insert));
            //}
            // this.Frame.Navigate(typeof(MainPage));
        }

        private async void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FilePicture != null)
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    //StorageFolder picturesLibrary = await KnownFolders.GetFolderForUserAsync(null /* current user */, KnownFolderId.PicturesLibrary);
                    
                    ModelAlbum model = new ModelAlbum();
                    model = uploadControl.saveImageInfo(model);
                    StorageFile fileCopy = await FilePicture.CopyAsync(folder, model.photoCaption, NameCollisionOption.GenerateUniqueName);
                    //  model.photoCaption = fileCopy.Name;
                    model.photoURL = new Uri(String.Format("{0}", fileCopy.Path));
                    //  uploadControl.
                    // model.photoDescription = descriptionTxt.Text;
                    //model.photoDescription = descriptionTxt.Document.ToString(); 
                    //model.photoDescription = "It is the photo with name of " + model.photoCaption;
                    //    labelsPanel.Children.ToList().ForEach(item=>{
                    //    if(item is TextBox)
                    //        model.labels.Add(new LabelItem(((TextBox)item).Text));
                    //});
                    mysql.getInsert(mysql.CreateCmd(model, EnumStatus.Insert));                   
                    this.Frame.Navigate(typeof(MainPage), flipView.Items.Count);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }
        public async Task<BitmapImage> UrlToImageSource(string url)
        {
            StorageFile imageFile = await StorageFile.GetFileFromPathAsync(url);
            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                // fileStream.AsStream().CopyTo(stream);

                return bitmapImage;
            }
        }


        private void AddMarkBtn_Click(object sender, RoutedEventArgs e)
        {
            var model = (ModelAlbum)flipView.SelectedItem;
            // AddMarkBtn
            //var ite =  flipView.ItemContainerGenerator.ContainerFromIndex(flipView.SelectedIndex) as FlipViewItem;
            // var txt =(flipView.FindName("newLabel") as TextBlock).Text;
            // model.labels.Add(new LabelItem(txt));
            MySQLHelper.getUpdate(mysql.CreateCmd(model, EnumStatus.Update));
            this.Frame.Navigate(typeof(MainPage), flipView.SelectedIndex);
        }

        private void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            var model = (ModelAlbum)flipView.SelectedItem;
            MySQLHelper.getDel(mysql.CreateCmd(model, EnumStatus.Delete));
            this.Frame.Navigate(typeof(MainPage));
        }

        private void labelTextBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.ShowAttachedFlyout(sender as TextBlock);
        }
        private void GotoHome_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void AppBarButton_Click_EditLabel(object sender, RoutedEventArgs e)
        {
            var model = (ModelAlbum)flipView.SelectedItem;
            EditLabels dialog = new EditLabels(model.labels);
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {

                model.labels = dialog.AppBarButton_Click_Edit(sender, e);
                MySQLHelper.getUpdate(mysql.CreateCmd(model, EnumStatus.Update));
                Frame.Navigate(typeof(MainPage), flipView.SelectedIndex);
            }

        }

        private void AppBarButton_Click_SaveAllChange(object sender, RoutedEventArgs e)
        {
            var model = (ModelAlbum)flipView.SelectedItem;
            //var ite = flipView.ItemContainerGenerator.ContainerFromIndex(flipView.SelectedIndex) as FlipViewItem;
            //var txt = (flipView.FindName("newLabel") as TextBlock).Text;
            //  model.labels.Add(new LabelItem(txt));
            MySQLHelper.getUpdate(mysql.CreateCmd(model, EnumStatus.Update));
            
            this.Frame.Navigate(typeof(MainPage), flipView.SelectedIndex);
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            var panel = flipView.ItemsPanel;
            var index = flipView.SelectedIndex;
            var parent = Frame.FindName("itemRoot");
            for (int i = 0; i < flipView.Items.Count; i++)
            {
                FlipViewItem item = flipView.ItemContainerGenerator.ContainerFromIndex(i) as FlipViewItem;
                if (item != null)
                {
                    item.IsEnabled = false;
                }
                else
                {
                    //flipView.UpdateLayout();
                    //flipView.SelectedIndex = i;
                    //i--;
                }
            }
        }

        private void leftArrow_Click(object sender, RoutedEventArgs e)
        {
            flipView.SelectedIndex--;

        }
        private void rightArrow_Click(object sender, RoutedEventArgs e)
        {
            flipView.SelectedIndex++;
           

        }

        private void selectIndex_Changed(object sender,SelectionChangedEventArgs e)
        {
            var itemoffset = thumbsScroll.ExtentWidth / flipView.Items.Count;
            var currentItemOffset = itemoffset * flipView.SelectedIndex;
            if (currentItemOffset > thumbsScroll.ActualWidth - itemoffset)
                thumbsScroll.ChangeView(currentItemOffset - thumbsScroll.ActualWidth + itemoffset, null, null, true);
            else if (currentItemOffset + thumbsScroll.ActualWidth < thumbsScroll.ExtentWidth)
                thumbsScroll.ChangeView(thumbsScroll.HorizontalOffset - itemoffset, null, null, true);

        }
        
    }
}
