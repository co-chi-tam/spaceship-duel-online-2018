using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSetupGameScene : MonoBehaviour {

	[Header("Cells")]
	[SerializeField]	protected GameObject m_CellRoot;
	[SerializeField]	protected CChess[] m_ListChesses;
	public CChess[] listChesses {
		get { return this.m_ListChesses; }
	}
	protected CChess[,] m_MapChesses;
	public CChess[,] mapChesses {
		get { return this.m_MapChesses; }
	}

	[Header("Grids")]
	[SerializeField]	protected int m_MapColumn = 7;
	[SerializeField]	protected int m_LimitSmallShip = 3;
	[SerializeField]	protected int m_LimitBigShip = 2;

	[Header("Spaceship")]
	[SerializeField]	protected string m_PlayerFormation = "";
	[SerializeField]	protected GameObject m_Battlefield;
	[SerializeField]	protected CSpaceship[] m_SmallSpaceships;
	public CSpaceship[] smallSpaceships { 
		get { return this.m_SmallSpaceships; } 
		set { this.m_SmallSpaceships = value; }
	}
	[SerializeField]	protected CSpaceship[] m_BigSpaceships;
	public CSpaceship[] bigSpaceships { 
		get { return this.m_BigSpaceships; } 
		set { this.m_BigSpaceships = value; }
	}
	[SerializeField]	protected List<CSpot> m_PlayerSpaceshipSpots = new List<CSpot>();

	protected CSpaceship[] m_Spaceships;

	protected CPlayer m_Player;

	protected virtual void Awake()
	{
		
	}

	protected virtual void Start() {
		this.m_Player = CPlayer.GetInstance();
		this.m_ListChesses = this.m_CellRoot.GetComponentsInChildren<CChess>();
		this.InitChesses ();
		this.InitSpaceship();
	}

	protected virtual void InitChesses() {
		this.m_MapChesses = new CChess[this.m_MapColumn, this.m_MapColumn];
		for (int y = 0; y < this.m_MapColumn; y++)
		{
			for (int x = 0; x < this.m_MapColumn; x++)
			{
				var index = (y * this.m_MapColumn) + x;
				var cell = this.m_ListChesses[index];
				cell.posX = x;
				cell.posY = y;
				cell.name = string.Format("x: {0}/y: {1}", x, y);
				this.m_MapChesses[x, y] = cell;
			}
		}
	}

	public virtual void InitSpaceship() {
		// Reset places
		var shipCount = this.m_Battlefield.transform.childCount;
		for (int i = 0; i < shipCount; i++)
		{
			var child = this.m_Battlefield.transform.GetChild(0);
			DestroyImmediate (child.gameObject);
		}
		this.m_PlayerSpaceshipSpots.Clear();
		var formationStr = String.Empty; // "index1:x1:y1,index2:x2:y2,..."
		// SMALL SHIPS
		for (int i = 0; i < this.m_LimitSmallShip; i++)
		{
			var randomShip = UnityEngine.Random.RandomRange (0, this.m_SmallSpaceships.Length);
			var ship = Instantiate(this.m_SmallSpaceships[randomShip]);
			ship.transform.SetParent (this.m_Battlefield.transform);
			ship.name = String.Format("Small_Ship_{0}", i);
			formationStr += this.PlaceSpaceship (randomShip, ship, ship.spots) + ","; 
		}
		// BIG SHIPS
		for (int i = 0; i < this.m_LimitBigShip; i++)
		{
			var randomShip = UnityEngine.Random.RandomRange (0, this.m_BigSpaceships.Length);
			var ship = Instantiate(this.m_BigSpaceships[randomShip]);
			ship.transform.SetParent (this.m_Battlefield.transform);
			ship.name = String.Format("Big_Ship_{0}", i);
			formationStr += this.PlaceSpaceship (this.m_SmallSpaceships.Length + randomShip, ship, ship.spots) + (i < this.m_LimitBigShip - 1 ? "," : "");
		}
		this.m_PlayerFormation = formationStr;
	}

	protected virtual string PlaceSpaceship(int index, CSpaceship ship, params CSpot[] spots) {
		var randomX = UnityEngine.Random.RandomRange(0, this.m_MapColumn);
		var randomY = UnityEngine.Random.RandomRange(0, this.m_MapColumn);
		var step = spots.Length + 9999;
		var resultStr = String.Empty;
		for (int i = 0; i < spots.Length;)
		{
			var spot = spots[i];
			var spotX = spot.posX;
			var spotY = spot.posY;
			var newSpot = new CSpot(spot.posX + randomX, spot.posY + randomY);
			if (randomX + spotX >= 0 // MIN_X
				&& randomX + spotX < this.m_MapColumn // MAX_X
				&& randomY + spotY >= 0 // MIN_Y
				&& randomY + spotY < this.m_MapColumn // MAX_Y 
				&& this.m_PlayerSpaceshipSpots.Contains (newSpot) == false) 
			{
				i++;
			} else {
				randomX = UnityEngine.Random.RandomRange(0, this.m_MapColumn);
				randomY = UnityEngine.Random.RandomRange(0, this.m_MapColumn);
				i = 0;
			}
			step--;
			if (step < 0)
				break;
		}
		if (step < 0) {
			ship.gameObject.SetActive(false);
		} else {
			for (int i = 0; i < ship.spots.Length; i++)
			{
				var spot = ship.spots[i];
				var newSpot = new CSpot(spot.posX + randomX, spot.posY + randomY);
				this.m_PlayerSpaceshipSpots.Add (newSpot);
			}
			ship.SetPositionWithSize (randomX, randomY);
			resultStr = String.Format("{0}:{1}:{2}", index, randomX, randomY);
		}
		return resultStr;
	}

	public virtual void SubmitFormation() {
		if (string.IsNullOrEmpty (this.m_PlayerFormation)) {
			this.m_Player.ShowMessage("User name must not empty.");
			return;
		}
		this.m_Player.SetPlayerFormation (this.m_PlayerFormation);
	}

}
