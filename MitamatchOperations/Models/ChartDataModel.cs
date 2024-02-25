using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mitama.Domain;
using static Google.Apis.Bigquery.v2.Data.TableDataInsertAllRequest;

namespace Mitama.Models;

public class DataModel(int xValue, int yValue)
{
    public int Status { get; set; } = xValue;

    public int Time { get; set; } = yValue;
}

public class ChartViewModel(SortedDictionary<TimeOnly, AllStatus> raw)
{
    private readonly SortedDictionary<TimeOnly, AllStatus> RawData = raw;
    public ObservableCollection<DataModel> Data { get; set; } = [.. raw.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.Attack))];

    public void SwithcTo(string target)
    {
        switch (target)
        {
            case "ATK":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.Attack))];
                break;
            case "DEF":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.Defense))];
                break;
            case "Sp.ATK":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.SpecialAttack))];
                break;
            case "Sp.DEF":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.SpecialDefense))];
                break;
            case "Fire ATK":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.FireAttack))];
                break;
            case "Fire DEF":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.FireDefense))];
                break;
            case "Water ATK":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.WaterAttack))];
                break;
            case "Water DEF":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.WaterDefense))];
                break;
            case "Wind ATK":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.WindAttack))];
                break;
            case "Wind DEF":
                Data = [.. RawData.Select(item => new DataModel(item.Key.Minute * 60 + item.Key.Second, item.Value.WindDefense))];
                break;
            default: break;
        }
    }
}
