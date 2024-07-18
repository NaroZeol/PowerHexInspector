using Wox.Plugin;
using Wox.Plugin.Logger;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Windows.Controls;
using ManagedCommon;

namespace PowerHexInspector
{
    public class HexInspector : IPlugin, IDisposable, ISettingProvider
    {
        public string Name => "HexInspector";
        public string Description => "A simple powertoys run plugin provides fast and easy way to peek other forms of an input value";
        public static string PluginID => "JSAKDJKALSJDIWDI1872Hdhad139319A";
        private string IconPath { get; set; }
        private PluginInitContext Context { get; set; }
        private bool _disposed;
        private readonly SettingsHelper settings;
        private readonly Convert converter;
        
        public HexInspector()
        {
            settings = new SettingsHelper();
            converter = new Convert(settings);
        }

        #region IPlugin
        private List<Result> ProduceResults(Query query)
        {
            var results = new List<Result>();
            var conversions = new List<(ConvertResult, string)>();
            bool isKeywordSearch = !string.IsNullOrEmpty(query.ActionKeyword);
            bool isEmptySearch = string.IsNullOrEmpty(query.Search);

            if (isEmptySearch && isKeywordSearch)
            {
                results.Add
                (
                    new Result
                    {
                        Title = $"Usage 1: {query.ActionKeyword} [value]",
                        SubTitle = "[value]: A C-style number, e.g. 123, 0x7b, 0b01111011, 0173",
                        IcoPath = IconPath,
                        Action = (e) => true
                    }
                );
                results.Add
                (
                    new Result
                    {
                        Title = $"Usage 2: {query.ActionKeyword} [format] [value]",
                        SubTitle = "[format]: h/H for hex, b/B for binary, d/D for decimal",
                        IcoPath = IconPath,
                        Action = (e) => true
                    }
                );
                return results;
            }

            QueryInterpretHelper.QueryInterpret(query, out string queryFormat, out string queryValue);

            if (queryFormat == "h" || queryFormat == "H")
            {
                converter.is_upper = queryFormat == "H";
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Hex, BaseType.Hex), "HEX"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Hex, BaseType.Oct), "OCT"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Hex, BaseType.Dec), "DEC"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Hex, BaseType.Bin), "BIN"));
            }
            else if (queryFormat == "b" || queryFormat == "B")
            {
                converter.is_upper = queryFormat == "B";
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Bin, BaseType.Hex), "HEX"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Bin, BaseType.Oct), "OCT"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Bin, BaseType.Dec), "DEC"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Bin, BaseType.Bin), "BIN"));
            }
            else if (queryFormat == "d" || queryFormat == "D")
            {
                converter.is_upper = queryFormat == "D";
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Dec, BaseType.Hex), "HEX"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Dec, BaseType.Oct), "OCT"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Dec, BaseType.Dec), "DEC"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Dec, BaseType.Bin), "BIN"));
            }
            else if (queryFormat == "o" || queryFormat == "O")
            {
                converter.is_upper = queryFormat == "O";
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Oct, BaseType.Hex), "HEX"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Oct, BaseType.Oct), "OCT"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Oct, BaseType.Dec), "DEC"));
                conversions.Add((converter.UniversalConvert(queryValue, BaseType.Oct, BaseType.Bin), "BIN"));
            }
            else
            {
                return results; // empty query
            }

            // Create result list
            string SubTitleAddition =
            $" ({settings.BitLength switch
            {
                8 => "BYTE",
                16 => "WORD",
                32 => "DWORD",
                64 => "QWORD",
                _ => "BYTE"
            }},{(settings.OutputEndian ? "Little Endian" : "Big Endian")})";
            foreach ((ConvertResult res, string type) in conversions)
            {
                results.Add
                (
                    new Result
                    {
                        Title = res.Formated,
                        SubTitle = type + SubTitleAddition,
                        IcoPath = IconPath,
                        Action = (e) =>
                        {
                            Utils.UtilsFunc.SetClipboardText(res.Raw);
                            return true;
                        }
                    }
                );
            }
            return results;
        }
        
        public List<Result> Query(Query query)
        {
            var results = new List<Result>();

            // if (queryStr.Length == 0)
            // {
            //     return results; // empty query
            // }

            try
            {
                results = ProduceResults(query);
            }
            catch (Exception e)
            {
                // Return Error message
                return new List<Result> {
                    new Result
                    {
                        Title = "Unhandled Exception",
                        SubTitle = e.Message,
                        IcoPath = IconPath,
                        Action = (e) => true
                    }
                };
            }

            return results;
        }
        public void Init(PluginInitContext context)
        {
            Log.Info("Hex Inspector plugin is initializeing", typeof(HexInspector));
            Context = context ?? throw new ArgumentNullException(paramName: nameof(context));

            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
            Log.Info("Hex Inspector plugin is initialized", typeof(HexInspector));
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
                DisplayDescription = "Select the bit length for the output",
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