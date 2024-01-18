using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorsHandler.Business.Abstraction;
using AuthorsHandler.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AuthorsHandler.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthorsHandlerController : ControllerBase {    
        private readonly IBusiness business_;
        private readonly ILogger logger_;

        public AuthorsHandlerController(IBusiness business, ILogger<AuthorsHandlerController> logger) {
            business_ = business;
            logger_ = logger; 
        }

        [HttpPut(Name = "CreateAuthor")]
        public async Task<IActionResult> CreateAuthor(AuthorDto authorDto) {
            
            bool isCreated = await business_.CreateAuthor(authorDto);

            if (isCreated)
                return Ok("Author " + authorDto.name + " " + authorDto.surname + " added");
            else
                return BadRequest("The author already exists");
        }

        [HttpGet(Name = "GetAuthorIdFromName")]
        public async Task<IActionResult> GetAuthorIdFromName(string name, string surname) {
            int? res = await business_.GetAuthorIdFromName(name, surname);

            if (res == null)
                return NotFound("Author " + name + " " + surname + " not found");

            return Ok("Author " + name + " " + surname + " has id: " + res);
        }

        [HttpGet(Name = "GetExternalLinksForAuthor")]
        public async Task<IActionResult> GetExternalLinksForAuthor(string name, string surname) {
            ICollection<string>? res = await business_.GetExternalLinksForAuthor(name, surname);

            if (res == null)
                return NotFound("Author " + name + " " + surname + " not found");

            return Ok("Author " + name + " " + surname + " has urls:\n" + String.Join("\n", res));
        }
    }
}