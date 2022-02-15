// <copyright file="BotActivityHandler.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Samples.UserSpecificViews.Bot
{
    using System;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Samples.UserSpecificViews.Cards;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Teams Bot Activity Handler.
    /// </summary>
    public class BotActivityHandler : TeamsActivityHandler
    {
        private readonly ICardFactory cardFactory;
        private readonly ILogger<BotActivityHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotActivityHandler"/> class.
        /// </summary>
        /// <param name="cardFactory">Card factory.</param>
        /// <param name="logger">Logger.</param>
        public BotActivityHandler(
            ICardFactory cardFactory,
            ILogger<BotActivityHandler> logger)
        {
            this.cardFactory = cardFactory ?? throw new ArgumentNullException(nameof(cardFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Auto refresh.
            if (turnContext.Activity.Text.Contains("ar"))
            {
                var card = this.cardFactory.GetAutoRefreshCard(0/*initialCounter*/, true);
                await turnContext.SendActivityAsync(MessageFactory.Attachment(card), cancellationToken);
            }
            // User specific view
            else if (turnContext.Activity.Text.Contains("usv"))
            {
                var member = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);
                var card = this.cardFactory.GetUserSpecificViewCard(0/*initialCounter*/, true, member.UserPrincipalName);
                await turnContext.SendActivityAsync(MessageFactory.Attachment(card), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync("Try 'ar' or 'usv' commands.", cancellationToken: cancellationToken);
            }
        }

        protected override async Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
             if (turnContext.Activity.Name == "adaptiveCard/action")
            {
                var actionData = JsonConvert.DeserializeObject<RefreshActionData>(turnContext.Activity.Value.ToString());
                switch (actionData.action.verb)
                {
                    case "AutoRefresh":
                            var card = this.cardFactory.GetAutoRefreshCard(++actionData.action.data.RefreshCount, true);
                            var newCardResponse = new AdaptiveCardInvokeResponse()
                            {
                                StatusCode = 200,
                                Type = card.ContentType,
                                Value = card.Content
                            };
                            return ActivityHandler.CreateInvokeResponse(newCardResponse);
                        break;

                    case "UserSpecificView":
                            var member = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);
                            card = this.cardFactory.GetUserSpecificViewCard(++actionData.action.data.RefreshCount, false, member.UserPrincipalName);
                            newCardResponse = new AdaptiveCardInvokeResponse()
                            {
                                StatusCode = 200,
                                Type = card.ContentType,
                                Value = card.Content
                            };
                            return ActivityHandler.CreateInvokeResponse(newCardResponse);
                        break;
                }
            }

            var adaptiveCardResponse = new AdaptiveCardInvokeResponse()
            {
                StatusCode = 200,
                Type = "application/vnd.microsoft.activity.message",
                Value = "No update."
            };

            return ActivityHandler.CreateInvokeResponse(adaptiveCardResponse);
        }
    }
}
