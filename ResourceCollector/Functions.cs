using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Pixel.Common.Cloud;

namespace ResourceCollector
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called addresources.
        public static void ProcessQueueMessage([QueueTrigger("addresources")] AddResourceInfo message, TextWriter log)
        {
            log.WriteLine($"add {message.Quantity} of {message.Type} to territory: {message.TerritoryId}");
            Console.WriteLine($"add {message.Quantity} of {message.Type} to territory: {message.TerritoryId}");
        }
    }
}
