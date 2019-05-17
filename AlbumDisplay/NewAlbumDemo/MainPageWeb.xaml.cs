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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NewAlbumDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPageWeb : Page
    {
        public MainPageWeb()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// Property to control the "Go" button text, forward/backward buttons and progress ring.
        /// </summary>
        private bool _pageIsLoading;
        bool pageIsLoading
        {
            get { return _pageIsLoading; }
            set
            {
                _pageIsLoading = value;
                GoBtn.Content = (value ? "Stop" : "Go");
                ProgressControl.IsActive = value;

                if (!value)
                {
                    NavigateBackButton.IsEnabled = WebViewControl.CanGoBack;
                    NavigateForwardButton.IsEnabled = WebViewControl.CanGoForward;
                }
            }
        }
        public bool IsWebPath { get; set; }
        static string UriToString(Uri uri)
        {
            return (uri != null) ? uri.ToString() : "";
        }
        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            string url = AddressBox.Text.Trim();
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                if (!pageIsLoading)
                {
                    NavigateWebView(AddressBox.Text);
                }
                else
                {
                    WebViewControl.Stop();
                    pageIsLoading = false;
                }
            }
        }
        private void NavigateWebView(string url)
        {
            try
            {
                Uri targetUri = new Uri(url);
                WebViewControl.Navigate(targetUri);
            }
            catch (UriFormatException ex)
            {
            }
        }
        /// <summary>
        /// Handler for the NavigateBackward button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigateBackward_Click()
        {
            if (WebViewControl.CanGoBack)
                WebViewControl.GoBack();
        }

        /// <summary>
        /// Handler for the GoForward button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigateForward_Click()
        {
            if (WebViewControl.CanGoForward) WebViewControl.GoForward();
        }
        /// <summary>
        /// Handle the event that indicates that WebView is starting a navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void WebViewControl_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            string url = UriToString(args.Uri);
            AddressBox.Text = url;
            AppendLog($"Starting navigation to \"{url}\".");
            pageIsLoading = true;
        }
        /// <summary>
        /// Handle the event that indicates that the webview content is not a wevb page
        /// for example, it may be a file download.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void WebViewControl_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            AppendLog($"Loading content for \"{UriToString(args.Uri)}\".");
        }

        private void WebViewControl_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            AppendLog($"Content for \"{UriToString(args.Uri)}\" has finished loading.");
        }

        private void WebViewControl_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            AppendLog($"Content for \"{UriToString(args.Uri)}\" Cannot be loaded into webview.");
            pageIsLoading = false;
        }

        private void WebViewControl_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            pageIsLoading = false;
            if (args.IsSuccess)
                AppendLog($"Navigation to \"{UriToString(args.Uri)}\" completed successfully.");
            else
                AppendLog($"Navigation to \"{UriToString(args.Uri)}\" failed eith error {args.WebErrorStatus}.");
        }
        void AppendLog(string logEntry)
        {
            logResults.Inlines.Add(new Run { Text = logEntry + "\n" });
            logScroller.ChangeView(0, logScroller.ScrollableHeight, null);
        }

        private void AddressBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                NavigateWebView(AddressBox.Text);
            }
        }
        
    }
}
