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
        private static List<Result> ProduceResults (string queryStr, string queryType) {
            List<Result> results = new List<Result>();
            if (queryStr.Length == 0) {
                return results;
            }
            queryStr = queryStr.TrimStart();
            try {
                List<(string, string)> conversions = new List<(string, string)>();
                if (queryType == "hex") {
                    conversions.Add((queryStr, queryType));   // hex
                    conversions.Add((Convert.Hex2Dec(queryStr), "dec"));
                    conversions.Add((Convert.Hex2Bin(queryStr), "bin"));
                }
                else if (queryType == "bin") {
                    conversions.Add((Convert.Bin2Hex(queryStr), "hex"));
                    conversions.Add((Convert.Bin2Dec(queryStr), "dec"));
                    conversions.Add((queryStr, queryType));   // bin
                }
                else if (queryType == "dec") {
                    conversions.Add((Convert.Dec2Hex(queryStr), "hex"));
                    conversions.Add((queryStr, queryType));   // dec
                    conversions.Add((Convert.Dec2Bin(queryStr), "bin"));
                }
                foreach ((string res, string type) in conversions) {
                    results.Add(
                        new Result {
                            Title = res,
                            SubTitle = type,
                            IcoPath = $"Images\\{type}.png",
                            Action = (e) => {
                                Utils.UtilsFunc.SetClipboardText(res);
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
            List<Result> results = new List<Result>();
            string queryStr = query.Search;

            if (queryStr.Length == 0) {
                return results; // empty query
            }

            try {
                if (queryStr[0] == 'b' || queryStr[0] == 'B') {
                    results = ProduceResults(queryStr.Substring(1), "bin");
                }
                else if (queryStr[0] == 'd' || queryStr[0] == 'D') {
                    results = ProduceResults(queryStr.Substring(1), "dec");
                }
                else if (queryStr[0] == 'h' || queryStr[0] == 'H') {
                    results = ProduceResults(queryStr.Substring(1), "hex");
                }
                else {
                    return results;
                }
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
            return; // Nothing need to dispose
        }
        #endregion

        #region ISettingProvider
        public IEnumerable<PluginAdditionalOption> AdditionalOptions { get; } = []; // TODO
        public Control CreateSettingPanel(){throw new NotImplementedException();} // TODO
        public void UpdateSettings(PowerLauncherPluginSettings settings) {
            return;
        }
        #endregion
    }
}