using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace NipaFriends
{
    public static class ScreenShot
    {
        public static void TakeScreenShot(string name)
        {
            var directoryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
            var fileName = name + "_" + System.DateTime.Now.ToFileTime() + ".png";
            var filePath = Path.Combine(directoryPath, fileName);
            ScreenCapture.CaptureScreenshot(filePath);
        }
    }
}
