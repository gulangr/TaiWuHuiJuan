using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AD3 RID: 2771
	public class CricketCombatSkillBubbleItem : MonoBehaviour
	{
		// Token: 0x17000F04 RID: 3844
		// (get) Token: 0x0600886F RID: 34927 RVA: 0x003F4820 File Offset: 0x003F2A20
		// (set) Token: 0x06008870 RID: 34928 RVA: 0x003F4828 File Offset: 0x003F2A28
		public CanvasGroup CanvasGroup { get; private set; }

		// Token: 0x17000F05 RID: 3845
		// (get) Token: 0x06008871 RID: 34929 RVA: 0x003F4831 File Offset: 0x003F2A31
		// (set) Token: 0x06008872 RID: 34930 RVA: 0x003F4839 File Offset: 0x003F2A39
		public RectTransform RectTransform { get; private set; }

		// Token: 0x06008873 RID: 34931 RVA: 0x003F4844 File Offset: 0x003F2A44
		public void Setup(string content)
		{
			bool flag = this.text != null;
			if (flag)
			{
				this.text.text = content;
			}
		}

		// Token: 0x06008874 RID: 34932 RVA: 0x003F4870 File Offset: 0x003F2A70
		private void Awake()
		{
			this.CanvasGroup = base.GetComponent<CanvasGroup>();
			bool flag = this.CanvasGroup == null;
			if (flag)
			{
				this.CanvasGroup = base.gameObject.AddComponent<CanvasGroup>();
			}
			this.RectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x04006886 RID: 26758
		[SerializeField]
		private TextMeshProUGUI text;
	}
}
