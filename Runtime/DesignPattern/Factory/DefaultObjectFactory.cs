/****************************************************
    文件：DefaultObjectFactory.cs
    日期：2020/11/9 18:2:11
	功能：Nothing
*****************************************************/

using UnityEngine;
namespace Framework
{
    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }
    }
}