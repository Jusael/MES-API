using MesApplicationAPI.Dto;
using MesApplicationAPI.Services;
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
public class SignController : ControllerBase
{
    private readonly SignService _signService;
    private readonly ExcuteSpService _excuteSpService;

    public SignController(SignService signService, ExcuteSpService excuteSpService)
    {
        _signService = signService;
        _excuteSpService = excuteSpService;
    }

    //GET은 HttpGet으로 선언 받는 값은 FromQuery로 받아야 오류가안난다.
    [HttpGet("getusersigninfo")]
    public async Task<IActionResult> SignAuthorityCheck([FromQuery] SignDto signDto)
    {
        try
        {
            DataTable dt = await _signService.SelectAsync("SEARCH_USER_AUTHORITY", signDto);

            var request = HttpContext.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"--======================================================");
            Console.WriteLine($"전자서명 대상여부 체크  URL: {fullUrl}");
            // 2. 개별 쿼리 파라미터 출력
            Console.WriteLine($"UserId: {signDto.UserId}");
            Console.WriteLine($"SignId: {signDto.SignId}");
            Console.WriteLine($"SignCd: {signDto.SignCd}");
            Console.WriteLine($"--======================================================");

            if (dt == null || dt.Rows.Count == 0)
                throw new Exception();

            return Ok(new
            {
                success = true,
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new { success = false });
        }
    }

    //GET은 HttpGet으로 선언 받는 값은 FromQuery로 받아야 오류가안난다.
    [HttpGet("getsearchSignInfo")]
    public async Task<IActionResult> SearchSignInfo([FromQuery] SignDto signDto)
    {
        try
        {
            DataTable dt = await _signService.SelectAsync("SEARCH_SIGN_INFO", signDto);


            var request = HttpContext.Request;
            Console.WriteLine($"--======================================================");
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            Console.WriteLine($"전자서명 정보 조회  URL: {fullUrl}");

            // 2. 개별 쿼리 파라미터 출력
            Console.WriteLine($"UserId: {signDto.UserId}");
            Console.WriteLine($"SignId: {signDto.SignId}");
            Console.WriteLine($"SignCd: {signDto.SignCd}");
            Console.WriteLine($"--======================================================");

            if (dt == null || dt.Rows.Count == 0)
                throw new Exception();

            var signDetailNm = dt.Rows[0]["SIGN_DETAIL_NM"].ToString();
            var signDetailUserNm = dt.Rows[0]["SIGN_DETAIL_USER_NM"].ToString();
            var signDetailUserId = dt.Rows[0]["SIGN_DETAIL_USER_ID"].ToString();
            var signSignEmpCd = dt.Rows[0]["SIGN_EMP_CD"].ToString();
            var signSignEmpNm = dt.Rows[0]["SIGN_EMP_NM"].ToString();
            var signTime = dt.Rows[0]["SIGN_TIME"].ToString();
            var signImage = dt.Rows[0]["SIGN_IMAGE"];


            return Ok(new
            {
                success = true,
                signDetailNm = signDetailNm,        // 서명 대상자 직무
                signDetailUserNm = signDetailUserNm,// 서명 대상자 명
                signDetailUserId = signDetailUserId,// 서명 대상자 
                signSignEmpCd = signSignEmpCd, // 서명자 사번
                signSignEmpNm = signSignEmpNm, // 서명자 명
                signTime = signTime, // 서명자 시간
                signImage = signImage // 서명자 서명이미지
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new
            {
                success = false,
                signDetailNm = string.Empty,
                signDetailUserNm = string.Empty,
                signDetailUserId = string.Empty,
                signSignEmpCd = string.Empty,
                signSignEmpNm = string.Empty,
                signTime = string.Empty,
                signImage = string.Empty
            });
        }
    }

    //GET은 HttpGet으로 선언 받는 값은 FromQuery로 받아야 오류가안난다.
    [HttpPost("postsigning")]
    public async Task<IActionResult> SignIng([FromBody] SignDto signDto)
    {
        try
        {
            #region - logger -
            var request = HttpContext.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"--======================================================");
            Console.WriteLine($" 요청 URL: {fullUrl}");

            // 2. Body 내용을 다시 출력 (signDto를 이미 받은 경우 직접 속성을 출력)
            Console.WriteLine("전자서명 업데이트 요청 Body 내용:");
            Console.WriteLine($"UserId: {signDto.UserId}");
            Console.WriteLine($"SignCd: {signDto.SignCd}");
            Console.WriteLine($"SignId: {signDto.SignId}");
            Console.WriteLine($"AppAlarmId: {signDto.AppAlarmId}");
            Console.WriteLine($"MesAlarmId: {signDto.MesAlarmId}");
            Console.WriteLine($"Key1: {signDto.Key1}");
            Console.WriteLine($"Key2: {signDto.Key2}");
            Console.WriteLine($"Key3: {signDto.Key3}");
            Console.WriteLine($"Key4: {signDto.Key4}");
            Console.WriteLine($"Key5: {signDto.Key5}");
            Console.WriteLine($"--======================================================");
            #endregion
            int? result = await _signService.UpdateAsync("UPDATE_SIGNING", signDto);

            if (result == null || result < 0)
                throw new Exception("전자서명에 실패하였습니다.");

            //리턴값을 절대 무시하는 비동기 실행으로  후속 처리
            _ = Task.Run(() => _excuteSpService.ExcuteStoredProcedure(signDto));

            return Ok(new
            {
                success = true,
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new { success = false });
        }
    }

}
