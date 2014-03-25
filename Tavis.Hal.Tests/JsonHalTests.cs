using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Tavis.IANA;
using Xunit;

namespace Tavis.Hal.Tests
{
    public class JsonHalTests
    {

        [Fact]
        public void EmptyHalDocument()
        {
            var hal = new HalResource();

            
            var output = HalToString(hal);
            Assert.Equal(output,"{}");
        }

        [Fact]
        public void HalWithSelfLink()
        {
            var hal = new HalResource(new SelfLink(){Target = new Uri("http://example.org/foo")});

            var output = JToken.Parse(HalToString(hal));
            Assert.True(JToken.DeepEquals(output, JToken.Parse(@"{ '_links' : { 'self' : { 'href': 'http://example.org/foo'}}}")));
        }



        private static string HalToString(HalResource hal)
        {
            var jsonhalWriter = new JsonHalWriter();
            var stream = jsonhalWriter.ToStream(hal);
            var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }


        [Fact]
        public void CreateHalManually()
        {
            var hal = new HalResource(new SelfLink() { Target = new Uri("http://example.org") },
                            new HalResource(new Link() { Relation="foo", Target = new Uri("http://example.org/foo") }),
                            new Link()
                            {
                                Relation = "bar",
                                Target = new Uri("http://example.org/bar")
                            }
                );

            var jsonhalWriter = new JsonHalWriter();
            var stream = jsonhalWriter.ToStream(hal);
            var sr = new StreamReader(stream);
            var text = sr.ReadToEnd();
            Assert.NotNull(hal);
        }


        [Fact]
        public void CreateHalUsingConstructor()
        {
            var hal = new HalResource(new SelfLink() { Target = new Uri("http://example.org") },
                new HalResource(new Link() { Relation = "foo", Target = new Uri("http://example.org/foo") }),
                new AboutLink(){Target = new Uri("http://example.org/bar")}
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
          

            var ownerResource = new HalResource(new Link() { Relation = "td:owner", Target = new Uri("http://range14sux.com/mike") }, new object[]
            {
                new HalProperty("name", "Mike"),
                new HalProperty("age", "36"),
                new Link() {Relation = "td:friend", Target = new Uri("http://mamund.com/")},
            });



            var item1Resource =
                new HalResource(new Link() {Relation = "td:item", Target = new Uri("http://home.com/tasks/126")},
                    new object[]
                    {
                        new HalProperty("title", "Find Mug"),
                        new HalProperty("details", "Find my mug."),
                    });



            var item2Resource =
                new HalResource(
                    new Link() {Relation = "td:item", Target = new Uri("http://work.com/todos/make-some-tea")},
                    new object[]
                    {
                         new HalProperty("title", "Make tea"),
                          new HalProperty("details", "Mike nice tea that is green. (Gyokuro)"),
                          new Link() {Relation = "td:prev", Target = new Uri("http://home.com/tasks/126")}
                    });

            var hal = new HalResource(new SelfLink() { Target = new Uri("http://example.org") }, new object[]
            {
                new Link() {Relation = "td:search", Target = new Uri("/todo-list/search;{searchterm}", UriKind.Relative)},
                new Link() {Relation = "td:description", Target = new Uri("/todo-list/description",UriKind.Relative)},
                new HalProperty("created_at", "2010-01-16"),
                new HalProperty("updated_at", "2010-02-21"),
                new HalProperty("summary", "An example list"),
                ownerResource,
                item1Resource,
                item2Resource
            });

            hal.AddNamespace("td", new Uri("http://mytodoapp.com/rels/"));


            var stringHal = new StreamReader(new JsonHalWriter().ToStream(hal)).ReadToEnd();
            Assert.NotNull(hal);
            Assert.NotNull(stringHal);
        }
    }
}
