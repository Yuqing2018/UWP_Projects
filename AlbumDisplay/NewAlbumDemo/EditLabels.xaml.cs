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
    public sealed partial class EditLabels : ContentDialog
    {
        public EditLabels(List<LabelItem> list)
        {
            this.InitializeComponent();
            Labels.ItemsSource = list;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            LabelItem item = new LabelItem("");
            var list = Labels.Items.ToList();
            list.Add(item);
            Labels.ItemsSource = list;
        }

        public List<LabelItem> AppBarButton_Click_Edit(object sender, RoutedEventArgs e)
        {
            var list = new List<LabelItem>();
            Labels.Items.ToList().ForEach(item =>
            {
                if (item is LabelItem)
                {
                    var temp = item as LabelItem;
                    if (!String.IsNullOrEmpty(temp.value))
                        list.Add(temp);
                }
            });
            //(Labels.ItemsSource as List<LabelItem>).ToList().ForEach(item=> {
            //    if (!String.IsNullOrEmpty(item.value))
            //        list.Add(item);
            //});
            return list;
           // return list.Where(x => !String.IsNullOrEmpty(x.value)).ToList();
            //labelStr.AddRange(
            //    newLabelsPanel.Children
            //    .Where(x => (x is TextBox) && !String.IsNullOrEmpty((x as TextBox).Text))
            //    .Select(x => new LabelItem((x as TextBox).Text)).ToList());
            //newLabelsPanel.Children.ToList().ForEach(item => {
            //    if (item is TextBox)
            //    {
            //        var txt = (item as TextBox).Text;
            //        if(String.IsNullOrEmpty(txt))
            //            labelStr.Add(new LabelItem(txt));
            //    }
            //});
        }
    }
}
