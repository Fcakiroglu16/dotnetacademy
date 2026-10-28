using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NestedGameState.Data;
using NestedGameState.Data.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Games")));
builder.Services.AddScoped<GameQueries>();

var app = builder.Build();

var games = app.MapGroup("/games");

games.MapGet("/victory/{kind}", async (string kind, GameQueries queries) =>
    await queries.UsingVictory(kind).Take(5).Select(GameDto.From).ToListAsync());

games.MapGet("/led-by/{playerId}", async (string playerId, GameQueries queries) =>
    await queries.LedBy(playerId).Select(GameDto.From).ToListAsync());

games.MapGet("/edge/{playerId}", async (string playerId, GameQueries queries) =>
    await queries.WithTileOnEdgeContainment(playerId).CountAsync());

app.Run();

record GameDto(Guid Id, string Leader, int Score, string Victory, int Tiles)
{
    // Translated to SQL, so only the needed columns are read.
    public static readonly Expression<Func<SavedGame, GameDto>> From = g =>
        new GameDto(g.Id, g.Summary.Leader.PlayerId, g.Summary.Leader.Score, g.Summary.Victory.Kind, g.Board.Tiles.Count);
}
