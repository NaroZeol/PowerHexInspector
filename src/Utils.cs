using System.Windows;

namespace PowerHexInspector.Utils
{
    public static class UtilsFunc
    {
        public static void SetClipboardText(string s)
        {
            Clipboard.SetDataObject(s);
        }
    }
}