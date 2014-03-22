using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Tavis.Hal.Tests
{
    public class JsonReaderTests
    {
        [Fact]
        public void LoadEmptyJson()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(),"Json.Empty.json");
            var resource = new JsonHalReader().Load(stream);

            Assert.NotNull(resource);
            Assert.Equal(0, resource.Contents.Count());
        }

        [Fact]
        public void LoadSimpleJson()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Json.Simple.json");
            var resource = new JsonHalReader().Load(stream);

            Assert.NotNull(resource);
            Assert.Equal(1, resource.Contents.Count());
            Assert.Equal("world", resource.FindProperty("hello").GetValue());
        }

        [Fact]
        public void LoadLinksJson()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Json.Links.json");
            var resource = new JsonHalReader().Load(stream);

            Assert.NotNull(resource);
            Assert.Equal(2, resource.Contents.Count());
            Assert.Equal("http://example.org/about", resource.FindLink("about").Target.OriginalString);
            Assert.Equal("http://example.org/next", resource.FindLink("next").Target.OriginalString);
        }

        [Fact]
        public void LoadEmbeddedJson()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Json.Embedded.json");
            var resource = new JsonHalReader().Load(stream);

            Assert.NotNull(resource);
            Assert.Equal(2, resource.Contents.Count());
            var resource1 = resource.FindResource(Uri.EscapeDataString("http://example.org/rels/order"));
            Assert.NotNull(resource1);
            Assert.Equal("1001",resource1.FindProperty("code").GetValue());
            var resource2 = resource.FindResource("http://example.org/rels/customer");
            Assert.Equal("Acme Inc.", resource2.FindProperty("name").GetValue());
            Assert.NotNull(resource2);
        }
        [Fact]
        public void LoadEmbeddedArrayJson()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Json.EmbeddedArray.json");
            var resource = new JsonHalReader().Load(stream);

            Assert.NotNull(resource);
            Assert.Equal(2, resource.Contents.Count());
            var resource1 = resource.FindResources("http://example.org/rels/order");
            Assert.NotNull(resource1);
           
        }
    }
}
