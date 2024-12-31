using System;
using System.Collections.Generic;

// Soyutlama: Soyut sınıf
public abstract class Urun
{
    public string Ad { get; set; }
    public decimal Fiyat { get; set; }

    // Soyut metod (her türetilmiş sınıfta uygulanacak)
    public abstract void BilgiGoster();
}

public class Kitap : Urun
{
    public string Yazar { get; set; }
    public int Stok { get; set; }

    // Kurucu (Constructor)
    public Kitap(string ad, string yazar, decimal fiyat, int stok)
    {
        Ad = ad;
        Yazar = yazar;
        Fiyat = fiyat;
        Stok = stok;
    }

    // Kapsülleme (Property'ler üzerinden erişim)
    public int GetStok()
    {
        return Stok;
    }

    public void StokGuncelle(int miktar)
    {
        if (Stok - miktar >= 0)
        {
            Stok -= miktar;
        }
        else
        {
            Console.WriteLine("Yeterli stok bulunmamaktadır.");
        }
    }

    // Polimorfizm (BilgiGoster farklı sınıflarda farklı şekilde çalışacak)
    public override void BilgiGoster()
    {
        Console.WriteLine($"{Ad} - {Yazar}, Fiyat: {Fiyat:C}, Stok: {Stok}");
    }
}

// Satış sınıfı
public class Satis
{
    public Kitap SatilanKitap { get; set; }
    public int Miktar { get; set; }
    public decimal ToplamTutar { get; set; }
    public DateTime Tarih { get; set; }

    // Kurucu
    public Satis(Kitap satilanKitap, int miktar)
    {
        SatilanKitap = satilanKitap;
        Miktar = miktar;
        ToplamTutar = SatilanKitap.Fiyat * Miktar;
        Tarih = DateTime.Now;
        SatilanKitap.StokGuncelle(Miktar);
    }

    public void SatisBilgisiGoster()
    {
        Console.WriteLine($"{SatilanKitap.Ad} - {Miktar} adet satıldı, Toplam: {ToplamTutar:C} tarihinde");
    }
}

// Satış raporu sınıfı
public class SatisRaporu
{
    private List<Satis> satislar = new List<Satis>();

    // Kapsülleme: Listeyi dışarıya erişime kapalı tutuyoruz
    public void SatisEkle(Satis satis)
    {
        satislar.Add(satis);
    }

    public void RaporuGoster()
    {
        Console.WriteLine("\n--- Satış Raporu ---");
        foreach (var satis in satislar)
        {
            satis.SatisBilgisiGoster();
        }
    }

    public decimal ToplamGelir()
    {
        decimal toplamGelir = 0;
        foreach (var satis in satislar)
        {
            toplamGelir += satis.ToplamTutar;
        }
        return toplamGelir;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Kitaplar listesi (Kalıtım ve Polimorfizm kullanıldı)
        List<Urun> urunler = new List<Urun>
        {
            new Kitap("Nesneye Yönelik Programlama", "Arzu Kilitci CALAYIR", 69.99m, 100),
            new Kitap("Veri Yapıları Ve Algoritma Analizi", "Ümit ÖZTÜRK", 49.99m, 50),
            new Kitap("Mantıksal Tasarım", "Mustafa YAĞIMLI", 99.99m, 30)
        };

        // Satış raporu nesnesi
        SatisRaporu satisRaporu = new SatisRaporu();

        // Kullanıcıya seçenekler sunuluyor
        while (true)
        {
            Console.WriteLine("\n--- Kitap Yönetim Sistemi ---");
            Console.WriteLine("1. Kitapları Göster");
            Console.WriteLine("2. Kitap Sat");
            Console.WriteLine("3. Satış Raporunu Göster");
            Console.WriteLine("4. Çıkış");
            Console.Write("Bir seçenek girin: ");
            string secim = Console.ReadLine();

            switch (secim)
            {
                case "1":
                    KitaplariGoster(urunler);
                    break;
                case "2":
                    KitapSat(urunler, satisRaporu);
                    break;
                case "3":
                    satisRaporu.RaporuGoster();
                    Console.WriteLine($"Toplam Gelir: {satisRaporu.ToplamGelir():C}");
                    break;
                case "4":
                    Console.WriteLine("Çıkılıyor...");
                    return;
                default:
                    Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                    break;
            }
        }
    }

    // Kitapları gösteren metot
    static void KitaplariGoster(List<Urun> urunler)
    {
        Console.WriteLine("\n--- Mevcut Kitaplar ---");
        foreach (var urun in urunler)
        {
            urun.BilgiGoster();  // Polimorfizm ile her ürünün bilgisi gösterilir
        }
    }

    // Kitap satma işlemi
    static void KitapSat(List<Urun> urunler, SatisRaporu satisRaporu)
    {
        KitaplariGoster(urunler);
        Console.Write("Satmak istediğiniz kitabın adını girin: ");
        string kitapAdi = Console.ReadLine();
        Kitap kitapSatilacak = urunler.Find(u => u is Kitap && ((Kitap)u).Ad.Equals(kitapAdi, StringComparison.OrdinalIgnoreCase)) as Kitap;

        if (kitapSatilacak != null)
        {
            Console.Write($"Satmak istediğiniz miktarı girin (Stok: {kitapSatilacak.GetStok()}): ");
            int miktar = int.Parse(Console.ReadLine());

            if (miktar > 0 && miktar <= kitapSatilacak.GetStok())
            {
                // Satış işlemi
                Satis satis = new Satis(kitapSatilacak, miktar);
                satisRaporu.SatisEkle(satis);
                Console.WriteLine($"Satış başarılı: {satis.ToplamTutar:C} için {kitapSatilacak.Ad}");
            }
            else
            {
                Console.WriteLine("Geçersiz miktar. Yeterli stok yok.");
            }
        }
        else
        {
            Console.WriteLine("Kitap bulunamadı.");
        }
    }
}
