using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebApplication3.DbBase;
using WebApplication3.Helper;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [ApiController]
    [ApiVersion("1.2")]
    [Route("v{version:apiVersion}/[controller]")]
    public class LotteryUserController : ControllerBase
    {

        private readonly ILogger<LotteryUserController> _logger;
        private readonly LotteryDb _lotteryDb;

        public LotteryUserController(ILogger<LotteryUserController> logger, LotteryDb lotteryDb)
        {
            _logger = logger;
            _lotteryDb = lotteryDb;
        }

        [HttpGet]
        public IEnumerable<LotteryUsers> Get()
        {

            var lottery = _lotteryDb.LotteryUsers.Where(a => a.Acq == 1).ToList();
            return lottery;
        }
        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>      
        [HttpGet("{id:int}", Name = "GetUser")]
        public string GetUser(int id)
        {

            List<LotteryUsers> lottery = _lotteryDb.LotteryUsers.Where(a => a.UserId == id).ToList();
            var ss = new ReturnData<LotteryUsers>(200, "获取成功", lottery);
            return JsonConvert.SerializeObject(ss);
        }
        [HttpPost]
        [Route("PostUser")]
        public async Task<JsonResult> PostUser([FromBody] LotteryUsers[] lotteryUser)
        {
                _lotteryDb.LotteryUsers.AddRange(lotteryUser);
          
                var response = await _lotteryDb.SaveChangesAsync();
                var ss = new ReturnData<LotteryUsers>(200, "添加成功", null); 
                if (response > 0)
                {
                    ss = new ReturnData<LotteryUsers>(200, "添加成功", null);
                  
                }
                
                return new JsonResult(ss);
              
            }

        [HttpGet]
        [Route("FindErrro")]
       
        public void FindError()
        {
            throw  new Exception("this is error");
        }
        
        /// <summary>
        /// 删除i🍔🍔🍔🍔🍔
        /// </summary>
        /// <param name="id"></param>        
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _lotteryDb.LotteryUsers.Find(id);

            if (todo == null)
            {
                return NotFound();
            }

            _lotteryDb.LotteryUsers.Remove(todo);
            _lotteryDb.SaveChanges();

            return NoContent();
        }


    }

   
}