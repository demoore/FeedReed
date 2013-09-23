using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FeedReed
{

    class FeedList
    {
        private ObservableCollection<FeedHandler> feeds;


        public FeedList()
        {
            feeds = new ObservableCollection<FeedHandler>();

        }

        public void add(FeedHandler feed)
        {
            if (feeds.Contains(feed))
            {
                return;
            }
            feeds.Add(feed);
        }

        public void remove(FeedHandler feed)
        {
            if (feeds.Contains(feed))
            {
                feeds.Remove(feed);
            }

            return;
        }

        public ObservableCollection<FeedHandler> getFeeds()
        {
            return feeds;
        }

        public FeedHandler getFeedAtIndex(int i)
        {
            return feeds.ElementAt(i);
        }


        public void saveXML()
        {
            XmlWriter writer = XmlWriter.Create("podcasts/podcastList.xml");
            writer.WriteStartDocument();
            writer.WriteStartElement("Podcasts");

            foreach (FeedHandler feed in feeds)
            {
                writer.WriteStartElement("Podcast");
                writer.WriteElementString("title", feed.Title);
                writer.WriteElementString("description", feed.getDescription());
                writer.WriteElementString("location", feed.getLocationString());
                writer.WriteElementString("image", feed.getImagePath());
                writer.WriteElementString("lastUpdated", feed.getLastUpdated());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        public void openXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("podcasts/podcastList.xml");
            XmlNodeList parameters = doc.SelectNodes("Podcasts/Podcast");
            Console.WriteLine(parameters.Count);
            foreach (XmlNode parameter in parameters)
            {
                String title = parameter.SelectSingleNode("title").InnerText;
                String description = parameter.SelectSingleNode("description").InnerText;
                String uri = parameter.SelectSingleNode("location").InnerText;
                String image = parameter.SelectSingleNode("image").InnerText;
                String lastUpdated = parameter.SelectSingleNode("lastUpdated").InnerText;

                feeds.Add(new FeedHandler(title, description, uri, image, lastUpdated));
                Console.WriteLine(title);
            }

            Console.WriteLine(feeds.Count);
        }

    }
}
