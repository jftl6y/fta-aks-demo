using aks_demo.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aks_demo.DataService
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IConfiguration _config;
        public DataController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<AuthorNote>> GetById(int id)
        {
            AuthorNote note = new AuthorNote();
            var result = await ExecuteQueryReturnJson($"exec spGetAuthorNote @noteId={id}");
            if (!string.IsNullOrEmpty(result))
            {
                note.Author = new Person();
                var ret = JArray.Parse(result);
                if (ret.Count > 0)
                { 
                    note.Author.LastName = (string)ret[0]["lastName"];
                    note.Author.PersonId = (int)ret[0]["personId"];
                    note.Author.LastName = (string)ret[0]["lastName"];
                    note.Author.FirstName = (string)ret[0]["firstName"];
                    note.Author.EmailAddress = (string)ret[0]["emailAddress"];
                    note.CreateDate = DateTime.Parse((string)ret[0]["createDate"]);
                    note.ModifyDate = DateTime.Parse((string)ret[0]["modifyDate"]);
                    note.NoteContent = (string)ret[0]["noteContent"];
                    if (int.TryParse((string)ret[0]["noteId"], out int noteId))
                        note.NoteId = noteId;
                    return Ok(note); 
                }
                return NotFound();
            }
            return NotFound();
        }
        private string SqlParamBuilder(string input)
        {
            if (int.TryParse(input, out int intValue) && intValue == 0)
                return "null";
            if (string.IsNullOrEmpty(input))
                return "null";
            return $"[{input}]";
        }
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> CreateNote([FromBody]AuthorNote note)
        {
            string sqlCommandText = $"exec spUpsertAuthorNote @noteId={SqlParamBuilder(note.NoteId.ToString())}, @authorId={SqlParamBuilder(note.Author.PersonId.ToString())}, @lastName={SqlParamBuilder(note.Author.LastName)}, @firstName={SqlParamBuilder(note.Author.FirstName)}, @emailAddress={SqlParamBuilder(note.Author.EmailAddress)}, @noteContent={SqlParamBuilder(note.NoteContent)}";
            var ret = await ExecuteNonQuery(sqlCommandText);

            return Ok();
        }
        
        private async Task<string> ExecuteQueryReturnJson(string sqlCommandText)
        {
            string ConnectionString = _config["Values:sqlConxString"]; 
            try 
            { 
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using(var command = conn.CreateCommand())
                {
                        conn.AccessToken = await new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/");
                        command.CommandText = sqlCommandText;
                        conn.Open();
                        var reader = await command.ExecuteReaderAsync();
                        var results = new List<Dictionary<string, object>>();
                        var cols = new List<string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                            cols.Add(reader.GetName(i));

                        while (reader.Read())
                            results.Add(SerializeRow(cols, reader));
                        string ret = JsonConvert.SerializeObject(results, Formatting.Indented);
                        conn.Close();
                        return ret;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }
        private async Task<int> ExecuteNonQuery(string sqlCommandText)
        {
            string ConnectionString = _config["Values:sqlConxString"];
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (var command = conn.CreateCommand())
                {
                    conn.AccessToken = await new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/");
                    command.CommandText = sqlCommandText;
                    conn.Open();
                    var ret = await command.ExecuteNonQueryAsync();
                   
                    conn.Close();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }
    }
}