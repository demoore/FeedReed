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
    /// Interaction logic for addFeedDialog.xaml
    /// </summary>
    public partial class AddFeedDialog : Window
    {
        public AddFeedDialog()
        {
            InitializeComponent();
            responseTextBox.Text = Clipboard.GetText();
        }

        public string ResponseText {
            get {return responseTextBox.Text;}
            set { responseTextBox.Text = value; }
        }

        public bool Canceled { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Canceled = false;
            Close();
        }

        
    }
}
