using System;
using UnityEngine;

[Serializable]
public class CSpot {

	[SerializeField]	protected int m_X = 0;
	public int posX { 
		get { return this.m_X; } 
		set { this.m_X = value; }
	}
	[SerializeField]	protected int m_Y = 0;
	public int posY { 
		get { return this.m_Y; } 
		set { this.m_Y = value; }
	}

	public CSpot() 
	{
		this.m_X = 0;
		this.m_Y = 0;
	}

	public CSpot(int x, int y)
	{
		this.m_X = x;
		this.m_Y = y;
	}

	public CSpot(CSpot value)
	{
		this.m_X = value.posX;
		this.m_Y = value.posY;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		// base.Equals (obj);
		var otherObj = obj as CSpot;
		return this.posX == otherObj.posX && this.posY == otherObj.posY;
	}
	
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 23 + this.posX.GetHashCode();
			hash = hash * 23 + this.posY.GetHashCode();
			return hash;
		}
		// return base.GetHashCode();
	}

	public override string ToString() {
		return string.Format("x: {0}/y: {1}", this.m_X, this.m_Y);
	}

}
