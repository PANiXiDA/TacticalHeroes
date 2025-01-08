using System;

namespace Assets.Scripts.Common.Constants
{
    public static class BattleSuggestionsConstants
    {
        public const string Suggestion1 = "God Abaddon sees everything!";
        public const string Suggestion2 = "Looking for mushrooms...";
        public const string Suggestion3 = "Get Up.";
        public const string Suggestion4 = "Zaedet?";
        public const string Suggestion5 = "The Crypt";

        private static readonly string[] SuggestionsArray =
        {
            Suggestion1,
            Suggestion2,
            Suggestion3,
            Suggestion4,
            Suggestion5
        };

        public static string GetRandomSuggestion()
        {
            Random random = new Random();
            return SuggestionsArray[random.Next(SuggestionsArray.Length)];
        }
    }
}
