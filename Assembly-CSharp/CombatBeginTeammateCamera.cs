using System;
using Game.Components.Avatar;
using Game.Views.Combat;
using TMPro;
using UICommon.Character;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class CombatBeginTeammateCamera : MonoBehaviour
{
	// Token: 0x17000219 RID: 537
	// (get) Token: 0x060012D8 RID: 4824 RVA: 0x00073127 File Offset: 0x00071327
	// (set) Token: 0x060012D7 RID: 4823 RVA: 0x0007311F File Offset: 0x0007131F
	public static CombatBeginTeammateCamera Instance { get; private set; }

	// Token: 0x060012D9 RID: 4825 RVA: 0x0007312E File Offset: 0x0007132E
	private void Awake()
	{
		CombatBeginTeammateCamera.Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x00073144 File Offset: 0x00071344
	private void OnDestroy()
	{
		this.CleanupResources();
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x00073150 File Offset: 0x00071350
	public void SetRenderTexture(ParticleSystemRenderer renderer)
	{
		bool flag = renderer == null || this.teammateRect == null || this.teammateCamera == null;
		if (!flag)
		{
			this.teammateCamera.enabled = true;
			bool needRecreate = this.currentRenderTexture == null || this.currentRenderTexture.width != (int)this.teammateRect.rect.width || this.currentRenderTexture.height != (int)this.teammateRect.rect.height;
			bool flag2 = needRecreate;
			if (flag2)
			{
				this.RecreateRenderTexture();
			}
			this.teammateCamera.targetTexture = this.currentRenderTexture;
			bool wasEnabled = this.teammateCamera.enabled;
			this.teammateCamera.enabled = true;
			this.teammateCamera.Render();
			this.teammateCamera.enabled = wasEnabled;
			bool flag3 = renderer.sharedMaterial != null;
			if (flag3)
			{
				bool flag4 = renderer.material != renderer.sharedMaterial;
				if (flag4)
				{
					renderer.material.SetTexture("_MainTex", this.currentRenderTexture);
				}
				else
				{
					Material materialInstance = new Material(renderer.sharedMaterial);
					materialInstance.SetTexture("_MainTex", this.currentRenderTexture);
					renderer.material = materialInstance;
				}
			}
		}
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x000732B8 File Offset: 0x000714B8
	public void SetCharData(int charId)
	{
		if (this.characterAvatar == null)
		{
			this.characterAvatar = new CharacterAvatar(this.avatar, true);
		}
		if (this.characterName == null)
		{
			this.characterName = new CharacterName(this.charName, null, null);
		}
		this.characterAvatar.CharacterId = charId;
		this.characterName.CharacterId = charId;
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x00073318 File Offset: 0x00071518
	private void RecreateRenderTexture()
	{
		bool flag = this.currentRenderTexture != null;
		if (flag)
		{
			this.currentRenderTexture.Release();
			Object.Destroy(this.currentRenderTexture);
		}
		int width = Mathf.Max(1, (int)this.teammateRect.rect.width);
		int height = Mathf.Max(1, (int)this.teammateRect.rect.height);
		this.currentRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
		this.currentRenderTexture.Create();
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x000733A4 File Offset: 0x000715A4
	private void CleanupChildObjects()
	{
		bool flag = this.teammateRect == null;
		if (!flag)
		{
			while (this.teammateRect.childCount > 0)
			{
				Transform child = this.teammateRect.GetChild(0);
				bool flag2 = child != null;
				if (flag2)
				{
					child.SetParent(null);
					Object.Destroy(child.gameObject);
				}
			}
		}
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x00073408 File Offset: 0x00071608
	private void CleanupResources()
	{
		bool flag = this.currentRenderTexture != null;
		if (flag)
		{
			this.currentRenderTexture.Release();
			Object.Destroy(this.currentRenderTexture);
			this.currentRenderTexture = null;
		}
		this.CleanupChildObjects();
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x0007344E File Offset: 0x0007164E
	public void Hide()
	{
		this.teammateCamera.enabled = false;
	}

	// Token: 0x04000FF9 RID: 4089
	public const float CombatBeginTeammateAnimDuration = 1f;

	// Token: 0x04000FFA RID: 4090
	[SerializeField]
	private RectTransform teammateRect;

	// Token: 0x04000FFB RID: 4091
	[SerializeField]
	private Camera teammateCamera;

	// Token: 0x04000FFC RID: 4092
	[SerializeField]
	private Game.Components.Avatar.Avatar avatar;

	// Token: 0x04000FFD RID: 4093
	[SerializeField]
	private TextMeshProUGUI charName;

	// Token: 0x04000FFE RID: 4094
	[SerializeField]
	private CombatDefeatMarkTotalCount combatDefeatMarkTotalCount;

	// Token: 0x04000FFF RID: 4095
	private RenderTexture currentRenderTexture;

	// Token: 0x04001000 RID: 4096
	private CharacterAvatar characterAvatar;

	// Token: 0x04001001 RID: 4097
	private CharacterName characterName;
}
