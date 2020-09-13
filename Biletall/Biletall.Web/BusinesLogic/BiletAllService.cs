using Biletall.Web.Controllers;
using Biletall.Web.Models;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Biletall.Web.BusinesLogic
{
    public class BiletAllService
    {
        public static string yetki = "<Kullanici><Adi>" + "stajyerWS" + "</Adi><Sifre>" + "2324423WSs099"
   + "</Sifre></Kullanici>";

        public static XmlIsletRequestBody ServistenCevapGetir()
        {
            XmlIsletRequestBody xirb = new XmlIsletRequestBody();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(yetki);
            xirb.xmlYetki = xml.DocumentElement;
            return xirb;
        }
        public static List<KaraNokta> KaraNoktalariGetir()
        {
            
            XmlIsletRequestBody isletRequestBody = ServistenCevapGetir();
            XmlDocument requestXml = new XmlDocument();
            requestXml.LoadXml(@"<KaraNoktaGetirKomut/>");
            isletRequestBody.xmlIslem = requestXml.DocumentElement;
            var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap).XmlIslet(isletRequestBody.xmlIslem, isletRequestBody.xmlYetki);
            List<KaraNokta> karaNoktaList = new List<KaraNokta>();
            XmlNodeList xnList = service.SelectNodes("/KaraNokta");
            foreach (XmlNode xn in xnList)
            {
                KaraNokta karaNokta = new KaraNokta
                {
                    ID = xn["ID"].InnerText,
                    Ad = xn["Ad"].InnerText,
                    Aciklama = xn["Aciklama"].InnerText,
                    BagliOlduguNoktaID = xn["BagliOlduguNoktaID"].InnerText,
                    Bolge = xn["Bolge"].InnerText,
                    MerkezMi = xn["MerkezMi"].InnerText,
                    SeyahatSehirID = xn["SeyahatSehirID"].InnerText
                };
                if (karaNokta.MerkezMi == "1")
                {
                    karaNoktaList.Add(karaNokta);
                }
            }
            return karaNoktaList;
        }

        public static List<Sefer> SeferleriGetir(string nereden, string nereye, DateTime tarih)
        {

            XmlIsletRequestBody isletRequestBody = ServistenCevapGetir();
            XmlDocument requestXml = new XmlDocument();
            requestXml.LoadXml(@"<Sefer><FirmaNo>0</FirmaNo><KalkisNoktaID>" + nereden + "</KalkisNoktaID><VarisNoktaID>" + nereye +
                "</VarisNoktaID><Tarih>" + tarih.ToString("yyyy-MM-dd") + "</Tarih><AraNoktaGelsin>1</AraNoktaGelsin><IslemTipi>0</IslemTipi><YolcuSayisi>1</YolcuSayisi><Ip>127.0.0.1</Ip></Sefer>");
            isletRequestBody.xmlIslem = requestXml.DocumentElement;
            List<Sefer> seferList = new List<Sefer>();

            var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap).XmlIslet(isletRequestBody.xmlIslem, isletRequestBody.xmlYetki);
            try
            {
                XmlNodeList xnList = service.SelectNodes("/Table");
                foreach (XmlNode xn in xnList)
                {
                    Sefer sefer = new Sefer
                    {
                        ID = xn["ID"].InnerText,
                        BiletFiyati1 = Convert.ToInt32(xn["BiletFiyati1"].InnerText),
                        BiletFiyatiInternet = Convert.ToInt32(xn["BiletFiyatiInternet"].InnerText),
                        FirmaAdi = xn["FirmaAdi"].InnerText,
                        KalkisNokta = xn["KalkisNokta"].InnerText,
                        OTipOzellik = xn["OTipOzellik"].InnerText,
                        VarisNokta = xn["VarisNokta"].InnerText,
                        YaklasikSeyahatSuresi = xn["YaklasikSeyahatSuresi"].InnerText,
                        OtobusKoltukYerlesimTipi = xn["OtobusKoltukYerlesimTipi"].InnerText,
                        SeferTakipNo = xn["SeferTakipNo"].InnerText,
                        FirmaNo = xn["FirmaNo"].InnerText,
                        KalkisNoktaID = xn["KalkisNoktaID"].InnerText,
                        VarisNoktaID = xn["VarisNoktaID"].InnerText,
                        //Format:(datetime, ‘yyyy-MM-dd’).
                        Tarih = xn["Tarih"].InnerText,
                        Saat = xn["Saat"].InnerText,
                        HatNo = xn["HatNo"].InnerText,
                        SeferNo = xn["SeferTakipNo"].InnerText,
                    };
                    sefer.Guzergahlar = GuzergahlariGetir(nereden, nereye, tarih, sefer.SeferTakipNo);
                    DateTime _date;
                    sefer.KalkisSaati = DateTime.TryParse(xn["Saat"].InnerText, out _date) ? _date.Hour.ToString() + ":" + _date.Minute.ToString("00") : "";
                    seferList.Add(sefer);
                }
                return seferList;
            }
            catch (Exception)
            {
                var sonuc = ((System.Xml.XmlCharacterData)service.SelectNodes("/Sonuc")[0].FirstChild).Data.ToString() != "false";
                return seferList;
            }
        }

        public static List<Guzergah> GuzergahlariGetir(string nereden, string nereye, DateTime tarih, string seferTakipNo)
        {

            XmlIsletRequestBody isletRequestBody = ServistenCevapGetir();
            XmlDocument requestXml = new XmlDocument();
            requestXml.LoadXml(@"<Hat><FirmaNo>37</FirmaNo><HatNo>1</HatNo><KalkisNoktaID>" + nereden + "</KalkisNoktaID><VarisNoktaID>" + nereye + "</VarisNoktaID><BilgiIslemAdi>GuzergahVerSaatli</BilgiIslemAdi>" +
                "<SeferTakipNo>" + seferTakipNo + "</SeferTakipNo><Tarih>" + tarih.ToString("yyyy-MM-dd") + "</Tarih></Hat>");
            isletRequestBody.xmlIslem = requestXml.DocumentElement;

            List<Guzergah> guzergahList = new List<Guzergah>();
            var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap).XmlIslet(isletRequestBody.xmlIslem, isletRequestBody.xmlYetki);
            try
            {
                XmlNodeList xnList = service.SelectNodes("/Table1");
                foreach (XmlNode xn in xnList)
                {
                    Guzergah guzergah = new Guzergah
                    {
                        VarisYeri = xn["VarisYeri"].InnerText,
                        SiraNo = xn["SiraNo"].InnerText,
                        //KalkisTarihSaat = xn["KalkisTarihSaat"].InnerText,
                        //VarisTarihSaat = xn["VarisTarihSaat"].InnerText,
                        KaraNoktaID = xn["KaraNoktaID"].InnerText,
                        KaraNoktaAd = xn["KaraNoktaAd"].InnerText
                    };
                    DateTime _kalkisTarihSaat, _varisTarihSaat;
                    guzergah.KalkisTarihSaat = DateTime.TryParse(xn["KalkisTarihSaat"].InnerText, out _kalkisTarihSaat) ? _kalkisTarihSaat.Hour.ToString() + ":" + _kalkisTarihSaat.Minute.ToString("00") : "";
                    guzergah.VarisTarihSaat = DateTime.TryParse(xn["VarisTarihSaat"].InnerText, out _varisTarihSaat) ? _varisTarihSaat.Hour.ToString() + ":" + _varisTarihSaat.Minute.ToString("00") : "";

                    guzergahList.Add(guzergah);
                }
                return guzergahList;
            }
            catch (Exception)
            {
                return guzergahList;
            }
        }

        public static List<Koltuk> KoltukBilgisiAl(string seferReferans)
        {
            List<Koltuk> koltuklar = new List<Koltuk>();
            XmlIsletRequestBody isletRequestBody = ServistenCevapGetir();
            XmlDocument requestXml = new XmlDocument();
            requestXml.LoadXml(@"<Otobus>
                                     <FirmaNo>37</FirmaNo>
                                     <KalkisNoktaID>738</KalkisNoktaID>
                                     <VarisNoktaID>84</VarisNoktaID>
                                     <Tarih>2020-12-09</Tarih>
                                     <Saat>1900-01-01T02:30:00+02:00</Saat>
                                     <HatNo>1</HatNo>
                                     <IslemTipi>0</IslemTipi>
                                     <SeferTakipNo>" + seferReferans + @"</SeferTakipNo>
                                     <Ip>127.0.0.1</Ip>
                                  </Otobus>");
            isletRequestBody.xmlIslem = requestXml.DocumentElement;

            var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap)
                .XmlIslet(isletRequestBody.xmlIslem, isletRequestBody.xmlYetki);
            XmlNodeList nodeKoltukList = service.SelectNodes("/Koltuk");
            foreach (XmlNode nodeKoltuk in nodeKoltukList)
            {
                koltuklar.Add(new Koltuk
                {
                    KoltukStr = nodeKoltuk["KoltukStr"].InnerText,
                    KoltukNo = nodeKoltuk["KoltukNo"].InnerText,
                    Durum = nodeKoltuk["Durum"].InnerText,
                    DurumYan = nodeKoltuk["DurumYan"].InnerText,
                    KoltukFiyatiInternet = nodeKoltuk["KoltukFiyatiInternet"].InnerText
                });
            }
            return koltuklar;
        }

        public static string Rezervasyon(Sefer sefer, string cinsiyet, string ad, string soyad, string tc, string koltukNo, string tel, string mail)
        {
            XmlIsletRequestBody isletRequestBody = ServistenCevapGetir();
            XmlDocument requestXml = new XmlDocument();
            string gecerliTarih = sefer.Tarih.Substring(0, 10);
            requestXml.LoadXml(@"<IslemRezervasyon>
                                     <FirmaNo>" + sefer.FirmaNo + "</FirmaNo><KalkisNoktaID>" + sefer.KalkisNoktaID + "</KalkisNoktaID>" +
                                   "<VarisNoktaID>" + sefer.VarisNoktaID + "</VarisNoktaID>" +
                                   "<Tarih>" + gecerliTarih + "</Tarih>" +
                                   "<Saat>" + sefer.Saat + "</Saat>" +
                                   "<HatNo>" + sefer.HatNo + "</HatNo>" +
                                   "<SeferNo>" + sefer.SeferNo + "</SeferNo>" +
                                   "<KalkisTerminalAdiSaatleri></KalkisTerminalAdiSaatleri>" +
                                   "<KoltukNo1>" + koltukNo + "</KoltukNo1>" +
                                   "<Adi1>" + ad + "</Adi1>" +
                                   "<Soyadi1>" + soyad + "</Soyadi1>" +
                                   "<TcKimlikNo1>" + tc + "</TcKimlikNo1>" +
                                   "<ServisYeriKalkis1></ServisYeriKalkis1>" +
                                   "<TelefonNo>" + tel + "</TelefonNo>" +
                                   "<Cinsiyet>" + cinsiyet + "</Cinsiyet>" +
                                   "<YolcuSayisi>1</YolcuSayisi>" +
                                  "<FirmaAciklama></FirmaAciklama>" +
                                   "<HatirlaticiNot></HatirlaticiNot>" +
                                  "<WebYolcu>" +
                                   "<WebUyeNo>0</WebUyeNo>" +
                                   "<Ip>0.0.0.0</Ip>" +
                                   "<Email>" + mail + "</Email>" +
                                   "</WebYolcu>" +
                                  "</IslemRezervasyon>");
            isletRequestBody.xmlIslem = requestXml.DocumentElement;
            var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap)
               .XmlIslet(isletRequestBody.xmlIslem, isletRequestBody.xmlYetki);
            return service["PNR"].InnerText;
        }

    }
}
