﻿#if UNITY_EDITOR || UNITY_ANDROID
using UnityEngine;

namespace ClawbearGames
{
	public class NSShareResultCallbackAndroid : AndroidJavaProxy
	{
		private readonly NSCallbackHelper callbackHelper;

		public NSShareResultCallbackAndroid( NativeShare.ShareResultCallback callback ) : base( "com.yasirkula.unity.NativeShareResultReceiver" )
		{
			callbackHelper = new GameObject( "NSCallbackHelper" ).AddComponent<NSCallbackHelper>();
			callbackHelper.callback = callback;
		}

		[UnityEngine.Scripting.Preserve]
		public void OnShareCompleted( int result, string shareTarget )
		{
			if( !callbackHelper )
			{
				Debug.LogWarning( "NSCallbackHelper is destroyed!" );
				return;
			}

			callbackHelper.OnShareCompleted( result, shareTarget );
		}

		[UnityEngine.Scripting.Preserve]
		public bool HasManagedCallback()
		{
			return callbackHelper && callbackHelper.callback != null;
		}
	}
}
#endif