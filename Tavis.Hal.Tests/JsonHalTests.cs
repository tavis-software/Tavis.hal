using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Tavis.IANA;
using Xunit;

namespace Tavis.Hal.Tests
{
    public class JsonHalTests
    {

        [Fact]
        public void CreateHalManually()
        {
            var hal = new HalDocument(new SelfLink() { Target = new Uri("http://example.org") });
            hal.Contents.Add("foo", new HalResource(
                "foo",
                "http://example.org/foo"
            ));
            hal.Contents.Add("bar", new HalLink()
            {
                Rel = "bar",
                Href = "http://example.org/bar"
            });


            var jsonhalWriter = new JsonHalWriter();
            var stream = jsonhalWriter.ToStream(hal);
            var sr = new StreamReader(stream);
            var text = sr.ReadToEnd();
            Assert.NotNull(hal);
        }


        [Fact]
        public void CreateHalUsingConstructor()
        {
            var hal = new HalDocument(new SelfLink(){ Target = new Uri("http://example.org")},
                new HalResource(new Link() { Relation = "foo", Target = new Uri("http://example.org/foo") }),
                new HalLink(new AboutLink(){Target = new Uri("http://example.org/bar")})
            );
           

            var jsonhalWriter = new JsonHalWriter();
            var stream = jsonhalWriter.ToStream(hal);
            var sr = new StreamReader(stream);
            var text = sr.ReadToEnd();
            Assert.NotNull(hal);
        }


        [Fact]
        public void CreateSampleHalFromSpec()
        {
            var hal = new HalDocument(new SelfLink() { Target = new Uri("http://example.org") });
            hal.AddNamespace("td", new Uri("http://mytodoapp.com/rels/"));

            hal.AddLink("td:search", "/todo-list/search;{searchterm}");
            hal.AddLink("td:description", "/todo-list/description");
            hal.AddJProperty("created_at", "2010-01-16");
            hal.AddJProperty("updated_at", "2010-02-21");
            hal.AddJProperty("summary", "An example list");

            hal.CreateResource("td:owner", "http://range14sux.com/mike")
                    .AddJProperty("name", "Mike")
                    .AddJProperty("age", "36")
                    .AddLink("td:friend", "http://mamund.com/")
                .End();

            hal.CreateResource("td:item", "http://home.com/tasks/126")
                .AddJProperty("title", "Find Mug")
                .AddJProperty("details", "Find my mug.")

                
                .AddLink("td:next", "http://work.com/todos/make-some-tea")
            .End();

            hal.CreateResource("td:item", "http://work.com/todos/make-some-tea")
                .AddJProperty("title", "Make tea")
                .AddJProperty("details", "Mike nice tea that is green. (Gyokuro)")
                .AddLink("td:prev", "http://home.com/tasks/126")
            .End();


            var stringHal = new StreamReader(new JsonHalWriter().ToStream(hal)).ReadToEnd();
            Assert.NotNull(hal);
            Assert.NotNull(stringHal);
        }
    }
}
