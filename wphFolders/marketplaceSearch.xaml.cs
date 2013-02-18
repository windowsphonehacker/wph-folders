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
using System.IO;
namespace wphFolders
{
    public partial class marketplaceSearch : PhoneApplicationPage
    {
        System.Collections.ObjectModel.Collection<appentry> apps = new System.Collections.ObjectModel.Collection<appentry>();
        System.Collections.ObjectModel.Collection<String> appimages = new System.Collections.ObjectModel.Collection<string>();
        public marketplaceSearch()
        {
            InitializeComponent();

            regionSelect.Text = System.Globalization.CultureInfo.CurrentCulture.Name;
        }
        void searchQuery(string query)
        {
            string url = "http://marketplaceedgeservice.windowsphone.com/v3.2/" + regionSelect.Text + "/apps?q=" + query + "&clientType=WinMobile 7.1&store=zest&store=&orderby=downloadRank&store=Nokia";
            System.Diagnostics.Debug.WriteLine(url);
            try
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(url));
            }
            catch (Exception ee)
            {
                MessageBox.Show("That didn't work ):\r\n" + ee.Message);
            }

        }
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                appimages.Clear();
                apps.Clear();
                string data = e.Result;
                string[] s = data.Split(new string[] { "<a:entry>" }, StringSplitOptions.None);
                string[] s2;
                bool isfirst = true;
                foreach (string app in s)
                {
                    if (isfirst)
                    {
                        isfirst = false;
                    }
                    else
                    {
                        appentry appentry = new appentry();
                        s2 = app.Split(new string[] { "<a:title" }, StringSplitOptions.None);
                        s2 = s2[1].Split(">".ToCharArray());
                        s2 = s2[1].Split("<".ToCharArray());
                        listBox1.Items.Add(s2[0]);
                        appentry.name = s2[0];

                        s2 = app.Split(new string[] { "<a:id>urn:uuid:" }, StringSplitOptions.None);
                        s2 = s2[1].Split("<".ToCharArray());
                        appentry.guid = s2[0];

                        s2 = app.Split(new string[] { "<id>urn:uuid:" }, StringSplitOptions.None);
                        s2 = s2[1].Split("<".ToCharArray());
                        
                        appimages.Add(s2[0]);
                        apps.Add(appentry);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("That didn't work ):\r\n" + ee.Message);
            }
            updateListbox();
        }
        void updateListbox()
        {
            listBox1.Items.Clear();
            foreach (appentry app in apps)
            {
                listBox1.Items.Add(app.name);
            }
        }
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                WebClient clien = new WebClient();
                clien.OpenReadCompleted += new OpenReadCompletedEventHandler(clien_OpenReadCompleted);
                clien.OpenReadAsync(new Uri("http://cdn.marketplaceimages.windowsphone.com/v3.2/" + regionSelect.Text + "/image/" + appimages[listBox1.SelectedIndex] + "?width=86&height=86&resize=true&contenttype=image/png"));
                txtName.Text = apps[listBox1.SelectedIndex].name;
                txtGUID.Text = apps[listBox1.SelectedIndex].guid;
            }
        }

        void clien_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = new IsolatedStorageFileStream(apps[listBox1.SelectedIndex].guid + ".jpg", FileMode.OpenOrCreate, store))
                {
                    e.Result.CopyTo(stream);

                    System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                    bmp.SetSource(stream);
                    bmp.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.None; //force load
                    image1.Source = bmp;

                    stream.Close();
                }
            }

        }
        
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            string folderName = "";
            
            NavigationContext.QueryString.TryGetValue("from", out folderName);
            if (folderName != "")
            {
                NavigationService.Navigate(new Uri("/editFolder.xaml?folder=" + folderName + "&add=" + txtName.Text + "&guid=" + txtGUID.Text, UriKind.Relative));
            }
            else
            {
                MessageBox.Show("This error should never happen.");
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchQuery(textBox1.Text);
            }
        }
    }
}