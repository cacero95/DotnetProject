using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Projects.Controllers
{
    [ Authorize ]
    [ ApiController ]
    [ Route( "api/[controller]" ) ]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        
        public CharacterController( ICharacterService characterService )
        {
            _characterService = characterService;
        }
        // [ AllowAnonymous ] // allow request without the token authorization
        // [ HttpGet( "GetAll" ) ]
        // public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetAll() {
        //     /**
        //         Ok the request was successfull
        //         BadRequest all wrong
        //         NotFound 
        //     */
        //     int id = int.Parse ( User.Claims.FirstOrDefault( c => c.Type == ClaimTypes.NameIdentifier )!.Value );
        //     return Ok( await _characterService.GetCharacters( id ));
        // }
        [ HttpGet( "GetAll" ) ]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetAll() {
            /**
                Ok the request was successfull
                BadRequest all wrong
                NotFound 
            */
            // User.Claims get the current user logged
            return Ok( await _characterService.GetCharacters());
        }

        [ HttpGet( "{id}" ) ]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetSingle( int id ) {
            /**
                Ok the request was successfull
                BadRequest all wrong
                NotFound 
            */
            return Ok( await _characterService.GetSingle( id ) );
        }
        
        [ HttpPost ]
        public async Task <ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter ( AddCharcterDto newCharacter ) {
            return Ok( await _characterService.AddCharacter( newCharacter ) );
        }

        [ HttpPut ]
        public async Task <ActionResult<ServiceResponse<List<GetCharacterDto>>>> PutCharacter ( UpdateCharacterDto UpdatedCharacter ) {
            var response = await _characterService.UpdateCharacter( UpdatedCharacter );
            return response.Data is null ? NotFound( response ) : Ok( response );
        }

        [ HttpDelete("{id}") ]
        public async Task <ActionResult<ServiceResponse<List<GetCharacterDto>>>> DeleteCharacter ( int id ) {
            var response = await _characterService.DeleteCharacter( id );
            return response.Data is null ? NotFound( response ) : Ok( response );
        }
    }
}