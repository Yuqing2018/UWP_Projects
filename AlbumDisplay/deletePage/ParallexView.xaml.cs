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
    public sealed partial class ParallexView : Page
    {
        ObservableCollection<String> Items;
        public ParallexView()
        {
              this.InitializeComponent();
            

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Items = new ObservableCollection<string>();
            for(int i = 0; i < 20; i++)
            {
                Items.Add(String.Format("{0:-6}", i).Replace(' ','-'));
            }
            base.OnNavigatedTo(e);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as HyperlinkButton).Name == "GotoMainPage")
            {
                Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
