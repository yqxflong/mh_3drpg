using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;


namespace Hotfix_LT.UI
{

	public class NationBattleModelHelper : DynamicMonoHotfix, IHotfixLateUpdate
	{

		public List<NationBattleHudController.TargetPosEntry> nextPosList;
		float moveSpeed;
		Vector3 moveDir;
		float collisionDistance = 0.03f;
		int posIndex;
		float fullTime;
		BattleCellData cellData;
		Transform shadow_t;
		Vector3 shadowOffset;
		Transform mySelfFlag;
		Vector3 mySelfFlagOffset;

		public void SetData(BattleCellData data, float speed, List<NationBattleHudController.TargetPosEntry> npl, float dis, Transform shadow, Vector3 shadowOffset)
		{
			posIndex = 0;
			cellData = data;
			moveSpeed = speed;
			nextPosList = npl;
			shadow_t = shadow;
			this.shadowOffset = shadowOffset;
			collisionDistance = dis;
			if (nextPosList.Count > 0)
			{
				moveDir = nextPosList[posIndex].dir;
			}
			else
			{
				mDMono.enabled = false;
				mDMono.gameObject.CustomSetActive(false);
				shadow_t.gameObject.CustomSetActive(false);
			}
		}

		public void SetSelfFlag(Transform flag, Vector3 offset)
		{
			mySelfFlag = flag;
			mySelfFlagOffset = offset;
			if (nextPosList.Count <= 0)
			{
				mySelfFlag.gameObject.CustomSetActive(false);
			}
		}

        public override void OnEnable()
        {
            base.OnEnable();
            RegisterMonoUpdater();
        }
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public void LateUpdate()
		{
			fullTime += Time.deltaTime;
			Vector3 moveLength = moveSpeed * Time.deltaTime * moveDir;
			Vector3 transPos = mDMono.transform.position;
			transPos += moveLength;

			Vector3 forwardDir = (nextPosList[posIndex].position - transPos).normalized;
			if (Mathf.RoundToInt(Vector3.Dot(moveDir, forwardDir)) == -1)
			{
				if (posIndex == nextPosList.Count - 1)
				{
					mDMono.enabled = false;
					mDMono.gameObject.CustomSetActive(false);
					shadow_t.gameObject.CustomSetActive(false);
					if (mySelfFlag != null)
					{
						mySelfFlag.gameObject.CustomSetActive(false);
					}
				}
				else
				{
					moveDir = nextPosList[posIndex + 1].dir;
					float littleMove = (nextPosList[posIndex].position - transPos).magnitude;
					shadow_t.position = mDMono.transform.position = nextPosList[posIndex].position + moveDir * littleMove;
					shadow_t.position += shadowOffset;
					if (mySelfFlag != null)
					{
						mySelfFlag.position = mDMono.transform.position;
						mySelfFlag.position += mySelfFlagOffset;
					}
					posIndex++;
				}
			}
			else
			{
				shadow_t.position = mDMono.transform.position = transPos;
				shadow_t.position += shadowOffset;
				if (mySelfFlag != null)
				{
					mySelfFlag.position = mDMono.transform.position;
					mySelfFlag.position += mySelfFlagOffset;
				}
			}

			if (cellData.state == eBattleCellState.Walk)
			{
				if (cellData.direction == eBattleDirection.UpAttack)
					CollisionDetection(NationManager.Instance.BattleSyncData.UpPathData.CacheDefendBattleCellList);
				else if (cellData.direction == eBattleDirection.UpDefend)
					CollisionDetection(NationManager.Instance.BattleSyncData.UpPathData.CacheAttackBattleCellList);
				else if (cellData.direction == eBattleDirection.MiddleAttack)
					CollisionDetection(NationManager.Instance.BattleSyncData.MiddlePathData.CacheDefendBattleCellList);
				else if (cellData.direction == eBattleDirection.MiddleDefend)
					CollisionDetection(NationManager.Instance.BattleSyncData.MiddlePathData.CacheAttackBattleCellList);
				else if (cellData.direction == eBattleDirection.DownAttack)
					CollisionDetection(NationManager.Instance.BattleSyncData.DownPathData.CacheDefendBattleCellList);
				else if (cellData.direction == eBattleDirection.DownDefend)
					CollisionDetection(NationManager.Instance.BattleSyncData.DownPathData.CacheAttackBattleCellList);
			}
		}

		void CollisionDetection(List<BattleCellData> battleCellDataList)
		{
			BattleCellData walkFrontestCellData = battleCellDataList.Find(c => c.state == eBattleCellState.Walk);
			if (walkFrontestCellData != null && walkFrontestCellData.modelTrans != null)
			{
				float distance = (walkFrontestCellData.modelTrans.position - mDMono.transform.position).sqrMagnitude;
				if (distance < collisionDistance)
				{
					mDMono.enabled = false;
				}
			}
		}
	}

}
