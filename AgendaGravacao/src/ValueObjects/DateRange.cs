
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudioSchedule.ValueObjects
{
    public sealed class DateRange : IEquatable<DateRange>
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }

        public DateRange(DateTimeOffset start, DateTimeOffset end)
        {
            if (end <= start)
                throw new ArgumentException("End must be after Start.", nameof(end));

            Start = start;
            End = end;
        }

        public bool Overlaps(DateRange other)
        {
            // Two ranges overlap if they share any time (excluding end==start touching as non-overlap)
            return Start < other.End && other.Start < End;
        }

        public TimeSpan Duration => End - Start;

        public DateRange Move(TimeSpan offset)
        {
            return new DateRange(Start + offset, End + offset);
        }

        public override bool Equals(object? obj) => Equals(obj as DateRange);
        public bool Equals(DateRange? other)
            => other is not null && Start.Equals(other.Start) && End.Equals(other.End);
        public override int GetHashCode() => HashCode.Combine(Start, End);
        public static bool operator ==(DateRange? a, DateRange? b) => EqualityComparer<DateRange>.Default.Equals(a, b);
        public static bool operator !=(DateRange? a, DateRange? b) => !(a == b);
    }
}