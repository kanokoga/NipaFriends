using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace NipaFriends.GuiMenus
{
    public class MultiLineLog
    {
        private List<string> logs = new List<string>();
        private int maxLog = 10;
        private string separation = "";

        public MultiLineLog(int maxLogCount)
            => this.maxLog = maxLogCount;

        public MultiLineLog(int maxLogCount, string separation)
        {
            this.maxLog = maxLogCount;
            this.separation = separation;
        }

        public void AddLog(string log)
        {
            this.logs.Add(log);
            while (this.logs.Count > this.maxLog)
            {
                this.logs.RemoveAt(0);
            }
        }

        public string Log
            => this.logs.Count > 0 ?
            (this.separation.Length > 0 ?
            this.logs.Select(v => v.ToString()).Aggregate((sum, element) => sum 
                + Environment.NewLine + this.separation + Environment.NewLine + element)
            : this.logs.Select(v => v.ToString()).Aggregate((sum, element) => sum + Environment.NewLine + element))
            : "(no log)";

        public void Clear()
            => this.logs.Clear();
    }
}