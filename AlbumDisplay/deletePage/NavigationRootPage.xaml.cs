using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NewAlbumDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationRootPage : Page
    {
        public static NavigationRootPage Current;
        public static Frame RootFrame = null;
        private PageHeader _header;
        MySQLHelper mysql = new MySQLHelper();
        ObservableCollection<ModelAlbum> m_items = new ObservableCollection<ModelAlbum>();
        public NavigationView NavigationView
        {
            get { return NavigationViewPanel; }
        }

        public NavigationRootPage()
        {
            this.InitializeComponent();
            Current = this;
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
            }
            NavigationViewPanel.MenuItemsSource = m_items;
            RootFrame = rootFrame;
        }
       
        private void OnNavigationViewItemInvoked(object sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                if (NavigationViewPanel.SelectedItem != NavigationViewPanel.SettingsItem)
                {
                    rootFrame.Navigate(typeof(MainPage));
                    // rootFrame.Navigate(typeof(SettingsPage));
                }
            }
            else
            {
                var invokedItem = NavigationView.MenuItems.Cast<NavigationViewItem>().Single(i => i.Content == args.InvokedItem);
                
                   // var itemId = ((ControlInfoDataGroup)invokedItem.DataContext).UniqueId;
                    rootFrame.Navigate(typeof(MainPage), invokedItem);
            }
        }
        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            NavigationViewPanel.AlwaysShowHeader = false;
        }
    }
}
