using System.Collections.Generic;

namespace BookLibrary.Storage.Models.Book
{
    public class BookTrackList
    {
        public int? BookId { get; set; }
        public string BookName { get; set; }
        public bool? BookAvailability { get; set; }
        public bool CanBePut { get; set; }
        public List<BookTrack> TracksList { get; set; } = [];
    }
}
