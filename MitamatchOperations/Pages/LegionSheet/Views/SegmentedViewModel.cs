using System.Collections.Generic;
using System.ComponentModel;

namespace mitama.Pages.LegionSheet.Views;

public class SegmentedViewModel
{
    public SegmentedViewModel()
    {
        Items = [];
        Items.Add(new() { Name = "オーダー" });
        Items.Add(new() { Name = "衣装" });
        Items.Add(new() { Name = "メモリア" });
    }

    public List<SegmentedModel> Items { get; set; }
}

public class SegmentedModel : INotifyPropertyChanged
{
    private string name;

    public string Name
    {
        get { return name; }
        set { name = value; OnPropertyChanged("Name"); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string parameter)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parameter));
    }
}
