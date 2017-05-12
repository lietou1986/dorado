using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace Vancl.IC.VWS.ClientService
{
    abstract public class ProcessAbstract
    {
        private bool _stop = false;
        private Timer _timer;
        private ILog _logger = LogManager.GetLogger(typeof(ProcessAbstract));

        public void Start(int dueTime, int period)
        {
            _stop = false;

            _timer = new Timer(CallBackTimer, null, dueTime, period);            
        }

        public void Stop()
        {
            _stop = true;
            _timer.Dispose();
        }

        private void CallBackTimer(object state)
        {
            if (_stop)
            {
                return;
            }

            try
            {
                DoWork();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        abstract public void DoWork();        
    }
}
