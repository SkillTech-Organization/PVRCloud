# PVRPCloud 1.5-TRoute specifikáció

[**Követelmények:    1**](#követelmények)

[**Implementáció:    1**](#implementáció)

[Input model, TRouteProject osztály:    1](#input-model,-trouteproject-osztály:)

[Ouput model, TRouteProjectRes osztály:    2](#ouput-model,-trouteprojectres-osztály:)

[Flow    2](#flow)

Feladat: egy jármű megadott túrájára kiszámolja a érkezési/indulási- ill. menetidőket, útvonalakat, és útdíjakat. 

Az API funkció neve : TRoute

# Követelmények {#követelmények}

* .NET 9 framework használata  
* A TRoute API bemenő adataira a PVRPCloudRequest API hívás PVRPCloudProject, a kimenő adatokra a PVRPCloudResult API hívás PVRPCloudRes adatszerkezetből származtatott osztályok legyenek használva.  
* Input paraméteren keresztül megadható legyen, hogy az útvonalszámításoknál a súly-, méret- és behajtási övezet korlátozások figyelmen kivül legyenek hagyva (WithoutRestrictions paraméter)  
* A validálási szabályok bővítése a bejövő modelre:  
  * WithoutRestrictions paraméter validálása  
  * csak egy jármű adható át  
  * max 25 túrapont lehetséges  
* Amennyiben két pont között nem talál útvonalat a program, a hibaüzenetben szerepeljen, hogy súly- ill. behajtási övezet korlátozások, vagy térképszakadás miatt van  
* A TRoute API-nak a jelenleg működő PVRPCloud 1.3-tól különböző App Service-ben kell futnia.  
* Az éles- és fejlesztői AppService-kre start-stop rendszert kell kiépíteni a FinOps igények miatt.  
* A rendszert az alábbi terhelésre kell felkészíteni Premium v3 P0V3 konfigurációjú (ebben működik a PVRPCloud 1.0) App service kiépítettség esetén:

# Implementáció {#implementáció}

## Input model, TRouteProject osztály: {#input-model,-trouteproject-osztály:}

* PVRPCloudProject \-ből származik  
* Új mező  
  * Súly- és behajtási övezet korlátozások figyelmen kivül hagyása: WithoutRestrictions, bool  
* plusz validálások  
  * WithoutRestrictions töltöttség ellenőrzés  
  * Trucks lista csak egy elemű lehet  
  * Orders lista max 25 elemű lehet

## Ouput model, TRouteProjectRes osztály: {#ouput-model,-trouteprojectres-osztály:}

* ProjectRes \-ből származik  

* Új mezők  
  Teljes túra:  
  
  * Távolság rakott: Tour.LoadedLength  
  * Távolság üres: TourUnloadedLength (utolsó túrapontból a raktárba vezető út)  
  * Behajtási övezetlista: Tour.RestrictedZones : string lista  
  * Súlykorlátozások útípusonként: Tour.WeigthRestrictions : Dictionary\<int, int\>  
  * Útdíjak útípusonként: Tour.Tolls: Dictionary\<int,int\>
  
  Túraszakaszra (TourPoint) nézve (előző érintett címétől számítva):
  
  * Behajtási övezetlista:TourPoint.RestrictedZones : string lista  
  * Súlykorlátozások útípusonként: TourPoint.WeigthRestrictions : Dictionary\<int,int\>  
  * Útdíjak útípusonként ProjectRes.TourPoint.Tolls: Dictionary\<int,int\>

## Flow {#flow}

PVRPCloudRequest:

1. Megtörténik az input adatok validációja  
2. Validációs hiba, vagy authorizációs hiba esetén Http 4xx visszatérés, hibaüzenetekkel  
3. Reguest ID generálása  
4. Kérés berakása egy belső queue-ba  
5. Http 2xx visszatérés, request ID-vel  
6. Nem kezelt kivétel esetén 500-as hiba visszadása

PVRPCloudQueueProcessor:

1. Figyeli a queue-t és ha van ott kérés, egy asszinkron belső folyamat elindítása  
2. Raktár, lerakó koordináták térképre illesztése  
3. Túrapontok közötti útvonalak kiszámolása  
4. Útvonalak útdíjköltségeinek kiszámolása  
5. Túrapontok teljesítési időpontjainak kiszámolása  
6. Eredmény JSON formában blob storage-be mentése  
7. Üzleti hiba, exception esetén az eredmény JSON-ba kerülnek a felmerült hibák  
   1. Hiányzó útvonal esetén meg kell állapítani, hogy gráf-szakadás vagy súlykorlátozás miatt nincs útvonal.  

TRouteResult

1. Megvizsgálja, hogy a kapott RequestID-re létezi-e eredményfájl a blob storage-ben  
2. Amennyiben létezik, az eredmény adatszerkezet feltöltésre kerül a blob alapján, és http 200-al visszaadja  
3. Ha nem található eredmény http 404 kerül visszaadásra

Teszt/dummy adatok
