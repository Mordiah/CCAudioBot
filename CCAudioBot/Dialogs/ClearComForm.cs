using System;
using Microsoft.Bot.Builder.FormFlow;
using Chronic;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CCAudioBot.Dialogs
{
    [Serializable]
    public class ClearComForm
    {
        private string memberName;
        private string channelName;
        private string joinOrLeave;

        [Prompt("Whos membership do you want to change?")]
        public string MemberName
        {
            get { return this.memberName; }
            set { this.memberName = value; }
        }
        
        [Prompt("Which channel would you like to change?")]
        public string ChannelName
        {
            get { return this.channelName; }
            set { this.channelName = value; }
        }

        public string JoinOrLeave
        {
            get { return this.joinOrLeave; }
            set { this.joinOrLeave = value; }
        }

        public static IForm<ClearComForm> BuildForm()
        {
            FormBuilder<ClearComForm> clearComForm = new FormBuilder<ClearComForm>();

            return clearComForm
                .Message("How can I help you")
                .AddRemainingFields()
                .OnCompletion(async (session, ClearComForm) =>
                {
                    await Task.Run(() => { Debug.WriteLine("{0}", ClearComForm); });
                })
                .Build();
        }
    }
}