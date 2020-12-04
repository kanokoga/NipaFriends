using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Common.Debugs
{
    public interface IGuiDebugger 
    {
        event Action OnClose;
        string Title { get; }
        void ShowGuiContent();
        void Open();
        void Close();
        void Dispose();
    }
}