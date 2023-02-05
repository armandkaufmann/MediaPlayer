using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TagLib.File currentFile = null;
        private NowPlaying nowPlayingView;
        private EditTag editView;
        private bool isPlaying = false;
        private bool isPaused = false;

        public MainWindow()
        {
            InitializeComponent();
            disable_start_buttons(); //disabling buttons that require a sound file
            nowPlayingView = new NowPlaying(); //creating user control for the now playing view
            editView = new EditTag(); //creating user control for the editing view
            editView.SaveChanges += new EventHandler(editView_Save_Clicked); //subscribing 
        }

        private void editView_Save_Clicked(object? sender, EventArgs e)
        {
            //stop playback so we can save
            isPlaying = false;
            isPaused = false;
            MediaPlayer.Stop();
            MediaPlayer.Close();
            Thread.Sleep(250); //giving sometime for the stream to close before saving

            try
            {
                currentFile.Tag.Title = editView.getTitle();
                currentFile.Tag.Performers = new string[] { editView.getArtist() };
                currentFile.Tag.Album = editView.getAlbum();
                currentFile.Tag.Year = (uint)editView.getYear();
                currentFile.Save();

                MessageBox.Show("Saved changes made to your audio file!", "Successfully saved!", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (ArgumentException error)
            {
                MessageBox.Show(error.Message, "Tag Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                set_edit_view();
            } catch (Exception error)
            {
                //MessageBox.Show("Could not save the changes you made to the tag. Please try again.", "Tag Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show(error.Message + ". Please try again.", "Tag Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                set_edit_view();
            }
            

        }

        private void disable_start_buttons()
        {
            //disabling buttons when program is first started up
            menu_tag_mp3.IsEnabled = false;
            menu_edit_tag.IsEnabled = false;
            menu_play.IsEnabled = false;
            menu_pause.IsEnabled = false;
            menu_stop.IsEnabled = false;

            btn_play.IsEnabled = false;
            btn_pause.IsEnabled = false;
            btn_stop.IsEnabled = false;
            btn_now_playing.IsEnabled = false;
            btn_edit_tag.IsEnabled = false;


        }

        private void enable_buttons()
        {
            //enabling buttons when file is loaded in
            menu_tag_mp3.IsEnabled = true;
            menu_edit_tag.IsEnabled = true;
            menu_play.IsEnabled = true;
            menu_pause.IsEnabled = true;
            menu_stop.IsEnabled = true;

            btn_play.IsEnabled = true;
            btn_pause.IsEnabled = true;
            btn_stop.IsEnabled = true;
            btn_now_playing.IsEnabled = true;
            btn_edit_tag.IsEnabled = true;
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.openFile();
        }

        private void PlayCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayer.Play(); //play the sound file with the media player
            isPlaying = true;
            isPaused = false;
            set_now_playing_view(); //setting the main view to show the now playing song
        }

        private void PlayCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.currentFile != null && (!isPlaying || isPaused);
        }

        private void PauseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayer.Pause(); //pausing the media player
            isPlaying = false; 
            isPaused = true; 
        }
        private void PauseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.isPlaying; //can only pause if the song is currently playing
        }
        private void StopCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayer.Stop(); //stopping the playback
            this.isPaused = false; //setting the playback flags
            this.isPlaying = false;
        }

        private void StopCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.isPlaying || this.isPaused; //can only stop if the song is playing, or is paused
        }


        private void openFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == true)
            {
                //checking if the file is a playable audio file
                //if not will catch error and throw message box error message
                try
                {
                    this.currentFile = TagLib.File.Create(fileDialog.FileName);
                    set_now_playing_view(); //set the view to now playing
                    MediaPlayer.Source = new Uri(currentFile.Name); //pointing the media player to the source audio file
                    enable_buttons(); //enabling all buttons for playback
                    currentFile.Dispose(); //closing the current file stream
                }
                catch(Exception e)
                {
                    MessageBox.Show("Could not load file, please ensure that it is a playable audio file (MP3, WAV, FLAC, etc..)", "Load error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.currentFile = null;
                }

            }
        }

        private void set_edit_view()
        {
            var tags = get_tags();//getting the tags of the audio file

            editView.loadSongInfo((string) tags[0], (string) tags[1], (string) tags[2], (string) tags[3]); //loading it into the edit view
            display_song.Content = editView; //displaying the content of the edit view on the main song display page
        }

        private void set_now_playing_view()
        {
            var tags = get_tags(); //getting the audio tags

            nowPlayingView.updateSongInfo((string) tags[0], (string) tags[1], (string) tags[2], (string) tags[3]);
            display_song.Content = nowPlayingView;
        }

        private ArrayList get_tags()
        {
            ArrayList tags = new ArrayList();

            //checking if any of the tags are empty
            //if so, adding a default value of empty string
            if (currentFile.Tag.Title == null)
            {
                tags.Add("");
            }
            else
            {
                tags.Add(currentFile.Tag.Title);
            }

            if (currentFile.Tag.Performers.Length == 0)
            {
                tags.Add("");
            }
            else
            {
                tags.Add(currentFile.Tag.Performers[0]);
            }

            if (currentFile.Tag.Album == null)
            {
                tags.Add("");
            }
            else
            {
                tags.Add(currentFile.Tag.Album);
            }

            if (currentFile.Tag.Year == 0)
            {
                tags.Add("");
            }
            else
            {
                tags.Add(currentFile.Tag.Year.ToString());
            }

            return tags;
        }

        private void btn_edit_tag_Click(object sender, RoutedEventArgs e)
        {
            set_edit_view();
        }

        private void btn_now_playing_Click(object sender, RoutedEventArgs e)
        {
            set_now_playing_view();
        }

        private void menu_edit_tag_Click(object sender, RoutedEventArgs e)
        {
            set_edit_view();
        }

        private void menu_tag_mp3_Click(object sender, RoutedEventArgs e)
        {
            set_edit_view();
        }
    }
}
