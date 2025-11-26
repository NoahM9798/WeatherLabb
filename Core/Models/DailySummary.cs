using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class DailySummary
    {
        //Det kommer finnas medeltemp sorterad på olika sätt så detta är ett objekt för att hålla reda på datum och värde
        public DateTime Datum { get; set; }
        public double Value { get; set; } //Värde komnmer vara olika beroende på uppgift jag löser
    }
}
