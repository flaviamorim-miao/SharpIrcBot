namespace SharpIrcBot.Plugins.Libraries.RegularExpressionReplacement.Internal
{
    public class EntireInputStringPlaceholder : IPlaceholder
    {
        public EntireInputStringPlaceholder()
        {
        }

        public string Replace(ReplacementState state)
        {
            return state.InputString;
        }
    }
}
