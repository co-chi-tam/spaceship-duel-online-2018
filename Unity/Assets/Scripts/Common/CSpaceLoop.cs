using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class CSpaceLoop : MonoBehaviour {

    [SerializeField]    protected float m_Speed = 0.1f;
    [SerializeField]    protected Vector2 m_Direction = new Vector2(0f, 1f);

	protected RawImage m_RawImage;

    protected virtual void Awake() {
        this.m_RawImage = this.GetComponent<RawImage> ();
    }

    protected virtual void Update() {
        var rect = this.m_RawImage.uvRect;
        rect.x += Time.deltaTime * this.m_Speed * this.m_Direction.x;
        rect.y += Time.deltaTime * this.m_Speed * this.m_Direction.y;
        this.m_RawImage.uvRect = rect;
    }
	
}
