namespace NestedGameState.Data.Model;

public sealed record BoardSnapshot
{
    public int Size { get; init; }

    public List<TileSnapshot> Tiles { get; init; } = [];
}

public sealed record TileSnapshot
{
    public required string OwnerId { get; init; }

    public Terrain Terrain { get; init; }

    public required Position Position { get; init; }
}

public sealed record Position
{
    public int X { get; init; }

    public int Y { get; init; }
}

public enum Terrain
{
    Grass,
    Forest,
    Water,
    Mountain
}
