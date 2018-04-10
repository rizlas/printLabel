using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing.Printing;
using log4net;
using PrintLabel.Models;

namespace PrintLabel
{
    public partial class EtichettaIgf : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        
        public EtichettaIgf(NiceLabel label, bool copieScelte, int copyNumber, string ipStampante, string tipologia) : this()
        {
            string reparto = ImpostaReparto(label.Macchina);

            if (reparto != "AI")
                this.cSimbolo.Children.Add(CreaPoligono(reparto));
            else
                this.cSimbolo.Children.Add(CreaCerchio());

            this.cSimbolo.Children.Add(LabelReparto(reparto));

            this.lblID.Content = label.ID;
            this.tbCommessa.Text = label.Commessa;

            if (!copieScelte)
            {
                this.tbCopieBancale.Text = label.QuantitaSuBancale.ToString();
                this.barCodeCommessa.Code = $"(02){label.Commessa}-{label.ID}";
                //this.tbBancale.Text = _label.Bancale.ToString();      // Disabilitate in attesa di edizione e segnatura
            }
            else
            {
                this.cSimboloCopieScelte.Visibility = Visibility.Visible;

                this.tbCopieBancale.Visibility = Visibility.Hidden;
                this.lblCopieBancale.Visibility = Visibility.Hidden;

                this.barCodeCommessa.Visibility = Visibility.Hidden;
                this.lblBarCode.Visibility = Visibility.Hidden;

                this.tbBancale.Visibility = Visibility.Hidden;
                this.lblBancale.Visibility = Visibility.Hidden;
            }
            
            this.tbEdizione.Text = label.Edizione.ToString().ToUpper();
            this.tbMacchina.Text = label.Macchina;
            this.txtOperatori.Text = " " + label.Presenti.TrimEnd(',', ' ');

            if (string.IsNullOrEmpty(label.DescLavorazione))
            {
                label.DescLavorazione = ".";
            }

            this.txtImpSegnatura.Text = " " + label.DescLavorazione;

            //log.Info(this.ToString(label));

            if (label.Titolo == null)
            {
                log.Info($"Titolo is null {label.Commessa} {label.Macchina} {label.Lavorazione}");
            }
            else
            {
                this.txtTitolo.Text = label.Titolo.ToUpper();
            }

            if (label.Macchina.Contains("CU"))
            {
                gSegnatura.Visibility = Visibility.Hidden;
                gTipologia.Visibility = Visibility.Visible;

                tbTipologia.Text = tipologia;
                this.tbTipologia.UpdateLayout();
            }
            else
            {
                gSegnatura.Visibility = Visibility.Visible;
                gTipologia.Visibility = Visibility.Hidden;
                tbTipologia.Visibility = Visibility.Hidden;
            }

            this.lblBarCode.Content = $"{label.Commessa}-{label.ID}";
            //this.qrCode.Code = ImpostaQRCode();

            Print(this, ipStampante, copyNumber);
        }

        private string ToString(NiceLabel label)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine($"Bancale {label.Bancale.ToString()}");
            sb.AppendLine($"Commessa {label.Commessa.ToString()}");
            sb.AppendLine($"Data {label.Data.ToString()}");
            sb.AppendLine($"DescLavorazione {label.DescLavorazione.ToString()}");
            sb.AppendLine($"Edizione {label.Edizione.ToString()}");
            sb.AppendLine($"IPLettore {label.IPLettore.ToString()}");
            sb.AppendLine($"Lavorazione {label.Lavorazione.ToString()}");
            sb.AppendLine($"Macchina {label.Macchina.ToString()}");
            sb.AppendLine($"Presenti {label.Presenti.ToString()}");
            sb.AppendLine($"QuantitaFatta {label.QuantitaFatta.ToString()}");
            sb.AppendLine($"QuantitaSuBancale {label.QuantitaSuBancale.ToString()}");
            sb.AppendLine($"Segnatura {label.Segnatura.ToString()}");
            sb.AppendLine($"Stato {label.Stato.ToString()}");
            sb.AppendLine($"Titolo {label.Titolo.ToString()}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string ImpostaQRCode()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append(this.tbCommessa.Text);
                sb.Append("|");
                sb.Append(this.tbMacchina.Text);
                sb.Append("|");
                sb.Append(this.txtOperatori.Text);
                sb.Append("|");
                sb.Append(this.tbBancale.Text);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _exception = ex.Message;
                return string.Empty;
            }
        }

        private string ImpostaReparto(string Macchina)
        {
            string tmp = Macchina;

            if (tmp.Contains("PI"))
                tmp = "mPI01";
            else if (tmp.Contains("CU"))
                tmp = "mCU01";
            else if (tmp.Contains("RA"))
                tmp = "mRA01";

            string reparto = string.Empty;

            switch (tmp)
            {
                case "mAI01":
                    reparto = "AI";
                    break;
                case "mBR01":
                case "mBR02":
                    reparto = "BR";
                    break;
                case "mCA09":
                case "mCA10":
                    reparto = "CO";
                    break;
                case "mCA01":
                case "mCA02":
                case "mCA03":
                case "mCA08":
                case "mCA11":
                    reparto = "CA";
                    break;
                case "mCU01":
                    reparto = "CU";
                    break;
                case "mEX01":
                    reparto = "EX";
                    break;
                case "mOL01":
                    reparto = "OL";
                    break;
                case "mPI01":
                    reparto = "PI";
                    break;
                case "mPM01":
                    reparto = "PM";
                    break;
                case "mRA01":
                    reparto = "RA";
                    break;
                case "mTA01":
                    reparto = "TA";
                    break;
            }

            return reparto;
        }

        private Label LabelReparto(string reparto)
        {
            Label myLabel = new Label();
            myLabel.Name = "lblReparto";
            myLabel.Content = reparto;
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

        private Polygon CreaPoligono(string reparto)
        {
            Polygon poligono = new Polygon();
            poligono.Fill = Brushes.Black;
            poligono.Width = 100;
            poligono.Height = 100;

            PointCollection pc = null;

            switch (reparto)
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

        private void Print(EtichettaIgf et, string destinationPrinter, int copyNumber)
        {
            PrintDialog printDlg = new PrintDialog();

            //Printer settings si riferisce alla stampante predefinita
            PrinterSettings settings = new PrinterSettings();
            string defaultPrinter = settings.PrinterName;
            
            if (System.Environment.MachineName == "CARAC-DELL")
                destinationPrinter = "192.168.172.220";

            try
            {
                System.Printing.LocalPrintServer lps = new System.Printing.LocalPrintServer();      //System.Printing
                System.Printing.PrintQueue pq = lps.GetPrintQueue(destinationPrinter);

                System.Printing.PrintTicket pt = new System.Printing.PrintTicket();
                pt.PageOrientation = System.Printing.PageOrientation.Portrait;
                pt.PageMediaSize = new System.Printing.PageMediaSize(this.Width, this.Height);
                pq.DefaultPrintTicket = pt;
                
                printDlg.PrintQueue = pq;
                printDlg.PrintTicket.CopyCount = copyNumber;
                
                bool founded = true;

                for (int i = 0; i < PrinterSettings.InstalledPrinters.Count && !founded; i++)
                {
                    if (PrinterSettings.InstalledPrinters[i] == destinationPrinter)
                        founded = true;
                }

                if (founded)
                {
                    printDlg.PrintVisual(et, $"{this.lblID.Content}-{this.tbCommessa.Text}-{this.tbMacchina.Text}");
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
    }
}
