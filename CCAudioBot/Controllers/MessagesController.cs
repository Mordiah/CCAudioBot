using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using CCAudioBot.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using CCAudioBot.Models;

namespace CCAudioBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                JoinOrLeaveAChannel joinOrLeave = await ParseUserInput(activity.Text);
                bool JoinLeave = false;
                string ChannelToUse = string.Empty;
                string MemberToUse = string.Empty;
                if (joinOrLeave.intents.Count() > 0)
                {
                    switch (joinOrLeave.intents[0].intent)
                    {
                        case "JoinOrLeaveAChannel":
                            foreach (Models.Entity entity in joinOrLeave.entities)
                            {
                                switch (entity.type)
                                {
                                    case "JoinLeave":
                                        JoinLeave = !((entity.entity.ToLower() == "remove") || (entity.entity.ToLower() == "leave"));
                                        break;
                                    case "ChannelToUse":
                                        ChannelToUse = entity.entity;
                                        break;
                                    case "MemberToUse":
                                        MemberToUse = entity.entity;
                                        break;
                                }
                            }
                            break;
                    }
                }

                // return our reply to the user
                Activity reply = activity.CreateReply($"You want { MemberToUse } to { JoinLeave } channel { ChannelToUse } ");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        public async Task<JoinOrLeaveAChannel> ParseUserInput(string strInput)
        {
            string retString = string.Empty;
            string strEscaped = Uri.EscapeDataString(strInput);

            using (var client = new HttpClient())
            {
                string uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/2c854884-394b-424f-9cc6-f660d81915b8?subscription-key=0091c7f5c04542249870c45525fe1fce&staging=true&verbose=true&timezoneOffset=0.0&q=" + strEscaped;
                HttpResponseMessage msg = await client.GetAsync(uri);

                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<JoinOrLeaveAChannel>(jsonResponse);

                    return data;
                }
            }

            return null;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}