///////////////////////////////////////////////////////////////////////
//
//  Controller.cs
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
using System.Collections.Generic;

public class Controller : BaseComponent
{
    protected ReplicationView _viewRPC;
    public ReplicationView ViewRPC
    {
        get
        {
            return _viewRPC;
        }
    }

    private List<Renderer> _meshRenderers = new List<Renderer>();

    private GameObject _skinnedRigPrefabInstance;

    protected CharacterComponent _characterComponent;
	public CharacterComponent CharacterComponent
	{
        set
        {
            _characterComponent = value;
        }
		get
		{
			return _characterComponent;
		}
	}

	protected CharacterTargetingComponent _targetingComponent;
	public CharacterTargetingComponent TargetingComponent
	{
        set
        {
            _targetingComponent = value;
        }
        get
		{
			return _targetingComponent;
		}
	}

	protected Vector3 m_SpawnLocation = Vector3.zero;
	public Vector3 SpawnLocation
	{
        set {
            m_SpawnLocation = value;
        }
		get
		{
			return m_SpawnLocation;
		}
	}
	
	protected CharacterModel _characterModel;
	public CharacterModel CharacterModel
	{
        set {
            _characterModel = value;
        }
		get
		{
			return _characterModel;
		}
	}

    public GameObject SkinnedRigPrefab
    {
        get
        {
            return _skinnedRigPrefabInstance;
        }

        set
        {
            DestroyInstancedMaterials();

            _skinnedRigPrefabInstance = value;

            if (_skinnedRigPrefabInstance != null)
            {
                _meshRenderers.Clear();

                _meshRenderers.AddRange(_skinnedRigPrefabInstance.GetComponentsInChildren<Renderer>());
            }
        }
    }

    public float ChaseDistance
    {
        get
        {
            if (_characterModel == null)
            {
                return 0.0f;
            }

            return _characterModel.chaseDistance;
        }
    }
    
    protected void Awake()
    {
        _characterComponent = GetComponent<CharacterComponent>();
        _viewRPC = FindReplicationViewForComponent<Controller>();
        _targetingComponent = GetComponent<CharacterTargetingComponent>();
    }
    
	public void DestroyInstancedMaterials()
	{
		if (_meshRenderers != null)
		{
			for (int i = 0; i < _meshRenderers.Count; i++)
			{
				Renderer mr = _meshRenderers[i];
				if (mr && mr.sharedMaterial)
				{
					if (mr.sharedMaterial.name.Contains(" (Instance)"))
					{
						if (mr.material != null)
						{
                            Destroy(mr.material);
						}
					}
				}
			}
		}
	}
    
}
