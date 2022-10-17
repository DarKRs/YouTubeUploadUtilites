using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using YouTubeUploadUtilites;
using YouTubeUploadUtilites.Extensions;

class Program
{
	private static TelegramBotClient botClient;
	static async Task Main(string[] args)
	{
		botClient = new TelegramBotClient("5764626043:AAGozqWNEPdpYhGGUtQKTK4YnOLtm6KPD7g");

		if (!Directory.Exists(@"./Downloads/"))
			Directory.CreateDirectory(@"./Downloads/");

		using var cts = new CancellationTokenSource();

		// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
		var receiverOptions = new ReceiverOptions
		{
			AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
		};
		botClient.StartReceiving(
			Handlers.HandleUpdateAsync,
			Handlers.HandleErrorAsync,
			receiverOptions,
			cts.Token
		);


		var me = await botClient.GetMeAsync();

		Console.WriteLine($"Start listening for @{me.Username}");
		Console.ReadLine();

		// Send cancellation request to stop bot
		cts.Cancel();

		Console.ReadKey();
	}

}