
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Spectre.Console;
//Bool för att avsluta programmet
bool running = true;
//Skapa databasen om den inte finns
EFContext.Initialize();

//Skapa EFContext o Repository
using var context = new EFContext();
var weatherRepo = new WeatherRepository(context);

//Läs in CSV via CsvService
var csvService = new CsvService();
// Om man vill kan man skicka in en egen fil här, annars används standardfilen från config, i detta fall kör vi standardfilen
var weatherData = csvService.ReadWeatherCsv();

//Lägg in datan i SQL-tabellen, om det inte redan finns data där
if (!context.Weathers.Any())
{
    Console.WriteLine("Skapar databas och lägger in värden...");
    weatherRepo.AddRange(weatherData);
}
Console.WriteLine("Databas skapad, om den inte redan fanns!");
//Skapa service för att kunna kalla på metoderna
var service = new Core.WeatherService(weatherRepo);



while (running)
{
    Console.Clear();
    var choice = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Välj vilken uppgift du vill köra:\nOBS det visar max 20 resultat, det blir alldeles för mycket annars")
        .PageSize(10)
        .AddChoices(new[]
        {
            "1. Hitta medeltemperatur för en specifik dag",
            "2. Hitta medeltemperatur för alla dagar sorterat fallande",
            "3. Hitta medelfuktigheten per dag stigande",
            "4. Hitta mögelrisken per dag fallande",
            "5. Avsluta"
        }));
    switch (choice)
    {
        case "1. Hitta medeltemperatur för en specifik dag":
            var dateInput = AnsiConsole.Ask<DateTime>("Ange datum (ÅÅÅÅ-MM-DD):");
            service.GetAverageTemperatureForDate(dateInput);
            var TemperatureDate = service.GetAverageTemperatureForDate(dateInput);
            if (TemperatureDate != 0)
            {
                    Console.WriteLine($"Datum: {dateInput.ToLongDateString()}, Medeltemperatur: {TemperatureDate:F2}");
            }
            else
            {
                Console.WriteLine("Inga data hittades för det angivna datumet.");
            }
            Console.ReadLine();
            break;
        case "2. Hitta medeltemperatur för alla dagar sorterat fallande":
            var AverageTemperature = service.GetAverageTemperatureDesc();
            foreach (var day in AverageTemperature)
            {
                Console.WriteLine($"Datum: {day.Datum.ToLongDateString()}, Medeltemperatur: {day.Value:F2}");
            }
            Console.ReadLine();
            break;
        case "3. Hitta medelfuktigheten per dag stigande":
            var AverageHumidity = service.GetAverageHumidityAsc();
            foreach (var day in AverageHumidity)
            {
                Console.WriteLine($"Datum: {day.Datum.ToLongDateString()}, Medelfuktighet: {day.Value:F2}");
            }
            Console.ReadLine();
            break;
        case "4. Hitta mögelrisken per dag fallande":
            var MoldRiskDays = service.GetDaysByMoldRisk();
            foreach (var day in MoldRiskDays)
            {
                Console.WriteLine($"Datum: {day.Datum.ToLongDateString()}, Mögelriskpoäng: {day.Value:F2}");
            }
            Console.ReadLine();
            break;
        case "5. Avsluta":
            running = false;
            break;
    }
}
