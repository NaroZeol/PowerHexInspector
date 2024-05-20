using Wox.Plugin;
using ManagedCommon;
using Wox.Plugin.Logger;
using Wox.Infrastructure;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Windows.Controls;

namespace PowerHexInspector
{
    public class HexInspector : IPlugin, IDisposable, IDelayedExecutionPlugin, ISettingProvider, IContextMenu, IReloadable
    {
        #region IPlugin
        public string Name => "Hex Inspector";
        public string Description => "A simple powertoys run plugin provides fast and easy way to peek other forms of an input value";
        
        public List<Result> Query(Query query) {throw new NotImplementedException();}  //  TODO
        public void Init (PluginInitContext context) {throw new NotImplementedException();}     //  TODO
        #endregion

        #region IDisposable
        public void Dispose() {throw new NotImplementedException();}//  TODO
        #endregion

        #region IDelayedExecutionPlugin
        public List<Result> Query(Query query, bool delayedExecution) {throw new NotImplementedException();}  //  TODO
        #endregion

        #region ISettingProvider
        public IEnumerable<PluginAdditionalOption> AdditionalOptions { get; } = []; // TODO
        public Control CreateSettingPanel(){throw new NotImplementedException();} // TODO
        public void UpdateSettings(PowerLauncherPluginSettings settings) {throw new NotImplementedException();} // TODO
        #endregion

        #region IContextMenu
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult) {throw new NotImplementedException();} // TODO
        #endregion

        #region IReloadable
        public void ReloadData() {throw new NotImplementedException();} // TODO
        #endregion
    }
}