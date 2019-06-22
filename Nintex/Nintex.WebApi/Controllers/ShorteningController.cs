using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nintex.Business;
using Nintex.Business.Service;
using Nintex.WebApi.Models;

namespace Nintex.WebApi.Controllers
{
    [Route("api/shortening")]
    [ApiController]
    public class ShorteningController : ControllerBase
    {
        IUrlShorteningService _urlShorteningService;
        public ShorteningController(IUrlShorteningService urlShorteningService)
        {
            _urlShorteningService = urlShorteningService;
        }

        [HttpPost("{data}")]
        public async Task<ObjectResult> Add([FromBody] UrlShorteningAddModel data)
        {
            return await Task.Run(() =>
            {
                var result = _urlShorteningService.Add(data.UserUrl, data.UserAlias);

                return Ok(result);
            });
        }

        //[HttpGet]
        //public async Task<ObjectResult> Exists(string key)
        //{
        //    return await Task.Run(() =>
        //    {
        //        var result = _urlShorteningService.Exists(key);

        //        return Ok(result);
        //    });
        //}

        [HttpGet("{key}")]
        public async Task<ObjectResult> Get(string key)
        {
            return await Task.Run(() =>
            {
                var result = _urlShorteningService.Get(key);

                return Ok(result);
            });
        }
    }
}