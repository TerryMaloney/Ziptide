using System.Text;

namespace Ziptide.Core
{
    /// <summary>
    /// Word-wrap for TextMesh subtitles (TextMesh has NO built-in wrapping — Terry's device report:
    /// world-entry lines ran too wide to read). Pure and CI-tested. Breaks on spaces at a max line
    /// width; a single word longer than the width is left intact on its own line (never split
    /// mid-word — subtitles must stay readable). Existing newlines are respected as hard breaks.
    /// </summary>
    public static class SubtitleText
    {
        /// <summary>Default line width for VR subtitles — about the width of the comfortable gaze cone.</summary>
        public const int DefaultLineChars = 38;

        public static string Wrap(string text, int maxLineChars = DefaultLineChars)
        {
            if (string.IsNullOrEmpty(text) || maxLineChars <= 0) return text ?? "";

            var sb = new StringBuilder(text.Length + 8);
            foreach (string hardLine in text.Split('\n'))
            {
                if (sb.Length > 0) sb.Append('\n');
                int lineLen = 0;
                foreach (string word in hardLine.Split(' '))
                {
                    if (word.Length == 0) continue;
                    if (lineLen == 0)
                    {
                        sb.Append(word);
                        lineLen = word.Length;
                    }
                    else if (lineLen + 1 + word.Length <= maxLineChars)
                    {
                        sb.Append(' ').Append(word);
                        lineLen += 1 + word.Length;
                    }
                    else
                    {
                        sb.Append('\n').Append(word);
                        lineLen = word.Length;
                    }
                }
            }
            return sb.ToString();
        }
    }
}
