using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing.Printing;
using log4net;

namespace PrintLabel
{
    /// <summary>
    /// Logica di interazione per ucEtichettaIgf.xaml
    /// </summary>
    public partial class EtichettaIgf : UserControl
    {
        private string _sscc_code;
        private string _reparto;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private iSysPrintEntities db = new iSysPrintEntities();
        
        private NiceLabel_S _label;
        public NiceLabel_S Label
        {
            get
            {
                return _label;
            }

            set
            {
                _label = value;
            }
        }

        private string _exception = string.Empty;
        public string Exception
        {
            get
            {
                return _exception;
            }

            set
            {
                _exception = value;
            }
        }

        private int _copyNumber = 1;        
        private string _ipStampante = string.Empty;

        public EtichettaIgf()
        {
            InitializeComponent();
            imgLogo.Source = new BitmapImage(new Uri(@"/PrintLabel;component/Resources/LogoIGF_SpA.jpg", UriKind.RelativeOrAbsolute));
            imgLogo.UpdateLayout();
        }

        public EtichettaIgf(string errore, string text) : this()
        {
            if(errore == "error")
            {
                gLabel.Visibility = Visibility.Hidden;
                gError.Visibility = Visibility.Visible;
                tbError.Text = text;
                
                // Introdurre print se serve                
            }
        }

        public EtichettaIgf(NiceLabel_S label, bool copieScelte, bool ristampa, int copyNumber) : this()
        {
            _label = new NiceLabel_S();
            _label.Bancale = label.Bancale;
            _label.Commessa = label.Commessa;
            //_label.Data = label.Data;
            _label.DescLavorazione = label.DescLavorazione;
            _label.Edizione = label.Edizione;
            _label.IPPistola = label.IPPistola;
            _label.Lavorazione = label.Lavorazione;
            _label.Macchina = label.Macchina;
            _label.Presenti = label.Presenti;
            _label.QuantitaFatta = label.QuantitaFatta;
            _label.QuantitaSuBancale = label.QuantitaSuBancale;
            _label.Segnatura = label.Segnatura;
            _label.sscc_code = label.sscc_code;
            _label.Stato = label.Stato;
            _label.Titolo = label.Titolo;

            _copyNumber = copyNumber;

            if (copieScelte)
                this.cSimboloCopieScelte.Visibility = Visibility.Visible;

            if (_reparto == "AI")
                this.cSimbolo.Children.Add(CreaCerchio());

            if (_label.Macchina.Contains("PI"))
                GetPrinterAssociated();

            this.cSimbolo.Children.Add(CreaPoligono());
            this.cSimbolo.Children.Add(LabelReparto());

            this.lblID.Content = _label.ID;
            this.tbBancale.Text = _label.Bancale.ToString();
            this.tbCommessa.Text = _label.Commessa;
            this.tbCopieBancale.Text = _label.QuantitaSuBancale.ToString();
            this.tbEdizione.Text = _label.Edizione.ToString();
            this.tbSegnatura.Text = _label.Segnatura;
            this.tbMacchina.Text = _label.Macchina;
            this.txtOperatori.Text = " " + _label.Presenti;
            this.txtImpSegnatura.Text = " " + _label.DescLavorazione;
            this.txtTitolo.Text = _label.Titolo;
            this.barCodeCommessa.Code = _label.Commessa;
            this.qrCode.Code = ImpostaQRCode();

            Print(this, _ipStampante != "" ? _ipStampante : _label.Macchina);
        }

