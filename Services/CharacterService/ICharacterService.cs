namespace Projects.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterDto>>> GetCharacters();
        Task<ServiceResponse<GetCharacterDto>> GetSingle( int id );
        Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter( AddCharcterDto character );
        Task<ServiceResponse<GetCharacterDto>> UpdateCharacter( UpdateCharacterDto updatedCharacter );
        Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter( int id );
    }
}