using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAC_BOT
{
    class MyBot
    {
        DiscordClient discord;

        public MyBot()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            var commands = discord.GetService<CommandService>();

            commands.CreateCommand("hello")
                .Alias("hi", "Hi", "Hello")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Heyo! Wanna see what I can do? Type '!commands'");
                });

            commands.CreateCommand("commands")
                .Alias("com", "comm", "command")
                .Description("List of all commands")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("'!hello' greets MAC-BOT.\n"
                                              + "'!greet Username123' will send Username123 a warm greeting for you.\n"
                                              + "'!youreNotNice Username123' will send Username123 a not-so-mean insult for you.\n"
                                          );
                });

            commands.CreateCommand("greet")
                .Alias("gr", "gret")
                .Parameter("GreetedPerson", ParameterType.Required)
                .Do(async (e) =>
                {
                    var logChannel = e.Server.FindChannels("general").FirstOrDefault();

                    Random rnd = new Random();
                    int responseNum = rnd.Next(1, 8);

                    switch (responseNum)
                    {
                        case 1:
                            await logChannel.SendMessage($"**{e.User.Name}** extends a firm :handshake::skin-tone-3: *hand shake* :handshake::skin-tone-4: to **{e.GetArg("GreetedPerson")}**");
                            break;
                        case 2:
                            await logChannel.SendMessage($"**{e.User.Name}** raises his glass to **{e.GetArg("GreetedPerson")}** :beer: :beer:");
                            break;
                        case 3:
                            await logChannel.SendMessage($"**{e.User.Name}** tips his elegant :tophat: *tophat* :tophat: to **{e.GetArg("GreetedPerson")}**");
                            break;
                        case 4:
                            await logChannel.SendMessage($"**{e.User.Name}** shares a loud :raised_hand::skin-tone-2: *high five* :raised_hand::skin-tone-5: with **{e.GetArg("GreetedPerson")}**");
                            break;
                        case 5:
                            await logChannel.SendMessage($"**{e.User.Name}** gives a nice :wave::skin-tone-3: *wave* :wave::skin-tone-1: to **{e.GetArg("GreetedPerson")}**");
                            break;
                        case 6:
                            await logChannel.SendMessage($"**{e.User.Name}** gives a :sunglasses: *cool-guy head nod* :sunglasses: to **{e.GetArg("GreetedPerson")}**");
                            break;
                        case 7:
                            await logChannel.SendMessage($"**{e.User.Name}** throws a :tada: *party* :tada: for **{e.GetArg("GreetedPerson")}**");
                            break;
                    }
                });

            commands.CreateCommand("youreNotNice")
                .Alias("sls", "saltlesssalt, yourenotnice", "ynn")
                .Parameter("NotNicePerson", ParameterType.Required)
                .Do(async (e) =>
                {
                    var logChannel = e.Server.FindChannels("general").FirstOrDefault();

                    Random rnd = new Random();
                    int responseNum = rnd.Next(1, 7);

                    switch (responseNum)
                    {
                        case 1:
                            await logChannel.SendMessage($"**{e.User.Name}** is :spy::skin-tone-5: *secretly plotting* :spy::skin-tone-2: against **{e.GetArg("NotNicePerson")}**");
                            break;
                        case 2:
                            await logChannel.SendMessage($"**{e.User.Name}** :man_dancing::skin-tone-4: *dances* :man_dancing::skin-tone-1: around **{e.GetArg("NotNicePerson")}**");
                            break;
                        case 3:
                            await logChannel.SendMessage($"**{e.User.Name}** releases the :bear: *bears* :bear: on  **{e.GetArg("NotNicePerson")}**");
                            break;
                        case 4:
                            await logChannel.SendMessage($"**{e.User.Name}** :zap: *zaps* :zap: **{e.GetArg("NotNicePerson")}**");
                            break;
                        case 5:
                            await logChannel.SendMessage($"**{e.User.Name}** launches :bomb: *bombs* :bomb: at **{e.GetArg("NotNicePerson")}**");
                            break;
                        case 6:
                            await logChannel.SendMessage($"**{e.User.Name}** hurls :tomato: tomatoes :tomato: towards **{e.GetArg("NotNicePerson")}**");
                            break;
                    }
                });

            discord.UserJoined += async (s, e) => {
                var logChannel = e.Server.FindChannels("general").FirstOrDefault();

                Random rnd = new Random();
                int responseNum = rnd.Next(1, 4);
                
                switch(responseNum)
                {
                    case 1:
                        await logChannel.SendMessage($"Everyone please welcome {e.User.Name} into the Lion's Den!");
                        break;
                    case 2:
                        await logChannel.SendMessage($"Hello {e.User.Name}! Welcome to the Lion's Den!");
                        break;
                    case 3:
                        await logChannel.SendMessage($"Add one to the litter! Welcome to the Lion's Den, {e.User.Name}!");
                        break;
                }
                
            };

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MjIwMzE5ODg4MzQ1MDA2MDkx.CzyF_g.OSBaYbXEcYlHUPUc0Qy50FNmN80", TokenType.Bot);
            });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
