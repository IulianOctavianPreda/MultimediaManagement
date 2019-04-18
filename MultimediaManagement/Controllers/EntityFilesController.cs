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
    public class EntityFilesController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;

        public EntityFilesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/EntityFiles
        [HttpGet]
        public async Task<IEnumerable<EntityFile>> GetEntityFile()
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                return await _unitOfWork.EntityFile.GetAll();
            }
        }

        // GET: api/EntityFiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntityFile>> GetEntityFile([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                var entityFile = await _unitOfWork.EntityFile.Get(id);

                if (entityFile == null)
                {
                    return NotFound();
                }

                return entityFile;
            }
        }

        // PUT: api/EntityFiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntityFile([FromRoute] Guid id, [FromBody] EntityFile entityFile)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();

                if (entityFile == null)
                {
                    return NotFound();
                }

                if (id != entityFile.Id)
                {
                    return BadRequest();
                }
                _unitOfWork.EntityFile.Update(entityFile);
                _unitOfWork.Commit();
                return Ok(entityFile);
            }
        }

        // POST: api/EntityFiles
        [HttpPost]
        public ActionResult<EntityFile> PostEntityFile([FromBody] EntityFile entityFile)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();

                entityFile.Id = Guid.NewGuid();
                _unitOfWork.EntityFile.Add(entityFile);

                _unitOfWork.Commit();
                return Ok(entityFile);
            }
        }

        // DELETE: api/EntityFiles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EntityFile>> DeleteEntityFile([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                var entityFile = await _unitOfWork.EntityFile.Get(id);
                _unitOfWork.EntityFile.Remove(entityFile);
                _unitOfWork.Commit();
                return Ok(entityFile);
            }
        }

        private bool EntityFileExists([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                return _unitOfWork.EntityFile.Any(e => e.Id == id);
            }
        }
    }
}
