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
        public void CopyToStream(HalDocument document, Stream stream)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(stringBuilder)))
            {
                WriteDocument(document, writer);
            }

            var sw = new StreamWriter(stream);
            sw.Write(stringBuilder.ToString());
            sw.Flush();
        }

        public Stream ToStream(HalDocument document)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(stringBuilder)))
            {
                WriteDocument(document, writer);
            }

            var stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            sw.Write(stringBuilder.ToString());
            sw.Flush();
            stream.Position = 0;

            return stream;
        }



        public static void WriteDocument(HalDocument halDocument, JsonWriter writer)
        {
            WriteResource(halDocument.Root, writer, halDocument.Namespaces);
        }

        internal static void WriteResource(HalResource halResource, JsonWriter writer, IEnumerable<HalNamespace> namespaces = null)
        {
            writer.WriteStartObject();

            if (halResource.Href != null)
            {
                writer.WritePropertyName("href");
                writer.WriteValue(halResource.Href);
            }

            if (!string.IsNullOrEmpty(halResource.Name))
            {
                writer.WritePropertyName("name");
                writer.WriteValue(halResource.Name);
            }
            if (!string.IsNullOrEmpty(halResource.Type))
            {
                writer.WritePropertyName("type");
                writer.WriteValue(halResource.Type);
            }

            if ((namespaces != null) && (namespaces.Any()))
            {
                writer.WritePropertyName("_curies");
                writer.WriteStartObject();
                foreach (var @namespace in namespaces)
                {
                    writer.WritePropertyName(@namespace.Prefix);
                    writer.WriteValue(@namespace.Namespace.AsString());
                }
                writer.WriteEndObject();

            }

            foreach (var halNode in halResource.Contents.Values)
            {
                if (halNode is HalLink)
                {
                    var halLink = (HalLink)halNode;
                    WriteDocument(halLink, writer);
                }
                else if (halNode is IHalProperty)
                {
                    var halProperty = (IHalProperty)halNode;
                    WriteDocument(halProperty, writer);
                }
                else if (halNode is HalResource)
                {
                    var halChildResource = (HalResource)halNode;
                    WriteDocument(halChildResource, writer);
                }
            }

            writer.WriteEndObject();
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
            var content = halProperty.GetContent() as JProperty;
            if (content != null)
            {
                content.WriteTo(writer);
            }


        }


    }
}
