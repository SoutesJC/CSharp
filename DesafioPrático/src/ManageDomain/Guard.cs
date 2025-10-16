using System;
using System.Diagnostics.CodeAnalysis;


namespace EventManagement.Domain.Guards;


public static class Guard
{
public static void AgainstNull(object? value, string paramName)
{
if (value is null)
throw new ArgumentNullException(paramName);
}


public static void AgainstNullOrEmpty(string? value, string paramName)
{
if (string.IsNullOrEmpty(value))
throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
}


public static void AgainstNullOrWhiteSpace(string? value, string paramName)
{
if (string.IsNullOrWhiteSpace(value))
throw new ArgumentException($"{paramName} cannot be null, empty or whitespace.", paramName);
}


public static void AgainstNegativeOrZero(int value, string paramName)
{
if (value <= 0)
throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be greater than zero.");
}


public static void AgainstPastDate(DateTime date, string paramName)
{
if (date < DateTime.Now)
throw new ArgumentException($"{paramName} cannot be in the past.", paramName);
}


public static bool IsValidEmail(string? email)
{
return !string.IsNullOrWhiteSpace(email) && email.Contains('@');
}


// TryParseNonEmpty: returns true if value is not null/empty/whitespace and outputs trimmed value
public static bool TryParseNonEmpty(string? value, out string? result)
{
if (string.IsNullOrWhiteSpace(value))
{
result = null;
return false;
}


result = value.Trim();
return true;
}
}