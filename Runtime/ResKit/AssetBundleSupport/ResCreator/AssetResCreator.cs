namespace  Framework
{
    public class AssetResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.resLoadType== ResLoadType.ABAsset;
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return AssetRes.Allocate(resSearchKeys.AssetName, resSearchKeys.OwnerBundle, resSearchKeys.AssetType);
        }
    }
}