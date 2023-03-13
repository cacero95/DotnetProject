namespace Projects.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        public AuthController( IAuthRepository authRepo )
        {
            _authRepo = authRepo;
        }

        [ HttpPost( "Register" )]
        public async Task<ActionResult<ServiceResponse<int>>> Register( UserRegisterDto request ) {
            var response = await _authRepo.Register (
                new User { UserName = request.Username }, request.Password
            );
            return response.Success ? Ok( response ) : BadRequest( response );
        }

        [ HttpPost( "Login" )]
        public async Task<ActionResult<ServiceResponse<int>>> Login( UserLoginDto request ) {
            var response = await _authRepo.Login ( request.Username, request.Password );
            return response.Success ? Ok( response ) : BadRequest( response );
        }
    }
}