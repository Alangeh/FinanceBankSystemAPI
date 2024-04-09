using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController(IGenericRepositoryInterface<ApplicationUser> genericRepositoryInterface, AppDbContext appDbContext) : GenericController<ApplicationUser>(genericRepositoryInterface)
    {
        [HttpGet("getbystatus/{status}")]
        public async Task<IActionResult> GetAllByStatusAsync(bool status) => Ok(await appDbContext.ApplicationUsers.Where(u => u.IsActive == status).ToListAsync());

    
    }
}
