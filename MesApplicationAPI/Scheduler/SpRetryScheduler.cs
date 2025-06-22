using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MesApplicationAPI.Services
{
    public class SpRetryScheduler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // 5분 간격

        public SpRetryScheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var spService = scope.ServiceProvider.GetRequiredService<ExcuteSpService>();

                    await spService.RetryFailedSpExecutionsAsync(); // 실제 재시도 로직 호출
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[스케줄러 오류] {ex.Message}");
                }

                await Task.Delay(_interval, stoppingToken); // 다음 실행까지 대기
            }
        }
    }
}