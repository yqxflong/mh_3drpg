using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 区域边界信息
/// </summary>
public class ZoneBoundary : MonoBehaviour 
{
	public Vector2 m_RectangleSize = new Vector2(64.0f, 68.0f);
	public Vector3 m_Center = new Vector3(0.0f, 0.0f, 0.0f);
    /// <summary>
    /// 区域高度
    /// </summary>
	public float m_Height = 10.0f;
	public bool m_LockRotation = false;
    /// <summary>
    /// 当前的边界信息数据
    /// </summary>
	private Bounds m_Bounds;
	static List<Vector3> m_RectangleContourPoints = new List<Vector3>(4);
	bool m_Initialized = false;

	// Use this for initialization
	void Start () 
	{
		m_Bounds = GetBounds();
		m_Initialized = true;
	}

	void OnDrawGizmos()
	{
		Vector3 leftBottom = new Vector3();
		Vector3 rightBottom = new Vector3();
		Vector3 rightTop =  new Vector3();
		Vector3 leftTop =  new Vector3();
		CalculateRectangleContourPoints(ref leftBottom, ref rightBottom, ref rightTop, ref leftTop);
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(leftTop, leftBottom);
		Gizmos.DrawLine(leftBottom, rightBottom);
		Gizmos.DrawLine(rightBottom, rightTop);
		Gizmos.DrawLine(rightTop, leftTop);
	}
    /// <summary>
    /// 是否在边界内
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
	public bool IsInBoundary(Vector3 position)
	{
		if(!m_Initialized)
		{
			m_Bounds = GetBounds();
		}

		//if(m_Bounds == null)
		//{
		//	return false;
		//}

		return m_Bounds.Contains(position);
	}

	private Bounds GetBounds()
	{
		m_RectangleContourPoints.Clear();
		CalculateRectangleContourPoints(ref m_RectangleContourPoints);			
		m_Bounds = GameUtils.CalculateBounds(m_RectangleContourPoints); // calculate the bounding box to encapsulate all points we need to consider
		m_Bounds.size = new Vector3(m_Bounds.size.x, m_Height, m_Bounds.size.z);

		return m_Bounds;
	}

	private void CalculateRectangleContourPoints(ref List<Vector3> outPoints)
	{
		Vector3 leftBottom = new Vector3();
		Vector3 rightBottom = new Vector3();
		Vector3 rightTop =  new Vector3();
		Vector3 leftTop =  new Vector3();
		CalculateRectangleContourPoints(ref leftBottom, ref rightBottom, ref rightTop, ref leftTop);
		
		outPoints.Add(leftBottom);
		outPoints.Add(rightBottom);
		outPoints.Add(rightTop);
		outPoints.Add(leftTop);
	}

	private void CalculateRectangleContourPoints(ref Vector3 leftBottom, ref Vector3 rightBottom, ref Vector3 rightTop, ref Vector3 leftTop)
	{
		Vector3 forward;
		if (m_LockRotation)
		{
			forward = new Vector3(0f, 0f, 1f);
		}
		else
		{
			forward = transform.forward;
			if (!GameUtils.NormalizeXZ(ref forward)) // if the normalize fails (because forward has a y component of +/-1)
			{
				forward = new Vector3(0f, 0f, 1f);
			}
		}
		Vector3 right = new Vector3(forward.z, 0f, -forward.x);
		
		const float HalfWidth = 0.5f;
		leftBottom = (-m_RectangleSize.x * right) + (-m_RectangleSize.y * forward);
		rightBottom = (m_RectangleSize.x * right) + (-m_RectangleSize.y * forward);
		rightTop = (m_RectangleSize.x * right) + (m_RectangleSize.y * forward);
		leftTop = (-m_RectangleSize.x * right) + (m_RectangleSize.y * forward);
		
		Vector3 woffset = transform.position + m_Center;
		leftBottom = (woffset + leftBottom * HalfWidth);
		rightBottom = (woffset + rightBottom * HalfWidth);
		rightTop = (woffset + rightTop * HalfWidth);
		leftTop = (woffset + leftTop * HalfWidth);
	}
}
