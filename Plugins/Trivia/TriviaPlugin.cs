using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using SharpIrcBot.Commands;
using SharpIrcBot.Events;
using SharpIrcBot.Events.Irc;
using SharpIrcBot.Plugins.Trivia.ORM;
using SharpIrcBot.Util;

namespace SharpIrcBot.Plugins.Trivia
{
    public class TriviaPlugin : IPlugin, IReloadableConfiguration
    {
        protected IConnectionManager ConnectionManager { get; }
        protected TriviaConfig Config { get; set; }
        protected GameState GameState { get; set; }
        protected Random Randomizer { get; set; }

        public TriviaPlugin(IConnectionManager connMgr, JObject config)
        {
            ConnectionManager = connMgr;
            Config = new TriviaConfig(config);
            GameState = null;
            Randomizer = new Random();

            ConnectionManager.ChannelMessage += HandleChannelMessage;
            ConnectionManager.BaseNickChanged += HandleBaseNickChanged;

            ConnectionManager.CommandManager.RegisterChannelMessageCommandHandler(
                new Command(
                    CommandUtil.MakeNames("question"),
                    forbiddenFlags: MessageFlags.UserBanned
                ),
                HandleQuestionCommand
            );
            ConnectionManager.CommandManager.RegisterChannelMessageCommandHandler(
                new Command(
                    CommandUtil.MakeNames("starttrivia"),
                    forbiddenFlags: MessageFlags.UserBanned
                ),
                HandleStartTriviaCommand
            );
            ConnectionManager.CommandManager.RegisterChannelMessageCommandHandler(
                new Command(
                    CommandUtil.MakeNames("stoptrivia"),
                    forbiddenFlags: MessageFlags.UserBanned
                ),
                HandleStopTriviaCommand
            );
        }

        public virtual void ReloadConfiguration(JObject newConfig)
        {
            Config = new TriviaConfig(newConfig);
            PostConfigReload();
        }

        protected virtual void PostConfigReload()
        {
        }

        protected virtual void HandleQuestionCommand(CommandMatch cmd, IChannelMessageEventArgs msg)
        {
            if (msg.Channel != Config.TriviaChannel)
            {
                return;
            }

            if (GameState != null)
            {
                lock (GameState.Lock)
                {
                    OutputCurrentQuestion();
                }
            }
        }

        protected virtual void HandleStartTriviaCommand(CommandMatch cmd, IChannelMessageEventArgs msg)
        {
            if (msg.Channel != Config.TriviaChannel)
            {
                return;
            }

            if (GameState == null)
            {
                StartGame();
            }
        }

        protected virtual void HandleStopTriviaCommand(CommandMatch cmd, IChannelMessageEventArgs msg)
        {
            if (msg.Channel != Config.TriviaChannel)
            {
                return;
            }

            StopGame();
        }

        protected virtual void HandleChannelMessage(object sender, IChannelMessageEventArgs e, MessageFlags flags)
        {
            if (flags.HasFlag(MessageFlags.UserBanned))
            {
                return;
            }

            if (e.Channel != Config.TriviaChannel)
            {
                return;
            }

            if (GameState != null)
            {
                lock (GameState.Lock)
                {
                    CheckForCorrectAnswer(e.SenderNickname, e.Message);
                }
            }
        }

        protected virtual void CheckForCorrectAnswer(string nick, string message)
        {
            Debug.Assert(GameState != null);

            lock (GameState.Lock)
            {
                string trimmedLowerMessage = message.Trim().ToLowerInvariant();
                bool hit = false;
                foreach (string answer in GameState.CurrentQuestion.Answers)
                {
                    Debug.Assert(answer == answer.Trim().ToLowerInvariant());
                    if (trimmedLowerMessage == answer)
                    {
                        hit = true;
                        break;
                    }
                }

                if (!hit)
                {
                    return;
                }

                // got it!
                StopTimer();

                ConnectionManager.SendChannelMessage(
                    Config.TriviaChannel,
                    $"{nick} got it! The correct answer was: {GameState.CurrentQuestion.MainAnswer}"
                );

                string user = ConnectionManager.RegisteredNameForNick(nick) ?? nick;
                string userLower = user.ToLowerInvariant();
                using (TriviaContext ctx = GetNewContext())
                {
                    LeaderboardEntry entry = ctx.LeaderboardEntries.FirstOrDefault(le => le.NicknameLowercase == userLower);
                    if (entry == null)
                    {
                        entry = new LeaderboardEntry
                        {
                            NicknameLowercase = userLower,
                            CorrectAnswers = 1
                        };
                        ctx.LeaderboardEntries.Add(entry);
                    }
                    else
                    {
                        ++entry.CorrectAnswers;
                    }
                    ctx.SaveChanges();
                }

                NewQuestion();
            }
        }

        protected virtual void StartGame()
        {
            var newState = new GameState
            {
                Questions = new List<QuestionAnswers>(),
                CurrentQuestionIndex = -1,
                HintsAlreadyShown = 0,
                Timer = new Timer(TimerElapsed, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan),
                Lock = new object()
            };

            // load questions
            foreach (string fileName in Config.QuestionFiles)
            {
                string fullPath = Path.Combine(SharpIrcBotUtil.AppDirectory, fileName);
                try
                {
                    using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(stream, Encoding.UTF8, true))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] pieces = line.Split('`');
                            if (pieces.Length < 2)
                            {
                                continue;
                            }

                            string question = pieces[0].Trim();
                            var answers = new List<string>(pieces.Skip(1).Select(p => p.Trim().ToLowerInvariant()));
                            var qa = new QuestionAnswers
                            {
                                Question = question,
                                Answers = answers
                            };
                            newState.Questions.Add(qa);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    // meh
                }
            }

