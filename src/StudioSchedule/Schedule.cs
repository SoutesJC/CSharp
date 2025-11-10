

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using StudioSchedule.Entities;
using StudioSchedule.ValueObjects;
using StudioSchedule.Exceptions;

namespace StudioSchedule
{
    public sealed class Schedule
    {
        private readonly List<Session> _sessions = new();
        public IReadOnlyList<Session> Sessions => _sessions.AsReadOnly();

        public void AddSession(Session session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            // ensure the session's date range is valid (DateRange already enforces end>start)
            // ensure no overlapping sessions in the same room
            var overlapping = _sessions.FirstOrDefault(s => s.Room.Equals(session.Room) && s.When.Overlaps(session.When));
            if (overlapping != null)
                throw new OverlappingSessionException($"Session overlaps with existing session (Id={overlapping.Id}) in room {session.Room.Name}.");

            _sessions.Add(session);
        }

        public void RemoveSession(Guid sessionId) => _sessions.RemoveAll(s => s.Id == sessionId);

        public void MoveSession(Guid sessionId, DateRange newRange)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == sessionId) ?? throw new ArgumentException("Session not found.", nameof(sessionId));

            // check overlap with other sessions in same room
            var conflict = _sessions.FirstOrDefault(s => s.Id != sessionId && s.Room.Equals(session.Room) && s.When.Overlaps(newRange));
            if (conflict != null)
                throw new OverlappingSessionException($"Reschedule causes overlap with session (Id={conflict.Id}).");

            session.Reschedule(newRange);
        }
    }
}