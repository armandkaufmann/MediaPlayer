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
    /// Interaction logic for NowPlaying.xaml
    /// </summary>
    public partial class NowPlaying : UserControl
    {
        public NowPlaying()
        {
            InitializeComponent();
        }

        public void updateSongInfo(string title, string aritst, string album, string year)
        {
            txt_song_title.Text = title;
            txt_song_artist.Text = aritst;
            txt_song_album.Text = album;
            txt_song_year.Text = year;
        }
    }
}
