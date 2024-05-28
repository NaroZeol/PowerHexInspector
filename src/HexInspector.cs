using Wox.Plugin;
using Wox.Plugin.Logger;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Windows.Controls;

namespace PowerHexInspector
{
    public class HexInspector : IPlugin, IDisposable, ISettingProvider
    {
        #region IPlugin
        public string Name => "Hex Inspector";
        public string Description => "A simple powertoys run plugin provides fast and easy way to peek other forms of an input value";
        public static string PluginID => "JSAKDJKALSJDIWDI1872Hdhad139319A";

        private bool _splitBinary;

        private List<Result> ProduceResults (string queryStr) {
            var results = new List<Result>();
            char queryChar = queryStr[0];
            queryStr = queryStr.Substring(1);
            if (queryStr.Length == 0) {
                return results;
            }
            queryStr = queryStr.TrimStart();
            try {
                var conversions = new List<(Convert.ConvertResult, string)>();
                if (queryChar == 'h' || queryChar == 'H') {
                    conversions.Add((Convert.HexFormat(queryStr, queryChar == 'H'), "hex"));   // hex
                    conversions.Add((Convert.Hex2Dec(queryStr), "dec"));
                    conversions.Add((Convert.Hex2Bin(queryStr, _splitBinary), "bin"));
                }
                else if (queryChar == 'b' || queryChar == 'B') {
                    conversions.Add((Convert.Bin2Hex(queryStr, queryChar == 'B'), "hex"));
                    conversions.Add((Convert.Bin2Dec(queryStr), "dec"));
                    conversions.Add((Convert.BinFormat(queryStr, _splitBinary), "bin"));   // bin
                }
                else if (queryChar == 'd' || queryChar == 'D') {
                    conversions.Add((Convert.Dec2Hex(queryStr, queryChar == 'D'), "hex"));
                    conversions.Add((Convert.DecFormat(queryStr), "dec"));   // dec
                    conversions.Add((Convert.Dec2Bin(queryStr, _splitBinary), "bin"));
                }

                foreach ((Convert.ConvertResult res, string type) in conversions)
                {
                    results.Add
                    (
                        new Result
                        {
                            Title = res.Format,
                            SubTitle = type,
                            IcoPath = $"Images\\{type}.png",
                            Action = (e) => {
                                Utils.UtilsFunc.SetClipboardText(res.Raw);
                                return true;
                            }
                        }
                    );
                }
            }
            catch (Exception) {
                return []; // empty results
            }
            return results;
        }
        public List<Result> Query(Query query) {
            var results = new List<Result>();
            string queryStr = query.Search;

            if (queryStr.Length == 0) {
                return results; // empty query
            }

            try {
                results = ProduceResults(queryStr);
            }
            catch (Exception) {
                return []; // empty results
            }

            return results;
        }
        public void Init (PluginInitContext context) {
            Log.Info("Hex Inspector plugin is initialized", typeof(HexInspector));
            return;
        }
        #endregion

        #region IDisposable
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region ISettingProvider
        public Control CreateSettingPanel(){throw new NotImplementedException();}
        public IEnumerable<PluginAdditionalOption> AdditionalOptions { get; } = new List<PluginAdditionalOption>()
        {
            new PluginAdditionalOption {
                Key = "SplitBinary",
                DisplayLabel = "Split Binary",
                DisplayDescription = "Split binary into 4-bit groups",
                Value = true,
            }
            // new PluginAdditionalOption {
            //     Key = "HexadecimalFormat",
            //     DisplayLabel = "Hexadecimal Format",
            //     DisplayDescription = "Choose the format of hexadecimal output(True: Upper, False: Lower)",
            //     Value = true,
            // }
        };
        public void UpdateSettings(PowerLauncherPluginSettings settings) {
            var SplitBinary = true;

            if (settings != null && settings.AdditionalOptions != null) {
                var optionSplitBin = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "SplitBinary");
                SplitBinary = optionSplitBin?.Value ?? SplitBinary;
            }

            _splitBinary = SplitBinary;

            return;
        }
        #endregion
    }
}