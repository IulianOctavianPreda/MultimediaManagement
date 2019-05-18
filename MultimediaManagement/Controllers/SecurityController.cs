using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultimediaManagement.Repository;

namespace MultimediaManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private IUserRepository _user;

        public SecurityController(IUserRepository user)
        {
            _user = user;
        }

        public class Token
        {
            public Guid token;
        }

        [HttpPost]
        [Route("searchToken")]
        public IActionResult searchTokenAndUpdateTime([FromBody]Token token)
        {
            using (_user)
            {
                var user = _user.FirstOrDefault(x => x.Token == token.token);
                if (user != null)
                {
                    if (user.Token.Equals(token.token) && (DateTime.Now - user.ModifiedOn).TotalHours < 24)
                    {
                        user.ModifiedOn = DateTime.Now;
                        _user.Update(user);
                        _user.Commit();
                        return Ok();
                    }
                }

                return NotFound();
            }
        }


        [HttpPost]
        [Route("getRole")]
        public IActionResult getRoleByToken([FromBody]Token token)
        {
            using (_user)
            {
                var user = _user.FirstOrDefault(x => x.Token == token.token);
                if(user.Username == "guest")
                {
                    return Ok(new {Role = "guest"});
                }
                else
                {
                    return Ok(new { Role = "user" });
                }
            }
        }
    }
}