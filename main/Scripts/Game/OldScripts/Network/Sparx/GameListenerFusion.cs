public class GameListenerFusion : EB.Sparx.GameListener
{
	public delegate void JoinedGameHandler(EB.Sparx.Game game);
	public event JoinedGameHandler JoinedGameEvent;

	public delegate void LeaveGameHandler(EB.Sparx.Game game, string reason);
	public event LeaveGameHandler LeaveGameEvent;

	public delegate void PlayerJoinedHandler(EB.Sparx.Game game, EB.Sparx.Player player);
	public event PlayerJoinedHandler PlayerJoinedEvent;

	public delegate void PlayerLeftHandler(EB.Sparx.Game game, EB.Sparx.Player player);
	public event PlayerLeftHandler PlayerLeftEvent;

	public delegate void GameEventHandler(EB.Sparx.Game game);
	public event GameEventHandler GameStartedEvent;
	public event GameEventHandler GameEndedEvent;

	public event System.Action<string> JoinGameFailedEvent;

	public void OnJoinedGame(EB.Sparx.Game game)
	{
		Replication.SetGame(game);

		if (JoinedGameEvent != null)
		{
			JoinedGameEvent(game);
		}
	}

	public void OnLeaveGame(EB.Sparx.Game game, string reason)
	{
		Replication.ClearGame();

		if (LeaveGameEvent != null)
		{
			LeaveGameEvent(game, reason);
		}
	}

	public void OnPlayerJoined(EB.Sparx.Game game, EB.Sparx.Player player)
	{
		if (PlayerJoinedEvent != null)
		{
			PlayerJoinedEvent(game, player);
		}
	}

	public void OnPlayerLeft(EB.Sparx.Game game, EB.Sparx.Player player)
	{
		Replication.OnPlayerLeft(game, player);
		
		if (PlayerLeftEvent != null)
		{
			PlayerLeftEvent(game, player);
		}
	}

	public void OnAttributesUpdated(EB.Sparx.Game game)
	{

	}

	public void OnReceive(EB.Sparx.Game game, EB.Sparx.Player player, EB.Sparx.Packet packet)
	{
		Replication.Receive(player != null ? player.PlayerId : EB.Sparx.Network.HostId, packet.Data);
	}

	public void OnGameStarted(EB.Sparx.Game game)
	{
		if (GameStartedEvent != null)
		{
			GameStartedEvent(game);
		}
	}

	public void OnGameEnded(EB.Sparx.Game game)
	{
		if (GameEndedEvent != null)
		{
			GameEndedEvent(game);
		}
	}

	public void OnJoinGameFailed(string err)
	{
		if (JoinGameFailedEvent != null)
		{
			JoinGameFailedEvent(err);
		}
	}

	public void OnUpdate(EB.Sparx.Game game)
	{

	}
}
