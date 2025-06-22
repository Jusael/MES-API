using MesApplicationAPI.Dto;
using MesApplicationAPI.Services;
using MesApplicationAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MesApi.Controllers;

// NOTE:
// DTO → Controller → Bll(Repository) → Dao → SqlCommand
//       Controller 할때 url에 포함 시켜 호출하게되면 LoginController이면,
//       Controller글자를 substring해서 login으로 호출된다. Login=login 대소문자는 상관없다.
//      하기 내용이 실행 되면 url은 http://175.XXX.XX.XX:5216/api/login/login 으로 실행된다.

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly UserService UserService;

    public LoginController(UserService service)
    {
        UserService = service;
    }

    [HttpPost("postuserinfo")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            DataTable dtLoginInfo = await UserService.SelectAsync("SEARCH_USER_INFO", loginDto);

            if (dtLoginInfo == null || dtLoginInfo.Rows.Count == 0)
                throw new Exception();

            return Ok(new
            {
                success = true,
                level = dtLoginInfo.Rows[0]["LEVEL"]
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new { success = false, level = -1 });
        }
    }

    [HttpPost("postfcm")]
    public async Task<IActionResult> postfcm([FromBody] FcmDto dto)
    {
        try
        {
            int? result = await UserService.UpdateAsync("UPDATE_FCM_TOKEN", dto);
            if (result == null || result < 0)
                throw new Exception();

            return Ok(new { success = true, message = "FCM 토큰 업데이트 완료" });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new { success = false, message = "FCM 토큰 업데이트 실패" });
        }
    }

    [HttpPost("postjwt")]
    public async Task<IActionResult> postJwt([FromBody] JwtDto dto)
    {
        try
        {

            DataTable? dt = await UserService.SelectAsync("SEARCH_EXPIRE_DAYS", dto);
            if (dt == null || dt.Rows.Count == 0)
                throw new Exception();

            // 유효기간이 있는 토큰 발급
            DateTime endDate = Convert.ToDateTime(dt.Rows[0]["END_DATE"]);
            TimeSpan diff = endDate - DateTime.Now;
            string jwtToken = TokenHelper.GenerateToken(dto.UserId, diff.Days);

            if (diff.TotalSeconds <= 0)
                throw new Exception("계정 유효기간이 만료되었습니다.");

            dto.JwtToken = jwtToken;

            int? result = await UserService.UpdateAsync("UPDATE_JWT_TOKEN", dto);
            if (result == null || result < -1)
                throw new Exception();

            return Ok(new
            {
                success = true,
                token = jwtToken,
                expiresInDays = diff.Days,
                message = "JWT 토큰 발급 완료"
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new { success = false, message = "JWT 토큰 발급 실패" });
        }
    }
}
