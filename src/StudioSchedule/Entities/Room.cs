// File: src/StudioSchedule/Domain/Entities/Room.cs
#nullable enable
using System;

namespace StudioSchedule.Entities
{
    public sealed class Room : IEquatable<Room>
    {
        public Guid Id { get; }
        public string Name { get; }

        public Room(Guid id, string name)
        {
            Id = id == Guid.Empty ? throw new ArgumentException("Id must not be empty.", nameof(id)) : id;
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Name required.", nameof(name));
        }

        public override bool Equals(object? obj) => Equals(obj as Room);
        public bool Equals(Room? other) => other is not null && Id.Equals(other.Id);
        public override int GetHashCode() => Id.GetHashCode();
    }
}
