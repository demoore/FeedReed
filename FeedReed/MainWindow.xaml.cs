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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel.Syndication;
using System.Net.Cache;
using System.Drawing;
using System.Net;


namespace FeedReed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FeedHandler feed;
        private FeedList feedList;
        private SyndicationItem selectedItem;
        private Queue<string> downloadUrls;
        private String progressLabelText;
        public MainWindow()
        {
            feedList = new FeedList();
            try
            {
                feedList.openXML();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            InitializeComponent();
            try
            {
                feedListBox.ItemsSource = feedList.getFeeds();
            }
            catch (Exception e)
            {
                Console.WriteLine("ListBox source failed");
            }
            feedListBox.SelectedIndex = 0;
            this.DataContext = this;
            downloadUrls = new Queue<string>();



        }


        public String getImage()
        {
            return feed.getImageLocation().ToString();
        }

        private void onAddClick(object sender, RoutedEventArgs e)
        {
            feedListBox.ItemsSource = feedList.getFeeds();
            AddFeedDialog addFeedDialog = new AddFeedDialog();

            if (addFeedDialog.ShowDialog() == false)
            {
                String feedName = addFeedDialog.ResponseText;
                try
                {
                    feedList.add(new FeedHandler(feedName));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Feed already in list");
                }
                feedList.saveXML();
            }

        }


        private void onSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            //TODO make a more elegant way of handling this shit
            try
            {
                feed = feedList.getFeedAtIndex(feedListBox.SelectedIndex);
                Console.WriteLine("Selected feed at: " + feedListBox.SelectedIndex);
                Console.WriteLine("Which is: " + feed.Title);
                titleLabel.Content = feed.Title;


                feedImage.Source = feed.getImageSource(new Bitmap(feed.getImageForDisplay()));
                itemList.ItemsSource = feed.getItems();
                itemList.SelectedIndex = 0;
                descBlock.Text = feed.getDescription();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Well, something happend");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void onUpdateClick(object sender, RoutedEventArgs e)
        {
            feed.updateFeed();
            feed = feedList.getFeedAtIndex(feedListBox.SelectedIndex);
            itemList.ItemsSource = feed.getItems();
        }

        private void onDownloadClick(object sender, RoutedEventArgs e)
        {
            Button downloadButton = (Button)sender;
            TextBlock buttonText = (TextBlock)downloadButton.Content;
            selectedItem = (SyndicationItem)((ListBoxItem)itemList.ContainerFromElement((Button)sender)).Content;
            Uri downloadFileLink = new Uri(selectedItem.Links[1].Uri.AbsoluteUri);
            Console.WriteLine("Downloading: " + downloadFileLink.ToString());
            Queue<string> urls = new Queue<string>();
            urls.Enqueue(downloadFileLink.ToString());
            downloadFile(urls);


        }

        private void onRemoveClick(object sender, RoutedEventArgs e)
        {
            if (feedListBox.SelectedItem != null)
            {
                feed = feedList.getFeedAtIndex(0);
                feedList.remove((FeedHandler)feedListBox.SelectedItem);
                feedList.saveXML();
            }
        }

        private void downloadFile(IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                downloadUrls.Enqueue(url);
            }

            DownloadFiles();
        }

        private void DownloadFiles()
        {
            if (downloadUrls.Any())
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;

                var url = downloadUrls.Dequeue();
                string FileName = url.Substring(url.LastIndexOf("/") + 1, (url.Length - url.LastIndexOf("/") - 1));
                progressLabelText = FileName;
                client.DownloadFileAsync(new Uri(url), feed.getFileDownloadLocation() + FileName);
                progressLabel.Content = progressLabelText;
                return;
            }

            progressLabel.Content = "Done!";

        }

        private void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }
            if (e.Cancelled)
            {
                Console.WriteLine("Well, something was cancelled");
            }
            DownloadFiles();
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            
            downloadProgressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
            //progressLabel.Content = downloadProgressBar.Value + "% " + progressLabelText;
        }

        private void downloadListButton_Click(object sender, RoutedEventArgs e)
        {
            
            DownloadList downloadListDialog = new DownloadList(downloadUrls);
            downloadListDialog.Show();
        }
    }
}
