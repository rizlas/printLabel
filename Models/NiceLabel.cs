using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLabel.Models
{
    public class NiceLabel
    {
        public int ID { get; set; }
        public string Macchina { get; set; }
        public string Commessa { get; set; }
        public string Titolo { get; set; }
        public int Lavorazione { get; set; }
        public string DescLavorazione { get; set; }
        public string Segnatura { get; set; }
        public string Stato { get; set; }
        public Nullable<int> QuantitaFatta { get; set; }
        public Nullable<int> QuantitaSuBancale { get; set; }
        public System.DateTimeOffset Data { get; set; }
        public Nullable<int> Bancale { get; set; }
        public string Presenti { get; set; }
        public string Edizione { get; set; }
        public string IPLettore { get; set; }
        public string IDPadre { get; set; }
    }
}
