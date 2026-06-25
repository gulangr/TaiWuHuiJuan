using System;
using System.Collections.Generic;
using PostProcessGlow;
using UnityEngine;

// Token: 0x020002F0 RID: 752
public class GlowTargetController : MonoBehaviour
{
	// Token: 0x170004D7 RID: 1239
	// (get) Token: 0x06002C0E RID: 11278 RVA: 0x00158DDB File Offset: 0x00156FDB
	public bool HasGlowTargets
	{
		get
		{
			return this._glowCopies.Count > 0;
		}
	}

	// Token: 0x170004D8 RID: 1240
	// (get) Token: 0x06002C0F RID: 11279 RVA: 0x00158DEB File Offset: 0x00156FEB
	// (set) Token: 0x06002C10 RID: 11280 RVA: 0x00158DF2 File Offset: 0x00156FF2
	public static GlowTargetController Instance { get; private set; }

	// Token: 0x06002C11 RID: 11281 RVA: 0x00158DFA File Offset: 0x00156FFA
	private void Awake()
	{
		this.InitSingleton();
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x06002C12 RID: 11282 RVA: 0x00158E0C File Offset: 0x0015700C
	private void InitSingleton()
	{
		bool flag = GlowTargetController.Instance != null;
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			GlowTargetController.Instance = this;
		}
	}

	// Token: 0x06002C13 RID: 11283 RVA: 0x00158E40 File Offset: 0x00157040
	public void Register(IGlowSource glowSource)
	{
		bool flag = glowSource.Transform.parent == this.copyParent;
		if (!flag)
		{
			bool flag2 = this._glowCopies.ContainsKey(glowSource);
			if (!flag2)
			{
				this.CreateCopy(glowSource);
			}
		}
	}

	// Token: 0x06002C14 RID: 11284 RVA: 0x00158E84 File Offset: 0x00157084
	public void Unregister(IGlowSource glowSource)
	{
		IGlowCopy copy;
		bool flag = !this._glowCopies.Remove(glowSource, out copy);
		if (flag)
		{
			Debug.LogWarning("GlowSource " + glowSource.Transform.name + " not found in copies.");
		}
		else
		{
			bool flag2 = (copy != null) ? copy.GameObject : null;
			if (flag2)
			{
				Object.Destroy(copy.GameObject);
			}
		}
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x00158EEC File Offset: 0x001570EC
	private void CreateCopy(IGlowSource glowSource)
	{
		IGlowCopy copy = glowSource.MakeCopy(this.copyParent);
		bool flag = copy == null;
		if (flag)
		{
			Debug.LogError("Failed to create copy for " + glowSource.Transform.name);
		}
		else
		{
			this._glowCopies[glowSource] = copy;
		}
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x00158F3B File Offset: 0x0015713B
	private void Update()
	{
		this.SyncObjects();
	}

	// Token: 0x06002C17 RID: 11287 RVA: 0x00158F48 File Offset: 0x00157148
	private void SyncObjects()
	{
		foreach (KeyValuePair<IGlowSource, IGlowCopy> keyValuePair in this._glowCopies)
		{
			IGlowSource glowSource;
			IGlowCopy glowCopy;
			keyValuePair.Deconstruct(out glowSource, out glowCopy);
			IGlowSource source = glowSource;
			IGlowCopy copy = glowCopy;
			bool flag = source.Transform == null || copy.Transform == null;
			if (flag)
			{
				Debug.LogWarning("Source or copy Transform is null for " + source.Transform.name);
			}
			else
			{
				RectTransform copyRect = copy.Transform as RectTransform;
				RectTransform sourceRect;
				bool flag2;
				if (copyRect != null)
				{
					sourceRect = (source.Transform as RectTransform);
					flag2 = (sourceRect != null);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					copyRect.pivot = sourceRect.pivot;
					copyRect.anchorMin = sourceRect.anchorMin;
					copyRect.anchorMax = sourceRect.anchorMax;
					copyRect.sizeDelta = sourceRect.sizeDelta;
				}
				copy.Transform.position = source.Transform.position;
				copy.Transform.rotation = source.Transform.rotation;
				copy.Transform.localScale = source.Transform.localScale;
				copy.Sync(source);
			}
		}
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x001590B8 File Offset: 0x001572B8
	public void SetAllAsGlowColor()
	{
		foreach (IGlowCopy copy in this._glowCopies.Values)
		{
			copy.SetAsGlowColor();
		}
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x00159118 File Offset: 0x00157318
	public void SetAllAsOriginalColor()
	{
		foreach (IGlowCopy copy in this._glowCopies.Values)
		{
			copy.SetAsOriginalColor();
		}
	}

	// Token: 0x04001FF2 RID: 8178
	private readonly Dictionary<IGlowSource, IGlowCopy> _glowCopies = new Dictionary<IGlowSource, IGlowCopy>();

	// Token: 0x04001FF4 RID: 8180
	[SerializeField]
	private RectTransform copyParent;
}
