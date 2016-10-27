using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace WpfApplication1
{
    /// <summary>
    /// Encapsulation of List<MediaItem> 
    /// Manipulation of media list
    /// 
    /// Playlist object can :
    /// Should be able to : Create and Delete a MediaItem, Clean a list, Serialize and Deserialize a list, Change position of a MediaItem
    /// Option : Sort MediaItem (name, author, duration), Randomize element form list 
    /// </summary>
    class Playlist
    {
        public List<MediaItem> list;
        private int _inPlay = 0;
        private int _numberOfItems = 0;
        private string _name;
        private bool _deletable;
        private string _path = AppDomain.CurrentDomain.BaseDirectory;

        public Playlist(string name, bool deletable)
        {
            list = new List<MediaItem>();

            _deletable = deletable;
            _name = name;
            _path = _path + name + ".xml";
        }

        public string getName()
        {
            return _name;
        }

        public int CountMediaInList()
        {
            int val = 0;

            val = list.Count();
            _numberOfItems = val;

            return val;
        }

        public void ClearPlaylist()
        {
            if (_deletable)
             list.Clear();
        }

        public int  AddMediainPlaylist(MediaItem item)
        {
            list.Add(item);
            int pos = list.IndexOf(item);
            
            _numberOfItems += 1;

            this.SerializePlaylist();

            return pos;
        }

        public void RemoveMediaFromPlaylistAtPosition(int index)
        {
            if ((index - 1) >= 0 && (index - 1) < _numberOfItems)
            {
                list.RemoveAt((index - 1));
                _numberOfItems -= 1;
            }
        }

        public void SerializePlaylist()
        {
            using (XmlWriter writer = XmlWriter.Create(_path)) {

                writer.WriteStartDocument();
                writer.WriteStartElement("List");

                foreach (MediaItem item in list)
                {
                    item.SerializeItemInFile(writer);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        /// Commantary = Debug line
        public void DeserializePlaylist()
        {
            Uri tmp = new Uri(_path);

            if (File.Exists(tmp.LocalPath))
            {
                var xml = XDocument.Load(@_path);

                var query = from mI in xml.Root.Descendants("MediaItem")
                            select mI.Element("Uri");

                //TextWriter tw = new StreamWriter("Debug.txt");


                foreach (string uri in query)
                {
                    //string tmp = "New uri : " + uri;
                    //tw.WriteLine(tmp);

                    Uri n_uri = new Uri(Uri.UnescapeDataString(uri));
                    //string tmp2 = "Uri(sation) => " + n_uri.ToString();
                    //tw.WriteLine(tmp2);

                    MediaItem n_mItem = new MediaItem(n_uri);
                    AddMediainPlaylist(n_mItem);
                }

                //tw.Close();
            }
        }

        public void getFromFile(string path)
        {

        }

        public void MoveItemInPlaylist(int oldIndex, int newIndex)
        {
            if (((oldIndex - 1) >= 0 && (oldIndex) < _numberOfItems)
                && (newIndex - 1) >= 0 && (newIndex - 1) < _numberOfItems)
            {
                MediaItem item = list[(oldIndex - 1)];

                list.RemoveAt((oldIndex - 1));

                if (newIndex > oldIndex)
                    newIndex--;

                list.Insert((newIndex - 1), item);
            }
        }

        public int getMediaInPlayPos()
        {
            return _inPlay;
        }

        public Uri getMediaUri(int pos)
        {
            MediaItem item = list[pos];

            _inPlay = pos;

            return item._Uri;
        }

        public string getMediaName(int pos)
        {
            MediaItem item = list[pos];

            return item._nameFile;
        }

        public string getMediaLength(int pos)
        {
            MediaItem item = list[pos];

            return item._length;
        }

        public string getMediaAlbum(int pos)
        {
            MediaItem item = list[pos];

            return item._album;
        }

        public string getMediaArtist(int pos)
        {
            MediaItem item = list[pos];

            return item._artist;
        }

        public bool canBeDelete()
        {
            return _deletable;
        }
    }
}
