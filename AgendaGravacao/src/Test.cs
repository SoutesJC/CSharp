// File: tests/StudioSchedule.Tests/ScheduleTests.cs
#nullable enable
using System;
using StudioSchedule.Entities;
using StudioSchedule.ValueObjects;
using StudioSchedule.Domain.Exceptions;
using System.Linq;

namespace StudioSchedule.Tests
{
    public class ScheduleTests
    {
        private Room CreateRoom() => new Room(Guid.NewGuid(), "Room A");
        private Musician CreateMusician(string name) => new Musician(Guid.NewGuid(), name);

        [Fact]
        public void AddSession_Succeeds_When_NoOverlap()
        {
            var schedule = new Schedule();
            var room = CreateRoom();
            var m1 = CreateMusician("Alice");
            var r1 = new DateRange(DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(3));
            var session1 = new Session(Guid.NewGuid(), room, r1, new []{ m1 });

            schedule.AddSession(session1);

            Assert.Single(schedule.Sessions);
        }

        [Fact]
        public void AddSession_Throws_OnOverlap_SameRoom()
        {
            var schedule = new Schedule();
            var room = CreateRoom();
            var m1 = CreateMusician("A");
            var m2 = CreateMusician("B");

            var start = DateTimeOffset.UtcNow.AddDays(1);
            var s1 = new DateRange(start, start.AddHours(2));
            var s2 = new DateRange(start.AddHours(1), start.AddHours(3)); // overlaps

            var session1 = new Session(Guid.NewGuid(), room, s1, new []{m1});
            var session2 = new Session(Guid.NewGuid(), room, s2, new []{m2});

            schedule.AddSession(session1);
            Assert.Throws<OverlappingSessionException>(() => schedule.AddSession(session2));
        }

        [Fact]
        public void AddSession_AllowsAdjacentSessions_TouchingEdges()
        {
            var schedule = new Schedule();
            var room = CreateRoom();
            var m1 = CreateMusician("A");
            var m2 = CreateMusician("B");

            var start = DateTimeOffset.UtcNow.AddDays(2);
            var s1 = new DateRange(start, start.AddHours(2));
            var s2 = new DateRange(start.AddHours(2), start.AddHours(4)); // adjacent, should not overlap

            var session1 = new Session(Guid.NewGuid(), room, s1, new []{m1});
            var session2 = new Session(Guid.NewGuid(), room, s2, new []{m2});

            schedule.AddSession(session1);
            schedule.AddSession(session2);

            Assert.Equal(2, schedule.Sessions.Count);
        }

        [Fact]
        public void Session_Cannot_Have_Duplicate_Participants()
        {
            var room = CreateRoom();
            var musician = CreateMusician("Dup");
            var range = new DateRange(DateTimeOffset.UtcNow.AddHours(5), DateTimeOffset.UtcNow.AddHours(7));

            Assert.Throws<InvalidSessionParticipantsException>(() =>
                new Session(Guid.NewGuid(), room, range, new []{ musician, musician })
            );
        }

        [Fact]
        public void Session_AddParticipant_Throws_OnDuplicate()
        {
            var room = CreateRoom();
            var m1 = CreateMusician("X");
            var m2 = CreateMusician("Y");
            var range = new DateRange(DateTimeOffset.UtcNow.AddHours(5), DateTimeOffset.UtcNow.AddHours(7));
            var session = new Session(Guid.NewGuid(), room, range, new []{ m1 });

            session.AddParticipant(m2);
            Assert.Throws<InvalidSessionParticipantsException>(() => session.AddParticipant(m2));
        }

        [Fact]
        public void DateRange_Invalid_EndBeforeStart_Throws()
        {
            var now = DateTimeOffset.UtcNow;
            Assert.Throws<ArgumentException>(() => new DateRange(now.AddHours(1), now));
        }

        [Fact]
        public void MovingSession_Throws_When_Overlap_Created()
        {
            var schedule = new Schedule();
            var room = CreateRoom();
            var s1Start = DateTimeOffset.UtcNow.AddDays(3);
            var s1 = new DateRange(s1Start, s1Start.AddHours(2));
            var s2 = new DateRange(s1Start.AddHours(3), s1Start.AddHours(5));

            var session1 = new Session(Guid.NewGuid(), room, s1, new []{ CreateMusician("A") });
            var session2 = new Session(Guid.NewGuid(), room, s2, new []{ CreateMusician("B") });

            schedule.AddSession(session1);
            schedule.AddSession(session2);

            // try to move session2 to overlap with session1
            var overlappingRange = new DateRange(s1Start.AddHours(1), s1Start.AddHours(3));
            Assert.Throws<OverlappingSessionException>(() => schedule.MoveSession(session2.Id, overlappingRange));
        }
    }
}