/****************************************************
    文件：SafeObjectPoolExample.cs
    日期：2020/11/26 11:46:23
	功能：Nothing
*****************************************************/

using UnityEngine;

namespace Framework.Example
{
    class Msg : IPoolable, IPoolType
    {
        #region IPoolAble 实现

        public void OnRecycled()
        {
            Debug.Log("OnRecycled");
        }

        public bool IsRecycled { get; set; }

        #endregion


        #region IPoolType 实现

        public static Msg Allocate()
        {
            return SafeObjectPool<Msg>.Instance.Allocate();
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<Msg>.Instance.Recycle(this);
        }

        #endregion
    }

    public class SafeObjectPoolExample : MonoBehaviour 
    {
        private void Start()
        {
            SafeObjectPool<Msg>.Instance.Init(100, 50);

            Debug.Log(string.Format( "msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount));

            var fishOne = Msg.Allocate();

            Debug.Log(string.Format("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount));

            fishOne.Recycle2Cache();

            Debug.Log(string.Format("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount));

            for (int i = 0; i < 10; i++)
            {
                Msg.Allocate();
            }

            Debug.Log(string.Format("msgPool.CurCount:{0}", SafeObjectPool<Msg>.Instance.CurCount));
        }
    }
}