using System;

namespace aks_demo.Shared
{
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


    public class AppSettings
    {
        public string CacheConnection { get; set; }
        public string StateServiceBaseURL { get; set; }
        public string DataServiceBaseURL { get; set; }
    }
}