﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    // TODO 4: unauthorized users should receive 401 status code
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAccountCache _accountCache;

        public AccountController(IAccountService accountService,IAccountCache accountCache)
        {
            _accountService = accountService;
            _accountCache = accountCache;
        }
        
        // TODO 3: Get user id from cookie 
        [Authorize] 
        [HttpGet]
        public ValueTask<Account> Get()
        {
            var cookies = HttpContext.Request.Cookies;

            cookies.TryGetValue("ExternalId", out var externalId);
            return _accountService.LoadOrCreateAsync(externalId);
            
        }

        //TODO 5: Endpoint should works only for users with "Admin" Role
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public Account GetByInternalId([FromRoute] int id)
        {
            return _accountService.GetFromCache(id);
        }

        [Authorize]
        [HttpPost("counter")]
        public async Task UpdateAccount()
        {
            //Update account in cache, don't bother saving to DB, this is not an objective of this task.
            var account = await Get();
            account.Counter++;
            _accountCache.AddOrUpdate(account);
        }
    }
}