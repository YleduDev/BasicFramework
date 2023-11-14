using System.Collections.Generic;
using System.Linq;

namespace  Framework
{
    public static class ResFactory
    {
        public static AssetBundleSceneResCreator AssetBundleSceneResCreator = new AssetBundleSceneResCreator();

        public static IRes Create(ResSearchKeys resSearchKeys)
        {
            var retRes = mResCreators
                .Where(creator => creator.Match(resSearchKeys))
                .Select(creator => creator.Create(resSearchKeys))
                .FirstOrDefault();

            if (retRes == null)
            {
                Log.E("Failed to Create Res. Not Find By ResSearchKeys:" + resSearchKeys);
                return null;
            }

            return retRes;
        }

        public static void AddResCreator<T>() where T : IResCreator, new()
        {
            mResCreators.Add(new T());
        }

        public static void RemoveResCreator<T>() where T : IResCreator, new()
        {
            mResCreators.RemoveAll(r => r.GetType() == typeof(T));
        }

        public static void AddResCreator(IResCreator resCreator)
        {
            mResCreators.Add(resCreator);
        }

        public  static List<IResCreator> mResCreators = new List<IResCreator>()
        {
            new ResourcesResCreator(),
            new LocalAudioResCreator(),
              new NetAudioResCreator(),
            new AssetBundleResCreator(),
            new AssetResCreator(),
            AssetBundleSceneResCreator,
            new NetImageResCreator(),
            new LocalImageResCreator()          
        };
    }

    public class NetImageResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetName.StartsWith("netimage:") || resSearchKeys.resLoadType == ResLoadType.NetImageRes;
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return NetImageRes.Allocate(resSearchKeys.AssetName,resSearchKeys.OriginalAssetName);
        }
    }

    public class LocalImageResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetName.StartsWith("localimage:")|| resSearchKeys.resLoadType== ResLoadType.LocalImageRes;
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return LocalImageRes.Allocate(resSearchKeys.AssetName, resSearchKeys.OriginalAssetName);
        }
    }
}