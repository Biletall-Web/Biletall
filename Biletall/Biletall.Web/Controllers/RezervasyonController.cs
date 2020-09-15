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
using Microsoft.CodeAnalysis.FlowAnalysis;

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
        public IActionResult Index(string SeferTakipNo, string Koltuklar, string nereden, string nereye, DateTime tarih, string toplamTutar)
        {
            //sefer getir
            var seferler = BiletAllService.SeferleriGetir(nereden, nereye, tarih);
            ViewData["Seferler"] = seferler.FirstOrDefault(n => n.SeferTakipNo == SeferTakipNo);
            ViewBag.Nereden = nereden;
            ViewBag.Nereye = nereye;
            ViewBag.Tarih = tarih;

            //koltuklar,fiyat ve cinsiyetler
            ViewBag.Koltuklar = Koltuklar;

            var koltuklar = Koltuklar.Split(',').ToList();
            var internetFiyatı = Int32.Parse(toplamTutar) / koltuklar.Count;
            koltuklar = koltuklar.Distinct().ToList();
            ViewBag.ToplamTutar = internetFiyatı * koltuklar.Count;
            var erkekler = koltuklar.Where(n => n.EndsWith("E")).Select(n => n.Split('-')[0]).ToList();
            ViewBag.erkekKoltuklar = erkekler;
            var kadinlar = koltuklar.Where(n => n.EndsWith("K")).Select(n => n.Split('-')[0]).ToList();
            ViewBag.kadinKoltuklar = kadinlar;


            return View("Index", "Rezervasyon");
        }

        [HttpPost]

        public ActionResult MusteriBilgileri(RezervasyonModel rezervasyonModel, string[] dynamicField)
        {
            ViewBag.Data = string.Join(",", dynamicField ?? new string[] { });

            var koltuklar = rezervasyonModel.Koltuklar.Split(',').ToList();
            koltuklar = koltuklar.Distinct().ToList();

            var seferler = BiletAllService.SeferleriGetir(rezervasyonModel.Nereden, rezervasyonModel.Nereye, rezervasyonModel.Tarih);
            var sefer = seferler.FirstOrDefault(n => n.SeferTakipNo == rezervasyonModel.SeferTakipNo);
            var erkekler = koltuklar.Where(n => n.EndsWith("E")).Select(n => n.Split('-')[0]).ToList();
            ViewBag.erkekKoltuklar = erkekler;

            var kadinlar = koltuklar.Where(n => n.EndsWith("K")).Select(n => n.Split('-')[0]).ToList();
            ViewBag.kadinKoltuklar = kadinlar;

            int i = 0;
            var rezervasyonlar = new List<Rezervasyon>();
            foreach (var item in erkekler)
            {
                //koltuk yolcu bilgileri
                var pnr = BiletAllService.Rezervasyon(sefer, "2", dynamicField[i], dynamicField[i + 1], dynamicField[i + 2], item, dynamicField[dynamicField.Length - 1], dynamicField[dynamicField.Length - 2]);
                Rezervasyon rezervasyon = new Rezervasyon
                {
                    Mail = dynamicField[dynamicField.Length - 2],
                    PhoneNumber = dynamicField[dynamicField.Length - 1],
                    Name = dynamicField[i],
                    Surname = dynamicField[i + 1],
                    Tc = dynamicField[i + 2],
                    PNR = pnr,
                    FirmaAdi = rezervasyonModel.FirmaAdi,
                    KalkisSaati = rezervasyonModel.KalkisSaati,
                    //Koltuk = erkekler,
                    YaklasikSeyahatSuresi = rezervasyonModel.YaklasikSeyahatSuresi,
                    VarisNokta = rezervasyonModel.VarisNokta,
                    KalkisNokta = rezervasyonModel.KalkisNokta,
                    Tarih = rezervasyonModel.Tarih.ToString(),
                    SeferNo = rezervasyonModel.SeferTakipNo,
                };
                _context.Rezervasyonlar.Add(rezervasyon);
                _context.SaveChanges();
                rezervasyonlar.Add(rezervasyon);

                i += 3;
            }
            foreach (var item in kadinlar)
            {
                //koltuk yolcu bilgileri
                var pnr = BiletAllService.Rezervasyon(sefer, "1", dynamicField[i], dynamicField[i + 1], dynamicField[i + 2], item, dynamicField[dynamicField.Length - 1], dynamicField[dynamicField.Length - 2]);
                Rezervasyon rezervasyon = new Rezervasyon
                {
                    Mail = dynamicField[dynamicField.Length - 2],
                    PhoneNumber = dynamicField[dynamicField.Length - 1],
                    Name = dynamicField[i],
                    Surname = dynamicField[i + 1],
                    Tc = dynamicField[i + 2],
                    PNR = pnr,
                    FirmaAdi = rezervasyonModel.FirmaAdi,
                    KalkisSaati = rezervasyonModel.KalkisSaati,
                    //Koltuk = kadinlar,
                    YaklasikSeyahatSuresi = rezervasyonModel.YaklasikSeyahatSuresi,
                    VarisNokta = rezervasyonModel.VarisNokta,
                    KalkisNokta = rezervasyonModel.KalkisNokta,
                    Tarih = rezervasyonModel.Tarih.ToString(),
                    SeferNo = rezervasyonModel.SeferTakipNo,
                };
                _context.Rezervasyonlar.Add(rezervasyon);
                _context.SaveChanges();
                rezervasyonlar.Add(rezervasyon);

                i += 3;
            }
            var model = new PNRSonuc { Rezervasyonlar = rezervasyonlar, Sefer = sefer, Email = dynamicField[dynamicField.Length - 2], Telefon = dynamicField[dynamicField.Length - 1] };
            return View("PNRSonuc", model);
        }
        public ActionResult PNRSonuc()
        {
            return View();
        }
        public JsonResult Result(List<object> modeller)
        {
            //
            foreach (var model in modeller)
            {

            }
            return Json(null);
        }
        public ActionResult PnrBul(RezervasyonModel rezervasyonModel, string pnrNo)
        {
            if (pnrNo == null || rezervasyonModel==null)
            {
                return View("PnrBul");
            }
          
            var rezervasyon = _context.Rezervasyonlar.Where(ap => ap.PNR == pnrNo).ToList();

            PNRSonuc model = new PNRSonuc()
            {
               PnrNo = rezervasyon
            };
            return View("PnrGetir", model);

        }
    }
}
