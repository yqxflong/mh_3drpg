using System.Collections;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class UIStoreShowItem : LTShowItem
    {
        protected override void BindingComponent()
        {
            var t = mDMono.transform;
            Icon = t.GetComponent<DynamicUISprite>("IMG");
            Frame = t.GetComponent<UISprite>("IMG/Border");
            FrameBG = t.GetComponent<UISprite>("IMG/BG");
            Count = t.GetComponent<UILabel>("IMG/Num");
            Name = t.GetComponent<UILabel>("IMG/Name");
            EquipType = t.GetComponent<DynamicUISprite>("IMG/EquipSuitIcon");
            GradeNum = t.GetComponent<UILabel>("IMG/BoxGradeNum");
            Corner = t.GetComponent<UISprite>("IMG/ShardFlag");
        }
    }
}
