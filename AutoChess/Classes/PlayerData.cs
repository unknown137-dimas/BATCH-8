public class PlayerData
{
	public List<IPiece> PlayerPieces = new();
	public bool Winner;
	public int Hp;
	public List<bool> Win = new();

	public Sides PlayerSide;

	public PlayerData(int hp, Sides playerSide)
	{
		Hp = hp;
		PlayerSide = playerSide;
	}

	public IPiece GetPieceById(Guid heroId)
	{
		var result = PlayerPieces.FirstOrDefault(piece => piece.PieceId == heroId);
		return result != null ? result : throw new KeyNotFoundException();
	}

	public bool TryGetPieceById(Guid heroId, out IPiece? pieceResult)
	{
		pieceResult = PlayerPieces.FirstOrDefault(piece => piece.PieceId == heroId);
		return pieceResult != null;
	}

	public int GetWinPoint() => Win.Where(win => win).Count();
}