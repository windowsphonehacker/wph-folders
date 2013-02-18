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
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Windows.Media.Imaging;
using Microsoft.Live;
namespace wphFolders
{
    public partial class MainPage : PhoneApplicationPage
    {
        Collection<menu> menuEntries = new Collection<menu>();

        bool pinMode = true;
        bool deleteMode = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            clsData.initInstalledApps();
            regenerateList();

        }

        void wph_updateAvailableEvent(object sender, EventArgs e)
        {
            MessageBox.Show("An update is available at WindowsPhoneHacker.com!");
            if (clsData.isRoot)
            {
                Microsoft.Phone.Tasks.WebBrowserTask wb = new Microsoft.Phone.Tasks.WebBrowserTask();
                wb.Uri = new Uri("http://windowsphonehacker.com/folders");
                wb.Show();
            }
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("WPH Folders v5\n\n" + (clsData.isRoot ? "Root mode" : "Marketplace mode") + "\n\nPowered by Heathcliff74's Root Tools SDK\nWP7 Root Tools SDK is copyrighted and licensed under terms of www.wp7roottools.com\n\n\nApp Written by Jaxbot\nwindowsphonehacker.com", "About", MessageBoxButton.OK);
        }

        private void regenerateList()
        {
            menuEntries.Clear();
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {

                foreach (string file in store.GetFileNames("*.folder"))
                {
                    using (var stream = new IsolatedStorageFileStream(file, FileMode.OpenOrCreate, store))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            try
                            {
                                var serial = new XmlSerializer(typeof(menu));
                                menuEntries.Add((menu)serial.Deserialize(stream));

                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            stackPanel1.Children.Clear();

            int n = 0;
            StackPanel curPanel = new StackPanel();
            foreach (menu menu in menuEntries)
            {
                if (n == 0)
                {
                    curPanel = new StackPanel();
                    curPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    stackPanel1.Children.Add(curPanel);
                    n = 2;
                }
                Image img = new Image();

                img.Name = menu.name;

                img.Margin = new Thickness(4, 4, 4, 4);
                img.Width = 174;
                img.Height = 174;

                img.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(img_Tap);

                img.Source = renderTileImg(menu, true);
                img.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                curPanel.Children.Add(img);
                n--;
            }
        }
        void img_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string menu = ((Image)sender).Name;
            if (pinMode)
            {
                pin(menu);
            }
            else
            {
                if (deleteMode)
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        try
                        {
                            store.DeleteFile(menu + ".folder");
                            regenerateList();
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    String page = "editFolder";
                    if (clsData.isRoot)
                        page = "editFolderRoot";

                    NavigationService.Navigate(new Uri("/" + page + ".xaml?folder=" + menu, UriKind.Relative));
                }
            }

        }
        public void pin(string menu)
        {
            menu menuEntry = new menu();
            foreach (menu menua in menuEntries)
            {
                if (menua.name == menu)
                {
                    menuEntry = menua;
                    break;
                }
            }
            string i = menuEntry.name;

            renderTileImg(menuEntry);
            var tile = new StandardTileData();

            tile.BackgroundImage = new Uri("isostore:/Shared/ShellContent/tile_" + i + ".jpg", UriKind.Absolute);
            
            if (menuEntry.apps.Count > 9)
            {
                tile.BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/tile_" + i + "_back.jpg", UriKind.Absolute);
            }
            try
            {
                ShellTile.Create(new Uri("/Folder.xaml?tile=" + menuEntry.name, UriKind.Relative), tile);
            }
            catch
            {
                try
                {
                    ShellTile.ActiveTiles.FirstOrDefault(zx => zx.NavigationUri.ToString().Contains("tile=" + menuEntry.name)).Update(tile);
                }
                catch
                {
                    MessageBox.Show("Looks like that's already pinned. Try deleting it first.");
                }
            }
        }
        public System.Windows.Media.Imaging.WriteableBitmap renderTileImg(menu menuEntry, bool rendertitle = false)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string i = (string)menuEntry.name;
                int x = 0;
                int y = 0;
                int s = 0;
                int z = 0;


                if (menuEntry.apps.Count < 5)
                {
                    s = 100;
                }
                else
                {
                    s = 67;
                }


                System.Windows.Media.Imaging.WriteableBitmap wb = new System.Windows.Media.Imaging.WriteableBitmap(200,200);

                System.Windows.Media.Imaging.WriteableBitmap wb_back = new System.Windows.Media.Imaging.WriteableBitmap(200,200);

                if (rendertitle)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = 200;
                    rect.Height = 200;

                    Color clr = (Color)Application.Current.Resources["PhoneAccentColor"];
                    clr.A = 160;

                    rect.Fill = new SolidColorBrush(clr);

                    wb.Render(rect, null);
                    wb_back.Render(rect, null);
                }
                else
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = 200;
                    rect.Height = 200;

                    rect.Fill = new SolidColorBrush(Color.FromArgb(160,0,0,0));

                    wb.Render(rect, null);
                    wb_back.Render(rect, null);
                }

