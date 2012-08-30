using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Hal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HalTests
{
	[TestClass]
	public class PathTests
	{
		[TestMethod]
		public void FindRootLink()
		{
			//Arrange
			var content = HalTests.GetContent();
			var hal = HalDocument.Parse(content);

			//Act
			var link = hal.FindLink("search");

			//Assert
			Assert.IsNotNull(link);
			Assert.AreEqual("search", link.Rel);

		}
		[TestMethod]
		public void FindRootResourceLink()
		{
			//Arrange
			var content = HalTests.GetContent();
			var hal = HalDocument.Parse(content);

			//Act
			var link = hal.FindLink("owner");

			//Assert
			Assert.IsNotNull(link);
			Assert.AreEqual("owner", link.Rel);
		}

		[TestMethod]
		public void CanParseHalPath()
		{
			//Arrange


			//Act
			var halpath = new HalPath("/foo/item[2]/bar");
			var itemSegment = halpath.Segments.ElementAt(1);

			//Assert
			Assert.AreEqual(3, halpath.Segments.Count());
			Assert.AreEqual("item[2]", itemSegment.Key);
		}

		[TestMethod]
		public void FindRootResourceByNameLink()
		{
			//Arrange
			var content = HalTests.GetContent();
			var hal = HalDocument.Parse(content);

			//Act

			var link = hal.FindLink("item[2]");

			//Assert
			Assert.IsNotNull(link);
			Assert.AreEqual("item", link.Rel);

		}
		[TestMethod]
		public void FindLinkInRootResource()
		{
			//Arrange
			var content = HalTests.GetContent();
			var hal = HalDocument.Parse(content);

			//Act
			var link = hal.FindLink("owner/friend");

			//Assert
			Assert.IsNotNull(link);
			Assert.AreEqual("friend", link.Rel);
		}

		[TestMethod]
		public void FindLinkChildResource()
		{
			//Arrange
			var content = HalTests.GetContent();
			var hal = HalDocument.Parse(content);

			//Act
			var link = hal.FindLink("item[2]/urn:tavis:prev");  // Currently semi-colons cause a problem in paths.

			//Assert
			Assert.IsNotNull(link);
            Assert.AreEqual("urn:tavis:prev", link.Rel);
		}



		[TestMethod]
		public void InvalidPathThrows()
		{
			// Arrange
			var content = HalTests.GetContent();
			var hal = HalDocument.Parse(content);

			// Act
			try {
				hal.FindNode("");
			}
			catch (HalPathException) {
				return;
			}

			//Assert
			Assert.Fail();
		}

		[TestMethod]
		public void FindRootNode()
		{
			// Arrange
			var content = HalTests.GetContent();
			var hal = HalDocument.Parse(content);

			// Act
			var node = hal.FindNode("/");

			//Assert
			Assert.IsNotNull(node);
			Assert.AreEqual(node.Key, "self");
		}

		[TestMethod]
		public void FindNamedNode()
		{
			// Arrange
			var content = HalTests.GetContent();
			var hal = HalDocument.Parse(content);

			// Act
			var node = hal.FindNode("/item[2]/title") as HalProperty;

			//Assert
			Assert.IsNotNull(node);
			Assert.AreEqual("Make tea", node.Value.Value);
		}

        [TestMethod]
        public void FindProperty() {
            // Arrange
            var content = HalTests.GetContent();
            var hal = HalDocument.Parse(content);

            // Act
            var value = hal.FindProperty("/item[2]/title").Value;

            //Assert
            
            Assert.AreEqual("Make tea", value.Value);
        }
		[TestMethod]
		public void FindNodes()
		{
			// Arrange
			var content = HalTests.GetContent();
			var hal = HalDocument.Parse(content);

			// Act
			var nodes = hal.SelectNodesAt("/owner");

			//Assert
			Assert.IsNotNull(nodes);
			Assert.IsTrue(nodes.Any());
			Assert.AreEqual(3, nodes.Count());
			Assert.AreEqual("Mike", (nodes.ElementAt(0) as HalProperty).Value.Value);
		}


        [TestMethod]
        public void FindNodesInSample()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = HalDocument.Load(stream);

            // Act
            var nodes = hal.SelectNodesAt("/urn:tavis:dashboard");

            //Assert
            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Any());
            Assert.AreEqual(3, nodes.Count());

        }


        [TestMethod]
        public void FindResourcesUsingIndexer()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = HalDocument.Load(stream);

            // Act
            var nodes = hal.Root.Resources["urn:tavis:status"];

            //Assert
            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Any());
            Assert.AreEqual(3, nodes.Count());

        }


        [TestMethod]
        public void FindRootResourcesByName()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = HalDocument.Load(stream);

            // Act
            var nodes = hal.Root.Resources["urn:tavis:dashboard"];

            //Assert
            
            Assert.AreEqual(1, nodes.Count());

        }

        [TestMethod]
        public void GetPropertyFromResource()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = HalDocument.Load(stream);

            var nodes = hal.Root.Resources["urn:tavis:status"];
            // Act
            var node = nodes.First();
            var isOK = node.Properties["IsOk"];


            //Assert
            Assert.AreEqual("True", isOK);

        }

        [TestMethod]
        public void GetPropertyFromResourceWithXPath()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = HalDocument.Load(stream);
            var nodes = hal.Root.Resources["urn:tavis:status"];

            // Act
            var node = nodes.First();
            var isRequired = node.Properties["ServerName/@IsRequired"];

            //Assert
            Assert.AreEqual("false", isRequired);

        }
	}
}
