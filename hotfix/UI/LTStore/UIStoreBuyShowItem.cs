using System.Collections;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class UIStoreBuyShowItem : LTShowItem
    {
        protected override void BindingComponent()
        {
            var t = mDMono.transform;
            Icon = t.GetComponent<DynamicUISprite>("0_Icon");
            Frame = t.GetComponent<UISprite>("0_Icon/0_IconBG");
            FrameBG = t.GetComponent<UISprite>("0_Icon/Sprite");
            Count = t.GetComponent<UILabel>("Count");
            Name = t.GetComponent<UILabel>("1_Name_Label");
            EquipType = t.GetComponent<DynamicUISprite>("0_Icon/EquipSuitIcon");
            GradeNum = t.GetComponent<UILabel>("0_Icon/BoxGradeNum");
            Corner = t.GetComponent<UISprite>("0_Icon/ShardFlag");
        }
    }
}
