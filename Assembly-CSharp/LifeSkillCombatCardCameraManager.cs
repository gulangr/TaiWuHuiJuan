using System;
using UnityEngine;

// Token: 0x02000245 RID: 581
public class LifeSkillCombatCardCameraManager : MonoBehaviour
{
	// Token: 0x170003F6 RID: 1014
	// (get) Token: 0x060025E5 RID: 9701 RVA: 0x0011621E File Offset: 0x0011441E
	// (set) Token: 0x060025E4 RID: 9700 RVA: 0x00116216 File Offset: 0x00114416
	public static LifeSkillCombatCardCameraManager Instance { get; private set; }

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x060025E6 RID: 9702 RVA: 0x00116225 File Offset: 0x00114425
	public Camera Camera
	{
		get
		{
			return this.lifeSkillCombatCardCamera;
		}
	}

	// Token: 0x060025E7 RID: 9703 RVA: 0x0011622D File Offset: 0x0011442D
	private void Awake()
	{
		LifeSkillCombatCardCameraManager.Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		this.Hide();
	}

	// Token: 0x060025E8 RID: 9704 RVA: 0x0011624C File Offset: 0x0011444C
	public void Show(bool isTaiwu, short strategyTemplateId)
	{
		bool flag = this.lifeSkillCombatCardCamera.targetTexture == null;
		if (flag)
		{
			Rect rect = this.enemyUsingFailedCardView.RectTransform.rect;
			RenderTexture texture = new RenderTexture((int)rect.width, (int)rect.height, 0);
			this.lifeSkillCombatCardCamera.targetTexture = texture;
		}
		LifeSkillCombatCardView cardView = isTaiwu ? this.taiwuUsingFailedCardView : this.enemyUsingFailedCardView;
		cardView.SetData(strategyTemplateId, -1);
		bool flag2 = !isTaiwu;
		if (flag2)
		{
			cardView.ShowCover(true);
		}
		cardView.SetEnabled(true, false);
		cardView.SetInteractable(false);
		cardView.SetPointerTrigger(false);
		cardView.gameObject.SetActive(true);
		this.lifeSkillCombatCardCamera.enabled = true;
	}

	// Token: 0x060025E9 RID: 9705 RVA: 0x00116303 File Offset: 0x00114503
	public void Hide()
	{
		this.lifeSkillCombatCardCamera.enabled = false;
		this.taiwuUsingFailedCardView.gameObject.SetActive(false);
		this.enemyUsingFailedCardView.gameObject.SetActive(false);
	}

	// Token: 0x04001C09 RID: 7177
	[SerializeField]
	private Camera lifeSkillCombatCardCamera;

	// Token: 0x04001C0A RID: 7178
	[SerializeField]
	private LifeSkillCombatCardView enemyUsingFailedCardView;

	// Token: 0x04001C0B RID: 7179
	[SerializeField]
	private LifeSkillCombatCardView taiwuUsingFailedCardView;
}
