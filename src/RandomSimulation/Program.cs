using log4net;
using RandomSimulationEngine;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;

namespace RandomSimulation
{
    public static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

#warning TODO - konfigurowalna ilość równoczesnego ściągania i liczenia 

#warning TODO - rotacja logów dockera na hoście: https://stackoverflow.com/questions/42510002/how-to-clear-the-logs-properly-for-a-docker-container

#warning todo - throttling - Ograniczenia loadu na sekundę (konfigurowalne) - middleware

#warning TODO - dodać na coniec FxCop i StyleCop do wsiech projektów
#warning TODO - wyszukać czy gdzieś nie zostały stringi ClassNamer

#warning TODO - dodać Swagger ui

#warning TODO - spłodzić Readme.md

        [MTAThread]
        public static void Main(string[] args)
        {
            log.Info("Starting RandomSimulation");

            // każdy worker sobie w tle pobiera raz na 5s-15s
            // mamy ring workerów, w czasie requestu o liczbę, bieżemy z jednego i obracamy ring; 
            //    worker, z którego brana jest liczba zasysa nowy image i nie bierze w udziału w requestach dopóki nie skończy nowej wartości obliczać
            //    A MOŻE wcale nie robić ring, tylko brać randomowy, dostępny worker - tak chyba lepiej

            // pomyśleć nad tym, czy można przechowywać w pamięci te dane dłużej
            // SHA512 produkuje 512bitów czyli 64 bajty. Można na przykład brać taki obrazek i robić hash z randomowej ilości bajtów
            // niech ta wartość randomowa będzie taka, że z obrazka jednego robimy minimum 20 (konfigurowalne od 5 do 50 np) hashy
            // jak ilość niewykorzystanych hashy spadnie poniżej 50% (konfigurowalne), to uruchamiamy znowu task zasysania

            // zobaczyć, czy nie ma jakiś darmowych serwerów, gdzie przez vpn mozna zasysać dane z internetu
            //    możnaby za każym razem losować z którego isę łączymy i gdzie

            // zrobic automatycznego, długo działającego testera, któy sprawdzi randomowość w długim czasie i przy dużym obciążeniu

            log.Info("Initializing injection container");
            Engine.InitializeContainer();

            log.Info("Starting engine hosting");
            Engine.StartHosting();

            //try
            //{
            //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sourceURL);

            //    using (WebResponse resp = req.GetResponse())
            //    using (Stream stream = resp.GetResponseStream())
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        stream.CopyTo(ms);
            //        using (Bitmap bmp = (Bitmap)Image.FromStream(ms))
            //        {
            //            bmp.Save($"FrameGrab{DateTime.Now:yyyyMMddHHmmss}.jpg");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLine(ex);
            //}
        }
    }
}