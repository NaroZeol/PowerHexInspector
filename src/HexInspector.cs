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

        private List<Result> ProduceResults(Query query)
        {
            var results = new List<Result>();
            var conversions = new List<(ConvertResult, Base)>();
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

            QueryInterpretHelper.QueryInterpret(query, out Base queryBase, out string queryValue, out bool isUpper);
            if (queryBase == Base.Invalid)
            {
                return results;
            }

            converter.is_upper = isUpper;
            conversions.Add((converter.UniversalConvert(queryValue, queryBase, Base.Hex), Base.Hex));
            conversions.Add((converter.UniversalConvert(queryValue, queryBase, Base.Oct), Base.Oct));
            conversions.Add((converter.UniversalConvert(queryValue, queryBase, Base.Dec), Base.Dec));
            conversions.Add((converter.UniversalConvert(queryValue, queryBase, Base.Bin), Base.Bin));

            // Create result list
            foreach ((ConvertResult res, Base type) in conversions)
            {
                results.Add
                (
                    new Result
                    {
                        Title = res.Formated,
                        SubTitle = $"{type.ToString().ToUpper()} "
                                 + $"({settings.BitLength}{(type == Base.Bin || type == Base.Hex ? $" {settings.OutputEndian}" : "")})",
                        IcoPath = IconPath,
                        Action = (e) =>
                        {
                            UtilsFunc.SetClipboardText(res.Raw);
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

        public Control CreateSettingPanel()
        {
            throw new NotImplementedException();
        }

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
                DisplayDescription = "Little or Big Endian setting for input, only for binary and hexacecimal",
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
                DisplayDescription = "Little or Big Endian setting for output, only for binary and hexacecimal",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Combobox,
                ComboBoxValue = (int)Endian.LittleEndian,
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
                ComboBoxValue = (int)BitLength.QWORD,
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
    }
}