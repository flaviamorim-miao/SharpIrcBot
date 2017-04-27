using SharpIrcBot.Plugins.CasinoBot.Cards;

namespace SharpIrcBot.Plugins.CasinoBot.Player
{
    public interface ICardCounter
    {
        void CardDealt(Card card);

        void ShoeShuffled();

        int TotalDecks { get; set; }

        int BetAdjustment { get; }
    }
}
