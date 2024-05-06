using chairs_dotnet7_api;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("chairlist"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var chairs = app.MapGroup("api/chair");

//TODO: ASIGNACION DE RUTAS A LOS ENDPOINTS
chairs.MapGet("/", GetChairs);

chairs.MapGet("/{name}",GetChairById);

chairs.MapPost("", CreateChairs);

chairs.MapPut("",UpdateChairs);

chairs.MapPut("",IncrementStock);

chairs.MapPost("",BuyChairs);

chairs.MapDelete("",DeleteChairs);

app.Run();

//TODO: ENDPOINTS SOLICITADOS
static async  Task<IResult> GetChairs(DataContext db)
{
    return TypedResults.Ok(await db.Chairs.ToArrayAsync());
}

static async Task<IResult> GetChairById(DataContext db, int id)
{
    return await db.Chairs.FindAsync(id)
        is Chair todo
        ? TypedResults.Ok(todo)
        : TypedResults.NotFound();
}
static async Task<IResult> CreateChairs(DataContext db, Chair chair)
{
    var chairFound = await db.Chairs.FindAsync(chair.Nombre);

    if (chairFound is null){
        return TypedResults.BadRequest("la silla ya existe");
    }
    var chairItem = new Chair
    {
        Nombre = chair.Nombre,
        Tipo = chair.Tipo,
        Material = chair.Material,
        Color = chair.Color,
        Altura = chair.Altura,
        Anchura = chair.Anchura,
        Profundidad = chair.Profundidad,
        Precio = chair.Precio,
    
    };

    db.Chairs.Add(chairItem);
    await db.SaveChangesAsync();

    chair = new chair(chairItem);
    return TypedResults.Created($"/chair/{chair.Id}", chairItem);
}

static async Task<IResult> UpdateChairs(DataContext db, int id, Chair inputLabel )
{
    var chair = await db.Chairs.FindAsync(id);

    if (chair is null) return TypedResults.NotFound();

    chair.Nombre = inputLabel.Nombre;
    chair.Tipo = inputLabel.Tipo;
    chair.Material = inputLabel.Material;
    chair.Color = inputLabel.Color;
    chair.Altura = inputLabel.Altura;
    chair.Anchura = inputLabel.Anchura;
    chair.Profundidad = inputLabel.Profundidad;
    chair.Precio = inputLabel.Precio;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
    
}

static async Task<IResult> IncrementStock(DataContext db, int id, Chair stock)
{
    var chair = await db.Chairs.FindAsync(id);

    if (chair is null) return TypedResults.NotFound();

    chair.Stock += stock.Stock ;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static IResult BuyChairs(DataContext db)
{
    return TypedResults.Ok();
}

static async Task<IResult> DeleteChairs(DataContext db, int id)
{
    if ( await db.Chairs.FindAsync(id) is Chair chair)
    {
        db.Chairs.Remove(chair);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
    return TypedResults.NotFound();
}