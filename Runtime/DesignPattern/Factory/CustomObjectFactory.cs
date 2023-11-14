/****************************************************
    文件：CustomObjectFactory.cs
    日期：2020/11/26 11:36:56
	功能：自定义对象工厂派生
*****************************************************/

using System;
using UnityEngine;
namespace Framework
{
    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        public CustomObjectFactory(Func<T> factoryMethod)
        {
            this.factoryMethod = factoryMethod;
        }

        protected Func<T> factoryMethod;

        public T Create()
        {
            return factoryMethod();
        }
    }
}