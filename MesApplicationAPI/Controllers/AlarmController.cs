using MesApplicationAPI.Dto;
using MesApplicationAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MesApi.Controllers;

// NOTE:
// DTO → Controller → Bll(Repository) → Dao → SqlCommand
//       Controller 할때 url에 포함 시켜 호출하게되면 LoginController이면,
//       Controller글자를 substring해서 login으로 호출된다. Login=login 대소문자는 상관없다.
//      하기 내용이 실행 되면 url은 http://175.XXX.XX.XX:5216/api/login/login 으로 실행된다.

[ApiController]
[Route("api/[controller]")]
public class AlarmController : ControllerBase
{
    private readonly AlarmService _alarmService;
    private readonly ExcuteSpService _excuteSpService;

    public AlarmController(AlarmService alarmService)
    {
        _alarmService = alarmService;
    }

    //GET은 HttpGet으로 선언 받는 값은 FromQuery로 받아야 오류가안난다.
    [HttpGet("getincomingalarmbutunread")]
    public async Task<IActionResult> SearchUnreadList([FromQuery] AlarmControlDto alarmControlDto)
    {
        try
        {
            List<AlarmDto> alarms = await _alarmService.SelectDtoAsync("GET_UNREAD_LIST", alarmControlDto);

            #region - logger -
            var request = HttpContext.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"--======================================================");
            Console.WriteLine($"앱 종료 후 알람 클릭시 누락 알람 리스트 조회 URL: {fullUrl}");
            // 2. 개별 쿼리 파라미터 출력
            Console.WriteLine($"UserId: {alarmControlDto.UserId}");
            Console.WriteLine($"--알람 리스트 총 전송 {alarms.Count}건--");

            foreach (var alarm in alarms)
            {
                //Console.WriteLine($"AppAlarmId: {alarm.AppAlarmId}" +
                //    $", Title: {alarm.Title}" +
                //    $", Content1: {alarm.Content1}" +
                //    $", CreateTime: {alarm.CreateTime}");
                Console.WriteLine($"--AppAlarmId :  {alarm.AppAlarmId} UserId : {alarm.UserId} ");
            }
            Console.WriteLine($"--======================================================");
            #endregion


            return Ok(new
            {
                success = true,
                alarms = alarms
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new { success = false  });
        }
    }


    //GET은 HttpGet으로 선언 받는 값은 FromQuery로 받아야 오류가안난다.
    [HttpPost("postalarmstatuscontroll")]
    public async Task<IActionResult> UpdateReadAlarm([FromBody] AlarmControlDto alarmControlDto)
    {
        try
        {
            await _alarmService.UpdateAsync("UPDATE_ALARM_STATUS", alarmControlDto);

            #region - logger -
            var request = HttpContext.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"--======================================================");
            Console.WriteLine($"알람 상태 변환 요청 URL: {fullUrl}");
            // 2. 개별 쿼리 파라미터 출력
            Console.WriteLine($"AppAlarmId: {alarmControlDto.AppAlarmId}");
            Console.WriteLine($"AlarmStatus: {alarmControlDto.AlarmStatus}");
            Console.WriteLine($"--======================================================");
            #endregion


            return Ok(new
            {
                success = true,
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("알람 데이터 미매칭 " + ex.Message);
            return Unauthorized(new { success = false });
        }
    }

}
