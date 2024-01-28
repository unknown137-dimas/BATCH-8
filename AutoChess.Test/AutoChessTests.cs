using Moq;

namespace AutoChess.Test;

public class AutoChessTests
{
	private GameController _game;
	private static Mock<IPlayer> _playerTest;
	private static Mock<IBoard> _boardTest;
	private static Mock<IPiece> _heroTest;
	
	
	[SetUp]
	public void Setup()
	{
		_playerTest = new Mock<IPlayer>();
		_boardTest = new Mock<IBoard>();
		_boardTest.SetupGet(board => board.Width).Returns(6);
		_boardTest.SetupGet(board => board.Height).Returns(6);
		_heroTest = new Mock<IPiece>();
		_game = new(_boardTest.Object, 4, 1);
	}

	[Test]
	public void SetGameStatus_GameStatusChanged()
	{
		_game.SetGameStatus(Status.OnGoing);
		
		Assert.That(_game.CurrentGameStatus, Is.EqualTo(Status.OnGoing));
	}
	
	[Test]
	public void AddPlayer_AddSuccess_NewPlayer()
	{
		var actual = _game.AddPlayer(_playerTest.Object, Sides.Fuchsia);

		Assert.IsTrue(actual);
	}

	[Test]
	public void AddPlayer_AddFailed_CantAddTheSamePlayerTwice()
	{
        _game.AddPlayer(_playerTest.Object, Sides.Fuchsia);

        bool actual = _game.AddPlayer(_playerTest.Object, Sides.Red);

        Assert.IsFalse(actual);
	}

	[Test]
	public void AddPlayerPiece_AddSuccess_PlayerExist()
	{
		var _heroTest = new Mock<IPiece>();
		_game.AddPlayer(_playerTest.Object, Sides.Fuchsia);

		var actual = _game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);

		Assert.IsTrue(actual);
	}

	[Test]
	public void AddPlayerPiece_AddFailed_PlayerNotExist()
	{
		var actual = _game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		
		Assert.IsFalse(actual);
	}
	
	[Test]
	public void PutPlayerPiece_Success_ValidPosition()
	{
		var _positionTest = new Mock<IPosition>();
		_positionTest.SetupGet(pos => pos.X).Returns(() => 3);
		_positionTest.SetupGet(pos => pos.Y).Returns(() => 3);
		_game.AddPlayer(_playerTest.Object, Sides.Red);
		_game.AddPlayerPiece(_playerTest.Object, _heroTest.Object);
		
		var actual = _game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _positionTest.Object);

		Assert.IsTrue(actual);
		_boardTest.Verify(board => board.AddHeroPosition(_playerTest.Object, It.IsNotNull<Guid>(), _positionTest.Object), Times.AtLeastOnce);
	}

	// [Test]
	// public void ClearBoard_RemoveAllBoard_PlayerExist()
	// {
	// 	var _heroTest = new Mock<IPiece>();
	// 	var _heroPositionTest = new Mock<IPosition>();
		
	// 	_game.PutPlayerPiece(_playerTest.Object, _heroTest.Object, _heroPositionTest.Object);
		
	// 	_game.ClearBoard();
		
	// 	Assert.That(_game.GetPlayerBoard(_playerTest.Object), Is.EqualTo(new Dictionary<IPosition, IPiece>()));
	// }
	
}