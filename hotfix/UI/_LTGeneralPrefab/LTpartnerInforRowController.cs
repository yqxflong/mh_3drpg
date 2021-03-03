namespace Hotfix_LT.UI
{
    public class LTpartnerInforRowController : DynamicRowController<Hotfix_LT.Data.HeroInfoTemplate, LTpartnerInfoItem>
    {
        public override void Awake()
        {
            base.Awake();

            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new LTpartnerInfoItem[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<LTpartnerInfoItem>();
                }
            }
        }
    }
}
