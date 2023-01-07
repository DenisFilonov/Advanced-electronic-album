using System;

namespace WpfApp3_pd
{
    [Serializable]
    internal class ImageMark
    {
        public string Path { get; set; }
        public int Mark { get; set; }

        public ImageMark(string path)
        {
            Path= path;
            Mark = 0;
        }
    }
}
