using ERBingoRandomizer.Utility;
using SoulsFormats.AC4;
using System.IO;
using System.Threading;
using System.Windows;

namespace ERBingoRandomizer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        static string BND0Path = @"G:\Steam\steamapps\common\ELDEN RING\Game\Data0.bhd";
        static string BND1Path = @"G:\Steam\steamapps\common\ELDEN RING\Game\Data1.bhd";
        static string BND2Path = @"G:\Steam\steamapps\common\ELDEN RING\Game\Data2.bhd";
        static string BND3Path = @"G:\Steam\steamapps\common\ELDEN RING\Game\Data3.bhd";
        public MainWindow() {
            // File.WriteAllBytes($"{BND0Path}.bak", File.ReadAllBytes(BND0Path));
            // File.WriteAllBytes($"{BND1Path}.bak", File.ReadAllBytes(BND1Path));
            // File.WriteAllBytes($"{BND2Path}.bak", File.ReadAllBytes(BND2Path));
            // File.WriteAllBytes($"{BND3Path}.bak", File.ReadAllBytes(BND3Path));
            // File.WriteAllBytes(BND0Path, CryptoUtil.DecryptRsa(BND0Path, Const.ArchiveKeys.DATA0, CancellationToken.None).ToArray());
            // File.WriteAllBytes(BND1Path, CryptoUtil.DecryptRsa(BND1Path, Const.ArchiveKeys.DATA1, CancellationToken.None).ToArray());
            // File.WriteAllBytes(BND2Path, CryptoUtil.DecryptRsa(BND2Path, Const.ArchiveKeys.DATA2, CancellationToken.None).ToArray());
            // File.WriteAllBytes(BND3Path, CryptoUtil.DecryptRsa(BND3Path, Const.ArchiveKeys.DATA3, CancellationToken.None).ToArray());

            InitializeComponent();
        }
    }
}
