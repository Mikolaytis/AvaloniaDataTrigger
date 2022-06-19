using Avalonia.Controls;

namespace Avalonia.DataTrigger;

internal static class PropertyNavigator
{
    private static readonly char[] s_trimChars = { '(', ')' };
    private static readonly char[] s_separator = { '.' };

    public static AvaloniaProperty? GetAvaloniaProperty(IAvaloniaObject obj, string property)
    {
        if (property.Contains('.'))
        {
            if (FindAttachedProperty(obj, property) is { } prop)
            {
                return prop;
            }
        }
        else if (AvaloniaPropertyRegistry.Instance.FindRegistered(obj, property) is { } prop)
        {
            return prop;
        }

        return null;
    }

    private static AvaloniaProperty? FindAttachedProperty(object? targetObject, string propertyName)
    {
        if (propertyName == "ToolTip.Tip")
        {
            return ToolTip.TipProperty;
        }
        if (targetObject is null)
        {
            return null;
        }
        var targetType = targetObject.GetType();
        var registeredAttached = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(targetType);
        var registeredAttachedCount = registeredAttached.Count;
        var propertyNames = propertyName.Trim().Trim(s_trimChars).Split(s_separator);
        if (propertyNames.Length != 2 ||
            propertyNames[0] != targetType.Name)
        {
            return null;
        }
        for (var i = 0; i < registeredAttachedCount; i++)
        {
            var avaloniaProperty = registeredAttached[i];
            if (avaloniaProperty.Name == propertyNames[1])
            {
                return avaloniaProperty;
            }
        }
        return null;
    }
}