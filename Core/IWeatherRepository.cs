using System.Collections.Generic;
using Core.Models;

namespace Core
{
    //Interface för Weather objektet, min lösning för dataåtkomst så Core inte är beroende av DataAccess
    public interface IWeatherRepository
    {
        //Denna kommer behövas för att få alla Weather items, men går snabbare än min föra GetAll-metod
        IQueryable<Weather> GetQueryable();
        void AddRange(IEnumerable<Weather> items);
        //Denna kommer behövas för att få en specifik Weather för ett datum, lättast hittad genom SQL i DataAccess där jag har EFContext
        IEnumerable<Weather> GetByDate(DateTime date);
    }
}