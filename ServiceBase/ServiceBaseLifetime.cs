using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Ardbot.ServiceBase {
    public class ServiceBaseLifetime : System.ServiceProcess.ServiceBase, IHostLifetime {
        private readonly TaskCompletionSource<object> _delayStart = new TaskCompletionSource<object>();

        [Obsolete]
        private IApplicationLifetime ApplicationLifetime {  get; }

        [Obsolete]
        public ServiceBaseLifetime(
            IApplicationLifetime applicationLifetime
        ){
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        [Obsolete]
        public Task WaitForStartAsync(CancellationToken cancellationToken){
            cancellationToken.Register(() => _delayStart.TrySetCanceled());
            ApplicationLifetime.ApplicationStopping.Register(Stop);

            new Thread(Run).Start();
            return _delayStart.Task;
        }

        public Task StopAsync(CancellationToken cancellationToken){
            Stop();
            return Task.CompletedTask;
        }

        private void Run() {
            try {
                Run(this); //This blocks until the service is stopped.
                _delayStart.TrySetException(new InvalidOperationException("Stopped without starting"));
            }
            catch (Exception ex) {
                _delayStart.TrySetException(ex);
            }
        }

        protected override void OnStart(string[] args){
            _delayStart.TrySetResult(null);
            base.OnStart(args);
        }

        [Obsolete]
        protected override void OnStop(){
            ApplicationLifetime.StopApplication();
            base.OnStop();
        }
    }
}