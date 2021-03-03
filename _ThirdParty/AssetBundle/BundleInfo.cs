using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GM
{
    /// <summary>
    /// 资源包信息数据结构
    /// </summary>
	public class BundleInfo
	{
        /// <summary>
        /// 资源包名称
        /// </summary>
		public string BundleName;
        /// <summary>
        /// short name only, no path, no extension
        /// 所有的资源名称列表
        /// </summary>
		public List<string> Includes;
        /// <summary>
        /// full path without extension like usage in Resources.Load
        /// 所有的资源路径列表
        /// </summary>
		public List<string> Paths;	
        /// <summary>
        /// 版本号
        /// </summary>
		public int Version;
        /// <summary>
        /// MD5码
        /// </summary>
		public string MD5;
        /// <summary>
        /// 父级名称 
        /// </summary>
		public string Parent;
        /// <summary>
        /// 资源包大小
        /// </summary>
		public long Size;
        /// <summary>
        /// 优先级
        /// </summary>
		public int Priority;
	}
}
