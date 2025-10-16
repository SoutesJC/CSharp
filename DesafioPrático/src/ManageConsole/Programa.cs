using System;
using EventManagement.Domain.Entities;



class Program

{

    static void Main()
    {

        Console.WriteLine("=== Speaker Examples ===");
        try
        {
            var speaker = new Speaker(1, "João Silva", "joao@email.com");

            speaker.SetBiography("Especialista em C# com 10 anos de experiência");

            speaker.Company = null; // allowed, returns empty when get

            speaker.LinkedInProfile = null;
            Console.WriteLine(speaker);

            Console.WriteLine($"Company: '{speaker.Company}' LinkedIn: '{speaker.LinkedInProfile}' Biography: '{speaker.Biography}'");
        }

        catch (Exception ex)
        {

            Console.WriteLine($"Error creating speaker: {ex.Message}");
        }

        Console.WriteLine();

        Console.WriteLine("=== Venue Examples ===");

        var defaultVenue = Venue.Default;

        Console.WriteLine(defaultVenue);

        var venue = new Venue(1, "Centro de Convenções", "Av. Principal, 100", 500);

        venue.SetDescription("Moderno centro com infraestrutura completa");

        venue.ParkingInfo = null; // allowed

        Console.WriteLine(venue);

        Console.WriteLine($"ParkingInfo: '{venue.ParkingInfo}' Description: '{venue.Description}'");


        Console.WriteLine();

        Console.WriteLine("=== Event Examples ===");

        var evento = new Event(1, ".NET Conference 2025", new DateTime(2025, 12, 15), TimeSpan.FromHours(8));
    }
}
