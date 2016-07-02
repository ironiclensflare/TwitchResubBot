using System;
using System.Configuration;
using ChatSharp.Events;
using log4net;
using log4net.Config;
using TwitchBot;

namespace TwitchResubBot
{
    class Program
    {
        static void Main()
        {
            XmlConfigurator.Configure();
            var log = LogManager.GetLogger("TwitchResubBot");
            var settingsReader = new AppSettingsReader();
            var resubLogPath = settingsReader.GetValue("ResubOutputFilepath", typeof(string)).ToString();
            var username = settingsReader.GetValue("Username", typeof(string)).ToString();
            var token = settingsReader.GetValue("OAuthToken", typeof(string)).ToString();
            var channel = settingsReader.GetValue("Channel", typeof(string)).ToString();
            if (string.IsNullOrEmpty(resubLogPath)) throw new ArgumentException("Path to resub log file was not found.  Please check the App.config.");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Username was not found.  Please check the App.config.");
            if (string.IsNullOrEmpty(token)) throw new ArgumentException("OAuth token was not found.  Please check the App.config.");
            if (string.IsNullOrEmpty(channel)) throw new ArgumentException("Channel name was not found.  Please check the App.config.");

            var resubLogger = new TextLogger(resubLogPath);
            log.Info("Application started.");

            var bot = new TwitchClient("irc.chat.twitch.tv", username, token, channel);
            bot.BotListening += (sender, eventArgs) => { log.Info("Bot is now listening for resubs."); };
            bot.UserSubbed += (sender, eventArgs) =>
            {
                log.Info($"{eventArgs.Username} has just subscribed!");
                resubLogger.WriteLine($"{eventArgs.Username} (new subscriber)");
            };
            bot.UserResubbed += (sender, eventArgs) =>
            {
                log.Info($"{eventArgs.Username} subscribed for {eventArgs.Months} months! - {eventArgs.Message}");
                resubLogger.WriteLine($"{eventArgs.Username} ({eventArgs.Months} months)");
            };

            bot.Connect();

            var exit = false;
            while (!exit)
            {
                var input = Console.ReadKey();
                if (input.KeyChar.Equals('q')) { exit = true; }
            }
        }
    }
}
