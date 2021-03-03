using _HotfixScripts.Utils;
using EB;

namespace Hotfix_LT.UI
{
    public class PlayerDataLookupSet : DataLookupHotfix, IHotfixUpdate
    {
        private string scenetype;
        private long userid;

        public string SceneType
        {
            set
            {
                scenetype = value;

            }
        }

        public long UserId
        {
            set
            {
                userid = value;
                SetupDataLookups();
            }
        }

        public static string getSceneType()
        {
            string state = "";
            DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out state);

            if (state == null)
            {
                return "";
            }

            if (state.Equals("MainLand"))
            {
                return "mainlands";
            }
            else if (state.Equals("Campaign"))
            {
                return "campaigns";
            }
            else
            {
                return "mainlands";
            }
        }

        public void Destroy()
        {
            if (stateDataLookup != null)
            {
                stateDataLookup = null;
            }

            if (pldl != null)
            {
                pldl = null;
            }

            if (nameDataLookup != null)
            {
                nameDataLookup.OnDestroy();
                nameDataLookup = null;
            }

            if (headBars2D != null)
            {
                headBars2D.ClearBars();
                headBars2D = null;
            }
        }

        private PlayerStateDataLookup stateDataLookup;
        private PlayerLocationDataLookup pldl;
        private PlayerNameDataLookup nameDataLookup;
        private HeadBars2D headBars2D;

        public PlayerLocationDataLookup LocationDataLookup
        {
            get
            {
                if (pldl == null)
                {
                    pldl = new PlayerLocationDataLookup();
                }
                return pldl;
            }
        }

        public void SetupDataLookups()
        {
            mDL.ClearRegisteredDataIDs();
            scenetype = getSceneType();
            MoveEditor.FXHelper fx_helper = mDL.gameObject.GetComponentInChildren<MoveEditor.FXHelper>();

            if (fx_helper != null)
            {
                if (pldl == null)
                {
                    pldl = new PlayerLocationDataLookup();
                }

                pldl.UserId = userid;
                mDL.RegisterDataID(string.Format("{0}.pl.{1}", scenetype, userid));
                headBars2D = fx_helper.gameObject.GetMonoILRComponent<HeadBars2D>(false);

                if (headBars2D == null)
                {
                    headBars2D = fx_helper.gameObject.AddMonoILRComponent<HeadBars2D>("Hotfix_LT.UI.HeadBars2D");
                }

                if (nameDataLookup == null)
                {
                    nameDataLookup = new PlayerNameDataLookup();
                    nameDataLookup.SetHeadBars(headBars2D);
                }

                mDL.RegisterDataID(string.Format("{0}.pl.{1}.un", scenetype, userid));

                if (stateDataLookup == null)
                {
                    stateDataLookup = new PlayerStateDataLookup();
                    stateDataLookup.SetHeadBars(headBars2D);
                }

                mDL.RegisterDataID(string.Format("{0}.pl.{1}.state", scenetype, userid));
                mDL.RegisterDataID(string.Format("{0}.pl.{1}.promoid", scenetype, userid));
            }
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            if (dataID == null || value == null)
            {
                return;
            }

            string[] strs = dataID.Split('.');

            if (strs.Length < 3)
            {
                return;
            }
            else if (strs.Length == 3)
            {
                if (pldl != null)
                {
                    pldl.OnLookupUpdate(dataID, value, mDL.hasStarted);
                }
            }
            else if (strs[3].CompareTo("un") == 0 && nameDataLookup != null)
            {
                nameDataLookup.OnLookupUpdate(dataID, value);
            }
            else if (strs[3].CompareTo("state") == 0 && stateDataLookup != null)
            {
                object data = mDL.GetDefaultLookupData<object>();
                stateDataLookup.OnLookupUpdate(dataID, value, data);
            }else if (strs[3].CompareTo("promoid") == 0 && stateDataLookup != null)
            {
                object temp;
                DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.pl.{1}.state", scenetype, userid), out temp);
                if (temp != null)
                {
                    stateDataLookup.OnLookupUpdate(dataID, temp, temp);
                }
            }
        }

        public void Update()
        {
            if (stateDataLookup != null)
            {
                stateDataLookup.OnLookUpUpdateView();
            }
        }
    }
}