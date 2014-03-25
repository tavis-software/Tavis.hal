using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tavis
{
    public class JsonHalWriter : IHalWriter
    {
        public void CopyToStream(HalResource document, Stream stream)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(stringBuilder)) {Formatting = Formatting.Indented})
            {

                WriteResource(document, writer);
            }

            var sw = new StreamWriter(stream);
            sw.Write(stringBuilder.ToString());
            sw.Flush();
        }

        public Stream ToStream(HalResource document)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(stringBuilder)) { Formatting = Formatting.Indented })
            {
                WriteResource(document, writer);
            }

            var stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            sw.Write(stringBuilder.ToString());
            sw.Flush();
            stream.Position = 0;

            return stream;
        }





        internal static void WriteResource(HalResource halResource, JsonWriter writer)
        {
            writer.WriteStartObject();


            
            var namespaces = halResource.Namespaces;
            if ((namespaces != null) && (namespaces.Any()))
            {
                writer.WritePropertyName("_curies");
                writer.WriteStartObject();
                foreach (var @namespace in namespaces)
                {
                    writer.WritePropertyName(@namespace.Prefix);
                    writer.WriteValue(@namespace.Namespace.OriginalString);
                }
                writer.WriteEndObject();

            }

            var links = halResource.Contents.Where(h => h is HalLink) .Cast<HalLink>().ToList();
            if (halResource.Link != null) links.Insert(0, new HalLink(halResource.Link));
            if (links.Any() || halResource.Link != null)
            {
                writer.WritePropertyName("_links");
                writer.WriteStartObject();

                foreach (var halLink in links)
                {
                    WriteLink(halLink,writer);
                }
                writer.WriteEndObject();

            }

            var properties = halResource.Contents.Where(h => h is IHalProperty).Cast<IHalProperty>().ToList();
            if (properties.Any())
            {
                foreach (var halProperty in properties)
                {
                    WriteProperty(halProperty,writer);
                }
            }

            var resources = halResource.Contents.Where(h => h is HalResource).Cast<HalResource>().ToList();
            if (resources.Any())
            {
                writer.WritePropertyName("_embedded");
                writer.WriteStartObject();
                foreach (var halChildResource in resources)
                {
                    writer.WritePropertyName(halChildResource.Link.Relation);
                    WriteResource(halChildResource,writer);
                }
                writer.WriteEndObject();
            }
            

            writer.WriteEndObject();
        }

        private static void RenderSelfLink(HalResource halResource, JsonWriter writer)
        {
            if (halResource.Link.Target != null)
            {
                writer.WritePropertyName("href");
                writer.WriteValue(halResource.Link.Target.OriginalString);
            }

            if (!string.IsNullOrEmpty(halResource.Name))
            {
                writer.WritePropertyName("name");
                writer.WriteValue(halResource.Name);
            }
            if (halResource.Link.Type != null)
            {
                writer.WritePropertyName("type");
                writer.WriteValue(halResource.Link.Type.ToString());
            }
        }


        internal static void WriteLink(HalLink halLink, JsonWriter writer)
        {
            writer.WritePropertyName(halLink.Rel);
            writer.WriteStartObject();
            writer.WritePropertyName("href");
            writer.WriteValue(halLink.Href);
            writer.WriteEndObject();

        }

        internal static void WriteProperty(IHalProperty halProperty, JsonWriter writer)
        {
            var property = new JProperty(halProperty.Name,halProperty.GetValue());
            property.WriteTo(writer);
        }


    }
}
