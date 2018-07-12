using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSingleton;
using SocketIO;

public class CGameManager : CMonoSingleton<CGameManager> {

	#region Fields

	[Header("Configs")]
	// TURN INDEX.
	// TRUE is RED. FALSE is BLUE.
	[SerializeField]	protected bool m_TurnIndex = false;
	public bool turnIndex { 
		get { return this.m_TurnIndex; } 
		set { this.m_TurnIndex = value; } 
	}

	[SerializeField]	protected int m_MapColumn = 7;

	[Header("Chess")]
	[SerializeField]	protected GameObject m_CellRoot;
	[SerializeField]	protected CChess[] m_ListChesses;
	public CChess[] listChesses {
		get { return this.m_ListChesses; }
	}

	protected CChess[,] m_MapChesses;
	public CChess[,] mapChesses {
		get { return this.m_MapChesses; }
	}

	[Header("Spaceship")]
	[SerializeField]	protected GameObject m_Battlefield;
	[SerializeField]	protected CSpaceship[] m_Spaceships;
	[SerializeField]	protected List<CSpot> m_PlayerSpaceshipSpots = new List<CSpot>();
	protected List<CSpot> m_PlayerCheckSpot;
	[SerializeField]	protected List<CSpot> m_EnemySpaceshipSpots = new List<CSpot>();
	protected List<CSpot> m_EnemyCheckSpot;

	[Header("Results")]
	[SerializeField]	protected GameObject m_ResultRoot;
	protected CCell[] m_ListResults;
	protected CCell[,] m_MapResults;
	protected Dictionary<string, CResult[,]> m_Results;

	protected CPlayer m_Player;

	protected bool m_IsGameEnd = false;

	#endregion

	#region MonoBehaviour Implementation

	protected override void Awake()
	{
		base.Awake();
	}

	protected virtual void Start() {
		this.m_Player = CPlayer.GetInstance();
		// Receive chess position
		this.m_Player.socket.Off("receiveChessPosition", this.OnReceiveChessPosition);
		this.m_Player.socket.On("receiveChessPosition", this.OnReceiveChessPosition);
		// Receive start game
		this.m_Player.RemoveListener("PlayerInRoomComplete", this.OnPlayerInRoomComplete);
		this.m_Player.AddListener("PlayerInRoomComplete", this.OnPlayerInRoomComplete);
		// Init game
		this.m_ListChesses = this.m_CellRoot.GetComponentsInChildren<CChess>();
		this.m_ListResults = this.m_ResultRoot.GetComponentsInChildren<CCell>();
		this.InitGame ();
	}

	#endregion

	#region Main methods

	public virtual void InitGame() {
		this.InitChesses ();
		this.m_IsGameEnd = false;
	}

	protected virtual void InitChesses() {
		this.m_TurnIndex = false;
		// CHESS
		this.m_MapChesses = new CChess[this.m_MapColumn, this.m_MapColumn];
		// RESULT
		this.m_MapResults = new CCell[this.m_MapColumn, this.m_MapColumn];
		this.m_Results = new Dictionary<string, CResult[,]>();
		this.m_Results.Add("Player", new CResult[this.m_MapColumn, this.m_MapColumn]);
		this.m_Results.Add("Enemy", new CResult[this.m_MapColumn, this.m_MapColumn]);
		// X, Y
		for (int y = 0; y < this.m_MapColumn; y++)
		{
			for (int x = 0; x < this.m_MapColumn; x++)
			{
				var index = (y * this.m_MapColumn) + x;
				// CHESS
				var chess = this.m_ListChesses[index];
				chess.posX = x;
				chess.posY = y;
				chess.name = string.Format("x: {0}/y: {1}", x, y);
				chess.InitChess(() => {
					this.OnUpdateGame (chess.posX, chess.posY);
				});
				this.m_MapChesses[x, y] = chess;
				// RESULT
				var cell = this.m_ListResults[index];
				cell.posX = x;
				cell.posY = y;
				cell.name = string.Format("x: {0}/y: {1}", x, y);
				this.m_MapResults[x, y] = cell;
				this.m_Results["Player"][x, y] = new CResult(x, y);
				this.m_Results["Enemy"][x, y] = new CResult(x, y);
			}
		}
	}

	public virtual void InitSpaceship(string formation, List<CSpot> saveSpots, bool display) {
		var shipStrs = formation.Split(','); // "index:X:Y"
		for (int i = 0; i < shipStrs.Length; i++)
		{
			// CONFIGS
			var shipFormat = shipStrs[i].Split(':');
			var shipIndex = int.Parse (shipFormat[0]); // INDEX
			var shipX = int.Parse (shipFormat[1]);		// X
			var shipY = int.Parse (shipFormat[2]);		// Y
			// SAVE
			var shipData = this.m_Spaceships[shipIndex];
			for (int s = 0; s < shipData.spots.Length; s++)
			{
				var spot = shipData.spots[s];
				var newSpot = new CSpot (shipX + spot.posX, shipY + spot.posY);
				saveSpots.Add(newSpot);
			}
			// DISPLAY
			if (display) {
				var ship = Instantiate(shipData);
				ship.transform.SetParent (this.m_Battlefield.transform);
				ship.name = String.Format("Ship_{0}", i);
				ship.SetPositionWithSize (shipX, shipY);
			}
		}
	}

