// -----------------------------------------------------------------------
// <copyright file="SettingsSyncer.cs" company="ContentSettings">
// Copyright (c) ContentSettings. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ContentSettings.API
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using global::ContentSettings.API.Settings;
    using MyceliumNetworking;
    using Zorro.Settings;

    public static class SettingsSyncer
    {
        /// <summary>
        /// All settings that inherit ISyncedSetting.
        /// </summary>
        private static List<Setting> syncedSettings = new List<Setting>();

        /// <summary>
        /// Initialises settingssyncer.
        /// </summary>
        internal static void Init()
        {
            MyceliumNetwork.LobbyEntered += () =>
            {
                SyncToLobbyData();
            };
            MyceliumNetwork.LobbyDataUpdated += obj =>
            {
                LoadFromLobbyData();
            };
        }

        /// <summary>
        /// Initialises a setting and registers it with Mycelium.
        /// </summary>
        internal static void InitSetting(Setting setting)
        {
            ISyncedSetting syncedSetting = (ISyncedSetting)setting;
            MyceliumNetwork.RegisterLobbyDataKey(syncedSetting.DataKey());
        }

        /// <summary>
        /// Syncs mycelium lobby data or loads if not host.
        /// </summary>
        internal static void SyncToLobbyData()
        {
            if (MyceliumNetwork.IsHost)
            {
                foreach (var setting in syncedSettings)
                {
                    ISyncedSetting syncedSetting = (ISyncedSetting)setting;
                    MyceliumNetwork.SetLobbyData(syncedSetting.DataKey(), syncedSetting.GetValue());
                }
            }
            else
            {
                LoadFromLobbyData();
            }
        }

        /// <summary>
        /// Loads lobby data from mycelium.
        /// </summary>
        internal static void LoadFromLobbyData()
        {
            if (MyceliumNetwork.IsHost)
            {
                return;
            }

            foreach (var setting in syncedSettings)
            {
                ISyncedSetting syncedSetting = (ISyncedSetting)setting;
                if (MyceliumNetwork.HasLobbyData(syncedSetting.DataKey()))
                {
                    // How to load???
                }
                else
                {
                    throw new Exception($"No lobby data under key: {syncedSetting.DataKey()}");
                }
            }
        }
    }
}
