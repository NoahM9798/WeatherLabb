using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    //Modell för väderdatal, all data i min tabell har denna formen
    public class Weather
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public string Plats { get; set; }
        public double Temp { get; set; }
        public double Luftfuktighet { get; set; }

    }
}
