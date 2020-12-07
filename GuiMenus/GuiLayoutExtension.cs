using UnityEngine;

namespace NipaFriends.GuiMenus
{
    public class GuiLayoutExtension : GUILayout
    {
        public static void StatusBox(string key, string value)
        {
            using(var v = new VerticalScope("box"))
            {
                BeginHorizontal();
                Label(key);
                FlexibleSpace();
                Label(value);
                EndHorizontal();
            }
        }

        public static void StatusBox(string key, string value, Color color)
        {
            using(var v = new VerticalScope("box"))
            {
                BeginHorizontal();
                Label(key);
                FlexibleSpace();
                var temp = GUI.color;
                GUI.color = color;
                Label(value);
                GUI.color = temp;
                EndHorizontal();
            }
        }

        public static void StatusBox(string key, string value, bool greenOrRed)
            => StatusBox(key, value, greenOrRed ? Color.green : Color.red);

        public static void StatusTextField(string key, string value)
        {
            using(var v = new VerticalScope("box"))
            {
                BeginHorizontal();
                Label(key);
                FlexibleSpace();
                TextArea(value);
                EndHorizontal();
            }
        }
    }
}
