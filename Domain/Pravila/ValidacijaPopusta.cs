using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Pravila
{
    public static class ValidacijaPopusta
    {
        public static string? Provjeri(ProcentualniPopust p)    //?vraca ili rijec(ako je greska) ili nista(null - ako sve valja)
        {
            if(string.IsNullOrWhiteSpace(p.Naziv) || string.IsNullOrWhiteSpace(p.Opis))
                return "Naziv i Opis ne smiju biti prazni!";
            if(p.Procenat < 1 || p.Procenat > 90)
                return "Procenat popusta mora biti izmedju 1 i 90!";
            return null;

        }

        public static string? Provjeri(FiksniPopust p)
        {
            if(string.IsNullOrWhiteSpace(p.Naziv) || string.IsNullOrWhiteSpace(p.Opis))
                return "Naziv i Opis ne smiju biti prazni!";
            if(p.Iznos <= 0)
                return "Iznos popusta mora biti pozitivan!";
            return null;
        }
    }
}
