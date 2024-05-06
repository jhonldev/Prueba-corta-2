using System.Reflection.Metadata.Ecma335;
using chairs_dotnet7_api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("chairlist"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var chairs = app.MapGroup("api/chair");

//TODO: ASIGNACION DE RUTAS A LOS ENDPOINTS
chairs.MapGet("/", GetChairs);
chairs.MapPost("/", CharCreate);
chairs.MapGet("/{Nombre}", GetChair);
chairs.MapPut("/{id}", PutChair);
chairs.MapPut("/{id}/stock", PutStock);
chairs.MapPost("/purchase", PostPurchase);
chairs.MapDelete("/{id}", DeleteChair);


app.Run();

//TODO: ENDPOINTS SOLICITADOS
static IResult GetChairs(DataContext db)
{
    return TypedResults.Ok(db.Chairs.ToListAsync());
}

static async Task CharCreate(DataContext db,Chair chair)
{
     if (await db.Chairs.FindAsync(chair.Nombre) != null){
        TypedResults.BadRequest("Ya existe");
        return;
   }
    db.Chairs.Add(chair);
    TypedResults.Created("Se creo correctamente");
    return;
}

static async Task GetChair(DataContext db,string name){
    if (await db.Chairs.FindAsync(name) != null){
        Chair chair = await db.Chairs.FindAsync(name);
        TypedResults.Ok(chair);
        return;
   }
   TypedResults.NotFound();
}

static async Task PutChair(DataContext db, int id, Chair chair){
    Chair chairInitial = await db.Chairs.FindAsync(id);

    if (chairInitial == null){
         TypedResults.NotFound();
         return;
   }

    if (chair.Nombre != null){
        chairInitial.Nombre = chair.Nombre;
    }
    if (chair.Material != null){
        chairInitial.Material = chair.Material;
    }
    if (chair.Tipo != null){
        chairInitial.Tipo = chair.Tipo;
    }
    if (chair.Color != null){
        chairInitial.Color =chair.Color;
    }
    if (chair.Altura != 0){
        chairInitial.Altura = chair.Altura;
    }
    if (chair.Anchura != 0){
        chairInitial.Anchura = chair.Anchura;
    }
    if (chair.Profundidad != 0){
        chairInitial.Profundidad =chair.Profundidad;
    }
    if (chair.Profundidad != 0){
        chairInitial.Precio = chair.Precio;
    }
    await db.SaveChangesAsync();
    TypedResults.NoContent();
    return;
}

static async Task PutStock(DataContext db, int id, int stock){
    Chair chairInitial = await db.Chairs.FindAsync(id);

    if (chairInitial == null){
         TypedResults.NotFound();
         return;
   }

    if (stock != 0){
        chairInitial.Stock = stock;
    }
    await db.SaveChangesAsync();
    TypedResults.Ok();
    return;
}

static async Task PostPurchase(DataContext db, int id, int cantidad, int dinero){
    Chair chairBuy = await db.Chairs.FindAsync(id);

    if (chairBuy == null){
        TypedResults.BadRequest("Error");
        return;
   }

   if (cantidad > chairBuy.Stock){
        TypedResults.BadRequest("Error");
        return;
   }

   if ((cantidad*chairBuy.Stock) != cantidad){
        TypedResults.BadRequest("Error");
   }

    chairBuy.Stock = (chairBuy.Stock - cantidad);
    await db.SaveChangesAsync();
    TypedResults.Ok("Exito");
}

static async Task DeleteChair(DataContext db, int id){

    if(await db.Chairs.FindAsync(id) is Chair chair){
        db.Chairs.Remove(chair);
        await db.SaveChangesAsync();
        TypedResults.NoContent();
        return;
    }
    TypedResults.NotFound();

}