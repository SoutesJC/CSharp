using System;
using System.Diagnostics.CodeAnalysis;
using EventManagement.Domain.Guards;


namespace EventManagement.Domain.Entities;


public sealed class Venue
{
public int VenueId { get; }
public string Name { get; }
public string Address { get; }
public int Capacity { get; }


private string? _description;
private string? _parkingInfo;


public string Description => _description ?? string.Empty;


[AllowNull]
public string ParkingInfo
{
get => _parkingInfo ?? string.Empty;
set => _parkingInfo = value;
}


public static Venue Default { get; } = new Venue(0, "Online Event", "Virtual", int.MaxValue);


public Venue(int venueId, string name, string address, int capacity)
{
if (venueId < 0)
throw new ArgumentOutOfRangeException(nameof(venueId), "VenueId must be >= 0 (0 allowed for Default).");


Guard.AgainstNullOrWhiteSpace(name, nameof(name));
Guard.AgainstNullOrWhiteSpace(address, nameof(address));
Guard.AgainstNegativeOrZero(capacity, nameof(capacity));


VenueId = venueId;
Name = name.Trim();
Address = address.Trim();
Capacity = capacity;
}


public void SetDescription(string? description)
{
if (Guard.TryParseNonEmpty(description, out var parsed))
_description = parsed;
else
_description = null;
}


public override bool Equals(object? obj)
{
if (obj is Venue other)
return VenueId == other.VenueId;
return false;
}


public override int GetHashCode() => VenueId.GetHashCode();


public override string ToString()
{
return $"Venue {{ Id={VenueId}, Name='{Name}', Address='{Address}', Capacity={Capacity} }}";
}
}