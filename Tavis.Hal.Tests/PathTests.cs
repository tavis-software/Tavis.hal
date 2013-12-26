using System;
using System.Linq;
using Xunit;

namespace Tavis.Hal.Tests
{

	public class PathTests
	{
		[Fact]
		public void FindRootLink()
		{
			//Arrange
			var content = HalTests.GetContent();
			var hal = new XmlHalReader().Load(content);

			//Act
			var link = hal.FindLink("search");

			//Assert
			Assert.NotNull(link);
			Assert.Equal("search", link.Rel);

		}
        [Fact]
		public void FindRootResourceLink()
		{
			//Arrange
			var content = HalTests.GetContent();
            var hal = new XmlHalReader().Load(content);

			//Act
			var link = hal.FindLink("owner");

			//Assert
			Assert.NotNull(link);
			Assert.Equal("owner", link.Rel);
		}

        [Fact]
		public void CanParseHalPath()
		{
			//Arrange


			//Act
			var halpath = new HalPath("/foo/item[2]/bar");
			var itemSegment = halpath.Segments.ElementAt(1);

			//Assert
			Assert.Equal(3, halpath.Segments.Count());
			Assert.Equal("item[2]", itemSegment.Key);
		}

        [Fact]
		public void FindRootResourceByNameLink()
		{
			//Arrange
			var content = HalTests.GetContent();
            var hal = new XmlHalReader().Load(content);

			//Act

			var link = hal.FindLink("item[2]");

			//Assert
			Assert.NotNull(link);
			Assert.Equal("item", link.Rel);

		}
        [Fact]
		public void FindLinkInRootResource()
		{
			//Arrange
			var content = HalTests.GetContent();
            var hal = new XmlHalReader().Load(content);

			//Act
			var link = hal.FindLink("owner/friend");

			//Assert
			Assert.NotNull(link);
			Assert.Equal("friend", link.Rel);
		}

        [Fact]
		public void FindLinkChildResource()
		{
			//Arrange
			var content = HalTests.GetContent();
            var hal = new XmlHalReader().Load(content);

			//Act
			var link = hal.FindLink("item[2]/urn:tavis:prev");  // Currently semi-colons cause a problem in paths.

			//Assert
			Assert.NotNull(link);
            Assert.Equal("urn:tavis:prev", link.Rel);
		}



        [Fact]
		public void InvalidPathThrows()
		{
			// Arrange
			var content = HalTests.GetContent();
            var hal = new XmlHalReader().Load(content);
            Exception expected = null;
			// Act
			try {
				hal.FindNode("");
			}
			catch (HalPathException e)
			{
			    expected = e;
			}

			//Assert
			Assert.NotNull(expected);
		}

        [Fact]
		public void FindRootNode()
		{
			// Arrange
			var content = HalTests.GetContent();
            var hal = new XmlHalReader().Load(content);

			// Act
			var node = hal.FindNode("/");

			//Assert
			Assert.NotNull(node);
			Assert.Equal(node.Key, "self");
		}

        [Fact]
		public void FindNamedNode()
		{
			// Arrange
			var content = HalTests.GetContent();
            var hal = new XmlHalReader().Load(content);

			// Act
			var node = hal.FindNode("/item[2]/title") as IHalProperty;

			//Assert
			Assert.NotNull(node);
			Assert.Equal("Make tea", node.GetValue());
		}

        [Fact]
        public void FindProperty() {
            // Arrange
            var content = HalTests.GetContent();
            var hal = new XmlHalReader().Load(content);

            // Act
            var value = hal.FindProperty("/item[2]/title") as IHalProperty;

            //Assert
            
            Assert.Equal("Make tea", value.GetValue());
        }
        [Fact]
		public void FindNodes()
		{
			// Arrange
			var content = HalTests.GetContent();
            var hal = new XmlHalReader().Load(content);

			// Act
			var nodes = hal.SelectNodesAt("/owner");

			//Assert
			Assert.NotNull(nodes);
			Assert.True(nodes.Any());
			Assert.Equal(3, nodes.Count());
			Assert.Equal("Mike", (nodes.ElementAt(0) as IHalProperty).GetValue());
		}


        [Fact]
        public void FindNodesInSample()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = new XmlHalReader().Load(stream);

            // Act
            var nodes = hal.SelectNodesAt("/urn:tavis:dashboard");

            //Assert
            Assert.NotNull(nodes);
            Assert.True(nodes.Any());
            Assert.Equal(3, nodes.Count());

        }


        [Fact]
        public void FindResourcesUsingIndexer()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = new XmlHalReader().Load(stream);

            // Act
            var nodes = hal.Root.Resources["urn:tavis:status"];

            //Assert
            Assert.NotNull(nodes);
            Assert.True(nodes.Any());
            Assert.Equal(3, nodes.Count());

        }


        [Fact]
        public void FindRootResourcesByName()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = new XmlHalReader().Load(stream);

            // Act
            var nodes = hal.Root.Resources["urn:tavis:dashboard"];

            //Assert
            
            Assert.Equal(1, nodes.Count());

        }

        [Fact]
        public void GetPropertyFromResource()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = new XmlHalReader().Load(stream);

            var nodes = hal.Root.Resources["urn:tavis:status"];
            // Act
            var node = nodes.First();
            var isOK = node.Properties["IsOk"];


            //Assert
            Assert.Equal("True", isOK);

        }

        [Fact]
        public void GetPropertyFromResourceWithXPath()
        {
            // Arrange
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "HalSample.xml");
            var hal = new XmlHalReader().Load(stream);
            var nodes = hal.Root.Resources["urn:tavis:status"];

            // Act
            var node = nodes.First();
            var isRequired = node.Properties["ServerName/@IsRequired"];

            //Assert
            Assert.Equal("false", isRequired);

        }
	}
}
