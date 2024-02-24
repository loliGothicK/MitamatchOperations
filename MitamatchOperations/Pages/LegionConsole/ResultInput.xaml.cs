using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic.FileIO;
using mitama.Domain;
using mitama.Pages.Common;
using MitamatchOperations.Lib;
using Windows.Storage;
using Windows.Storage.Pickers;
using static mitama.Pages.LegionConsole.BattleLogParser;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.LegionConsole
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResultInput : Page
    {
        private StorageFile log;
        private readonly SortedDictionary<uint, BattleLogItem> battleLogMap = [];
        private DateTime? _date = null;

        public ResultInput()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            var Legion = Director.ReadCache().Legion;
            AllyLegionName.Text = Legion;
            var players = Util.LoadMemberNames(Legion);
            List<TextBox> list =
            [
                AllyPlayer1,
                AllyPlayer2,
                AllyPlayer3,
                AllyPlayer4,
                AllyPlayer5,
                AllyPlayer6,
                AllyPlayer7,
                AllyPlayer8,
                AllyPlayer9,
            ];
            for (int i = 0; i < players.Take(9).Count(); i++)
            {
                list[i].Text = players[i];
            }
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
            if (log is null)
            {
                GeneralInfoBar.Title = "ログファイルを選択してください";
                GeneralInfoBar.Severity = InfoBarSeverity.Error;
                GeneralInfoBar.IsOpen = true;
                await Task.Delay(3000);
                GeneralInfoBar.IsOpen = false;
                return;
            }
            else if (AllyLegionName.Text == string.Empty || OpponentLegionName.Text == string.Empty)
            {
                GeneralInfoBar.Title = "レギオン名を入力してください";
                GeneralInfoBar.Severity = InfoBarSeverity.Error;
                GeneralInfoBar.IsOpen = true;
                await Task.Delay(3000);
                GeneralInfoBar.IsOpen = false;
                return;
            }
            else if (AllyPlayer1.Text == string.Empty
                  || AllyPlayer2.Text == string.Empty
                  || AllyPlayer3.Text == string.Empty
                  || AllyPlayer4.Text == string.Empty
                  || AllyPlayer5.Text == string.Empty
                  || AllyPlayer6.Text == string.Empty
                  || AllyPlayer7.Text == string.Empty
                  || AllyPlayer8.Text == string.Empty
                  || AllyPlayer9.Text == string.Empty
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
                var flag = false;
                var dialog = new DialogBuilder(XamlRoot)
                    .WithTitle("全てのプレイヤーが入力されていませんが解析しますか？")
                    .WithPrimary("解析", new Defer(delegate { flag = true; return Task.CompletedTask; }))
                    .WithCancel("キャンセル")
                    .Build();
                await dialog.ShowAsync();
                while (!flag)
                {
                    await Task.Delay(100);
                }
            }

            GeneralInfoBar.Title = $"解析中...";
            GeneralInfoBar.Severity = InfoBarSeverity.Informational;
            GeneralInfoBar.IsOpen = true;

            var hints = new Hints(
                new LegionHint(AllyLegionName.Text, [
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
                new LegionHint(OpponentLegionName.Text, [
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
            // Skip header
            if (!parser.EndOfData) parser.ReadLine();
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
            var logDir = @$"{Director.ProjectDir()}\{Director.ReadCache().Legion}\BattleLog";
            if (!Directory.Exists(logDir))
            {
                Director.CreateDirectory(logDir);
            }
            var path = @$"{logDir}\{_date ?? DateTime.Now:yyyy-MM-dd}";
            if (!Directory.Exists(path))
            {
                Director.CreateDirectory(path);
            }

            BattleLog battleLog = new([.. battleLogMap.Values]);
            await JsonSerializer.SerializeAsync(new FileStream($@"{path}/all.json", FileMode.Create), battleLog);

            var (allies, opponents) = battleLog.ExtractPlayers();

            await SaveUnits(logDir, battleLog, allies, "Ally");
            await SaveUnits(logDir, battleLog, opponents, "Opponent");
            await SaveStatusInfo(logDir, battleLog, allies, "Ally");
            await SaveStatusInfo(logDir, battleLog, opponents, "Opponent");
            await SaveSummary(logDir, battleLog);
            await SaveUnitChanges(logDir, battleLog);

            GeneralInfoBar.Title = $"解析が完了しました。";
            GeneralInfoBar.Severity = InfoBarSeverity.Success;
            await Task.Delay(3000);
            GeneralInfoBar.IsOpen = false;
        }

        private async Task SaveUnitChanges(string logDir, BattleLog battleLog)
        {
            var unitChanges = battleLog.ExtractUnitChanges();
            // output as json
            var path = $@"{logDir}\{_date ?? DateTime.Now:yyyy-MM-dd}\unitChanges.json";
            using var unitChangesFile = File.Create(path);
            await unitChangesFile.WriteAsync(new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(new
            {
                Data = unitChanges
            })));
        }

        private async Task SaveSummary(string logDir, BattleLog battleLog)
        {
            var AllyPoints = AllyLegionPoints.Text == string.Empty ? 0 : int.Parse(AllyLegionPoints.Text);
            var OpponentPoints = OpponentLegionPoints.Text == string.Empty ? 0 : int.Parse(OpponentLegionPoints.Text);
            var NeunWelt = NeunWeltResult.SelectedIndex == 0 ? "勝ち" : "負け";
            var (AllyOrders, OpponentOrders) = await battleLog.ExtractOrders();
            var (Allies, Opponents) = battleLog.ExtractPlayers();
            Comment.TextDocument.GetText(TextGetOptions.UseCrlf, out var comment);
            // output as json
            var path = $@"{logDir}\{_date ?? DateTime.Now:yyyy-MM-dd}\summary.json";
            using var unitFile = File.Create(path);
            await unitFile.WriteAsync(new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(new
            {
                Opponent = OpponentLegionName.Text,
                Comment = comment,
                AllyPoints,
                OpponentPoints,
                NeunWelt,
                Allies,
                Opponents,
                AllyOrders,
                OpponentOrders,
            })));
        }

        private async Task SaveUnits(string logDir, BattleLog battleLog, Player[] players, string v)
        {
            foreach (var player in players)
            {
                var path = $@"{logDir}\{_date ?? DateTime.Now:yyyy-MM-dd}\{v}\[{ToRemoveRegex().Replace(player.Name, string.Empty)}]\Units";
                Director.CreateDirectory(path);
                var units = await battleLog.ExtractUnits(player.Name);
                foreach (var (unit, index) in units.Select((unit, index) => (unit, index)))
                {
                    using var unitFile = File.Create($@"{path}\Unit-{index + 1}.json");
                    await unitFile.WriteAsync(new UTF8Encoding(true).GetBytes(unit.ToJson()));
                }
            }
        }

        private async Task SaveStatusInfo(string logDir, BattleLog battleLog, Player[] players, string v)
        {
            var data = battleLog.ExtractIncreaseDecrease();
            foreach (var player in players)
            {
#pragma warning disable CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
                if (!data.ContainsKey(player.Name) || data[player.Name].Length == 0) continue;
#pragma warning restore CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
                var path = $@"{logDir}\{_date ?? DateTime.Now:yyyy-MM-dd}\{v}\[{ToRemoveRegex().Replace(player.Name, string.Empty)}]";
                Director.CreateDirectory(path);
                using var playerFile = File.Create($@"{path}\status.json");
                bool isStandBy = false;
                SortedDictionary<TimeOnly, AllStatus> history = [];
                foreach (var (time, stat) in data[player.Name])
                {
                    isStandBy = Update(ref history, time, stat, isStandBy);
                }
                await playerFile.WriteAsync(new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(history)));
            }
        }

        private static bool Update(ref SortedDictionary<TimeOnly, AllStatus> history, TimeOnly time, StatusIncreaseDecrease stat, bool isStandBy)
        {
            var status = history.Keys.Count != 0 ? history.Last().Value : new AllStatus();
            switch (stat.Value)
            {
                case Attack atk:
                    status.Attack += atk.Value;
                    isStandBy = false;
                    break;
                case Defense def:
                    status.Defense += def.Value;
                    isStandBy = false;
                    break;
                case SpecialAttack spatk:
                    status.SpecialAttack += spatk.Value;
                    isStandBy = false;
                    break;
                case SpecialDefense spdef:
                    status.SpecialDefense += spdef.Value;
                    isStandBy = false;
                    break;
                case WaterAttack waterAttack:
                    status.WaterAttack += waterAttack.Value;
                    isStandBy = false;
                    break;
                case WaterDefense waterDefense:
                    status.WaterDefense += waterDefense.Value;
                    isStandBy = false;
                    break;
                case FireAttack fireAttack:
                    status.FireAttack += fireAttack.Value;
                    isStandBy = false;
                    break;
                case FireDefense fireDefense:
                    status.FireDefense += fireDefense.Value;
                    isStandBy = false;
                    break;
                case WindAttack windAttack:
                    status.WindAttack += windAttack.Value;
                    isStandBy = false;
                    break;
                case WindDefense windDefense:
                    status.WindDefense += windDefense.Value;
                    isStandBy = false;
                    break;
                case StandByPhase:
                    if (!isStandBy)
                    {
                        status.Attack = status.Attack > 0 ? status.Attack : 0;
                        status.Defense = status.Defense > 0 ? status.Defense : 0;
                        status.SpecialAttack = status.SpecialAttack > 0 ? status.SpecialAttack : 0;
                        status.SpecialDefense = status.SpecialDefense > 0 ? status.SpecialDefense : 0;
                        status.WaterAttack = status.WaterAttack > 0 ? status.WaterAttack : 0;
                        status.WaterDefense = status.WaterDefense > 0 ? status.WaterDefense : 0;
                        status.FireAttack = status.FireAttack > 0 ? status.FireAttack : 0;
                        status.FireDefense = status.FireDefense > 0 ? status.FireDefense : 0;
                        status.WindAttack = status.WindAttack > 0 ? status.WindAttack : 0;
                        status.WindDefense = status.WindDefense > 0 ? status.WindDefense : 0;
                    }
                    isStandBy = true;
                    break;
                default:
                    break;
            }
            if (history.ContainsKey(time))
            {
                history[time] = status;
            }
            else
            {
                history.Add(time, status);
            }
            return isStandBy;
        }

        private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            _date = sender.Date?.Date;
        }

        [GeneratedRegex("""/|:|\*|\?|"|<|>|\|""")]
        private static partial Regex ToRemoveRegex();

    }
}
