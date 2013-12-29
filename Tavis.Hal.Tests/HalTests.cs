using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tavis.IANA;
using Tavis.UriTemplates;
using Xunit;

namespace Tavis.Hal.Tests
{
    public class HalTests
    {

        //public void Create_simplest_hal_document_possible()
        //{
        //    var hal = new HalDocument();
        //    hal.AddProperty("foo", "bar");

        //    Assert.NotNull(hal);
        //}

        [Fact]
        public void CreateHalManually()
		{
            var hal = new HalDocument(new SelfLink() { Target = new Uri("http://example.org") });
			hal.Root.Contents.Add("foo", new HalResource(
				"foo",
				"http://example.org/foo"
			));
            hal.Root.Contents.Add("bar", new HalLink()
            {
				Rel = "bar",
				Href = "http://example.org/bar"
			});

			Assert.NotNull(hal);
		}


		[Fact]
		public void CreateHalManually2()
		{
            var hal = new HalDocument(new SelfLink() { Target = new Uri("http://example.org") });
            hal.Root.Contents.Add("foo", new HalResource(
				"foo",
				"http://example.org/foo"
			));
            hal.Root.Contents.Add("bar", new HalLink
            {
				Rel = "bar",
				Href = "http://example.org/bar"
			});

			Assert.NotNull(hal);
		}


        [Fact]
        public void CreateHalManuallyUsingBuilder() {
            var hal = new HalDocument(new SelfLink() { Target = new Uri("http://example.org") });


            hal.Root.CreateResource("foo", "http://example.org/foo").End()
                .CreateResource("bar", "http://example.org/bar").End();
            


            var stringHal = new StreamReader(new XmlHalWriter().ToStream(hal)).ReadToEnd();
            Assert.NotNull(hal);
            Assert.NotNull(stringHal);
        }

        [Fact]
        public void CreateSampleHalFromSpec() {
            var haldoc = new HalDocument(new SelfLink() { Target = new Uri("http://example.org") });
            haldoc.AddNamespace("td", new Uri("http://mytodoapp.com/rels/"));
            var hal = haldoc.Root;

            hal.AddLink("td:search", "/todo-list/search;{searchterm}");
            hal.AddLink("td:description", "/todo-list/description");
            hal.AddXProperty("created_at", "2010-01-16");
            hal.AddXProperty("updated_at", "2010-02-21");
            hal.AddXProperty("summary", "An example list");

            hal.CreateResource("td:owner", "http://range14sux.com/mike")
                    .AddXProperty("name","Mike")
                    .AddXProperty("age","36")
                    .AddLink("td:friend", "http://mamund.com/")
                .End();

            hal.CreateResource("td:item", "http://home.com/tasks/126")
                .AddXProperty("title", "Find Mug")
                .AddXProperty("details", "Find my mug.")

                .AddTypedResource(new Link() {Relation  = "td:attachment", Type = new MediaTypeWithQualityHeaderValue("text/plain")}, @"
            **********************************
                PLACES MY MUG COULD BE
            **********************************
              - Garden
              - Roof
              - Zipcar
              - Shelf
			")
                .AddLink("td:next", "http://work.com/todos/make-some-tea")
            .End();

            hal.CreateResource("td:item", "http://work.com/todos/make-some-tea")
                .AddXProperty("title", "Make tea").AddXProperty("details", "Mike nice tea that is green. (Gyokuro)")
                .AddLink("td:prev", "http://home.com/tasks/126")
            .End();


            var stringHal = new StreamReader(new XmlHalWriter().ToStream(haldoc)).ReadToEnd();
            Assert.NotNull(hal);
            Assert.NotNull(stringHal);
        }


		[Fact]
		public void DeserializeHal()
		{
			var content = GetContent();

			var hal = new XmlHalReader().Load(content);

			Assert.NotNull(hal);
		}

		[Fact]
		public void SerializeHal()
		{
			//Arrange
			var content = GetContent();
			var hal = new XmlHalReader().Load(content);

			//Act

            var stream = new XmlHalWriter().ToStream(hal);

			//Assert
			var result = new StreamReader(stream);

			var resultstring = result.ReadToEnd();
		}


        [Fact]
        public void ReservedCharacterExpansion()
        {
            var link = new Link();
            link.Target = new Uri("http://foo.com/{?format}");

            link.SetParameter("format", "application/vnd.foo+xml");

            var result = link.CreateRequest();


            Assert.Equal("http://foo.com/?format=application%2Fvnd.foo%2Bxml", result.RequestUri.OriginalString);
           
        }


		public static string GetContent()
		{
			return @"<resource rel='self' href='/todo-list'>
    <link rel='search' href='/todo-list/search;{searchterm}' />
    <link rel='description' href='/todo-list/description' />
    <created_at>2010-01-16</created_at>
    <updated_at>2010-02-21</updated_at>
    <summary>An example list</summary>
    <resource rel='owner' href='http://range14sux.com/mike'>
        <name>Mike</name>
        <age>36</age>
        <link rel='friend' href='http://mamund.com/' />
    </resource>
    <resource rel='item' name='1' href='http://home.com/tasks/126'>
        <title>Find Mug</title>
        <content>Find my mug.</content>
        <link rel='next' href='http://work.com/todos/make-some-tea' />
    </resource>
    <resource rel='item' name='2' href='http://work.com/todos/make-some-tea'>
        <title>Make tea</title>
        <content>For make drinking goodly tea that is green.</content>
        <link rel='urn:tavis:prev' href='http://home.com/tasks/126' />
    </resource>
</resource>";
		}
	}

    }


