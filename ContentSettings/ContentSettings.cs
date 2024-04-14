// -----------------------------------------------------------------------
// <copyright file="ContentSettings.cs" company="ContentSettings">
// Copyright (c) ContentSettings. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ContentSettings;

using BepInEx;
using BepInEx.Logging;
using global::ContentSettings.API;
using HarmonyLib;

/// <summary>
/// The main Content Settings plugin class.
/// </summary>
[ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, true)]
[BepInAutoPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public partial class ContentSettings : BaseUnityPlugin
{
    /// <summary>
    /// Gets the logger of the plugin.
    /// </summary>
    internal static new ManualLogSource Logger { get; private set; } = null!;

    private Harmony Harmony { get; } = new (MyPluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        Logger = base.Logger;

        Harmony.PatchAll();

        SettingsSyncer.Init();
    }
}
