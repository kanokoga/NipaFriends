using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Linq;

namespace NipaFriends.FileWriters
{
    public static class NipaWriter
    {
        public static void Write(string text, string dirPath, string fileName, bool newLine = true)
        {
            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }
            if (newLine == true)
            {
                text += Environment.NewLine;
            }

            File.AppendAllText(System.IO.Path.Combine(dirPath, fileName), text);
        }

        public static string NewLine
            => Environment.NewLine;
    }
}
