using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class BlockActionState : AnimationOnceReactionActionState
    {
        public static readonly string blockMoveName = "Block";

        public BlockActionState(Combatant combatant)
            : base(combatant)
        {
            MoveName = blockMoveName;
            RotateToIdle = true;
            UseDefaultMove = true;
        }

        public BlockActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveName = blockMoveName;
            RotateToIdle = true;
            UseDefaultMove = true;
        }

        public override void End()
        {
            base.End();

            TryReturnToOriginPosition();
        }
    }
}