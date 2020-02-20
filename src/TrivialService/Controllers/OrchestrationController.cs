using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using TrivialService;


namespace TrivialService.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Produces(MediaTypeNames.Application.Json)]
    public class OrchestrationController : ControllerBase
    {
        private AppSettings _settings;

        public OrchestrationController(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<AuthorNote>> GetById(int id)
        {
            var note = await GetNote(id);

            return Ok(note);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<AuthorNote>> CreateNote([FromBody]AuthorNote note)
        {
            //Create new note attached to current user
            return Ok();
            // return CreatedAtAction(nameof(GetById), new { id = note.NoteId, note });
        }


        private async Task<AuthorNote> GetNote(int id)
        {

            //Get note from cache
            var theNote = await GetNoteFromCache(id);

            //if note is null then get note from db and store in cache
            if (theNote is null)
            {
                // theNote = GetNoteFromDB(id);
                if (theNote is null)
                {
                    //return null?  Throw a 404?
                }
                else
                {
                    // AddNoteToCache(theNote);
                }
            }
            //return note
            return theNote;
        }

        private async Task<AuthorNote> GetNoteFromCache(int id)
        {

            using (var client = new HttpClient())
            {
                //var response = await client.GetAsync(new Uri(_clientBaseUrl + userId + "?requestID=" + requestId + "&passwordHash=" + hmacString));
                var response = await client.GetAsync(new Uri(_settings.StateServiceBaseURL + '/' + id));
                

                // Raise exception if retrieval failed
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    return null;
                }

                // On success, return AuthorNote results from the server response packet
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AuthorNote>(responseContent);
            };

        }
    }
}