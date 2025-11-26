using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Core.Models;
namespace DataAccess
{
    public class CsvService
    {
        public string fileName
        {
            get
            {
                var basePath = Path.GetDirectoryName(typeof(EFContext).Assembly.Location);

                var builder = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("Config.json", optional: false, reloadOnChange: true);

                var config = builder.Build();

                return config["CSVFileName"];
            }
        }
        public IEnumerable<Weather> ReadWeatherCsv(string? file = null)
        {
            if (file == null)
            {
                // Användaren angav ingen fil, använd standardfilen från konfigurationen
                file = fileName;
            }
            var basePath = Path.GetDirectoryName(typeof(CsvService).Assembly.Location);
            var fullPath = Path.Combine(basePath, file);
            var csvText = File.ReadAllText(fullPath).Replace("−", "-"); // Ersätter felaktiga minustecken, programmet kunde inte köra annars.


            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ","
            };

            using var reader = new StringReader(csvText);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<WeatherMap>();

            return csv.GetRecords<Weather>().ToList();
        }


        public sealed class WeatherMap : ClassMap<Weather>
        {
            public WeatherMap()
            {
                Map(m => m.Datum).Name("Datum");
                Map(m => m.Plats).Name("Plats");
                Map(m => m.Temp).Name("Temp");
                Map(m => m.Luftfuktighet).Name("Luftfuktighet");
            }
        }

    }
}

