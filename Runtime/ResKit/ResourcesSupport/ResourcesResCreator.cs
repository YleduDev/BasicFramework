namespace  Framework
{
    public class ResourcesResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return IsResPath (resSearchKeys.AssetName);
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            var resourcesRes = ResourcesRes.Allocate(resSearchKeys.AssetName,
                resSearchKeys.AssetName.StartsWith("resources://")
                    ? InternalResNamePrefixType.Url
                    : InternalResNamePrefixType.Folder, resSearchKeys.OriginalAssetName);
            resourcesRes.AssetType = resSearchKeys.AssetType;
            return resourcesRes;
        }
        public static bool IsResPath( string name)
        {
            return name.ToLower().StartsWith("resources/") ||
                   name.StartsWith("resources://");
        }
    }
}