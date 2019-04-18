using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultimediaManagement.Models;
using MultimediaManagement.UoW;

namespace MultimediaManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IEnumerable<User>> GetUser()
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                return await _unitOfWork.User.GetAll();
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                var user = await _unitOfWork.User.Get(id);

                if (user == null)
                {
                    return NotFound();
                }

                return user;
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public IActionResult PutUser([FromRoute] Guid id, [FromBody] User user)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();

                if (user == null)
                {
                    return NotFound();
                }

                if (id != user.Id)
                {
                    return BadRequest();
                }
                _unitOfWork.User.Update(user);
                _unitOfWork.Commit();
                return Ok(user);
            }
        }

        // POST: api/Users
        [HttpPost]
        public ActionResult<User> PostUser([FromBody] User user)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();

                user.Id = Guid.NewGuid();
                _unitOfWork.User.Add(user);

                _unitOfWork.Commit();
                return Ok(user);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                var user = await _unitOfWork.User.Get(id);
                _unitOfWork.User.Remove(user);
                _unitOfWork.Commit();
                return Ok(user);
            }
        }

        private bool UserExists([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                return _unitOfWork.User.Any(e => e.Id == id);
            }
        }
    }
}
