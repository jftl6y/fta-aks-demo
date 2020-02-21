using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TrivialService;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Options;

namespace TrivialService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StateController : ControllerBase
    {
        private AppSettings _settings;
        private static string _cacheConn;
        ConnectionMultiplexer Connection { get { return lazyConnection.Value; } }
        private Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(_cacheConn);
        });

        public StateController(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;

            _cacheConn = _settings.CacheConnection;
            //lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            // {
            //     string cacheConnection = _settings.CacheConnection;
            //     return ConnectionMultiplexer.Connect(cacheConnection);
            // });
        }


 
        
        [Route("{noteId}")]
        [HttpGet]
        public ActionResult<AuthorNote> Get(int noteId)
        {
            IDatabase cache = Connection.GetDatabase();
            if (cache.KeyExists(noteId.ToString()))
            {
                var noteJson = cache.StringGet(noteId.ToString(), CommandFlags.None).ToString();
                return Ok(JsonConvert.DeserializeObject<AuthorNote>(noteJson));
            }
            else
            {
                return NotFound($"Note with id {noteId} wasn't found.");
            }
        }

        [HttpGet]
        [Route("load")]
        public ActionResult<IEnumerable<AuthorNote>> Load()
        {
            List<AuthorNote> notes = new List<AuthorNote>();

            //Loads the cache with dummy records
            for (int i = 0; i < 20; i++)
            {
                AuthorNote curNote = CreateDemoNote(i + 1);
                notes.Add(curNote);
                Set(curNote);
            }
            return notes;
        }

        [HttpGet]
        [Route("clean")]
        public ActionResult Clean()
        {
            List<AuthorNote> notes = new List<AuthorNote>();

            //Loads the cache with dummy records
            for (int i = 0; i < 200; i++)
            {
                Invalidate(i);
            }
            return Ok();
        }

        [HttpPost]
        public ActionResult<int> Set(AuthorNote note)
        {
            IDatabase cache = Connection.GetDatabase();
            cache.StringSet(note.NoteId.ToString(), JsonConvert.SerializeObject(note));
            return Ok(note.NoteId);
        }

        [HttpPatch]
        public void Update(AuthorNote note)
        {
            Set(note);
        }

        [Route("{noteId}")]
        [HttpDelete]
        public void Invalidate(int noteId)
        {
            IDatabase cache = Connection.GetDatabase();
            cache.KeyDelete(noteId.ToString(), CommandFlags.None);
        }





        private AuthorNote CreateDemoNote(int noteId)
        {
            return new AuthorNote
            {
                Author = CreateDemoPerson(),
                NoteContent = "Hiya",
                NoteId = noteId,
                CreateDate = DateTime.UtcNow,
                ModifyDate = DateTime.UtcNow
            };
        }
        private Person CreateDemoPerson()
        {
            string lastName = "Smith";
            string firstName = "Isaac";
            return new Person
            {
                LastName = lastName,
                FirstName = firstName,
                EmailAddress = $"{firstName}.{lastName}@domain.com"
            };
        }
    }


}