	#endregion

	#region State Game

	public virtual void OnPlayerInRoomComplete() {
		var players = this.m_Player.room.roomPlayes;
		for (int i = 0; i < players.Length; i++)
		{
			var playerData = players[i];
			if (playerData.name == this.m_Player.playerData.name) 
			{
				// IS PLAYER
				this.InitSpaceship (playerData.formation, this.m_PlayerSpaceshipSpots, true);
			} 
			else 
			{ 
				// IS ENEMY
				this.InitSpaceship (playerData.formation, this.m_EnemySpaceshipSpots, false);
			}
		}
		this.m_PlayerCheckSpot = new List<CSpot>(this.m_PlayerSpaceshipSpots);
		this.m_EnemyCheckSpot = new List<CSpot>(this.m_EnemySpaceshipSpots);
		this.OnStartGame ();
	}

	public virtual void OnStartGame() {
		// TODO
	}

	public virtual void OnUpdateGame(int x, int y) {
		if (this.m_IsGameEnd) {
			this.OnEndGame();
		} else {
			if (this.m_TurnIndex == (this.m_Player.playerData.turnIndex == 1)) {
				this.m_Player.SendChessPosition(x, y);
			} else {
				this.m_Player.ShowMessage ("This is not your turn.");
			}
			this.CheckTurn();
		}
	}

	protected virtual void OnReceiveChessPosition(SocketIOEvent e) {
		var currentPos = e.data.GetField("currentPos");
		var x = int.Parse (currentPos.GetField("x").ToString());
		var y = int.Parse (currentPos.GetField("y").ToString());
		var turnIndex = int.Parse (e.data.GetField("turnIndex").ToString());
		var chess =	this.m_MapChesses[x, y];
		this.m_TurnIndex = turnIndex == 1;
		var newSpot = new CSpot(x, y);
		if (this.IsLocalTurn()) {
			var isEnemyContain = this.m_EnemyCheckSpot.Contains(newSpot);
			chess.SetState(CChess.ESpotState.PLAYER_FIRED, isEnemyContain);
			if (isEnemyContain) {
				this.m_EnemyCheckSpot.Remove (newSpot);
				this.SetResult("Player", x, y, CResult.EResult.EXPLOSIVE);
			} else {
				this.SetResult("Player", x, y, CResult.EResult.INACTIVE);
			}
			this.ShowResult ("Enemy");
		} else {
			var isPlayerContain = this.m_PlayerCheckSpot.Contains(newSpot);
			chess.SetState(CChess.ESpotState.ENEMY_FIRED, isPlayerContain);
			if (isPlayerContain) {
				this.m_PlayerCheckSpot.Remove(newSpot);
				this.SetResult("Enemy", x, y, CResult.EResult.EXPLOSIVE);
			} else {
				this.SetResult("Enemy", x, y, CResult.EResult.INACTIVE);
			}
			this.ShowResult ("Player");
		}
		this.CheckTurn();
		this.ChangeTurn();
	}

	public virtual void OnEndGame() {
		#if UNITY_DEBUG
		Debug.Log (this.m_EnemyCheckSpot.Count == 0 ? "...YOU WIN..." : "...YOU LOSE...");
		#endif
		if (this.m_EnemyCheckSpot.Count == 0) {
			this.m_Player.ShowMessage ("...YOU WIN...", this.OnResetGame);
		} else {
			this.m_Player.ShowMessage ("...YOU LOSE...", this.OnResetGame);
		}
		this.m_IsGameEnd = true;
	}

	public virtual void OnResetGame() {
		this.m_Player.LeaveRoom();
	}

	#endregion

	#region Logics game

	public virtual void SetResult(string name, int x, int y, CResult.EResult value) {
		if (this.m_Results.ContainsKey(name) == false) 
			return;
		this.m_Results[name][x, y].value = value;
	}

	public virtual void ShowResult(string name) {
		if (this.m_Results.ContainsKey(name) == false) 
			return;
		var results = this.m_Results[name];
		for (int y = 0; y < this.m_MapColumn; y++)
		{
			for (int x = 0; x < this.m_MapColumn; x++)
			{
				var value = results[x, y].value;
				this.m_MapResults[x, y].SetCellValue (value);
			}
		}
	}

	public virtual void CheckTurn() {
		if (this.m_PlayerCheckSpot.Count == 0 
			|| this.m_EnemyCheckSpot.Count == 0) {
			this.OnEndGame();
		}
	}

	public virtual void ChangeTurn() {
		this.m_TurnIndex = !this.m_TurnIndex;
	}

	#endregion

	#region Getter && Setter

	public virtual void SetTurn (bool value) {
		this.m_TurnIndex = value;
	}

	public virtual bool IsLocalTurn() {
		if (this.m_Player == null || this.m_Player.playerData == null)
			return false;
		return this.m_TurnIndex == (this.m_Player.playerData.turnIndex == 1);
	}

	public virtual bool IsRed() {
		return this.m_TurnIndex == true;
	}

	public virtual bool IsBlue() {
		return this.m_TurnIndex == false;
	}

	#endregion

}
