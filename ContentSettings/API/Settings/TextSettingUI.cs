// -----------------------------------------------------------------------
// <copyright file="TextSettingUI.cs" company="ContentSettings">
// Copyright (c) ContentSettings. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ContentSettings.API.Settings;

using System.Diagnostics.CodeAnalysis;
using TMPro;
using Zorro.Settings;

/// <summary>
/// The UI cell for a text setting.
/// </summary>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307", Justification = "Required for Unity serialization.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401", Justification = "Required for Unity serialization.")]
public class TextSettingUI : SettingInputUICell
{
    /// <summary>
    /// Gets the input field for the text setting.
    /// </summary>
    public TMP_InputField? inputField;

    /// <summary>
    /// Sets up the setting UI cell with the provided setting and setting handler.
    /// </summary>
    /// <param name="setting">The text setting to set up the UI cell with.</param>
    /// <param name="settingHandler">The setting handler to use for saving the setting.</param>
    public override void Setup(Setting setting, ISettingHandler settingHandler)
    {
        if (setting is not TextSetting textSetting)
        {
            return;
        }

        if (inputField == null)
        {
            return;
        }

        inputField.SetTextWithoutNotify(textSetting.Expose(textSetting.Value));
        inputField.onValueChanged.AddListener(OnInputChanged);

        return;

        void OnInputChanged(string value)
        {
            inputField.SetTextWithoutNotify(value);
            textSetting.SetValue(value, settingHandler);
        }
    }
}
