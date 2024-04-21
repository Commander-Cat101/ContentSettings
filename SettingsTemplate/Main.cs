namespace SettingsTemplate;

using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using ContentSettings.API.Attributes;
using ContentSettings.API.Settings;
using JetBrains.Annotations;
using Unity.Mathematics;
using Zorro.Settings;
using IntSetting = ContentSettings.API.Settings.IntSetting;

[ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, true)]
[BepInAutoPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public partial class Main : BaseUnityPlugin
{
    /// <summary>
    /// Gets the logger of the plugin.
    /// </summary>
    internal static new ManualLogSource Logger { get; private set; } = null!;

    public static Main Instance { get; private set; } = null!;

    public bool FeatureEnabled { [UsedImplicitly] get; internal set; }

    private void Awake()
    {
        Instance = this;

        Logger = base.Logger;
    }
}

[SettingRegister("MODDED")]
public class ModFeatureSetting1 : EnumSetting, ICustomSetting
{
    public override void ApplyValue()
    {
        Main.Logger.LogInfo($"Enum Feature value: {Value}");
    }

    public string GetDisplayName() => "Enum Feature";

    public override int GetDefaultValue() => 1;

    public override List<string> GetChoices() => ["Off", "On"];
}

[SettingRegister("MODDED")]
public class ModFeatureSetting2 : FloatSetting, ICustomSetting
{
    public override void ApplyValue()
    {
        Main.Logger.LogInfo($"Float Feature value: {Value}");
    }

    public string GetDisplayName() => "Float Feature";

    public override float GetDefaultValue() => 0.5f;

    public override float2 GetMinMaxValue() => new(0.0f, 1.0f);
}

[SettingRegister("MODDED", "NEW SETTINGS")]
public class ModFeatureSetting3 : IntSetting, ICustomSetting
{
    public override void ApplyValue()
    {
        Main.Logger.LogInfo($"Integer Feature value: {Value}");
    }

    public string GetDisplayName() => "Integer Feature";

    /// <summary>
    /// Normally, this should be between your min and max values, but for demonstration purposes, it's set to 50
    /// so that we can demonstrate the clamping feature.
    /// </summary>
    /// <returns>The default value of the setting.</returns>
    protected override int GetDefaultValue() => 50;

    override protected (int, int) GetMinMaxValue() => (0, 10);
}

[SettingRegister("MODDED", "NEW SETTINGS")]
public class ModFeatureSetting4 : TextSetting, ICustomSetting
{
    public override void ApplyValue()
    {
        Main.Logger.LogInfo($"Text Feature value: {Value}");
    }

    public string GetDisplayName() => "Text Feature";

    protected override string GetDefaultValue() => "Placeholder Text";
}

[SettingRegister("MODDED", "NEW SETTINGS")]
public class ModFeatureSetting5 : BoolSetting, ICustomSetting
{
    public override void ApplyValue()
    {
        Main.Logger.LogInfo($"Boolean Feature value: {Value}");
        Main.Instance.FeatureEnabled = Value;
    }

    public string GetDisplayName() => "Boolean Feature";

    protected override bool GetDefaultValue() => true;
}
