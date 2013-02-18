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
using wphFolders;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
namespace IndependentFolder
{
    public partial class MainPage : PhoneApplicationPage
    {

        Imangodll libwph;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }
        menu curMenu;
        System.Windows.Threading.DispatcherTimer tim;
        double op = 0;
        int top = 20;

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            uint retval = Microsoft.Phone.InteropServices.ComBridge.RegisterComDll("libwph.dll", new Guid("56624E8C-CF91-41DF-9C31-E25A98FAF464"));
            libwph = (Imangodll)new Cmangodll(); 
            string str = "";
            
                Rectangle rect = new Rectangle();
                rect.Width = 62;
                rect.Height = 62;
                rect.Fill = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);

                var stream = System.Windows.Application.GetResourceStream(new Uri("menu.folder", UriKind.Relative));
               
                    curMenu = new menu();
                    try
                    {
                        var serial = new XmlSerializer(typeof(menu));
                        curMenu = (menu)serial.Deserialize(stream.Stream);

                    }
                    catch
                    {
                    }
               


                int y = 0;

                foreach (appentry app in curMenu.apps)
                {
                    StackPanel panel = new StackPanel();



                    panel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    System.Windows.Media.Imaging.WriteableBitmap wb = new System.Windows.Media.Imaging.WriteableBitmap(62, 62);

                    wb.Render(rect, null);

                    Image img = new Image();

                    img.Margin = new Thickness(1, 1, 10, 1);
                    img.Width = 62;
                    img.Height = 62;
                    /*System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                    using (var file = store.OpenFile(app.guid + ".jpg", FileMode.Open, FileAccess.Read))
                    {
                        bmp.SetSource(file);
                        //img.Source = Microsoft.Phone.PictureDecoder.DecodeJpeg(file);
                    }*/
                    System.Diagnostics.Debug.WriteLine(app.guid);
                    System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage(new Uri("/apps/" + app.guid + ".jpg", UriKind.Relative));
                    bmp.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.None;
                    img.Source = bmp;
                    wb.Render(img, null);
                    wb.Invalidate();
                    img.Source = wb;
                    img.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    panel.Children.Add(img);
                    TextBlock text = new TextBlock();
                    text.Text = app.name;
                    text.FontSize = 31;
                    text.FontWeight = FontWeights.Light;
                    text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    panel.Children.Add(text);
                    
                    panel.Name = "panel_" + y;
                    panel.Margin = new Thickness(1, 1, 1, 10);
                    
                    ListBoxItem foo = new ListBoxItem();
                    foo.Content = panel;
                    foo.Tap += new EventHandler<GestureEventArgs>(panel_Tap);
                    foo.Name = "danel_" + y;


                    stackPanel1.Children.Add(foo);
                    y += 1;
                    wb = null;


                }

                PageTitle.Text = curMenu.name;
                PageTitle.Opacity = op;
                scrollViewer1.Opacity = op;
                scrollViewer1.Margin = new Thickness(46, top, 0, 0);




            tim = new System.Windows.Threading.DispatcherTimer();
            tim.Interval = new TimeSpan(0, 0, 0, 0, 10);
            tim.Tick += new EventHandler(tim_Tick);
            tim.Start();
        }

        void tim_Tick(object sender, EventArgs e)
        {
            op += 0.1;
            PageTitle.Opacity = op;
            scrollViewer1.Opacity = op;
            
            top -= 1;
            scrollViewer1.Margin = new Thickness(46, top, 0, 0);
            if (top < 1)
            {
                tim.Stop();
            }
        }

        void panel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                string name = ((Control)sender).Name;
                int id = Convert.ToInt16(name.Substring("panel_".Length));
                string path = curMenu.apps[id].path;

                var re = libwph.StringCall("aygshell", "SHLaunchSessionByUri", path);
            }
            catch
            {
            }
        }

    }
}