///////////////////////////////////////////////////////////////////////
//
//  ZoneHelper.cs
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
/// «¯”Ú∏®÷˙¿‡
/// </summary>
[ ExecuteInEditMode, RequireComponent ( typeof( ZoneDescriptor ) ) ]
public class ZoneHelper : MonoBehaviour 
{
	// this is best here as both environments and levels have the "Zones" root, so this is not appropriate to go in one of them
	public const string ZonesRootName = "Zones";

	// [SerializeField]
	public bool showGrid = true;
	
	// [SerializeField]
	public bool useGridSnapping = true;

	// [SerializeField]
	public float snapUnitsX = 64.0f;
	
	// [SerializeField]
	public float snapUnitsY = 1.0f;
	
	// [SerializeField]
	public float snapUnitsZ = 64.0f;
	
	// [SerializeField]
	public float rotateUnits = 90.0f;

	// [SerializeField]
	public bool rotateAroundCenter = false;

	public bool allowRegenerationOfNavMesh = true; // if false, the generate nav mesh button will be disabled, and generating nav meshes for all zones will not generate a new nav mesh for this zone

#if UNITY_EDITOR
	private ZoneDescriptor _zoneDescriptor;
	private GridHelper _helper;
	private Vector3 _previousPosition;
	
	private float _activeGridThickness;
	private Color _exitColorInner;

	private Transform _activeInteractionSet;
	public Transform ActiveInteractionSet
	{
		get
		{
			return _activeInteractionSet;
		}
		set
		{
			if (value != null && _activeInteractionSet != null && value.GetInstanceID() == _activeInteractionSet.GetInstanceID())
			{
				return;
			}

			Transform oldInteractionSet = _activeInteractionSet;

			if (oldInteractionSet != null)
			{
				oldInteractionSet.gameObject.name = _activeInteractionSet.GetComponent<InteractionSetComponent>().BaseName;
			}

			_activeInteractionSet = value;

			if (_activeInteractionSet != null)
			{
				_activeInteractionSet.gameObject.name = _activeInteractionSet.GetComponent<InteractionSetComponent>().BaseName + InteractionSetComponent.activeTag;
			}
		}
	}
	
	void Awake()
	{
		_zoneDescriptor = gameObject.GetComponent<ZoneDescriptor>();
		if(transform != null && transform.parent != null && transform.parent.parent != null)
		{
			_helper = transform.parent.parent.gameObject.GetComponent<GridHelper>();
		}
	}

	void OnDrawGizmos()
	{
		if( _helper == null )
			return;
		
		_exitColorInner = _helper.exitColor * 0.75f;
		
		float gridSizeX = EditorVars.GridSize;
		float gridSizeZ = EditorVars.GridSize;
		GlobalNavHelper nav_helper = GetComponent<GlobalNavHelper>();
		if(nav_helper != null)
		{
			gridSizeX = nav_helper.m_Range.x;
			gridSizeZ = nav_helper.m_Range.z;
		}

		if ( showGrid == true && _helper != null )
		{
			// render horizontal lines
			RenderGrid( true );
			
			// render vertical lines
			RenderGrid( false );
		}

		if ( _helper.showExits && _zoneDescriptor.zoneExits != 0 )
		{
			float startX = 0.0f;
			float startZ = 0.0f;

			Gizmos.color = _helper.exitColor;

			// render exit gizmos
			if ( ( _zoneDescriptor.zoneExits & ZoneDescriptor.eZoneExit.North ) != 0 )
			{
				startX = ( float )( gridSizeX / 2 ) + transform.position.x;
				startZ = ( float )( gridSizeZ ) + transform.position.z;

				Gizmos.color = _helper.exitColor;
				Gizmos.DrawCube( new Vector3( startX, 0.01f, startZ ), new Vector3( EditorVars.GridSize / 8, 0.01f, 2.0f ) );
				Gizmos.color = _exitColorInner;
				Gizmos.DrawCube( new Vector3( startX, 0.011f, startZ ), new Vector3( EditorVars.GridSize / 8, 0.011f, 2.0f ) * 0.75f );
			}

			if ( ( _zoneDescriptor.zoneExits & ZoneDescriptor.eZoneExit.South ) != 0 )
			{
				startX = ( float )( gridSizeX / 2 ) + transform.position.x;
				startZ = transform.position.z;

				Gizmos.color = _helper.exitColor;
				Gizmos.DrawCube( new Vector3( startX, 0.01f, startZ ), new Vector3( EditorVars.GridSize / 8, 0.01f, 2.0f ) );
				Gizmos.color = _exitColorInner;
				Gizmos.DrawCube( new Vector3( startX, 0.011f, startZ ), new Vector3( EditorVars.GridSize / 8, 0.011f, 2.0f ) * 0.75f );
			}

			if ( ( _zoneDescriptor.zoneExits & ZoneDescriptor.eZoneExit.West ) != 0 )
			{
				startX = transform.position.x;
				startZ = ( float )( gridSizeZ / 2 ) + transform.position.z;

				Gizmos.color = _helper.exitColor;
				Gizmos.DrawCube( new Vector3( startX, 0.01f, startZ ), new Vector3( 2.0f, 0.01f, EditorVars.GridSize / 8 ) );
				Gizmos.color = _exitColorInner;
				Gizmos.DrawCube( new Vector3( startX, 0.011f, startZ ), new Vector3( 2.0f, 0.011f, EditorVars.GridSize / 8 ) * 0.75f );
			}

			if ( ( _zoneDescriptor.zoneExits & ZoneDescriptor.eZoneExit.East ) != 0 )
			{
				startX = ( float )( gridSizeX ) + transform.position.x;
				startZ = ( float )( gridSizeZ / 2 ) + transform.position.z;

				Gizmos.color = _helper.exitColor;
				Gizmos.DrawCube( new Vector3( startX, 0.01f, startZ ), new Vector3( 2.0f, 0.01f, EditorVars.GridSize / 8 ) );
				Gizmos.color = _exitColorInner;
				Gizmos.DrawCube( new Vector3( startX, 0.011f, startZ ), new Vector3( 2.0f, 0.011f, EditorVars.GridSize / 8 ) * 0.75f );
			}
		}
	
		// debug: render a sphere to indicate central rotation point
		// Gizmos.DrawSphere( new Vector3( transform.position.x + 32.0f, 0.0f, transform.position.z + 32.0f ), 1.0f );
	}
	
