using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Library
    {
        public List<Playlist> list;
        private int _numberOfPlaylist = 0;
        private int _currentPlaylist = 0;

        public Library()
        {
            list = new List<Playlist>();

            this.AddPlaylist("Musics", false);
            PlaylistForFile("Musics", list[0]);
            this.AddPlaylist("Videos", false);
            PlaylistForFile("Videos", list[1]);
            this.AddPlaylist("Pictures", false);
            PlaylistForFile("Pictures", list[2]);
        }

        public void PlaylistForFile(string name, Playlist PlayL)
        {
            string tpath = null;

            if (name == "Musics")
                tpath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            else if (name == "Videos")
                tpath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            else if (name == "Pictures")
                tpath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            else
                tpath = null;

            if (tpath != null)
            {
                var files = Directory.EnumerateFiles(tpath, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".mp3") || s.EndsWith(".jpg") || s.EndsWith(".png") 
                    || s.EndsWith(".mkv") || s.EndsWith(".flac") || s.EndsWith(".wma") || s.EndsWith(".avi") 
                    || s.EndsWith(".mp4") || s.EndsWith(".jpeg") || s.EndsWith(".mpg") || s.EndsWith(".mpeg"));

                foreach (string fpath in files)
                    PlayL.AddMediainPlaylist(new MediaItem(new Uri(fpath)));

            }
        }

        public void AddPlaylist(string name, bool deletable)
        {
            Playlist nPl = new Playlist(name, deletable);
            list.Add(nPl);

            _numberOfPlaylist += 1;
        }

        public void DeletePlaylist(int index)
        {
            if ((index) < _numberOfPlaylist && (index) > 0)
            {
                Playlist tmp = list[index];

                if (tmp.canBeDelete())
                {
                    tmp.ClearPlaylist();
                    list.RemoveAt(index);
                    _currentPlaylist = 0;
                    _numberOfPlaylist -= 1;
                }
            }
        }

        public Playlist getPlaylist(int index)
        {
            Playlist tmp = null;

            if (index < _numberOfPlaylist && index >= 0)
            {
                tmp = list[index];
                _currentPlaylist = index;
            }

            return tmp;
        }

        public int getNumberOfPlaylist()
        {
            return _numberOfPlaylist;
        }

        public int getCurrentPlaylist()
        {
            return _currentPlaylist;
        }
    }
}
