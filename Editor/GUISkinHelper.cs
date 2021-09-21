using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class GUISkinHelper
{
    /// <summary>
    /// 字体皮肤
    /// </summary>
    public static GUIStyle TextStyle(Color fontColor, FontStyle fontStyle = FontStyle.Normal)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = fontColor;
        style.fontStyle = fontStyle;
        return style;
    }
    public static GUIStyle TextStyle(Color fontColor, int fontSize, FontStyle fontStyle = FontStyle.Normal)
    {
        GUIStyle style = TextStyle(fontColor, fontStyle);
        style.fontSize = fontSize;
        return style;
    }
    public static GUIStyle TextStyle(TextAnchor alignment, Color fontColor, int fontSize, FontStyle fontStyle = FontStyle.Normal)
    {
        GUIStyle style = TextStyle(fontColor, fontSize, fontStyle);
        style.alignment = alignment;
        return style;
    }

    public static GUIStyle ButtonStyle(Color textColor, FontStyle fontStyle = FontStyle.Normal)
    {
        GUIStyle btn_style = new GUIStyle();
        btn_style.fontStyle = fontStyle;
        btn_style.normal.textColor = textColor;

        return btn_style;
    }
}
