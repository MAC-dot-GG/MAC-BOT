using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

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

            // COMMANDS
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
                    await e.Channel.SendMessage("All available commands are as follows:\n\n"
                                              + "**!hello** greets MAC-BOT.\n\n"
                                              //+ "!greet Username123 will send Username123 a warm greeting for you.\n"
                                              //+ "!youreNotNice will send Username123 a not-so-mean insult for you.\n"
                                              + "__Game Notification Lists__\n\n"
                                              + "**!gameLists** will show you a complete listing of Game Notification Lists available to you.\n__Note:__ __**The names displayed in this list = the names required below**__\n\n"
                                              + "**!getgl ** will display all users listed on a specific notification list.\n*Requires the name of the notification list you wish to view\n\n"
                                              + "**!glAdd ** will add you to the notification list you specified.\n*Requires the name of the notification list you wish to join\n\n"
                                              + "**!glRemove ** will remove you from the notification list you specified.\n*Requires the name of the notification list you wish to leave\n\n"
                                              + "**!glCall ** will call/tag all players in a notification list.\n*Requires the name of the notification list you wish to call\n\n"
                                              + "**!owsr ** will update your Overwatch Skill Ranking in the Competitive Overwatch notification list.\n*Requires the name of the notification list you wish to call\n\n"
                                              + "__Birthdays__\n\n"
                                              + "**!setBDay ** MAC-BOT will know your birthday!!\n*Requires your birthday in mm/dd/yyyy\n\n"
                                              + "**!unsetBDay ** I DONT LIKE IT WHEN PEOPLE WISH ME A HAPPY BIRTHDAY! SO RUDE!\n\n"
                                          );
                });

            commands.CreateCommand("adminCommands")
                .Alias("acom", "adcomm", "adcommand")
                .Description("List of all commands")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("All available ADMIN-ONLY commands are as follows:\n\n"
                                              + "__Game Notification Lists__\n\n"
                                              + "**'!glCreate'** will create a new Game Notification list and notify community\nRequires the name of the notification list you wish to create\n\n"
                                              + "__Timer Notifications__\n\n"
                                              + "**!toggleBDay** will stop/start the BDay timer. WHEN OFF, ALL BIRTHDAY MESSAGES WILL STOP.\n\n"
                                              + "**!toggleAnniv** will stop/start the Anniv timer. WHEN OFF, ALL ANNIVERSARY MESSAGES WILL STOP.\n\n"
                                              + "**!togglefilter** will stop/start the Litter Filter timer. WHEN OFF, ALL LITTER FILTER REPORTS WILL STOP.\n\n"
                                              + "__Reports__\n\n"
                                           // + "**!runFilter** will run the litter filter and spit out report.\n\n"
                                           // + "**!resetFilter** clear the filter DB, and start it over.\n\n"
                                          );
                });

            commands.CreateCommand("gameLists")
                .Alias("gl", "GL", "gls", "GLs")
                .Description("Lists all Game Notification Lists")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(getGameLists(e));
                });

            commands.CreateCommand("glAdd")
                .Alias("gladd", "GLadd")
                .Parameter("gameJoining", ParameterType.Required)
                .Description("Add yourself to a notification list")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(addToGameList(e));
                });

            commands.CreateCommand("glRemove")
                .Alias("glremove", "GLremove")
                .Parameter("gameLeaving", ParameterType.Required)
                .Description("Remove yourself from a notification list")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(removeFromGameList(e));
                });

            commands.CreateCommand("glCall")
                .Alias("letsPlay", "glcall", "lp", "letsplay")
                .Parameter("callingPlayers", ParameterType.Required)
                .Description("Call all listed players to start a party")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(callPlayersFromList(e));
                });

            commands.CreateCommand("glCreate")
                .Alias("glc", "glcreate")
                .Parameter("newName", ParameterType.Required)
                .Description("Make a new notification list for a game.")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(createNewGameList(e));
                });

            commands.CreateCommand("owsr")
                .Alias("OWsr", "OWSR")
                .Parameter("SR", ParameterType.Required)
                .Description("Record SR for OW Comp Game List")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(updateSR(e));
                });

            commands.CreateCommand("getgl")
                .Alias("getGL")
                .Parameter("gl", ParameterType.Required)
                .Description("Show list of players on a specific Game List.")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(getSpecGameList(e));
                });

            commands.CreateCommand("toggleBDayChecks")
                .Alias("toggleBDay", "togBDay")
                .Description("Start")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(toggleBDayChecks(e));
                });

            commands.CreateCommand("toggleAnnivChecks")
                .Alias("toggleAnniv", "togAnniv", "toggleAnniversaryChecks")
                .Description("Start")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(toggleAnnivChecks(e));
                });

            commands.CreateCommand("toggleFilterChecks")
                .Alias("togglefilter", "togfilter", "togfil")
                .Description("Start")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(toggleFilterChecks(e));
                });

            commands.CreateCommand("runFilter")
                .Alias("runfil")
                .Description("Start")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(runFilter(e));
                });

            commands.CreateCommand("setBDay")
                .Alias("setbday", "setbd", "setBirthday", "setbrithday")
                .Parameter("bday", ParameterType.Required)
                .Description("Set your birthday and MAC BOT will wish you a Happy Birthday!")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(setBDay(e));
                });

            commands.CreateCommand("unsetBDay")
                .Alias("unsetbday", "unsetbd", "unsetBirthday", "unsetbrithday")
                .Description("Unset your birthday. MAC BOT will no longer give a crap...")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(unsetBDay(e));
                });

            commands.CreateCommand("testDM")
                .Alias("tdm")
                .Description("test DM error message")
                .Do(async (e) =>
                {
                    await e.Server.FindUsers("MAC").First().SendMessage(testDM(e));
                });

            commands.CreateCommand("testError")
                .Alias("testerr")
                .Description("test error message")
                .Do(async (e) =>
                {
                    await e.Server.FindUsers("MAC").First().SendMessage(testError(e));
                });

            /*
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
            */

            /*
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
            */

            discord.UserJoined += async (s, e) =>
            {
                User MAC = e.Server.FindUsers("MAC").FirstOrDefault();
                var logChannel = e.Server.FindChannels("general").FirstOrDefault(); 
                var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); 

                Random rnd = new Random();
                int responseNum = rnd.Next(1, 4);

                // ADD user to litter role
                // CHANGE LATER
                await e.User.AddRoles(e.Server.FindRoles("Pleb").First());

                switch (responseNum)
                {
                    case 1:
                        await logChannel.SendMessage($"Everyone please welcome <@{e.User.Id}> into the Lion's Den! @here");
                        break;
                    case 2:
                        await logChannel.SendMessage($"Hello <@{e.User.Id}>! Welcome to the Lion's Den! @here");
                        break;
                    case 3:
                        await logChannel.SendMessage($"Add one to the litter! Welcome to the Lion's Den, <@{e.User.Id}>! @here");
                        break;
                }

                // ADD DATE TO ANNIV TABLE
                string returnString = "SUCCESS!!";
                string queryString = "INSERT INTO anniv (userid, username, anniv) VALUES(@userid, @username, @anniv)";
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(queryString, connection);

                        connection.Open();

                        command.Parameters.AddWithValue("@userid", e.User.Id.ToString());
                        command.Parameters.AddWithValue("@username", e.User.Name);
                        command.Parameters.AddWithValue("@anniv", DateTime.Today.ToShortDateString());

                        command.ExecuteNonQuery();
                    }
                    catch (Exception err)
                    {
                        await MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __discord.UserJoined__\n\nError Details:\n\n" + err.Source  + " - " + err.Message);
                        await errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __discord.UserJoined__\n\nThe same error has been send to MAC.");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                await logChannel.SendMessage(returnString);
            };

            discord.UserLeft += async (s, e) =>
            {
                User MAC = e.Server.FindUsers("MAC").FirstOrDefault();
                var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault();
                var logChannel = e.Server.FindChannels("general").FirstOrDefault(); // CHANGE THIS TO DEN STAFF
                
                string queryString = "DELETE FROM anniv WHERE userid='" + e.User.Id.ToString() + "';";
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(queryString, connection);

                        connection.Open();

                        command.ExecuteNonQuery();
                    }
                    catch (Exception err)
                    {
                        await MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __UserLeft__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                        await errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __UserLeft - anniv__\n\nThe same error has been sent to MAC.");
                        await logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                queryString = "DELETE FROM bday WHERE userid='" + e.User.Id.ToString() + "';";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(queryString, connection);

                        connection.Open();

                        command.ExecuteNonQuery();
                    }
                    catch (Exception err)
                    {
                        await MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __UserLeft__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                        await errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __UserLeft - bday__\n\nThe same error has been sent to MAC.");
                        await logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                queryString = "SELECT game FROM  gameLists";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            string newQueryString = "DELETE FROM " + reader["game"].ToString() + " WHERE userid='" + e.User.Id.ToString() + "';";
                            string newConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

                            using (SqlConnection newConnection = new SqlConnection(newConnectionString))
                            {
                                try
                                {
                                    SqlCommand newCommand = new SqlCommand(newQueryString, newConnection);

                                    newConnection.Open();

                                    newCommand.ExecuteNonQuery();
                                }
                                catch (Exception err)
                                {
                                    await MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __UserLeft__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                                    await errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __UserLeft - inside game lists__\n\nThe same error has been sent to MAC.");
                                    await logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                                }
                                finally
                                {
                                    connection.Close();
                                }
                            }
                        }

                    }
                    catch (Exception err)
                    {
                        await MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __UserLeft__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                        await errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __UserLeft - game lists__\n\nThe same error has been sent to MAC.");
                        await logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                    }
                    finally
                    {

                    }
                }

                        await logChannel.SendMessage($"@Den Staff - {e.User.Name} has left the server - **RIP :(**");
            };

            discord.UserUpdated += async (s, e) =>
            {
                User MAC = e.Server.FindUsers("MAC").FirstOrDefault();
                var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault();
                var logChannel = e.Server.FindChannels("general").FirstOrDefault(); // change later
                var hasRole = e.After.HasRole(e.Server.FindRoles("Pleb").First()); // Change Later
                if (!hasRole) return;

                if (e.After.VoiceChannel != null)
                {
                    // ADD NAME to ACTIVE LITTER DB
                    string queryString = "SELECT COUNT(*) FROM filterList WHERE userid='" + e.After.Id.ToString() + ";";
                    string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";
                    var savedCount = "0";
                    using (SqlConnection command = new SqlConnection(connectionString))
                    {
                        try
                        {
                            SqlCommand insideComd = new SqlCommand(queryString, command);
                            command.Open();
                            savedCount = insideComd.ExecuteScalar().ToString();
                        }
                        catch (Exception err)
                        {
                            await MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __UserUpdated__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                            await errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __UserUpdated__\n\nThe same error has been sent to MAC.");
                            await logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            command.Close();
                        }
                    }

                    if (savedCount.ToString() == "0")
                    {
                        // ADD NAME to ACTIVE LITTER DB
                        queryString = "INSERT INTO filterList (userid, username, lastActive) VALUES(@userid, @username, @lastActive)";
                        connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            try
                            {
                                SqlCommand command = new SqlCommand(queryString, connection);

                                connection.Open();

                                command.Parameters.AddWithValue("@userid", e.After.Id.ToString());
                                command.Parameters.AddWithValue("@username", e.After.Name);
                                command.Parameters.AddWithValue("@lastActive", DateTime.Now.ToString());

                                command.ExecuteNonQuery();
                            }
                            catch (Exception err)
                            {
                                await MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __discord.UserUpdated__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                                await errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __discord.UserUpdated__\n\nThe same error has been send to MAC.");
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
            };

            discord.MessageReceived += async (s, e) =>
            {
                User MAC = e.Server.FindUsers("MAC").FirstOrDefault();
                var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault();
                var logChannel = e.Server.FindChannels("general").FirstOrDefault(); // change later
                var hasRole = e.User.HasRole(e.Server.FindRoles("Pleb").First()); // change later
                if (!hasRole) return;

                // ADD NAME to ACTIVE LITTER DB
                string queryString = "SELECT COUNT(*) FROM filterList WHERE userid='" + e.User.Id.ToString() + ";";
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";
                var savedCount = "0";
                using (SqlConnection command = new SqlConnection(connectionString))
                {
                    try
                    {
                        SqlCommand insideComd = new SqlCommand(queryString, command);
                        command.Open();
                        savedCount = insideComd.ExecuteScalar().ToString();
                    }
                    catch (Exception err)
                    {
                        await MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __MessageRecieved__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                        await errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __MessageRecieved__\n\nThe same error has been sent to MAC.");
                        await logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        command.Close();
                    }
                }

                if (savedCount.ToString() == "0")
                {
                    // ADD NAME to ACTIVE LITTER DB
                    queryString = "INSERT INTO filterList (userid, username, lastActive) VALUES(@userid, @username, @lastActive)";
                    connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            SqlCommand command = new SqlCommand(queryString, connection);

                            connection.Open();

                            command.Parameters.AddWithValue("@userid", e.User.Id.ToString());
                            command.Parameters.AddWithValue("@username", e.User.Name);
                            command.Parameters.AddWithValue("@lastActive", DateTime.Now.ToString());

                            command.ExecuteNonQuery();
                        }
                        catch (Exception err)
                        {
                            await MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __discord.MessageReceived__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                            await errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __discord.MessageReceived__\n\nThe same error has been send to MAC.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
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

        private string getGameLists(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = e.Channel;
            string funcName = "getGameLists";
            /* END ERROR REPORTING VARIABLES */

            string queryString = "SELECT * FROM  gameLists";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

            int colWidthID = 5;
            int colWidthGame = 15;

            string returnString = "Here is a listing of all current Game Notification Lists:```\nID   Game           Players on List\n-----------------------------------";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int charsOfID = reader["id"].ToString().Length;
                        int charsOfGame = reader["game"].ToString().Length;
                        // Console.WriteLine(String.Format("{0}", reader["username"]));// etc  ...  , reader["tPatSFirstname"]
                        returnString += "\n" + reader["id"];
                        while (charsOfID < colWidthID)
                        {
                            returnString += " ";
                            charsOfID++;
                        }
                        returnString += reader["game"];
                        while (charsOfGame < colWidthGame)
                        {
                            returnString += " ";
                            charsOfGame++;
                        }

                        string insideQS = "SELECT COUNT(*) FROM " + reader["game"] + ";";
                        // Console.WriteLine(insideQS);
                        var savedCount = "0";
                        using (SqlConnection insideConn = new SqlConnection(connectionString))
                        {
                            try
                            {
                                SqlCommand insideComd = new SqlCommand(insideQS, insideConn);
                                insideConn.Open();
                                savedCount = insideComd.ExecuteScalar().ToString();
                            }
                            catch (Exception err)
                            {
                                MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                                errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nThe same error has been sent to MAC.");
                                logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                            }
                            finally
                            {
                                // Always call Close when done reading.
                                insideConn.Close();
                            }
                        }
                        returnString += savedCount;
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
                return returnString + "\n```";
            }
        }

        private string getSpecGameList(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = e.Channel;
            string funcName = "getSpecGameList";
            /* END ERROR REPORTING VARIABLES */

            string queryString = "SELECT * FROM  " + e.GetArg("gl");
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

            int colWidthID = 5;
            int colWidthUserId = 25;
            int colWidthUsername = 20;

            string returnString = "Here is a listing of all players on the " + e.GetArg("gl") + " Game Notification List:```\nID   UserId                   Username            ";
            if (e.GetArg("gl").ToUpper() == "OWCOMP")
            {
                returnString += "OW SR";
            }

            returnString += "\n-------------------------------------------------------";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                //command.Parameters.AddWithValue("@tPatSName", "Your-Parm-Value");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int charsOfID = reader["id"].ToString().Length;
                        int charsOfUserId = reader["userid"].ToString().Length;
                        int charsOfUserName = reader["username"].ToString().Length;

                        returnString += "\n" + reader["id"];
                        while (charsOfID < colWidthID)
                        {
                            returnString += " ";
                            charsOfID++;
                        }
                        /*
                        returnString += reader["userid"];
                        while (charsOfUserId < colWidthUserId)
                        {
                            returnString += " ";
                            charsOfUserId++;
                        }
                        */
                        returnString += reader["username"];
                        while (charsOfUserName < colWidthUsername)
                        {
                            returnString += " ";
                            charsOfUserName++;
                        }
                        if (e.GetArg("gl").ToUpper() == "OWCOMP")
                        {
                            returnString += reader["owsr"];
                        }
                    }
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
                return returnString + "\n```";
            }
        }

        private string addToGameList(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = e.Channel;
            string funcName = "getSpecGameList";
            /* END ERROR REPORTING VARIABLES */

            string queryString = "SELECT * FROM " + e.GetArg("gameJoining") + " WHERE userid='" + e.User.Id.ToString() + "';";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";
            string returnString = "You have successfully been ADDED TO the " + e.GetArg("gameJoining") + " notification list.";
            int flag = 0;

            // CHECK TO SEE IF THEY'RE ALREADY ADDED
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        flag = 1;
                    }
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }

            if (flag != 0)
            {
                return "You are already apart of this list! Try a different one.";
            }

            // OW COMP -- SR
            if (e.GetArg("gameJoining").ToUpper() == "OWCOMP")
            {
                e.Channel.SendMessage("For this list, you will be notified regardless of the SR of others. Also, you will not be able to call this list unless your SR is listed. If you would like us to list your SR, please type '!owsr xxxx' in the chat.");
            }

            // NEW QUERY STRING -- INSERT INTO GAME LIST
            queryString = "INSERT INTO " + e.GetArg("gameJoining") + " (userid, username) VALUES(@userid, @username)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.Parameters.AddWithValue("@userid", e.User.Id.ToString());
                    command.Parameters.AddWithValue("@username", e.User.Name);

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 2nd catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 2nd catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }

            // NEW QUERY STRING -- UPDATE GAME LIST TABLE
            queryString = "UPDATE gameLists SET [numOfPlayers] = [numOfPlayers]+1 WHERE [game]='" + e.GetArg("gameJoining") + "';";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 3rd catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 3rd catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }
            return returnString;
        }

        private string removeFromGameList(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = e.Channel;
            string funcName = "removeFromGameList";
            /* END ERROR REPORTING VARIABLES */

            string queryString = "SELECT * FROM  [dbo].[" + e.GetArg("gameLeaving") + "] WHERE userid='" + e.User.Id.ToString() + "';";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";
            string returnString = "You have successfully been REMOVED FROM the " + e.GetArg("gameLeaving") + " notification list.";
            int flag = 0;

            // CHECK TO SEE IF THEY'RE ON THE LIST
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        flag = 1;
                    }
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }

            if (flag == 0)
            {
                return "Whoops! You are currently NOT apart of this list.";
            }

            queryString = "DELETE FROM " + e.GetArg("gameLeaving") + " WHERE userid='" + e.User.Id.ToString() + "';";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 2nd catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 2nd catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }


            // NEW QUERY STRING -- UPDATE GAME LIST TABLE
            queryString = "UPDATE gameLists SET [numOfPlayers] = [numOfPlayers]-1 WHERE [game]='" + e.GetArg("gameLeaving") + "';";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 3rd catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 3rd catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }
            return returnString;
        }

        private string callPlayersFromList(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = e.Channel;
            string funcName = "callPlayersFromList";
            /* END ERROR REPORTING VARIABLES */

            // CHECK THAT SENDING USER IS ON LIST
            // IF SO, SAVE
            // IF NOT, EXIT

            Console.WriteLine(e.GetArg("callingPlayers"));

            string queryString = "SELECT * FROM " + e.GetArg("callingPlayers") + " WHERE userid = '" + e.User.Id + "'";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";
            string returnString = e.User.Name + " is calling all " + e.GetArg("callingPlayers") + " players. ";
            int sendingUserSR = 0;

            if (e.GetArg("callingPlayers").ToUpper() == "OWCOMP")
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(queryString, connection);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader["owsr"].GetType() == typeof(DBNull))
                            {
                                return "You cannot call this list unless you have updated your SR. To do this, type '!owsr xxxx'.";
                            }
                            else
                            {
                                sendingUserSR = Convert.ToInt32(reader["owsr"]);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                        errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nThe same error has been sent to MAC.");
                        logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            int SRMIN = 0;
            int SRMAX = 0;
            int SRdiff = 0;

            if (sendingUserSR != 0)
            {
                if (sendingUserSR > 3000)
                {
                    SRdiff = 500;
                }
                else
                {
                    SRdiff = 1000;
                }

                SRMIN = sendingUserSR - SRdiff;
                SRMAX = sendingUserSR + SRdiff;

                returnString = e.User.Name + " calls all " + e.GetArg("callingPlayers") + " players with an Skill Rating between " + SRMIN + " and " + SRMAX + ". ";
            }

            // GET WHOLE GAME LIST
            // COMP: IF BETWEEN THE ABOVE NUMBERS KEEP, IF NOT, SKIP
            queryString = "SELECT * FROM " + e.GetArg("callingPlayers");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        returnString += "<@" + reader["userid"] + "> ";
                    }
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 2nd catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 2nd catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }

            return returnString;
        }

        private string createNewGameList(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = e.Channel;
            string funcName = "createNewGameList";
            /* END ERROR REPORTING VARIABLES */

            // CHANGE LATER
            if (e.User.HasRole(e.Server.FindRoles("ADMIN").First()) || e.User.HasRole(e.Server.FindRoles("MOD").First()))
            { return "You are not the correct rank to run this command. Contact a staff member for help."; }

            // CREAT NEW TABLE
            string queryString = "CREATE TABLE " + e.GetArg("newName") + "(id int IDENTITY(1, 1) PRIMARY KEY, userid varchar(255) NOT NULL, username varchar(255) NOT NULL)";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";
            string returnString = "@everyone A new notification list was created for " + e.GetArg("newName") + " by " + e.User.Name + ". Add yourself now by typing '!glAdd " + e.GetArg("newName") + "'.";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }

            // ADD TO LIST OF GAME LISTS
            queryString = "INSERT INTO gameLists (game, numOfPlayers) VALUES(@game, @numOfPlayers)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.Parameters.AddWithValue("@game", e.GetArg("newName"));
                    command.Parameters.AddWithValue("@numOfPlayers", 0);

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 2nd catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 2nd catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }
            return returnString;
        }

        private string updateSR(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = e.Channel;
            string funcName = "updateSR";
            /* END ERROR REPORTING VARIABLES */

            string queryString = "UPDATE OWcomp SET [owsr] = " + e.GetArg("SR") + " WHERE userid='" + e.User.Id + "';";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";
            string returnString = "Your Overwatch SR has been updated successfully.";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }

            return returnString;
        }

        /* BIRTHDAY SET */
        private string setBDay(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = e.Channel;
            string funcName = "setBDay";
            /* END ERROR REPORTING VARIABLES */

            string returnString = "SUCCESS!!";
            string enteredBday = e.GetArg("bday");
            Regex r = new Regex("^([0]?[1-9]|[1][0-2])[/]([0]?[1-9]|[1|2][0-9]|[3][0|1])[/]([0-9]{4}|[0-9]{2})$");
            Match m = r.Match(enteredBday);
            if(!m.Success)
            {
                returnString = "Error! The date must be formatted like this mm/dd/yyyy. Try Again!";
                return returnString;
            }

            DateTime parsedBday = DateTime.Parse(enteredBday);
            string queryString = "INSERT INTO bday (userid, username, bday) VALUES(@userid, @username, @bday)";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.Parameters.AddWithValue("@userid", e.User.Id.ToString());
                    command.Parameters.AddWithValue("@username", e.User.Name);
                    command.Parameters.AddWithValue("@bday", parsedBday.ToShortDateString());

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }

            return returnString;
        }

        /* UNSET (DELETE) BDAY */
        private string unsetBDay(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = e.Server.FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = e.Channel;
            string funcName = "unsetBDay";
            /* END ERROR REPORTING VARIABLES */

            string returnString = "SUCCESS!!";
            string queryString = "DELETE FROM bday WHERE userid = '" + e.User.Id.ToString() + "';";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + " 1st catch__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }

            return returnString;
        }

        /* BIRTHDAY CHECKS */
        Timer tmrBDay = new Timer();
        private string toggleBDayChecks(CommandEventArgs e)
        {
            // CHANGE LATER
            if (e.User.HasRole(e.Server.FindRoles("ADMIN").First()) || e.User.HasRole(e.Server.FindRoles("MOD").First()))
            { return "You are not the correct rank to run this command. Contact a staff member for help."; }

            string returnString = "Birthday Checker has been turned ";
            if(tmrBDay.Enabled)
            {
                stopBDayTimer();
                returnString += "**OFF**";
            }
            else
            {
                startBDayTimer();
                returnString += "**ON**";
            }
            return returnString;
        }

        public void startBDayTimer() {
            Console.Write("BDay Timer Started.\n");
            tmrBDay.Elapsed += new ElapsedEventHandler(dailyBDayCheck);
            //tmrBDay.Interval = 5000; // FOR TESTING -- 5 SEC
            tmrBDay.Interval = 86400000; // 1 day
            tmrBDay.Enabled = true;
        }

        public void stopBDayTimer()
        {
            Console.Write("BDay Timer Stopped.\n");
            tmrBDay.Enabled = false;
        }

        public void dailyBDayCheck(object source, ElapsedEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = discord.FindServers("MAC").First().FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = discord.FindServers("MAC").First().FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = discord.FindServers("MAC").First().FindChannels("general").FirstOrDefault();
            string funcName = "dailyBDayCheck";
            /* END ERROR REPORTING VARIABLES */

            Console.Write("Daily Birthday Check ran at " + DateTime.Now.ToLongTimeString() + "\n");
            string queryString = "SELECT * FROM bday";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToDateTime(reader["bday"].ToString().Trim()).Month == DateTime.Today.Month
                         && Convert.ToDateTime(reader["bday"].ToString().Trim()).Day == DateTime.Today.Day)
                        {
                            logChannel.SendMessage($"@here Today is <@{reader["userid"].ToString().Trim()}>'s birthday!\n\n  :sparkler:  :sparkler:  HAPPY BIRTHDAY!!  :sparkler:  :sparkler:  ");
                        }
                    }
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /* ANNIVERSARY CHECKS */
        Timer tmrAnniv = new Timer();
        private string toggleAnnivChecks(CommandEventArgs e)
        {
            // CHANGE LATER
            if (e.User.HasRole(e.Server.FindRoles("ADMIN").First()) || e.User.HasRole(e.Server.FindRoles("MOD").First()))
            { return "You are not the correct rank to run this command. Contact a staff member for help."; }

            string returnString = "Anniversary Checker has been turned ";
            if (tmrAnniv.Enabled)
            {
                stopAnnivTimer();
                returnString += "**OFF**";
            }
            else
            {
                startAnnivTimer();
                returnString += "**ON**";
            }
            return returnString;
        }

        public void startAnnivTimer()
        {
            Console.Write("Anniversary Timer Started.\n");
            tmrAnniv.Elapsed += new ElapsedEventHandler(dailyAnnivCheck);
            //tmrAnniv.Interval = 5000; // FOR TESTING -- 5 SEC
            tmrAnniv.Interval = 86400000; // 1 day
            tmrAnniv.Enabled = true;
        }

        public void stopAnnivTimer()
        {
            Console.Write("Anniversary Timer Stopped.\n");
            tmrAnniv.Enabled = false;
        }

        public void dailyAnnivCheck(object source, ElapsedEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = discord.FindServers("MAC").First().FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = discord.FindServers("MAC").First().FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = discord.FindServers("MAC").First().FindChannels("general").FirstOrDefault();
            string funcName = "dailyAnnivCheck";
            /* END ERROR REPORTING VARIABLES */

            Console.Write("Daily Anniversary Check ran at " + DateTime.Now.ToLongTimeString() + "\n");
            string queryString = "SELECT * FROM anniv";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["anniv"].ToString().Trim() == DateTime.Today.ToShortDateString())
                        {
                            int annivYear = Convert.ToDateTime(reader["anniv"].ToString().Trim()).Year;
                            int currentYear = DateTime.Today.Year;
                            int numOfYears = currentYear - annivYear;
                            logChannel.SendMessage($"@here Today is <@{reader["userid"].ToString().Trim()}>'s {numOfYears}-year anniversary at the Lion's Den!\n\n  :champagne:  :champagne_glass:  HAPPY ANNIVERSARY!!  :champagne_glass:  :champagne:  ");
                        }
                    }
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /* FILTER LITTER CHECKS */
        Timer tmrFilter = new Timer();

        private string toggleFilterChecks(CommandEventArgs e)
        {
            // CHANGE LATER
            if (e.User.HasRole(e.Server.FindRoles("ADMIN").First()) || e.User.HasRole(e.Server.FindRoles("MOD").First()))
            { return "You are not the correct rank to run this command. Contact a staff member for help."; }

            string returnString = "Filter Checker has been turned ";
            if (tmrFilter.Enabled)
            {
                stopFilterTimer();
                returnString += "**OFF**";
            }
            else
            {
                startFilterTimer();
                returnString += "**ON**";
            }
            return returnString;
        }

        public void startFilterTimer()
        {
            Console.Write("Filter Timer Started.\n");
            tmrFilter.Elapsed += new ElapsedEventHandler(weeklyFilterCheck);
            tmrFilter.Interval = 5000; // FOR TESTING -- 5 SEC
            //tmrFilter.Interval = 604800000; // 1 week
            tmrFilter.Enabled = true;
        }

        public void stopFilterTimer()
        {
            Console.Write("Anniversary Timer Stopped.\n");
            tmrFilter.Enabled = false;
        }

        public void weeklyFilterCheck(object source, ElapsedEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = discord.FindServers("MAC").First().FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = discord.FindServers("MAC").First().FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = discord.FindServers("MAC").First().FindChannels("general").FirstOrDefault();
            string funcName = "dailyAnnivCheck";
            /* END ERROR REPORTING VARIABLES */

            Console.Write("Weekly Filter Check ran at " + DateTime.Now.ToLongTimeString() + "\n");
            string queryString = "SELECT * FROM filterList";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";
            string returnString = "The following Users are in the role 'Litter' AND did **NOT** access the Lion's Den this week: \n";
            var thisServer = discord.FindServers("MAC").First(); // change later
            List<string> activeLitter = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        activeLitter.Add(reader["username"].ToString());
                    }
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }

            string[] activeLitterArray = activeLitter.ToArray();
            Console.WriteLine(activeLitterArray[0].Trim());

            foreach (User aUser in thisServer.Users)
            {
                //if (aUser.HasRole(thisServer.FindRoles("Pleb").First())) // change later
                //{
                    bool found = false;
                    for(int i = 0; activeLitterArray.Length > i; i++)
                    {
                        if((string)activeLitterArray[i].Trim() == aUser.Name)
                        {
                            found = true;
                        }
                    }
                    if(!found)
                    {
                        returnString += aUser.Name + "\n";
                    }
                //}
            }
            logChannel.SendMessage(returnString);
        }

        private string runFilter(CommandEventArgs e)
        {
            User MAC = e.Server.FindUsers("MAC").First(); // Find Me for Error Reporting
            return "Under Contruction";
        }

        private string testDM(CommandEventArgs e)
        {
            return "test";
        }

        private string testError(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = discord.FindServers("MAC").First().FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = discord.FindServers("MAC").First().FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = discord.FindServers("MAC").First().FindChannels("general").FirstOrDefault();
            string funcName = "dailyAnnivCheck";
            /* END ERROR REPORTING VARIABLES */

            try
            {
                throw new DivideByZeroException();
            }
            catch (Exception err)
            {
                MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __testError__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __testError__\n\nThe same error has been sent to MAC.");
                logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
            }
            return "Error Sent";
        }

        private string lastFilterReset(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = discord.FindServers("MAC").First().FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = discord.FindServers("MAC").First().FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = discord.FindServers("MAC").First().FindChannels("general").FirstOrDefault();
            string funcName = "lastFilterReset";
            /* END ERROR REPORTING VARIABLES */

            string queryString = "SELECT lastActive FROM filterList;";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";
            string LFRdate = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        if(reader["lastActive"] == DBNull.Value)
                        {
                            LFRdate = "No entries yet.";
                        }
                        else
                        {
                            LFRdate =  reader["lastActive"].ToString();
                        }
                    }
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }
            return LFRdate;
        }

        private string resetFilter(CommandEventArgs e)
        {
            /* ERROR REPORTING VARIABLES */
            User MAC = discord.FindServers("MAC").First().FindUsers("MAC").First(); // Find Me for Error Reporting
            var errLogChannel = discord.FindServers("MAC").First().FindChannels("den_staff").FirstOrDefault(); // den staff error reporting
            var logChannel = discord.FindServers("MAC").First().FindChannels("general").FirstOrDefault();
            string funcName = "resetFilter";
            /* END ERROR REPORTING VARIABLES */

            string queryString = "TRUNCATE TABLE filterList;";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\MAC\\Documents\\LionsDen\\MAC-BOT\\MAC-BOT\\MAC-BOT\\LionsDen.mdf;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    MAC.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nError Details:\n\n" + err.Source + " - " + err.Message);
                    errLogChannel.SendMessage("**MAC-BOT ERROR** Error thrown in __" + funcName + "__\n\nThe same error has been sent to MAC.");
                    logChannel.SendMessage("**Uh oh... Something went wrong.**\n\nDen Staff has been notified, and will be in touch regarding the issue. Please avoid using this command again until the issue can be addressed. Thank you!");
                }
                finally
                {
                    connection.Close();
                }
            }
            return "Filter successfully reset!";
        }

        // NOTES
        // VenetianLion userid = <@192684191454986240>
    }
}
