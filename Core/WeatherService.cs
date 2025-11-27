using Core.Models;

namespace Core
{
    public class WeatherService
    {
        private readonly IWeatherRepository _weatherRepository;

        public WeatherService(IWeatherRepository weatherRepository)
        {
            _weatherRepository = weatherRepository;
        }

        //Metoder som gör uppgifter med väderdata

        public double GetAverageTemperatureForDate(DateTime date)
        {
            //Alla mätningar för den dagen
            var recordsForDay = _weatherRepository.GetByDate(date);

            //Ifall det skulle va null
            if (recordsForDay == null || !recordsForDay.Any())
            {
                return 0;
            }

            //Räknar ut resultatet med hjälp av LINQ
            var indoorRecords = recordsForDay.Where(w => w.Plats == "Inne"); //Filtrera så att det är tempen räknad inomhus
            double averageTemperedTemperature = indoorRecords.Average(w => w.Temp);

            return averageTemperedTemperature;
        }

        public IEnumerable<DailySummary> GetAverageTemperatureDesc(int limit = 20)
        {
            return _weatherRepository.GetQueryable()
                .Where(w => w.Plats == "Inne")
                .GroupBy(w => w.Datum.Date)
                .Select(g => new DailySummary
                {
                    Datum = g.Key,
                    Value = g.Average(w => w.Temp)
                })
                .OrderByDescending(ds => ds.Value)
                .Take(limit)
                .ToList();
        }

        public IEnumerable<DailySummary> GetAverageHumidityAsc(int limit = 20)
        {
            return _weatherRepository.GetQueryable()
                .Where(w => w.Plats == "Ute")
                .GroupBy(w => w.Datum.Date)
                .Select(g => new DailySummary
                {
                    Datum = g.Key,
                    Value = g.Average(w => w.Luftfuktighet)
                })
                .OrderBy(ds => ds.Value)
                .Take(limit)
                .ToList();
        }

        public IEnumerable<DailySummary> GetDaysByMoldRisk(string plats = "Inne", int limit = 20)
        {
            return _weatherRepository.GetQueryable()
                .Where(w => w.Plats == plats)
                .Select(w => new
                {
                    w.Datum.Date,
                    //Fick fram att mögelrisken kan man räkna Riskvärde = Luftfuktighet + Temp * 0.5
                    //Om temperaturen är under 0 grader så är risken 0
                    //I datan vi har så är inte risken direkt hög nån gång inomhus, så om vi kan sätta in utomhus om vi vill se högre riskvärden
                    //Lågt värde är ungefär 30-50, då finns ingen risk
                    RiskScore = (w.Temp < 0) ? 0 : (w.Luftfuktighet * 1.0) + (w.Temp * 0.5)
                })
                .GroupBy(x => x.Date)
                .Select(g => new DailySummary
                {
                    Datum = g.Key,
                    // Vi tar medelvärdet av riskpoängen för hela dagen
                    Value = g.Average(x => x.RiskScore)
                })
                .OrderByDescending(s => s.Value) // Sortera så "farligast" dagar kommer först
                .Take(limit)
                .ToList();
        }
    }
}
