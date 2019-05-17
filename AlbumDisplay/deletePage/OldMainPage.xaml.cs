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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NewAlbumDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OldMainPage : Page
    {
        private Stream stream = new MemoryStream();
        FileOpenPicker openPicker = new FileOpenPicker();
        ObservableCollection<ModelAlbum> m_items = new ObservableCollection<ModelAlbum>();
        MySQLHelper mysql = new MySQLHelper();
        StorageFile FilePicture;
        public OldMainPage()
        {
            this.InitializeComponent();
            //  splitView.IsPaneOpen = true;
            // listview.ItemsSource = m_items;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
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
                listview.ItemsSource = m_items;
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
                imageTitle.Text = FilePicture.Name;
                uploadBtn.Visibility = Visibility;
                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await FilePicture.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    fileStream.AsStream().CopyTo(stream);
                    tempImage.Source = bitmapImage;

                }
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
                    StorageFile fileCopy = await FilePicture.CopyAsync(folder, FilePicture.Name, NameCollisionOption.GenerateUniqueName);
                    ModelAlbum model = new ModelAlbum();
                    model.photoCaption = fileCopy.Name;
                    model.photoURL = new Uri(String.Format("{0}", fileCopy.Path));
                    model.photoDescription = descriptionTxt.DataContext.ToString();
                    //model.photoDescription = "It is the photo with name of " + model.photoCaption;
                    labelsPanel.Children.ToList().ForEach(item => {
                        if (item is TextBox)
                            model.labels.Add(new LabelItem(((TextBox)item).Text));
                    });
                    mysql.getInsert(mysql.CreateCmd(model, EnumStatus.Insert));
                    this.Frame.Navigate(typeof(MainPage));
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
                fileStream.AsStream().CopyTo(stream);

                return bitmapImage;
            }
        }


        private void AddMarkBtn_Click(object sender, RoutedEventArgs e)
        {
            var model = (ModelAlbum)listview.SelectedItem;
            model.labels.Add(new LabelItem(newMark.Text));
            MySQLHelper.getUpdate(mysql.CreateCmd(model, EnumStatus.Update));
            this.Frame.Navigate(typeof(MainPage));
        }

        private void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            var model = (ModelAlbum)listview.SelectedItem;
            MySQLHelper.getDel(mysql.CreateCmd(model, EnumStatus.Delete));
            this.Frame.Navigate(typeof(MainPage));
        }
        #region 暂时不用
        private async void BtnSaveFile_Click1(object sender, RoutedEventArgs e)
        {
            FileSavePicker saveFile = new FileSavePicker();
            saveFile.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // 显示在下拉列表的文件类型
            saveFile.FileTypeChoices.Add("图片", new List<string>() { ".png" });

            // 默认的文件名
            saveFile.SuggestedFileName = "SaveFile";

            StorageFile file = await saveFile.PickSaveFileAsync();
            if (file != null)
            {
                // 在用户完成更改并调用CompleteUpdatesAsync之前，阻止对文件的更新
                CachedFileManager.DeferUpdates(file);
                string fileContent = "@echo off \n dir/s \n  pause";
                await FileIO.WriteTextAsync(file, fileContent);

                // 当完成更改时，其他应用程序才可以对该文件进行更改。
                FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
                if (updateStatus == FileUpdateStatus.Complete)
                {
                    //  PictureStatus.Text = file.Name + " 已经保存好了。";
                }
                else
                {
                    //   PictureStatus.Text = file.Name + " 保存失败了。";
                }
            }
            else
            {
                //  PictureStatus.Text = "保存操作被取消。";
            }
        }
        private DataTable connectMySQL(string sqlStr)
        {
            MySqlConnectionStringBuilder s = new MySqlConnectionStringBuilder();
            s.Server = "localhost";
            s.UserID = "root";
            s.Database = "album";
            s.Password = "mysql2018";
            DataTable dt = new DataTable("album");
            // var connectStr = "server=localhost;User Id=root;password=mysql2018;Database=sakila";
            try
            {
                using (MySqlConnection mc = new MySqlConnection(s.ToString()))
                {
                    mc.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd = mc.CreateCommand();
                    if (!String.IsNullOrEmpty(sqlStr))
                        cmd.CommandText = sqlStr; //"SELECT * FROM actor";
                    cmd.CommandText = "select * from album";
                    MySqlDataAdapter Da = new MySqlDataAdapter(cmd);
                    Da.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ModelAlbum temp = new ModelAlbum();
                        temp.photoID = int.Parse(dt.Rows[i][0].ToString());
                        temp.photoCaption = dt.Rows[i][1].ToString();
                        temp.photoURL = new Uri(dt.Rows[i][2].ToString());
                        temp.photoDescription = dt.Rows[i][3].ToString();
                        //temp.labels = dt.Rows[i][4].ToString();
                        m_items.Add(temp);
                    }
                    listview.ItemsSource = m_items;
                    mc.Close();

                }
            }
            catch (System.InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return dt;

        }
        #endregion

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            TextBox temp = new TextBox();
            temp.IsEnabled = true;
            labelsPanel.Children.Add(temp);


        }
    }

}
