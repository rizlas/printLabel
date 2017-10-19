using log4net;
using Newtonsoft.Json;
using PrintLabel.Models;
using System;
using System.Net;
using System.Threading;

namespace PrintLabel.WcfService
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di classe "Service1" nel codice e nel file di configurazione contemporaneamente.
    public class PrintService : IPrintService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int _threadTimeout = 20000;

        HttpStatusCode IPrintService.Print(string errore, string text)
        {
            EtichettaIgf et = null;
            log.Info("Print(string errore, string text)");
            
            Thread Etichetta = new Thread(() =>
            {
                try
                {
                    et = new EtichettaIgf(errore, text);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            });

            Etichetta.SetApartmentState(ApartmentState.STA);
            Etichetta.Name = "ThreadLabel";
            Etichetta.Start();
            Etichetta.Join(_threadTimeout);

            if (et.Exception == string.Empty)
                return HttpStatusCode.Created;
            else if (et == null || et.Exception != string.Empty)
                return HttpStatusCode.InternalServerError;
            else
                return HttpStatusCode.NoContent;
        }

        HttpStatusCode IPrintService.Print(string labelJson, bool copieScelte, int copyNumber, string ipStampante)
        {
            EtichettaIgf et = null;
            NiceLabel label = JsonConvert.DeserializeObject<NiceLabel>(labelJson);
            log.Info("Print(string labelJson, bool copieScelte, int copyNumber, string ipStampante)");
            ipStampante = ipStampante.Replace("\"", "");
            log.Info($"IpStampante: {ipStampante}");

            Thread Etichetta = new Thread(() =>
            {
                try
                {
                    et = new EtichettaIgf(label, copieScelte, copyNumber, ipStampante);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            });

            Etichetta.SetApartmentState(ApartmentState.STA);
            Etichetta.Name = "ThreadLabel";
            Etichetta.Start();
            Etichetta.Join(_threadTimeout);

            if (et.Exception == string.Empty)
                return HttpStatusCode.Created;
            else if (et == null || et.Exception != string.Empty)
                return HttpStatusCode.InternalServerError;
            else
                return HttpStatusCode.NoContent;
        }

        HttpStatusCode IPrintService.Print(string labelJson, bool copieScelte, bool ristampa, int copyNumber, string ipStampante)
        {
            EtichettaIgf et = null;
            NiceLabel label = JsonConvert.DeserializeObject<NiceLabel>(labelJson);
            log.Info("Print(string labelJson, bool copieScelte, bool ristampa, int copyNumber, string ipStampante)");
            ipStampante = ipStampante.Replace("\"", "");
            log.Info($"IpStampante: {ipStampante}");

            Thread Etichetta = new Thread(() =>
            {
                try
                {
                    et = new EtichettaIgf(label, copieScelte, ristampa, copyNumber, ipStampante);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            });

            Etichetta.SetApartmentState(ApartmentState.STA);
            Etichetta.Name = "ThreadLabel";
            Etichetta.Start();
            Etichetta.Join(_threadTimeout);

            if (et.Exception == string.Empty)
                return HttpStatusCode.Created;
            else if (et == null || et.Exception != string.Empty)
                return HttpStatusCode.InternalServerError;
            else
                return HttpStatusCode.NoContent;
        }
    }
}
