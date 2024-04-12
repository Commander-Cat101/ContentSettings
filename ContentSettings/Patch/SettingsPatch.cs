// -----------------------------------------------------------------------
// <copyright file="SettingsPatch.cs" company="ContentSettings">
// Copyright (c) ContentSettings. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ContentSettings.Patch;

using API;
using HarmonyLib;
using Zorro.Settings;

/// <summary>
/// Patches for the settings system.
/// </summary>
[HarmonyPatch]
internal class SettingsPatch
{
    /// <summary>
    /// Patches the <see cref="DefaultSettingsSaveLoad.WriteToDisk"/> method to save our custom settings.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DefaultSettingsSaveLoad), nameof(DefaultSettingsSaveLoad.WriteToDisk))]
    internal static void WriteToDiskPatch()
    {
        SettingsLoader.SaveSettings();
    }

    /// <summary>
    /// Patches the <see cref="SettingCategoryTab.Select"/> method to load our custom settings when the modded settings tab is selected.
    /// </summary>
    /// <param name="__instance">The instance of the <see cref="SettingCategoryTab"/>.</param>
    /// <returns>Whether the original method should be called.</returns>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SettingCategoryTab), nameof(SettingCategoryTab.Select))]
    internal static bool SelectPatch(SettingCategoryTab __instance)
    {
        if (!SettingsLoader.HasTab(__instance.name))
        {
            return true;
        }

        SettingsLoader.LoadSettingsMenu(__instance.name, __instance.settingsMenu);
        return false;
    }

    /// <summary>
    /// Patches the <see cref="SettingsMenu"/> to add our custom settings tab to the settings menu.
    /// </summary>
    /// <param name="__instance">The instance of the <see cref="SettingsMenu"/>.</param>
    /// <returns>Whether the original method should be called.</returns>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SettingsMenu), nameof(SettingsMenu.OnEnable))]
    internal static bool OnEnablePatch(SettingsMenu __instance)
    {
        SettingsLoader.RegisterSettings();
        SettingsLoader.CreateSettings(__instance);

        return true;
    }
}
