﻿using System.Collections.ObjectModel;
using Syncfusion.UI.Xaml.Core;

namespace Mitama.Models;

public class CheckBoxModel : NotificationObject
{
    private ObservableCollection<CheckBoxModel> models;

    private string state;

    public CheckBoxModel()
    {
        Models = new ObservableCollection<CheckBoxModel>();
    }

    public ObservableCollection<CheckBoxModel> Models
    {
        get
        {
            return models;
        }

        set
        {
            models = value;
            this.RaisePropertyChanged(nameof(Models));
        }
    }

    public string State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
            this.RaisePropertyChanged(nameof(State));
        }
    }
}
