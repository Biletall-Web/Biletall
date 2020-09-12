using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Biletall.Web.Models;
using System.Xml;
using ServiceReference1;
using Biletall.Web.BusinesLogic;
using System.Xml.Schema;
using System.Linq;
using Biletall.Web.Data.Entity;
using Biletall.Web.Data;

namespace Biletall.Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
      
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var karaNoktalari = BiletAllService.KaraNoktalariGetir();
            ViewBag.Cities = karaNoktalari;
            return View();
        }
        [HttpPost]
        public IActionResult Index(string nereden, string nereye, string tarih)
        {
            DateTime seferTarihi = new DateTime(Convert.ToInt32(tarih.Split('/')[2]), Convert.ToInt32(tarih.Split('/')[0]), Convert.ToInt32(tarih.Split('/')[1]));
            var seferler = BiletAllService.SeferleriGetir(nereden, nereye, seferTarihi);
            if (seferler.Count > 0)
            {
                ViewData["Seferler"] = seferler;
                return RedirectToAction("Seferler", "Home", new { nereden = nereden, nereye = nereye, tarih = seferTarihi });
            }
            else
            {
                var sonuc = "Sefer bulunamadı!";
                var karaNoktalari = BiletAllService.KaraNoktalariGetir();
                ViewBag.Cities = karaNoktalari;
                ViewBag.Sonuc = sonuc;
                return View();
            }

        }

        public IActionResult Seferler(string nereden, string nereye, DateTime tarih, bool tariheGoreSirala = false, bool fiyataGoreSırala = false, bool ucluKoltukFiltre = false, bool ikiliKoltukFiltre = false)
        {
            ViewBag.Nereden = nereden;
            ViewBag.Nereye = nereye;
            ViewBag.Tarih = tarih;

            ViewBag.UcluKoltukFiltre = ucluKoltukFiltre;
            ViewBag.IkiliKoltukFiltre = ikiliKoltukFiltre;
            //Seferleri getiren servis
            var seferler = BiletAllService.SeferleriGetir(nereden, nereye, tarih);
            SeferViewModal modal = new SeferViewModal
            {
                Seferler = seferler
            };
            if (tariheGoreSirala)
            {
                seferler = seferler.OrderBy(n => n.KalkisSaati).ToList();
            }
            if (fiyataGoreSırala)
            {
                seferler = seferler.OrderBy(n => n.BiletFiyatiInternet).ToList();
            }
            if (ucluKoltukFiltre)
            {
                seferler = seferler.Where(n => n.OtobusKoltukYerlesimTipi == "2+1").ToList();
            }
            if (ikiliKoltukFiltre)
            {
                seferler = seferler.Where(n => n.OtobusKoltukYerlesimTipi == "1+1").ToList();
            }
            ViewData["Seferler"] = seferler;
            return View("Seferler");

        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
