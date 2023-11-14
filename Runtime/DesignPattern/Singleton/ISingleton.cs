/****************************************************
    文件：ISingleton.cs
    日期：2020/11/9 18:11:59
	功能：考虑到单例的继承 ,和自定义 都需要一个初始化 接口,这里目前统一
*****************************************************/

using UnityEngine;
namespace Framework
{
    public interface ISingleton  
    {
        /// <summary>
        /// 单例 初始化方法
        /// </summary>
        void OnSingletonInit();
    }
}