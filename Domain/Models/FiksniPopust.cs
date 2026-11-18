using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class FiksniPopust : Zapis
    {
        public string SifraKupona { get; set; } = string.Empty;
        public DateTime DatumVazenja { get; set; }
        public decimal Iznos { get; set; }
    }
}
