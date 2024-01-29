public interface IBoard
{
	int Width {get;}
	int Height {get;}
	Dictionary<IPlayer, IDictionary<IPosition, Guid>> PiecesPositions {get;}

	bool IsPositionValid(IPosition position);
	bool AddPlayerToBoard(IPlayer player);
	bool RemovePlayerFromBoard(IPlayer player);
	IDictionary<IPosition, Guid> GetPlayerBoard(IPlayer player);
	bool TryGetPlayerBoard(IPlayer player, out IDictionary<IPosition, Guid>? playerBoardResult);
	bool AddHeroPosition(IPlayer player, Guid heroId, IPosition position);
	bool UpdateHeroPosition(IPlayer player, Guid heroId, IPosition newPosition);
	IPosition GetHeroPosition(IPlayer player, Guid heroId);
	bool TryGetHeroPosition(IPlayer player, Guid heroId, out IPosition? positionResult);
	bool RemoveHeroPosition(IPlayer player, Guid heroId);
	IEnumerable<Guid> GetAllEnemyId(IPlayer player, IPiece hero);
}