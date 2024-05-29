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
        private bool _splitBinary;
        private bool _inputEndian;
        private bool _outputEndian;
        private static readonly bool LittleEndian = true;
        private static readonly bool BigEndian = false;
        private bool _disposed;

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
            queryStr = queryStr.TrimStart();
            try
            {
                var conversions = new List<(Convert.ConvertResult, string)>();
                if (queryChar == 'h' || queryChar == 'H')
                {
                    conversions.Add((Convert.HexFormat(queryStr, queryChar == 'H', _inputEndian, _outputEndian), "HEX"));   // hex
                    conversions.Add((Convert.Hex2Dec(queryStr, _inputEndian, _outputEndian), "DEC"));
                    conversions.Add((Convert.Hex2Bin(queryStr, _splitBinary, _inputEndian, _outputEndian), "BIN"));
                }
                else if (queryChar == 'b' || queryChar == 'B')
                {
                    conversions.Add((Convert.Bin2Hex(queryStr, queryChar == 'B', _inputEndian, _outputEndian), "HEX"));
                    conversions.Add((Convert.Bin2Dec(queryStr, _inputEndian, _outputEndian), "DEC"));
                    conversions.Add((Convert.BinFormat(queryStr, _splitBinary, _inputEndian, _outputEndian), "BIN"));   // bin
                }
                else if (queryChar == 'd' || queryChar == 'D')
                {
                    conversions.Add((Convert.Dec2Hex(queryStr, queryChar == 'D', _inputEndian, _outputEndian), "HEX"));
                    conversions.Add((Convert.DecFormat(queryStr), "DEC"));   // dec
                    conversions.Add((Convert.Dec2Bin(queryStr, _splitBinary, _inputEndian, _outputEndian), "BIN"));
                }

                foreach ((Convert.ConvertResult res, string type) in conversions)
                {
                    results.Add
                    (
                        new Result
                        {
                            Title = res.Format,
                            SubTitle = type + (_outputEndian ? " (Little" : " (Big") + " Endian)",
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
                ComboBoxItems = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Little Endian", "0"),
                    new KeyValuePair<string, string>("Big Endian", "1"),
                }
            },
            new PluginAdditionalOption {
                Key = "OutputEndian",
                DisplayLabel = "Output Endian",
                DisplayDescription = "Little or Big Endian setting for output",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Combobox,
                ComboBoxValue = 0,
                ComboBoxItems = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Little Endian", "0"),
                    new KeyValuePair<string, string>("Big Endian", "1"),
                }
            }
        };
        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            var SplitBinary = true;
            var InputEndian = LittleEndian;
            var OutputEndian = LittleEndian;

            if (settings != null && settings.AdditionalOptions != null)
            {
                var optionSplitBin = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "SplitBinary");
                SplitBinary = optionSplitBin?.Value ?? SplitBinary;

                var optionAffectInput = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "InputEndian");
                InputEndian = optionAffectInput.ComboBoxValue == 0 ? LittleEndian : BigEndian;

                var optionEndian = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "OutputEndian");
                OutputEndian = optionEndian.ComboBoxValue == 0 ? LittleEndian : BigEndian;
            }

            _splitBinary = SplitBinary;
            _inputEndian = InputEndian;
            _outputEndian = OutputEndian;
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