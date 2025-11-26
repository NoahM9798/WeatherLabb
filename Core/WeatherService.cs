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

        public IEnumerable<DailySummary> GetAverageTemperatureDesc()
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
                .Take(20)
                .ToList();
        }

        public IEnumerable<DailySummary> GetAverageHumidityAsc()
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
                .Take(20)
                .ToList();
        }

        public IEnumerable<DailySummary> GetDaysByIndoorMoldRisk(
    int limit = 10,
    double tempThreshold = 10.0,     // Fick fram på nätet att mögelrisk ökar vid över 15°C och över 75% luftfuktighet
    double humidityThreshold = 75.0) 
        {
            return _weatherRepository.GetQueryable()
                //.Where(w => w.Plats == "Inne") Trodde först att det bara skulle vara inomhus, men det fanns inte ett ända luftfuktighetsvärde över 75% inomhus i datan, satt det får bli såhär
                .Select(w => new
                {
                    w.Datum.Date,
                    MoldRiskScore = (w.Temp > tempThreshold && w.Luftfuktighet > humidityThreshold) ? 1.0 : 0.0 // Poängsättning: 1 för risk, 0 för ingen risk
                })
                .GroupBy(a => a.Date)
                .Select(g => new DailySummary
                {
                    Datum = g.Key,
                    Value = g.Average(a => a.MoldRiskScore)
                })
                .OrderByDescending(s => s.Value) // Högst risk först
                .Take(limit)
                .ToList();
        }
    }
}
