using Biletall.Web.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biletall.Web.Models
{
    public class PNRSonuc
    {
        public Sefer Sefer { get; set; }
        public List<Rezervasyon> Rezervasyonlar { get; set; }
     
        public List<Rezervasyon> PnrNo { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
       

    }
}
