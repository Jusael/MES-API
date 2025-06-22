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
public class CommonController : ControllerBase
{

    private readonly CommonService _commonService;

    public CommonController(CommonService commonService)
    {
        _commonService = commonService;
    }


    //GET은 HttpGet으로 선언 받는 값은 FromQuery로 받아야 오류가안난다.
    [HttpGet("getlocationbarcodeinfo")]
    public async Task<IActionResult> GetBarocdeInfo([FromQuery] BarcodeDto barcodeDto)
    {
        try
        {
            DataTable dt = await _commonService.SelectAsync("SEARCH_LOCATION_BARCODE_INFO", barcodeDto);


            var request = HttpContext.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"--======================================================");
            Console.WriteLine($"장소바코드 정보 확인 요청  URL: {fullUrl}");
            // 2. 개별 쿼리 파라미터 출력
            Console.WriteLine($"BarCode: {barcodeDto.BarCode}");
            Console.WriteLine($"--======================================================");

            if (dt == null || dt.Rows.Count == 0)
                throw new Exception("일치하는 바코드 정보가 없습니다.");

            return Ok(new
            {
                success = true,
                wareHouseCd = "WH01",
                wareHouseNm= "원자재 창고",
                zoneCd = dt.Rows[0]["ZONE_CD"].ToString(),
                zoneNm = "3구역",
                cellCd = dt.Rows[0]["CELL_CD"].ToString(),
                cellNm = dt.Rows[0]["CELL_REMARK"].ToString(),
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new { success = false });
        }
    }

    //HttpPost으로 선언 받는 값은 FromBody 받아야 오류가안난다.
    [HttpPost("postExample")]
    public async Task<IActionResult> Example([FromBody] SignDto signDto)
    {
        try
        {
           
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
