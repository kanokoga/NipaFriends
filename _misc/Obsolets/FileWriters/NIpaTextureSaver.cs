using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace NipaFriends.FileWriters
{
    public static class NipaTextureSaver
    {
        public static void Save(Texture2D texture, string dirPath, string fileName)
        {
            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }
            fileName += ".png";
            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(System.IO.Path.Combine(dirPath, fileName), bytes);
        }
    }
}