using ers_server_net6_minapi.Middleware;
using ers_server_net6_minapi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(x => {
    var connStrKey = "AppDbContext"; // cloud production
#if DEBUG
    connStrKey += Environment.OSVersion.Platform == PlatformID.Win32NT ? "Win" : "Mac";
#endif
    x.UseSqlServer(builder.Configuration.GetConnectionString(connStrKey));
});
builder.Services.AddCors(x =>
    x.AddPolicy("ProdCors", x =>
        x.WithOrigins("http://localhost:4200", "http://localhost")
         .AllowAnyHeader()
         .AllowAnyMethod()
    ));

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ApiKeyMiddleware>();

app.UseCors("ProdCors");

using(var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
    scope.ServiceProvider.GetService<AppDbContext>()!.Database.Migrate();
}

/* 
 * Expense APIs 
 */

app.MapGet("/api/expenses", async (AppDbContext db) => {
    return await db.Expenses!
                    .ToListAsync();
});
app.MapGet("/api/expenses/{id}", async (int id, AppDbContext db) => {
    var expense = await db.Expenses!
                            .Include(x => x.Expenselines)
                                .ThenInclude(x => x.Category)
                            .SingleOrDefaultAsync(x => x.Id == id);
    return expense != null ? Results.Ok(expense) : Results.NotFound();
});
app.MapPost("/api/expenses", async (Expense expense, AppDbContext db) => {
    db.Expenses!.Add(expense);
    expense.Active = true;
    expense.Created = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Created(String.Empty, expense);
});
app.MapPut("/api/expenses/{id}", async (int id, Expense expense, AppDbContext db) => {
    expense.Updated = DateTime.UtcNow;
    db.Entry(expense).State = EntityState.Modified;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/api/expenses/{id}", async (int id, AppDbContext db) => {
    var expense = await db.Expenses!.FindAsync(id);
    if (expense == null) {
        Results.NotFound();
    }
    db.Expenses.Remove(expense!);
    await db.SaveChangesAsync();
});

async Task Recalc(int expenseId, AppDbContext db) {
    var exp = await db.Expenses!.FindAsync(expenseId);
    if(exp == null) {
        throw new Exception("Recalc failed with invalid Expense Id!");
    }
    exp.TotalExpenses = db.Expenselines!
                            .Where(x => x.ExpenseId == expenseId)
                            .Sum(x => x.Amount);
    exp.TotalReimbursed = db.Expenselines!
                                .Where(x => x.ExpenseId == expenseId)
                                .Sum(x => x.Amount <= x.Category!.MaxAmount ? x.Amount : x.Category.MaxAmount);
    await db.SaveChangesAsync();
}

/* 
 * Expenseline APIs 
 */

app.MapGet("/api/expenselines", async (AppDbContext db) => {
    return await db.Expenselines!
                    .ToListAsync();
});
app.MapGet("/api/expenselines/{id}", async (int id, AppDbContext db) => {
    var expenseline = await db.Expenselines!
                                .Include(x => x.Category)
                                .SingleOrDefaultAsync(x => x.Id == id);
    return expenseline != null ? Results.Ok(expenseline) : Results.NotFound();
});
app.MapPost("/api/expenselines", async (Expenseline expenseline, AppDbContext db) => {
    db.Expenselines!.Add(expenseline);
    expenseline.Active = true;
    expenseline.Created = DateTime.UtcNow;
    await db.SaveChangesAsync();
    await Recalc(expenseline.ExpenseId, db);

    return Results.Created(String.Empty, expenseline);
});
app.MapPut("/api/expenselines/{id}", async (int id, Expenseline expenseline, AppDbContext db) => {
    expenseline.Updated = DateTime.UtcNow;
    db.Entry(expenseline).State = EntityState.Modified;
    await db.SaveChangesAsync();
    await Recalc(expenseline.ExpenseId, db);
    return Results.NoContent();
});
app.MapDelete("/api/expenselines/{id}", async (int id, AppDbContext db) => {
    var expenseline = await db.Expenselines!.FindAsync(id);
    if (expenseline == null) {
        Results.NotFound();
    }
    db.Expenselines.Remove(expenseline!);
    await db.SaveChangesAsync();
    await Recalc(expenseline!.ExpenseId, db);
});

/* 
 * Category APIs 
 */

app.MapGet("/api/categories", async (AppDbContext db) => {
    return await db.Categories!.ToListAsync();
});
app.MapGet("/api/categories/{id}", async (int id, AppDbContext db) => {
    var category = await db.Categories!.FindAsync(id);
    return category != null ? Results.Ok(category) : Results.NotFound();
});
app.MapPost("/api/categories", async (Category category, AppDbContext db) => {
    db.Categories!.Add(category);
    category.Active = true;
    category.Created = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Created(String.Empty, category);
});
app.MapPut("/api/categories/{id}", async (int id, Category category, AppDbContext db) => {
    category.Updated = DateTime.UtcNow;
    db.Entry(category).State = EntityState.Modified;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/api/categories/{id}", async (int id, AppDbContext db) => {
    var category = await db.Categories!.FindAsync(id);
    if (category == null) {
        Results.NotFound();
    }
    db.Categories.Remove(category!);
    await db.SaveChangesAsync();
});

app.Run();

