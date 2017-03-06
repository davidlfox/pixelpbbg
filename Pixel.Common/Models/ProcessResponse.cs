using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pixel.Common.Models
{
    public class ProcessResponse
    {
        public ProcessResponse()
        {
            Messages = new Dictionary<string, object>();
        }

        public ProcessResponse(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Messages = new Dictionary<string, object>() { { "Message", message } };
        }

        public ProcessResponse(bool isSuccessful, Dictionary<string, object> messages)
        {
            IsSuccessful = isSuccessful;
            Messages = messages;
        }

        // Constructor shortcut when you only need to add one "message" key value pair
        public ProcessResponse(bool isSuccessful, string key, object value)
        {
            IsSuccessful = isSuccessful;
            Messages = new Dictionary<string, object>() { { key, value } };
        }

        /// <summary>
        /// Was the process successful?
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Any messages returned by the process
        /// </summary>
        public Dictionary<string, object> Messages { get; set; }

        public List<string> GetFlattenedMessages()
        {
            var flattenedMessages = FlattenMessages(Messages);
            return flattenedMessages.Select(x => string.Format("{0}, {1}", x.Item1, x.Item2)).ToList();
        }

        public IEnumerable<Tuple<string, string>> FlattenMessages(IDictionary dict)
        {
            foreach (DictionaryEntry kvp in dict)
            {
                var childDictionary = kvp.Value as IDictionary;
                if (childDictionary != null)
                {
                    foreach (var tuple in FlattenMessages(childDictionary))
                        yield return tuple;
                }
                else
                {
                    var type = kvp.Value.GetType();
                    if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime))
                        yield return Tuple.Create(kvp.Key.ToString(), kvp.Value.ToString());
                }
            }
        }
    }
}
