using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("1. feladat:");
        Console.WriteLine("Fuvar azonosítóval");

        List<Fuvar> fuvarok = Beolvas("fuvar.csv");

        Console.WriteLine("\n3. feladat:");
        Console.WriteLine($"A fuvarok száma: {fuvarok.Count}");

        Console.WriteLine("\n4. feladat:");
        KeresFuvar(fuvarok, 6185);

        Console.WriteLine("\n5. feladat:");
        FizetesiModokOsszesitese(fuvarok);

        Console.WriteLine("\n6. feladat:");
        double osszesTavolsagKm = fuvarok.Sum(f => f.Tavolsag * 1.6);
        Console.WriteLine($"Az összes megtett távolság: {osszesTavolsagKm:F2} km");

        Console.WriteLine("\n7. feladat:");
        Fuvar legHosszabbFuvar = fuvarok.OrderByDescending(f => f.Idotartam).FirstOrDefault();
        if (legHosszabbFuvar != null)
        {
            Console.WriteLine($"Leghosszabb fuvar adatai:");
            Console.WriteLine($"- Fuvar azonosítója: {legHosszabbFuvar.Azonosito}");
            Console.WriteLine($"- Indulás időpontja: {legHosszabbFuvar.IndulasIdopont}");
            Console.WriteLine($"- Időtartam: {legHosszabbFuvar.Idotartam} másodperc");
            Console.WriteLine($"- Megtett távolság: {legHosszabbFuvar.Tavolsag} mérföld");
            Console.WriteLine($"- Viteldíj: {legHosszabbFuvar.Viteldij} dollár");
            Console.WriteLine($"- Borravaló: {legHosszabbFuvar.Borravalo} dollár");
            Console.WriteLine($"- Fizetési mód: {legHosszabbFuvar.FizetesModja}");
        }
        else
        {
            Console.WriteLine("Nincs rögzített fuvar az adatokban.");
        }

        List<string> hibasSorok = fuvarok
            .Where(f => f.Idotartam > 0 && f.Viteldij > 0 && f.Tavolsag == 0)
            .OrderBy(f => f.IndulasIdopont)
            .Select(f => $"{f.Azonosito};{f.IndulasIdopont};{f.Idotartam};{f.Tavolsag};{f.Viteldij};{f.Borravalo};{f.FizetesModja}")
            .ToList();

        if (hibasSorok.Any())
        {
            File.WriteAllLines("hibak.txt", hibasSorok);
            Console.WriteLine("\nHibás sorok kiírva a hibak.txt állományba.");
        }
        else
        {
            Console.WriteLine("\nNincs hibás sor az adatokban.");
        }

        Console.ReadLine();
    }

    static List<Fuvar> Beolvas(string fajlnev)
    {
        List<Fuvar> fuvarok = new List<Fuvar>();

        try
        {
            string[] sorok = File.ReadAllLines(fajlnev);

            for (int i = 1; i < sorok.Length; i++)
            {
                string[] adatok = sorok[i].Split(';');

                Fuvar fuvar = new Fuvar
                {
                    Azonosito = int.Parse(adatok[0]),
                    IndulasIdopont = DateTime.Parse(adatok[1]),
                    Idotartam = int.Parse(adatok[2]),
                    Tavolsag = double.Parse(adatok[3]),
                    Viteldij = double.Parse(adatok[4]),
                    Borravalo = double.Parse(adatok[5]),
                    FizetesModja = adatok[6]
                };

                fuvarok.Add(fuvar);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Hiba történt a fájl olvasása közben: {e.Message}");
        }

        return fuvarok;
    }

    static void KeresFuvar(List<Fuvar> fuvarok, int azonosito)
    {
        var talaltFuvarok = fuvarok.Where(f => f.Azonosito == azonosito).ToList();

        if (talaltFuvarok.Any())
        {
            double bevetel = talaltFuvarok.Sum(f => f.Viteldij + f.Borravalo);
            Console.WriteLine($"Az {azonosito}-es azonosítójú taxis bevétele: {bevetel} dollár");
            Console.WriteLine($"Az {azonosito}-es azonosítójú taxis {talaltFuvarok.Count} fuvarból állt.");
        }
        else
        {
            Console.WriteLine($"Nincs fuvar az {azonosito}-es azonosítójú taxisnak.");
        }
    }

    static void FizetesiModokOsszesitese(List<Fuvar> fuvarok)
    {
        var fizetesiModok = fuvarok.GroupBy(f => f.FizetesModja)
                                   .Select(g => new { Mod = g.Key, Szam = g.Count() })
                                   .OrderBy(g => g.Mod);

        foreach (var fizetesMod in fizetesiModok)
        {
            Console.WriteLine($"{fizetesMod.Mod}: {fizetesMod.Szam} fuvar");
        }
    }
}

class Fuvar
{
    public int Azonosito { get; set; }
    public DateTime IndulasIdopont { get; set; }
    public int Idotartam { get; set; }
    public double Tavolsag { get; set; }
    public double Viteldij { get; set; }
    public double Borravalo { get; set; }
    public string FizetesModja { get; set; }
}
