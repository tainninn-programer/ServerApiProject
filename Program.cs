using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=item.db"));
builder.Services.AddProblemDetails();

var app = builder.Build();

var items = new List<ItemInfo>
{
    new ItemInfo(Convert.ToHexString(RandomNumberGenerator.GetBytes(16)), "apple"),
    new ItemInfo(Convert.ToHexString(RandomNumberGenerator.GetBytes(16)), "banana"),
    new ItemInfo(Convert.ToHexString(RandomNumberGenerator.GetBytes(16)), "chocolate")
};

app.MapGet("/items", (ApplicationDbContext db) => db.Items.ToList());

app.MapGet("/items/{id}", IResult (int id, ApplicationDbContext db) => {
    var itemsValue = db.Items.FirstOrDefault(e => id == e.Id);
    if(itemsValue == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(itemsValue);
    };
});

app.MapPost("/items", IResult(SimpleItem value, ApplicationDbContext db) =>
{
    if(value != null &&!string.IsNullOrWhiteSpace(value.Name))
    {        
        Item returnValue = new Item {Name = value.Name};
        db.Items.Add(returnValue);
        db.SaveChanges();
        return Results.Created($"/items/{returnValue.Id}", returnValue);
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

app.MapPatch("/items/{id}", IResult(string id, SimpleItem value) =>
{
    if (string.IsNullOrWhiteSpace(value.Name))
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

        ItemInfo patchItem = new ItemInfo(id, value.Name);
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
        ItemInfo? targetItem = items.FirstOrDefault(item => item.Id == id);

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

public record ItemInfo(string Id, string Name);

public record SimpleItem(string Name);