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
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
namespace wphFolders
{
    public partial class editFolderRoot : PhoneApplicationPage
    {
        public editFolderRoot()
        {
            InitializeComponent();
            foreach (appentry app in clsData.installedApps)
            {
                System.Diagnostics.Debug.WriteLine(app.name);
            }
        }

        menu curMenu;
        string folderName;
        Collection<CheckBox> selectedApps = new Collection<CheckBox>();
        
        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
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
            }
            else
            {
                MessageBox.Show("I didn't get any arguments...", "Well this is awkward", MessageBoxButton.OK);
            }
            
            Rectangle rect = new Rectangle();
            rect.Width = 62;
            rect.Height = 62;
            rect.Fill = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                int y = 0;

                foreach (appentry app in clsData.installedApps)
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
                        
                        CheckBox cb = new CheckBox();
                        cb.Name = "app_" + app.guid;
                        
                        foreach (appentry folderapp in curMenu.apps)
                        {
                            if (folderapp.guid == app.guid)
                                cb.IsChecked = true;
                        }

                        cb.Checked += new RoutedEventHandler(cb_Checked);
                        cb.Unchecked += new RoutedEventHandler(cb_Unchecked);

                        selectedApps.Add(cb);

                        //panel.Name = app.guid;
                        panel.Name = "panel_" + y;
                        //panel.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(panel_Tap);
                        panel.Margin = new Thickness(0, 1, 1, 10);

                        panel.Children.Add(cb);
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

        void cb_Unchecked(object sender, RoutedEventArgs e)
        {
            int i = 0;
            CheckBox cb = (CheckBox)sender;
            foreach (appentry app in curMenu.apps)
            {
                if ("app_" + app.guid == cb.Name)
                {
                    //MessageBox.Show("Removing " + app.guid);
                    curMenu.apps.RemoveAt(i);
                    break;
                }
                i++;
            }
        }

