namespace NestedGameState.Data.Model;

public sealed class SavedGame
{
    public Guid Id { get; init; }

    public DateTimeOffset SavedAt { get; init; }

    // Board -> Tiles -> Position: one jsonb column.
    public required BoardSnapshot Board { get; init; }

    // Summary -> Leader / Victory: ordinary columns (table splitting).
    public required GameSummary Summary { get; init; }
}