            // deploy
            GameState = newState;
            NewQuestion();
        }

        protected virtual void StopGame()
        {
            if (GameState == null)
            {
                ConnectionManager.SendChannelMessage(Config.TriviaChannel,"There is no running trivia game.");
                return;
            }

            lock (GameState.Lock)
            {
                GameState.Timer.Dispose();
                GameState = null;
            }

            ConnectionManager.SendChannelMessage(Config.TriviaChannel, "Trivia game stopped.");
        }

        protected virtual void TimerElapsed(object theStateIDontActuallyCareAbout)
        {
            if (GameState == null)
            {
                return;
            }

            lock (GameState.Lock)
            {
                if (GameState.HintsAlreadyShown >= Config.HintCount)
                {
                    ConnectionManager.SendChannelMessage(
                        Config.TriviaChannel,
                        $"Time's up! The correct answer was: {GameState.CurrentQuestion.MainAnswer}"
                    );
                    NewQuestion();
                }
                else
                {
                    ShowHint();
                    ++GameState.HintsAlreadyShown;
                }
            }
        }

        protected virtual void NewQuestion()
        {
            Debug.Assert(GameState != null);

            lock (GameState.Lock)
            {
                GameState.CurrentQuestionIndex = Randomizer.Next(GameState.Questions.Count);
                GameState.HintsAlreadyShown = 0;

                OutputCurrentQuestion();
                StartTimer();
            }
        }

        protected virtual void OutputCurrentQuestion()
        {
            Debug.Assert(GameState != null);

            lock (GameState.Lock)
            {
                ConnectionManager.SendChannelMessage(
                    Config.TriviaChannel,
                    $"Question: {GameState.CurrentQuestion.Question}"
                );

                // calculate and display answer pattern
                var pattern = new UnicodeStringBuilder(GameState.CurrentQuestion.MainAnswer);
                for (int i = 0; i < pattern.Length; ++i)
                {
                    if (char.IsLetterOrDigit(pattern.CharAtAsString(i), 0))
                    {
                        pattern[i] = '_';
                    }
                }
                ConnectionManager.SendChannelMessage(
                    Config.TriviaChannel,
                    $"Answer Pattern: {pattern}"
                );
            }
        }

        protected virtual void ShowHint()
        {
            Debug.Assert(GameState != null);

            lock (GameState.Lock)
            {
                int currentHint = GameState.HintsAlreadyShown + 1;
                Debug.Assert(currentHint <= Config.HintCount);

                var maskedAnswer = new UnicodeStringBuilder(GameState.CurrentQuestion.MainAnswer);
                var unmaskedIndexes = new List<int>();
                for (int i = 0; i < maskedAnswer.Length; ++i)
                {
                    if (char.IsLetterOrDigit(maskedAnswer.CharAtAsString(i), 0))
                    {
                        unmaskedIndexes.Add(i);
                    }
                }

                int lettersToShowCount = currentHint * unmaskedIndexes.Count / (Config.HintCount + 1);
                int lettersToMaskCount = unmaskedIndexes.Count - lettersToShowCount;

                for (int i = 0; i < lettersToMaskCount; ++i)
                {
                    int unmaskedIndexesIndex = Randomizer.Next(unmaskedIndexes.Count);
                    int indexToMask = unmaskedIndexes[unmaskedIndexesIndex];
                    unmaskedIndexes.RemoveAt(unmaskedIndexesIndex);

                    maskedAnswer[indexToMask] = '_';
                }

                ConnectionManager.SendChannelMessage(
                    Config.TriviaChannel,
                    $"Hint {currentHint}/{Config.HintCount}: {maskedAnswer}"
                );

                // restart the timer
                StartTimer();
            }
        }

        protected virtual void StopTimer()
        {
            Debug.Assert(GameState != null);

            lock (GameState.Lock)
            {
                GameState.Timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }
        }

        protected virtual void StartTimer()
        {
            Debug.Assert(GameState != null);

            lock (GameState.Lock)
            {
                GameState.Timer.Change(TimeSpan.FromSeconds(Config.SecondsBetweenHints), Timeout.InfiniteTimeSpan);
            }
        }

        protected virtual void HandleBaseNickChanged(object sender, BaseNickChangedEventArgs args)
        {
            // merge score of old and new

            object gameLock = GameState?.Lock ?? new object();
            lock (gameLock)
            {
                var oldNickLower = args.OldBaseNick.ToLowerInvariant();
                var newNickLower = args.NewBaseNick.ToLowerInvariant();

                using (TriviaContext ctx = GetNewContext())
                {
                    LeaderboardEntry oldEntry = ctx.LeaderboardEntries
                        .FirstOrDefault(le => le.NicknameLowercase == oldNickLower);

                    if (oldEntry == null)
                    {
                        // don't bother
                        return;
                    }

                    LeaderboardEntry newEntry = ctx.LeaderboardEntries
                        .FirstOrDefault(le => le.NicknameLowercase == newNickLower);

                    if (newEntry == null)
                    {
                        newEntry = new LeaderboardEntry
                        {
                            NicknameLowercase = newNickLower,
                            CorrectAnswers = oldEntry.CorrectAnswers
                        };
                        ctx.Add(newEntry);
                    }
                    else
                    {
                        newEntry.CorrectAnswers += oldEntry.CorrectAnswers;
                    }
                    ctx.Remove(oldEntry);
                    ctx.SaveChanges();
                }
            }
        }

        protected TriviaContext GetNewContext()
        {
            var opts = DatabaseUtil.GetContextOptions<TriviaContext>(Config);
            return new TriviaContext(opts);
        }
    }
}
