// -----------------------------------------------------------------------
// <copyright file="SettingsTab.cs" company="ContentSettings">
// Copyright (c) ContentSettings. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ContentSettings.API.Settings.UI;

using BepInEx;
using Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A component representing a tab in a settings menu, which can be selected to show settings belonging to the tab.
/// </summary>
public class SettingsTab : SettingsButton
{
    /// <summary>
    /// The current settings menu instance.
    /// </summary>
    [SerializeField]
    private SettingsMenu? settingsMenu;

    /// <summary>
    /// Shows the settings for the tab.
    /// </summary>
    public void Show()
    {
        if (!SettingsLoader.TryGetTab(Name, out var settingsByCategory))
        {
            return;
        }

        if (settingsMenu == null)
        {
            return;
        }

        foreach (var settingsCell in settingsMenu.m_cells)
        {
            Destroy(settingsCell.gameObject);
        }

        settingsMenu.m_cells.Clear();

        foreach (Transform child in settingsMenu.m_settingsContainer)
        {
            Destroy(child.gameObject);
        }

        if (settingsMenu.m_settingsCell == null || settingsMenu.m_settingsContainer == null)
        {
            return;
        }

        var settingsHandler = GameHandler.Instance.SettingsHandler;

        foreach (var (category, settings) in settingsByCategory)
        {
            if (!category.IsNullOrWhiteSpace())
            {
                var categoryCell = Instantiate(
                    SettingsAssets.SettingsCategoryPrefab,
                    settingsMenu.m_settingsContainer);
                categoryCell.GetComponentInChildren<TextMeshProUGUI>().text = category;
            }

            foreach (var setting in settings)
            {
                var component = Instantiate(settingsMenu.m_settingsCell, settingsMenu.m_settingsContainer)
                    .GetComponent<SettingsCell>();
                component.Setup(setting, settingsHandler);
                settingsMenu.m_cells.Add(component);
            }
        }
    }

    /// <summary>
    /// Called when the tab is clicked, selecting the tab.
    /// </summary>
    public void OnPointerClicked()
    {
        transform.parent.GetComponent<SettingsNavigation>().Select(this);
    }

    /// <summary>
    /// Called by Unity when the script instance has been loaded.
    /// </summary>
    protected void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnPointerClicked);
    }

    /// <inheritdoc />
    protected override void Awake()
    {
        base.Awake();

        settingsMenu = GetComponentInParent<SettingsMenu>();
    }
}
