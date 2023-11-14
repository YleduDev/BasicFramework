/****************************************************
    文件：IPool.cs
    日期：2020/11/9 16:26:45
	功能：Pool 接口base
*****************************************************/

using UnityEngine;
namespace Framework
{
    public interface IPool<T>
    {
        //分配
        T Allocate();

        //回收
        bool Recycle(T obj);
    }
}