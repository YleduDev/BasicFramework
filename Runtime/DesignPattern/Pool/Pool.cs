/****************************************************
    文件：Pool.cs
    日期：2020/11/9 17:41:6
	功能：抽象 池 基类,完成池 基本逻辑,可在此基础上 派生自定义池子
*****************************************************/

using System.Collections.Generic;
namespace Framework
{
    public abstract class Pool<T> : IPool<T>
    {
        //缓存队列（先进后出）
        protected readonly Stack<T> mCacheStack = new Stack<T>();

        //抽象工厂模式对象 
        protected IObjectFactory<T> mFactory;

        public int CurCount
        {
            get
            {  
                return mCacheStack.Count;
            }
        }
        //默认最大计数
        protected int mMaxCount = 5;

        public abstract bool Recycle(T obj); 

        public virtual T Allocate()
        {
            return mCacheStack.Count == 0 ? mFactory.Create() : mCacheStack.Pop();
        }
    
    }
}