                foreach (appentry app in menuEntry.apps)
                {
                    Image img = new Image();
                    img.Stretch = Stretch.Uniform;
                    img.Width = s;
                    img.Height = s;
                    BitmapImage bmp = new BitmapImage();
                    using (var file = store.OpenFile(app.guid + ".jpg", FileMode.Open, FileAccess.Read))
                    {
                        bmp.SetSource(file);
                    }
                    img.Source = bmp;

                    if (z < 9)
                    {
                        wb.Render(img, new TranslateTransform { X = x, Y = y });
                    }
                    else
                    {
                        wb_back.Render(img, new TranslateTransform { X = x, Y = y });
                    }
                    x += s;
                    if (x > 200 - (s - 5))
                    {
                        x = 0;
                        y += s;
                    }
                    if (z == 8)
                    {
                        x = 0;
                        y = 0;
                    }
                    z++;
                }
                /*if (rendertitle)
                {
                    TextBlock title = new TextBlock();
                    title.FontSize = 21;
                    title.Text = i;
                    title.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                
                    wb.Render(title, new TranslateTransform { X = 10, Y = 170 });
                }*/
                wb.Invalidate();
                
                System.Diagnostics.Debug.WriteLine(i);
                if (!rendertitle)
                {
                    var filename = "/Shared/ShellContent/tile_" + i + ".jpg";
                    using (var st = new IsolatedStorageFileStream(filename, FileMode.Create, FileAccess.Write, store))
                    {
                        WriteableBitmapExtensions.WritePNG(wb, st);
                    }
                    if (menuEntry.apps.Count > 9)
                    {
                        wb_back.Invalidate();
                        filename = "/Shared/ShellContent/tile_" + i + "_back.jpg";
                        using (var st = new IsolatedStorageFileStream(filename, FileMode.Create, FileAccess.Write, store))
                        {
                            WriteableBitmapExtensions.WritePNG(wb_back, st);
                        }
                    }
                }
                return wb;
            }
        }
        public Color strColor(string str)
        {
            str = str.Replace("#", "");
            Color c = new Color();
            c.A = 255;
            c.R = 200;
            c.G = 200;
            c.B = 200;
            try
            {
                c.R = byte.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                c.G = byte.Parse(str.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                c.B = byte.Parse(str.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            catch
            {
            }

            return c;
        }


        private void editBtn_Click(object sender, EventArgs e)
        {
            //ApplicationBar.Buttons
            if (pinMode)
            {
                PageTitle.Text = "edit";
                pinMode = false;
            }
            else
            {
                pinMode = true;
                PageTitle.Text = "folders";
            }
            deleteMode = false;
        }


        private void addBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/addFolder.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string query = "";
            if (NavigationContext.QueryString.TryGetValue("pin", out query))
            {
                pin(query);
            }

            WindowsPhoneHacker.wph.updateAvailableEvent += new EventHandler(wph_updateAvailableEvent);
            WindowsPhoneHacker.wph.bacon("wphnewfolders3_" + (clsData.isRoot ? "root" : "mp"));
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            //ApplicationBar.Buttons
            if (pinMode)
            {
                PageTitle.Text = "delete";
                pinMode = false;
                deleteMode = true;
            }
            else
            {
                pinMode = true;
                PageTitle.Text = "folders";
                deleteMode = false;
            }
            
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            // Zip up the iso store

            MemoryStream original = new MemoryStream();
            ICSharpCode.SharpZipLib.Zip.ZipFile file = new ICSharpCode.SharpZipLib.Zip.ZipFile(original);

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (string fname in store.GetFileNames())
                {

                    file.BeginUpdate();

                    CustomStaticDataSource sdsf = new CustomStaticDataSource();
                    var storefile = store.OpenFile(fname, FileMode.Open, FileAccess.Read);

                    storefile.CopyTo(sdsf._stream);
                    storefile.Close();

                    sdsf._stream.Position = 0;

                    file.Add(sdsf, fname);
                    file.CommitUpdate();

                }

                file.IsStreamOwner = false;
                file.Close();
                
                original.Position = 0;

                // Connect to live API
                LiveAuthClient auth = new LiveAuthClient("00000000440E7119");

                auth.InitializeAsync();
                auth.InitializeCompleted += (e1, e2) =>
                {
                    // Show login dialog
                    auth.LoginAsync(new string[] { "wl.skydrive_update" });

                    auth.LoginCompleted += (e3, e4) =>
                    {
                        if (e4.Status == LiveConnectSessionStatus.Connected)
                        {
                            LiveConnectClient client = new LiveConnectClient(e4.Session);

                            // Upload that zip we just made
                            client.UploadAsync("me/skydrive/", "wphfolders.zip", original, OverwriteOption.Overwrite);
                            client.UploadCompleted += (ucSender, ucEvent) =>
                            {
                                MessageBox.Show("Uploaded wphfolders.zip to the root of your SkyDrive. Feel free to move it, but put it back if you need to restore!");
                            };
                        }
                        else
                        {
                            MessageBox.Show("Not connected to SkyDrive");
                        }
                    };
                };

            }
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            // Download the zip

            // Connect to live API
            LiveAuthClient auth = new LiveAuthClient("00000000440E7119");

            auth.InitializeAsync();
            auth.InitializeCompleted += (e1, e2) =>
            {
                // Show login dialog
                auth.LoginAsync(new string[] { "wl.skydrive_update", "wl.skydrive" });

                auth.LoginCompleted += (e3, e4) =>
                {
                    if (e4.Status == LiveConnectSessionStatus.Connected)
                    {
                        LiveConnectClient client = new LiveConnectClient(e4.Session);

                        // Upload that zip we just made
                        client.GetAsync("me/skydrive/files");
                        client.GetCompleted += (getSender, getEvents) =>
                        {
                            Dictionary<string, object> fileData = (Dictionary<string, object>)getEvents.Result;
                            List<object> files = (List<object>)fileData["data"];

                            foreach (object item in files)
                            {
                                Dictionary<string, object> file = (Dictionary<string, object>)item;
                                if (file["name"].ToString() == "wphfolders.zip")
                                {
                                    System.Diagnostics.Debug.WriteLine(file["name"]);
                                    client.DownloadAsync(file["id"].ToString() + "/content");
                                    client.DownloadCompleted += (downloadSender, downloadEventArgs) =>
                                    {
                                        SharpGIS.UnZipper unzipper = new SharpGIS.UnZipper(downloadEventArgs.Result);
                                        using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                                        {
                                            foreach (string name in unzipper.FileNamesInZip)
                                            {
                                                var storefile = store.OpenFile(name, FileMode.Create, FileAccess.Write);
                                                unzipper.GetFileStream(name).CopyTo(storefile);
                                                storefile.Close();
                                            }
                                        }
                                        MessageBox.Show("Restored.");
                                        regenerateList();
                                    };
                                }
                            }
                        };
                    }
                    else
                    {
                        MessageBox.Show("Not connected to SkyDrive");
                    }
                };
            };

        }



    }
}