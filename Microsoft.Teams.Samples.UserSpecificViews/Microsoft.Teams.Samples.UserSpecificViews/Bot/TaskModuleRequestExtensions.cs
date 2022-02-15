﻿// <copyright file="TaskModuleRequestExtensions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Samples.LinkUnfurling.Web.Bot
{
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Teams.Samples.UserSpecificViews.Bot;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Task module request extensions.
    /// </summary>
    public static class TaskModuleRequestExtensions
    {
        /// <summary>
        /// Checks if its a review in a meeting action request.
        /// </summary>
        /// <param name="request">Task module request.</param>
        /// <returns>True if its a meeting action request, false otherwise.</returns>
        public static bool IsAutoRefresh(this TaskModuleRequest request)
        {
            var data = request.Data as JObject;
            data = JObject.FromObject(data);
            var actionId = data["actionId"]?.ToString();
            return !string.IsNullOrEmpty(actionId) && actionId.Equals(InvokeActionType.AutoRefresh.ToString());
        }
    }
}
