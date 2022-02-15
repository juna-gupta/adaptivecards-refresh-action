// <copyright file="CardFactory.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Samples.UserSpecificViews.Cards
{
    using System;
    using System.IO;
    using AdaptiveCards;
    using AdaptiveCards.Templating;
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;

    /// <summary>
    /// Card factory implementation.
    /// </summary>
    public class CardFactory : ICardFactory
    {
        private const string AutoRefreshCardTemplatePath = "{0}\\assets\\templates\\auto-refresh-card.json";
        private const string UserSpecificViewCardTemplatePath = "{0}\\assets\\templates\\user-specific-view-card.json";
        private const string TestCardTemplatePath = "{0}\\assets\\templates\\wide-card.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="CardFactory"/> class.
        /// </summary>
        /// <param name="appSettings">App settings.</param>
        public CardFactory()
        {
        }

        /// <inheritdoc/>
        public Attachment GetAutoRefreshCard(int count, bool shouldRefresh)
        {
            var data = new
            {
                count = count,
                shouldRefresh = shouldRefresh
            };

            var template = GetCardTemplate(AutoRefreshCardTemplatePath);
            var serializedJson = template.Expand(data);
            return CreateAttachment(serializedJson);
        }

        /// <inheritdoc/>
        public Attachment GetUserSpecificViewCard(int count, bool shouldRefresh, string userUpn)
        {
            var data = new
            {
                count = count,
                shouldRefresh = shouldRefresh,
                userUpn = userUpn
            };

            var template = GetCardTemplate(UserSpecificViewCardTemplatePath);
            var serializedJson = template.Expand(data);
            return CreateAttachment(serializedJson);
        }

        private AdaptiveCardTemplate GetCardTemplate(string templatePath)
        {
            templatePath = string.Format(templatePath, AppDomain.CurrentDomain.BaseDirectory);
            return new AdaptiveCardTemplate(File.ReadAllText(templatePath));
        }

        private Attachment CreateAttachment(string adaptiveCardJson)
        {
            var adaptiveCard = AdaptiveCard.FromJson(adaptiveCardJson);
            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
        }
    }
}
