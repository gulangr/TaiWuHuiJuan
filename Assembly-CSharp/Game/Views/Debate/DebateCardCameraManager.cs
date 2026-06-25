using System;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000A9B RID: 2715
	public class DebateCardCameraManager : MonoBehaviour
	{
		// Token: 0x17000E93 RID: 3731
		// (get) Token: 0x060084E1 RID: 34017 RVA: 0x003DC3F2 File Offset: 0x003DA5F2
		// (set) Token: 0x060084E0 RID: 34016 RVA: 0x003DC3EA File Offset: 0x003DA5EA
		public static DebateCardCameraManager Instance { get; private set; }

		// Token: 0x17000E94 RID: 3732
		// (get) Token: 0x060084E2 RID: 34018 RVA: 0x003DC3F9 File Offset: 0x003DA5F9
		public Camera Camera
		{
			get
			{
				return this.lifeSkillCombatCardCamera;
			}
		}

		// Token: 0x060084E3 RID: 34019 RVA: 0x003DC401 File Offset: 0x003DA601
		private void Awake()
		{
			DebateCardCameraManager.Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			this.Hide();
		}

		// Token: 0x060084E4 RID: 34020 RVA: 0x003DC420 File Offset: 0x003DA620
		public void Show(bool isTaiwu, short strategyTemplateId)
		{
			bool flag = this.lifeSkillCombatCardCamera.targetTexture == null;
			if (flag)
			{
				Rect rect = this.enemyUsingFailedCardView.RectTransform.rect;
				RenderTexture texture = new RenderTexture((int)rect.width, (int)rect.height, 0);
				this.lifeSkillCombatCardCamera.targetTexture = texture;
			}
			DebateCardView cardView = isTaiwu ? this.taiwuUsingFailedCardView : this.enemyUsingFailedCardView;
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

		// Token: 0x060084E5 RID: 34021 RVA: 0x003DC4D7 File Offset: 0x003DA6D7
		public void Hide()
		{
			this.lifeSkillCombatCardCamera.enabled = false;
			this.taiwuUsingFailedCardView.gameObject.SetActive(false);
			this.enemyUsingFailedCardView.gameObject.SetActive(false);
		}

		// Token: 0x040065DD RID: 26077
		[SerializeField]
		private Camera lifeSkillCombatCardCamera;

		// Token: 0x040065DE RID: 26078
		[SerializeField]
		private DebateCardView enemyUsingFailedCardView;

		// Token: 0x040065DF RID: 26079
		[SerializeField]
		private DebateCardView taiwuUsingFailedCardView;
	}
}
