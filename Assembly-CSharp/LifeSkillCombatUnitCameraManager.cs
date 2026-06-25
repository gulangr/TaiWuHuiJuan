using System;
using UnityEngine;

// Token: 0x0200024A RID: 586
public class LifeSkillCombatUnitCameraManager : MonoBehaviour
{
	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x0600267A RID: 9850 RVA: 0x00119E9F File Offset: 0x0011809F
	// (set) Token: 0x06002679 RID: 9849 RVA: 0x00119E97 File Offset: 0x00118097
	public static LifeSkillCombatUnitCameraManager Instance { get; private set; }

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x0600267B RID: 9851 RVA: 0x00119EA6 File Offset: 0x001180A6
	public Camera Camera
	{
		get
		{
			return this.lifeSkillCombatUnitCamera;
		}
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x00119EAE File Offset: 0x001180AE
	private void Awake()
	{
		LifeSkillCombatUnitCameraManager.Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		this.Hide();
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x00119ECC File Offset: 0x001180CC
	public void Show(bool isTaiwu, sbyte lifeSkillType)
	{
		bool flag = this.lifeSkillCombatUnitCamera.targetTexture == null;
		if (flag)
		{
			Rect rect = this.unitImage.rectTransform.rect;
			RenderTexture texture = new RenderTexture((int)rect.width, (int)rect.height, 0);
			this.lifeSkillCombatUnitCamera.targetTexture = texture;
		}
		string spName = LifeSkillCombatUnit.GetSpriteName(isTaiwu, lifeSkillType);
		this.unitImage.transform.localScale = Vector3.one.SetX((float)(isTaiwu ? 1 : -1));
		this.unitImage.SetSprite(spName, true, null);
		this.lifeSkillCombatUnitCamera.enabled = true;
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x00119F6A File Offset: 0x0011816A
	public void Hide()
	{
		this.lifeSkillCombatUnitCamera.enabled = false;
	}

	// Token: 0x04001C58 RID: 7256
	[SerializeField]
	private Camera lifeSkillCombatUnitCamera;

	// Token: 0x04001C59 RID: 7257
	[SerializeField]
	private CImage unitImage;
}
