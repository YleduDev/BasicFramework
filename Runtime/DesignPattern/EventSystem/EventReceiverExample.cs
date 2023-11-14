using Framework;
using UnityEngine;

namespace Framework
{
	#region 事件定义

	public enum TestEvent
	{
        Start= QMgrID.Game,
		TestOne,
		End,
	}

	public enum TestEventB
	{
		Start = TestEvent.End, // 为了保证每个消息 Id 唯一，需要头尾相接
		TestB,
		End,
	}

	#endregion 事件定义

	public class EventReceiverExample : MonoBehaviour
	{
		void Start()
		{
			QEventSystem.RegisterEvent(TestEvent.TestOne, OnEvent);
		}

		void OnEvent(int key, params object[] obj)
		{
			switch (key)
			{
				case (int) TestEvent.TestOne:
				 Debug.Log(obj[0]);
					break;
			}
		}

		private void OnDestroy()
		{
			QEventSystem.UnRegisterEvent(TestEvent.TestOne, OnEvent);
		}
	}
}