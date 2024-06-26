﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T>(IGenericRepositoryInterface<T> genericRepositoryInterface) :
        ControllerBase where T : class
    {
        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAll() => Ok(await genericRepositoryInterface.GetAllAsync());

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return BadRequest("Invalid request");
            return Ok(await genericRepositoryInterface.DeleteById(id));
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid request");
            return Ok(await genericRepositoryInterface.GetByIdAsync(id));
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> Add(T model)
        {
            if (model is null) return BadRequest("Invalid request");
            return Ok(await genericRepositoryInterface.Insert(model));
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update(T model)
        {
            if (model is null) return BadRequest("Invalid request");
            return Ok(await genericRepositoryInterface.Update(model));
        }
    }
}
