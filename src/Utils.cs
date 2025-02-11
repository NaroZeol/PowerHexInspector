using System.Windows;

namespace Community.PowerToys.Run.Plugin.HexInspector
{
    public static class UtilsFunc
    {
        public static void SetClipboardText(string s)
        {
            Clipboard.SetDataObject(s);
        }
    }
}