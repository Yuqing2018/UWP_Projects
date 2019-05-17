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
    public sealed partial class MediaPage : Page
    {
        long token;
        public MediaPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            token = mediaPlayer.RegisterPropertyChangedCallback(MediaElement.IsFullWindowProperty, OnMEfullWindowChanged);
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            mediaPlayer.UnregisterPropertyChangedCallback(MediaElement.IsFullWindowProperty, token);
            base.OnNavigatedFrom(e);
        }

        private void OnMEfullWindowChanged(DependencyObject sender, DependencyProperty dp)
        {
            MediaElement me = sender as MediaElement;
            if(me !=null && dp == MediaElement.IsFullWindowProperty)
            {
                if (me.IsFullWindow == true)
                    mediaPlayerPopup.Visibility = Visibility.Collapsed;
                else
                    mediaPlayerPopup.Visibility = Visibility.Visible;
            }
        }
        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            // If the Popup is open, then close it.
            if (mediaPlayerPopup.IsOpen) { mediaPlayerPopup.IsOpen = false; }
        }

        // Handles the Click event on the Button on the page and opens the Popup. 
        private void ShowPopupClicked(object sender, RoutedEventArgs e)
        {
            // Open the Popup if it isn't open already. 
            if (!mediaPlayerPopup.IsOpen) { mediaPlayerPopup.IsOpen = true; }
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
