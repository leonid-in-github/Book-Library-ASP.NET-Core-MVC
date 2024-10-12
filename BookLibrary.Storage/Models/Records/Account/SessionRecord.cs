using System;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Storage.Models.Records.Account
{
    public class SessionRecord
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? LastRenewalDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string SessionId { get; set; }
    }
}
