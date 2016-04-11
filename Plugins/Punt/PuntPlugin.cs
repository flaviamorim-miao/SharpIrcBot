﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using Newtonsoft.Json.Linq;
using SharpIrcBot;
using SharpIrcBot.Events.Irc;

namespace Punt
{
    public class PuntPlugin : IPlugin, IReloadableConfiguration
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected IConnectionManager ConnectionManager { get; }
        protected PuntConfig Config { get; set; }
        protected Dictionary<string, Regex> RegexCache { get; }
        protected Random Randomizer { get; }

        public PuntPlugin(IConnectionManager connMgr, JObject config)
        {
            ConnectionManager = connMgr;
            Config = new PuntConfig(config);
            RegexCache = new Dictionary<string, Regex>(2 * Config.CommonPatterns.Count);
            Randomizer = new Random();

            ConnectionManager.ChannelAction += HandleChannelAction;
            ConnectionManager.ChannelMessage += HandleChannelMessage;
            ConnectionManager.ChannelNotice += HandleChannelNotice;
        }

        public void ReloadConfiguration(JObject newConfig)
        {
            Config = new PuntConfig(newConfig);
            RegexCache.Clear();
        }

        protected void HandleChannelAction(object sender, IChannelMessageEventArgs e, MessageFlags flags)
        {
            try
            {
                ActuallyHandleMessage(e.Channel, e.SenderNickname, e.Message);
            }
            catch (Exception exc)
            {
                Logger.Error("error handling channel action", exc);
            }
        }

        protected void HandleChannelMessage(object sender, IChannelMessageEventArgs e, MessageFlags flags)
        {
            try
            {
                ActuallyHandleMessage(e.Channel, e.SenderNickname, e.Message);
            }
            catch (Exception exc)
            {
                Logger.Error("error handling channel message", exc);
            }
        }

        protected void HandleChannelNotice(object sender, IChannelMessageEventArgs e, MessageFlags flags)
        {
            try
            {
                ActuallyHandleMessage(e.Channel, e.SenderNickname, e.Message);
            }
            catch (Exception exc)
            {
                Logger.Error("error handling channel notice", exc);
            }
        }

        protected static TValue FetchOrMakeAndStore<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueDerivatorIfNotFound)
        {
            try
            {
                return dict[key];
            }
            catch (KeyNotFoundException)
            {
                var ret = valueDerivatorIfNotFound(key);
                dict[key] = ret;
                return ret;
            }
        }

        protected virtual void ActuallyHandleMessage(string channel, string nick, string body)
        {
            if (!Config.ChannelsPatterns.ContainsKey(channel))
            {
                // don't police this channel
                return;
            }

            var relevantPatterns = Config.CommonPatterns
                .Concat(Config.ChannelsPatterns[channel]);
            foreach (var pattern in relevantPatterns)
            {
                var nickRegex = FetchOrMakeAndStore(RegexCache, pattern.NickPattern, r => new Regex(r));
                var bodyRegex = FetchOrMakeAndStore(RegexCache, pattern.BodyPattern, r => new Regex(r));

                var normalizedNick = ConnectionManager.RegisteredNameForNick(nick);

                if (!nickRegex.IsMatch(nick) && (normalizedNick == null || !nickRegex.IsMatch(normalizedNick)))
                {
                    // wrong user
                    continue;
                }

                if (pattern.ChancePercent.HasValue)
                {
                    var val = Randomizer.Next(100);
                    if (val >= pattern.ChancePercent.Value)
                    {
                        // luck is on their side
                        continue;
                    }
                }

                if (bodyRegex.IsMatch(body))
                {
                    // match! kick 'em!
                    ConnectionManager.KickChannelUser(channel, nick, pattern.KickMessage);
                    return;
                }
            }
        }
    }
}
