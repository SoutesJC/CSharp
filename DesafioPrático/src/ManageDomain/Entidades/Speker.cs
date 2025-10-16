using System;
using System.Diagnostics.CodeAnalysis;
using EventManagement.Domain.Guards;


namespace EventManagement.Domain.Entities;


public sealed class Speaker
{
public int SpeakerId { get; }
public string FullName { get; }
public string Email { get; }


private string? _biography;
private string? _company;
private string? _linkedInProfile;


public string Biography => _biography ?? string.Empty;


[AllowNull]
public string Company
{
get => _company ?? string.Empty;
set => _company = value;
}


[AllowNull]
public string LinkedInProfile
{
get => _linkedInProfile ?? string.Empty;
set => _linkedInProfile = value;
}


public Speaker(int speakerId, string fullName, string email)
{
Guard.AgainstNegativeOrZero(speakerId, nameof(speakerId));
Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
if (!Guard.IsValidEmail(email))
throw new ArgumentException("Email must be a valid email containing '@'.", nameof(email));


SpeakerId = speakerId;
FullName = fullName.Trim();
Email = email.Trim();
}


public void SetBiography(string? biography)
{
if (Guard.TryParseNonEmpty(biography, out var parsed))
{
_biography = parsed;
}
else
{
_biography = null; // allowed
}
}


public override bool Equals(object? obj)
{
if (obj is Speaker other)
return SpeakerId == other.SpeakerId;
return false;
}


public override int GetHashCode() => SpeakerId.GetHashCode();


public override string ToString()
{
return $"Speaker {{ Id={SpeakerId}, Name='{FullName}', Email='{Email}' }}";
}
}