        public EtichettaIgf(NiceLabel_S label, bool copieScelte, int copyNumber) : this()
        {
            _label = label;
            _copyNumber = copyNumber;

            if (copieScelte)
                this.cSimboloCopieScelte.Visibility = Visibility.Visible;

            PRODUZIONE_S produzione = (from prod in db.PRODUZIONE_S
                                     where prod.Macchina == _label.Macchina //"mPI01"
                                     select prod).First();

            this.SetStaticData(produzione);
            this.CalcolaQuantita_E_NumeroBancale(produzione);
            this.GetPresenti();
            if(_label.Titolo == null)
                this.GetTitolo();

            if(_label.DescLavorazione == null || _label.DescLavorazione == string.Empty)
                this.GetDescLavorazione();

            this.GetID();

            if (_label.Macchina.Contains("PI"))
                GetPrinterAssociated();

            this._sscc_code = _label.sscc_code;

            ImpostaReparto();

            if (_reparto == "AI")
                this.cSimbolo.Children.Add(CreaCerchio());

            this.cSimbolo.Children.Add(CreaPoligono());
            this.cSimbolo.Children.Add(LabelReparto());

            this.lblID.Content = _label.ID;
            //this.tbBancale.Text = _label.Bancale.ToString();      // Disabilitate in attesa di edizione e segnatura
            this.tbCommessa.Text = _label.Commessa;
            this.tbCopieBancale.Text = _label.QuantitaSuBancale.ToString();
            //this.tbEdizione.Text = _label.Edizione.ToString();    // Disabilitate in attesa di edizione e segnatura
            this.tbMacchina.Text = _label.Macchina;
            this.txtOperatori.Text = " " + _label.Presenti;
            this.txtImpSegnatura.Text = " " + _label.DescLavorazione;
            this.txtTitolo.Text = _label.Titolo;
            this.barCodeCommessa.Code = $"(02){_label.Commessa}";
            //this.lblBarCode.Content = $"(02){this.lblBarCode.Content}";
            this.qrCode.Code = ImpostaQRCode();

            string[] segnature = null;

            if (_label.Segnatura.Contains("-"))
                segnature = _label.Segnatura.Split('-');

            //if(segnature == null)     // Disabilitate in attesa di edizione e segnatura
            //    this.tbSegnatura.Text = _label.Segnatura.ToString();

            _label.Data = DateTime.Now;

            if (label.QuantitaSuBancale > 0)
            {
                if (segnature != null)
                {
                    for (int i = 0; i < segnature.Length; i++)
                    {
                        _label.Segnatura = segnature[i];
                        this.tbSegnatura.Text = _label.Segnatura.ToString();
                        this.tbSegnatura.UpdateLayout();

                        PrintAndSave();
                    }
                }
                else
                {
                    PrintAndSave();
                }
            }
            else
            {
                _exception = $"Quantità su bancale pari a zero {this.ToString()}";
                log.Error(_exception);
            }
        }

        private void PrintAndSave()
        {
            log.Info($"Ip Stampante {_ipStampante}");

            Print(this, _ipStampante != "" ? _ipStampante : _label.Macchina);

            db.NiceLabel_S1.Add(_label);
            db.SaveChangesAsync();
        }

        private string ImpostaQRCode()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_label.Commessa);
            sb.Append("|");
            sb.Append(_label.Macchina);
            sb.Append("|");
            sb.Append(_label.Presenti.Replace(", ", ";"));
            sb.Append("|");
            sb.Append(_label.Bancale);
            sb.Append("|");
            sb.Append(_sscc_code);

