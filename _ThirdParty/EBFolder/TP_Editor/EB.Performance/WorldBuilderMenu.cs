using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class WorldBuilderMenu : MonoBehaviour 
{
	// TRACKS

	public static void Build(string track)
	{
		LevelBuilder builder = new LevelBuilder(track);
		LevelBuilder.BuildStages buildStages = new LevelBuilder.BuildStages();
		buildStages.Merge = true;
		buildStages.BakeLighting = true;
		builder.BuildByName(buildStages);
		//LevelBuilder.BuildByName(track, buildStages);
	}

	//[MenuItem("Build/WorldBuild/CampaignView", false, 199)]
	public static void Build_CampaignView()
	{
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/_GameAssets/Res/Environments/CampaignView/CampaignView.unity");
        Build("CampaignView");
	}

	//[MenuItem("Build/WorldBuild/MapView", false, 200)]
	public static void Build_MapView()
	{
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/_GameAssets/Res/Environments/CampaignView/MapView.unity");
        Build("MapView");
	}

	//[MenuItem("Build/WorldBuild/CityView", false, 201)]
	public static void Build_CityView()
	{
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/_GameAssets/Res/Environments/CampaignView/CityView.unity");
        Build("CityView");
	}	
	
	// TRACKS + PROBES
	
	public static void BuildProbes(string track)
	{
		LevelBuilder builder = new LevelBuilder(track);
		LevelBuilder.BuildStages buildStages = new LevelBuilder.BuildStages();
		buildStages.BakeLightProbes = true;
		builder.BuildByName(buildStages);
	}
	
	// TRACKS FOR DEVICE
	
	public static void BuildForDevice(string track)
	{
		LevelBuilder builder = new LevelBuilder(track);
		LevelBuilder.BuildStages buildStages = new LevelBuilder.BuildStages();
		buildStages.BakeLightProbes = true;
		buildStages.Merge = true;
		buildStages.UseGlobalBakeLightmaps = EditorUtility.DisplayDialog("Choose lightmaps", "Which lightmaps do you want to build with?", "Global lightmaps", "Block lightmaps");
		buildStages.BakeLightProbes = EditorUtility.DisplayDialog("Light Probes", "Re-bake light probes?", "Re-bake", "Use existing");
		
		bool shouldContinue = EditorUtility.DisplayDialog("Confirm", "Are you sure you want to build this track?", "Build", "Cancel");
		
		if (shouldContinue) builder.BuildByName(buildStages);
	}
	
	//TRACKS FOR BAKING
	
	public static void BuildForDeviceWithBaking(string track)
	{
		LevelBuilder builder = new LevelBuilder(track);
		LevelBuilder.BuildStages buildStages = new LevelBuilder.BuildStages();
		buildStages.BakeLighting = true;
		buildStages.BakeLightProbes = true;
		buildStages.Merge = true;
		builder.BuildByName(buildStages);
	}
	
	//[MenuItem("Fast/Tracks/Build For Device + Bake/track_tokyo_garage", false, 499)]
	public static void BuildDeviceBake_track_tokyo_garage()
	{
		BuildForDeviceWithBaking("track_tokyo_garage");
	}
	
	//[MenuItem("Fast/Tracks/Build For Device + Bake/track_miami_drag", false, 500)]
	public static void BuildDeviceBake_track_miami_drag()
	{
		BuildForDeviceWithBaking("track_miami_drag");
	}
	
	//[MenuItem("Fast/Tracks/Build For Device + Bake/track_miami_street", false, 501)]
	public static void BuildDeviceBake_track_miami_street()
	{
		BuildForDeviceWithBaking("track_miami_street");
	}
	
	//[MenuItem("Fast/Tracks/Build For Device + Bake/track_tokyo_drag", false, 510)]
	public static void BuildDeviceBake_track_tokyo_drag()
	{
		BuildForDeviceWithBaking("track_tokyo_drag");
	}
	
	//[MenuItem("Fast/Tracks/Build For Device + Bake/track_tokyo_street", false, 511)]
	public static void BuildDeviceBake_track_tokyo_street()
	{
		BuildForDeviceWithBaking("track_tokyo_street");
	}
	
	//[MenuItem("Fast/Tracks/Build For Device + Bake/track_tokyo_drift", false, 511)]
	public static void BuildDeviceBake_track_tokyo_drift()
	{
		BuildForDeviceWithBaking("track_tokyo_drift");
	}
	
}
