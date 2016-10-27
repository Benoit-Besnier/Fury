using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace WpfApplication1
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// MEDIA PLAY Fonctionnality listing : Open/Plays/Pause/Stops media files, Slider, Volume, Fullscreen, Media file rescale.
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        private bool mediaWindowIsFullscreen = false;
        private bool mediaPlayerIsLooping = false;
        private bool listIsShown = false;
        private bool orgaIsShown = false;
        private Library lib;
        private Playlist curPlaylist;
   
        /*
        ** Constructor
        */
        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            lib = new Library();
            lib.AddPlaylist("playlist-debug", true);
            curPlaylist = lib.getPlaylist(3);

            curPlaylist.DeserializePlaylist();

            if (curPlaylist.CountMediaInList() > 0)
                Display.Source = curPlaylist.getMediaUri(0);

            PlayListScreen.Visibility = Visibility.Collapsed;
            Organizer.Visibility = Visibility.Collapsed;
            ButtonForFullscreen.Visibility = Visibility.Collapsed;
            ButtonImageOnScreen();
        }

        private void ButtonImageOnScreen()
        {
            ImageOpen.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\upload58.png"));
            ImagePlay.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\play106.png"));
            ImagePause.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\pause44.png"));
            ImagePrev.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\rewind44.png"));
            ImageStop.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\media26.png"));
            ImageNext.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\fast44.png"));
            ImageLoop.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\update21.png"));
            ImageList.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\list89.png"));
            ImageTree.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\hierarchical9.png"));

            ImagePlay2.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\play106.png"));
            ImagePause2.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\pause44.png"));
            ImagePrev2.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\rewind44.png"));
            ImageStop2.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\media26.png"));
            ImageNext2.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\fast44.png"));
        }

            /*
            ** Method for Time
            */
        private void Timer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if ((Display.Source != null) && (Display.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = Display.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = Display.Position.TotalSeconds;
            }

        }

        /*
        ** Method for Event
        */
        private void ViewGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Display.Volume += (e.Delta > 0) ? 0.05 : -0.05;
            if (Display.Volume >= 1.0)
                Display.Volume = 1.0;
            if (Display.Volume <= 0.0)
                Display.Volume = 0.0;
        }

        private void Display_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mediaWindowIsFullscreen)
            {
                ActionButton.Visibility = Visibility.Collapsed;
                Sliders.Visibility = Visibility.Collapsed;
                MenuBar.Visibility = Visibility.Collapsed;
                MediaWindow.ResizeMode = ResizeMode.NoResize;
                MediaWindow.WindowStyle = WindowStyle.None;
                MediaWindow.WindowState = WindowState.Maximized;
                Organizer.Visibility = Visibility.Collapsed;
                PlayListScreen.Visibility = Visibility.Collapsed;
                ButtonForFullscreen.Visibility = Visibility.Visible;
                mediaWindowIsFullscreen = true;
                listIsShown = false;
                orgaIsShown = false;
            }
            else
            {
                ActionButton.Visibility = Visibility.Visible;
                Sliders.Visibility = Visibility.Visible;
                MenuBar.Visibility = Visibility.Visible;
                MediaWindow.ResizeMode = ResizeMode.CanResize;
                MediaWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                MediaWindow.WindowState = WindowState.Normal;
                ButtonForFullscreen.Visibility = Visibility.Collapsed;
                mediaWindowIsFullscreen = false;
            }
        }

        private void PlayListScreen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int Sidx = PlayListScreen.SelectedIndex;

            Display.Stop();

            Uri Furi = curPlaylist.getMediaUri(Sidx);
            if (Furi != null)
            {
                Display.Source = curPlaylist.getMediaUri(Sidx);
                Display.Play();
                mediaPlayerIsPlaying = true;
            }
            else
                mediaPlayerIsPlaying = false;
        }

        private void Border_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string link = (string)((System.Windows.DataObject)e.Data).GetFileDropList()[0];
            Uri n_uri = new Uri(link);

            curPlaylist.AddMediainPlaylist(new MediaItem(n_uri));
            Update_playlistscreen();

            Display.Stop();
            Display.Source = curPlaylist.getMediaUri(curPlaylist.CountMediaInList() - 1);
            Display.Play();

            PlayListScreen.SelectedIndex = curPlaylist.CountMediaInList() - 1;

            if ((Display != null) && (Display.Source != null))
            {
                Display.Play();
                mediaPlayerIsPlaying = true;
            }
        }

        private void Organizer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int Sidx = Organizer.SelectedIndex;

            Display.Stop();
            curPlaylist = lib.getPlaylist(Sidx);
            Update_playlistscreen();

            if (curPlaylist.CountMediaInList() > 0)
            {
                Display.Source = curPlaylist.getMediaUri(0);
                PlayListScreen.SelectedIndex = 0;
                Display.Play();
                mediaPlayerIsPlaying = true;
            }
            else
                mediaPlayerIsPlaying = false;
        }

        /*
        ** Method for Action
        */
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            Uri tmp = null;
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                tmp = new Uri(openFileDialog.FileName);

            //Add playlist method here
            if (tmp != null)
            {
                curPlaylist.AddMediainPlaylist(new MediaItem(tmp));
                Update_playlistscreen();

                Display.Source = curPlaylist.getMediaUri(curPlaylist.CountMediaInList() - 1);
            }
            if ((Display != null) && (Display.Source != null))
            {
                Display.Play();
                mediaPlayerIsPlaying = true;
            }
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (Display != null) && (Display.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Display.Play();
            mediaPlayerIsPlaying = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Display.Pause();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Display.Stop();
            mediaPlayerIsPlaying = false;
        }

        private void Display_MediaEnded(object sender, RoutedEventArgs e)
        {
            ///
            /// The code below is needed for the 'repeat' button
            ///
            if (mediaPlayerIsLooping)
            {
                Display.Stop();
                Display.Play();
            }
            else if (curPlaylist.getMediaInPlayPos() < (curPlaylist.CountMediaInList() - 1))
            {
                Next_track(null, null);
            }
            else
            {
                Display.Stop();
                if (mediaWindowIsFullscreen)
                    Display_MouseUp(null, null);
                mediaPlayerIsPlaying = false;
            }
        }

        /*
        ** If you want to change the appearance of the 'repeat', 'next', 'previous', 'list' buttons, insert your code below
        */
        private void Previous_track(object sender, RoutedEventArgs e)
        {
            int pos = curPlaylist.getMediaInPlayPos();

            if (!mediaPlayerIsPlaying)
                mediaPlayerIsPlaying = true;

            Display.Stop();
            if ((pos - 1) < 0)
            {
                Display.Play();
            }
            else
            {
                Display.Source = curPlaylist.getMediaUri(pos - 1);
                Display.Play();
                PlayListScreen.SelectedIndex = PlayListScreen.SelectedIndex - 1;
            }
        }

        private void Next_track(object sender, RoutedEventArgs e)
        {
            int pos = curPlaylist.getMediaInPlayPos();

            if (!mediaPlayerIsPlaying)
                mediaPlayerIsPlaying = true;

            Display.Stop();
            if ((pos + 1) >= curPlaylist.CountMediaInList())
            {
                Display.Play();
            }
            else
            {
                Display.Source = curPlaylist.getMediaUri(pos + 1);
                Display.Play();
                PlayListScreen.SelectedIndex = PlayListScreen.SelectedIndex + 1;
            }
        }

        private void Loop_track(object sender, RoutedEventArgs e)
        {
            //Loop_button.H
            mediaPlayerIsLooping = !mediaPlayerIsLooping;
            if (mediaPlayerIsLooping)
                ImageLoop.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\update21-2.png"));
            else
                ImageLoop.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\update21.png"));

        }

        private void Update_playlistscreen()
        {

            PlayListScreen.Items.Clear();

            int max = curPlaylist.CountMediaInList();

            for (int i = 0; i < max; ++i)
            {
                if (curPlaylist.list[i].MediaItemStillAccessible())
                {
                    string[] myList = new string[4];

                    myList[0] = curPlaylist.getMediaName(i);
                    myList[1] = curPlaylist.getMediaLength(i);
                    myList[2] = curPlaylist.getMediaAlbum(i);
                    myList[3] = curPlaylist.getMediaArtist(i);

                    //PlayListScreen.Items.Add(new {Name=myList[0], Duration=myList[1], Album=myList[2], Artist=myList[3]} );

                    PlayListScreen.Items.Add(myList[0] + "\t\t\t" + myList[1] + "\t\t\t" + myList[2] + "\t\t\t" + myList[3]);

                }
                else
                    PlayListScreen.Items.Add(curPlaylist.getMediaName(i) + " - NOT FOUND");
            }
        }

        private void Aff_list(object sender, RoutedEventArgs e)
        {
            if (!listIsShown)
            {
                Update_playlistscreen();
                PlayListScreen.Visibility = Visibility.Visible;
                PlayListScreen.SelectedIndex = curPlaylist.getMediaInPlayPos();
                ImageList.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\list89-2.png"));
            }
            else
            {
                PlayListScreen.Visibility = Visibility.Collapsed;
                ImageList.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\list89.png"));
            }
            listIsShown = !listIsShown;
        }

        private void Update_organizer()
        {
            Organizer.Items.Clear();

            Organizer.Items.Add("Musics");
            Organizer.Items.Add("Videos");
            Organizer.Items.Add("Pictures");

            int numberofplaylist = lib.getNumberOfPlaylist();
            Playlist tlist = null;

            for (int i = 4; i <= numberofplaylist; ++i)
            {
                tlist = lib.list[(i - 1)];
                string name = tlist.getName();
                Organizer.Items.Add(name);
            }

            Organizer.SelectedIndex = lib.getCurrentPlaylist();
        }

        private void Tree_button_Click(object sender, RoutedEventArgs e)
        {
            if (!orgaIsShown)
            {
                Update_organizer();
                Organizer.Visibility = Visibility.Visible;
                ImageTree.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\hierarchical9-2.png"));

            }
            else
            {
                Organizer.Visibility = Visibility.Collapsed;
                ImageTree.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\hierarchical9.png"));

            }
            orgaIsShown = !orgaIsShown;
        }

        /*
        ** Method for Slider
        */
        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            Display.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
            lblProgressStatus2.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private void sliProgress_MouseUp(object sender, RoutedEventArgs e)
        {
            Display.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

    }
}
