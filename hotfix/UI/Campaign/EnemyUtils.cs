//EnemyUtils
//创建敌人
//Johny

using UnityEngine;

namespace Hotfix_LT.UI
{
	public static class EnemyUtils
	{

		private const string RES_Enemy = "Bundles/Prefab/EnemyCharacter";

		public static void CreateEnemy(string name, string model_name, Vector3 pos, Quaternion qt, System.Action<EnemyController> fn)
		{
			CharacterModel modelToSpawn = CharacterCatalog.Instance.GetModel(model_name);
			if (model_name.Contains("NPC_Template"))
			{
				modelToSpawn = CharacterCatalog.Instance.GetModelTemplate();
				modelToSpawn.NPC_Template = model_name.Replace("_NPC_Template", "");
			}
			if (modelToSpawn == null)
			{
				EB.Debug.LogError("EnemySpawnLogic:: No model in CharacterModel for Name:{0}" , model_name);
				fn(null);
				return;
			}

			EB.Assets.LoadAsync(RES_Enemy, typeof(GameObject), o =>
			{
				if(!o)
				{
					return;
				}

				GameObject spawnedEnemy = Replication.Instantiate(o, pos, qt, RES_Enemy) as GameObject;
				if (spawnedEnemy == null)
				{
					EB.Debug.LogError("EnemySpawnLogic::Trying to initialize null spawned enemy");
					fn(null);
				}
				else
				{
					EnemyController enemyController = spawnedEnemy.GetComponent<EnemyController>();
					if (enemyController == null)
					{
						EB.Debug.LogError("EnemySpawnLogic::Update can't obtain EnemyController component from spawned enemy object");
						fn(null);
					}
					else
					{
                        Player.EnemyHotfixController ehc = enemyController.transform.GetMonoILRComponent<Player.EnemyHotfixController>();
                        ehc.ObjectName = spawnedEnemy.name;
						spawnedEnemy.name = name;
                        ehc.OnSpawn(modelToSpawn, -1, false, () => fn(enemyController));
						//EventManager.instance.Raise(new SpawnEndedEvent(spawnedEnemy));
						//fn(enemyController);
					}
				}
			});
		}
	}
}

