

using UnityEngine;

public class AndroidCallNative 
{
    private AndroidJavaObject obj;

    public AndroidCallNative(string calssName) {
        obj = new AndroidJavaObject(calssName);
    }


#if UNITY_ANDROID
    public  T CallMethod<T>(string methodName, params object[] methodParams)
    {
        T result = IsAvailable(methodParams) ? obj.Call<T>(methodName, methodParams) : obj.Call<T>(methodName);
        return result;
    }
#endif

#if UNITY_ANDROID
    public  T CallStaticMethod<T>( string methodName, params object[] methodParams)
    {
        T result = IsAvailable(methodParams) ? obj.CallStatic<T>(methodName, methodParams) : obj.CallStatic<T>(methodName);
        return result;
    }
#endif

#if UNITY_ANDROID 
    public  void CallMethod( string methodName, params object[] methodParams)
    {
        if (IsAvailable(methodParams))
        {
            obj.Call(methodName, methodParams);
        }
        else
        {
            obj.Call(methodName);
        }
    }
#endif
    public  bool IsAvailable(object[] param)
    {
        return param != null && param.Length > 0;
    }

    public void OnDestory() {
        obj.Dispose();
    }
}
