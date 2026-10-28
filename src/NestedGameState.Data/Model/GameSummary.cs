namespace NestedGameState.Data.Model;

public sealed record GameSummary
{
    public int Round { get; init; }

    public required LeaderSnapshot Leader { get; init; }

    public required VictorySnapshot Victory { get; init; }
}

public sealed record LeaderSnapshot
{
    public required string PlayerId { get; init; }

    public int Score { get; init; }
}

public sealed record VictorySnapshot
{
    public const string MostPoints = "mostPoints";
    public const string FirstToEdge = "firstToEdge";
    public const string LastPlayerStanding = "lastPlayerStanding";

    public required string Kind { get; init; }

    // Only "mostPoints" has a round limit.
    public int? Rounds { get; init; }
}
