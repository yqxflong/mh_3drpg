// #define LOG_LoginManager

using System;
using EB;
using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class NoticeItem
    {
        public int id;
        public string type;
        public string notice;
    
        public NoticeItem(object notices)
        {
            id = Dot.Integer("id", notices, 0);
            type = Localizer.GetString(EB.Dot.String("type", notices, ""));
            notice = Dot.String("notice", notices, "");
        }
    }
    
    
    public class LoginManager : ManagerUnit
    {
        protected LoginAPI _api;
        protected User _user;
        protected Account _account;
        protected Hashtable _authData;
        protected bool _initialized;
        protected string _lastAuthenticator = string.Empty;
        protected List<Authenticator> _authenticators = new List<Authenticator>();
        protected List<GameWorld> _gameWorlds = new List<GameWorld>();
        // protected string _notice = string.Empty;

        public User LocalUser { get { return _user; } set { _user = value; Hub.Instance.DataStore.LoginDataStore.LocalUserId = (_user != null) ? _user.Id : Id.Null; } }
        public Id LocalUserId { get { if (LocalUser != null) return LocalUser.Id; return Id.Null; } }
        public LoginState LoginState { get; private set; }
        public Authenticator[] Authenticators { get { return _authenticators.ToArray(); } }
        public Account Account { get { return _account; } }
        public GameWorld[] GameWorlds { get { return _gameWorlds.ToArray(); } }
        public LoginAPI Api { get { return _api; } }
        public List<NoticeItem> Notices = new List<NoticeItem>();

        public LoginListener LoginExtraListener;

        public static void LogWithTime(string msg)
        {
#if LOG_LoginManager
            DateTime now = DateTime.Now;
            EB.Debug.LogError($"{now.Hour}:{now.Minute}:{now.Second}=={msg}");
#endif
        }

        #region instance
        private static LoginManager sInstance = null;

        public static LoginManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<LoginManager>(); }
        }
        #endregion


        public override void Initialize(Config config)
        {
            LoginState = LoginState.Disconnected;

            if (Hub.Instance.Config.LoginConfig.Listener == null)
            {
                throw new System.ApplicationException("Missing Login Listener");
            }

            _api = new LoginAPI(Hub.Instance.ApiEndPoint);
            LoginExtraListener = new LoginListener();
            Hub.Instance.DataStore.LoginDataStore.LoginData = Johny.HashtablePool.Claim();
        }

        public long GetLoginID()
        {
            return LocalUserId.Value;
        }
        public Authenticator GetAuthenticator(string name)
        {
            for (var i = 0; i < _authenticators.Count; i++)
            {
                var authenticator = _authenticators[i];
                if (authenticator.Name == name)
                {
                    return authenticator;
                }
            }
            EB.Debug.LogError("LoginManager: Error no authenticator found for name {0}" ,name);
            return null;
        }

        public bool HasAuthenticator(string name)
        {
            for (var i = 0; i < _authenticators.Count; i++)
            {
                var authenticator = _authenticators[i];
                if (authenticator.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetDefaultRealmId()
        {
            return System.Array.Find(GameWorlds, w => w.Default).Id;
        }
        private int m_LastFrameCount;

        public void Enumerate()
        {
            if (m_LastFrameCount == UnityEngine.Time.frameCount)
            {
                return;
            }
            m_LastFrameCount = UnityEngine.Time.frameCount;
            EB.Debug.Log("--------------------SparxLoginManager : Enumerate------------------------");
            Init(delegate
            {
                EB.Debug.Log("InitAuthorizes: Init ok, silent authorize ...");

                Coroutines.Run(_Authorize(true, delegate (string err, object authData)
                {
                    if (!string.IsNullOrEmpty(err))
                    {
                        _LoginFailed(err);
                        return;
                    }

                    _authData = authData as Hashtable;
                    if (_authData.Count <= 0)
                    {
                        string LastAuthenticator = PlayerPrefs.GetString("LastAuthenticator", string.Empty);
                        EB.Debug.Log("最后一次的登入平台为:LastAuthenticator = {0},不为空的话，就直接拿去验证最后一次平台", LastAuthenticator);
                        Authenticator auth = null;
                        if (!string.IsNullOrEmpty(LastAuthenticator))
                        {
                            auth = GetAuthenticator(LastAuthenticator);
                        }
                        if (auth != null)
                        {
                            LoginExtraListener.OnEnumerate(new Authenticator[] { auth });
                        }
                        else
                        {
                            LoginExtraListener.OnEnumerate(_authenticators.ToArray());
                        }
                        return;
                    }

                    Authenticator bestAuthenticator = null;
                    foreach (DictionaryEntry entry in _authData)
                    {
                        Authenticator authenticator = GetAuthenticator(entry.Key.ToString());
                        if (bestAuthenticator == null)
                        {
                            bestAuthenticator = authenticator;
                        }
                        else if (authenticator.IsLoggedIn && (int)authenticator.Quailty > (int)bestAuthenticator.Quailty)
                        {
                            bestAuthenticator = authenticator;
                        }

                        if (!string.IsNullOrEmpty(_lastAuthenticator) && bestAuthenticator.Name == _lastAuthenticator)
                        {
                            break;
                        }
                    }

                    EB.Debug.Log("InitAuthorizes: Choose best authenticator = {0} to login ...", bestAuthenticator.Name);
                    Login(bestAuthenticator.Name);
                }));
            });
        }

        public void SetName(string name, System.Action<bool> callback)
        {
            if (_user == null)
            {
                EB.Debug.LogError("Must be logged in before setting name!");
                callback(false);
                return;
            }

            _api.SetName(name, delegate (string err, object result) {
                if (!string.IsNullOrEmpty(err))
                {
                    callback(false);
                    return;
                }

                var user = Dot.Object("user", result, null);
                if (user == null)
                {
                    EB.Debug.LogError("Missing user object on set name!!!");
                    callback(false);
                    return;
                }

                _user.Update(user);

                callback(true);
            });
        }

        public void DebugResetWorldUser(Hashtable chardata, System.Action<string, object> callback)
        {
            _api.ResetWorldUser(chardata, callback);
        }

        public void CheckName(string name, System.Action<string, object> callback)
        {
            _api.CheckName(name, callback);
        }

        public void ChangeUser(string characterId, System.Action<string, object> callback)
        {
            _api.ChangeUser(characterId, callback);
        }

        public void EnterGame(Hashtable charData)
        {
            Hub.Instance.DataCacheManager.LoadAll();
            Hashtable versions = Johny.HashtablePool.Claim(Hub.Instance.DataCacheManager.GetVersions());
            charData["versions"] = versions;
            try
            {
                _PostLogin(charData);
            }
            catch(NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
        }

        public void Login(string authenticatorName)
        {
            Init(delegate {
                var authenticator = GetAuthenticator(authenticatorName);
                if (authenticator == null)
                {
                    _LoginFailed("ID_SPARX_ERROR_UNKNOWN");
                    return;
                }

                SetState(LoginState.Authenticating);

                authenticator.Authenticate(false, delegate (string err, object data) {
                    if (!string.IsNullOrEmpty(err))
                    {
                        _LoginFailed(err);
                        return;
                    }
                    else if (data == null)
                    {
                        EB.Debug.Log("Login: authenticater {0} need relogin", authenticator.Name);
                        _LoginFailed(null);
                        return;
                    }

                    _authData[authenticator.Name] = data;
                    _Login(authenticator.Name, _authData);
                });
            });
        }

        public void GetSupportUrl(System.Action<string, string> cb)
        {
            _api.GetSupportUrl(delegate (string err, object result) {
                cb(err, EB.Dot.String("url", result, string.Empty));
            });
        }

        private void Init(System.Action callback)
        {
            if (_initialized)
            {
                SetState(LoginState.Initialized);
                callback();
                return;
            }

            SetState(LoginState.Initializing);

            _api.Init(delegate (string err, object data) {
                if (!string.IsNullOrEmpty(err))
                {
                    _LoginFailed(err);
                    return;
                }
                Coroutines.Run(_PostInit(data, callback));
            });
        }

        public class WorldCompare : IComparer<GameWorld>
        {
            public int Compare(GameWorld x, GameWorld y)
            {
                return x.Id - y.Id;
            }
        }

        void _Login(string authenticatorName, object authData)
        {
            _PreLogin(delegate (Hashtable secData) {
                _api.Login(authenticatorName, authData, secData, delegate (string err, object obj) {
                    if (!string.IsNullOrEmpty(err))
                    {
                        _LoginFailed(err);
                        return;
                    }

                    if (LoginState != LoginState.Authenticating)
                    {
                        EB.Debug.Log("_Login: already authenticated");
                        return;
                    }

                    _lastAuthenticator = authenticatorName;
                    PlayerPrefs.SetString("LastAuthenticator", _lastAuthenticator);

                    SetState(LoginState.Authenticated);

                    var account = Dot.Object("account", obj, null);
                    _account = new Account(account);

                    var worldList = Dot.Array("worldlist", obj, null);
                    _gameWorlds.Clear();
                    if (worldList != null)
                    {
                        for (var i = 0; i < worldList.Count; i++)
                        {
                            var world = worldList[i];
                            _gameWorlds.Add(new GameWorld(world));
                        }
                    }
                    _gameWorlds.Sort(new WorldCompare());

                    var defaultWorld = _gameWorlds.Find(w => w.Default);
                    if (_user != null && defaultWorld != null && _user.WorldId == defaultWorld.Id)
                    {
                        LocalUser = System.Array.Find(_account.Users, u => u.Id == _user.Id);
                    }

                    // var notice = Dot.String("notice.content", obj, null);
                    Notices.Clear();
                    var notices = Dot.Array("notice.notices", obj, null);
                    if (notices != null)
                    {
                        for (var i = 0; i < notices.Count; i++)
                        {
                            var notice = notices[i];
                            Notices.Add(new NoticeItem(notice));
                        }
                    }
                    Notices.Sort((a,b) => { return (a.id.CompareTo(b.id)); });
                    if (_user != null)
                    {
                        Hashtable charData = Johny.HashtablePool.Claim();
                        charData["worldId"] = _user.WorldId;
                        EnterGame(charData);
                    }
                    else
                    {
                        LoginExtraListener.OnAuthorized(GetAuthenticator(authenticatorName), _account);
                    }
                });
            });
        }

        void _PreLogin(System.Action<Hashtable> callback)
        {
            _api.PreLogin(delegate (string err, object data) {
                if (!string.IsNullOrEmpty(err))
                {
                    var url = Dot.String("url", data, null);
                    if (!string.IsNullOrEmpty(url))
                    {
                        _UpdateRequired(url);
                        return;
                    }
                    else
                    {
                        _LoginFailed(err);
                        return;
                    }
                }

                var salt = Dot.String("salt", data, string.Empty);
                Coroutines.Run(Protect.CalculateHmac(salt, delegate (string challenge) {

                    var securityData = Johny.HashtablePool.Claim();
                    securityData["salt"] = salt;
                    securityData["chal"] = challenge;

                    callback(securityData);
                }));
            });
        }

        void _PostLogin(Hashtable reqData)
        {
            LogWithTime("_PostLogin=====>");
            LoadingLogic.AddCustomProgress(5);
            _api.LoginData(reqData, delegate (string err, Hashtable data) {
                LogWithTime("_api.LoginData=====>1");
                LoadingLogic.AddCustomProgress(5);
                if (!string.IsNullOrEmpty(err))
                {
                    _LoginFailed(err);
                    return;
                }

                LogWithTime("_api.LoginData=====>2");
                Hub.Instance.DataStore.LoginDataStore.LoginData = data ?? Hub.Instance.DataStore.LoginDataStore.LoginData;

                LogWithTime("_api.LoginData=====>3");
                var user = Dot.Object("user", data, null);
                if (user == null)
                {
                    return;
                }

                LogWithTime("_api.LoginData=====>4");
                LocalUser = Hub.Instance.UserManager.GetUser(user);
                _account.Add(_user);
                BugReport.AddData("uid", _user.Id.ToString());
                Hashtable configs = EB.Dot.Object("configs", Hub.Instance.DataStore.LoginDataStore.LoginData, null);
                Hub.Instance.DataStore.LoginDataStore.LoginData.Remove("configs");
                LogWithTime("_api.LoginData=====>5");

                //从服务器拉取表，解析表
                Hub.Instance.DataCacheManager.ProcessCaches(configs, error=>
                {
                    LogWithTime("<=====ProcessCaches");
                    if (!string.IsNullOrEmpty(error))
                    {
                        _LoginFailed(error);
                        return;
                    }
                    SetState(LoginState.LoggedIn);
                }, LoadingLogic.AddCustomProgress);
            });
        }

        IEnumerator _Authorize(bool silent, System.Action<string, object> callback)
        {
            var result = Johny.HashtablePool.Claim();
            for (var i = 0; i < _authenticators.Count; i++)
            {
                var authenticator = _authenticators[i];
                var done = false;
                var err = string.Empty;
                var data = default(object);
                EB.Debug.Log("当前请求验证的平台:{0}" , authenticator.Name);
                authenticator.Authenticate(silent, delegate (string authErr, object authData) {

                    EB.Debug.Log("authErr:{0},silent:{1}" , authErr , silent);
                    err = authErr;
                    data = authData;
                    done = true;
                });

                while (!done) yield return null;

                if (!string.IsNullOrEmpty(err))
                {
                    _LoginFailed(err);
                    yield break;
                }
                else if (data != null)
                {
                    result[authenticator.Name] = data;
                }
            }

            callback(null, result);
        }

        void _LoginFailed(string error)
        {
            EB.Debug.LogWarning("LoginManager: Login Failed!: {0}", error);
            LocalUser = null;
            Hub.Instance.Config.LoginConfig.Listener.OnLoginFailed(error);
            SetState(LoginState.Disconnected);
        }

        void _UpdateRequired(string url)
        {
            LocalUser = null;
            Hub.Instance.Config.LoginConfig.Listener.OnUpdateRequired(url);
            SetState(LoginState.Disconnected);
        }

        private bool _IsAuthSuppose(Hashtable auth, string name)
        {
            if (auth.Contains(name) && EB.Dot.Bool(name, auth, false))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        IEnumerator _PostInit(object data, System.Action callback)
        {
            EB.Debug.Log("Post init");

            //_geo = Dot.String("geo", data, _geo);
            Hashtable auth = Dot.Object("auth", data, null);

            var list = new List<Authenticator>();
            var empty = Johny.HashtablePool.Claim();
            for (var i = 0; i < Hub.Instance.Config.LoginConfig.Authenticators.Length; i++)
            {
                System.Type type = Hub.Instance.Config.LoginConfig.Authenticators[i];
                var authenticator = (Authenticator)System.Activator.CreateInstance(type);

                if (!_IsAuthSuppose(auth, authenticator.Name)) continue;

                var done = false;
                var err = string.Empty;
                var valid = false;

                authenticator.Init(Dot.Object(authenticator.Name, data, empty), delegate (string errAuth, bool validAuth) {
                    err = errAuth;
                    valid = validAuth;
                    done = true;
                });

                while (!done) yield return null;

                if (!string.IsNullOrEmpty(err))
                {
                    _LoginFailed(err);
                    yield break;
                }

                if (valid)
                {
                    EB.Debug.Log("LoginManager: Adding authenticator: {0}", authenticator.Name);
                    list.Add(authenticator);
                }
            }

            if (list.Count == 0)
            {
                EB.Debug.LogError("LoginManager: No valid authenticators found!");
                _LoginFailed("ID_SPARX_ERROR_UNKNOWN");
                yield break;
            }

            _authenticators = list;
            _initialized = true;
            SetState(LoginState.Initialized);

            yield return null;

            callback();
        }

        protected void SetState(LoginState state)
        {
            var prev = LoginState;
            LoginState = state;
            if (prev != state || state == LoginState.LoggedIn)
            {
                Hub.Instance.SendMessage("OnLoginStateChanged", state);
            }
        }

        public override void Async(string message, object payload)
        {
            switch (message.ToLower())
            {
                case "logout":
                    {
                        EB.Debug.LogError("Got forced logout from the server {0}", payload);
                        string error = "ID_SPARX_ERROR_UNKNOWN";
                        if (payload != null)
                        {
                            error = payload.ToString();
                        }
                        string errorStr= Localizer.GetString(error);
                        EB.Debug.LogError("SubSystem Error: {0}", errorStr);
                        Hub.Instance.FatalError(Localizer.GetString(errorStr));
                    }
                    break;
            }
        }
        
        public override void Disconnect(bool isLogout)
        {
            EB.Debug.Log("断开主系统isLogout：{0}" , isLogout);
            _account = null;
            LoginState = LoginState.Disconnected;

            if (isLogout)
            {
                LocalUser = null;
                _lastAuthenticator = string.Empty;
                PlayerPrefs.SetString("LastAuthenticator", string.Empty);

                if (_authData != null)
                {
                    foreach (DictionaryEntry entry in _authData)
                    {
                        var authenticator = GetAuthenticator(entry.Key as string);
                        EB.Debug.Log("authenticator：{0},IsLoggedIn:{1}", authenticator.Name , authenticator.IsLoggedIn);
                        if (authenticator.IsLoggedIn)
                        {
                            authenticator.Logout();
                        }
                    }
                    _authData = null;
                }
            }
        }

        public GameWorld GetDefaultGameWorld(int WorldId)
        {
            for (int i = 0; i < GameWorlds.Length; i++)
            {
                if (GameWorlds[i].Id==WorldId)
                {
                    return GameWorlds[i];
                }
            }

            return GameWorlds[0];
        }
    }
}