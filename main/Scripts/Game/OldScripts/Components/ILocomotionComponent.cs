///////////////////////////////////////////////////////////////////////
//
//  ILocomotionComponent.cs
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

public interface ILocomotionComponent
{
	float MaxSpeed { get; set; }
	float CurrentSpeed { get; }
	float AngularSpeed { get; set; }
	bool ShouldIgnoreRotation { get; set; }
	bool PathPending { get; }
	bool Enabled { get; set; }
	Vector3 Velocity { get; }
	float RemainingDistance { get; }
	float ArrivalThreshold { get; }
	Vector3 Destination { get; set; }
	
	void Initialize();
	void Stop();
	bool HasLineOfSight( Vector3 target, GameObject ignore );
    void TouchTap();
	void TouchStarted();
    void TouchEnded();
    bool WasMostRecentDestinationRequestAccepted();
}
