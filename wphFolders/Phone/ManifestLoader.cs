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
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.IO;

namespace wphFolders
{
    public class ManifestLoader
    {
        public static appentry getAppFromManifest(string path)
        {
            appentry toreturn = new appentry();
            toreturn.name = "";
            System.Diagnostics.Debug.WriteLine(path);
            var bdata = WP7RootToolsSDK.FileSystem.ReadFile(path + "WMAppManifest.xml");
            string data = System.Text.Encoding.UTF8.GetString(bdata, 0, bdata.Length);
            //MessageBox.Show(data);
            data = data.Substring(data.IndexOf("<"));

            try
            {
                var appManifestXml = XDocument.Parse(data);
                using (var rdr = appManifestXml.CreateReader(ReaderOptions.None))
                {
                    rdr.ReadToDescendant("App");
                    if (!rdr.IsStartElement())
                    {
                        MessageBox.Show("App tag not found in WMAppManifest.xml");
                        throw new System.FormatException("App tag not found in WMAppManifest.xml");
                    }
                    rdr.MoveToFirstAttribute();
                    while (rdr.MoveToNextAttribute())
                    {
                        if (rdr.Name == "ProductID")
                            toreturn.guid = rdr.Value;
                    }
                    rdr.MoveToContent();
                    rdr.ReadToDescendant("IconPath");
                    string icon = rdr.ReadElementContentAsString();

                    toreturn.name = WP7RootToolsSDK.Applications.GetApplicationName(new Guid(toreturn.guid));
                    

                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!store.FileExists(toreturn.guid + ".jpg"))
                        {
                            using (var stream = new IsolatedStorageFileStream(toreturn.guid + ".jpg", FileMode.OpenOrCreate, store))
                            {

                                byte[] buf = WP7RootToolsSDK.FileSystem.ReadFile(path + icon);
                                stream.Write(buf, 0, buf.Length);
                                //CSharp___DllImport.Phone.IO.File7.WriteAllBytes("/Applications/Install/a2e30887-7682-43cf-9a71-c20104b190fa/Install/apps/" + toreturn.guid + ".jpg", buf);
                            }
                        }
                    }
                    rdr.ReadToNextSibling("Tasks");
                    //rdr.MoveToContent();
                    rdr.ReadToDescendant("DefaultTask");
                    //rdr.ReadToDescendant("DefaultTask");

                    rdr.MoveToFirstAttribute();
                    //while (rdr.MoveToNextAttribute())
                    //  if (rdr.Name == "Name")
                    toreturn.path = "app://" + toreturn.guid.Replace("{", "").Replace("}", "") + "/" + rdr.Value;


                    //MessageBox.Show(toreturn.path);
                    //MessageBox.Show(rdr.ReadToDescendant("IconPath").ToString());

                }

            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);

            }

            return toreturn;
        }
    }

}
