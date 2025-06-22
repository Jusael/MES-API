using MesApplicationAPI.Interface;

namespace MesApplicationAPI.Dto
{
    public class FcmDto : IBaseDto
    {
        public string UserId { get; set; }
        public string FcmToken { get; set; }
    }
}
