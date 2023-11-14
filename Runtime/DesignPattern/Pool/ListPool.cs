/****************************************************
    文件：ListPool.cs
    日期：2020/11/12 13:40:49
	功能：List 泛型池子 用于过度的New 情况下优化
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
namespace Framework
{
    public static class ListPool<T> 
    {
        //默认
        static Stack<List<T>> mListStack = new Stack<List<T>>(8);

        public static List<T> Get()
        {
            //木有缓存
            if (mListStack.Count == 0)
                return new List<T>(8);
            //有缓存
            return mListStack.Pop();
        }

        //回收
        public static void Release(List<T> toReleaseList)
        {
            if (toReleaseList == null) return;
            toReleaseList.Clear();
            mListStack.Push(toReleaseList);
        }

    }

    //方法拓展
    public static class ListPoolExtensions
    {
        //回收方法拓展，可以说语法糖吧
        public static void Release2Pool<T>(this List<T> toRelease)
        {
            ListPool<T>.Release(toRelease);
        }
    }
}