	private void RenderGrid( bool isHorizontal = true )
	{
		// draw some debug lines (ridSize * EditorVars.GridSize @ 1 meter gaps)
		Vector3 cachedPosition = transform.position;
		Color color = Color.white;
		float gridSizeX = EditorVars.GridSize;
		float gridSizeZ = EditorVars.GridSize;
		GlobalNavHelper nav_helper = GetComponent<GlobalNavHelper>();
		if(nav_helper != null)
		{
			gridSizeX = nav_helper.m_Range.x;
			gridSizeZ = nav_helper.m_Range.z;
		}

		for ( int i = 0; i < (isHorizontal ? gridSizeZ + 1 : gridSizeX + 1); i++ )
		{
			Vector3 src = new Vector3( isHorizontal? cachedPosition.x : cachedPosition.x + i, cachedPosition.y, isHorizontal? cachedPosition.z + i : cachedPosition.z );
			Vector3 dest = src + new Vector3( isHorizontal? gridSizeX : 0.0f, 0.0f, isHorizontal? 0.0f : gridSizeZ );

			if( i == 0 || i == gridSizeZ )
			{
				if( transform == EnvironmentHelper.ActiveZone )
				{
					color = _helper.activeGridColor;
					DrawGridLine( src, dest, color, isHorizontal, !isHorizontal );
					// DrawGridLine( src, dest, color, false, true );
				}
				else
				{
					DrawGridLine( src, dest, _helper.gridBorderColor );
				}
			}
			else
			{
				if( transform == EnvironmentHelper.ActiveZone )
				{
					color = _helper.gridColor;
				}
				else
				{
					Color inactiveColor = _helper.gridColor;
					color = inactiveColor * 0.55f;
				}
				
				DrawGridLine( src, dest, color );
			}
		}
	}
	
	private void DrawGridLine( Vector3 src, Vector3 dest, Color col, bool isThickHoriz = false, bool isThickVert = false )
	{
		Gizmos.color = col;
		// Gizmos.color = new Color( Random.value, Random.value, Random.value );
		
		if( !isThickHoriz && !isThickVert )
		{
			Gizmos.DrawLine( src, dest );
			return;
		}
		
		if( isThickHoriz )
		{
			Gizmos.DrawCube( new Vector3( src.x + ( EditorVars.GridSize / 2 ), 0.01f, src.z ), new Vector3( EditorVars.GridSize, 0.01f, _helper.activeGridThickness ) );
		}
		else
		{
			Gizmos.DrawCube( new Vector3( src.x, 0.01f, src.z + ( EditorVars.GridSize / 2 ) ), new Vector3( _helper.activeGridThickness, 0.01f, EditorVars.GridSize ) );
		}
	}
#endif
}
