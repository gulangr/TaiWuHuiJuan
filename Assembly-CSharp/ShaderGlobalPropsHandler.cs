using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
[ExecuteAlways]
[DisallowMultipleComponent]
[AddComponentMenu("")]
public class ShaderGlobalPropsHandler : MonoBehaviour
{
	// Token: 0x0600010C RID: 268 RVA: 0x00007FAC File Offset: 0x000061AC
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void TryInit()
	{
		bool flag = ShaderGlobalPropsHandler._instance == null;
		if (flag)
		{
			GameObject go = new GameObject("ShaderGlobalPropsHandler")
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			ShaderGlobalPropsHandler._instance = go.AddComponent<ShaderGlobalPropsHandler>();
		}
		bool isPlaying = Application.isPlaying;
		if (isPlaying)
		{
			Object.DontDestroyOnLoad(ShaderGlobalPropsHandler._instance.gameObject);
		}
	}

	// Token: 0x0600010D RID: 269 RVA: 0x00008008 File Offset: 0x00006208
	private void Update()
	{
		float t = Time.unscaledTime;
		Shader.SetGlobalVector(ShaderGlobalPropsHandler.UnscaledTimeID, new Vector4(t * 0.05f, t, t * 2f, t * 3f));
	}

	// Token: 0x0600010E RID: 270 RVA: 0x00008044 File Offset: 0x00006244
	private void OnDestroy()
	{
		bool flag = ShaderGlobalPropsHandler._instance == this;
		if (flag)
		{
			ShaderGlobalPropsHandler._instance = null;
		}
	}

	// Token: 0x040000AC RID: 172
	private static readonly int UnscaledTimeID = Shader.PropertyToID("_UnscaledTime");

	// Token: 0x040000AD RID: 173
	private static ShaderGlobalPropsHandler _instance;
}
