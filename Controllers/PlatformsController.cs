using AutoMapper;
using CommandService.Data;
using CommandService.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms() 
        {
            Console.WriteLine("-->Getting Platforms from Commands Service");

            var platformitems = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformitems));
        }

        [HttpPost]
        public ActionResult TestInboundConnection() 
        {
            Console.WriteLine("--> Inbound POST # Command Service");
            return Ok("Inbound test of platforms Controller");
        }
    }
}
