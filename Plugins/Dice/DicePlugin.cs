﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using Meebey.SmartIrc4net;
using Newtonsoft.Json.Linq;
using SharpIrcBot;

namespace Dice
{
    public class DicePlugin : IPlugin
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly Regex DiceThrowRegex = new Regex(
            "^" +
            "!roll +" +
            "(?<firstRoll>" +
                "(?:[1-9][0-9]*)?" + // number of dice
                "d" +
                "[1-9][0-9]*" + // number of sides
                "(?:[+-][1-9][0-9]*)?" + // add a value?
            ")" +
            "(?:" +
                "[, ]+" +
                "(?<nextRoll>" +
                    "(?:[1-9][0-9]*)?" + // number of dice
                    "d" +
                    "[1-9][0-9]*" + // number of sides
                    "(?:[+-][1-9][0-9]*)?" + // add a value?
                ")" +
            ")*" +
            "[ ]*$",
            RegexOptions.IgnoreCase
        );
        public static readonly Regex RollRegex = new Regex(
            "^" +
            "(?<dice>[1-9][0-9]*)?" +
            "d" +
            "(?<sides>[1-9][0-9]*)" +
            "(?<addvalue>[+-][1-9][0-9]*)?" +
            "$",
            RegexOptions.IgnoreCase
        );

        protected ConnectionManager ConnectionManager { get; set; }
        protected DiceConfig Config { get; set; }
        protected Random RNG { get; set; }

        public DicePlugin(ConnectionManager connMgr, JObject config)
        {
            ConnectionManager = connMgr;
            Config = new DiceConfig(config);
            RNG = new Random();

            ConnectionManager.ChannelMessage += HandleChannelMessage;
        }

        private void HandleChannelMessage(object sender, IrcEventArgs args, MessageFlags flags)
        {
            try
            {
                ActuallyHandleChannelMessage(sender, args, flags);
            }
            catch (Exception exc)
            {
                Logger.Error("error handling message", exc);
            }
        }

        protected virtual void ActuallyHandleChannelMessage(object sender, IrcEventArgs args, MessageFlags flags)
        {
            if (flags.HasFlag(MessageFlags.UserBanned))
            {
                return;
            }

            var match = DiceThrowRegex.Match(args.Data.Message);
            if (match.Success)
            {
                HandleDiceRoll(args, match);
                return;
            }

            if (args.Data.Message.StartsWith("!yn ", StringComparison.InvariantCultureIgnoreCase) && Config.YesNoAnswers.Count > 0)
            {
                var yesNoAnswer = Config.YesNoAnswers[RNG.Next(Config.YesNoAnswers.Count)];
                ConnectionManager.SendChannelMessageFormat(args.Data.Channel, "{0}: {1}", args.Data.Nick, yesNoAnswer);
                return;
            }

            if (args.Data.Message.StartsWith("!decide ", StringComparison.InvariantCultureIgnoreCase) && Config.DecisionSplitters.Count > 0)
            {
                var decisionString = args.Data.Message.Substring(("!decide ").Length).Trim();

                string splitter = Config.DecisionSplitters.FirstOrDefault(ds => decisionString.Contains(ds));
                if (splitter == null)
                {
                    ConnectionManager.SendChannelMessageFormat(args.Data.Channel, "{0}: Uhh... that looks like only one option to decide from.", args.Data.Nick);
                    return;
                }

                if (Config.SpecialDecisionAnswers.Count > 0)
                {
                    int percent = RNG.Next(100);
                    if (percent < Config.SpecialDecisionAnswerPercent)
                    {
                        // special answer instead!
                        var specialAnswer = Config.SpecialDecisionAnswers[RNG.Next(Config.SpecialDecisionAnswers.Count)];
                        ConnectionManager.SendChannelMessageFormat(args.Data.Channel, "{0}: {1}", args.Data.Nick, specialAnswer);
                        return;
                    }
                }

                var options = decisionString.Split(new[] {splitter}, StringSplitOptions.None);
                var chosenOption = options[RNG.Next(options.Length)];
                ConnectionManager.SendChannelMessageFormat(args.Data.Channel, "{0}: {1}", args.Data.Nick, chosenOption);
            }
        }

