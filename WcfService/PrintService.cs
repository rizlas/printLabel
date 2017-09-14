using log4net;
using System;
using System.Net;
using System.Threading;

namespace PrintLabel.WcfService
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di classe "Service1" nel codice e nel file di configurazione contemporaneamente.
    public class PrintService : IPrintService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                    log.Error(et.ToString());
                    log.Error(ex);
                }
            });

            Etichetta.SetApartmentState(ApartmentState.STA);
            Etichetta.Name = "ThreadLabel";
            Etichetta.Start();
            Etichetta.Join(10000);

            if (et.Exception == string.Empty)
                return HttpStatusCode.Created;
            else if (et == null || et.Exception != string.Empty)
                return HttpStatusCode.InternalServerError;
            else
                return HttpStatusCode.NoContent;
        }

        HttpStatusCode IPrintService.Print(NiceLabel_S label, bool copieScelte, int copyNumber)
        {
            EtichettaIgf et = null;
            log.Info("Print(NiceLabel label, bool copieScelte, int copyNumber)");

            Thread Etichetta = new Thread(() =>
            {
                try
                {
                    et = new EtichettaIgf(label, copieScelte, copyNumber);
                }
                catch (Exception ex)
                {
                    log.Error(et.ToString());
                    log.Error(ex);
                }
            });

            Etichetta.SetApartmentState(ApartmentState.STA);
            Etichetta.Name = "ThreadLabel";
            Etichetta.Start();
            Etichetta.Join(20000);

            if (et == null)
                return HttpStatusCode.InternalServerError;

            if (et.Exception == string.Empty)
                return HttpStatusCode.Created;
            else if (et == null || et.Exception != string.Empty)
                return HttpStatusCode.InternalServerError;
            else
                return HttpStatusCode.NoContent;
        }

        HttpStatusCode IPrintService.Print(NiceLabel_S label, bool copieScelte, bool ristampa, int copyNumber)
        {
            EtichettaIgf et = null;
            log.Info("Print(NiceLabel label, bool copieScelte, bool ristampa, int copyNumber)");

            Thread Etichetta = new Thread(() =>
            {
                try
                {
                    et = new EtichettaIgf(label, copieScelte, ristampa, copyNumber);
                }
                catch (Exception ex)
                {
                    log.Error(et.ToString());
                    log.Error(ex);
                }
            });

            Etichetta.SetApartmentState(ApartmentState.STA);
            Etichetta.Name = "ThreadLabel";
            Etichetta.Start();
            Etichetta.Join(10000);

            if (et.Exception == string.Empty)
                return HttpStatusCode.Created;
            else if (et == null || et.Exception != string.Empty)
                return HttpStatusCode.InternalServerError;
            else
                return HttpStatusCode.NoContent;
        }
    }
}
