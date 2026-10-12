using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

var app = builder.Build();

var items = new List<Item>
{
    new Item(Convert.ToHexString(RandomNumberGenerator.GetBytes(16)), "apple"),
    new Item(Convert.ToHexString(RandomNumberGenerator.GetBytes(16)), "banana"),
    new Item(Convert.ToHexString(RandomNumberGenerator.GetBytes(16)), "chocolate")
};

app.MapGet("/items", () => items);

app.MapGet("/items/{id}", IResult (string id) => {
    var itemsValue = items.FirstOrDefault(e => id == e.Id);
    if(itemsValue == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(itemsValue);
    };
});

app.MapPost("/items", IResult(SimpleItem value) =>
{
    if(value != null &&!string.IsNullOrWhiteSpace(value.Name))
    {
        string id = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        
        Item returnValue = new Item(id, value.Name);
        items.Add(returnValue);
        return Results.Created($"/items/{id}", returnValue);
    }
    else
    {
        var erros = new Dictionary<string, string[]>
        {
            {"name", new[] {"名前が正しくありません"}}
        };
        return Results.ValidationProblem(errors:erros);
    }
});

app.MapPatch("/items/{id}", IResult(string id, Item item) =>
{
    if (string.IsNullOrWhiteSpace(item.Name))
    {
        var errors = new Dictionary<string, string[]>
        {
            {"name", new[] {"名前が空です"}}
        };
        return Results.ValidationProblem(errors);
    }
    
    bool itemExists = items.Any(e => e.Id == id);

    if (!itemExists)
    {
        return Results.NotFound();
    }
    else
    {
        int index = items.FindIndex(e => e.Id == id);

        Item patchItem = new Item(item.Id, item.Name);
        items[index] = patchItem;
        Console.WriteLine("アイテムを新しいインスタンスに差し替えました。");
        return Results.Ok();
    }
});

app.MapDelete("/items/{id}", IResult(string id) =>
{
    bool itemExists = items.Any(e => e.Id == id);

    if (!itemExists)
    {
        return Results.NotFound();
    }
    else
    {
        Item? targetItem = items.FirstOrDefault(item => item.Id == id);

        // 2. 見つかった場合、リストから削除
        if (targetItem != null)
        {
            items.Remove(targetItem);
            Console.WriteLine($"ID: {id} を削除しました。");
        }
        return Results.NoContent();
    }
});


app.Run();

public record Item(string Id, string Name);

public record SimpleItem(string Name);