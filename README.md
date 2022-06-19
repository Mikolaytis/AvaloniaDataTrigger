# Avalonia DataTrigger

This library tries to implement DataTrigger and MultiDataTrigger like it works in WPF.

Nuget is not ready yet. Copy paste the code and use it if you want.
Samples will come later.

Example
```

 <Path>
    <Interaction.Behaviors>
        <DataTrigger Binding="{CompiledBinding NodeType}"
                     Default="{x:Static Icons.Tree_Vector}"
                     Property="Data">
            <Set When="{x:Static viewModels:NodeIconType.Hyperlink}" Value="{x:Static Icons.Button_Link}" />
            <Set When="{x:Static viewModels:NodeIconType.Sprite}" Value="{x:Static Icons.Tree_Image}" />
            <Set When="{x:Static viewModels:NodeIconType.Artboard}" Value="{x:Static Icons.Tree_Artboard}" />
            <Set When="{x:Static viewModels:NodeIconType.Text}" Value="{x:Static Icons.Tree_Text}" />
            <Set When="{x:Static viewModels:NodeIconType.Hotspot}" Value="{x:Static Icons.Tree_Hotspot}" />
            <Set When="{x:Static viewModels:NodeIconType.Slice}" Value="{x:Static Icons.Tree_Slice}" />
            <Set When="{x:Static viewModels:NodeIconType.Avatar}" Value="{x:Static Icons.Tree_Avatar}" />
            <Set When="{x:Static viewModels:NodeIconType.LayerStyleOverride}" Value="{x:Static Icons.Tree_LayerStyleOverride}" />
            <Set When="{x:Static viewModels:NodeIconType.ForeignComponent}" Value="{x:Static Icons.Tree_ForeignComponent}" />
            <Set When="{x:Static viewModels:NodeIconType.MainComponent}" Value="{CompiledBinding GroupLayout, Converter={x:Static IconConverters.SmartLayout}, ConverterParameter={x:Static Icons.Tree_Component}}" />
            <Set When="{x:Static viewModels:NodeIconType.Component}" Value="{CompiledBinding ComponentType, Converter={x:Static IconConverters.ComponentType}, ConverterParameter={x:Static Icons.Tree_Instance}}" />
            <Set When="{x:Static viewModels:NodeIconType.Group}" Value="{CompiledBinding GroupLayout, Converter={x:Static IconConverters.SmartLayout}, ConverterParameter={x:Static Icons.Tree_Group}}" />
        </DataTrigger>
        <DataTrigger Binding="{x:True}"
                     Default="{DynamicResource BlackBrush}"
                     Property="Fill">
            <Set When="{CompiledBinding IsHyperlink}" Value="{DynamicResource AccentBlueBrush}" />
            <Set When="{CompiledBinding IsComponentStyle}" Value="{DynamicResource PinkBrush}" />
            <Set When="{CompiledBinding IsFontMissing}" Value="{DynamicResource RedBrush}" />
        </DataTrigger>
    </Interaction.Behaviors>
</Path>

```

## License

Avalonia DataTrigger is licensed under the [MIT license](LICENSE.TXT).
