using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class CCell : MonoBehaviour {

	[SerializeField]	private CResult.EResult m_Result;
	[SerializeField]	private GameObject m_InactiveObj;
	[SerializeField]	protected CSpot m_Spot;
	public int posX { 
		get { return this.m_Spot.posX; } 
		set { this.m_Spot.posX = value; }
	}
	public int posY { 
		get { return this.m_Spot.posY; } 
		set { this.m_Spot.posY = value; }
	}
	
	protected Animator m_Animator;

	protected virtual void Awake()
	{
		this.m_Animator = this.GetComponent<Animator> ();
		if (this.m_InactiveObj != null) {
			this.m_InactiveObj.SetActive (false);
		}
	}

	public virtual void PlayAnimation() {
		if (this.m_Animator != null) {
			this.m_Animator.SetTrigger("IsFire");
		}
	}

	public virtual void PlayLoopAnimation(bool value) {
		if (this.m_Animator != null) {
			this.m_Animator.SetBool("IsLoop", value);
		}
	}

	public virtual void SetCellValue(CResult.EResult value) {
		this.m_Result = value;
		switch (value)
		{
			default:
			case CResult.EResult.NONE:
				// NONE
				this.m_InactiveObj.SetActive (false);
				this.PlayLoopAnimation (false);
			break;
			case CResult.EResult.INACTIVE:
				// INACTIVE
				this.m_InactiveObj.SetActive (true);
				this.PlayLoopAnimation (false);
			break;
			case CResult.EResult.EXPLOSIVE:
				// PLAY ANIMATION
				this.PlayAnimation();
				this.PlayLoopAnimation (true);
				this.m_InactiveObj.SetActive (false);
			break;
		}
	}
	
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 23 + this.m_Spot.posX.GetHashCode();
			hash = hash * 23 + this.m_Spot.posY.GetHashCode();
			return hash;
		}
	}

}
