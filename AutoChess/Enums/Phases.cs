/// <summary>
/// Represents the different phases of the game.
/// </summary>
public enum Phases
{
    /// <summary>
    /// The game is not yet initialized.
    /// </summary>
    NotInitialized,

    /// <summary>
    /// Players are choosing their pieces.
    /// </summary>
    ChoosingPiece,

    /// <summary>
    /// Players are placing their pieces on the board.
    /// </summary>
    PlaceThePiece,

    /// <summary>
    /// The battle phase has begun. Each player's win state is reset to false.
    /// </summary>
    BattleBegin,

    /// <summary>
    /// The battle phase has ended. Round winner is decided
    /// </summary>
    BattleEnd,

    /// <summary>
    /// The champion phase, indicating the end of the game.
    /// The final winner (champion) from all rounds is decided.
    /// </summary>
    TheChampion
}