using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CCAudioBot.Dialogs
{
    [Serializable]
    [LuisModel("2c854884-394b-424f-9cc6-f660d81915b8", "0091c7f5c04542249870c45525fe1fce")]
    public class ClearComLuisDialog : LuisDialog<object>
    {
        private const string PickDateEntityType = "builtin.datetime.date";
        private const string PickTimeEntityType = "builtin.datetime.time";
        private const string PickLocationEntityType = "builtin.geography.city";

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("JoinOrLeaveAChannel")]
        public async Task JoinOrLeaveAChannel(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            //foreach (var entity in result.Entities)
            //{
            //    switch (entity.Type)
            //    {
            //        case PickLocationEntityType:
            //            entities.Add(new EntityRecommendation(type: nameof(ClearComForm.PickLocation)) { Entity = entity.Entity });
            //            break;
            //        case PickDateEntityType:
            //            EntityRecommendation pickTime;
            //            result.TryFindEntity(PickTimeEntityType, out pickTime);
            //            var pickDateAndTime = entity.Entity + " " + pickTime?.Entity;
            //            if (!string.IsNullOrWhiteSpace(pickDateAndTime))
            //                entities.Add(new EntityRecommendation(type: nameof(ClearComForm.PickDateAndTime)) { Entity = pickDateAndTime });
            //            break;
            //        default:
            //            break;
            //    }
            //}

            var rentForm = new FormDialog<ClearComForm>(new ClearComForm(), ClearComForm.BuildForm, FormOptions.PromptInStart, entities);
            context.Call(rentForm, RentComplete);
        }

        private async Task RentComplete(IDialogContext context, IAwaitable<ClearComForm> result)
        {
            try
            {
                var form = await result;

                await context.PostAsync($"Your reservation is confirmed");

                context.Wait(MessageReceived);
            }
            catch (Exception e)
            {
                string reply;
                if (e.InnerException == null)
                {
                    reply = $"You quit --maybe you can finish next time!";
                }
                else
                {
                    reply = "Sorry, I've had a short circuit.  Please try again.";
                }
                await context.PostAsync(reply);
            }
        }
    }
}