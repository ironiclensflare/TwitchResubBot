using System;
using System.Configuration;
using System.IO;

namespace TwitchResubBot
{
    public class TextLogger
    {
        private readonly string _path;

        public TextLogger(string path)
        {
            _path = path;
        }

        public void WriteLine(string text)
        {
            using (var stream = File.AppendText(_path))
            {
                stream.WriteLine(text);
            }
        }
    }
}
