using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace wphFolders
{
    public partial class editFolder : PhoneApplicationPage
    {
        bool isInit = false;
        menu curMenu;
        string folderName;

        public editFolder()
        {
            InitializeComponent();
            
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isInit)
            {
                NavigationContext.QueryString.TryGetValue("folder", out folderName);
                if (folderName != "")
                {
                    PageTitle.Text = "edit: " + folderName;
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var stream = new IsolatedStorageFileStream(folderName + ".folder", FileMode.OpenOrCreate, store))
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                curMenu = new menu();
                                try
                                {
                                    var serial = new XmlSerializer(typeof(menu));
                                    curMenu = (menu)serial.Deserialize(stream);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    if (curMenu.apps == null)
                    {
                        curMenu.apps = new System.Collections.ObjectModel.Collection<appentry>();
                    }

                    string appName = "";
                    NavigationContext.QueryString.TryGetValue("add", out appName);
                    if (appName != "" && appName != null)
                    {
                        string appGuid = "";
                        NavigationContext.QueryString.TryGetValue("guid", out appGuid);
                        appentry app = new appentry();
                        app.name = appName;
                        app.guid = appGuid;
                        string appPath = "";
                        NavigationContext.QueryString.TryGetValue("path", out appPath);
                        if (appPath != "" && appPath != null)
                        {
                            app.path = "app://" + appPath;
                        }
                        curMenu.apps.Add(app);
                    }
                    save();
                    regenList();
                }
                else
                {
                    MessageBox.Show("I didn't get any arguments...", "Well this is awkward", MessageBoxButton.OK);
                }
                isInit = true;
            }

            
        }

        void regenList()
        {
            /*
            listBox1.Items.Clear();
            foreach (appentry app in curMenu.apps)
            {
                listBox1.Items.Add(app.name);
            }
             */
            stackPanel1.Children.Clear();
            Rectangle rect = new Rectangle();
            rect.Width = 62;
            rect.Height = 62;
            rect.Fill = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                int y = 0;
                foreach (appentry app in curMenu.apps)
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
                        using (var file = store.OpenFile(app.guid + ".jpg", FileMode.Open, FileAccess.Read))
                        {
                            bmp.SetSource(file);
                        }
                        System.Diagnostics.Debug.WriteLine("apps/" + app.guid + ".jpg");
                        //bmp.SetSource(Application.GetResourceStream(new Uri("apps/" + app.guid + ".jpg", UriKind.Relative)).Stream);
                        //System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage(new Uri("/" + app.image, UriKind.Relative));
                        img.Source = bmp;
                        wb.Render(img, null);
                        wb.Invalidate();
                        img.Source = wb;
                        img.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                        TextBlock text = new TextBlock();
                        text.Text = app.name;
                        text.FontSize = 30;
                        text.FontWeight = FontWeights.Light;
                        text.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                        
                        //selectedApps.Add(cb);

                        //panel.Name = app.guid;
                        panel.Name = "panel_" + y;
                        panel.Tap += new EventHandler<GestureEventArgs>(panel_Tap);
                        panel.Margin = new Thickness(0, 1, 1, 10);

                        //panel.Children.Add(cb);
                        panel.Children.Add(img);
                        panel.Children.Add(text);

                        stackPanel1.Children.Add(panel);
                        y += 1;
                        //wb = null;
                    }
                    catch (Exception ez)
                    {
                        //MessageBox.Show(ez.Message + " " + ez.StackTrace);
                    }
                }

            }
        }

        void panel_Tap(object sender, GestureEventArgs e)
        {
            Panel src = (Panel)sender;
            int i = Int16.Parse(src.Name.Remove(0,6));
            if (MessageBox.Show("Remove " + curMenu.apps[i].name + "?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                curMenu.apps.RemoveAt(i);
                regenList();
            }
            
        }

        void save()
        {
            curMenu.name = folderName;

            //sort
            curMenu.apps = new Collection<appentry>(curMenu.apps.OrderBy(app => app.name).ToList());


            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(folderName + ".folder"))
                    store.DeleteFile(folderName + ".folder");
                using (var stream = new IsolatedStorageFileStream(folderName + ".folder", FileMode.OpenOrCreate, store))
                {
                    var serial = new XmlSerializer(typeof(menu));
                    serial.Serialize(stream, curMenu);
                    stream.Close();
                }
            }
        }



        private void savebtn_Click(object sender, EventArgs e)
        {
            save();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void pinbtn_Click(object sender, EventArgs e)
        {
            save();
            NavigationService.Navigate(new Uri("/MainPage.xaml?pin=" + curMenu.name, UriKind.Relative));
        }

        private void cancelbtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml?pin=" + curMenu.name, UriKind.Relative));
        }


        private void addAppClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/marketplaceSearch.xaml?from=" + folderName, UriKind.Relative));
        }

        private void addSystemAppClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/addSystemApp.xaml?from=" + folderName, UriKind.Relative));
        }

    }
}