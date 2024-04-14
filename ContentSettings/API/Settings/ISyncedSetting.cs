// -----------------------------------------------------------------------
// <copyright file="ISyncedSetting.cs" company="ContentSettings">
// Copyright (c) ContentSettings. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ContentSettings.API.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using JetBrains.Annotations;

    /// <summary>
    /// Interface for syncing settings from the host.
    /// </summary>
    [UsedImplicitly]
    internal interface ISyncedSetting<TSetting>
    {
        /// <summary>
        /// The lobby data key used for this setting.
        /// </summary>
        /// <returns></returns>
        string LobbyDataGUID();

        TSetting GetValue();
    }
}
