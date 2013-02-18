using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using CSharp___DllImport;
using System.IO.IsolatedStorage;
namespace wphFolders
{
    public class clsData
    {
        public static bool isRoot = false;

        public static string[] defaultApps = 
        {
            "5B04B775-356B-4AA0-AAF8-6491FFEA5601/Default", //Settings
            "5B04B775-356B-4AA0-AAF8-6491FFEA5603/Default", //Calc
            "5B04B775-356B-4AA0-AAF8-6491FFEA5610/Default", //Messaging
            "5B04B775-356B-4AA0-AAF8-6491FFEA5612/Default", //Calendar
            "5B04B775-356B-4AA0-AAF8-6491FFEA5615/Default", //People
            "5B04B775-356B-4AA0-AAF8-6491FFEA561E/Default", //Office hub
            "5B04B775-356B-4AA0-AAF8-6491FFEA5631/Default", //Camera
            "5B04B775-356B-4AA0-AAF8-6491FFEA5632/Default", //Pictures
            "5B04B775-356B-4AA0-AAF8-6491FFEA5630/MarketplaceHub", //Marketplace
            "5B04B775-356B-4AA0-AAF8-6491FFEA5634/Hub", //Games
            "5B04B775-356B-4AA0-AAF8-6491FFEA5660/_default", //IE
            "5B04B775-356B-4AA0-AAF8-6491FFEA5661/Maps", //Maps
            "5B04B775-356B-4AA0-AAF8-6491FFEA5630/MusicAndVideoHub", //Zune
            "5B04B775-356B-4AA0-AAF8-6491FFEA560A/Default", //Alarms
            "5B04B775-356B-4AA0-AAF8-6491FFEA5621/_default", //Flight mode
            "5B04B775-356B-4AA0-AAF8-6491FFEA561F/_default", //Cellular
            "5B04B775-356B-4AA0-AAF8-6491FFEA5623/Default", //WiFi Settings
            "5B04B775-356B-4AA0-AAF8-6491FFEA5620/_default", //Bluetooth
            "5B04B775-356B-4AA0-AAF8-6491FFEA560D/_default" //Sounds
        };
        public static string[] defaultImgs = 
        {
            "apps/def_settings.png",
            "apps/def_calculator.png",
            "apps/def_sms.png",
            "apps/def_calendar.png",
            "apps/def_people.png",
            "apps/def_office.png",
            "apps/def_camera.png",
            "apps/def_camera.png",
            "apps/def_marketplace.png",
            "apps/def_games.png",
            "apps/def_ie.png",
            "apps/def_maps.png",
            "apps/def_zune.png",
            "apps/def_alarms.png",
            "apps/def_settings.png",
            "apps/def_settings.png",
            "apps/def_wifi.png",
            "apps/def_bluetooth.png",
            "apps/def_settings.png"
        };
        public static string[] appTitles = 
        {
            "Settings",
            "Calculator",
            "Messaging",
            "Calendar",
            "People",
            "Office hub",
            "Camera",
            "Pictures",
            "Marketplace",
            "Games",
            "IE",
            "Maps",
            "Zune",
            "Alarms",
            "Flight mode",
            "Cellular",
            "WiFi Settings",
            "Bluetooth",
            "Sounds"
        };

        public static Collection<appentry> installedApps = new Collection<appentry>();
        public static void initInstalledApps()
        {
            if (installedApps.Count < 1)
            {
                //init applications list
                isRoot = WP7RootToolsSDK.Environment.HasRootAccess();
                
                if (isRoot)
                {
                    foreach (var x in WP7RootToolsSDK.Applications.GetApplicationList())
                    {
                        clsData.installedApps.Add(ManifestLoader.getAppFromManifest(@"\Applications\Install\" + x.ProductID.ToString() + @"\Install\"));
                    }
                }
            }

            for (int i = 0; i < appTitles.Length; i++)
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = new IsolatedStorageFileStream(appTitles[i] + ".jpg", FileMode.OpenOrCreate, store))
                    {
                        System.Windows.Resources.StreamResourceInfo str = Application.GetResourceStream(new Uri(defaultImgs[i], UriKind.Relative));
                        str.Stream.CopyTo(stream);
                        stream.Close();
                    }
                }
                appentry app = new appentry();
                app.guid = appTitles[i];
                app.name = appTitles[i];
                app.path = "app://" + defaultApps[i];

                clsData.installedApps.Add(app);
            }
            clsData.installedApps = new Collection<appentry>(clsData.installedApps.OrderBy(arr => arr.name).ToList());
        }
        public static Collection<menu> getMenus()
        {
            Collection<menu> arr = new Collection<menu>();
            System.Windows.Resources.StreamResourceInfo stream = Application.GetResourceStream(new Uri("menus.txt", UriKind.Relative));
            StreamReader reader = new StreamReader(stream.Stream);
            XDocument xml = XDocument.Load(reader);
            foreach (XElement ee in xml.Descendants("menu"))
            {
                menu item = new menu();
                item.name = (ee.Descendants(XName.Get("name")).First().Value);
                item.apps = new Collection<appentry>();
                foreach (XElement application in ee.Descendants("application"))
                {
                    appentry app = new appentry();
                    app.name = application.Attribute(XName.Get("name")).Value;
                    app.guid = application.Attribute(XName.Get("guid")).Value;
                    app.path = application.Attribute(XName.Get("path")).Value;
                    item.apps.Add(app);
                }
                arr.Add(item);
            }
            return arr;
        }
    }
}
