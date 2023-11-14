/****************************************************
    文件：IObjectFactory.cs
    日期：2020/11/9 17:47:22
	功能：作为 抽象工厂的基类
*****************************************************/

using UnityEngine;
namespace Framework
{
    public interface IObjectFactory <T>
    {
        T Create();
    }
}