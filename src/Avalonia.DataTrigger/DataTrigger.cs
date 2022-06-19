using System.Diagnostics;
using System.Reactive.Linq;
using Avalonia.Collections;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.DataTrigger;

/// <summary>
/// A behavior that performs actions when the bound data meets a specified condition.
/// </summary>
public sealed class DataTrigger : Trigger
{
    /// <summary>
    /// Identifies the <seealso cref="Binding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> BindingProperty =
        AvaloniaProperty.Register<DataTrigger, object?>(nameof(Binding));

    /// <summary>
    /// Gets or sets the bound object that the <see cref="DataTrigger"/> will listen to. This is a avalonia property.
    /// </summary>
    public object? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <summary>
    /// Identifies the <seealso cref="Default"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> DefaultProperty =
        AvaloniaProperty.Register<DataTrigger, object?>(nameof(Default));

    /// <summary>
    /// Gets or sets the bound object that the <see cref="DataTrigger"/> will listen to. This is a avalonia property.
    /// </summary>
    public object? Default
    {
        get => GetValue(DefaultProperty);
        set => SetValue(DefaultProperty, value);
    }
        
    /// <summary>
    /// Identifies the <seealso cref="Property"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyProperty =
        AvaloniaProperty.Register<DataTrigger, string?>(nameof(Property));
        
    /// <summary>
    /// Gets or sets the name of the property to change. This is a avalonia property.
    /// </summary>
    public string? Property
    {
        get => GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }

    static DataTrigger() => Observable.Merge<AvaloniaPropertyChangedEventArgs>(
            BindingProperty.Changed, PropertyProperty.Changed, DefaultProperty.Changed)
        .AddClassHandler<DataTrigger>(OnValueChanged);

    public DataTrigger() => Actions?.GetWeakCollectionChangedObservable().Subscribe(_ => OnValueChanged(this, null));

    protected override void OnAttached() => OnValueChanged(this, null);

    public static void OnValueChanged(IAvaloniaObject? avaloniaObject, AvaloniaPropertyChangedEventArgs? args)
    {
        if (avaloniaObject is not DataTrigger
        {
            AssociatedObject: { } obj,
            Actions: { } actions,
            Binding: { } binding
        } behavior || actions.Count == 0)
        {
            return;
        }


        var anyWorked = false;
        foreach (var x in actions.OfType<Set>())
        {
            x.Parent = behavior;
            if (BindingValueComparer.Compare(binding, x.When))
            {
                if (x.Execute(obj, args) is true)
                {
                    anyWorked = true;
                }
                else
                {
                    Debugger.Break();//wtf?
                }
            }
        }

        if (!anyWorked &&
            behavior.Property is {} property && 
            behavior.Default is {} def &&
            AvaloniaPropertyRegistry.Instance.FindRegistered(obj, property) is {} prop)
        {
            obj.SetValue(prop, Set.CastValue(def, prop.PropertyType));
        }
    }
}
