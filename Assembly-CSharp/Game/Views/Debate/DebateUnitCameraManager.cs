using System;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000AA2 RID: 2722
	public class DebateUnitCameraManager : MonoBehaviour
	{
		// Token: 0x17000EB7 RID: 3767
		// (get) Token: 0x0600858C RID: 34188 RVA: 0x003E0591 File Offset: 0x003DE791
		// (set) Token: 0x0600858B RID: 34187 RVA: 0x003E0589 File Offset: 0x003DE789
		public static DebateUnitCameraManager Instance { get; private set; }

		// Token: 0x17000EB8 RID: 3768
		// (get) Token: 0x0600858D RID: 34189 RVA: 0x003E0598 File Offset: 0x003DE798
		public Camera Camera
		{
			get
			{
				return this.lifeSkillCombatUnitCamera;
			}
		}

		// Token: 0x0600858E RID: 34190 RVA: 0x003E05A0 File Offset: 0x003DE7A0
		private void Awake()
		{
			DebateUnitCameraManager.Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			this.Hide();
		}

		// Token: 0x0600858F RID: 34191 RVA: 0x003E05C0 File Offset: 0x003DE7C0
		public void Show(bool isTaiwu, sbyte lifeSkillType)
		{
			bool flag = this.lifeSkillCombatUnitCamera.targetTexture == null;
			if (flag)
			{
				Rect rect = this.unitImage.rectTransform.rect;
				RenderTexture texture = new RenderTexture((int)rect.width, (int)rect.height, 0);
				this.lifeSkillCombatUnitCamera.targetTexture = texture;
			}
			string spName = DebateUnit.GetSpriteName(isTaiwu, lifeSkillType);
			this.unitImage.transform.localScale = Vector3.one.SetX((float)(isTaiwu ? 1 : -1));
			this.unitImage.SetSprite(spName, true, null);
			this.lifeSkillCombatUnitCamera.enabled = true;
		}

		// Token: 0x06008590 RID: 34192 RVA: 0x003E065E File Offset: 0x003DE85E
		public void Hide()
		{
			this.lifeSkillCombatUnitCamera.enabled = false;
		}

		// Token: 0x0400666E RID: 26222
		[SerializeField]
		private Camera lifeSkillCombatUnitCamera;

		// Token: 0x0400666F RID: 26223
		[SerializeField]
		private CImage unitImage;
	}
}
