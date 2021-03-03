//CoreJSON
//项目主流JSON工具类
//Johny

using System;
using System.Collections;
using System.Text;
using Johny;

namespace EB
{	
	public static class JSON
	{
		public class Fragment
		{			
			public string JsonString { get; private set; }
			
			public Fragment(string json)
			{
				JsonString = json;
			}
		}

		static JSON()
		{
			Johny.JSONNodePool.Prepare(typeof(Johny.JSONObject), 0);
			Johny.JSONNodePool.Prepare(typeof(Johny.JSONArray), 0);
			Johny.JSONNodePool.Prepare(typeof(Johny.JSONString), 0);
			Johny.HashtablePool.Prepare(0);
			Johny.ArrayListPool.Prepare(0);
		}

		private static bool IsNumeric(object obj){
			return obj is int
					|| obj is long
					|| obj is float
					|| obj is double;
		}

		private static void JSONNode2Normal(JSONNode jnode, out object normal){
			normal = ParseValueSimple<ICollection>(string.Empty, jnode, null);
		}

		private static void RecycleJsonNode(JSONNode node){
			if(node == null){
				return;
			}
			if(node.IsArray){
				var root = node.AsArray;
				for(int i = 0; i < root.Count; i++)
				{
					RecycleJsonNode(root[i]);
				}
				Johny.JSONNodePool.Release(root);
			}
			else if(node.IsObject){
				var root = node.AsObject;
				foreach(var it in root.Children){
					RecycleJsonNode(it.Value);
				}
				Johny.JSONNodePool.Release(root);
			}
			else if(node.IsString){
				Johny.JSONNodePool.Release(node);
			}
		}

		#region 反序列化
	    /// <summary>
	    /// JSONNode解析后转为Normal
	    /// </summary>
	    /// <param name="json">A JSON string.</param>
	    /// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
	    public static object Parse(string json)//91ms
	    {
			object ret = null;

            Johny.JSONNode jnode = null;
            try
            {
                jnode = Johny.JSONNode.Parse(json);
            }
            catch (NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
                return null;
            }

			if(jnode != null){
				JSONNode2Normal(jnode, out ret);
			}

			return ret;
	    }
		#endregion

		#region 序列化
		private static StringBuilder st_StringfySB = new StringBuilder(2048);
		/// <summary>
		/// 将obj序列化为jsonstring
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string Stringify(object obj)
	    {
			JSONNode jnode;
			Object2JSONNode(obj, out jnode);
			st_StringfySB.Clear();
			jnode.ToJSONFormatString(st_StringfySB);
			RecycleJsonNode(jnode);
			return st_StringfySB.ToString();
	    }
		#endregion

		#region Normal -> JSONNode
		private static void Object2JSONNode(object obj, out JSONNode jnode){
			jnode = DoObject2JSONNode(string.Empty, obj, null);
		}

		private static bool DoObject2JSONNode_AddToParent(JSONNode parent, string key, JSONNode o) 
		{
			if(parent != null)
			{
				if(parent.IsObject){
					var parentCon = parent.AsObject;
					parentCon.Add(key, o);
				}
				else if(parent.IsArray){
					var parentCon = parent.AsArray;
					parentCon.Add(o);
				}
			}
			else
			{
				return false;
			}

			return true;
		}

