using Microsoft.PowerToys.Settings.UI.Library;

namespace PowerHexInspector
{
    public class SettingsHelper
    {
        public bool SplitBinary;
        public bool InputEndian;
        public bool OutputEndian;
        public int BitLength;
        public static readonly bool LittleEndian = true;
        public static readonly bool BigEndian = false;

        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            var _splitBinary = true;
            var _inputEndian = LittleEndian;
            var _outputEndian = LittleEndian;
            var _bitLength = 64;

            if (settings != null && settings.AdditionalOptions != null)
            {
                var optionSplitBin = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "SplitBinary");
                _splitBinary = optionSplitBin?.Value ?? SplitBinary;

                var optionAffectInput = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "InputEndian");
                _inputEndian = optionAffectInput.ComboBoxValue == 0 ? LittleEndian : BigEndian;

                var optionEndian = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "OutputEndian");
                _outputEndian = optionEndian.ComboBoxValue == 0 ? LittleEndian : BigEndian;

                var optionBitLength = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "BitLength");
                _bitLength = optionBitLength.ComboBoxValue;
            }

            SplitBinary = _splitBinary;
            InputEndian = _inputEndian;
            OutputEndian = _outputEndian;
            BitLength = _bitLength;
            return;
        }
    }
}