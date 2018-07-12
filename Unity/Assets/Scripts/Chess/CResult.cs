using System;
using UnityEngine;

[Serializable]
public class CResult : CSpot {

	public enum EResult {
		NONE = 0,
		INACTIVE = 1,
		EXPLOSIVE = 2
	}

	public EResult value = EResult.NONE;

	public CResult(): base()
	{
		this.value = EResult.NONE;
	}

	public CResult(int x, int y): base(x, y)
	{
		this.value = EResult.NONE; 
	}

	public CResult(CSpot value): base(value)
	{
		this.value = EResult.NONE;
	}

	public CResult(CResult value)
	{
		this.m_X = value.posX;
		this.m_Y = value.posY;
		this.value = value.value;
	}

}
