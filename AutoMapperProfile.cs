namespace Projects
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharcterDto, Character>();
            CreateMap<UpdateCharacterDto, Character>();
        }
    }
}