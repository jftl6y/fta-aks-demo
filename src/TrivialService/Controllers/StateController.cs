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


namespace TrivialService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StateController : ControllerBase
    {
        ConnectionMultiplexer Connection { get { return lazyConnection.Value; } }
        [HttpGet]
        public IEnumerable<AuthorNote> Get()
        {
            //todo 
        //    IDatabase cache = Connection.GetDatabase();
        
            var rng = new Random();
            return Enumerable.Range(1, rng.Next(1, 10)).Select(index => CreateDemoNote(rng.Next(1, 10))
            )
            .ToArray();

        }
        [Route("{noteId}")]
        [HttpGet]
        public AuthorNote Get(int noteId)
        {
            IDatabase cache = Connection.GetDatabase();
            var noteJson = cache.StringGet(noteId.ToString(),CommandFlags.None).ToString();
            if (!String.IsNullOrEmpty(noteJson))
                return JsonConvert.DeserializeObject<AuthorNote>(noteJson);
            return new AuthorNote();
        }

        [HttpPost]
        public void Set(AuthorNote note)
        {
            IDatabase cache = Connection.GetDatabase();
            cache.StringSet(note.NoteId.ToString(), JsonConvert.SerializeObject(note));
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
            cache.KeyDelete(noteId.ToString(),CommandFlags.None);
        }
        private Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string redisConxString = "fta-aks-demo.redis.cache.windows.net:6380,password=qh2+NOcmSKX22kDRv+ta37Vx9iXfJjdaEkMYUQTVgQY=,ssl=True,abortConnect=False";
            string cacheConnection = redisConxString;

            return ConnectionMultiplexer.Connect(cacheConnection);
        });
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