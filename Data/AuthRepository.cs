using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Projects.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration; // with thid variable it allowed to access to the project configuration
        public AuthRepository( DataContext context, IConfiguration configuration )
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<int>> Register( User user, string password )
        {
            var response = new ServiceResponse<int>();
            if ( await UserExists( user.UserName ) ) {
                response.Success = false;
                response.Message = "El usuario ya existe";
                return response;
            }
            CreatePasswordHash( password, out byte[] passwordHash, out byte[] passwordSalt );
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.Users.Add( user );
            await _context.SaveChangesAsync();
            response.Data = user.Id;
            return response;
        }

        public async Task<ServiceResponse<string>> Login( string username, string password )
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.UserName.ToLower().Equals( username.ToLower())
            );
            if ( user == null || !VerifyPasswordHash( password, user.PasswordHash, user.PasswordSalt )) {
                response.Success = false;
                response.Message = "Usuario no encontrado";
                return response;
            } else if ( !VerifyPasswordHash( password, user.PasswordHash, user.PasswordSalt )) {
                response.Success = false;
                response.Message = "La contraseña es incorrecta";
                return response;
            }
            response.Success = true;
            response.Data = CreateToken( user );
            return response;
        }

        public async Task<bool> UserExists( string username )
        {
            // validate if the username exist in the database
            return await _context.Users.AnyAsync( user => user.UserName.ToLower() == username.ToLower());
        }
        private void CreatePasswordHash( string password, out byte[] passwordHash, out byte[] passwordSalt ) {
            using( var hmac = new System.Security.Cryptography.HMACSHA512() ) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash( System.Text.Encoding.UTF8.GetBytes( password ));
            }
        }
        private bool VerifyPasswordHash ( string password, byte[] passswordHash, byte[] passwordSalt ) {
            using ( var hmac = new System.Security.Cryptography.HMACSHA512( passwordSalt )) {
                // validate the password hash with database byte by byte
                return hmac.ComputeHash( System.Text.Encoding.UTF8.GetBytes( password )).SequenceEqual( passswordHash );
            }
        }
        private string CreateToken ( User user ) {
            var claims = new List<Claim> {
                new Claim( ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim( ClaimTypes.Name, user.UserName ),
                new Claim( "permisos", "administrador" )
            };
            var appSettingsToken = _configuration.GetSection( "AppSettings:Token" ).Value;
            if ( appSettingsToken == null ) {
                throw new Exception( "No esta configurado el token en appsettings.json en el backend" );
            }
            SymmetricSecurityKey key = new SymmetricSecurityKey( System.Text.Encoding.UTF8.GetBytes( appSettingsToken ));
            SigningCredentials credentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha512Signature );
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity( claims ),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken( tokenDescriptor );
            return tokenHandler.WriteToken( token );
        }
    }
}