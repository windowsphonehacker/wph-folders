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

namespace wphFolders
{
    public partial class addFolder : PhoneApplicationPage
    {
        public addFolder()
        {
            InitializeComponent();
        }


        private void addBtn_Click(object sender, EventArgs e)
        {
            String page = "editFolder";
            if (clsData.isRoot)
                page = "editFolderRoot";

            NavigationService.Navigate(new Uri("/" + page + ".xaml?folder=" + textBox1.Text, UriKind.Relative));
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}