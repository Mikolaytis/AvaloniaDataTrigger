using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.DataTrigger;

public sealed class CloseParentMenuBehavior : Behavior<Button>
{
    protected override void OnAttached()
    {
        AssociatedObject?.AddHandler(Button.ClickEvent, OnClick, RoutingStrategies.Bubble);
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject?.AddHandler(Button.ClickEvent, OnClick, RoutingStrategies.Bubble);
        base.OnDetaching();
    }

    private void OnClick(object? sender, RoutedEventArgs e)
    {
        IControl? item = AssociatedObject;
        while (item != null)
        {
            if (item is ContextMenu menu)
            {
                menu.Close();
                return;
            }
            item = item.Parent;
        }
    }
}