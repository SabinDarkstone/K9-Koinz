
using K9_Koinz.Data;
using static System.Formats.Asn1.AsnWriter;

namespace K9_Koinz.Services.Meta {

    public enum Cron {
        Hourly,
        Daily,
        Weekly
    }

    public struct CronData {
        public readonly Cron Cron;
        public readonly int multiplier;

        public CronData(Cron cron, int multiplier) {
            this.Cron = cron;
            this.multiplier = multiplier;
        }
    }

    public abstract class AbstractWorker<T> : IHostedService, IDisposable {
        protected readonly Timer _timer;
        protected readonly IServiceScopeFactory _scopeFactory;
        protected KoinzContext _context;
        protected readonly ILogger<T> _logger;

        protected AbstractWorker(IServiceScopeFactory scopeFactory, DateTime startTime, CronData repeat, Boolean doRunImmediatelyToo) {
            _scopeFactory = scopeFactory;
            using (var scope =  scopeFactory.CreateScope()) {
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<T>>();
                CreateScopeOnInit(scope);
            }

            if (startTime < DateTime.Now) {
                startTime += getRepeatFromCron(repeat);
            }
            var timeUntilStart = startTime - DateTime.Now;

            _timer = new Timer(DoWork, null, timeUntilStart, getRepeatFromCron(repeat));
            _logger.LogInformation("Creating scheduled job to start at " + startTime.ToString() + " which is in " + timeUntilStart.TotalHours.ToString() + " hours and will repeat every " + getRepeatFromCron(repeat).TotalHours + " hours");

            if (doRunImmediatelyToo) {
                _logger.LogInformation("Run Immediately is set, so running an instance of the job now...");
                DoWork(null);
            }
        }

        protected virtual void CreateScopeOnInit(IServiceScope scope) {
            _logger.LogDebug("No init scope needed for " + this.GetType().Name);
        }

        private TimeSpan getRepeatFromCron(CronData cron) {
            switch (cron.Cron) {
                case Cron.Daily:
                    return TimeSpan.FromDays(cron.multiplier);
                case Cron.Weekly:
                    return TimeSpan.FromDays(7 * cron.multiplier);
                case Cron.Hourly:
                    return TimeSpan.FromHours(cron.multiplier);
                default:
                    return TimeSpan.Zero;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        protected virtual void CreateAdditionalScope(IServiceScope scope) {
            _logger.LogDebug("No additional scope needed for " + this.GetType().Name);
        }

        protected virtual void DoWork(object state) {
            BeforeWork();
        }

        private void BeforeWork() {
            var scope = _scopeFactory.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<KoinzContext>();
            CreateAdditionalScope(scope);
        }

        public virtual void Dispose() {
            _timer.Dispose();
            _context = null;
        }
    }
}
