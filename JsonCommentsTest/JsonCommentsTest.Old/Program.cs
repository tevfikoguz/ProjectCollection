﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace JsonCommentsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = GetTestJson();

            //// JsonConvert.DeserializeObject - JToken
            //var obj = JsonConvert.DeserializeObject(json, JsonExtensions.ObjectSerializationSettings);
            //Console.WriteLine(obj);

            //// JsonConvert.DeserializeObject - Template
            //var template = JsonConvert.DeserializeObject<Template>(json, JsonExtensions.ObjectSerializationSettings);
            //Console.WriteLine(template);

            //// JToken.Parse
            //var token = JToken.Parse(json);
            //Console.WriteLine(token);

            //// FromJson - JToken
            //var token1 = json.FromJson<JToken>();
            //Console.WriteLine(token1);

            //// FromJson - Template
            //var template1 = json.FromJson<Template>();
            //Console.WriteLine(template1);

            // HttpContent.Read
            var content = new StringContent(json, Encoding.Unicode, "application/json");
            var template2 = content.ReadAsAsync<Template>(JsonExtensions.JsonMediaTypeFormatters).Result;
            Console.WriteLine(template2);

            Console.ReadLine();
        }

        static string GetTestJson()
        {
            return File.ReadAllText("invalid2.json");
        }
    }

    public static class JsonExtensions
    {
        /// <summary>
        /// The max depth for serialization.
        /// </summary>
        public const int JsonSerializationMaxDepth = 512;

        /// <summary>
        /// The JSON object serialization settings.
        /// </summary>
        public static readonly JsonSerializerSettings ObjectSerializationSettings = new JsonSerializerSettings
        {
            MaxDepth = JsonExtensions.JsonSerializationMaxDepth,
            TypeNameHandling = TypeNameHandling.None,

            DateParseHandling = DateParseHandling.None,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,

            NullValueHandling = NullValueHandling.Ignore,

            ContractResolver = new CamelCasePropertyNamesWithOverridesContractResolver(),

            Converters = new List<JsonConverter>
            {
                new LineInfoConverter(),
                new TimeSpanConverter(),
                new StringEnumConverter { CamelCaseText = false },
                new AdjustToUniversalIsoDateTimeConverter(),
            },
        };

        /// <summary>
        /// The JSON media type formatter settings.
        /// </summary>
        public static readonly JsonSerializerSettings MediaTypeFormatterSettings = new JsonSerializerSettings
        {
            MaxDepth = JsonExtensions.JsonSerializationMaxDepth,
            TypeNameHandling = TypeNameHandling.None,

            DateParseHandling = DateParseHandling.None,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,

            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error,

            ContractResolver = new CamelCasePropertyNamesWithOverridesContractResolver(),

            Converters = new List<JsonConverter>
            {
                new LineInfoConverter(),
                new TimeSpanConverter(),
                new StringEnumConverter { CamelCaseText = false },
                new AdjustToUniversalIsoDateTimeConverter(),
            },
        };

        /// <summary>
        /// The JSON object type serializer.
        /// </summary>
        public static readonly JsonSerializer JsonObjectTypeSerializer = JsonSerializer.Create(ObjectSerializationSettings);

        /// <summary>
        /// Gets the JSON media type formatter.
        /// </summary>
        public static readonly MediaTypeFormatter JsonMediaTypeFormatter = new JsonMediaTypeFormatter { SerializerSettings = JsonExtensions.MediaTypeFormatterSettings, UseDataContractJsonSerializer = false };

        /// <summary>
        /// Gets the JSON media type formatters.
        /// </summary>
        public static readonly MediaTypeFormatter[] JsonMediaTypeFormatters = new MediaTypeFormatter[] { JsonExtensions.JsonMediaTypeFormatter };

        /// <summary>
        /// Deserialize object directly from JToken.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="jtoken">The JToken to be deserialized.</param>
        public static T FromJToken<T>(this JToken jtoken)
        {
            return jtoken.ToObject<T>(JsonExtensions.JsonObjectTypeSerializer);
        }

        /// <summary>
        /// Deserialize object from the JSON.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="json">JSON representation of object</param>
        public static T FromJson<T>(this string json)
        {
            using (var stringReader = new StringReader(json))
            using (var jsonTextReader = new JsonTextReader(stringReader))
            {
                return (T)JsonExtensions.JsonObjectTypeSerializer.Deserialize(jsonTextReader, typeof(T));
            }
        }

        /// <summary>
        /// Serialize object to JToken.
        /// </summary>
        /// <param name="value">The object.</param>
        public static JToken ToJToken(this object value)
        {
            return JToken.FromObject(value, JsonExtensions.JsonObjectTypeSerializer);
        }

        /// <summary>
        /// Serialize object to the JSON.
        /// </summary>
        /// <param name="value">The object.</param>
        public static string ToJson(this object value)
        {
            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = JsonExtensions.JsonObjectTypeSerializer.Formatting;

                    JsonExtensions.JsonObjectTypeSerializer.Serialize(jsonTextWriter, value);
                }

                return stringWriter.ToString();
            }
        }
    }
}
