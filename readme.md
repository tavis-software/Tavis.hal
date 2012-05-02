# Tavis.Hal #

This is a C# implementation of a parser that conforms to the Hal specification http://stateless.co/hal_specification.html

Hal is a simple hypermedia format that has both a XML and JSON variant. This parser currently only supports the XML variant. Hal is an easy way to return representations of resources that have links and other embedded resources. Application semantics are defined through the use of known link relations.

Hal documents can be created like this:

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

This parser also provides a simple path syntax to allow access to individual components of the Hal document

    XElement propertyElement1 = hal.FindProperty("/summary").Value;    
    XElement propertyElement2 = hal.FindProperty("/td:owner/name").Value;
    XElement propertyElement3 = hal.FindProperty("/td:item[2]/title").Value;