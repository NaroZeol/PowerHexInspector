using Microsoft.PowerToys.Settings.UI.Library;

namespace PowerHexInspector
{
    public class SettingsHelper
    {
        public bool SplitBinary;
        public Endian InputEndian;
        public Endian OutputEndian;
        public BitLength BitLength;
        
        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            var _splitBinary = true;
            var _inputEndian = Endian.LittleEndian;
            var _outputEndian = Endian.BigEndian;
            var _bitLength = BitLength.QWORD;

            if (settings != null && settings.AdditionalOptions != null)
            {
                var optionSplitBin = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "SplitBinary");
                _splitBinary = optionSplitBin?.Value ?? SplitBinary;

                var optionInputEndian = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "InputEndian");
                _inputEndian = (Endian)optionInputEndian.ComboBoxValue;

                var optionOutputEndian = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "OutputEndian");
                _outputEndian = (Endian)optionOutputEndian.ComboBoxValue;

                var optionBitLength = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "BitLength");
                _bitLength = (BitLength)optionBitLength.ComboBoxValue;
            }

            SplitBinary = _splitBinary;
            InputEndian = _inputEndian;
            OutputEndian = _outputEndian;
            BitLength = _bitLength;
            return;
        }
    }
}