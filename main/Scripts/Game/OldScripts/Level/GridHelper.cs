///////////////////////////////////////////////////////////////////////
//
//  GridHelper.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/// <summary>
/// 网格色彩辅助基类
/// </summary>
public class GridHelper : MonoBehaviour
{
	public Color gridBorderColor = new Color( 1.0f, 1.0f, 1.0f, 0.95f );
	public Color gridColor = new Color( 1.0f, 1.0f, 1.0f, 0.45f );
	public float activeGridThickness = 0.7f;
	public Color activeGridColor = new Color( 1.0f, 0.56f, 0.0f, 0.49f );
	public bool showExits = true;
	public Color exitColor = new Color( 0f, 1f, 0f, 1.0f );	
}
