using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class EchoDialog : IDialog<object>
{
    protected int count = 1;

    public Task StartAsync(IDialogContext context)
    {
        try
        {
            context.Wait(MessageReceivedAsync);
        }
        catch (OperationCanceledException error)
        {
            return Task.FromCanceled(error.CancellationToken);
        }
        catch (Exception error)
        {
            return Task.FromException(error);
        }

        return Task.CompletedTask;
    }

    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        if (message.Text == "reset")
        {
            PromptDialog.Confirm(
                context,
                AfterResetAsync,
                "Are you sure you want to reset the count?",
                "Didn't get that!",
                promptStyle: PromptStyle.Auto);
        }
        if (message.Text == "foo")
        {
            string worriedFace = "\U0001F61F";
            string smilingFace = "\U0001F642";

            await context.PostAsync("I'm sorry, I didn't get that " + worriedFace + '.');
            await context.PostAsync("Here are some things I know how to talk about!" + smilingFace);

            var message2 = context.MakeMessage();

            var receiptCard = new ReceiptCard
            {
                Title = "John Doe",
                Facts = new List<Fact> { new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 5555-****") },
                Items = new List<ReceiptItem>
            {
                new ReceiptItem("Data Transfer", price: "$ 38.45", quantity: "368", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png")),
                new ReceiptItem("App Service", price: "$ 45.00", quantity: "720", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png")),
            },
                Tax = "$ 7.50",
                Total = "$ 90.95",
                Buttons = new List<CardAction>
            {
                new CardAction(
                    ActionTypes.OpenUrl,
                    "More information",
                    "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png",
                    "https://azure.microsoft.com/en-us/pricing/")
            }
            };

            message2.Attachments = new List<Attachment>();
            message2.Attachments.Add(receiptCard.ToAttachment());

            await context.PostAsync(message2);
			context.Wait(MessageReceivedAsync);
		}
        else
        {
            await context.PostAsync($"{this.count++}: You said {message.Text}");
            context.Wait(MessageReceivedAsync);
        }
    }

    public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
    {
        var confirm = await argument;
        if (confirm)
        {
            this.count = 1;
            await context.PostAsync("Reset count.");
        }
        else
        {
            await context.PostAsync("Did not reset count.");
        }
        context.Wait(MessageReceivedAsync);
    }
}