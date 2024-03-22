using K9_Koinz.Data;
using K9_Koinz.Models;

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
        protected IRepositoryWrapper _data;
        protected readonly ILogger<T> _logger;

        private ScheduledJobStatus statusRecord;
        private TimeSpan repeat;

        protected AbstractWorker(IServiceScopeFactory scopeFactory, DateTime startTime, CronData repeat, bool doRunImmediatelyToo) {
            this.repeat = getRepeatFromCron(repeat);
            _scopeFactory = scopeFactory;
            using (var scope =  scopeFactory.CreateScope()) {
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<T>>();
                CreateScopeOnInit(scope);
            }

            if (startTime < DateTime.Now) {
                startTime += getRepeatFromCron(repeat);
            }
            var timeUntilStart = startTime - DateTime.Now;

            _timer = new Timer(ExecuteJob, null, timeUntilStart, getRepeatFromCron(repeat));
            _logger.LogInformation("Creating scheduled job to start at " + startTime.ToString() + " which is in " + timeUntilStart.TotalHours.ToString() + " hours and will repeat every " + getRepeatFromCron(repeat).TotalHours + " hours");

            if (doRunImmediatelyToo) {
                _logger.LogInformation("Run Immediately is set, so running an instance of the job now...");
                ExecuteJob(null);
            }
        }

        protected virtual void CreateScopeOnInit(IServiceScope scope) {
            _logger.LogDebug("No init scope needed for " + this.GetType().Name);
        }

        private void ExecuteJob(object state) {
            BeforeWork();
            try {
                DoWork(state);
                AfterWork();
            } catch (Exception ex) {
                WorkError(ex);
            }
            FinalizeJob();
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

        protected abstract void DoWork(object state);

        private void BeforeWork() {
            statusRecord = new ScheduledJobStatus {
                StartTime = DateTime.Now,
                JobName = this.GetType().Name,
                Status = "In Progress"
            };

            var scope = _scopeFactory.CreateScope();
            _data = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

            _data.JobStatusRepository.Add(statusRecord);
            _data.Save();

            CreateAdditionalScope(scope);
        }

        private void WorkError(Exception ex) {
            statusRecord.ErrorMessages = ex.Message;
            statusRecord.Status = "Error";
            statusRecord.StackTrace = ex.StackTrace;
            statusRecord.EndTime = DateTime.Now;
        }

        private void AfterWork() {
            statusRecord.Status = "Complete";
            statusRecord.EndTime = DateTime.Now;
        }

        private void FinalizeJob() {
            statusRecord.NextRunTime = statusRecord.StartTime + repeat;

            _data.JobStatusRepository.Update(statusRecord);
            _data.Save();
        }

        public virtual void Dispose() {
            _timer.Dispose();
            _data = null;
        }
    }
}
