namespace Framework
{
    public class AssetBundleSceneResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            if (resSearchKeys != null)
            {
                return resSearchKeys.resLoadType == ResLoadType.ABScene;
            }

            return false;
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            var resourcesRes = AssetBundleSceneRes.Allocate(resSearchKeys.AssetName, resSearchKeys.OwnerBundle);
            resourcesRes.AssetType = resSearchKeys.AssetType;
            return resourcesRes;
        }
    }
}