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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NewAlbumDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class flyoutPage : Page
    {
        public flyoutPage()
        {
            this.InitializeComponent();
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.ShowAttachedFlyout(sender as TextBlock);
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as HyperlinkButton).Name == "GotoMainPage")
            {
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void AcceptItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            progressBar.Value++;
        }
        private void Flag_ItemInvoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            progressBar.Value++;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
