using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;


namespace TrivialService.Controllers
{
    [ApiController]
    public class StateController : ControllerBase
    {
        ConnectionMultiplexer Connection {get {return lazyConnection.Value;}}
        [Route("[controller]")]
        [HttpGet]
        public IEnumerable<AuthorNote> Get()
        {
            IDatabase cache = Connection.GetDatabase();
/*
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Foo
            {
                Bar = cache.Execute("PING").ToString()
            })
            .ToArray();
            */
            return null;
        }
        public IEnumerable<AuthorNote> Get(string authorName, TimeSpan dateRange, DateTime createDate)
        {
            return null;
        }
        public AuthorNote Get(int noteId)
        {
            return null;
        }

        public void Set(AuthorNote note)
        {

        }
        public void Update (AuthorNote note, int noteId)
        {
            
        }

        private Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = Environment.GetEnvironmentVariable("CacheConnection").ToString();
            return ConnectionMultiplexer.Connect(cacheConnection);
        });
    }

    public class AuthorNote
    {
        public Person Author {get;set;}
        public int NoteId {get;set;}
        public DateTime CreateDate {get;set;}
        public DateTime ModifyDate {get;set;}
        public string NoteContent {get;set;}
    
    }
    public class Person
    {
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public string EmailAddress {get;set;}
    }
}