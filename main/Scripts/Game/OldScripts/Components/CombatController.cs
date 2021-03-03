using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatController : MonoBehaviour
{
	public enum eCombatType
	{
		Melee,
		Ranged
	}

	private eCombatType _combatType;
	private AnimationController _animController;

	private AutoAttackSetModel _attackSet;
	//private Effect _currentAutoAttackProcs;

	private CharacterModel _characterModel;
	private int _currentAttackIndex;
	private int _attackLayerMask;

    private int _enemiesAttackingMe = 0;

    private bool _canInterruptCombo = false;
    private bool _hasAttackHit = false;

    private int _shouldMuteAutoAttackSounds = 0;

    public delegate void EffectEventHandler(eEffectEvent evt);
	public event EffectEventHandler OnEffectEvent;

	public event System.Action AutoAttackChangedEvent;

	public const float GAM_COMBAT_RANGE = 1.8f;

	public int AnimationLayer
	{
		get; set;
	}

	public int ShouldMuteAutoAttackSounds
	{
		get
		{
			return _shouldMuteAutoAttackSounds;
		}
		set
		{
			if (value < 0)
			{
				_shouldMuteAutoAttackSounds = 0;
			}
			else
			{
				_shouldMuteAutoAttackSounds = value;
			}
		}
	}

    public int EnemiesAttackingMe
    {
        get
        {
            return _enemiesAttackingMe;
        }
    }

	public bool IsMelee
	{
		get
		{
			return _combatType == eCombatType.Melee;
		}
	}

	public bool IsRanged
	{
		get
		{
			return _combatType == eCombatType.Ranged;
		}
	}

	public float AttackRange
	{
		get
		{
			return _characterModel.autoAttack.range;
		}
	}

	public float AttackRangeSq
	{
		get
		{
			float attackRange = AttackRange;
			return attackRange * attackRange;
		}
	}

	public bool CanInterruptCombo
	{
		get
		{
			return _canInterruptCombo;
		}
	}

	public bool HasAttackHit
	{
		get
		{
			return _hasAttackHit;
		}
	}

	public bool CanMoveAndAttack
	{
		get 
		{
			return _attackSet.movementSlowFactor > 0.0f;
		}
	}

	public float AttackMovementSlowFactor
	{
		get 
		{
			return _attackSet.movementSlowFactor;
		}
	}

	public bool CanChargeAttack
	{
		get
		{
			return GetCurrentAttack().canCharge;
		}
	}

	public AutoAttackSetModel AttackSet 
	{
		get 
		{
			return _attackSet;
		}
	}

	void Awake()
	{

	}

	void Start()
	{

	}

	public void Initialize(Transform muzzleParentTransform, CharacterModel character, int attackLayerMask)
	{
		_currentAttackIndex = 0;
		_canInterruptCombo = false;
		_hasAttackHit = false;
		
		_animController = GetComponent<AnimationController>();
		_attackLayerMask = attackLayerMask;
		_characterModel = character;

		//InventoryComponent inventoryComponent = GetComponent<InventoryComponent>();
		//if (inventoryComponent != null)
		//{
  //          //TODO:palyercontroller删除classid属性屏蔽，不知道还有没用
		//	//inventoryComponent.AutoAttackSpiritActivatedEvent += ActivateSpirit;
		//	inventoryComponent.AutoAttackSpiritDeactivatedEvent += DeactivateSpirit;
		//}

		enabled = true;
	}

	public void SetAutoAttack(AutoAttackSetModel attackSet)//, EffectModel autoAttackProc = null)
	{
		_attackSet = attackSet;
		_combatType = _attackSet.IsRanged ? eCombatType.Ranged : eCombatType.Melee;

		if (_characterModel.isPlayer)
		{
			_attackSet.Preload();
		}

		SendEffectEvent(eEffectEvent.AutoAttackChanged);
		//if (autoAttackProc != null)
		//{
		//	autoAttackProc.Preload();
		//	EffectContext newContext = EffectContext.Create();
		//	newContext.Initialize(gameObject, gameObject, this);
		//	autoAttackProc.CreateAndCast(gameObject, gameObject, newContext);
		//}

		if (AutoAttackChangedEvent != null)
		{
			AutoAttackChangedEvent();
		}
	}

	public AutoAttackModel ResetAttackSequence()
	{
		if (_attackSet.setType == AutoAttackSetModel.eSetType.Random)
		{
			_currentAttackIndex = Random.Range(0, _attackSet.Count);
		}
		else
		{
			_currentAttackIndex = 0;
		}

		_hasAttackHit = false;
		_canInterruptCombo = false;
		return GetCurrentAttack();
	}

	public AutoAttackModel GetCurrentAttack()
	{
		return _attackSet[_currentAttackIndex];
	}

	public bool IsInRange(GameObject target)
	{
		return IsInRange(target, AttackRange);
	}

	public bool IsInCombatRange(GameObject target)
	{
		return IsInRange(target, GAM_COMBAT_RANGE);
	}

	public bool IsInRange(GameObject target, float range)
	{
		CharacterComponent targetChar = target.GetComponent<CharacterComponent>();
		float otherRvoRadius = targetChar != null && targetChar.Model != null ? targetChar.Model.rvoRadius : 0.0f;
		return GameUtils.GetDistSqXZ(transform.position, target.transform.position) < (range + _characterModel.rvoRadius + otherRvoRadius) * (range + _characterModel.rvoRadius + otherRvoRadius);	
	}

	public void StartAttack(GameObject target)
	{
		AutoAttackModel attack = GetCurrentAttack();
		SendEffectEvent(eEffectEvent.AutoAttackStart);
		//if (attack.startAttackEffectModel != null)
		//{
		//	EffectContext newContext = EffectContext.Create();
		//	newContext.Initialize(gameObject, target, this);
		//	newContext.Muted = ShouldMuteAutoAttackSounds > 0;
		//	attack.startAttackEffectModel.CreateAndCast(gameObject, target, newContext);
		//}
	}

	public void CancelAttack()
	{
		SendEffectEvent(eEffectEvent.AutoAttackCancel);
	}

	public void TryAttack(GameObject target)
	{
		if (_hasAttackHit)
			return;

		if (_combatType == eCombatType.Ranged)
		{
			AutoAttackModel attack = GetCurrentAttack();
			//EffectContext context = EffectContext.Create();
			//context.Initialize(gameObject, target, this);
			//if (_currentAttackIndex == _attackSet.attacks.Count - 1)
			//{
			//	context["AttackLength"] = GetCurrentAttackEvent(eAnimationEvent.ResetCombo, AnimationLayer);
			//}
			//else
			//{
			//	context["AttackLength"] = GetCurrentAttackEvent(eAnimationEvent.InterruptCombo, AnimationLayer);
			//}
			
			//context.Muted = ShouldMuteAutoAttackSounds > 0;
			//attack.attackEffectModel.CreateAndCast(gameObject, target, context);
		}
		else
		{
			HitMelee(target);
		}

		SendEffectEvent(eEffectEvent.AutoAttackHit);
		_hasAttackHit = true;
	}

	public void StartChargeAttack(GameObject target)
	{
		AutoAttackModel attack = GetCurrentAttack();
		//if (attack.startChargeEffectModel != null)
		//{
		//	EffectContext newContext = EffectContext.Create();
		//	newContext.Initialize(gameObject, target, this);
		//	attack.startChargeEffectModel.CreateAndCast(gameObject, target, newContext);
		//}
		SendEffectEvent(eEffectEvent.ChargeAttackStart);
	}

	public void CancelChargeAttack()
	{
		SendEffectEvent(eEffectEvent.ChargeAttackCancel);
	}

	public void ChargeAttack(GameObject target, float timeHeld)
	{
		if (_combatType == eCombatType.Ranged)
		{
			//EffectContext context = EffectContext.Create();
			//context.Initialize(gameObject, target, this);
			AutoAttackModel attack = GetCurrentAttack();
			timeHeld = Mathf.Min(timeHeld, attack.maxChargeTime);
			//context["ChargeTimeNormalized"] = timeHeld / attack.maxChargeTime;
			//context["ChargeTime"] = timeHeld;
			//attack.chargeEffectModel.CreateAndCast(gameObject, target, context);
			SendEffectEvent(eEffectEvent.ChargeAttackHit);
		}
		else
		{
			EB.Debug.LogWarning("Melee charge attacks are not implemented");
		}
	}

	public AutoAttackModel IncrementAttackSequence()
	{
		if (_attackSet.setType == AutoAttackSetModel.eSetType.Random)
		{
			_currentAttackIndex = Random.Range(0, _attackSet.Count);
		}
		else
		{
			_currentAttackIndex++;
			if (_currentAttackIndex >= _attackSet.Count)
			{
				ResetAttackSequence();
			}
		}

		_hasAttackHit = false;
		_canInterruptCombo = false;
		return GetCurrentAttack();
	}

	public List<GameObject> GetMeleeTargets()
	{
		AutoAttackModel attack = GetCurrentAttack();
		int maxVictims = attack.maxVictims;

		//maxVictims += (int)StatsComponent.ExtraAutoAttackTargets.Value;

		if (maxVictims == 0)
			return new List<GameObject>();

#if DEBUG
		//GlobalEffectReceiver globalEffectReceiver = GlobalEffectReceiver.Instance;
		//if (globalEffectReceiver != null)
		//{
		//	globalEffectReceiver.AddDebugAOE(gameObject, 0.5f, transform.position, AttackRange, 0, _attackSet.coneWidthDegrees, transform.forward);
		//}
#endif

		Collider[] hits = Physics.OverlapSphere(transform.position, AttackRange + _characterModel.rvoRadius, _attackLayerMask);
		//System.Array.Sort(hits, Effect.SortTargetCollider);
		List<GameObject> allHits = new List<GameObject>();
		foreach (Collider entity in hits)
		{
			if (Vector3.Angle(transform.forward, entity.transform.position - transform.position) < _attackSet.coneWidthDegrees / 2.0f)
			{
				allHits.Add(entity.gameObject);
			}
		}

		int victimsSelected = 0;
		List<GameObject> victims = new List<GameObject>();

		// Make sure the player's target is selected as a victim if it was hit by the 
		GameObject currentTarget = GetCurrentTarget();
		if (currentTarget != null && allHits.Contains(currentTarget))
		{
			victims.Add(currentTarget);
			allHits.Remove(currentTarget);
			//if (currentTarget.GetComponent<BaseStatsComponent>().IsCountedAsTarget)
			//{
			//	victimsSelected++;
			//}
		}

		//IEnumerator<GameObject> iter = allHits.GetEnumerator();
		//while (iter.MoveNext() && victimsSelected < maxVictims)
		//{
		//	victims.Add(iter.Current);
			//BaseStatsComponent baseStatsComponent = iter.Current.GetComponent<BaseStatsComponent>();
			//if (baseStatsComponent == null)
			//{
			//	EB.Debug.LogError("Object " + iter.Current + " was marked for attackable layer, but has no stats componenet!");
			//}
			//else
			//{
			//	if (iter.Current.GetComponent<BaseStatsComponent>().IsCountedAsTarget)
			//	{
			//		victimsSelected++;
			//	}
			//}
		//}

		return victims;
	}

	private void HitMelee(GameObject target)
	{
		AutoAttackModel attack = GetCurrentAttack();
		float attackLength = GetCurrentAttackEvent(eAnimationEvent.InterruptCombo);
		List<GameObject> meleeTargets = GetMeleeTargets();

		foreach (GameObject enemy in meleeTargets)
		{
			//EffectContext context = EffectContext.Create();
			//context.Initialize(gameObject, enemy, this);
			//context["AttackLength"] = attackLength;
			//context.Muted = ShouldMuteAutoAttackSounds > 0;
			//attack.attackEffectModel.CreateAndCast(gameObject, enemy, context);
		}
	}

	private GameObject GetCurrentTarget()
	{
		CharacterTargetingComponent targeting = GetComponent<CharacterTargetingComponent>();
		return targeting == null ? null : targeting.AttackTarget;
	}

    public void OnStartAttack(GameObject attacker)
    {
		_enemiesAttackingMe++;
    }

    public void OnStopAttack(GameObject attacker)
    {
		_enemiesAttackingMe--;
    }

    public float GetCurrentAttackEvent(eAnimationEvent evt, int layer = 0) 
    {
       	AnimationClip clip = _animController.GetPrimaryAnimationClip(layer);
    	if (clip == null)
    		return 1.0f;

    	string animName = clip.name;
   		AnimationMetadata animMetadata = AnimationMetadataCatalog.Instance.GetAnimationMetadata(animName);
		float attackTime;
		if (animMetadata == null)
		{
			EB.Debug.LogWarning("Failed to find animation metadata for {0}. Did you forget to reload it?" ,animName);
			attackTime = clip.length;
		}
		else
		{
			attackTime = animMetadata.GetEventTime(evt);
			if (animMetadata.GetEventTime(eAnimationEvent.Hit) == -1)
			{
				//Not an attack animation, don't scale it.
				return 1.0f;
			}
			if (attackTime == -1)
			{
				attackTime = clip.length;
			}
		}

		return attackTime;
    }

    public bool HasLineOfSight(Vector3 to, GameObject ignore, bool ignoreOpponents = true)
    {
    	RaycastHit hit;
        Vector3 centerOffset = new Vector3(0.0f, 0.5f, 0.0f);
        int layerMask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Ground");
        if (!ignoreOpponents) 
        {
        	layerMask = layerMask | _attackLayerMask;
        }
        // if we haven't hit anything, we have line of sight, if we hit something we can ignore, we still have line of sight
        return !Physics.Linecast(transform.position + centerOffset, to + centerOffset, out hit, layerMask) || hit.collider.gameObject == ignore;
    }

    //public void ActivateSpirit(SpiritInstance spirit)
    //{
    //	PlayerController playerController = GetComponent<PlayerController>();
    //	if (playerController != null)
    //	{
    //		AbilitySetModel abilitySet = spirit.Model.abilitySet;
    //		if (abilitySet != null)
    //		{
    //			//AutoAttackSetModel autoAttack = abilitySet.GetAutoAttackForClass(playerController.ClassId);
	   // 		//EffectModel autoAttackProc = abilitySet.GetAutoAttackProcsForClass(playerController.ClassId);

	   // 		//SetAutoAttack(autoAttack, autoAttackProc);
    //		}
    //		else 
    //		{
    //			_attackSet = _characterModel.autoAttack;
    //		}
    //	}
    //}

    public void DeactivateSpirit()
    {
    	PlayerController playerController = GetComponent<PlayerController>();
    	if (playerController != null)
    	{
    		_attackSet = _characterModel.autoAttack;

			SendEffectEvent(eEffectEvent.AutoAttackChanged);
    	}
    }

   	private void SendEffectEvent(eEffectEvent evt)
	{
		if (null != OnEffectEvent)
		{
			OnEffectEvent(evt);
		}
	}
}
