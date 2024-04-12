// -----------------------------------------------------------------------
// <copyright file="TextSettingBuilder.cs" company="ContentSettings">
// Copyright (c) ContentSettings. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ContentSettings.API.Settings;

using UnityEngine;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.UI;
using TMPro;

/// <summary>
/// Creates the UI cell for a text setting.
/// </summary>
internal static class TextSettingBuilder
{
    /// <summary>
    /// Builds the UI cell for a text setting programmatically.
    /// </summary>
    /// <returns>A game object representing the UI cell for a text setting.</returns>
    public static GameObject Build()
    {
        // Copy the FloatSettingCell to reuse the UI as a base
        var textInput = Object.Instantiate(InputCellMapper.Instance.FloatSettingCell) !;
        textInput.name = "TEXT INPUT";
        textInput.SetActive(false);

        // Remove the FloatSettingUI component
        Object.Destroy(textInput.GetComponent<FloatSettingUI>());

        // Remove the slider
        Object.Destroy(textInput.transform.FindChildRecursive("Slider")?.gameObject);

        var textSettingUi = textInput.AddComponent<TextSettingUI>();

        var tmpInputField = textInput.GetComponentInChildren<TMP_InputField>();
        tmpInputField.contentType = TMP_InputField.ContentType.Standard;

        textSettingUi.inputField = tmpInputField;

        const float sizeDeltaX = 257.5799f;
        const float sizeDeltaY = 53.4572f;

        var inputField = textInput.transform.Find("InputField (TMP)");
        if (inputField != null)
        {
            var rectTransform = inputField.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = new Vector2(sizeDeltaX, sizeDeltaY);
        }

        textInput.gameObject.MarkDirty();
        textInput.gameObject.SetActive(false);
        textInput.gameObject.SetActive(true);

        return textInput;
    }
}
