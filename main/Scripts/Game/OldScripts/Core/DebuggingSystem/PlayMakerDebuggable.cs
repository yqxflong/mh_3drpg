///////////////////////////////////////////////////////////////////////
//
//  PlayMakerDebuggable.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

#if DEBUG
using UnityEngine;
using System.Collections;

public class PlayMakerDebuggable : IDebuggableEx
{
    public bool controlMouseCursor;
    public bool drawStateLabels;
    public bool enableGUILayout;
    public bool filterLabelsWithDistance;
    public bool GUITextStateLabels;
    public bool GUITextureStateLabels;
    //public float maxLabelDistance;
    public bool previewOnGUI;

    private PlayMakerGUI _gui;

    public PlayMakerDebuggable(PlayMakerGUI playMakerGUI)
    {
        _gui = playMakerGUI;
    }

    public void OnDrawDebug()
    {
    }

    public void OnDebugGUI()
    {
    }

    public void OnDebugPanelGUI()
    {
    }

    public void OnPreviousValuesLoaded()
    {
    }

    public void OnValueChanged(System.Reflection.FieldInfo field, object oldValue, object newValue)
    {
        typeof(PlayMakerGUI).GetField(field.Name).SetValue(_gui, newValue);
    }
}
#endif
