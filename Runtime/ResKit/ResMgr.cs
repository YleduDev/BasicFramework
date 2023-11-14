using System.Collections;
using System.Linq;

namespace  Framework
{
    using System.Collections.Generic;
    using UnityEngine;

    [MonoSingletonPath("[Framework]/ResMgr")]
    public class ResMgr : MonoSingleton<ResMgr>
    {
        #region ID:RKRM001 Init v0.1.0 Unity5.5.1p4

        private static bool mResMgrInited = false;
        
        /// <summary>
        /// 初始化bin文件
        /// </summary>
        public static void Init()
        {
            if (mResMgrInited) return;
            mResMgrInited = true;

            SafeObjectPool<AssetBundleRes>.Instance.Init(40, 20);
            SafeObjectPool<AssetRes>.Instance.Init(40, 20);
            SafeObjectPool<ResourcesRes>.Instance.Init(40, 20);
            SafeObjectPool<NetImageRes>.Instance.Init(40, 20);
            SafeObjectPool<ResSearchKeys>.Instance.Init(40, 20);
            SafeObjectPool<ResLoader>.Instance.Init(100, 40);
            
        }


        public static IEnumerator InitAsync()
        {
            if (mResMgrInited) yield break;
            mResMgrInited = true;

            SafeObjectPool<AssetBundleRes>.Instance.Init(40, 20);
            SafeObjectPool<AssetRes>.Instance.Init(40, 20);
            SafeObjectPool<ResourcesRes>.Instance.Init(40, 20);
            SafeObjectPool<NetImageRes>.Instance.Init(40, 20);
            SafeObjectPool<ResSearchKeys>.Instance.Init(40, 20);
            SafeObjectPool<ResLoader>.Instance.Init(40, 20);
            
        }

        #endregion

        public int Count
        {
            get { return mTable.Count(); }
        }

        #region 字段

        private ResTable mTable = new ResTable();

        [SerializeField] private int mCurrentCoroutineCount;
        private int mMaxCoroutineCount = 6; //最快协成大概在6到8之间
        private LinkedList<IEnumeratorTask> mIEnumeratorTaskStack = new LinkedList<IEnumeratorTask>();

        //Res 在ResMgr中 删除的问题，ResMgr定时收集列表中的Res然后删除
        private bool mIsResMapDirty;

        #endregion

        #region 属性

        public void ClearOnUpdate()
        {
            mIsResMapDirty = true;
        }

        public void PushIEnumeratorTask(IEnumeratorTask task)
        {
            if (task == null)
            {
                return;
            }

            mIEnumeratorTaskStack.AddLast(task);
            TryStartNextIEnumeratorTask();
        }


        public IRes GetRes(ResSearchKeys resSearchKeys, bool createNew = false)
        {
            var res = mTable.GetResBySearchKeys(resSearchKeys);

            if (res != null)
            {
                return res;
            }

            if (!createNew)
            {
                Log.I("createNew:{0}", createNew);
                return null;
            }

            res = ResFactory.Create(resSearchKeys);

            if (res != null)
            {
                mTable.Add(res);
            }

            return res;
        }

        public T GetRes<T>(ResSearchKeys resSearchKeys) where T : class, IRes
        {
            return GetRes(resSearchKeys) as T;
        }

        #endregion

        #region Private Func

        private void Update()
        {
            if (mIsResMapDirty)
            {
                RemoveUnusedRes();
            }
        }

        private void RemoveUnusedRes()
        {
            if (!mIsResMapDirty)
            {
                return;
            }

            mIsResMapDirty = false;

            foreach (var res in mTable.ToArray())
            {
                if (res.RefCount <= 0 && res.State != ResState.Loading)
                {
                    if (res.ReleaseRes())
                    {
                        mTable.Remove(res);

                        res.Recycle2Cache();
                    }
                }
            }
        }

        //private void OnGUI()
        //{
        //    if (PlatformFromUnityToDll.IsEditor && Input.GetKeyDown(KeyCode.F1))
        //    {
        //        GUILayout.BeginVertical("box");

        //        GUILayout.Label("ResKit", new GUIStyle {fontSize = 30});
        //        GUILayout.Space(10);
        //        GUILayout.Label("ResInfo", new GUIStyle {fontSize = 20});
        //        mTable.ToList().ForEach(res => { GUILayout.Label((res as Res).ToString()); });
        //        GUILayout.Space(10);

        //        GUILayout.Label("Pools", new GUIStyle() {fontSize = 20});
        //        GUILayout.Label(string.Format("ResSearchRule:{0}",
        //            SafeObjectPool<ResSearchKeys>.Instance.CurCount));
        //        GUILayout.Label(string.Format("ResLoader:{0}",
        //            SafeObjectPool<ResLoader>.Instance.CurCount));
        //        GUILayout.EndVertical();
        //    }
        //}
#if UNITY_EDITOR
        private void OnGUI()
        {
            if ( Input.GetKey(KeyCode.F1))
            {
                GUILayout.BeginVertical("box");

                GUILayout.Label("ResKit", new GUIStyle { fontSize = 30 });
                GUILayout.Space(10);
                GUILayout.Label("ResInfo", new GUIStyle { fontSize = 20 });
                mTable.ToList().ForEach(res => { GUILayout.Label((res as Res).ToString()); });
                GUILayout.Space(10);

                GUILayout.Label("Pools", new GUIStyle() { fontSize = 20 });
                GUILayout.Label(string.Format("ResSearchRule:{0}",
                    SafeObjectPool<ResSearchKeys>.Instance.CurCount));
                GUILayout.Label(string.Format("ResLoader:{0}",
                    SafeObjectPool<ResLoader>.Instance.CurCount));
                GUILayout.EndVertical();
            }
        }
#endif
        private void OnIEnumeratorTaskFinish()
        {
            --mCurrentCoroutineCount;
            TryStartNextIEnumeratorTask();
        }

        private void TryStartNextIEnumeratorTask()
        {
            if (mIEnumeratorTaskStack.Count == 0)
            {
                return;
            }

            if (mCurrentCoroutineCount >= mMaxCoroutineCount)
            {
                return;
            }

            var task = mIEnumeratorTaskStack.First.Value;
            mIEnumeratorTaskStack.RemoveFirst();

            ++mCurrentCoroutineCount;
            StartCoroutine(task.DoLoadAsync(OnIEnumeratorTaskFinish));
        }

        #endregion

        
    }
}