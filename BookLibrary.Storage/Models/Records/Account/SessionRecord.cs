using System;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Storage.Models.Records.Account
{
    public class SessionRecord
    {
        [Key]
        public int ID { get; set; }
        public int AccountId { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? LastRenewalDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string SessionId { get; set; }
    }
}
