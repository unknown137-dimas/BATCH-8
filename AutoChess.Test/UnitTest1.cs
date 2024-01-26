namespace AutoChess.Test;

public class Tests
{
	public GameController game;
	public Player playerTest;
	
	[SetUp]
	public void Setup()
	{
		game = new(new Board(4), 4, 1);
		playerTest = new Player("Test");
		game.AddPlayer(playerTest, Sides.Gold1);
	}

	[Test]
	public void ClearBoard_RemoveAllBoard()
	{
		game.PutPlayerPiece(playerTest, new Hero("HeroTest", PieceTypes.Mage, 100, 200, 10, 5), new Position(3, 2));
		
		game.ClearBoard();
		
		Assert.That(game.GetPlayerBoard(playerTest), Is.EqualTo(new Dictionary<IPosition, IPiece>()));
	}
	
	[Test]
	public void PutPlayerPiece_CheckPosition()
	{
		game.PutPlayerPiece(playerTest, new Hero("HeroTest", PieceTypes.Mage, 100, 200, 10, 5), new Position(3, 2));
		var position = new Position(3,2);
		var hero = game.GetPlayerPieces(playerTest).FirstOrDefault();
		
		Assert.That(position, Is.EqualTo(game.GetHeroPosition(playerTest, hero!.PieceId)));
	}
}