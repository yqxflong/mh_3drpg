using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	//public class AuthData
	//{
	//	public Authenticator Authenticator {get;private set;}
	//	public string Id { get; private set; }
	//	public object Data { get; private set; }

	//	public AuthData(string name, object obj)
	//	{
	//		Authenticator = Hub.Instance.LoginManager.GetAuthenticator(name);
	//		Id = Dot.String("id", obj, string.Empty);
	//		Data = Dot.Object("data", obj, null);
	//	}
	//}

	//public class Account
	//{
	//	public long AccountId				{get; private set;}
	//	public User[] Users 				{get; private set;}
	//	public AuthData[] Authenticators 	{get; private set;}

	//	public Account( object obj )
	//	{
	//		AccountId = Dot.Long("naid", obj, AccountId);

	//		ArrayList users = Dot.Array("users", obj, null);
	//		if (users != null)
	//		{
	//			Users = new User[users.Count];
	//			for (int i = 0; i < Users.Length; ++i)
	//			{
	//				Users[i] = Hub.Instance.UserManager.GetUser(users[i] as Hashtable);
	//			}
	//		}

	//		var auth = Dot.Object("auth", obj, null);
	//		if (auth != null)
	//		{
	//			var list = new List<AuthData>();
	//			foreach( DictionaryEntry entry in auth)
	//			{
	//				var authData = new AuthData(entry.Key.ToString(), entry.Value);
	//				list.Add(authData);
	//			}
	//			Authenticators = list.ToArray();
	//		}
	//	}

	//	public void Remove(User user)
	//	{
	//		Users = System.Array.FindAll(Users, u => u.Id != user.Id);
	//	}

	//	public void Add(User user)
	//	{
	//		int index = System.Array.FindIndex(Users, u => u.Id == user.Id);
	//		if (index < 0)
	//		{
	//			List<User> list = new List<User>(Users);
	//			list.Add(user);
	//			Users = list.ToArray();
	//		}
	//		else if (user != Users[index])
	//		{
	//			Users[index] = user;
	//		}
	//	}

	//	public void Remove(AuthData auth)
	//	{
	//		Authenticators = System.Array.FindAll(Authenticators, a => a.Id != auth.Id);
	//	}

	//	public void Add(AuthData auth)
	//	{
	//		int index = System.Array.FindIndex(Authenticators, a => a.Id == auth.Id);
	//		if (index < 0)
	//		{
	//			List<AuthData> list = new List<AuthData>(Authenticators);
	//			list.Add(auth);
	//			Authenticators = list.ToArray();
	//		}
	//		else if (auth != Authenticators[index])
	//		{
	//			Authenticators[index] = auth;
	//		}
	//	}
	//}
}
