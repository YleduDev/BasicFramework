/****************************************************
    文件：Log.cs
    日期：2021/1/8 15:10:29
	功能：打印管理类
*****************************************************/

using System;
using UnityEngine;

/// <summary>
/// 打印的级别
/// </summary>
public enum LogLevel
{
    //任何都不
    None = 0,
    //异常
    Exception = 1,
    //错误
    Error = 2,
    //警告
    Warning = 3,
    //默认
    Normal = 4,
    //最高级别 都打印
    Max = 5,
}

/// <summary>
/// 打印工具
/// </summary>
public static class Log
{

    private static string HEAD_LOG = "一帧 ---- >  ";

    public static void LogInfo(this object selfMsg)
    {
        I(selfMsg);
    }

    public static void LogWarning(this object selfMsg)
    {
        W(selfMsg);
    }

    public static void LogError(this object selfMsg)
    {
        E(selfMsg);
    }

    public static void LogException(this Exception selfExp)
    {
        E(selfExp);
    }

    /// <summary>
    /// 设置的输出级别，后期上线后其他情况，可以设置此处，减少debug 消耗
    /// </summary>
    private static LogLevel mLogLevel =  LogLevel.Normal ;

    public static LogLevel Level
    {
        get { return mLogLevel; }
        set { mLogLevel = value; }
    }
     
    public static void I(object msg, params object[] args)
    {
        if (mLogLevel < LogLevel.Normal)
        {
            return;
        }

        if (args == null || args.Length == 0)
        {
            Debug.Log(HEAD_LOG + msg);
        }
        else
        {
            Debug.LogFormat(HEAD_LOG + msg.ToString(), args);
        }
    }
    public static void I(object msg, Color color, params object[] args)
    {
        if (mLogLevel < LogLevel.Normal)
        {
            return;
        }

        if (args == null || args.Length == 0)
        {
            Debug.Log(HEAD_LOG + msg);
        }
        else
        {
            Debug.LogFormat(HEAD_LOG + msg.ToString().SetColor(color), args);
        }
    }

    public static void E(Exception e)
    {
        if (mLogLevel < LogLevel.Exception)
        {
            return;
        }
        Debug.LogException(e);
    }

    public static void E(object msg, params object[] args)
    {
        if (mLogLevel < LogLevel.Error)
        {
            return;
        }

        if (args == null || args.Length == 0)
        {
            Debug.LogError(HEAD_LOG + msg);
        }
        else
        {

            Debug.LogErrorFormat(HEAD_LOG + string.Format(msg.ToString(), args));
        }

    }

    public static void W(object msg)
    {
        if (mLogLevel < LogLevel.Warning)
        {
            return;
        }

        Debug.LogWarning(HEAD_LOG + msg);
    }

    public static void W(string msg, params object[] args)
    {
        if (mLogLevel < LogLevel.Warning)
        {
            return;
        }

        Debug.LogWarning(HEAD_LOG + string.Format(msg, args));
    }
}
