using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PrintLabel.WcfService
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IService1" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract]
    public interface IPrintService
    {
        [OperationContract(Name = "PrintError")]
        HttpStatusCode Print(string errore, string text);
        [OperationContract(Name = "Print")]
        HttpStatusCode Print(NiceLabel_S label, bool copieScelte, int copyNumber);
        [OperationContract(Name = "PrintIsReprint")]
        HttpStatusCode Print(NiceLabel_S label, bool copieScelte, bool ristampa, int copyNumber);
    }
}
