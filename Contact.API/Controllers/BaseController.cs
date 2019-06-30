using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contact.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Contact.API.Controllers
{
    public class BaseController : Controller
    {
        protected UserIdentity UserIdentity => new UserIdentity { UserId = "08d6f068-3130-adea-b95d-aa75e162a7c6", Name = "xcz" };
    }
}