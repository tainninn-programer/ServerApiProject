using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=item.db"));
builder.Services.AddProblemDetails();

var app = builder.Build();


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

app.MapPatch("/items/{id}", IResult(int id, SimpleItem value, ApplicationDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(value.Name))
    {
        var errors = new Dictionary<string, string[]>
        {
            {"name", new[] {"名前が空です"}}
        };
        return Results.ValidationProblem(errors);
    }
    
    Item? changeItem = db.Items.FirstOrDefault(e => e.Id == id);

    if(changeItem == null) return Results.NotFound();

    changeItem.Name = value.Name;
    db.SaveChanges();
    return Results.Ok();
});

app.MapDelete("/items/{id}", IResult(int id, ApplicationDbContext db) =>
{
    Item? deleteItem = db.Items.FirstOrDefault(item => item.Id == id);

    if(deleteItem == null) return Results.NotFound();

    db.Items.Remove(deleteItem);
    db.SaveChanges();

    return Results.NoContent();
});


app.Run();

public record SimpleItem(string Name);