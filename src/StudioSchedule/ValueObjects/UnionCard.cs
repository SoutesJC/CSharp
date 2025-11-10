// File: src/StudioSchedule/Domain/ValueObjects/UnionCard.cs
#nullable enable
using System;

namespace StudioSchedule.ValueObjects
{
    public sealed class UnionCard : IEquatable<UnionCard>
    {
        public string RegistrationNumber { get; }

        public UnionCard(string registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
                throw new ArgumentException("Registration number must be provided.", nameof(registrationNumber));

            RegistrationNumber = registrationNumber.Trim();
        }

        public override bool Equals(object? obj) => Equals(obj as UnionCard);
        public bool Equals(UnionCard? other) => other is not null && RegistrationNumber == other.RegistrationNumber;
        public override int GetHashCode() => RegistrationNumber.GetHashCode();
        public static bool operator ==(UnionCard? a, UnionCard? b) => EqualityComparer<UnionCard>.Default.Equals(a, b);
        public static bool operator !=(UnionCard? a, UnionCard? b) => !(a == b);
    }
}