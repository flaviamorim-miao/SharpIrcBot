﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SharpIrcBot;
using SharpIrcBot.Events.Irc;

namespace Sockpuppet
{
    public class SockpuppetPlugin : IPlugin, IReloadableConfiguration
    {
        private static readonly ILogger Logger = SharpIrcBotUtil.LoggerFactory.CreateLogger<SockpuppetPlugin>();

        protected IConnectionManager ConnectionManager { get; }
        protected SockpuppetConfig Config { get; set; }

        public SockpuppetPlugin(IConnectionManager connMgr, JObject config)
        {
            ConnectionManager = connMgr;
            Config = new SockpuppetConfig(config);

            ConnectionManager.QueryMessage += HandleQueryMessage;
        }

        public void ReloadConfiguration(JObject newConfig)
        {
            Config = new SockpuppetConfig(newConfig);
        }

        protected string VerifyIdentity(IUserMessageEventArgs message)
        {
            var username = message.SenderNickname;
            if (Config.UseIrcServices)
            {
                username = ConnectionManager.RegisteredNameForNick(username);
                if (username == null)
                {
                    Logger.LogInformation("{0} is not logged in; ignoring", message.SenderNickname);
                    return null;
                }
            }

            if (!Config.Puppeteers.Contains(username))
            {
                Logger.LogInformation("{0} is not a puppeteer; ignoring", username);
                return null;
            }

            return username;
        }

        protected void PerformSockpuppet(string username, string nick, string message)
        {
            var command = message.Substring("!sockpuppet ".Length);

            var unescapedCommand = SharpIrcBotUtil.UnescapeString(command);
            if (unescapedCommand == null)
            {
                Logger.LogInformation("{0} bollocksed up their escapes; ignoring", username);
                return;
            }

            Logger.LogInformation("{0} (nick: {1}) issued the following command: {2}", username, nick, command);

            ConnectionManager.SendRawCommand(unescapedCommand);
            ConnectionManager.SendQueryMessage(nick, "OK");
        }

        protected void HandleQueryMessage(object sender, IPrivateMessageEventArgs args, MessageFlags flags)
        {
            if (flags.HasFlag(MessageFlags.UserBanned))
            {
                return;
            }

            if (args.SenderNickname == ConnectionManager.MyNickname)
            {
                return;
            }

            if (args.Message.StartsWith("!sockpuppet "))
            {
                string username = VerifyIdentity(args);
                if (username == null)
                {
                    return;
                }
                PerformSockpuppet(username, args.SenderNickname, args.Message);
                return;
            }

            if (args.Message == "!reload")
            {
                string username = VerifyIdentity(args);
                if (username == null)
                {
                    return;
                }
                ConnectionManager.ReloadConfiguration();
                ConnectionManager.SendQueryMessage(args.SenderNickname, "OK");
                return;
            }
        }
    }
}
