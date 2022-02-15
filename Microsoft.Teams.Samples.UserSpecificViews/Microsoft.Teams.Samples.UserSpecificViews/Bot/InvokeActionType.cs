// <copyright file="InvokeActionType.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Samples.UserSpecificViews.Bot
{
    /// <summary>
    /// Defined different types of invoke actions supported by the app.
    /// </summary>
    public enum InvokeActionType
    {
        /// <summary>
        /// Automatic refresh action.
        /// </summary>
        AutoRefresh,

        /// <summary>
        /// Manual refresh action.
        /// </summary>
        ManualRefresh,
    }
}
