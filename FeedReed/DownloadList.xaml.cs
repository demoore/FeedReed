using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FeedReed
{
    /// <summary>
    /// Interaction logic for DownloadList.xaml
    /// </summary>
    public partial class DownloadList : Window
    {
        private Queue<string> downloadList;
        public DownloadList(Queue<string> inDownloadList)
        {
            InitializeComponent();
            this.downloadList = inDownloadList;
            DownloadItemList.ItemsSource = downloadList;
            Console.WriteLine(downloadList.ToString());
        }


    }
}
