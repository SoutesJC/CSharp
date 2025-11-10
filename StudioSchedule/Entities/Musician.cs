// File: src/StudioSchedule/Domain/Entities/Musician.cs
#nullable enable
using System;
using StudioSchedule.ValueObjects;

namespace StudioSchedule.Entities
{
    public sealed class Musician : IEquatable<Musician>
    {
        public Guid Id { get; }
        public string FullName { get; }
        public UnionCard? UnionCard { get; }

        public Musician(Guid id, string fullName, UnionCard? unionCard = null)
        {
            Id = id == Guid.Empty ? throw new ArgumentException("Id must not be empty.", nameof(id)) : id;
            FullName = !string.IsNullOrWhiteSpace(fullName) ? fullName : throw new ArgumentException("FullName required.", nameof(fullName));
            UnionCard = unionCard;
        }

        public override bool Equals(object? obj) => Equals(obj as Musician);
        public bool Equals(Musician? other) => other is not null && Id.Equals(other.Id);
        public override int GetHashCode() => Id.GetHashCode();
    }
}
