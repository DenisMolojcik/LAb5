using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MedicalInstitution.Infrastructure
{
    public static class SessionExtensions
    {      
        public static void SetList<T>(this ISession session, string key, IEnumerable<T> value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