            return sb.ToString();
        }

        private void ImpostaReparto()
        {
            string tmp = _label.Macchina;

            if (tmp.Contains("PI"))
                tmp = "mPI01";
            else if (tmp.Contains("CU"))
                tmp = "mCU01";
            else if (tmp.Contains("RA"))
                tmp = "mRA01";

            switch (tmp)
            {
                case "mAI01":
                    _reparto = "AI";
                    break;
                case "mBR01":
                case "mBR02":
                    _reparto = "BR";
                    break;
                case "mCA01":
                    _reparto = "CO";
                    break;
                case "mCA02":
                    _reparto = "TT";
                    break;
                case "mCA03":
                    _reparto = "TC";
                    break;
                case "mCA04":
                    _reparto = "BR";
                    break;
                case "mCA05":
                    _reparto = "CA";
                    break;
                case "mCA08":
                    _reparto = "TR";
                    break;
                case "mCU01":
                    _reparto = "CU";
                    break;
                case "mEX01":
                    _reparto = "EX";
                    break;
                case "mOL01":
                    _reparto = "OL";
                    break;
                case "mPI01":
                    _reparto = "PI";
                    break;
                case "mPM01":
                    _reparto = "PM";
                    break;
                case "mRA01":
                    _reparto = "RA";
                    break;
                case "mTA01":
                    _reparto = "TA";
                    break;
            }
        }

        private Label LabelReparto()
        {
            Label myLabel = new Label();
            myLabel.Name = "lblReparto";
            myLabel.Content = _reparto;
            myLabel.Foreground = new SolidColorBrush(Colors.White);
            myLabel.Height = 100;
            myLabel.Width = 100;
            myLabel.HorizontalAlignment = HorizontalAlignment.Center;
            myLabel.VerticalAlignment = VerticalAlignment.Center;
            myLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            myLabel.VerticalContentAlignment = VerticalAlignment.Center;
            myLabel.FontFamily = new FontFamily("Arial");
            myLabel.FontSize = 32;

            return myLabel;
        }

        //Reparto AI
        private Ellipse CreaCerchio()
        {
            Ellipse e = new Ellipse();
            e.Fill = Brushes.Black;
            e.Height = 100;
            e.Width = 100;

            return e;
        }

        private Polygon CreaPoligono()
        {
            Polygon poligono = new Polygon();
            poligono.Fill = Brushes.Black;
            poligono.Width = 100;
            poligono.Height = 100;

            PointCollection pc = null;

            switch (_reparto)
            {
                case "TA":
                    // Diamond
                    pc = PointCollection.Parse("10,50 50,0 90,50 50,100");
                    break;
                case "PI":
                    // Square
                    pc = PointCollection.Parse("0,0 100,0 100,100 0,100");
                    break;
                case "CU":
                    // Pentagon
                    pc = PointCollection.Parse("0,45 50,0 100,45 75,100 25,100");
                    break;
                case "BR":
                    // Triangle
                    pc = PointCollection.Parse("50,0 100,100 0,100");
                    break;
                case "CO":
                    // Hexagon
                    pc = PointCollection.Parse("0,50 20,0 80,0 100,50 80,100 20,100");
                    break;
                case "CA":
                    // Octagon
                    pc = PointCollection.Parse("30,0 70,0 100,30 100,70 70,100 30,100 0,70 0,30");
                    break;
                case "PM":
                    // Star
                    pc = PointCollection.Parse("50,0 65,35 100,35 75,65 85,100 50,75 15,100 25,65 0,35 35,35");
                    break;
                case "RA":
                    // Trapezoid
                    pc = PointCollection.Parse("25,25 75,25 100,75 0,75");
                    break;
                case "EX":
                    // Reverse Diamond
                    pc = PointCollection.Parse("0,50 50,25 100,50 50,75");
                    break;
                case "OL":
                    // Rectangle
                    pc = PointCollection.Parse("0,25 100,25 100,75 0,75");
                    break;
            }

            poligono.Points = pc;

            return poligono;
        }
        
        private void SetStaticData(PRODUZIONE_S produzione)
        {
            _label.Macchina = produzione.Macchina;
            _label.Commessa = produzione.Commessa;
            _label.Lavorazione = (short)produzione.Lavorazione;
            _label.Stato = produzione.Stato;
            _label.QuantitaFatta = (int)produzione.QuantitàFatta;
            //_label.Data = DateTimeOffset.Now;

            if(_label.Segnatura == null)
                _label.Segnatura = produzione.Segnatura.ToString();

            if(_label.Edizione == null)
                _label.Edizione = produzione.Edizione.ToString();
        }

        private void GetID()
        {
            var max = (from etichette in db.NiceLabel_S1
                       select etichette.ID);

            if (max.Count() > 0)
                _label.ID = max.Max() + 1;
            else
                _label.ID = 1;
        }

        private void GetTitolo()
        {
            try
            {
                // Il titolo della commessa viene recuperato dalla tabella Prev 
                // aggiungendo C. alla commessa che attualmente si trova in macchina

                var query = (from commesse in db.Prev_S
                             where commesse.Stato == "C." + _label.Commessa
                             select new { commesse.Titolo });

                if (query.Count() > 0)
                    _label.Titolo = query.Single().Titolo;
                else
                    _label.Titolo = "ERRORE: From Prev where Stato = C. + _commessa -> Count = 0";
            }
            catch (Exception ex)
            {
                _exception = string.Format("{0:dd-MM-yyyy HH:mm:ss} - Exception: {1} - {2} \r\nStack trace: {3}", DateTime.Now, ex.Source.ToString(), ex.Message, ex.StackTrace);
                log.Error(_exception);
            }
        }

        private void GetPresenti()
        {
            try
            {
                // I presenti vengono semplicemente selezionati dalla vista creata nel database

                var query = (from presenti in db.V_Presenti_S
                             where presenti.macchina == _label.Macchina
                             select new { presenti.presenti });

                if (query.Count() > 0)
                    _label.Presenti = query.Single().presenti;
                else
                {
                    _label.Presenti = string.Format("Non ci sono operatori presenti sulla {0}", _label.Macchina);
                    _exception = "ERRORE: From V_Presenti where macchina = _macchina->Count = 0";
                    log.Info(_exception);
                }
            }
            catch (Exception ex)
            {
                _exception = string.Format("{0:dd-MM-yyyy HH:mm:ss} - Exception: {1} - {2} \r\nStack trace: {3}", DateTime.Now, ex.Source.ToString(), ex.Message, ex.StackTrace);
                log.Error(_exception);
            }

        }

        private void GetDescLavorazione()
        {
            try
            {
                // La descrizione del lavoro viene selezionata dalla tabella Lavorazioni e può risultare solo un record 
                // per quella commessa, fase e macchina

                var query = (from lav in db.Lavorazioni_S
                             where lav.Commessa == _label.Commessa &&
                                   lav.Fase == _label.Lavorazione &&
                                   lav.Macchina == _label.Macchina
                             select new { lav.DescLavorazione });

                if (query.Count() > 0)
                    _label.DescLavorazione = query.Single().DescLavorazione;
                else
                    _label.DescLavorazione = "ERRORE: From Lavorazioni where commessa = _commessa AND Fase = _lavorazione AND Macchina = _macchina -> Count = 0";
            }
            catch (Exception ex)
            {
                _exception = string.Format("{0:dd-MM-yyyy HH:mm:ss} - Exception: {1} - {2} \r\nStack trace: {3}", DateTime.Now, ex.Source.ToString(), ex.Message, ex.StackTrace);
                log.Error(_exception);
            }
        }

        private void CalcolaQuantita_E_NumeroBancale(PRODUZIONE_S produzione)
        {
            try
            {
                // Trovo tutti i record salvati in NiceLabel che hanno stessa macchina e stessa commessa e stessa segnatura
                // Se sono più di uno cerco l'ultimo altrimenti il primo bancale avrà come quantità 
                // quella fatta dalla macchina al momento (qtaAtParziale = qtaAtTime)
                // Vedi query produzione nel costruttore

                // Trovo tutti i record salvati in NiceLabel che hanno stessa macchina e stessa commessa e stessa segnatura
                // Se esiste almeno un record già salvato con stessa macchina, stessa commessa e stessa fase allora prendo 
                // il suo numero di bancale e salvo quest'ultimo facendo + 1, altrimenti si tratta di primo bancale
                // Controllo dello stato, perchè se l'ultimo bancale sparato ha come stato ES .... allora la quantità
                // sulla macchina è stata azzerata (cambio segnature, ecc.)

                //NO SSCC_CODE perchè la quantità sul terminale non si azzerra quando è terminato il bancale....
                //Devo trovare un modo per cambiare sscc_code al cambio bancale, devono spararlo per forza
                // && etichette.sscc_code == this._sscc_code
                //Cambio segnatura sul terminale

                var query = from etichette in db.NiceLabel_S1
                            where etichette.Macchina == _label.Macchina &&
                                  etichette.Commessa == _label.Commessa &&
                                  etichette.Lavorazione == _label.Lavorazione &&
                                  etichette.Segnatura == _label.Segnatura
                            orderby etichette.Data descending
                            select etichette;

                int count = query.Count();

                // Check stato

                // 0 e non 1 perchè quando si spara l'etichetta il record non è ancora inserito, quindi se è il primo count risulterà 0
                if (count > 0)
                {
                    NiceLabel_S record = query.First();
                    if (_label.QuantitaFatta >= record.QuantitaSuBancale)// && record.Segnatura == _label.Segnatura)
                    {
                        _label.QuantitaSuBancale = (int)(_label.QuantitaFatta - record.QuantitaFatta);     // Se quantitaSuBancale = 0 chiedo di ristampare l'ultimo?
                        _label.Bancale = (int)record.Bancale + 1;
                    }
                    else
                    {
                        //La segnatura è cambiata o la quantitàfatta è minore dell'ultimo (minore significa che è stato azzerato il counter per cambio edizione o altro)
                        _label.QuantitaSuBancale = _label.QuantitaFatta;
                        _label.Bancale = 1;     //Check
                    }
                }
                else
                {
                    _label.QuantitaSuBancale = _label.QuantitaFatta;
                    _label.Bancale = 1;
                }
            }
            catch (Exception ex)
            {
                _exception = string.Format("{0:dd-MM-yyyy HH:mm:ss} - Exception: {1} - {2} \r\nStack trace: {3}", DateTime.Now, ex.Source.ToString(), ex.Message, ex.StackTrace);
                log.Error(_exception);
            }
        }

        private void GetPrinterAssociated()
        {
            Stampanti_NiceLabel_S sn = (from stampante in db.Stampanti_NiceLabel_S
                                      where stampante.Ip_Pda == _label.IPPistola
                                      select stampante).First();

            this._ipStampante = sn.Ip_Stampante;
        }

        private void Print(EtichettaIgf et, string destinationPrinter)
        {
            PrintDialog printDlg = new PrintDialog();

            //Printer settings si riferisce alla stampante predefinita
            PrinterSettings settings = new PrinterSettings();
            string defaultPrinter = settings.PrinterName;
            
            if (System.Environment.MachineName == "CARAC-DELL")
                destinationPrinter = "192.168.172.200";

            try
            {
                System.Printing.LocalPrintServer lps = new System.Printing.LocalPrintServer();      //System.Printing
                System.Printing.PrintQueue pq = lps.GetPrintQueue(destinationPrinter);

                System.Printing.PrintTicket pt = new System.Printing.PrintTicket();
                pt.PageOrientation = System.Printing.PageOrientation.Portrait;
                //pt.CopyCount = _copyNumber;
                pt.PageMediaSize = new System.Printing.PageMediaSize(this.Width, this.Height);
                pq.DefaultPrintTicket = pt;
                
                printDlg.PrintQueue = pq;
                printDlg.PrintTicket.CopyCount = _copyNumber;
                //System.Printing.PrintCapabilities pc = pq.GetPrintCapabilities(pt);
                
                bool founded = true;

                //for (int i = 0; i < PrinterSettings.InstalledPrinters.Count && !founded; i++)
                //{
                //    if (PrinterSettings.InstalledPrinters[i] == destinationPrinter)
                //        founded = true;
                //}

                if (founded)
                {
                    printDlg.PrintVisual(et, $"{_label.ID}-{_label.Commessa}-{_label.Macchina}");
                }
                else
                {
                    _exception = $"Stampante non trovata {destinationPrinter}";
                    log.Error(_exception);
                }
            }
            catch (Exception ex)
            {
                _exception = ex.Message;
                log.Error(_exception);
            }
        }

        public override string ToString()
        {
            return string.Format("Macchina: {0} Commessa: {1} - Stato: {2} Segnatura: {3} QuantitaFatta: {4} QuantitaSuBancale: {5} Bancale: {6}", _label.Macchina, _label.Commessa, _label.Stato, _label.Segnatura, _label.QuantitaFatta, _label.QuantitaSuBancale, _label.Bancale);
        }
    }
}
