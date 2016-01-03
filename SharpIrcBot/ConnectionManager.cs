﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using Meebey.SmartIrc4net;
using Timer = System.Timers.Timer;

namespace SharpIrcBot
{
    public class ConnectionManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static int MaxMessageLength = 230;

        public BotConfig Config;
        public readonly IrcClient Client;
        public readonly TimerTrigger Timers;

        protected Thread IrcThread;
        protected CancellationTokenSource Canceller;
        protected HashSet<string> SyncedChannels;

        public event SharpIrcBotEventHandler<IrcEventArgs> ChannelMessage;
        public event SharpIrcBotEventHandler<ActionEventArgs> ChannelAction;
        public event SharpIrcBotEventHandler<IrcEventArgs> ChannelNotice;
        public event SharpIrcBotEventHandler<IrcEventArgs> QueryMessage;
        public event SharpIrcBotEventHandler<ActionEventArgs> QueryAction;
        public event SharpIrcBotEventHandler<IrcEventArgs> QueryNotice;
        public event EventHandler<EventArgs> ConnectedToServer;
        public event EventHandler<NickMappingEventArgs> NickMapping;
        public event EventHandler<IrcEventArgs> RawMessage;
        public event EventHandler<NamesEventArgs> NamesInChannel;
        public event EventHandler<JoinEventArgs> JoinedChannel;
        public event EventHandler<NickChangeEventArgs> NickChange;
        public event EventHandler<PartEventArgs> UserLeftChannel;
        public event EventHandler<QuitEventArgs> UserQuitServer;
        public event EventHandler<OutgoingMessageEventArgs> OutgoingChannelMessage;
        public event EventHandler<OutgoingMessageEventArgs> OutgoingChannelAction;
        public event EventHandler<OutgoingMessageEventArgs> OutgoingChannelNotice;
        public event EventHandler<OutgoingMessageEventArgs> OutgoingQueryMessage;
        public event EventHandler<OutgoingMessageEventArgs> OutgoingQueryAction;
        public event EventHandler<OutgoingMessageEventArgs> OutgoingQueryNotice;

        public ConnectionManager(BotConfig config)
        {
            SyncedChannels = new HashSet<string>();

            Config = config;
            Client = new IrcClient
            {
                UseSsl = Config.UseTls,
                ValidateServerCertificate = Config.VerifyTlsCertificate,
                AutoReconnect = false,
                AutoRejoin = false,
                AutoRelogin = false,
                Encoding = Encoding.GetEncoding(Config.Encoding),
                SendDelay = Config.SendDelay,
                SupportNonRfc = true,
                ActiveChannelSyncing = true
            };
            Client.OnCtcpRequest += HandleCtcpRequest;
            Client.OnChannelMessage += HandleChannelMessage;
            Client.OnChannelAction += HandleChannelAction;
            Client.OnChannelNotice += HandleChannelNotice;
            Client.OnChannelActiveSynced += HandleChannelSynced;
            Client.OnRawMessage += HandleRawMessage;
            Client.OnNames += HandleNames;
            Client.OnJoin += HandleJoin;
            Client.OnNickChange += HandleNickChange;
            Client.OnQueryMessage += HandleQueryMessage;
            Client.OnQueryAction += HandleQueryAction;
            Client.OnQueryNotice += HandleQueryNotice;
            Client.OnRegistered += HandleRegistered;
            Client.OnPart += HandlePart;
            Client.OnQuit += HandleQuit;
            Timers = new TimerTrigger();
            Canceller = new CancellationTokenSource();
        }

        public void Start()
        {
            IrcThread = new Thread(OuterProc)
            {
                Name = "IRC thread"
            };
            IrcThread.Start();

            Timers.Start();
        }

        public void Stop()
        {
            Timers.Stop();

            Canceller.Cancel();
            DisconnectOrWhatever();
            IrcThread.Join();
        }

        protected void DisconnectOrWhatever()
        {
            try
            {
                Client.Disconnect();
            }
            catch (NotConnectedException)
            {
            }
        }

