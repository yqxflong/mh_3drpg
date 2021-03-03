namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceCurseDataLookup : DataLookupHotfix
    {
        public ParticleSystemUIComponent Fx;

        public override void Awake()
        {
            base.Awake();

            var t = mDL.transform;
            Fx = t.GetComponent<ParticleSystemUIComponent>("FX");
        }
    
        public override void OnLookupUpdate(string dataID, object value)
        {
            if (dataID == null || value == null)
            {
                return;
            }
    
            int num = int.Parse(value.ToString());
            Fx.gameObject.CustomSetActive(num > 0);
        }
    }
}
