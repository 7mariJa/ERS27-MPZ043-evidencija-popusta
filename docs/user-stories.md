# Корисничке приче

<!-- Самостално написати шест прича према додељеној спецификацији. -->
<!-- За сваку причу навести улогу, могућност, корист, F-ознаку и >= 2 критеријума прихватања. -->

## US-01 (F1)
Kao menadzer prodaje, zelim da evidentiram procentualni popust sa nazivom, opisom, sifrom kupona, procentom popusta i datumom vazenja, kako bih kupcima mogao da ponudim umanjene cijene u procentima.
Kriterijumi prihvatanja:
1. Ako unesem naziv i opis, sifru kupona, procenat od 1 do 90 i datum vazenja, onda se popust cuva i prikazuje mi se dodijeljeni identifilkator.
2. Ako unesem procenat van opsega (npr. 91 i vise) ili ostavim prazan naziv ili opis, onda se popust ne cuva , dobijam poruku o gresci i nijedan podatak nije djelimicno upisan.
## US-02 (F2)
Kao menadzer prodaje, zelim da evidentiram fiksan popust sa nazivom, opisom, sifrom kupona, iznosom popusta i datumom vazenja, kako bih kupcima mogao da ponudim umanjene cijene za tacan iznos
Kriterijumi prihvatanja:
1. Ako unesem naziv i opis, sifru kupona, pozitivan iznos (npr. 500) i datum vazenja, onda se popust cuva i priazuje mi se dodijeljeni identifikator.
2. Ako unesem 0 ili negativan iznos, onda se popust ne cuva i dobijam poruku da iznos mora biti pozitivan.
## US-03 (F3)
Kao menadzer prodaje, zelim da vidim spisak svih evidentiranih popusta sa tipom, identifikatorom i statusom, kao i posebno spisak vazecih kupona, kako bih imao pregled svih ponuda i znao koje se jos mogu koristiti.
Kriterijumi prihvatanja: 
1. Ako postoje evidentirani popusti, onda se prikazuju svi sa tipom, identifikatorom i statusom. Mogu da izaberem sazeti prikaz (tip, ID, naziv, status) ili detaljni prikaz (sa procentom, odnosno iznosom popusta).
2. Ako izaberem prikaz vazecih kupona, onda se prikazuju samo popusti ciji datum vazenja nije prosao.
3. Ako nema nijednog evidentiranog popusta ili nijednog vazeceg kupona, onda se prikazuje poruka „Nema zapisa za prikaz“, bez prazne tabele.
## US-04 (F4)
Kao prodavac, zelim da pronadjem popust prema sifri kupona, kako bih brzo provjerio kupon koji kupac pokaze.
Kriterijumi prihvatanja:
1. Ako unesem sifru bez obzira na velika i mala slova (npr. „ljeto2026“ za kupon „LJETO2026“), onda se prikazuju popusti sa tom sifrom.
2. Ako nijedan popust nema unesenu sifru, onda se prikazuje poruka „Nema rezultata za unesenu sifru“.
## US-05 (F5)
Kao menadzer prodaje, zelim da aktiviram popust, kako bih omogucio kupcima da ga koriste.
Kriterijumi prihvatanja: 
1. Ako unesem identifikator postojeceg popusta ciji datum vazenja nije prosao, onda popust prelazi u status „Aktivan“ i dobijam poruku o uspjehu.
2. Ako unesem identifikator koji ne postoji, onda operacija vraca neuspjeh sa porukom „Popust nije pronadjen“.
3. Ako je datum vazenja popusta prosao, onda se aktiviranje odbija i status ostaje nepromijenjen.
## US-06 (F6)
Kao menadzer prodaje, zelim da deaktiviram popust, kako bih prekinuo njegovo koriscenje kada akcija vise ne vazi.
Kriterijumi prihvatanja:
1. Ako unesem identifikator popusta koji je aktivan, onda on prelazi u status „Neaktivan“ i dobijam poruku o uspjehu.
2. Ako pokusam da deaktiviram popust koji nije bio aktivan, onda se operacija odbija uz poruku o gresci i status popusta ostaje nepromijenjen.