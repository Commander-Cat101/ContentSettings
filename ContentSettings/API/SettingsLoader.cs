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
using UnityEngine;
using UnityEngine.UI;
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
        var settingType = setting.GetType();
        ContentSettings.Logger.LogInfo($"Registering setting {settingType.Name}({settingType.BaseType?.Name}) to tab {tab} and category {category}.");

        var settingsByCategory = SettingsByCategoryByTab
            .GetValueOrDefault(tab, new Dictionary<string, List<Setting>>());
        var settings = settingsByCategory.GetValueOrDefault(category, new List<Setting>());

        settingsByCategory[category] = settings;
        SettingsByCategoryByTab[tab] = settingsByCategory;

        settings.Add(setting);

        if (!RegisteredSettings.ContainsKey(settingType))
        {
            setting.Load(SaveLoader);
            setting.ApplyValue();

            RegisteredSettings.Add(settingType, setting);
        }

        if (IsInitialized)
        {
            IsDirty = true;
        }
    }

    /// <summary>
    /// Register a custom setting to the default MODDED tab and empty category.
    /// </summary>
    /// <param name="setting">The setting to register.</param>
    [UsedImplicitly]
    public static void RegisterSetting(Setting setting)
    {
        RegisterSetting("MODDED", string.Empty, setting);
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
        var tabs = menu.transform.Find("Content")?.Find("TABS");
        if (tabs == null)
        {
            throw new Exception("Failed to find settings tab.");
        }

        // BuildSettingsMenu(menu, tabs);
        var existingTab = tabs.GetChild(0)?.gameObject;
        if (existingTab == null)
        {
            throw new Exception("Failed to find existing tab.");
        }

        foreach (var tab in SettingsByCategoryByTab.Keys)
        {
            if (tabs.Find(tab) != null)
            {
                continue;
            }

            var customSettingsTab = Object.Instantiate(existingTab, tabs, true);
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
        if (IsInitialized)
        {
            return;
        }

        ContentSettings.Logger.LogInfo("Registering settings.");

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

        ContentSettings.Logger.LogInfo($"Registered {RegisteredSettings.Count} settings.");

        IsInitialized = true;
    }

    private static void BuildSettingsMenu(SettingsMenu menu, Transform tabs)
    {
        var tabsRectTransform = tabs.GetComponent<RectTransform>();

        var scrollViewObject = new GameObject("TabScrollView");
        scrollViewObject.AddComponent<CanvasRenderer>();
        scrollViewObject.AddComponent<RectTransform>();
        scrollViewObject.AddComponent<ScrollRect>();
        scrollViewObject.transform.SetParent(tabs.transform.parent, false);

        var scrollRectTransform = scrollViewObject.GetComponent<RectTransform>();
        scrollRectTransform.localPosition = Vector2.zero;
        scrollRectTransform.sizeDelta = tabsRectTransform.sizeDelta;
        scrollRectTransform.anchorMin = tabsRectTransform.anchorMin;
        scrollRectTransform.anchorMax = tabsRectTransform.anchorMax;
        scrollRectTransform.pivot = tabsRectTransform.pivot;

        var scrollRect = scrollViewObject.GetComponent<ScrollRect>();
        scrollRect.horizontal = true;
        scrollRect.vertical = false;

        var viewport = new GameObject("Viewport");
        viewport.AddComponent<CanvasRenderer>();
        viewport.AddComponent<RectTransform>();
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = Color.clear;

        var viewportRectTransform = viewport.GetComponent<RectTransform>();
        viewport.transform.SetParent(scrollViewObject.transform, false);
        viewportRectTransform.anchorMin = Vector2.zero;
        viewportRectTransform.anchorMax = Vector2.one;
        viewportRectTransform.sizeDelta = Vector2.zero;
        viewportRectTransform.pivot = new Vector2(0.5f, 0.5f);

        tabs.transform.SetParent(viewport.transform, false);
        tabsRectTransform.anchorMin = Vector2.zero;
        tabsRectTransform.anchorMax = Vector2.up;
        tabsRectTransform.pivot = new Vector2(0, 0.5f);
        tabsRectTransform.sizeDelta = new Vector2(0, tabsRectTransform.sizeDelta.y);

        var contentSizeFitter = tabs.gameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        scrollRect.content = tabsRectTransform;
    }
}
