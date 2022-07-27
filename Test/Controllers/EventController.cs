using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.Controllers
{
    [Route("api/Events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private GraphServiceClient _client;
        private IConfiguration _config;
        private string _userId;

        public EventController(GraphServiceClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
            _userId = _config.GetValue("Credentials:UserId", "");
        }

        [HttpGet]
        [Route("List")]
        public async Task<IEnumerable<Event>> ListEvents()
        {
            List<Event> result = new List<Event>();

            try
            {
                var request = _client.Users[_userId].Events.Request();

                while(request != null)
                {
                    var data = await request.GetAsync();
                    result.AddRange(data.CurrentPage);

                    request = data.NextPageRequest;
                }

            }
            catch(Exception)
            {
                
            }

            return result;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<string> CreateEvent(Event @event)
        {
            try
            {
                var response = await _client.Users[_userId].Events.Request().AddAsync(@event);

                return $"Event added successfully! response = {JsonConvert.SerializeObject(response)}";
            }
            catch(Exception e)
            {
                return $"Error Creating an event. message - {e.Message}";
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<string> DeleteEvent(string id)
        {
            try
            {
                await _client.Users[_userId].Events[id].Request().DeleteAsync();

                return "Event Deleted Successfully!";
            }
            catch(Exception e)
            {
                return $"error Deleting the event. message - {e.Message}";
            }
        }

        [HttpPut]
        [Route("Update")]
        public async Task<string> UpdateEvent(Event @event)
        {
            try
            {
                var response = await _client.Users[_userId].Events[@event.Id].Request().UpdateAsync(@event);

                return $"Event Updated Successfully! response = {JsonConvert.SerializeObject(response)}";
            }
            catch (Exception e)
            {
                return $"error Deleting the event. message - {e.Message}";
            }
        }
    }
}
