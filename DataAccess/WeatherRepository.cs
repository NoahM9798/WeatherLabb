using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;

namespace DataAccess
{
    // Repository för väderdata
    public class WeatherRepository : IWeatherRepository
    {
        private readonly EFContext _context;

        public WeatherRepository(EFContext context)
        {
            _context = context;
        }

        public IQueryable<Weather> GetQueryable()
        {
            return _context.Weathers;
        }

        public void AddRange(IEnumerable<Weather> items)
        {
            _context.Weathers.AddRange(items);
            _context.SaveChanges();
        }

        public IEnumerable<Weather> GetByDate(DateTime date)
        {
            return _context.Weathers
                .Where(w => w.Datum.Date == date.Date)
                .ToList();
        }
    }
}
