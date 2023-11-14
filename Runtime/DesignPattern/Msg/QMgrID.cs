using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework{
    /// <summary>/// message Span/// </summary>
    public class QMsgSpan
    {
        public const int Count = 3000;
    }

    public abstract class QMgrID {

        public const int Framework = 0;
        public const int UI = Framework + QMsgSpan.Count; // 3000
        public const int Audio = UI + QMsgSpan.Count; // 6000
        public const int Network = Audio + QMsgSpan.Count;
        public const int UIFilter = Network + QMsgSpan.Count;
        public const int Game = UIFilter + QMsgSpan.Count;
        //拓展
        //public const int PCConnectMobile = Game + QMsgSpan.Count;
        //public const int FrameworkEnded = PCConnectMobile + QMsgSpan.Count;
        //public const int FrameworkMsgModuleCount = 7;
    }

    public class XMgrID: QMgrID
    {

    }
}
