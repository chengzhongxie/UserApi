using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : BaseController
    {
        [HttpPost]
        public Task<IActionResult> CreateProject()
        {

        }

        [HttpPut]
        public Task<IActionResult> ViewProject()
        {

        }

        [HttpPut]
        public Task<IActionResult> JoinProject()
        {

        }
    }
}
