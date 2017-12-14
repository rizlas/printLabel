using System.Net;
using System.ServiceModel;

namespace PrintLabel.WcfService
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IService1" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract]
    public interface IPrintService
    {
        [OperationContract(Name = "PrintError")]
        HttpStatusCode Print(string errore, string text);
        [OperationContract(Name = "Print")]
        HttpStatusCode Print(string labelJson, bool copieScelte, int copyNumber, string ipStampante, string tipologia);
        [OperationContract(Name = "PrintIsReprint")]
        HttpStatusCode Print(string labelJson, bool copieScelte, bool ristampa, int copyNumber, string ipStampante, string tipologia);
    }
}
