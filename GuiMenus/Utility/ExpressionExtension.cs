using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends.GuiMenus
{
    public static class ExpressionExtension
    {
        public static string ToYesNo(this bool positive)
            => positive ? "YES" : "NO";
    }
}
