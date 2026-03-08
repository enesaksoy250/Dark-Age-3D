using System;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public readonly struct PlayerId : IEquatable<PlayerId>
    {
        public PlayerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Player id cannot be empty.", nameof(value));
            }

            Value = value;
        }

        public string Value { get; }

        public bool Equals(PlayerId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value != null ? StringComparer.Ordinal.GetHashCode(Value) : 0;
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(PlayerId playerId)
        {
            return playerId.Value;
        }
    }
}
