using Moq;

namespace AutoChess.Test;

public class AutoChessTests
{
	private GameController _game;
	private static Mock<IPosition> _positionTest;
	private static Mock<IPiece> _heroTest;
	private static Mock<IPlayer> _playerTest;
	private static IBoard _boardTest;
	private static IHeroDetails _heroDetailsTest;

	
	[SetUp]
	public void Setup()
	{
		_boardTest = new Board(6);
		_heroDetailsTest = new HeroDetails(PieceTypes.Knight, 0, 0, 0, 0);
		
		_playerTest = new Mock<IPlayer>();
		_heroTest = new Mock<IPiece>();
		_heroTest.SetupGet(hero => hero.PieceType).Returns(PieceTypes.Knight);
		_heroTest.SetupGet(hero => hero.Name).Returns("Hero Test");
		_positionTest = new Mock<IPosition>();
		
		_game = new(_boardTest, 4, 1);
	}

	[Test]
	public void SetGameStatus_GameStatusChanged()
	{
		_game.SetGameStatus(Status.OnGoing);
		
		Assert.That(_game.CurrentGameStatus, Is.EqualTo(Status.OnGoing));
	}
	
	[Test]
	public void AddPlayer_Success_NewPlayer()
	{
		var actual = _game.AddPlayer(_playerTest.Object, Sides.Fuchsia);

		Assert.IsTrue(actual);
	}

	[Test]
	public void AddPlayer_Failed_CantAddTheSamePlayerTwice()
	{
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);

		bool actual = _game.AddPlayer(_playerTest.Object, Sides.Red);

		Assert.IsFalse(actual);
	}

	[Test]
	public void AddPlayerPiece_Success_PlayerExist()
	{
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);
		_game.AddHero(_heroTest.Object.Name, _heroDetailsTest);

		var actual = _game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);

		Assert.IsTrue(actual);
	}

	[Test]
	public void AddPlayerPiece_Failed_PlayerNotExist()
	{
		var actual = _game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		
		Assert.IsFalse(actual);
	}

	[Test]
	public void AddPlayerPiece_Failed_HeroDetailsNotValid()
	{
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);
		_heroTest.SetupGet(hero => hero.Hp).Returns(1000);

		var actual = _game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		
		Assert.IsFalse(actual);
	}

	[Test]
	public void AddPlayerPiece_Failed_PieceNotExist()
	{
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);

		var actual = _game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		
		Assert.IsFalse(actual);
	}
	
	[Test]
	public void PutPlayerPiece_Success_PositionValid()
	{
		_positionTest = new Mock<IPosition>();
		_positionTest.SetupGet(pos => pos.X).Returns(3);
		_positionTest.SetupGet(pos => pos.Y).Returns(3);
		_game.AddPlayer(_playerTest.Object, Sides.Red);
		_game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		
		var actual = _game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _positionTest.Object);

		Assert.IsTrue(actual);
	}

	[TestCase(-1, -2)]
	[TestCase(6, 6)]
	public void PutPlayerPiece_Failed_PositionNotValid(int x, int y)
	{
		_positionTest.SetupGet(pos => pos.X).Returns(x);
		_positionTest.SetupGet(pos => pos.Y).Returns(y);
		_game.AddPlayer(_playerTest.Object, Sides.Red);
		_game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		
		var actual = _game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _positionTest.Object);

		Assert.IsFalse(actual);
	}

	[Test]
	public void PutPlayerPiece_Failed_PlayerNotExist()
	{
		_positionTest.SetupGet(pos => pos.X).Returns(3);
		_positionTest.SetupGet(pos => pos.Y).Returns(3);
		_game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		
		var actual = _game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _positionTest.Object);

		Assert.IsFalse(actual);
	}

	[Test]
	public void PutPlayerPiece_Success_NewPositionValid()
	{
		_positionTest.SetupGet(pos => pos.X).Returns(3);
		_positionTest.SetupGet(pos => pos.Y).Returns(3);
		_game.AddPlayer(_playerTest.Object, Sides.Red);
		_game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		_game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _positionTest.Object);

		var _newPositionTest = new Mock<IPosition>();
		_newPositionTest.SetupGet(pos => pos.X).Returns(3);
		_newPositionTest.SetupGet(pos => pos.Y).Returns(2);
		
		var actual = _game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _newPositionTest.Object);

		Assert.IsTrue(actual);
	}

	[TestCase(-1, -2)]
	[TestCase(6, 6)]
	public void PutPlayerPiece_Failed_NewPositionNotValid(int x, int y)
	{
		_positionTest.SetupGet(pos => pos.X).Returns(3);
		_positionTest.SetupGet(pos => pos.Y).Returns(3);
		_game.AddPlayer(_playerTest.Object, Sides.Red);
		_game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		_game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _positionTest.Object);

		var _newPositionTest = new Mock<IPosition>();
		_newPositionTest.SetupGet(pos => pos.X).Returns(x);
		_newPositionTest.SetupGet(pos => pos.Y).Returns(y);
		
		var actual = _game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _newPositionTest.Object);

		Assert.IsFalse(actual);
	}

	[Test]
	public void ClearBoard_Success_PlayerExist()
	{
		_positionTest.SetupGet(pos => pos.X).Returns(3);
		_positionTest.SetupGet(pos => pos.Y).Returns(3);
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);
		_game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _positionTest.Object);
		
		_game.ClearBoard();
		
		Assert.That(_game.GetPlayerBoard(_playerTest.Object), Is.EqualTo(new Dictionary<IPosition, IPiece>()));
	}

	[Test]
	public void TryGetPieceById_Success_PieceExist()
	{
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);
		_game.AddHero(_heroTest.Object.Name, _heroDetailsTest);
		_game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);

		var actual = _game.TryGetPieceById(_heroTest.Object.PieceId, out _);

		Assert.IsTrue(actual);
	}

	[Test]
	public void TryGetPieceById_Failed_PieceNotExist()
	{
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);

		var actual = _game.TryGetPieceById(_heroTest.Object.PieceId, out _);

		Assert.IsFalse(actual);
	}

	[Test]
	public void TryGetPieceById_Failed_PlayerNotExist()
	{
		var actual = _game.TryGetPieceById(_heroTest.Object.PieceId, out _);

		Assert.IsFalse(actual);
	}

	[Test]
	public void TryGetPlayerByPieceId_Success_PlayerExist()
	{
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);
		_game.AddHero(_heroTest.Object.Name, _heroDetailsTest);
		_game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);

		var actual = _game.TryGetPlayerByPieceId(_heroTest.Object.PieceId, out _);

		Assert.IsTrue(actual);
	}

	[Test]
	public void TryGetPlayerByPieceId_Failed_HeroNotExist()
	{
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);

		var actual = _game.TryGetPlayerByPieceId(_heroTest.Object.PieceId, out _);

		Assert.IsFalse(actual);
	}

	[Test]
	public void TryGetPlayerByPieceId_Failed_PlayerNotExist()
	{
		var actual = _game.TryGetPlayerByPieceId(_heroTest.Object.PieceId, out _);

		Assert.IsFalse(actual);
	}
}