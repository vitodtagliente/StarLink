using System;
using System.Collections.Generic;
using System.Text.Json;

namespace StarLink
{
    static class StarSerializer
    {
        public static string Serialize<T>(T data)
        {
            try
            {
                return JsonSerializer.Serialize(data);
            }
            catch
            {
                return null;
            }
        }

        public static T Deserialize<T>(string data)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions()
                {

                });
            }
            catch
            {
                return default(T);
            }
        }
    }
}
