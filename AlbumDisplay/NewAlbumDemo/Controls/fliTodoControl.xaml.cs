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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NewAlbumDemo.Controls
{
    public sealed partial class fliTodoControl : UserControl
    {
        public fliTodoControl()
        {
            this.InitializeComponent();
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            //var parentUI = this.Parent as UIElement;
            //.GetTemplateChild();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this); i++)
            {
                var child = VisualTreeHelper.GetChild(this, i);
                //if (child is FlipViewItem)
                //    (child as FlipViewItem).IsEnabled = false;
                //Grid1.Children.Remove((Ellipse)child);
            }
        }
        private void photo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var myPopup = new Popup
            {
                Child = new Image
                {
                    Source = ((Image)sender).Source,
                    Stretch = Stretch.UniformToFill,
                    Height = Window.Current.Bounds.Height,
                    Width = Window.Current.Bounds.Width,
                }
            };
            myPopup.IsOpen = true;
        }
    }
}
