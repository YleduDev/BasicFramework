/****************************************************
    文件：IPoolable.cs
    日期：2020/11/9 16:30:37
	功能：Pool able
*****************************************************/

using UnityEngine;
namespace Framework
{
    public interface IPoolable
    {
        //回收 实现
        void OnRecycled();

        //回收状态
        bool IsRecycled { set; get; }
    }
}