		private static JSONNode DoObject2JSONNode(string key, object obj, JSONNode parent){
			if(obj == null){
			    JSONNode jnull = new JSONNull();
				if(!DoObject2JSONNode_AddToParent(parent, key, jnull)){
					return jnull;
				}
			}
			else if(obj is IDictionary){
				JSONObject jobject = Johny.JSONNodePool.Claim(typeof(Johny.JSONObject)).AsObject;
				var it = (obj as IDictionary).GetEnumerator();
				while(it.MoveNext()){
					DoObject2JSONNode(it.Key as string, it.Value, jobject);
				}

				if(!DoObject2JSONNode_AddToParent(parent, key, jobject)){
					return jobject;
				}
			}
			else if(obj is ICollection){
				JSONArray jarray = Johny.JSONNodePool.Claim(typeof(Johny.JSONArray)).AsArray;
				var it = (obj as ICollection).GetEnumerator();
				while(it.MoveNext()){
					DoObject2JSONNode(string.Empty, it.Current, jarray);
				}

				if(!DoObject2JSONNode_AddToParent(parent, key, jarray)){
					return jarray;
				}
			}
			else if(obj is string){
				JSONString jstring = Johny.JSONNodePool.Claim(typeof(Johny.JSONString)) as JSONString;
				jstring.InitString(obj as string);

				if(!DoObject2JSONNode_AddToParent(parent, key, jstring)){
					return jstring;
				}
			}
			else if(obj is bool){
				JSONBool jbool = new JSONBool((bool)obj);
				if(!DoObject2JSONNode_AddToParent(parent, key, jbool)){
					return jbool;
				}
			}
			else if(obj is System.Enum){
				JSONNumber jnum = new JSONNumber((int)obj);
				if(!DoObject2JSONNode_AddToParent(parent, key, jnum)){
					return jnum;
				}
			}
			else if(IsNumeric(obj))
			{
				double dd = Convert.ToDouble(obj);
				JSONNumber jnum = new JSONNumber(dd);
				if(!DoObject2JSONNode_AddToParent(parent, key, jnum)){
					return jnum;
				}
			}
			else
			{
				EB.Debug.LogError($"此类型JSON无法解析，请自行转换成基本类型！==> {obj.GetType().FullName}");
			}
			
			return null;
		}
		#endregion
	
		#region JSONNode -> Normal
		private static bool ParseValueSimple_AddToParent<T> (ICollection parent, string key, T o) 
		{
			if(parent != null){
				if(parent is Hashtable){
					var parentCon = parent as Hashtable;
					parentCon.Add(key, o);
				}
				else if(parent is ArrayList){
					var parentCon = parent as ArrayList;
					parentCon.Add(o);
				}
			}else
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// JSONNode转为Normal
		/// </summary>
		/// <param name="key"></param>
		/// <param name="jnode"></param>
		/// <param name="parent"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static object ParseValueSimple<T>(string key, Johny.JSONNode jnode, T parent) where T : ICollection
		{
			if(jnode.IsObject){
				#region parse object
				Hashtable ht = Johny.HashtablePool.Claim(Johny.HashtablePoolType.t16);
				var jobject = jnode.AsObject;
				foreach(var it in jobject.Children)
				{
					ParseValueSimple(it.Key, it.Value, ht);
				}
				Johny.JSONNodePool.Release(jobject);

				if(!ParseValueSimple_AddToParent<Hashtable>(parent, key, ht)){
					return ht;
				}
				#endregion
			}
			else if(jnode.IsArray){
				#region  parse array
				ArrayList al = Johny.ArrayListPool.Claim();
				var jarray = jnode.AsArray;
				for(int i = 0; i < jarray.Count; i++){
					ParseValueSimple(string.Empty, jarray[i], al);
				}
				Johny.JSONNodePool.Release(jarray);

				if(!ParseValueSimple_AddToParent<ArrayList>(parent, key, al)){
					return al;
				}
				#endregion
			}
			else if(jnode.IsString){
				#region  parse string
				string str = jnode.ToString();
				Johny.JSONNodePool.Release(jnode);

				if(!ParseValueSimple_AddToParent<string>(parent, key, str)){
					return str;
				}
				#endregion
			}
			else if(jnode.IsNumber){
				#region  parse string
				double num = jnode.AsDouble;

				if(!ParseValueSimple_AddToParent<double>(parent, key, num)){
					return num;
				}
				#endregion
			}
			else if(jnode.IsBoolean){
				#region  parse bool
				bool bb = jnode.AsBool;

				if(!ParseValueSimple_AddToParent<bool>(parent, key, bb)){
					return bb;
				}
				#endregion
			}
			else if(jnode.IsNull){
				if(!ParseValueSimple_AddToParent<Johny.JSONNode>(parent, key, null)){
					return null;
				}
			}

			return null;
		}
		#endregion
	}	
}