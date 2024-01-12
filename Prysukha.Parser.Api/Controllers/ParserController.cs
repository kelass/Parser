using Microsoft.AspNetCore.Mvc;
using Prysukha.Parser.Infrastructure.Interfaces;

namespace Prysukha.Parser.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        private readonly IParser _parser;
        public ParserController(IParser parser)
        {
            _parser = parser;
        }

        [HttpGet]
        public async Task Parse()
        {
            await _parser.StartParsingAsync();
        }
    }
}
