// -----------------------------------------------------------------------
// <copyright file="SettingsMapper.cs" company="ContentSettings">
// Copyright (c) ContentSettings. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ContentSettings.API;

using Settings;
using UnityEngine;

/// <summary>
/// Used to map input cells to game objects.
/// </summary>
public class SettingsMapper
{
    private static SettingsMapper? _instance;

    private SettingsMapper()
    {
        TextSettingCell = TextSettingBuilder.Build();
    }

    /// <summary>
    /// Gets the instance of the input cell mapper.
    /// </summary>
    public static SettingsMapper Instance => _instance ??= new SettingsMapper();

    /// <summary>
    /// Gets the text setting cell.
    /// </summary>
    public GameObject TextSettingCell { get; }
}
