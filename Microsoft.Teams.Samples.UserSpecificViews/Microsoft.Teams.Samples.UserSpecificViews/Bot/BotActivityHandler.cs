// <copyright file="BotActivityHandler.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Samples.UserSpecificViews.Bot
{
    using System;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Teams.Samples.UserSpecificViews.Cards;
    using Newtonsoft.Json;

    /// <summary>
    /// Teams Bot Activity Handler.
    /// </summary>
    public class BotActivityHandler : TeamsActivityHandler
    {
        private const string AllUserCardType = "All Users";
        private const string OnlyMeUserCardType = "Me Only";

        private readonly ICardFactory cardFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotActivityHandler"/> class.
        /// </summary>
        /// <param name="cardFactory">Card factory.</param>
        public BotActivityHandler(
            ICardFactory cardFactory)
        {
            this.cardFactory = cardFactory ?? throw new ArgumentNullException(nameof(cardFactory));
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Send initial card.
            var initialCard = this.cardFactory.GetSelectCardTypeCard();
            await turnContext.SendActivityAsync(MessageFactory.Attachment(initialCard), cancellationToken);
        }

        protected override async Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Name == "adaptiveCard/action")
            {
                var actionData = JsonConvert.DeserializeObject<RefreshActionData>(turnContext.Activity.Value.ToString());

                // Increase the refresh count.
                actionData.action.data.RefreshCount++;
                switch (actionData.action.verb)
                {
                    case "onlyme":
                        // Sends an auto refresh user specific view card for the sender.
                        var card = this.cardFactory.GetAutoRefreshForSpecificUserBaseCard(turnContext.Activity.From.Id, OnlyMeUserCardType);
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(card), cancellationToken);
                        break;

                    case "allusers":
                        // Sends an auto refresh user specific card for all the users in the chat.
                        card = this.cardFactory.GetAutoRefreshForAllUsersBaseCard(AllUserCardType);
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(card), cancellationToken);
                        break;

                    case "UpdateBaseCard":
                        // Updates base card for all users.
                        card = this.cardFactory.GetUpdatedBaseCard(actionData);
                        var activity = MessageFactory.Attachment(card);
                        activity.Id = turnContext.Activity.ReplyToId;
                        await turnContext.UpdateActivityAsync(activity, cancellationToken);
                        break;

                    case "RefreshUserSpecificView":
                        card = this.cardFactory.GetUpdatedCardForUser(turnContext.Activity.From.Id, actionData);
                        return PrepareInvokeResponse(card);
                }
            }

            var adaptiveCardResponse = new AdaptiveCardInvokeResponse()
            {
                StatusCode = 200,
                Type = "application/vnd.microsoft.activity.message",
                Value = "Success!" // Optional message to be shown to the user.
            };
            return ActivityHandler.CreateInvokeResponse(adaptiveCardResponse);
        }

        private InvokeResponse PrepareInvokeResponse(Attachment card)
        {
            var newCardResponse = new AdaptiveCardInvokeResponse()
            {
                StatusCode = 200,
                Type = card.ContentType,
                Value = card.Content
            };
            return ActivityHandler.CreateInvokeResponse(newCardResponse);
        }
    }
}
