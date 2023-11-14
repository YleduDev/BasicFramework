/****************************************************
    文件：DictionaryPool.cs
    日期：2020/11/12 13:51:14
	功能：字典数据结构 池子 用于过度的New 情况下优化
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
namespace Framework
{
    public static class DictionaryPool<TKey,TValue>
    {
        //默认
        static Stack<Dictionary<TKey, TValue>> mDictionaryStack = new Stack<Dictionary<TKey, TValue>>(8);

        public static Dictionary<TKey, TValue> Get()
        {
            //木有缓存
            if (mDictionaryStack.Count == 0) return new Dictionary<TKey, TValue>(8);
            //有缓存
            return mDictionaryStack.Pop();
        }

        //回收
        public static void Release(Dictionary<TKey, TValue> toReleaseList)
        {
            if (toReleaseList == null) return;
            toReleaseList.Clear();
            mDictionaryStack.Push(toReleaseList);
        }

    }
    //方法拓展
    public static class DictionaryPoolExtensions
    {
        //回收方法拓展，可以说语法糖吧
        public static void Release2Pool<TKey, TValue>(this Dictionary<TKey, TValue> toRelease)
        {
            DictionaryPool<TKey, TValue>.Release(toRelease);
        }
    }
}