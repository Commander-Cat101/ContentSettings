// -----------------------------------------------------------------------
// <copyright file="SettingsLoader.cs" company="ContentSettings">
// Copyright (c) ContentSettings. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ContentSettings.API;

using System.Collections.Generic;
using Zorro.Settings;
using TMPro;
using UnityEngine.Localization.PropertyVariants;
using JetBrains.Annotations;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

/// <summary>
/// Settings loader for custom settings belonging to mods.
/// </summary>
public static class SettingsLoader
{
    /// <summary>
    /// Gets all registered settings.
    /// </summary>
    [UsedImplicitly]
    public static IEnumerable<Setting> Settings => RegisteredSettings.Values;

    private static DefaultSettingsSaveLoad SaveLoader { get; } = new ();

    private static Dictionary<string, Dictionary<string, List<Setting>>> SettingsByCategoryByTab { get; } = new ();

    private static Dictionary<Type, Setting> RegisteredSettings { get; } = new ();

    private static bool IsInitialized { get; set; }

    private static bool IsDirty { get; set; }

    /// <summary>
    /// Returns whether the settings manager has a tab.
    /// </summary>
    /// <param name="tab">The tab to check for.</param>
    /// <returns>True if the settings manager has the tab; otherwise, false.</returns>
    public static bool HasTab(string tab) => SettingsByCategoryByTab.ContainsKey(tab);

    /// <summary>
    /// Register a custom setting.
    /// </summary>
    /// <remarks>This will apply the value of the setting immediately. See <see cref="Setting.ApplyValue"/>.</remarks>
    /// <param name="tab">The tab to register the setting to.</param>
    /// <param name="category">The category of the setting.</param>
    /// <param name="setting">The setting to register.</param>
    [UsedImplicitly]
    public static void RegisterSetting(string tab, string category, Setting setting)
    {
        var settingsByCategory = SettingsByCategoryByTab
            .GetValueOrDefault(tab, new Dictionary<string, List<Setting>>());
        var settings = settingsByCategory.GetValueOrDefault(category, new List<Setting>());

        settingsByCategory[category] = settings;
        SettingsByCategoryByTab[tab] = settingsByCategory;

        settings.Add(setting);

        if (!RegisteredSettings.ContainsKey(setting.GetType()))
        {
            setting.Load(SaveLoader);
            setting.ApplyValue();

            RegisteredSettings.Add(setting.GetType(), setting);
        }

        if (IsInitialized)
        {
            IsDirty = true;
        }
    }

    /// <summary>
    /// Loads the settings into the settings menu.
    /// </summary>
    /// <param name="tab">The tab to load the settings from.</param>
    /// <param name="menu">The settings menu to load the settings into.</param>
    internal static void LoadSettingsMenu(string tab, SettingsMenu menu)
    {
        if (IsDirty)
        {
            CreateSettings(menu);
            IsDirty = false;
        }

        if (!SettingsByCategoryByTab.TryGetValue(tab, out var settingsByCategory))
        {
            return;
        }

        foreach (var settingsCell in menu.m_cells)
        {
            Object.Destroy(settingsCell.gameObject);
        }

        menu.m_cells.Clear();

        var settingsHandler = GameHandler.Instance.SettingsHandler;

        foreach (var (category, settings) in settingsByCategory)
        {
            // var categoryCell = settingsHandler.CreateCategoryCell(menu.transform, category);
            // menu.m_cells.Add(categoryCell);
            foreach (var setting in settings)
            {
                var component = Object.Instantiate(menu.m_settingsCell, menu.m_settingsContainer)
                    .GetComponent<SettingsCell>();
                component.Setup(setting, settingsHandler);
                menu.m_cells.Add(component);
            }
        }
    }

    /// <summary>
    /// Saves all registered settings.
    /// </summary>
    internal static void SaveSettings()
    {
        foreach (var setting in Settings)
        {
            setting.Save(SaveLoader);
        }
    }

    /// <summary>
    /// Creates the settings tab for the modded settings.
    /// </summary>
    /// <param name="menu">The settings menu to create the tab in.</param>
    /// <exception cref="System.Exception">Thrown when the existing tab to create the modded settings tab from is not found.</exception>
    internal static void CreateSettings(SettingsMenu menu)
    {
        var settingsTabs = menu.transform.Find("Content")?.Find("TABS");
        if (settingsTabs == null)
        {
            throw new Exception("Failed to find settings tab.");
        }

        var existingTab = settingsTabs.GetChild(0)?.gameObject;
        if (existingTab == null)
        {
            throw new Exception("Failed to find existing tab.");
        }

        foreach (var tab in SettingsByCategoryByTab.Keys)
        {
            if (settingsTabs.Find(tab) != null)
            {
                continue;
            }

            var customSettingsTab = Object.Instantiate(existingTab, settingsTabs, true);
            customSettingsTab.name = tab;

            var customSettingsTabText = customSettingsTab.transform.GetChild(1);
            Object.Destroy(customSettingsTabText.GetComponent<GameObjectLocalizer>());
            customSettingsTabText.GetComponent<TextMeshProUGUI>().SetText(tab);

            LoadSettingsMenu(tab, menu);
        }
    }

    /// <summary>
    /// Register all settings in the current domain.
    /// </summary>
    internal static void RegisterSettings()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                if (!type.IsSubclassOf(typeof(Setting)))
                {
                    continue;
                }

                if (RegisteredSettings.ContainsKey(type))
                {
                    ContentSettings.Logger.LogInfo($"Setting {type.Name} is already registered.");
                    continue;
                }

                var settingDefinitions = type.GetCustomAttributes<Attributes.SettingRegister>(false);
                Setting? setting = null;
                foreach (var settingDefinition in settingDefinitions)
                {
                    setting ??= (Setting)Activator.CreateInstance(type);

                    RegisterSetting(settingDefinition.Tab, settingDefinition.Category, setting);
                }
            }
        }

        IsInitialized = true;
    }
}