        protected virtual void OuterProc()
        {
            var cancelToken = Canceller.Token;
            TimeSpan cooldown = TimeSpan.FromSeconds(1);
            TimeSpan cooldownIncreaseThreshold = TimeSpan.FromMinutes(Config.CooldownIncreaseThresholdMinutes);

            while (!cancelToken.IsCancellationRequested)
            {
                var connectPoint = DateTime.UtcNow;

                try
                {
                    Proc();
                }
                catch (Exception exc)
                {
                    Logger.Error("exception while running IRC", exc);
                    DisconnectOrWhatever();

                    var failPoint = DateTime.UtcNow;
                    if (cooldownIncreaseThreshold == TimeSpan.Zero || (failPoint - connectPoint) < cooldownIncreaseThreshold)
                    {
                        // increase cooldown
                        cooldown = TimeSpan.FromTicks(cooldown.Ticks * 2);
                    }
                }

                Thread.Sleep(cooldown);
            }
        }

        protected virtual void Proc()
        {
            SyncedChannels.Clear();

            Client.Connect(Config.ServerHostname, Config.ServerPort);

            Client.Login(Config.Nickname, Config.DisplayName, 0, Config.Username, Config.ServerPassword);

            // perform autocommands
            foreach (var autoCmd in Config.AutoConnectCommands)
            {
                Client.WriteLine(autoCmd);
            }

            // autojoin
            foreach (var channel in Config.AutoJoinChannels)
            {
                Client.RfcJoin(channel);
            }

            // listen
            Client.Listen();

            // disconnect
            DisconnectOrWhatever();
        }