        protected DiceGroup ObtainDiceGroup(Match rollMatch, string channel, string senderNick)
        {
            int? dice = MaybeParseIntGroup(rollMatch.Groups["dice"], defaultValue: 1);
            int? sides = SharpIrcBotUtil.MaybeParseInt(rollMatch.Groups["sides"].Value);
            long? addValue = MaybeParseLongGroup(rollMatch.Groups["addValue"], defaultValue: 0);
            if (!dice.HasValue || dice.Value > Config.MaxDiceCount)
            {
                ConnectionManager.SendChannelMessageFormat(channel, "{0}: Too many dice.", senderNick);
                return null;
            }
            if (!sides.HasValue || sides.Value > Config.MaxSideCount)
            {
                ConnectionManager.SendChannelMessageFormat(channel, "{0}: Too many sides.", senderNick);
                return null;
            }
            if (!addValue.HasValue)
            {
                ConnectionManager.SendChannelMessageFormat(channel, "{0}: Value to add too large.", senderNick);
                return null;
            }
            return new DiceGroup(dice.Value, sides.Value, addValue.Value);
        }

        protected void HandleDiceRoll(IrcEventArgs args, Match match)
        {
            var diceGroups = new List<DiceGroup>();
            var firstRollMatch = RollRegex.Match(match.Groups["firstRoll"].Value);
            Debug.Assert(firstRollMatch.Success);

            var firstDiceGroup = ObtainDiceGroup(firstRollMatch, args.Data.Channel, args.Data.Nick);
            if (firstDiceGroup == null)
            {
                // error occurred and reported; bail out
                return;
            }
            diceGroups.Add(firstDiceGroup);

            var nextRollCaptures = match.Groups["nextRoll"].Captures.OfType<Capture>();
            foreach (var nextRoll in nextRollCaptures)
            {
                var nextRollMatch = RollRegex.Match(nextRoll.Value);
                Debug.Assert(nextRollMatch.Success);

                var nextDiceGroup = ObtainDiceGroup(nextRollMatch, args.Data.Channel, args.Data.Nick);
                if (nextDiceGroup == null)
                {
                    // error occurred and reported; bail out
                    return;
                }
                diceGroups.Add(nextDiceGroup);

                if (diceGroups.Count > Config.MaxRollCount)
                {
                    ConnectionManager.SendChannelMessageFormat(args.Data.Channel, "{0}: Too many rolls.", args.Data.Nick);
                    return;
                }
            }

            // special-case 2d1
            if (diceGroups.Count == 1 && diceGroups[0].DieCount == 2 && diceGroups[0].SideCount == 1 && diceGroups[0].AddValue == 0)
            {
                ConnectionManager.SendChannelAction(args.Data.Channel, "rolls its eyes");
                return;
            }

            var allRolls = new List<string>();
            foreach (var diceGroup in diceGroups)
            {
                var theseRolls = new List<string>(diceGroup.DieCount);
                for (int i = 0; i < diceGroup.DieCount; ++i)
                {
                    if (diceGroup.SideCount == 1 && Config.ObstinateAnswers.Count > 0)
                    {
                        // special case: give an obstinate answer instead since a 1-sided toss has an obvious result
                        string obstinateAnswer = Config.ObstinateAnswers[RNG.Next(Config.ObstinateAnswers.Count)];
                        theseRolls.Add(obstinateAnswer);
                    }
                    else
                    {
                        long roll = RNG.Next(diceGroup.SideCount) + 1 + diceGroup.AddValue;
                        theseRolls.Add(roll.ToString(CultureInfo.InvariantCulture));
                    }
                }
                var theseRollsString = string.Join(" ", theseRolls);
                allRolls.Add(theseRollsString);
            }
            var allRollsString = string.Join("; ", allRolls);

            ConnectionManager.SendChannelMessageFormat(args.Data.Channel, "{0}: {1}", args.Data.Nick, allRollsString);
        }

        protected static int? MaybeParseIntGroup(Group grp, int? defaultValue = null)
        {
            if (!grp.Success)
            {
                return defaultValue;
            }

            return SharpIrcBotUtil.MaybeParseInt(grp.Value, NumberStyles.AllowLeadingSign);
        }

        protected static long? MaybeParseLongGroup(Group grp, long? defaultValue = null)
        {
            if (!grp.Success)
            {
                return defaultValue;
            }

            return SharpIrcBotUtil.MaybeParseLong(grp.Value, NumberStyles.AllowLeadingSign);
        }
    }
}