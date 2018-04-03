using log4net;
using PrintLabel.WcfService;
using System;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;

namespace PrintLabel
{
    public partial class Scheduler : ServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Thread _threadOnStart;
        ServiceHost _svcWcfHost = null;
        System.Timers.Timer _timerGarbage;
        const int _timerInterval = 21600000;    // 6 Ore

        public Scheduler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            if (_svcWcfHost != null)
            {
                if(_svcWcfHost.State == CommunicationState.Opened)
                    _svcWcfHost.Close();

                _svcWcfHost = null;
            }
        }

        public void Start()
        {
            inizializzaThread();
        }

        private void inizializzaThread()
        {
            log4net.Config.XmlConfigurator.Configure();

            _threadOnStart = new Thread(StartThread);
            _threadOnStart.Name = "StartThread";
            _threadOnStart.IsBackground = false;
            _threadOnStart.Start();

            _timerGarbage = new System.Timers.Timer();
            _timerGarbage.Interval = _timerInterval;
            _timerGarbage.Elapsed += _timerGarbage_Elapsed;
            _timerGarbage.Enabled = true;

            log.Info("Servizio partito....");
        }

        private void _timerGarbage_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GC.Collect();
        }

        private void StartThread()
        {
            try
            {

                var c = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                if (_svcWcfHost != null)
                    _svcWcfHost.Close();

                _svcWcfHost = new ServiceHost(typeof(PrintService));
                _svcWcfHost.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                log.Error(ex);
            }

        }
    }
}
