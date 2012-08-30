using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Hal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HalTests
{
	[TestClass]
	public class HalTests
	{
		[TestMethod]
		public void CreateHalManually()
		{
			var hal = new HalDocument("http://example.org");
			hal.Contents.Add("foo", new HalResource(
				"foo",
				"http://example.org/foo"
			));
			hal.Contents.Add("bar", new HalLink() {
				Rel = "bar",
				Href = "http://example.org/bar"
			});

			Assert.IsNotNull(hal);
		}


		[TestMethod]
		public void CreateHalManually2()
		{
			var hal = new HalDocument("http://example.org");
			hal.Contents.Add("foo", new HalResource(
				"foo",
				"http://example.org/foo"
			));
			hal.Contents.Add("bar", new HalLink {
				Rel = "bar",
				Href = "http://example.org/bar"
			});

			Assert.IsNotNull(hal);
		}


        [TestMethod]
        public void CreateHalManuallyUsingBuilder() {
            var hal = new HalDocument("http://example.org");


            hal.CreateResource("foo", "http://example.org/foo").End()
                .CreateResource("bar", "http://example.org/bar").End();
            

            var stringHal = new StreamReader(hal.ToStream()).ReadToEnd();
            Assert.IsNotNull(hal);
            Assert.IsNotNull(stringHal);
        }

        [TestMethod]
        public void CreateSampleHalFromSpec() {
            var hal = new HalDocument("http://example.org");
            hal.AddNamespace("td", new Uri("http://mytodoapp.com/rels/"));

            hal.AddLink("td:search", "/todo-list/search;{searchterm}");
            hal.AddLink("td:description", "/todo-list/description");
            hal.AddProperty("created_at", "2010-01-16");
            hal.AddProperty("updated_at", "2010-02-21");
            hal.AddProperty("summary", "An example list");

            hal.CreateResource("td:owner", "http://range14sux.com/mike")
                    .AddProperty("name","Mike")
                    .AddProperty("age","36")
                    .AddLink("td:friend", "http://mamund.com/")
                .End();

            hal.CreateResource("td:item", "http://home.com/tasks/126")
                .AddProperty("title", "Find Mug")
                .AddProperty("details", "Find my mug.")

                .AddTypedResource("td:attachment", "text/plain", @"
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
                .AddProperty("title", "Make tea")
                .AddProperty("details", "Mike nice tea that is green. (Gyokuro)")
                .AddLink("td:prev", "http://home.com/tasks/126")
            .End();


            var stringHal = new StreamReader(hal.ToStream()).ReadToEnd();
            Assert.IsNotNull(hal);
            Assert.IsNotNull(stringHal);
        }


		[TestMethod]
		public void DeserializeHal()
		{
			var content = GetContent();

			var hal = HalDocument.Parse(content);

			Assert.IsNotNull(hal);
		}

		[TestMethod]
		public void SerializeHal()
		{
			//Arrange
			var content = GetContent();
			var hal = HalDocument.Parse(content);

			//Act

			var stream = hal.ToStream();

			//Assert
			var result = new StreamReader(stream);

			var resultstring = result.ReadToEnd();
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
