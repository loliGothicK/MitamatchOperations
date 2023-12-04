using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic.FileIO;
using mitama.Domain;
using mitama.Pages.Common;
using Windows.Storage;
using Windows.Storage.Pickers;
using static mitama.Pages.RegionConsole.BttaleLogParser;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.RegionConsole
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResultInput : Page
    {
        private StorageFile log;
        private SortedDictionary<uint, BattleLogItem> battleLogMap = [];

        public ResultInput()
        {
            InitializeComponent();
        }

        private async void PickOpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a file picker
            var openPicker = new FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = WindowHelper.GetWindowForElement(this);
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".csv");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                log = file;
                GeneralInfoBar.Title = $"{file.Name} を読み込みました";
                GeneralInfoBar.Severity = InfoBarSeverity.Success;
                GeneralInfoBar.IsOpen = true;
            }
        }

        private async void Analyse(object _, RoutedEventArgs _e)
        {
            if (AllyRegionName.Text == string.Empty
             || AllyPlayer1.Text == string.Empty
             || AllyPlayer2.Text == string.Empty
             || AllyPlayer3.Text == string.Empty
             || AllyPlayer4.Text == string.Empty
             || AllyPlayer5.Text == string.Empty
             || AllyPlayer6.Text == string.Empty
             || AllyPlayer7.Text == string.Empty
             || AllyPlayer8.Text == string.Empty
             || AllyPlayer9.Text == string.Empty
             || OpponentRegionName.Text == string.Empty
             || OpponentPlayer1.Text == string.Empty
             || OpponentPlayer2.Text == string.Empty
             || OpponentPlayer3.Text == string.Empty
             || OpponentPlayer4.Text == string.Empty
             || OpponentPlayer5.Text == string.Empty
             || OpponentPlayer6.Text == string.Empty
             || OpponentPlayer7.Text == string.Empty
             || OpponentPlayer8.Text == string.Empty
             || OpponentPlayer9.Text == string.Empty)
            {
                GeneralInfoBar.Title = "全てのプレイヤー名を入力してください";
                GeneralInfoBar.Severity = InfoBarSeverity.Error;
                GeneralInfoBar.IsOpen = true;
                await Task.Delay(3000);
                GeneralInfoBar.IsOpen = false;
                return;
            }
            if (log is null)
            {
                GeneralInfoBar.Title = "ログファイルを選択してください";
                GeneralInfoBar.Severity = InfoBarSeverity.Error;
                GeneralInfoBar.IsOpen = true;
                await Task.Delay(3000);
                GeneralInfoBar.IsOpen = false;
                return;
            }

            var hints = new Hints(
                new RegionHint(AllyRegionName.Text, [
                    AllyPlayer1.Text,
                    AllyPlayer2.Text,
                    AllyPlayer3.Text,
                    AllyPlayer4.Text,
                    AllyPlayer5.Text,
                    AllyPlayer6.Text,
                    AllyPlayer7.Text,
                    AllyPlayer8.Text,
                    AllyPlayer9.Text,
                ]),
                new RegionHint(OpponentRegionName.Text, [
                    OpponentPlayer1.Text,
                    OpponentPlayer2.Text,
                    OpponentPlayer3.Text,
                    OpponentPlayer4.Text,
                    OpponentPlayer5.Text,
                    OpponentPlayer6.Text,
                    OpponentPlayer7.Text,
                    OpponentPlayer8.Text,
                    OpponentPlayer9.Text,
                ])
            );

            using TextFieldParser parser = new(log.Path);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                var blockNumber = uint.Parse(fields[1]);
                if (battleLogMap.TryGetValue(blockNumber, out BattleLogItem value))
                {
                    var parsed = ParseFragment(fields);
                    if (parsed.HasValue)
                    {
                        value.Fragments.Add(parsed.Value);
                    }
                }
                else
                {
                    var parsed = ParseAll(fields, hints);
                    if (parsed.HasValue)
                    {
                        battleLogMap.Add(blockNumber, parsed.Value);
                    }
                }
            }
            var logDir = @$"{Director.ProjectDir()}\{Director.ReadCache().Region}\BattleLog";
            if (!Directory.Exists(logDir))
            {
                Director.CreateDirectory(logDir);
            }
            var path = @$"{logDir}\{DateTime.Now:yyyy-MM-dd}";
            if (!Directory.Exists(path))
            {
                Director.CreateDirectory(path);
            }
            
            BattleLog battleLog = new([.. battleLogMap.Values]);
            await JsonSerializer.SerializeAsync(new FileStream($@"{path}/all.json", FileMode.Create), battleLog);

            var (_, opponents) = battleLog.ExtractPlayers();

            await SaveUnits(logDir, battleLog, opponents);

            GeneralInfoBar.Title = $"解析が完了しました。";
            GeneralInfoBar.Severity = InfoBarSeverity.Success;
            GeneralInfoBar.IsOpen = true;
            await Task.Delay(3000);
            GeneralInfoBar.IsOpen = false;
        }

        private async Task SaveUnits(string logDir, BattleLog battleLog, Player[] players)
        {
            foreach (var player in players)
            {
                var path = $@"{logDir}\{DateTime.Now:yyyy-MM-dd}\{player.Region}\「{player.Name}」";
                Director.CreateDirectory(path);
                var units = await battleLog.ExtractUnits(player.Name);
                foreach (var (unit, index) in units.Select((unit, index) => (unit, index)))
                {
                    using var unitFile = File.Create($@"{path}\Unit-{index+1}.json");
                    await unitFile.WriteAsync(new UTF8Encoding(true).GetBytes(unit.ToJson()));
                }
            }
        }
    }
}
