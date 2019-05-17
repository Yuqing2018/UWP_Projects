using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
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
    public sealed partial class SelectPage : Page
    {
        //ObservableCollection<Scenario> ScenarioList = new ObservableCollection<Scenario>();
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="MySql Database", ClassType=typeof(MainPage),iconName="MoveToFolder"},
            new Scenario() { Title="Local File ", ClassType=typeof(MainPageLocal),iconName="OpenLocal"},
            new Scenario() { Title="Web File ", ClassType=typeof(MainPageWeb),iconName="OpenLocal"}
        };
        public string iconName { get; set; }
        public SelectPage()
        {
            this.InitializeComponent();
            listView.ItemsSource = scenarios;
            if (listView.Items.Count > 0)
                listView.SelectedIndex = 0;
        }
        

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            ListBox listview = (sender as ListBox);
            Scenario s = (listview.SelectedItem as Scenario);
            if(s !=null)
            {
               // NotifyUser(s.Title, NotifyType.StatusMessage); 
                ScenarioFrame.Navigate(s.ClassType);
                //if (Window.Current.Bounds.Width < 640)
                //{
                //    splitter.IsPaneOpen = false;
                //}
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is ToggleButton)
            {
               // ScenarioFrame.UpdateLayout();
                splitter.IsPaneOpen = !splitter.IsPaneOpen;
                
            }
        }

        async void PrivacyLink_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }
        /// <summary>
        /// Display a message to the user.
        /// This method may be called from any thread.
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        public void NotifyUser(string strMessage, NotifyType type)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }
        }
        private void UpdateStatus(string strMessage,NotifyType type)
        {
            switch(type)
            {
                case NotifyType.ErrorMessage:
                    statusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
                case NotifyType.StatusMessage:
                    statusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
            }
            statusBlock.Text = strMessage;
           // statusBorder.Visibility = (statusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (statusBlock.Text != String.Empty)
            {
                statusBorder.Visibility = Visibility.Visible;
                statusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                statusBorder.Visibility = Visibility.Collapsed;
                statusPanel.Visibility = Visibility.Collapsed;
            }
            var peer = FrameworkElementAutomationPeer.FromElement(statusBlock);
            if (peer != null)
            {
                peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
            }
        }
    }
}
