using MesApplicationAPI.Interface;

namespace MesApplicationAPI.Dto
{
    public class PutAwayBarcodeDto : IBaseDto
    {
        public string BarCode { get; set; }
        public string Location { get; set; }
    }
}
