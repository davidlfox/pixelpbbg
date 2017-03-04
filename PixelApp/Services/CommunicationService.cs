using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Services
{
    public class CommunicationService
    {
        public static Note CreateNotification(string userId, string title, string body)
        {
            var note = new Note
            {
                Body = body,
                IsActive = true,
                Sent = DateTime.Now,
                Title = title,
                UserId = userId,
            };

            return note;
        }
    }
}