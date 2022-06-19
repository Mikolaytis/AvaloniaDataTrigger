using System.Globalization;
using System.Reflection;
using System.Reactive.Linq;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.DataTrigger;

/// <summary>
/// An action that will change a specified property to a specified value when invoked.
/// </summary>
public sealed class Set : AvaloniaObject, IAction
{
    protected override void LogBindingError(AvaloniaProperty property, Exception e) {}//todo binding errors disabled
    
    /// <summary>
    /// Identifies the <seealso cref="Property"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyProperty =
        AvaloniaProperty.Register<Set, string?>(nameof(Property));

    /// <summary>
    /// Identifies the <seealso cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> TargetProperty =
        AvaloniaProperty.Register<Set, object?>(nameof(Target));

    /// <summary>
    /// Identifies the <seealso cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<Set, object?>(nameof(Value));

    /// <summary>
    /// Identifies the <seealso cref="When"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> WhenProperty =
        AvaloniaProperty.Register<Set, object?>(nameof(When));

    /// <summary>
    /// Gets or sets the value to be compared with the value of <see cref="DataTrigger.Binding"/>. This is a avalonia property.
    /// </summary>
    public object? When
    {
        get => GetValue(WhenProperty);
        set => SetValue(WhenProperty, value);
    }

    /// <summary>
    /// Gets or sets the name of the property to change. This is a avalonia property.
    /// </summary>
    public string? Property
    {
        get => GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to set. This is a avalonia property.
    /// </summary>
    [Content]
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the object whose property will be changed.
    /// If <seealso cref="Target"/> is not set or cannot be resolved, the sender of <seealso cref="Execute"/> will be used. This is a avalonia property.
    /// </summary>
    public object? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }
        
    static Set() => Observable.Merge<AvaloniaPropertyChangedEventArgs>(
            WhenProperty.Changed, 
            ValueProperty.Changed,
            TargetProperty.Changed,
            PropertyProperty.Changed)
        .AddClassHandler<Set>((x, y) => DataTrigger.OnValueChanged(x.Parent, y));

    public DataTrigger? Parent { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if updating the property value succeeds; else false.</returns>
    public object Execute(object? sender, object? parameter)
    {
        if ((Target ?? sender) is not {} targetObject ||
            (Property ?? Parent?.Property) is not {} propertyName)
        {
            return false;
        }

        if (targetObject is IAvaloniaObject obj)
        {
            if ((_propertyCache ??= PropertyNavigator.GetAvaloniaProperty(obj, propertyName)) is not
                {IsReadOnly: false} prop)
            {
                return false;
            }
            try
            {
                obj.SetValue(prop, CastValue(Value, prop.PropertyType));
            }
            catch (FormatException e)
            {
                Throw(obj, e, propertyName);
            }
            catch (ArgumentException e)
            {
                Throw(obj, e, propertyName);
            }
            return true;
        }

        UpdatePropertyValue(targetObject, propertyName);
        return true;
    }

    private AvaloniaProperty? _propertyCache;

    private void UpdatePropertyValue(object targetObject, string propertyName)
    {
        var targetType = targetObject.GetType();
        var targetTypeName = targetType.Name;
        var propertyInfo = targetType.GetRuntimeProperty(propertyName);

        if (propertyInfo is null || 
            !propertyInfo.CanWrite)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Cannot find a property named {0} on type {1}.",
                propertyName,
                targetTypeName));
        }
        
        try
        {
            object? result = null;
            var propertyType = propertyInfo.PropertyType;
            var propertyTypeInfo = propertyType.GetTypeInfo();
            if (Value is null)
            {
                // The result can be null if the type is generic (nullable), or the default value of the type in question
                result = propertyTypeInfo.IsValueType ? Activator.CreateInstance(propertyType) : null;
            }
            else if (propertyTypeInfo.IsAssignableFrom(Value.GetType().GetTypeInfo()))
            {
                result = Value;
            }
            else
            {
                var valueAsString = Value.ToString();
                if (valueAsString is { })
                {
                    result = propertyTypeInfo.IsEnum ? Enum.Parse(propertyType, valueAsString, false) :
                        TypeConverterHelper.Convert(valueAsString, propertyType);
                }
            }

            propertyInfo.SetValue(targetObject, result, Array.Empty<object>());
        }
        catch (FormatException e)
        {
            Throw(targetObject, e, propertyName);
        }
        catch (ArgumentException e)
        {
            Throw(targetObject, e, propertyName);
        }
    }

    private void Throw(object avaloniaObject, Exception innerException, string propertyName) =>
        throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Cannot assign value of type {0} to property {1} of type {2}. The {1} property can be assigned only values of type {2}.",
                Value?.GetType().Name ?? "null",
                propertyName,
                avaloniaObject.GetType().Name),
            innerException);

    public static object? CastValue(object? val, Type type)
    {
        var propertyTypeInfo = type.GetTypeInfo();

        if (val is null)
        {
            // The result can be null if the type is generic (nullable), or the default value of the type in question
            return propertyTypeInfo.IsValueType ? Activator.CreateInstance(type) : null;
        }

        if (propertyTypeInfo.IsAssignableFrom(val.GetType().GetTypeInfo()))
        {
            return val;
        }

        if (type == typeof(Thickness) &&
            val is string thickness)
        {
            return Thickness.Parse(thickness);
        }

        if (type == typeof(IBrush) &&
            val is string brush)
        {
            return SolidColorBrush.Parse(brush);
        }

        var valueAsString = val.ToString();
        if (valueAsString is { })
        {
            return propertyTypeInfo.IsEnum
                ? Enum.Parse(type, valueAsString, false)
                : TypeConverterHelper.Convert(valueAsString, type);
        }

        return null;
    }
}
