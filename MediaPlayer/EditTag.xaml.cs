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

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for EditTag.xaml
    /// </summary>
    public partial class EditTag : UserControl
    {
        public event EventHandler SaveChanges; //delegate so that the parent view can subscribe to this event

        public EditTag()
        {
            InitializeComponent();
        }

        public void loadSongInfo(string title, string aritst, string album, string year)
        {
            txt_song_title.Text = title;
            txt_song_artist.Text = aritst;
            txt_song_album.Text = album;
            txt_song_year.Text = year;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (this.SaveChanges != null)
            {
                this.SaveChanges(this, e);
            }
        }

        public string getTitle()
        {
            return txt_song_title.Text;
        }

        public string getArtist()
        {
            return txt_song_artist.Text;
        }

        public string getAlbum()
        {
            return txt_song_album.Text;
        }

        public int getYear()
        {
            //try-catch to catch any errors with parsing string to int
            try
            {
                //checking if the year is empty, if so
                //return default 0
                if (txt_song_year.Text == "")
                {
                    return 0;
                }

                int year = int.Parse(txt_song_year.Text);

                return year;
            }
            catch (FormatException)
            {
                throw new ArgumentException("Year has to be a valid number.");
            }

        }
    }
}
