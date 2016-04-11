﻿using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Newtonsoft.Json.Linq;
using SharpIrcBot;
using SharpIrcBot.Events.Irc;

namespace GroupPressure
{
    /// <summary>
    /// Submit to group pressure: if enough people say a specific thing in the last X messages,
    /// join in on the fray!
    /// </summary>
    public class GroupPressurePlugin : IPlugin, IReloadableConfiguration
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Queue<BacklogMessage> Backlog;
        protected PressureConfig Config;
        protected IConnectionManager Connection;

        public GroupPressurePlugin(IConnectionManager connMgr, JObject config)
        {
            Backlog = new Queue<BacklogMessage>();
            Config = new PressureConfig(config);
            Connection = connMgr;

            Connection.ChannelMessage += HandleChannelMessage;
            Connection.ChannelAction += HandleChannelAction;
        }

        public void ReloadConfiguration(JObject newConfig)
        {
            Config = new PressureConfig(newConfig);
        }

        private void HandleChannelMessage(object sender, IChannelMessageEventArgs args, MessageFlags flags)
        {
            try
            {
                ActuallyHandleChannelMessageOrAction(sender, args, flags, action: false);
            }
            catch (Exception exc)
            {
                Logger.Error("error handling message", exc);
            }
        }

        private void HandleChannelAction(object sender, IChannelMessageEventArgs args, MessageFlags flags)
        {
            try
            {
                ActuallyHandleChannelMessageOrAction(sender, args, flags, action: true);
            }
            catch (Exception exc)
            {
                Logger.Error("error handling action", exc);
            }
        }

        private void ActuallyHandleChannelMessageOrAction(object sender, IChannelMessageEventArgs e, MessageFlags flags, bool action)
        {
            if (flags.HasFlag(MessageFlags.UserBanned))
            {
                return;
            }

            var body = e.Message;
            if (body.Length == 0)
            {
                return;
            }

            if (!Config.Channels.Contains(e.Channel))
            {
                return;
            }

            // clean out the backlog
            while (Backlog.Count > Config.BacklogSize)
            {
                Backlog.Dequeue();
            }

            var normalizedSender = Connection.RegisteredNameForNick(e.SenderNickname) ?? e.SenderNickname;

            // append the message
            Backlog.Enqueue(new BacklogMessage
            {
                Sender = normalizedSender,
                Body = body,
                Action = action
            });

            // perform accounting
            var messageToSenders = new Dictionary<string, HashSet<string>>();
            foreach (var backMessage in Backlog)
            {
                var actualBody = (backMessage.Action ? 'A' : 'M') + backMessage.Body;
                if (backMessage.Sender == Connection.MyNickname)
                {
                    // this is my message -- start counting from zero, so to speak
                    messageToSenders[actualBody] = new HashSet<string>();
                }
                else
                {
                    if (!messageToSenders.ContainsKey(actualBody))
                    {
                        messageToSenders[actualBody] = new HashSet<string>();
                    }
                    messageToSenders[actualBody].Add(backMessage.Sender);
                }
            }

            foreach (var messageAndSenders in messageToSenders)
            {
                var msg = messageAndSenders.Key;
                var senders = messageAndSenders.Value;
                if (senders.Count < Config.TriggerCount)
                {
                    continue;
                }

                Logger.DebugFormat(
                    "bowing to the group pressure of ({0}) sending {1}",
                    string.Join(", ", senders.Select(s => SharpIrcBotUtil.LiteralString(s))),
                    SharpIrcBotUtil.LiteralString(msg)
                );

                // submit to group pressure
                bool lastAction;
                if (msg[0] == 'A')
                {
                    Connection.SendChannelAction(e.Channel, msg.Substring(1));
                    lastAction = true;
                }
                else
                {
                    Connection.SendChannelMessage(e.Channel, msg.Substring(1));
                    lastAction = false;
                }

                // fake this message into the backlog to prevent duplicates
                Backlog.Enqueue(new BacklogMessage
                {
                    Sender = Connection.MyUsername,
                    Body = body,
                    Action = lastAction
                });
            }
        }
    }
}
