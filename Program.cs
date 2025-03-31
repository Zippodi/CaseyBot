using System;
using System.Threading;
using System.Threading.Tasks;
using CaseyBot.commands;
using CaseyBot.config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using Newtonsoft.Json;

namespace CaseyBot
{
    class Program
    {
        public static DiscordClient Client { get; set; }

        //public static DiscordGuild Guild { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);

            var voiceConfig = new VoiceNextConfiguration
            {
                EnableIncoming = true
            };
            var voice = Client.UseVoiceNext(voiceConfig);

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            //DiscordChannel channel;
            //VoiceNextConnection connection = await channel.ConnectAsync();

            Client.Ready += Client_Ready;
            Client.MessageCreated += MessageCreatedHandler;
            Client.VoiceStateUpdated += VoiceChannelHandler;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<BasicCommands>();

            await Client.ConnectAsync();
            await Task.Delay(-1);

        }

        private static async Task VoiceChannelHandler(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            if (e.Before == null && e.Channel.Name == "General")
            {
                await e.Channel.SendMessageAsync($"{e.User.Username} has joined the channel!");
            }
        }

        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (!e.Message.Author.IsBot)
            {
                await e.Channel.SendMessageAsync("This event was triggered");

            }
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
