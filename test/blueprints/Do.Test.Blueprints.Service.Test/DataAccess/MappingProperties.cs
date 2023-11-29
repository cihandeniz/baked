﻿namespace Do.Test.DataAccess;

public class MappingProperties : TestServiceSpec
{
    [Test]
    public async Task Guid()
    {
        var entity = GiveMe.An<Entity>().With(guid: GiveMe.AGuid("EB8DD0A1-07B7-42EC-AAD3-14B2A623C01E"));
        entity.Guid.ShouldBe(GiveMe.AGuid("EB8DD0A1-07B7-42EC-AAD3-14B2A623C01E"));

        await entity.Update(guid: GiveMe.AGuid("AB8DD0A1-07B7-42EC-AAD3-14B2A623C01E"));
        entity.Guid.ShouldBe(GiveMe.AGuid("AB8DD0A1-07B7-42EC-AAD3-14B2A623C01E"));

        var actual = GiveMe.The<Entities>().By(guid: GiveMe.AGuid("AB8DD0A1-07B7-42EC-AAD3-14B2A623C01E")).FirstOrDefault();
        actual.ShouldBe(entity);
    }

    [Test]
    public async Task String()
    {
        var entity = GiveMe.An<Entity>().With(@string: "string");
        entity.String.ShouldBe("string");

        await entity.Update(@string: "test");
        entity.String.ShouldBe("test");

        var actual = GiveMe.The<Entities>().By(@string: "test").FirstOrDefault();
        actual.ShouldBe(entity);
    }

    [Test]
    public async Task String_data()
    {
        var entity = GiveMe.An<Entity>().With(stringData: "string");
        entity.StringData.ShouldBe("string");

        await entity.Update(stringData: "test");
        entity.StringData.ShouldBe("test");

        var actual = GiveMe.The<Entities>().By(stringData: "test").FirstOrDefault();
        actual.ShouldBe(entity);
    }

    [Test]
    public async Task Object()
    {
        var entity = GiveMe.An<Entity>().With(dynamic: new { dynamic = "dynamic" });
        entity.Dynamic.ShouldBe(new { dynamic = "dynamic" });

        await entity.Update(dynamic: new { update = "update" });
        entity.Dynamic.ShouldBe(new { update = "update" });
    }

    [Test]
    public async Task Int()
    {
        var entity = GiveMe.An<Entity>().With(int32: 5);
        entity.Int32.ShouldBe(5);

        await entity.Update(int32: 1);
        entity.Int32.ShouldBe(1);

        var actual = GiveMe.The<Entities>().By(int32: 1).FirstOrDefault();
        actual.ShouldBe(entity);
    }

    [Test]
    public async Task Enum()
    {
        var entity = GiveMe.An<Entity>().With(@enum: Status.Enabled);
        entity.Enum.ShouldBe(Status.Enabled);

        await entity.Update(@enum: Status.Disabled);
        entity.Enum.ShouldBe(Status.Disabled);

        var actual = GiveMe.The<Entities>().By(status: Status.Disabled).FirstOrDefault();
        actual.ShouldBe(entity);
    }

    [Test]
    public async Task DateTime()
    {
        var entity = GiveMe.An<Entity>().With(dateTime: System.DateTime.Parse("5/1/2008 8:30:52 AM"));
        entity.DateTime.ShouldBe(System.DateTime.Parse("5/1/2008 8:30:52 AM"));

        await entity.Update(dateTime: System.DateTime.Parse("6/1/2008 8:30:52 AM"));
        entity.DateTime.ShouldBe(System.DateTime.Parse("6/1/2008 8:30:52 AM"));

        var actual = GiveMe.The<Entities>().By(dateTime: System.DateTime.Parse("6/1/2008 8:30:52 AM")).FirstOrDefault();
        actual.ShouldBe(entity);
    }
}
