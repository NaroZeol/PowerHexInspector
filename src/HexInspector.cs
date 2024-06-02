using Wox.Plugin;
using Wox.Plugin.Logger;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Windows.Controls;
using ManagedCommon;

namespace PowerHexInspector
{
    public class HexInspector : IPlugin, IDisposable, ISettingProvider
    {
        public string Name => "Hex Inspector";
        public string Description => "A simple powertoys run plugin provides fast and easy way to peek other forms of an input value";
        public static string PluginID => "JSAKDJKALSJDIWDI1872Hdhad139319A";
        private string IconPath { get; set; }
        private PluginInitContext Context { get; set; }
        private static readonly List<string> FilterStrs = [" ", "_", ",", "0x"];
        private bool _disposed;
        private readonly SettingsHelper settings;
        private readonly Convert converter;
        public HexInspector()
        {
            settings = new SettingsHelper();
            converter = new Convert(settings);
        }

        #region IPlugin
        private List<Result> ProduceResults(string queryStr)
        {
            var results = new List<Result>();
            char queryChar = queryStr[0];
            queryStr = queryStr.Substring(1);
            if (queryStr.Length == 0)
            {
                return results;
            }
            foreach (string filterStr in FilterStrs)
            {
                queryStr = queryStr.Replace(filterStr, "");
            }
            try
            {
                var conversions = new List<(Convert.ConvertResult, string)>();
                if (queryChar == 'h' || queryChar == 'H')
                {
                    bool is_upper = queryChar == 'H';
                    conversions.Add((converter.HexFormat(queryStr, is_upper), "HEX"));   // hex
                    conversions.Add((converter.Hex2Dec(queryStr), "DEC"));
                    conversions.Add((converter.Hex2Bin(queryStr), "BIN"));
                }
                else if (queryChar == 'b' || queryChar == 'B')
                {
                    bool is_upper = queryChar == 'B';
                    conversions.Add((converter.Bin2Hex(queryStr, is_upper), "HEX"));
                    conversions.Add((converter.Bin2Dec(queryStr), "DEC"));
                    conversions.Add((converter.BinFormat(queryStr), "BIN"));   // bin
                }
                else if (queryChar == 'd' || queryChar == 'D')
                {
                    bool is_upper = queryChar == 'D';
                    conversions.Add((converter.Dec2Hex(queryStr, is_upper), "HEX"));
                    conversions.Add((converter.DecFormat(queryStr), "DEC"));   // dec
                    conversions.Add((converter.Dec2Bin(queryStr), "BIN"));
                }

                foreach ((Convert.ConvertResult res, string type) in conversions)
                {
                    results.Add
                    (
                        new Result
                        {
                            Title = res.Format,
                            SubTitle = type + (settings.OutputEndian ? " (Little" : " (Big") + " Endian)",
                            IcoPath = IconPath,
                            Action = (e) =>
                            {
                                Utils.UtilsFunc.SetClipboardText(res.Raw);
                                return true;
                            }
                        }
                    );
                }
            }
            catch (Exception)
            {
                return []; // empty results
            }
            return results;
        }
        public List<Result> Query(Query query)
        {
            var results = new List<Result>();
            string queryStr = query.Search;

            if (queryStr.Length == 0)
            {
                return results; // empty query
            }

            try
            {
                results = ProduceResults(queryStr);
            }
            catch (Exception)
            {
                return []; // empty results
            }

            return results;
        }
        public void Init(PluginInitContext context)
        {
            Log.Info("Hex Inspector plugin is initialized", typeof(HexInspector));
            Context = context ?? throw new ArgumentNullException(paramName: nameof(context));

            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
        }
        #endregion

        #region ISettingProvider
        public Control CreateSettingPanel() { throw new NotImplementedException(); }
        public IEnumerable<PluginAdditionalOption> AdditionalOptions { get; } = new List<PluginAdditionalOption>()
        {
            new PluginAdditionalOption {
                Key = "SplitBinary",
                DisplayLabel = "Split Binary",
                DisplayDescription = "Split binary into 4-bit groups",
                Value = true,
            },
            new PluginAdditionalOption {
                Key = "InputEndian",
                DisplayLabel = "Input Endian",
                DisplayDescription = "Little or Big Endian setting for input",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Combobox,
                ComboBoxValue = 0,
                ComboBoxItems =
                [
                    new KeyValuePair<string, string>("Little Endian", "0"),
                    new KeyValuePair<string, string>("Big Endian", "1"),
                ]
            },
            new PluginAdditionalOption {
                Key = "OutputEndian",
                DisplayLabel = "Output Endian",
                DisplayDescription = "Little or Big Endian setting for output",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Combobox,
                ComboBoxValue = 0,
                ComboBoxItems =
                [
                    new KeyValuePair<string, string>("Little Endian", "0"),
                    new KeyValuePair<string, string>("Big Endian", "1"),
                ]
            },
            new PluginAdditionalOption {
                Key = "BitLength",
                DisplayLabel = "Bit Lengths",
                DisplayDescription = "",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Combobox,
                ComboBoxValue = 64,
                ComboBoxItems =
                [
                    new KeyValuePair<string, string>("BYTE", "8"),
                    new KeyValuePair<string, string>("WORD", "16"),
                    new KeyValuePair<string, string>("DWORD", "32"),
                    new KeyValuePair<string, string>("QWORD", "64"),
                ]
            }
        };
        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            this.settings.UpdateSettings(settings);
            return;
        }
        #endregion

        #region Icon
        private void OnThemeChanged(Theme currentTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private void UpdateIconPath(Theme theme)
        {
            if (theme == Theme.Light || theme == Theme.HighContrastWhite)
            {
                IconPath = "Images/HexInspector.light.png";
            }
            else
            {
                IconPath = "Images/HexInspector.dark.png";
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (Context != null && Context.API != null)
                    {
                        Context.API.ThemeChanged -= OnThemeChanged;
                    }

                    _disposed = true;
                }
            }
        }
        #endregion
    }
}