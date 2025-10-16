
using System;
using System.Diagnostics.CodeAnalysis;
using EventManagement.Domain.Guards;

namespace EventManagement.Domain.Entities;


public sealed class Event
{
    public int EventId { get; }
    public string Title { get; }


    private string _eventCode = string.Empty;


    [DisallowNull]
    public string EventCode
    {
        get => _eventCode;
        set
        {
            Guard.AgainstNull(value, nameof(EventCode));
            _eventCode = value.Trim();
        }
    }
    public DateTime EventDate { get; }
    public TimeSpan Duration { get; }


    private Venue? _venue;
    private Speaker? _mainSpeaker;


    // Optional fields
    private string? _description;
    private string? _requirements;
    private string? _notes;


    public string Description => _description ?? string.Empty;
    [AllowNull]
    public string Requirements
    {
        get => _requirements ?? string.Empty;
        set => _requirements = value;
    }


    [AllowNull]
    public string Notes
    {
        get => _notes ?? string.Empty;
        set => _notes = value;
    }


    public Speaker? MainSpeaker => _mainSpeaker;


    public Event(int eventId, string title, DateTime eventDate, TimeSpan duration)
    {
        Guard.AgainstNegativeOrZero(eventId, nameof(eventId));
        Guard.AgainstNullOrWhiteSpace(title, nameof(title));
        Guard.AgainstPastDate(eventDate, nameof(eventDate));
        if (duration < TimeSpan.FromMinutes(30))
            throw new ArgumentOutOfRangeException(nameof(duration), "Duration must be at least 30 minutes.");
        EventId = eventId;
        Title = title.Trim();
        EventDate = eventDate;
        Duration = duration;


        // EventCode default is empty string
        _eventCode = string.Empty;
    }


    [MemberNotNull(nameof(_venue))]
    private void EnsureVenueLoaded()
    {
        if (_venue is null)
            _venue = Venue.Default;
    }



    public Venue Venue
    {
        get
        {
            EnsureVenueLoaded();
            return _venue!;
        }
        set => _venue = value ?? throw new ArgumentNullException(nameof(Venue));
    }
    public void SetEventCode(string code)
    {
        Guard.AgainstNull(code, nameof(code));
        _eventCode = code.Trim();
    }



    public void SetDescription(string? description)
    {
        if (Guard.TryParseNonEmpty(description, out var parsed))
            _description = parsed;
        else
            _description = null;
    }



    public void AssignMainSpeaker(Speaker speaker)
    {
        Guard.AgainstNull(speaker, nameof(speaker));
        _mainSpeaker = speaker;
    }
    public override string ToString()
    {
        return $"Event {{ Id={EventId}, Title='{Title}', Date={EventDate:u}, Duration={Duration}, Code='{_eventCode}' }}";
    }
}
