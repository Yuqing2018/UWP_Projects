using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NewAlbumDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPageLocal : Page
    {
        public List<ModelAlbum> _ImagesItems { get; set; }
        FolderPicker folderPicker = new FolderPicker();
        StorageFolder imagefolder;
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
        public MainPageLocal()
        {
            this.InitializeComponent();
        }
        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            ProgressControl1.IsActive = true;
            logResults.Inlines.Clear();
            //_ImagesItems.Clear();
            string url = AddressBox.Text.Trim();
            if (url.StartsWith("c:\\") || url.StartsWith("C:\\"))
            {
                _ImagesItems = new List<ModelAlbum>();
                IsWebPath = false;
               // FlipPanel.UpdateLayout();
                //Scenario2Root.UpdateLayout();
                GetLoacalFiles(url);
                
            }
            else if(url.StartsWith("http://") || url.StartsWith("https://"))
            {
                IsWebPath = true;
                //controlPanel.UpdateLayout();
                //webviewPanel.UpdateLayout();
                //FlipPanel.UpdateLayout();
                if (!pageIsLoading)
                {
                    NavigateWebView(AddressBox.Text);
                }
                else
                {
                    WebViewControl.Stop();
                    pageIsLoading = false;
                }
               // this.Frame.Navigate(typeof(MainPageLocal), IsWebPath);
                 Scenario2Root.UpdateLayout();
            }
            else
            {
                var el =(this.Parent as Page);
            }
           // Scenario2Root.UpdateLayout();

        }
        private async void GetLoacalFiles(string localdirectoryPath)
        {
            try
            {
                IReadOnlyList<StorageFile> images = await imagefolder.GetFilesAsync();
                AppendLog($"get all images from directory Path: {localdirectoryPath} .");
                var trueList = images.Where(x => x.ContentType.Contains("image")).ToList();
                List<Task> tasks = new List<Task>();
                trueList.ForEach(item => tasks.Add(AddFlipViewItem(item)));
                await Task.WhenAll(tasks);
                flipView.ItemsSource = _ImagesItems;
                flipView.SelectedIndex =0;
                sumCount.Text = _ImagesItems.Count.ToString();
                ProgressControl1.IsActive = false;
                //this.Frame.Navigate(typeof(MainPageLocal), IsWebPath);
                FlipPanel.UpdateLayout();
                //controlPanel.UpdateLayout();
                //webviewPanel.UpdateLayout();
                //FlipPanel.UpdateLayout();
                //Scenario2Root.UpdateLayout();
            }
            catch(Exception ex)
            {
                AppendLog($"error Message: \"{ex.Message}\" .");
            }
            finally
            {
                AppendLog($"there are {_ImagesItems.Count} images have been added to flipView control .");
            }

        }
        public async Task AddFlipViewItem(StorageFile item)
        {
            _ImagesItems.Add(new ModelAlbum()
            {
                photoCaption = item.Name,
                photoPath = item.Path,
                photoURL = new Uri(item.Path),
                imageSource = await UrlToImageSource(item.Path),
            });
            AppendLog($"the {item.Name} has been added.");
        }
        public async Task<BitmapImage> UrlToImageSource(string url)
        {
            StorageFile imageFile = await StorageFile.GetFileFromPathAsync(url);
            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                return bitmapImage;
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
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                NavigateWebView(AddressBox.Text);
            }
        }

        private async void selectDirectory_Click(object sender, RoutedEventArgs e)
        {
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");
            imagefolder = await folderPicker.PickSingleFolderAsync();
            if(imagefolder != null)
            {
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", imagefolder);
                AddressBox.Text = imagefolder.Path;
            }
        }
    }
}
