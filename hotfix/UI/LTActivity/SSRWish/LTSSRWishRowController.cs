namespace Hotfix_LT.UI
{
    
    public class LTSSRWishRowController : DynamicRowController<Data.HeroInfoTemplate, SSRWishItem>
    {
        public override void Awake()
        {
            base.Awake();
            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new SSRWishItem[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<SSRWishItem>();
                }
            }
        }

    }
}