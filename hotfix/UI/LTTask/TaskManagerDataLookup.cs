using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Hotfix_LT.UI
{
	public class TaskManagerDataLookup : DataLookupHotfix
	{

		// Use this for initialization
		public override void OnLookupUpdate(string dataID, object value)
		{
			base.OnLookupUpdate(dataID, value);

			mDL.transform.GetMonoILRComponent<MainLandLogic>().SpawnTasks();

		}
	}
}