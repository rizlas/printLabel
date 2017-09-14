using log4net;
using PrintLabel.WcfService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrintLabel
{
    public partial class Scheduler : ServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Thread _threadOnStart;
        ServiceHost _svcWcfHost = null;

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

            log.Info("Servizio partito....");
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
                log.Error(ex);
            }

        }
    }
}
