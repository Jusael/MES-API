using MesApplicationAPI.Interface;

namespace MesApplicationAPI.Dto
{
    public class InventoryListDto : IBaseDto
    {
        public string? ItemCd { get; set; }
        public string? ItemNm { get; set; }
        public string? LotNo { get; set; }
        public string? PackBarCode { get; set; }
        public string? ReceiptQty { get; set; }
        public string? RemainQty { get; set; }
    }
}
