using EB;
using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class GameWorld
    {
        public const int RECOMMEND = 1;
        public enum eState
        {
            Smooth = 0,
            Busy = 1,
            Hot = 2,
            Down = 3,
            New = 4
        }
        public int Id { get; set; }
        public eState State { get; set; }
        public string Name { get; set; }
        public bool Default { get; set; }
        public int P1 { get; set; }
        public int OpenTime { get; set; }
        public GameWorld(object world)
        {
            Id = Dot.Integer("WID", world, Id);
            State = (eState)EB.Dot.Integer("State", world, (int)State);
            Name = Dot.String("WName", world, Name);
            Default = Dot.Bool("Default", world, Default);
            P1 = Dot.Integer("P1", world, P1);
            OpenTime = Dot.Integer("openTime", world, OpenTime);

        }
    }

    public class AuthData
    {
        public Authenticator Authenticator { get; private set; }
        public string Id { get; private set; }
        public object Data { get; private set; }

        public AuthData(string name, object obj)
        {
            Authenticator = LoginManager.Instance.GetAuthenticator(name);
            Id = Dot.String("id", obj, string.Empty);
            Data = Dot.Object("data", obj, null);
        }
    }

    public class Account
    {
        public long AccountId { get; private set; }
        public User[] Users { get; private set; }
        public AuthData[] Authenticators { get; private set; }

        public Account(object obj)
        {
            AccountId = Dot.Long("naid", obj, AccountId);

            ArrayList users = Dot.Array("users", obj, null);
            if (users != null)
            {
                Users = new User[users.Count];
                for (int i = 0; i < Users.Length; ++i)
                {
                    Users[i] = Hub.Instance.UserManager.GetUser(users[i] as Hashtable);
                }
            }

            var auth = Dot.Object("auth", obj, null);
            if (auth != null)
            {
                var list = new List<AuthData>();
                foreach (DictionaryEntry entry in auth)
                {
                    var authData = new AuthData(entry.Key.ToString(), entry.Value);
                    list.Add(authData);
                }
                Authenticators = list.ToArray();
            }
        }

        public void Remove(User user)
        {
            Users = System.Array.FindAll(Users, u => u.Id != user.Id);
        }

        public void Add(User user)
        {
            int index = System.Array.FindIndex(Users, u => u.Id == user.Id);
            if (index < 0)
            {
                List<User> list = new List<User>(Users);
                list.Add(user);
                Users = list.ToArray();
            }
            else if (user != Users[index])
            {
                Users[index] = user;
            }
        }

        public void Remove(AuthData auth)
        {
            Authenticators = System.Array.FindAll(Authenticators, a => a.Id != auth.Id);
        }

        public void Add(AuthData auth)
        {
            int index = System.Array.FindIndex(Authenticators, a => a.Id == auth.Id);
            if (index < 0)
            {
                List<AuthData> list = new List<AuthData>(Authenticators);
                list.Add(auth);
                Authenticators = list.ToArray();
            }
            else if (auth != Authenticators[index])
            {
                Authenticators[index] = auth;
            }
        }
    }
}
