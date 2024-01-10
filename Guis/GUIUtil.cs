using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends
{
    public static class GUIUtil
    {
        public static Rect GetRect(Camera camera, Vector3 worldPosition, float width, float height)
        {
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);
            return new Rect(screenPosition.x, Screen.height - screenPosition.y, width, height);
        }
    }
}
