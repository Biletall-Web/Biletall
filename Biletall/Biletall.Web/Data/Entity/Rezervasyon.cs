﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Biletall.Web.Data.Entity
{
    public class Rezervasyon
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string Surname { get; set; }
        public string Tc { get; set; }
        public string  Mail { get; set; }
        public string PhoneNumber { get; set; }
        public string PNR { get; set; }
        public string FirmaAdi { get; set; }
        public string KalkisSaati { get; set; }
        
        public string Koltuk { get; set; }
        public string YaklasikSeyahatSuresi { get; set; }
        public string VarisNokta { get; set; }
        public string KalkisNokta { get; set; }
        public string Tarih { get; set; }
        public string SeferNo { get; set; }
        public int PnrId { get; set; }
    }
}