        protected virtual void HandleCtcpRequest(object sender, CtcpEventArgs e)
        {
            try
            {
                switch (e.CtcpCommand)
                {
                    case "VERSION":
                        Client.SendMessage(SendType.CtcpReply, e.Data.Nick, "VERSION " + Config.CtcpVersionResponse);
                        break;
                    case "FINGER":
                        Client.SendMessage(SendType.CtcpReply, e.Data.Nick, "FINGER " + Config.CtcpFingerResponse);
                        break;
                    case "PING":
                        if (e.Data.Message.Length > 7)
                        {
                            Client.SendMessage(SendType.CtcpReply, e.Data.Nick, "PONG " + e.Data.Message.Substring(6, e.Data.Message.Length - 7));
                        }
                        else
                        {
                            Client.SendMessage(SendType.CtcpReply, e.Data.Nick, "PONG");
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                Logger.Warn("exception while handling CTCP request", exc);
            }
        }

        protected virtual void HandleChannelMessage(object sender, IrcEventArgs e)
        {
            if (!SyncedChannels.Contains(e.Data.Channel))
            {
                return;
            }
            OnChannelMessage(e);
        }

        protected virtual void HandleChannelAction(object sender, ActionEventArgs e)
        {
            if (!SyncedChannels.Contains(e.Data.Channel))
            {
                return;
            }
            OnChannelAction(e);
        }

        protected virtual void HandleChannelNotice(object sender, IrcEventArgs e)
        {
            if (!SyncedChannels.Contains(e.Data.Channel))
            {
                return;
            }
            OnChannelNotice(e);
        }

        protected virtual void HandleQueryMessage(object sender, IrcEventArgs e)
        {
            OnQueryMessage(e);
        }

        protected virtual void HandleQueryAction(object sender, ActionEventArgs e)
        {
            OnQueryAction(e);
        }

        protected virtual void HandleQueryNotice(object sender, IrcEventArgs e)
        {
            OnQueryNotice(e);
        }

        protected virtual void HandleChannelSynced(object sender, IrcEventArgs e)
        {
            SyncedChannels.Add(e.Data.Channel);
        }

        protected virtual void HandleRawMessage(object sender, IrcEventArgs e)
        {
            OnRawMessage(e);
        }

        protected virtual void HandleJoin(object sender, JoinEventArgs e)
        {
            OnJoinedChannel(e);
        }

        protected virtual void HandleNames(object sender, NamesEventArgs e)
        {
            OnNamesInChannel(e);
        }

        protected virtual void HandleNickChange(object sender, NickChangeEventArgs e)
        {
            OnNickChange(e);
        }

        protected virtual void HandleRegistered(object sender, EventArgs e)
        {
            OnConnectedToServer(e);
        }

        protected virtual void HandleQuit(object sender, QuitEventArgs e)
        {
            OnUserQuitServer(e);
        }

        protected virtual void HandlePart(object sender, PartEventArgs e)
        {
            OnUserLeftChannel(e);
        }

        protected virtual MessageFlags FlagsForNick(string nick)
        {
            if (Config.BannedUsers.Contains(nick))
            {
                return MessageFlags.UserBanned;
            }
            var regNick = RegisteredNameForNick(nick);
            if (regNick != null && Config.BannedUsers.Contains(regNick))
            {
                return MessageFlags.UserBanned;
            }
            return MessageFlags.None;
        }

        protected virtual void OnChannelMessage(IrcEventArgs e)
        {
            if (ChannelMessage != null)
            {
                var flags = FlagsForNick(e.Data.Nick);
                ChannelMessage(this, e, flags);
            }
        }

        protected virtual void OnChannelAction(ActionEventArgs e)
        {
            if (ChannelAction != null)
            {
                var flags = FlagsForNick(e.Data.Nick);
                ChannelAction(this, e, flags);
            }
        }

        protected virtual void OnChannelNotice(IrcEventArgs e)
        {
            if (ChannelNotice != null)
            {
                var flags = FlagsForNick(e.Data.Nick);
                ChannelNotice(this, e, flags);
            }
        }

        protected virtual void OnQueryMessage(IrcEventArgs e)
        {
            if (QueryMessage != null)
            {
                var flags = FlagsForNick(e.Data.Nick);
                QueryMessage(this, e, flags);
            }
        }

        protected virtual void OnQueryAction(ActionEventArgs e)
        {
            if (QueryAction != null)
            {
                var flags = FlagsForNick(e.Data.Nick);
                QueryAction(this, e, flags);
            }
        }

        protected virtual void OnQueryNotice(IrcEventArgs e)
        {
            if (QueryNotice != null)
            {
                var flags = FlagsForNick(e.Data.Nick);
                QueryNotice(this, e, flags);
            }
        }

        protected virtual void OnConnectedToServer(EventArgs e)
        {
            if (ConnectedToServer != null)
            {
                ConnectedToServer(this, e);
            }
        }

        protected virtual void OnNickMapping(NickMappingEventArgs e)
        {
            if (NickMapping != null)
            {
                NickMapping(this, e);
            }
        }

        protected virtual void OnRawMessage(IrcEventArgs e)
        {
            if (RawMessage != null)
            {
                RawMessage(this, e);
            }
        }

        protected virtual void OnNamesInChannel(NamesEventArgs e)
        {
            if (NamesInChannel != null)
            {
                NamesInChannel(this, e);
            }
        }

        protected virtual void OnJoinedChannel(JoinEventArgs e)
        {
            if (JoinedChannel != null)
            {
                JoinedChannel(this, e);
            }
        }

        protected virtual void OnNickChange(NickChangeEventArgs e)
        {
            if (NickChange != null)
            {
                NickChange(this, e);
            }
        }

        protected virtual void OnUserLeftChannel(PartEventArgs e)
        {
            if (UserLeftChannel != null)
            {
                UserLeftChannel(this, e);
            }
        }

        protected virtual void OnUserQuitServer(QuitEventArgs e)
        {
            if (UserQuitServer != null)
            {
                UserQuitServer(this, e);
            }
        }

        protected virtual void OnOutgoingChannelMessage(OutgoingMessageEventArgs e)
        {
            if (OutgoingChannelMessage != null)
            {
                OutgoingChannelMessage(this, e);
            }
        }

        protected virtual void OnOutgoingChannelAction(OutgoingMessageEventArgs e)
        {
            if (OutgoingChannelAction != null)
            {
                OutgoingChannelAction(this, e);
            }
        }

        protected virtual void OnOutgoingChannelNotice(OutgoingMessageEventArgs e)
        {
            if (OutgoingChannelNotice != null)
            {
                OutgoingChannelNotice(this, e);
            }
        }

        protected virtual void OnOutgoingQueryMessage(OutgoingMessageEventArgs e)
        {
            if (OutgoingQueryMessage != null)
            {
                OutgoingQueryMessage(this, e);
            }
        }

        protected virtual void OnOutgoingQueryAction(OutgoingMessageEventArgs e)
        {
            if (OutgoingQueryAction != null)
            {
                OutgoingQueryAction(this, e);
            }
        }

        protected virtual void OnOutgoingQueryNotice(OutgoingMessageEventArgs e)
        {
            if (OutgoingQueryNotice != null)
            {
                OutgoingQueryNotice(this, e);
            }
        }

        public string RegisteredNameForNick(string nick)
        {
            // perform nick mapping
            var eventArgs = new NickMappingEventArgs(nick);
            OnNickMapping(eventArgs);

            return eventArgs.MapsTo.FirstOrDefault();
        }

        /// <remarks><paramref name="words"/> will be modified.</remarks>
        protected string GetLongestWordPrefix(IList<string> words, int length = 230)
        {
            if (words.Count == 0)
            {
                throw new ArgumentException("words is empty", "words");
            }

            var firstWord = words[0];
            words.RemoveAt(0);

            if (Client.Encoding.GetBytes(firstWord).Length >= length)
            {
                // cutting on words isn't enough
                var returnValue = new StringBuilder(firstWord);
                var newFirstWord = new StringBuilder();

                while (Client.Encoding.GetBytes(returnValue.ToString()).Length >= length)
                {
                    // move a character from the end of returnValue to the beginning of newFirstWord
                    newFirstWord.Insert(0, returnValue[returnValue.Length - 1]);
                    --returnValue.Length;
                }

                // replace the new first word and return the return value
                words.Insert(0, newFirstWord.ToString());
                return returnValue.ToString();
            }

            // start taking words
            var ret = firstWord;
            while (words.Count > 0)
            {
                var testReturn = ret + " " + words[0];
                if (Client.Encoding.GetBytes(testReturn).Length >= length)
                {
                    // nope, not this one anymore
                    return ret.ToString();
                }

                // take a word
                ret = testReturn;
                words.RemoveAt(0);
            }

            // we took all the remaining words!
            return ret;
        }

        public List<string> SplitMessageToLength(string message, int length = 479)
        {
            // normalize newlines
            message = message.Replace("\r\n", "\n").Replace("\r", "\n");

            var lines = new List<string>();
            foreach (var origLine in message.Split('\n'))
            {
                if (Client.Encoding.GetBytes(origLine).Length < length || origLine.Length == 0)
                {
                    // short-circuit
                    lines.Add(origLine);
                    continue;
                }

                var words = origLine.Split(' ').ToList();
                while (words.Count > 0)
                {
                    var line = GetLongestWordPrefix(words, length);
                    lines.Add(line);
                }
            }
            return lines;
        }

        public void SendChannelMessage(string channel, string message)
        {
            foreach (var line in SplitMessageToLength(message, MaxMessageLength))
            {
                OnOutgoingChannelMessage(new OutgoingMessageEventArgs(channel, line));
                Client.SendMessage(SendType.Message, channel, line);
            }
        }

        public void SendChannelMessageFormat(string channel, string format, params object[] args)
        {
            SendChannelMessage(channel, string.Format(format, args));
        }

        public void SendChannelAction(string channel, string message)
        {
            foreach (var line in SplitMessageToLength(message, MaxMessageLength))
            {
                OnOutgoingChannelAction(new OutgoingMessageEventArgs(channel, line));
                Client.SendMessage(SendType.Action, channel, line);
            }
        }

        public void SendChannelActionFormat(string channel, string format, params object[] args)
        {
            SendChannelAction(channel, string.Format(format, args));
        }

        public void SendChannelNotice(string channel, string message)
        {
            foreach (var line in SplitMessageToLength(message, MaxMessageLength))
            {
                OnOutgoingChannelNotice(new OutgoingMessageEventArgs(channel, line));
                Client.SendMessage(SendType.Notice, channel, line);
            }
        }

        public void SendChannelNoticeFormat(string channel, string format, params object[] args)
        {
            SendChannelNotice(channel, string.Format(format, args));
        }

        public void SendQueryMessage(string nick, string message)
        {
            foreach (var line in SplitMessageToLength(message, MaxMessageLength))
            {
                OnOutgoingQueryMessage(new OutgoingMessageEventArgs(nick, line));
                Client.SendMessage(SendType.Message, nick, line);
            }
        }

        public void SendQueryMessageFormat(string nick, string format, params object[] args)
        {
            SendQueryMessage(nick, string.Format(format, args));
        }

        public void SendQueryAction(string nick, string message)
        {
            foreach (var line in SplitMessageToLength(message, MaxMessageLength))
            {
                OnOutgoingQueryAction(new OutgoingMessageEventArgs(nick, line));
                Client.SendMessage(SendType.Action, nick, line);
            }
        }

        public void SendQueryActionFormat(string nick, string format, params object[] args)
        {
            SendQueryAction(nick, string.Format(format, args));
        }

        public void SendQueryNotice(string nick, string message)
        {
            foreach (var line in SplitMessageToLength(message, MaxMessageLength))
            {
                OnOutgoingQueryNotice(new OutgoingMessageEventArgs(nick, line));
                Client.SendMessage(SendType.Notice, nick, line);
            }
        }

        public void SendQueryNoticeFormat(string nick, string format, params object[] args)
        {
            SendQueryNotice(nick, string.Format(format, args));
        }

        public void SendRawCommand(string cmd)
        {
            Client.WriteLine(cmd);
        }
    }
}