        void cb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            foreach (appentry app in clsData.installedApps)
            {
                if ("app_" + app.guid == cb.Name)
                {
                    curMenu.apps.Add(app);
                }
            }
        }

        void save()
        {
            curMenu.name = folderName;

            
            //clean up missing apps incase of uninstall
            for (int i = curMenu.apps.Count - 1; i >= 0; i--)
            {
                bool found = false;
                foreach (appentry app in clsData.installedApps)
                {
                    if (app.guid == curMenu.apps[i].guid)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) 
                    curMenu.apps.RemoveAt(i);
            }

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

        private void deploybtn_click(object sender, EventArgs e)
        {
            save();   
            string filedata = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
"<Deployment xmlns=\"http://schemas.microsoft.com/windowsphone/2009/deployment\" AppPlatformVersion=\"7.1\">" +
"  <App xmlns=\"\" ProductID=\"{" + Guid.NewGuid().ToString() + "}\" Title=\"" + curMenu.name + "\" RuntimeType=\"Silverlight\" Version=\"1.0.0.0\" Genre=\"apps.normal\"  Author=\"Jaxbot\" Description=\"Folder\" Publisher=\"WindowsPhoneHacker\">" +
"    <IconPath IsRelative=\"true\" IsResource=\"false\">ApplicationIcon.png</IconPath>" +
"    <Capabilities>" +
"      <Capability Name=\"ID_CAP_GAMERSERVICES\"/>" +
"      <Capability Name=\"ID_CAP_IDENTITY_DEVICE\"/>" +
"      <Capability Name=\"ID_CAP_IDENTITY_USER\"/>" +
"      <Capability Name=\"ID_CAP_LOCATION\"/>" +
"      <Capability Name=\"ID_CAP_MEDIALIB\"/>" +
"      <Capability Name=\"ID_CAP_MICROPHONE\"/>" +
"      <Capability Name=\"ID_CAP_NETWORKING\"/>" +
"      <Capability Name=\"ID_CAP_PHONEDIALER\"/>" +
"      <Capability Name=\"ID_CAP_PUSH_NOTIFICATION\"/>" +
"      <Capability Name=\"ID_CAP_SENSORS\"/>" +
"      <Capability Name=\"ID_CAP_WEBBROWSERCOMPONENT\"/>" +
"      <Capability Name=\"ID_CAP_ISV_CAMERA\"/>" +
"      <Capability Name=\"ID_CAP_CONTACTS\"/>" +
"      <Capability Name=\"ID_CAP_APPOINTMENTS\"/>" +
"    </Capabilities>" +
"    <Tasks>" +
"      <DefaultTask  Name =\"_default\" NavigationPage=\"MainPage.xaml\"/>" +
"    </Tasks>" +
"    <Tokens>" +
"      <PrimaryToken TokenID=\"IndependentFolderToken\" TaskName=\"_default\">" +
"        <TemplateType5>" +
"          <BackgroundImageURI IsRelative=\"true\" IsResource=\"false\">Background.png</BackgroundImageURI>" +
"          <Count>0</Count>" +
"          <Title>Folder</Title>" +
"        </TemplateType5>" +
"      </PrimaryToken>" +
"    </Tokens>" +
"  </App>" +
"</Deployment>";

            MemoryStream original = new MemoryStream();
            var res = Application.GetResourceStream(new Uri("IndependentFolder.xap", UriKind.Relative)).Stream;
            res.CopyTo(original);
            ICSharpCode.SharpZipLib.Zip.ZipFile file = new ICSharpCode.SharpZipLib.Zip.ZipFile(original);
            file.BeginUpdate();

            MemoryStream filestr = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(filedata));

            CustomStaticDataSource sds = new CustomStaticDataSource();
            sds.SetStream(filestr);

            file.Add(sds, "WMAppManifest.xml");
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (appentry app in curMenu.apps)
                {
                    var imgfile = store.OpenFile(app.guid + ".jpg", FileMode.Open, FileAccess.Read);
                    
                        CustomStaticDataSource sdsimg = new CustomStaticDataSource();
                    
                        imgfile.CopyTo(sdsimg._stream);
                        imgfile.Close();
                        sdsimg._stream.Position = 0;
                        //sdsimg.SetStream(imgfile);

                        file.Add(sdsimg, "apps/" + app.guid + ".jpg");
                        file.CommitUpdate();
                        file.BeginUpdate();
                }

                var menufile = store.OpenFile(curMenu.name + ".folder", FileMode.Open, FileAccess.Read);
                
                    CustomStaticDataSource sdsmenu = new CustomStaticDataSource();
                    sdsmenu.SetStream(menufile);

                    file.Add(sdsmenu, "menu.folder");
                
                file.CommitUpdate();
                file.IsStreamOwner = false;
                file.Close();

                original.Position = 0;


                using (var stream = new IsolatedStorageFileStream("package." + curMenu.name + ".zip", FileMode.Create, store))
                {
                    original.CopyTo(stream);
                }
            }

            /*
            ICSharpCode.SharpZipLib.Zip.ZipOutputStream str = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(new MemoryStream());
            str.PutNextEntry(new ICSharpCode.SharpZipLib.Zip.ZipEntry("WMAppManifest.xml"));
            */

            //ICSharpCode.SharpZipLib.Zip.zipout
            //file.Add(str);
            
            XapHandler.XapDeployerInterop.Initialize();
            XapHandler.XapDeployerInterop.ReadyIsAppInstalled(@"\Applications\Data\5b594f78-a744-4f8a-85d2-f0f55f411832\Data\IsolatedStore\package." + curMenu.name + ".zip");
            XapHandler.XapDeployerInterop.FinishInstall(false);
        }

    }
    public class CustomStaticDataSource : ICSharpCode.SharpZipLib.Zip.IStaticDataSource
    {
        public Stream _stream;
        // Implement method from IStaticDataSource
        public Stream GetSource()
        {
            return _stream;
        }

        // Call this to provide the memorystream
        public void SetStream(Stream inputStream)
        {
            _stream = inputStream;
            _stream.Position = 0;
        }
        public CustomStaticDataSource()
        {
            _stream = new MemoryStream();
        }
    }
}