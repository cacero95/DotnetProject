namespace Projects.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character> {
            new Character{
                Name = "Sam"
            },
            new Character {
                Name = "Frodo",
                Id = 1
            }
        };
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CharacterService( IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor )
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Get the current user logged
        private int GetUserId () => int.Parse (
            _httpContextAccessor.HttpContext!
                .User.FindFirstValue( ClaimTypes.NameIdentifier )!
        );

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharcterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>( newCharacter );
            var userId = GetUserId();
            character.User = await _context.Users.FirstOrDefaultAsync(
                user => user.Id == userId
            );
            _context.Characters.Add( _mapper.Map<Character>( character ));
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Characters
                .Where( c => c.User!.Id == userId )
                .Select( character => _mapper.Map<GetCharacterDto>( character))
                .ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try {
                var userId = GetUserId();
                var character = await _context.Characters.FirstAsync( c => c.Id == id && c.User!.Id == userId );
                if ( character == null ) {
                    throw new Exception($"Personaje con el Id { id } no fue encontrado");
                }
                _context.Characters.Remove( character );
                await _context.SaveChangesAsync();
                serviceResponse.Data = await _context.Characters
                    .Where( c => c.User!.Id == userId )
                    .Select( c => _mapper.Map<GetCharacterDto>(c) )
                    .ToListAsync();
                return serviceResponse;
            } catch ( Exception err ) {
                serviceResponse.Success = false;
                serviceResponse.Message = err.Message;
                return serviceResponse;
            }
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters
                .Where( c => c.User!.Id == GetUserId())
                .ToListAsync();
            serviceResponse.Data = dbCharacters.Select( c => _mapper.Map<GetCharacterDto>(c) ).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetSingle(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _context.Characters
                .FirstOrDefaultAsync( character => character.Id == id && character.User!.Id == GetUserId() );
            if ( dbCharacter == null ) {
                serviceResponse.Message = "No se encontro el registro";
                serviceResponse.Success = false;
            }
            serviceResponse.Data = _mapper.Map<GetCharacterDto>( dbCharacter );
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try {
                var character = await _context.Characters
                    .Include( c => c.User )
                    .FirstOrDefaultAsync( c => c.Id == updatedCharacter.Id && c.User!.Id == GetUserId() );
                if ( character == null ) {
                    throw new Exception($"Personaje con el Id { updatedCharacter.Id } no fue encontrado");
                }
                character = _mapper.Map( updatedCharacter, character );
                // character.Name = updatedCharacter.Name;
                // character.HitPoints = updatedCharacter.HitPoints;
                // character.Strength = updatedCharacter.Strength;
                // character.Defense = updatedCharacter.Defense;
                // character.Intelligence = updatedCharacter.Intelligence;
                // character.Class = updatedCharacter.Class;
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterDto>( character );
                return serviceResponse;
            } catch ( Exception err ) {
                serviceResponse.Success = false;
                serviceResponse.Message = err.Message;
                return serviceResponse;
            }
        }
    }
}