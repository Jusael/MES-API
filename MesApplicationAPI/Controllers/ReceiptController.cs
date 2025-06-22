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
public class ReceiptController : ControllerBase
{

    private readonly ReceiptService _receiptService;
    private readonly ReceiptPackService _receiptPackService;

    public ReceiptController(ReceiptService receiptService, ReceiptPackService receiptPackService)
    {
        _receiptService = receiptService;
        _receiptPackService = receiptPackService;
    }


    //GET은 HttpGet으로 선언 받는 값은 FromQuery로 받아야 오류가안난다.
    [HttpGet("getbarcodeinfo")]
    public async Task<IActionResult> GetBarocdeInfo([FromQuery] BarcodeDto barcodeDto)
    {
        try
        {
            DataTable dt = await _receiptPackService.SelectAsync("SEARCH_BARCODE_INFO", barcodeDto);


            var request = HttpContext.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"--======================================================");
            Console.WriteLine($"바코드 정보 확인 요청  URL: {fullUrl}");
            // 2. 개별 쿼리 파라미터 출력
            Console.WriteLine($"BarCode: {barcodeDto.BarCode}");
            Console.WriteLine($"--======================================================");

            if (dt == null || dt.Rows.Count == 0)
                throw new Exception("일치하는 바코드 정보가 없습니다.");

            return Ok(new
            {
                success = true,
                itemNm = dt.Rows[0]["ITEM_NM"].ToString(),
                itemCd = dt.Rows[0]["ITEM_CD"].ToString(),
                receiptLotNo = dt.Rows[0]["RECEIPT_LOT_NO"].ToString(),
                receiptValidDate = dt.Rows[0]["RECEIPT_VALID_DATE"].ToString(),
                receiptPackQty = dt.Rows[0]["RECEIPT_PACK_QTY"].ToString(),
                receiptPackRemainQty = dt.Rows[0]["RECEIPT_PACK_REMAIN_QTY"].ToString(),
                receiptStatus = dt.Rows[0]["RECEIPT_STATUS"].ToString(),
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new { success = false });
        }
    }

    //GET은 HttpGet으로 선언 받는 값은 FromQuery로 받아야 오류가안난다.
    [HttpGet("getinventorylist")]
    public async Task<IActionResult> GetInventoryList([FromQuery] BarcodeDto barcodeDto)
    {
        try
        {
            List<InventoryListDto> result = await _receiptPackService.SelectAsyncDto<BarcodeDto, InventoryListDto>("SEARCH_INVENTORY_LIST", barcodeDto);

            var request = HttpContext.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"--======================================================");
            Console.WriteLine($"적치 바코드 정보 확인 요청  URL: {fullUrl}");
            // 2. 개별 쿼리 파라미터 출력
            Console.WriteLine($"BarCode: {barcodeDto.BarCode}");
            Console.WriteLine($"--======================================================");

            if (result == null)
                throw new Exception("적치 재고 조회중 오류가 발생하였습니다.");

            return Ok(new
            {
                success = true,
                result = result
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Unauthorized(new { success = false });
        }
    }


    //HttpPost으로 선언 받는 값은 FromBody 받아야 오류가안난다.
    [HttpPost("productputaway")]
    public async Task<IActionResult> UpdatePutAwayBarocde([FromBody] PutAwayBarcodeDto putAwayBarcodeDto)
    {
        try
        {
            #region - logger -
            var request = HttpContext.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"--======================================================");
            Console.WriteLine($" 요청 URL: {fullUrl}");

            // 2. Body 내용을 다시 출력 (signDto를 이미 받은 경우 직접 속성을 출력)
            Console.WriteLine("바코드 피킹 및 적치 요청:");
            Console.WriteLine($"BarCode: {putAwayBarcodeDto.BarCode}");
            Console.WriteLine($"Location: {putAwayBarcodeDto.Location}");
            Console.WriteLine($"--======================================================");
            #endregion
            int? result = await _receiptPackService.UpdateAsync("UPDATE_BARCODE_INFO", putAwayBarcodeDto);

            if (result == null || result < 0)
                throw new Exception("적치에 실패하였습니다.");

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


    //HttpPost으로 선언 받는 값은 FromBody 받아야 오류가안난다.
    [HttpPost("productpicking")]
    public async Task<IActionResult> UpdatePickingBarocde([FromBody] BarcodeDto barcodeDto)
    {
        try
        {
            #region - logger -
            var request = HttpContext.Request;
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"--======================================================");
            Console.WriteLine($" 요청 URL: {fullUrl}");

            // 2. Body 내용을 다시 출력 (signDto를 이미 받은 경우 직접 속성을 출력)
            Console.WriteLine("바코드 피킹 및 적치 요청:");
            Console.WriteLine($"BarCode: {barcodeDto.BarCode}");
            Console.WriteLine($"--======================================================");
            #endregion
            int? result = await _receiptPackService.UpdateAsync("UPDATE_PICKING_BARCODE", barcodeDto);

            if (result == null || result < 0)
                throw new Exception("피킹에 실패하였습니다.");

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
