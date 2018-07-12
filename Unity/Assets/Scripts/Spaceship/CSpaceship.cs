using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSpaceship : MonoBehaviour {

	[SerializeField]	protected CSpot[] m_Spots;
	public CSpot[] spots { 
		get { return this.m_Spots; } 
		set { this.m_Spots = value; }
	}
	[SerializeField]	protected Vector2 m_Size = new Vector2(100f, 100f);

	protected RectTransform m_RectTransform;

	protected virtual void Awake() {
		this.m_RectTransform = this.transform as RectTransform;
	}

	public virtual void SetPositionWithSize(int x, int y) {
		var curX = (this.m_Size.x / 2f) + (this.m_Size.x * x);
		var curY = (this.m_Size.y / 2f) + (this.m_Size.y * y);
		var curPos = new Vector2(curX, -curY);
		this.m_RectTransform.anchoredPosition = curPos;
	}
	
}
