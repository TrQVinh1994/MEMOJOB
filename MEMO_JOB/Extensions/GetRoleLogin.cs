using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MEMO_JOB.Models;
using MEMO_JOB.Models.JobSeekers;
using MEMO_JOB.Models.JobSeekers.AccountViewModels;
using MEMO_JOB.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using MEMO_JOB.Extensions;

namespace MEMO_JOB.Extensions
{
    public class GetRoleLogin
    {
        public MEMO_JOBContext _context;
        public GetRoleLogin(MEMO_JOBContext context)
        {
            _context = context;
        }
    }
}
