using Biletall.Web.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biletall.Web.Models
{
    public class RezervasyonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Tc { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public string PNR { get; set; }
        public string SeferTakipNo { get; set; }
        public string Koltuklar { get; set; }
        public string Nereden { get; set; }
        public string Nereye { get; set; }
        public DateTime Tarih { get; set; }
        public string FirmaAdi { get; set; }
        public string KalkisSaati { get; set; }
        public string Koltuk { get; set; }
        public string YaklasikSeyahatSuresi { get; set; }
        public string VarisNokta { get; set; }
        public string KalkisNokta { get; set; }


    }
}
