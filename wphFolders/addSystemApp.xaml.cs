using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.IO.IsolatedStorage;

namespace wphFolders
{
    public partial class addSystemApp : PhoneApplicationPage
    {
        public addSystemApp()
        {
            InitializeComponent();
            Rectangle rect = new Rectangle();
            rect.Width = 62;
            rect.Height = 62;
            rect.Fill = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                int y = 0;
                foreach (string app in clsData.appTitles)
                {
                    try
                    {
                        System.Windows.Media.Imaging.WriteableBitmap wb = new System.Windows.Media.Imaging.WriteableBitmap(52, 52);
                        StackPanel panel = new StackPanel();

                        panel.Orientation = System.Windows.Controls.Orientation.Horizontal;

                        wb.Render(rect, null);

                        Image img = new Image();

                        img.Margin = new Thickness(1, 1, 10, 1);
                        img.Width = 52;
                        img.Height = 52;
                        System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                        using (var file = store.OpenFile(app + ".jpg", FileMode.Open, FileAccess.Read))
                        {
                            bmp.SetSource(file);
                        }
                        
                        img.Source = bmp;
                        wb.Render(img, null);
                        wb.Invalidate();
                        img.Source = wb;
                        img.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                        TextBlock text = new TextBlock();
                        text.Text = app;
                        text.FontSize = 30;
                        text.FontWeight = FontWeights.Light;
                        text.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                        panel.Name = "panel_" + y;
                        panel.Tap += new EventHandler<GestureEventArgs>(panel_Tap);
                        panel.Margin = new Thickness(0, 1, 1, 10);

                        panel.Children.Add(img);
                        panel.Children.Add(text);

                        stackPanel1.Children.Add(panel);
                        y += 1;
                    }
                    catch (Exception ez)
                    {
                        MessageBox.Show(ez.Message + " " + ez.StackTrace);
                    }
                }
            }
        }

        void panel_Tap(object sender, GestureEventArgs e)
        {

            Panel src = (Panel)sender;
            int i = Int16.Parse(src.Name.Remove(0, 6));
            string folderName = "";

            NavigationContext.QueryString.TryGetValue("from", out folderName);
            if (folderName != "")
            {
                NavigationService.Navigate(new Uri("/editFolder.xaml?folder=" + folderName + "&add=" + clsData.appTitles[i] + "&guid=" + clsData.appTitles[i] + "&path=" + clsData.defaultApps[i], UriKind.Relative));
            }
            else
            {
                MessageBox.Show("This error should never happen.");
            }
        }
    }
}