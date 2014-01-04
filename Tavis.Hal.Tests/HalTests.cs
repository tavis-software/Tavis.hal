using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tavis.IANA;
using Xunit;

namespace Tavis.Hal.Tests
{
    public class HalTests
    {

        [Fact]
        public void Create_simplest_hal_document_possible()
        {
            var hal = new HalResource();
            Assert.NotNull(hal);
        }

        [Fact]
        public void Create_a_hal_resource_with_self_link()
        {
            var hal = new HalResource(new SelfLink(){Target = new Uri("http://example.org/foo")});
            Assert.NotNull(hal);
        }

        [Fact]
        public void Create_a_hal_resource_with_many_links()
        {
            var hal = new HalResource(null, 
                new AboutLink() { Target = new Uri("http://example.org/about") },
                new LicenseLink() { Target = new Uri("http://example.org/license") });

            Assert.NotNull(hal);

            Assert.IsType(typeof (AboutLink), hal.FindLink("about"));
            Assert.IsType(typeof(LicenseLink), hal.FindLink("license"));
        }


        [Fact]
        public void Create_a_hal_resource_with_embedded_resource()
        {
            var hal = new HalResource(null, new HalResource(new AboutLink()));

            Assert.NotNull(hal);

            var childResource = hal.FindResource("about");
            Assert.IsType(typeof(HalResource), childResource);

         
        }

        [Fact]
        public void Create_a_hal_resource_with_embedded_resource_using_constructor()
        {
            var hal = new HalResource(new SelfLink(), new HalResource(new AboutLink()));

  
            var childResource = hal.GetNode("about");
            Assert.IsType(typeof(HalResource), childResource);
        }

        [Fact]
        public void Create_a_hal_resource_with_using_constructor_list()
        {
            var hal = new HalResource(new SelfLink(), new HalResource(new AboutLink() {Target = new Uri("http://example.org/about")}), 
                new LicenseLink() {Target = new Uri("http://example.org/license")},
                new NextLink() { Target = new Uri("http://example.org/next") });


            var childResource = hal.FindResource("about");
            Assert.IsType(typeof(HalResource), childResource);

            var licenseLink = hal.FindLink("license");
            Assert.IsType(typeof(LicenseLink), licenseLink);

            var nextLink = hal.FindLink("next");
            Assert.IsType(typeof(NextLink), nextLink);

        }

        [Fact]
        public void CreateHalManually()
		{
            var hal = new HalResource(new SelfLink() { Target = new Uri("http://example.org") },
			new HalResource(new LicenseLink() {Target = new Uri("http://example.org/license")}),
            new HalResource(new Link()
            {
                Relation = "bar",
                Target = new Uri("http://example.org/bar")
            })
                
                );
            

			Assert.NotNull(hal);
		}


		[Fact]
		public void CreateHalManually2()
		{
            var hal = new HalResource(new SelfLink() { Target = new Uri("http://example.org") },
                new HalResource(new Link() {Relation = "foo", Target = new Uri( "http://example.org/foo")}),
                new Link() { Relation = "bar", Target = new Uri("http://example.org/bar")}
                );
            
         
			Assert.NotNull(hal);
		}


        [Fact]
        public void CreateHalManuallyUsingBuilder() {
            var hal = new HalResource(new SelfLink() { Target = new Uri("http://example.org") },
                new HalResource(new AlternateLink() { Target = new Uri("http://example.org/alternat") }),
                new HalResource(new DescribedByLink() {Target = new Uri("http://example.org/describedby")})
                );


            var xdoc = XDocument.Load(new XmlHalWriter().ToStream(hal));
            Assert.NotNull(xdoc);
            
        }

        [Fact]
        public void CreateSampleHalFromSpec() {
            var hal = new HalResource(new SelfLink() { Target = new Uri("http://example.org") }, new object[]
            {
                new Link() {Relation = "td:search", Target = new Uri( "/todo-list/search;{searchterm}", UriKind.Relative) },
                new Link() {Relation = "td:description", Target = new Uri( "/todo-list/description", UriKind.Relative) },
                new HalProperty("created_at", "2010-01-16"),
                new HalProperty("updated_at", "2010-02-21"),
                new HalProperty("summary", "An example list"),
                new HalResource(new Link() {Relation = "td:owner", Target = new Uri("http://range14sux.com/mike")}, new object[]
                {
                    new HalProperty("name", "Mike"),
                    new HalProperty("age", "36"),
                    new Link(){ Relation = "td:friend", Target=new Uri("http://mamund.com/")}
                }),
                new HalResource(new Link() {Relation = "td:item", Target = new Uri("http://home.com/tasks/126")}, new object[]
                {
                    new HalProperty("title", "Find Mug"),
                    new HalProperty("details", "Find my mug.")
                }),
               new HalResource(new Link() {Relation  = "td:attachment", Type = new MediaTypeWithQualityHeaderValue("text/plain")}, new object[]
               {
                   new HalTypedResourceContents(@"
            **********************************
                PLACES MY MUG COULD BE
            **********************************
              - Garden
              - Roof
              - Zipcar
              - Shelf
			") 
               }),
                new Link() {Relation = "td:next", Target = new Uri("http://work.com/todos/make-some-tea")},
                  new HalResource(new Link() {Relation = "td:item", Target = new Uri("http://work.com/todos/make-some-tea")}, new object[]
                {
                    new HalProperty("title", "Make tea"),
                    new HalProperty("details", "Mike nice tea that is green. (Gyokuro)."),
                    new Link(){ Relation = "td:prev", Target=new Uri("http://home.com/tasks/126")}
                }),

            });
           
            hal.AddNamespace("td", new Uri("http://mytodoapp.com/rels/"));

            var xmlHal = new StreamReader(new XmlHalWriter().ToStream(hal)).ReadToEnd();
            var jsonHal = new StreamReader(new JsonHalWriter().ToStream(hal)).ReadToEnd();
            Assert.NotNull(hal);
            Assert.NotNull(xmlHal);
            Assert.NotNull(jsonHal);
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


