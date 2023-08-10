using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ERBingoRandomizer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            // string[] entries = File.ReadAllLines(@"C:\Users\Nord\Downloads\er_items.txt");
            // List<int> itemLotParam_map = new();
            // List<int> itemLotParam_enemy = new();
            // foreach (string entry in entries) {
            //     string[] parts = entry.Split(" ");
            //     if (parts.Length < 3) {
            //         continue;
            //     }
            //
            //     string tag = parts[2].ToLower();
            //     if (!tag.Contains("unknown") && !tag.Contains("unused")) {
            //         continue;
            //     }
            //     
            //     switch (parts[0]) {
            //         case "ItemLotParam_map":
            //             itemLotParam_map.Add(int.Parse(parts[1]));
            //             continue;
            //         case "ItemLotParam_enemy":
            //             itemLotParam_enemy.Add(int.Parse(parts[1]));
            //             continue;
            //     }
            // }
            //
            // PrintUnused(itemLotParam_map, itemLotParam_enemy);
            InitializeComponent();
        }
        public static void PrintUnused(IEnumerable<int> itemLotParamMap, IEnumerable<int> itemLotParamEnemy) {
            StringBuilder sb = new();

            sb.AppendLine("int[] itemLotParamMap = {");
            foreach (int id in itemLotParamMap) {
                sb.AppendLine($"\t{id},");
            }
            sb.AppendLine("};");
            sb.AppendLine("int[] itemLotParamEnemy = {");
            foreach (int id in itemLotParamEnemy) {
                sb.AppendLine($"\t{id},");
            }
            sb.AppendLine("};");
            
            File.WriteAllText(@"C:\Users\Nord\source\CSharp\ERBingoRandomizer\src\ERBingoRandomizer\Randomizer\Unk2.cs", sb.ToString());
        }
    }
}
