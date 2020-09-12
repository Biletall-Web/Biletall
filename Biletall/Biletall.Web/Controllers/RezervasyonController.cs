using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biletall.Web.BusinesLogic;
using Biletall.Web.Data;
using Biletall.Web.Data.Entity;
using Biletall.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Biletall.Web.Controllers
{

    public class RezervasyonController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RezervasyonController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult Index(string SeferTakipNo,string Koltuklar, string nereden,string nereye,DateTime tarih,string toplamTutar)
        {
            //sefer getir
            var seferler = BiletAllService.SeferleriGetir(nereden, nereye, tarih);
            ViewData["Seferler"] = seferler.FirstOrDefault(n=>n.SeferTakipNo == SeferTakipNo);
            ViewBag.Nereden = nereden;
            ViewBag.Nereye = nereye;
            ViewBag.Tarih = tarih;

            //koltuklar,fiyat ve cinsiyetler
            ViewBag.Koltuklar = Koltuklar;

            var koltuklar = Koltuklar.Split(',').ToList();
            var internetFiyatı = Int32.Parse(toplamTutar) / koltuklar.Count;
            koltuklar = koltuklar.Distinct().ToList();
            ViewBag.ToplamTutar = internetFiyatı * koltuklar.Count;
            var erkekler = koltuklar.Where(n => n.EndsWith("E")).Select(n=>n.Split('-')[0]).ToList();
            ViewBag.erkekKoltuklar = erkekler;
            var kadinlar = koltuklar.Where(n => n.EndsWith("K")).Select(n => n.Split('-')[0]).ToList();
            ViewBag.kadinKoltuklar = kadinlar;


            return View("Index", "Rezervasyon");
        }
        [HttpPost]
        public JsonResult SaveList(List<String> values)
        {
            return Json(new { Result = String.Format("Fist item in list: '{0}'", values[0]) });
        }

        [HttpPost]
        public ActionResult Index2(string[] dynamicField, string SeferTakipNo, string Koltuklar, string nereden, string nereye, DateTime tarih)
        {
            
            ViewBag.Data = string.Join(",", dynamicField ?? new string[] { });

            var koltuklar = Koltuklar.Split(',').ToList();
            koltuklar = koltuklar.Distinct().ToList();

            var seferler = BiletAllService.SeferleriGetir(nereden, nereye, tarih);
            var sefer = seferler.FirstOrDefault(n => n.SeferTakipNo == SeferTakipNo);
            var erkekler = koltuklar.Where(n => n.EndsWith("E")).Select(n => n.Split('-')[0]).ToList();
            ViewBag.erkekKoltuklar = erkekler;
            var kadinlar = koltuklar.Where(n => n.EndsWith("K")).Select(n => n.Split('-')[0]).ToList();
            ViewBag.kadinKoltuklar = kadinlar;
            int i = 0;
            var rezervasyonlar = new List<Rezervasyon>();
            foreach (var item in erkekler)
            {
                //koltuk yolcu bilgileri
                var pnr = BiletAllService.Rezervasyon(sefer,"2", dynamicField[i], dynamicField[i+1], dynamicField[i+2], item,dynamicField[dynamicField.Length-1], dynamicField[dynamicField.Length - 2]);
                Rezervasyon rezervasyon = new Rezervasyon 
                {
                Mail = dynamicField[dynamicField.Length - 2],
                PhoneNumber = dynamicField[dynamicField.Length - 1],
                Name= dynamicField[i],
                Surname= dynamicField[i+1],
                Tc = dynamicField[i + 2],
                PNR = pnr
                };
                _context.Rezervasyonlar.Add(rezervasyon); 
                _context.SaveChanges();
                rezervasyonlar.Add(rezervasyon);

                i += 3;
            }
            foreach (var item in kadinlar)
            {
                //koltuk yolcu bilgileri
                var pnr = BiletAllService.Rezervasyon(sefer,"1",dynamicField[i], dynamicField[i + 1], dynamicField[i + 2], item, dynamicField[dynamicField.Length - 1], dynamicField[dynamicField.Length - 2]);
                Rezervasyon rezervasyon = new Rezervasyon
                {
                    Mail = dynamicField[dynamicField.Length - 2],
                    PhoneNumber = dynamicField[dynamicField.Length - 1],
                    Name = dynamicField[i],
                    Surname = dynamicField[i + 1],
                    Tc = dynamicField[i + 2],
                    PNR = pnr,

                };
                _context.Rezervasyonlar.Add(rezervasyon);
                _context.SaveChanges();
                rezervasyonlar.Add(rezervasyon);

                i += 3;
            }
            var model = new PNRSonuc { Rezervasyonlar = rezervasyonlar, Sefer = sefer,Email= dynamicField[dynamicField.Length - 2] ,Telefon= dynamicField[dynamicField.Length - 1] };
            return View("PNRSonuc", model);
        }
        public ActionResult PNRSonuc()
        {
            return View();
        }
    }
}
