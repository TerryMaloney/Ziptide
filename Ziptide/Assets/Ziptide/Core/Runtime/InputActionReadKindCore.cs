using System;

namespace Ziptide.Core
{
    public enum InputActionReadKind
    {
        Vector2 = 0,
        Scalar = 1,
        Object = 2,
    }

    /// <summary>
    /// Pure resolver used by input-settle probes. InputActionProperty owners do not all promise the same
    /// value type; reading a valid Button/Axis action as Vector2 produces the same InvalidOperationException
    /// as a genuinely stale InputActionState. Treating that mismatch as "not settled" makes a fail-closed
    /// restoration window impossible to complete.
    /// </summary>
    public static class InputActionReadKindCore
    {
        public static InputActionReadKind Resolve(string expectedControlType, string runtimeValueTypeName)
        {
            string expected = (expectedControlType ?? string.Empty).Trim();
            string runtime = (runtimeValueTypeName ?? string.Empty).Trim();

            if (EqualsAny(expected, "Button", "Axis", "Integer")
                || EndsWithAny(runtime, ".Single", ".Double", ".Int32", ".Boolean"))
                return InputActionReadKind.Scalar;

            if (EqualsAny(expected, "Vector2", "Stick", "Dpad")
                || EndsWithAny(runtime, ".Vector2"))
                return InputActionReadKind.Vector2;

            return InputActionReadKind.Object;
        }

        private static bool EqualsAny(string value, params string[] candidates)
        {
            for (int i = 0; i < candidates.Length; i++)
                if (string.Equals(value, candidates[i], StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private static bool EndsWithAny(string value, params string[] suffixes)
        {
            for (int i = 0; i < suffixes.Length; i++)
                if (value.EndsWith(suffixes[i], StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}
