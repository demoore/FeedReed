using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Net.Http;
using System.Net;
using System.Windows.Media;
using System.Windows.Interop;
using System.Reflection;
using System.IO;

namespace FeedReed
{

    class FeedHandler
    {
        private Rss20FeedFormatter rssFormatter;
        private XmlWriter writer;
        private XmlReader reader;
        private SyndicationFeed myFeed;
        private Image feedPic;
        public String titleName;
        public static String podcastLocation = "podcasts/";
        public String podcastDownloadLocation;
        public Uri feedLocation;


        public FeedHandler(String feedName)
        {

            try
            {
                reader = XmlReader.Create(feedName);
                feedLocation = 
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                myFeed = feed;
                rssFormatter = new Rss20FeedFormatter(feed);
                writer = XmlWriter.Create("podcasts/" + myFeed.Title.Text + ".xml");
                rssFormatter.WriteTo(writer);
                writer.Close();
                Title = myFeed.Title.Text;
                podcastDownloadLocation = podcastLocation + "/downloads/" + myFeed.Title.Text + "/";
                System.IO.Directory.CreateDirectory(podcastDownloadLocation);

            }
            catch (Exception e)
            {
                Console.WriteLine("Malformed URI");
                Console.WriteLine(e.StackTrace);
            }

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(myFeed.ImageUrl, "podcasts/" + myFeed.Title.Text + ".png");
                    feedPic = Image.FromFile("podcasts/" + myFeed.Title.Text + ".png");
                }
                catch (Exception e)
                {
                    System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                    Stream myStream = myAssembly.GetManifestResourceStream("FeedReed.unknown.png");
                    feedPic = new Bitmap(myStream);
                    Console.WriteLine("Image not found");
                    Console.WriteLine(e.StackTrace);
                    
                }
            }
        }

        public FeedHandler(String title, String description ,String location, String imageLocation, String lastUpdated)
        {
            myFeed = SyndicationFeed.Load(XmlReader.Create("podcasts/"+title+".xml"));
            myFeed.Title = new TextSyndicationContent(title);
            myFeed.BaseUri = new Uri(location);
            feedLocation = new Uri(location);
            Title = title;            
            myFeed.ImageUrl = new Uri(imageLocation, UriKind.Relative);
            podcastDownloadLocation = podcastLocation + "/downloads/" + myFeed.Title.Text + "/";
            System.IO.Directory.CreateDirectory(podcastDownloadLocation);
            try
            {
                feedPic = Image.FromFile(imageLocation);
            }
            catch (Exception e)
            {
                System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                Stream myStream = myAssembly.GetManifestResourceStream("FeedReed.unknown.png");
                feedPic = new Bitmap(myStream);
                Console.WriteLine("Image not found");
                Console.WriteLine(e.StackTrace);
            }
            
        }

        public void saveXML()
        {
            rssFormatter = new Rss20FeedFormatter(myFeed);
            writer = XmlWriter.Create("podcasts/" + myFeed.Title.Text + ".xml");
            rssFormatter.WriteTo(writer);
            writer.Close();
        }


        public void printOut()
        {
            Console.WriteLine("DONE");
        }

        public String getItemText()
        {
            String output = "";
            try
            {
                foreach (SyndicationItem item in myFeed.Items)
                {
                    output += "============================================" + "\r\n";
                    output += item.Title.Text + "\r\n";
                    output += item.Summary.Text + "\r\n";
                    output += item.Links.First().Uri + "\r\n";
                    output += "============================================" + "\r\n";
                }
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "FUCK";
            }
        }

        public List<SyndicationItem> getItems()
        {
            List<SyndicationItem> tempList = new List<SyndicationItem>();
            foreach (SyndicationItem item in myFeed.Items)
            {
                item.Summary = item.Summary;
                tempList.Add(item);
            }
            return tempList;
        }

        public Uri getImageLocation()
        {
            return myFeed.ImageUrl;
        }

        public Image getImage()
        {
            WebClient client = new WebClient();
            return Image.FromStream(client.OpenRead(getImageLocation()));
        }

        public ImageSource getImageSource(Bitmap bitmap)
        {
            var hbitmap = bitmap.GetHbitmap();
            try
            {
                var imageSource = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero,
                    System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));


                return imageSource;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }

        }

        public String Title
        {
            get { return titleName; }
            set { titleName = value; }
        }

        public String getLocationString()
        {
            if (reader != null)
            {
                return reader.BaseURI.ToString();
            }
            if (myFeed.BaseUri != null)
            {
                return myFeed.BaseUri.ToString();
            }
            else
            {
                return "null";
            }
        }

        public Image getImageForDisplay()
        {
            return feedPic;


        }

        public String getImagePath()
        {
            return "podcasts/" + myFeed.Title.Text + ".png";
        }

        public String getLastUpdated()
        {
            return myFeed.LastUpdatedTime.ToString();
        }

        public String getDescription()
        {
            return myFeed.Description.Text;
        }

        override
        public String ToString()
        {
            return myFeed.Title.Text;
        }

        public void updateFeed()
        {
            myFeed = SyndicationFeed.Load(XmlReader.Create(myFeed.BaseUri.ToString()));
            saveXML();
        }

        public SyndicationItem getItemByName(String itemName)
        {
            try
            {
                List<SyndicationItem> itemList = getItems();
                foreach (SyndicationItem item in itemList)
                {
                    if (itemName.Equals(item.Title.Text));
                    return item;
                }
            } catch (Exception e)
            {
                Console.WriteLine("Feed was probably null");
                Console.WriteLine(e.StackTrace);
            }

            return null;
        }

        public SyndicationItem getItemByIndex(int i)
        {
            List<SyndicationItem> itemList = getItems();
            return itemList.ElementAt(i);
        }

        public String getFileDownloadLocation()
        {
            return podcastDownloadLocation;
        }

    }

}