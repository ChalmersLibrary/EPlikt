using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ComponentModel.DataAnnotations;
using EPlikt.Models;

namespace EPlikt.Controllers
{
    public class EPliktController : ApiController
    {
        /// <summary>
        /// Get the main feed.
        /// </summary>
        /// <returns>The feed.</returns>
        [HttpGet]
        public ResultResponse Feed()
        {
            var res = new ResultResponse();

            res.Success = true;
            res.Message = "TODO!";

            return res;
        }
    }
}
