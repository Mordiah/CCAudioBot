using CCAudioBot.IVR;
using Microsoft.Bot.Builder.Calling;
using Microsoft.Bot.Connector;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CCAudioBot.Controllers
{
    // Only enable authentication once you've configured your bot in the Bot Framework Portal and 
    // have set the BotId, MicrosoftAppId and MicrosoftAppPassword in the Web.Config for your bot 
    [BotAuthentication]
    [RoutePrefix("api/calling")]
    public class CallingController : ApiController
    {
        public CallingController()
              : base()
        {
            CallingConversation.RegisterCallingBot(c => new ClearComCallingBot(c));
        }

        // Callback route for Skype calling events.
        // Make sure to set the Microsoft.Bot.Builder.Calling.CallbackUrl in web.config
        [Route("callback")]
        public async Task<HttpResponseMessage> ProcessCallingEventAsync()
        {
            return await CallingConversation.SendAsync(Request, CallRequestType.CallingEvent);
        }

        // Route for incoming call events
        [Route("call")]
        public async Task<HttpResponseMessage> ProcessIncomingCallAsync()
        {
            return await CallingConversation.SendAsync(Request, CallRequestType.IncomingCall);
        }
    }
}