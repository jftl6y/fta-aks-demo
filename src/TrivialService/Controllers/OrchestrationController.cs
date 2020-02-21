using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
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
            ActionResult<AuthorNote> note = await GetNote(id);

            return note;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<AuthorNote>> CreateNote([FromBody]AuthorNote note)
        {
            //Submit note to the DB and cache
            await SaveNoteToDB(note);
            await SaveNoteToCache(note);

            return CreatedAtAction(nameof(GetById), new { id = note.NoteId }, note);
        }


        private async Task<ActionResult<AuthorNote>> GetNote(int id)
        {

            //Get note from cache
            var theNote = (await GetNoteFromCache(id)).Value;

            //if note is null then get note from db and store in cache
            if (theNote is null)
            {
                theNote = (await GetNoteFromDB(id)).Value;
                if (theNote is null)
                {
                    return NotFound();
                }
                else
                {
                    // AddNoteToCache(theNote);
                }
            }
            //return note
            return Ok(theNote);
        }

        private async Task<ActionResult<AuthorNote>> GetNoteFromCache(int id)
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
                    return NotFound();
                }

                // On success, return AuthorNote results from the server response packet
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AuthorNote>(responseContent);
            };
        }
        private async Task<ActionResult<int>> SaveNoteToCache(AuthorNote note)
        {
            int returned;

            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(note), Encoding.UTF8, "application/json");

                using (var response = await client.PostAsync(_settings.StateServiceBaseURL, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    int.TryParse(apiResponse, out returned);
                }
                // On success, return AuthorNote id results from the server response packet
                return Ok(returned);
            };
        }



        private async Task<ActionResult<AuthorNote>> GetNoteFromDB(int id)
        {
            using (var client = new HttpClient())
            {
                //var response = await client.GetAsync(new Uri(_clientBaseUrl + userId + "?requestID=" + requestId + "&passwordHash=" + hmacString));
                var response = await client.GetAsync(new Uri(_settings.DataServiceBaseURL + '/' + id));

                // Raise exception if retrieval failed
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    return NotFound();
                }

                // On success, return AuthorNote results from the server response packet
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AuthorNote>(responseContent);
            };
        }

        private async Task<ActionResult<int>> SaveNoteToDB(AuthorNote note)
        {
            int returned;

            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(note), Encoding.UTF8, "application/json");

                using (var response = await client.PostAsync(_settings.DataServiceBaseURL, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    int.TryParse(apiResponse, out returned);
                }
                // On success, return AuthorNote id results from the server response packet
                return Ok(returned);
            };



        }

    }
}