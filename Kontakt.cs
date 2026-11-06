using System;
using System.Collections.Generic;
using System.Text;

namespace Kontaktverwaltung
{
    internal class Kontakt
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        
        public Kontakt(int id, string name, string telefon)
        {
            Id = id;
            Name = name;
            Telefon = telefon;
        }


    }
}
