// <copyright file="ICardFactory.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Samples.UserSpecificViews.Cards
{
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Card factory contract.
    ///
    /// Provides methods to create different ACs.
    /// </summary>
    public interface ICardFactory
    {
        /// <summary>
        /// Gets auto refresh card.
        /// </summary>
        /// <param name="count">Count</param>
        /// <param name="shouldRefresh">Should refresh.</param>
        /// <returns>Card attachment.</returns>
        Attachment GetAutoRefreshCard(int count, bool shouldRefresh);

        /// <summary>
        /// Gets user specifiv view card.
        /// </summary>
        /// <param name="count">Refesh count.</param>
        /// <param name="shouldRefresh">Should refresh.</param>
        /// <param name="userUpn">User Upn.</param>
        /// <returns>Card attachment.</returns>
        Attachment GetUserSpecificViewCard(int count, bool shouldRefresh, string userUpn);
    }
}
