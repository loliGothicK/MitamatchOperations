using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using mitama.Models;
using Syncfusion.UI.Xaml.Core;

namespace mitama.Pages.DeckBuilder;

public class CheckBoxView : NotificationObject
{
    public ObservableCollection<object> CheckedItems { get; set; }

    public ObservableCollection<CheckBoxModel> Items { get; set; }

    public CheckBoxView(Dictionary<string, string[]> config)
    {
        Items = [];

        foreach (var (state, options) in config)
        {
            var category = new CheckBoxModel { State = state };
            foreach (var option in options)
            {
                category.Models.Add(new CheckBoxModel { State = option });
            }
            Items.Add(category);
        }

        CheckedItems = [.. Items.SelectMany(item => item.Models)];
    }

    public CheckBoxView(Dictionary<string, Dictionary<string, string[]>> config)
    {
        Items = [];

        foreach (var (topState, inner) in config)
        {
            var outerCategory = new CheckBoxModel { State = topState };
            foreach (var (innerState, labels) in inner)
            {
                var innerCategory = new CheckBoxModel { State = innerState };
                foreach (var label in labels)
                {
                    innerCategory.Models.Add(new CheckBoxModel { State = label });
                }
                outerCategory.Models.Add(innerCategory);
            }
            Items.Add(outerCategory);
        }
    }
}

public class NullableTreeCheckbox : DependencyObject
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(NullableTreeCheckbox), new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.RegisterAttached("IsChecked", typeof(object),
        typeof(NullableTreeCheckbox), new PropertyMetadata(default(object), IsCheckedChanged));

    private static readonly DependencyProperty IsInternalCheckedProperty =
        DependencyProperty.RegisterAttached("IsInternalChecked", typeof(object),
        typeof(NullableTreeCheckbox), new PropertyMetadata(null, OnIsInternalCheckedChanged));

    public static bool GetIsEnabled(DependencyObject obj)
    {
        if (obj == null)
            return false;

        return (bool)obj.GetValue(IsEnabledProperty);
    }

    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
        if (obj == null)
            return;
        obj.SetValue(IsEnabledProperty, value);
    }

    public static object GetIsChecked(DependencyObject obj)
    {
        if (obj == null)
            return false;

        return (bool?)obj.GetValue(IsCheckedProperty);
    }

    public static void SetIsChecked(DependencyObject obj, object value)
    {
        if (obj == null)
            return;

        obj.SetValue(IsCheckedProperty, value);
    }

    private static void OnIsInternalCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        SetIsChecked(d, (bool?)e.NewValue);
    }

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var checkbox = d as Microsoft.UI.Xaml.Controls.CheckBox;
        if ((bool)e.NewValue)
        {
            var binding = new Binding
            {
                Path = new PropertyPath("IsChecked"),
                Mode = BindingMode.TwoWay,
                Source = checkbox,
            };
            checkbox.SetBinding(NullableTreeCheckbox.IsInternalCheckedProperty, binding);
        }
    }

    private static void IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var checkbox = d as Microsoft.UI.Xaml.Controls.CheckBox;
        bool? newValue = null;
        if (e.NewValue is bool?)
            newValue = (bool?)e.NewValue;
        else if (e.NewValue != null)
            newValue = (bool)e.NewValue;
        if (!checkbox.IsChecked.Equals(newValue))
            checkbox.IsChecked = newValue;
    }
}
