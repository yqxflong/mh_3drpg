namespace Hotfix_LT.UI
{
    public class PLayTweenAni : DynamicMonoHotfix
    {
        private UITweener[] twe;

        public override void OnEnable()
        {
            if (twe == null)
            {
                twe = mDMono.transform.GetComponents<UITweener>();
            }
    
            for (int i = 0; i < twe.Length; i++)
            {
                twe[i].ResetToBeginning();
                twe[i].PlayForward();
            }
        }
    }
}
