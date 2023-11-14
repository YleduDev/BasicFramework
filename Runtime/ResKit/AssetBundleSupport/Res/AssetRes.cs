using System;

namespace  Framework
{
	using UnityEngine;
	using System.Collections;

	public class AssetRes : Res
	{
		protected string[]           mAssetBundleArray;
		protected AssetBundleRequest mAssetBundleRequest;
		protected string              mOwnerBundleName;

		public override string OwnerBundleName
		{
			get { return mOwnerBundleName; }
			set { mOwnerBundleName = value; }
		}
		
		public static AssetRes Allocate(string name, string onwerBundleName, Type assetTypde)
		{
			var res = SafeObjectPool<AssetRes>.Instance.Allocate();
			if (res != null)
			{
				res.AssetName = name;
				res.mOwnerBundleName = onwerBundleName;
				res.AssetType = assetTypde;
				res.InitAssetBundleName();
			}

			return res;
		}

		public string AssetBundleName
		{
			get { return mAssetBundleArray == null ? null : mAssetBundleArray[0]; }
		}

		public AssetRes(string assetName) : base(assetName)
		{

		}

		public AssetRes()
		{

		}

		public override bool LoadSync()
		{
			if (!CheckLoadAble())
			{
				return false;
			}

			if (string.IsNullOrEmpty(AssetBundleName))
			{
				return false;
			}


			Object obj = null;

			if (AssetBundlePathHelper.SimulationMode && !string.Equals(mAssetName, "assetbundlemanifest"))
			{
				var resSearchKeys = ResSearchKeys.Allocate(AssetBundleName,null,typeof(AssetBundle));

				var abR = ResMgr.Instance.GetRes<AssetBundleRes>(resSearchKeys);
				resSearchKeys.Recycle2Cache();

				var assetPaths =  AssetBundlePathHelper.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetName, mAssetName);
				if (assetPaths.Length == 0)
				{
					Log.E("Failed Load Asset:" + mAssetName);
					OnResLoadFaild();
					return false;
				}
				
				HoldDependRes();

				State = ResState.Loading;

				if (AssetType != null)
				{

					obj = AssetBundlePathHelper.LoadAssetAtPath(assetPaths[0],AssetType);
				}
				else
				{
					obj = AssetBundlePathHelper.LoadAssetAtPath<Object>(assetPaths[0]);
				}
			}
			else
			{
				var resSearchKeys = ResSearchKeys.Allocate(AssetBundleName, null, typeof(AssetBundle));
				var abR = ResMgr.Instance.GetRes<AssetBundleRes>(resSearchKeys);
				resSearchKeys.Recycle2Cache();

				
				if (abR == null || !abR.AssetBundle)
				{
 					Log.E("Failed to Load Asset, Not Find AssetBundle:" + AssetBundleName);
					return false;
				}
				
				HoldDependRes();

				State = ResState.Loading;

				if (AssetType != null)
				{
					obj = abR.AssetBundle.LoadAsset(mAssetName,AssetType);
				}
				else
				{
					obj = abR.AssetBundle.LoadAsset(mAssetName);
				}
			}

			UnHoldDependRes();

			if (obj == null)
			{
				Log.E("Failed Load Asset:" + mAssetName + ":" + AssetType + ":" + AssetBundleName);
				OnResLoadFaild();
				return false;
			}

			mAsset = obj;

			State = ResState.Ready;
			return true;
		}

		public override void LoadAsync()
		{
			if (!CheckLoadAble())
			{
				return;
			}

			if (string.IsNullOrEmpty(AssetBundleName))
			{
				return;
			}

			State = ResState.Loading;

			ResMgr.Instance.PushIEnumeratorTask(this);
		}

		public override IEnumerator DoLoadAsync(System.Action finishCallback)
		{
			if (RefCount <= 0)
			{
				OnResLoadFaild();
				finishCallback();
				yield break;
			}

			
            //Object obj = null;
            var resSearchKeys = ResSearchKeys.Allocate(AssetBundleName,null,typeof(AssetBundle));
            var abR = ResMgr.Instance.GetRes<AssetBundleRes>(resSearchKeys);
			resSearchKeys.Recycle2Cache();

			if (AssetBundlePathHelper.SimulationMode && !string.Equals(mAssetName, "assetbundlemanifest"))
			{
				var assetPaths = AssetBundlePathHelper.GetAssetPathsFromAssetBundleAndAssetName(abR.AssetName, mAssetName);
				if (assetPaths.Length == 0)
				{
					Log.E("Failed Load Asset:" + mAssetName);
					OnResLoadFaild();
					finishCallback();
					yield break;
				}

				//确保加载过程中依赖资源不被释放:目前只有AssetRes需要处理该情况
				HoldDependRes();
				State = ResState.Loading;

				// 模拟等一帧
				yield return new WaitForEndOfFrame();
				
				UnHoldDependRes();

				if (AssetType != null)
				{

					mAsset = AssetBundlePathHelper.LoadAssetAtPath(assetPaths[0],AssetType);
				}
				else
				{
					mAsset = AssetBundlePathHelper.LoadAssetAtPath<Object>(assetPaths[0]);
				}

			}
			else
			{
				
				if (abR == null || abR.AssetBundle == null)
				{
					Log.E("Failed to Load Asset, Not Find AssetBundleImage:" + AssetBundleName);
					OnResLoadFaild();
					finishCallback();
					yield break;
				}
				
				
				HoldDependRes();

				State = ResState.Loading;

				AssetBundleRequest abQ = null;
				
				if (AssetType != null)
				{
					abQ = abR.AssetBundle.LoadAssetAsync(mAssetName,AssetType);
					mAssetBundleRequest = abQ;

					yield return abQ;
				}
				else
				{
					abQ = abR.AssetBundle.LoadAssetAsync(mAssetName);
					mAssetBundleRequest = abQ;

					yield return abQ;
				}

				mAssetBundleRequest = null;

				UnHoldDependRes();

				if (!abQ.isDone)
				{
					Log.E("Failed Load Asset:" + mAssetName);
					OnResLoadFaild();
					finishCallback();
					yield break;
				}

				mAsset = abQ.asset;
			}

			State = ResState.Ready;

			finishCallback();
		}

		public override string[] GetDependResList()
		{
			return mAssetBundleArray;
		}

        public override bool IsDependResLoadFinish()
        {
			var depends = GetDependResList();
			if (depends == null || depends.Length == 0)
			{
				return true;
			}

			for (var i = depends.Length - 1; i >= 0; --i)
			{
				var resSearchRule = ResSearchKeys.Allocate(depends[i],null,typeof(AssetBundle));
				var res = ResMgr.Instance.GetRes(resSearchRule, false);
				resSearchRule.Recycle2Cache();

				if (res == null || res.State != ResState.Ready)
				{
					return false;
				}
			}

			return true;
		}

        public override void OnRecycled()
		{
			mAssetBundleArray = null;
		}

		public override void Recycle2Cache()
		{
			SafeObjectPool<AssetRes>.Instance.Recycle(this);
		}

		protected override float CalculateProgress()
		{
			if (mAssetBundleRequest == null)
			{
				return 0;
			}

			return mAssetBundleRequest.progress;
		}

		protected void InitAssetBundleName()
		{
			mAssetBundleArray = null;
            //此处应走配置表

            mAssetBundleArray = new string[1];
            mAssetBundleArray[0] = mOwnerBundleName;
        }

		public override string ToString()
		{
			return string.Format("Type:Asset\t {0}\t FromAssetBundle:{1}", base.ToString(), AssetBundleName);
		}
	}
}