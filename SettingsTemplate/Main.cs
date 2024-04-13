namespace SettingsTemplate;

using BepInEx;
using System.Collections.Generic;
using ContentSettings.API.Attributes;
using ContentSettings.API.Settings;
using JetBrains.Annotations;
using Zorro.Settings;

[ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, true)]
[BepInAutoPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public partial class Main : BaseUnityPlugin
{
    public static Main Instance { get; private set; } = null!;

    public bool FeatureEnabled { [UsedImplicitly] get; internal set; }

    private void Awake()
    {
        Instance = this;
    }
}

[SettingRegister("MODDED5", "TEST")]
[SettingRegister("MODDED4", "TEST")]
[SettingRegister("MODDED3", "TEST")]
[SettingRegister("MODDED2", "TEST")]
[SettingRegister("MODDED", "TEST")]
public class ModFeatureSetting : EnumSetting, ICustomSetting
{
    public override void ApplyValue() => Main.Instance.FeatureEnabled = Value != 0;

    public override List<string> GetChoices() => ["Off", "On"];

    public string GetDisplayName() => "Mod Feature Enabled?";

    public override int GetDefaultValue() => 1;
}

[SettingRegister("MODDED", "TEST")]
public class ModFeatureSetting2 : TextSetting, ICustomSetting
{
    public override void ApplyValue()
    { }

    public string GetDisplayName() => "Cool Feature";

    protected override string GetDefaultValue() => "On";
}
