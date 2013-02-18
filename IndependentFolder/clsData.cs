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

using System.IO.IsolatedStorage;
namespace wphFolders
{
    public class menu
    {
        public string name;
        public Collection<appentry> apps;
    }
    public class appentry
    {
        public string Title
        {
            get { return name; }
        }
        public string name;
        public string path;
        public string guid;
        public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register(
    "Title", typeof(string),
    typeof(appentry), null
    );

    }
}
