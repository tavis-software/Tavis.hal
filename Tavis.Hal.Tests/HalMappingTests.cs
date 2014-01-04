using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tavis.IANA;
using Xunit;

namespace Tavis.Hal.Tests
{
    public class HalMappingTests
    {
        

        [Fact]
        public void MapSimpleClassWithoutHelpers()
        {
            var entity = new Author()
            {
                FirstName = "Joe",
                LastName = "Brown",
                Address = new Address()
                {
                    Street = "1 Dastardly Way",
                    City = "Brighton"
                },
                Books = new List<Book>() {
                    new Book() {Title = "The last of the great foos"},
                    new Book() {Title = "The first born bar"},
                }
            };

            var resource = new HalResource(new AuthorLink(), new object[]
            {
                new HalProperty("firstName", entity.FirstName),          
                new HalProperty("lastName", entity.LastName),
                new DescribesLink() {Target = new Uri("http://example.org/describes")},
                new HalResource(new Link(), new object[]
                {
                    new HalProperty("street",entity.Address.Street),
                    new HalProperty("city",entity.Address.City),
                }),
                entity.Books.Select(b=> new HalResource(new ItemLink(), new HalProperty("title",b.Title))).ToList()
            });

            var jsonHal = new StreamReader(new JsonHalWriter().ToStream(resource)).ReadToEnd();
            var xmlHal = new StreamReader(new XmlHalWriter().ToStream(resource)).ReadToEnd();

        }
    }

    public class Author
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public List<Book> Books { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }

    public class Book
    {
        public string Title { get; set; }
    }
}
