using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace NipaFriends.FileWriters
{
    public static class NipaService
    {

        public static string GetDirPath(Environment.SpecialFolder baseDir, string dir)
            => Path.Combine(Environment.GetFolderPath(baseDir), dir);

        public static string GetDirPath(Environment.SpecialFolder baseDir, List<string> directories)
        {
            directories.Insert(0, Environment.GetFolderPath(baseDir));
            return Path.Combine(directories.ToArray());
        }


        public static string GetTime(TimeLevel level, bool year = false)
        {
            var format = "";

            switch (level)
            {
                case TimeLevel.Day:
                    format = "MM-dd";
                    break;
                case TimeLevel.Hour:
                    format = "MM-dd_HH";
                    break;
                case TimeLevel.Minute:
                    format = "MM-dd_HHmm";
                    break;
                case TimeLevel.Second:
                    format = "MM-dd_HHmm-ss";
                    break;
                case TimeLevel.MiliSecond:
                    format = "MM-dd_HHmm-ssfff";
                    break;
                default:
                    break;
            }

            if (year == true)
            {
                format = "yyyy-" + format;
            }

            return DateTime.Now.ToString(format);
        }
    }
}
