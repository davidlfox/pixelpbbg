using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Mailbox.Models
{
    public class MailboxViewModel
    {
        public List<NoteSkinny> Notes { get; set; }

        public MailboxViewModel()
        {
            this.Notes = new List<NoteSkinny>();
        }
    }

    public class NoteSkinny
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Sent { get; set; }
        public bool IsRead { get; set; }
    }
}