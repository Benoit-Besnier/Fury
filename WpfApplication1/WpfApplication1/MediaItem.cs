using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TagLib;

namespace WpfApplication1
{
    /// <summary>
    /// Information from Media files
    /// 
    /// Media Item provides : Uri
    /// Should provides : Name, Author, Duration, Date of production...
    /// </summary>
    class MediaItem
    {
        public Uri _Uri { get; set; }
        public string _nameFile { get; set; }
        public string _length = "__:__:__";
        public string _album = "unknown";
        public string _artist = "unknown";
        private bool _isValid = false;


        public MediaItem(Uri Uri)
        {
            this._Uri = Uri;
            if (this._Uri.IsFile)
            {
                this._nameFile = Path.GetFileName(this._Uri.LocalPath);
                if (System.IO.File.Exists(this._Uri.LocalPath))
                {
                    this._isValid = true;
                    string path = Path.GetFullPath(this._Uri.LocalPath);
                    string ext = Path.GetExtension(path);
                    /** All media supported by TagLib **/
                    if (ext == ".mp3" || ext == ".mp4" || ext == ".wma" || ext == ".flac" || ext == ".wav" || ext == ".mkv")
                    {
                        TagLib.File media = TagLib.File.Create(path);
                        this._length = TimeSpan.FromSeconds(Math.Ceiling(media.Properties.Duration.TotalSeconds)).ToString();
                        this._album = media.Tag.Album;
                        this._artist = media.Tag.Performers[0];
                    }
                }
            }
        }

        public bool MediaItemStillAccessible()
        {
            bool check = System.IO.File.Exists(this._Uri.LocalPath);
            if ((_isValid && !check) || (!_isValid && check))
            {
                _isValid = check;
            }
            return _isValid;
        }

        public void SerializeItemInFile(XmlWriter writer)
        {
            writer.WriteStartElement("MediaItem");

            writer.WriteElementString("Uri", this._Uri.AbsolutePath);
            
            writer.WriteEndElement();
        }
    }
}