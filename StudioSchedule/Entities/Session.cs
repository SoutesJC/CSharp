// File: src/StudioSchedule/Domain/Entities/Session.cs
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using StudioSchedule.ValueObjects;
using StudioSchedule.Exceptions;

namespace StudioSchedule.Entities
{
    public sealed class Session
    {
        public Guid Id { get; }
        public Room Room { get; }
        public DateRange When { get; private set; }
        public IReadOnlyList<Musician> Participants => _participants.AsReadOnly();

        private readonly List<Musician> _participants = new();

        public Session(Guid id, Room room, DateRange when, IEnumerable<Musician> participants)
        {
            Id = id == Guid.Empty ? throw new ArgumentException("Id must not be empty.", nameof(id)) : id;
            Room = room ?? throw new ArgumentNullException(nameof(room));
            When = when ?? throw new ArgumentNullException(nameof(when));
            if (participants == null) throw new ArgumentNullException(nameof(participants));

            var partList = participants.ToList();
            if (!partList.Any()) throw new InvalidSessionParticipantsException("Session must have at least one participant.");

            // ensure no duplicate musician ids
            var duplicate = partList.GroupBy(p => p.Id).FirstOrDefault(g => g.Count() > 1);
            if (duplicate != null)
                throw new InvalidSessionParticipantsException("Duplicate participants are not allowed.");

            _participants.AddRange(partList);
        }

        public void Reschedule(DateRange newRange)
        {
            if (newRange == null) throw new ArgumentNullException(nameof(newRange));
            When = newRange;
        }

        public void AddParticipant(Musician musician)
        {
            if (musician == null) throw new ArgumentNullException(nameof(musician));
            if (_participants.Any(p => p.Id == musician.Id))
                throw new InvalidSessionParticipantsException("Musician already part of the session.");

            _participants.Add(musician);
        }

        public void RemoveParticipant(Guid musicianId)
        {
            _participants.RemoveAll(m => m.Id == musicianId);
        }
    }
}
