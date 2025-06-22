using MesApplicationAPI.Dao;
using MesApplicationAPI.Dto;
using MesApplicationAPI.Interface;
using MesApplicationAPI.Services;

public class AlarmScheduler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AlarmScheduler> _logger;

    public AlarmScheduler(IServiceProvider serviceProvider, ILogger<AlarmScheduler> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAndSendAlarms();
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }


    private async Task CheckAndSendAlarms()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var alarmService = scope.ServiceProvider.GetRequiredService<IAlarmService>();
            List<AlarmDto> alarms = await alarmService.Select();


            bool isSent = false;

            foreach (var item in alarms)
            {// 3. 전송 요청
                isSent = await FcmHelper.SendAsync(item);

                if (isSent)
                {
                    #region - logger -
                    Console.WriteLine($"--======================================================");
                    Console.WriteLine($"[전송완료] 알람 ID {item.AppAlarmId}");
                    Console.WriteLine($"--======================================================");
                    #endregion

                    int? result = await alarmService.UpdateAsync("UPDATE_SEND_YN", item);
                    if (result == null || result < 0)
                        throw new Exception();

                }
                else
                {
                    #region - logger -
                    Console.WriteLine($"--======================================================");
                    Console.WriteLine($"[전송실패] 알람 ID {item.AppAlarmId}");
                    Console.WriteLine($"--======================================================");
                    #endregion

                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
