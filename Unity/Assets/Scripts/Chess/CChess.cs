using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Animator))]
public class CChess : CCell {

	public enum ESpotState: byte {
		None = 0,
		PLAYER_FIRED = 1,
		ENEMY_FIRED = 2
	}
	
	[SerializeField]	protected ESpotState m_ChessState = ESpotState.None;
	public ESpotState chessState { 
		get { return this.m_ChessState; } 
		set { this.m_ChessState = value; } 
	}
	protected Button m_Button;
	protected RectTransform m_RectTransform;

	protected override void Awake()
	{
		base.Awake();
		this.m_Button = this.GetComponent<Button> ();
		this.m_RectTransform = this.transform as RectTransform;
		this.PlayLoopAnimation (false);
	}

	protected virtual void Start()
	{
		this.m_Button.interactable = true;
	}

	public virtual void InitChess(UnityAction callback) {
		this.m_Button.onClick.RemoveListener (callback);
		this.m_Button.onClick.AddListener (callback);
	}

	public virtual void SetState(ESpotState value, bool isLoop = false) {
		if (value == ESpotState.None)
			return;
		// CHANGE STATE
		this.m_ChessState = value;
		// END UPDATE STATE
		this.m_Button.interactable = value != ESpotState.PLAYER_FIRED;
		// PLAY ANIMATION
		this.PlayAnimation();
	}

	public virtual Vector2 GetPosition() {
		return this.m_RectTransform.localPosition;
	}
	
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = base.GetHashCode();
			hash = hash * 23 + this.m_ChessState.GetHashCode();
			return hash;
		}
	}

}
