using MesApplicationAPI.Interface;

namespace MesApplicationAPI.Dto
{
    public class LoginDto : IBaseDto
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
