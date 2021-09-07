using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using amplitude.Services;
using CloudNative.CloudEvents;

namespace amplitude.Controllers
{
    [ApiController]
    [Route("/")]
    public class AmplitudeController : ControllerBase
    {
        private readonly ILogger Logger;
        private IEventForwarderQueue Queue;

        public AmplitudeController(ILogger logger, 
                                IEventForwarderQueue queue)
        {
            Logger = logger;
            Queue = queue;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CloudEvent cloudEvent)
        {

            try
            {
                await Queue.QueueEventAsync(cloudEvent);
                return Ok();
            } 
            catch (Exception) 
            {
                return BadRequest();
            }
        }
    }
}