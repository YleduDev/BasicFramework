namespace  Framework
{
    using System.Collections;
    
    public interface IEnumeratorTask
    {
        IEnumerator DoLoadAsync(System.Action finishCallback);
    }
}