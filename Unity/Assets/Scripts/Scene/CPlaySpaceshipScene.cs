using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CPlaySpaceshipScene : MonoBehaviour {

	[SerializeField]	protected Animator m_Animator;
	[SerializeField]	protected Text m_RoonNameDisplay;
	[SerializeField]	protected CUIPlayerInRoom[] m_DisplayPlayers;

	[Header("Events")]
	public UnityEvent OnLocalTurn;
	public UnityEvent OnOffTurn;

	protected CPlayer m_Player;
	protected CGameManager m_GameManager;
	protected bool m_IsStartGame = false;

	protected virtual void Start() {
		this.m_Player = CPlayer.GetInstance();
		this.m_GameManager = CGameManager.GetInstance();
		this.m_IsStartGame = false;
		// Receive setup player
		this.m_Player.RemoveListener("PlayerInRoomUI", this.SetupPlayers);
		this.m_Player.AddListener("PlayerInRoomUI", this.SetupPlayers);
		this.SetupPlayers();
		InvokeRepeating("SetupPlayers", 0f, 1f);
	}

	protected virtual void SetupPlayers() {
		#if UNITY_DEBUG
		Debug.Log ("SetupPlayers");
		#endif
		var currentRoom = this.m_Player.room;
		var maximumPlayer = currentRoom.roomPlayes.Length > 2 ? 2 : currentRoom.roomPlayes.Length;
		for (int i = 0; i < maximumPlayer; i++) {
			this.m_DisplayPlayers[i].SetPlayerName (currentRoom.roomPlayes[i].name);
		}
		this.m_RoonNameDisplay.text = currentRoom.roomName;
		if (maximumPlayer >= 2) {
			this.PlayAnimStartGame ();
			var turnIndex = this.m_GameManager.turnIndex;
			this.m_DisplayPlayers[0].SetInTurnActive (!turnIndex);
			this.m_DisplayPlayers[1].SetInTurnActive (turnIndex);
		}
		if (this.m_GameManager.IsLocalTurn()) {
			if (this.OnLocalTurn != null) {
				this.OnLocalTurn.Invoke();
			}
		} else {
			if (this.OnOffTurn != null) {
				this.OnOffTurn.Invoke();
			}
		}
	}

	protected virtual void PlayAnimStartGame() {
		if (this.m_IsStartGame == false) {
			this.m_Animator.SetTrigger ("StartGame");
			this.m_IsStartGame = true;
		}
	}

}
