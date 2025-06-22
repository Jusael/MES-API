using MesApplicationAPI.Interface;

namespace MesApplicationAPI.Dto
{
    public class JwtDto : IBaseDto
    {
        public string UserId { get; set; }
        public string? JwtToken { get; set; }
    }
}
