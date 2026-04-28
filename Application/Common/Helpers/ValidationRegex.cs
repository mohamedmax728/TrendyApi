namespace Application.Common.Helpers
{
    using System.Text.RegularExpressions;

    namespace Utilities.Validation
    {
        public static class ValidationRegex
        {
            // =========================
            // 🔹 Egyptian Mobile Number
            // =========================
            // Starts with 010, 011, 012, 015 and total 11 digits
            public static readonly Regex EgyptianMobile =
                new Regex(@"^(010|011|012|015)\d{8}$", RegexOptions.Compiled);

            // =========================
            // 🔹 Arabic Letters Only
            // =========================
            public static readonly Regex ArabicLetters =
                new Regex(@"^[\u0600-\u06FF\s]+$", RegexOptions.Compiled);

            // More strict Arabic (optional)
            public static readonly Regex ArabicLettersStrict =
                new Regex(@"^[\u0621-\u064A\s]+$", RegexOptions.Compiled);

            // =========================
            // 🔹 English Letters Only
            // =========================
            public static readonly Regex EnglishLetters =
                new Regex(@"^[a-zA-Z\s]+$", RegexOptions.Compiled);

            // =========================
            // 🔹 English Letters, Numbers and Punctuation
            // =========================
            public static readonly Regex EnglishLettersAndNumbersAndPunctuation =
                new Regex(@"^[a-zA-Z0-9\s.,!?'\-()&]+$", RegexOptions.Compiled);

            // =========================
            // 🔹 Arabic Letters, Numbers and Punctuation
            // =========================
            public static readonly Regex ArabicLettersAndNumbersAndPunctuation =
                new Regex(@"^[\u0600-\u06FF\s0-9.,!?'\-()&]+$", RegexOptions.Compiled);

            // =========================
            // 🔹 Strong Password
            // =========================
            public static readonly Regex StrongPassword =
                new Regex(@"^(?=.*[a-z])(?=.*[A-A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", RegexOptions.Compiled);

            // =========================
            // 🔹 Helper Method
            // =========================
            public static bool IsMatch(string input, Regex regex)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return false;

                return regex.IsMatch(input);
            }
        }
    }
}
