using System;
using UnityEngine;

namespace RapidGUI
{
    public static partial class RGUI
    {
        static GUILayoutOption fieldWidthMin = GUILayout.MinWidth(50f);

        static object StandardField(object v, Type type) => StandardField(v, type, null);
        private static object StandardField(object v, Type type, GUILayoutOption option)
        {
            object obj = v;
            UnparsedStr unparsedStr = UnparsedStr.Create();
            using (new ColorScope((unparsedStr.hasStr && !unparsedStr.CanParse(type)) ? Color.red : GUI.color))
            {
                //string text = unparsedStr.Get() ?? ((v != null) ? string.Format("{0:0}", v) : "");
                string text = unparsedStr.Get() ?? ((v != null) ? string.Format("{0:0.000}", v) : "");
                string text2 = GUILayout.TextField(text, new GUILayoutOption[]
                {
            GUILayout.Height(21f),
            option ?? fieldWidthMin
                });
                if (text2 != text)
                {
                    try
                    {
                        obj = Convert.ChangeType(text2, type);
                        if (obj.ToString() == text2)
                        {
                            text2 = null;
                        }
                    }
                    catch
                    {
                    }
                    unparsedStr.Set(text2);
                }
            }
            return obj;
        }
    }
}