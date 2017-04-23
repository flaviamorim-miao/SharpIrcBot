﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpIrcBot.Collections;
using SharpIrcBot.Events.Irc;
using SharpIrcBot.Plugins.CasinoBot.Cards;

namespace SharpIrcBot.Plugins.CasinoBot.Player
{
    public class CasinoPlayerBotPlugin : IPlugin, IReloadableConfiguration
    {
        const int BlackjackSafeMaximum = 11;
        const int BlackjackDealerMinimum = 17;
        const int BlackjackTargetValue = 21;

        protected IConnectionManager ConnectionManager { get; }
        protected PlayerConfig Config { get; set; }
        protected EventDispatcher Dispatcher { get; set; }
        protected BlackjackState State { get; set; }
        protected SortedMultiset<Card> MyHand { get; set; }
        protected Random Randomizer { get; set; }

        public CasinoPlayerBotPlugin(IConnectionManager connMgr, JObject config)
        {
            ConnectionManager = connMgr;
            Config = new PlayerConfig(config);
            Dispatcher = new EventDispatcher();
            State = BlackjackState.None;
            MyHand = null;
            Randomizer = new Random();

            ConnectionManager.ChannelMessage += HandleChannelMessage;
            ConnectionManager.QueryMessage += HandleQueryMessage;
        }

        public virtual void ReloadConfiguration(JObject newConfig)
        {
            Config = new PlayerConfig(newConfig);
            PostConfigReload();
        }

        protected virtual void PostConfigReload()
        {
        }

        protected virtual void HandleChannelMessage(object sender, IChannelMessageEventArgs args, MessageFlags flags)
        {
            if (flags.HasFlag(MessageFlags.UserBanned))
            {
                return;
            }

            if (args.Channel != Config.CasinoChannel)
            {
                return;
            }

            bool botJoin = false;
            if (args.Message.Trim() == "?botjoin")
            {
                // "?botjoin"
                botJoin = true;
            }
            else
            {
                string[] bits = args.Message.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if (bits.Length >= 2 && bits[0] == "?botjoin" && bits.Skip(1).Any(b => b == ConnectionManager.MyNickname))
                {
                    // "?botjoin MyBot" or "?botjoin ThisBot ThatBot MyBot"
                    botJoin = true;
                }
            }

            if (botJoin)
            {
                ConnectionManager.SendChannelMessage(args.Channel, ".botjoin");
            }
        }

        protected virtual void HandleQueryMessage(object sender, IPrivateMessageEventArgs args, MessageFlags flags)
        {
            if (flags.HasFlag(MessageFlags.UserBanned))
            {
                return;
            }

            if (!string.Equals(args.SenderNickname, Config.GameMasterNickname, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            JObject eventObject;
            try
            {
                eventObject = JObject.Parse(args.Message);
            }
            catch (JsonReaderException)
            {
                return;
            }

            Dispatcher.DispatchEvent(this, eventObject);
        }

        [Event("turn_info_betting")]
        public virtual void HandleEventTurnInfoBetting([EventValue("player")] string player, [EventValue("stack")] int stack)
        {
            State = (player == ConnectionManager.MyNickname)
                ? BlackjackState.MyBetting
                : BlackjackState.OthersBetting;

            if (State != BlackjackState.MyBetting)
            {
                return;
            }

            PlaceBlackjackBet();
        }

        [Event("turn_info")]
        public virtual void HandleEventTurnInfo([EventValue("player")] string player)
        {
            State = (player == ConnectionManager.MyNickname)
                ? BlackjackState.MyTurn
                : BlackjackState.OthersTurn;

            if (State == BlackjackState.MyTurn)
            {
                MakeBlackjackMove();
            }
        }

        [Event("hand_info")]
        public virtual void HandleEventHandInfo([EventValue("player")] string player, [EventValue("hand")] List<Card> hand)
        {
            if (player != ConnectionManager.MyNickname)
            {
                return;
            }

            MyHand = new SortedMultiset<Card>(hand);

            if (State == BlackjackState.MyTurn)
            {
                MakeBlackjackMove();
            }
        }

        [Event("round_end")]
        public virtual void HandleEventRoundEnd()
        {
            State = BlackjackState.None;
        }

        protected virtual void PlaceBlackjackBet()
        {
            // TODO: implement smarter betting strategy
            ConnectionManager.SendChannelMessage(Config.CasinoChannel, ".bet 5");
        }

        protected virtual void MakeBlackjackMove()
        {
            List<int> handValues = MyHand.BlackjackValues()
                .ToList();
            if (handValues.Any(hv => hv == BlackjackTargetValue))
            {
                // aw yiss
                if (Config.Gloats.Count > 0)
                {
                    string gloat = Config.Gloats[Randomizer.Next(Config.Gloats.Count)];
                    ConnectionManager.SendChannelMessage(Config.CasinoChannel, gloat);
                }
                ConnectionManager.SendChannelMessage(Config.CasinoChannel, ".stand");
                return;
            }

            int minValue = handValues.Min();
            if (minValue > BlackjackTargetValue)
            {
                // bust
                if (Config.Curses.Count > 0)
                {
                    string curse = Config.Curses[Randomizer.Next(Config.Curses.Count)];
                    ConnectionManager.SendChannelMessage(Config.CasinoChannel, curse);
                }
                return;
            }

            // TODO: implement card counting
            // FIXME: ConnectionManager.SendChannelMessage(Config.CasinoChannel, "there are 52 cards") is not enough

            bool stand = false;
            if (minValue <= BlackjackSafeMaximum)
            {
                stand = false;
            }
            else if (minValue < BlackjackDealerMinimum)
            {
                // low rate
                stand = (Randomizer.Next(0, Config.LowStandDen) < Config.LowStandNum);
            }
            else
            {
                // high rate
                stand = (Randomizer.Next(0, Config.HighStandDen) < Config.HighStandNum);
            }

            ConnectionManager.SendChannelMessage(Config.CasinoChannel, stand ? ".stand" : ".hit");
        }
    }
}
