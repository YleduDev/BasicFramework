
namespace Assets.Scripts
{
    public interface IMGUILayout : IMGUIView
    {
        IMGUILayout AddChild(IMGUIView view);

        void RemoveChild(IMGUIView view);

        void Clear();
    }
}