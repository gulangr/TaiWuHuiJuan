using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.Tutorial
{
	// Token: 0x02000C8A RID: 3210
	public class GuidingChapterItem : MonoBehaviour
	{
		// Token: 0x17001115 RID: 4373
		// (get) Token: 0x0600A3A9 RID: 41897 RVA: 0x004C907C File Offset: 0x004C727C
		// (set) Token: 0x0600A3AA RID: 41898 RVA: 0x004C9084 File Offset: 0x004C7284
		public short TemplateId { get; private set; }

		// Token: 0x0600A3AB RID: 41899 RVA: 0x004C908D File Offset: 0x004C728D
		public void SetTemplateId(short id)
		{
			this.TemplateId = id;
		}

		// Token: 0x0600A3AC RID: 41900 RVA: 0x004C9098 File Offset: 0x004C7298
		public void SetStateIcon(sbyte state)
		{
			this.stateIcon.gameObject.SetActive(state >= 0);
			bool activeSelf = this.stateIcon.gameObject.activeSelf;
			if (activeSelf)
			{
				this.stateIcon.SetSprite(this.GetCurrentLanguageState(state), false, null);
			}
		}

		// Token: 0x0600A3AD RID: 41901 RVA: 0x004C90E8 File Offset: 0x004C72E8
		private string GetCurrentLanguageState(sbyte state)
		{
			bool flag = state == 0;
			string result;
			if (flag)
			{
				switch (LocalStringManager.CurLanguageType)
				{
				case LocalStringManager.LanguageType.EN:
					result = "ui9_icon_tutorial_triggered_state_en_" + state.ToString();
					break;
				case LocalStringManager.LanguageType.KO:
					result = "ui9_icon_tutorial_triggered_state_kr_" + state.ToString();
					break;
				case LocalStringManager.LanguageType.CNH:
					result = "ui9_icon_tutorial_triggered_state_" + state.ToString();
					break;
				default:
					result = "ui9_icon_tutorial_triggered_state_" + state.ToString();
					break;
				}
			}
			else
			{
				LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
				LocalStringManager.LanguageType languageType = curLanguageType;
				if (languageType != LocalStringManager.LanguageType.CNH)
				{
					result = "ui9_icon_tutorial_triggered_state_all_" + state.ToString();
				}
				else
				{
					result = "ui9_icon_tutorial_triggered_state_" + state.ToString();
				}
			}
			return result;
		}

		// Token: 0x0600A3AE RID: 41902 RVA: 0x004C91AC File Offset: 0x004C73AC
		private void Awake()
		{
			this.hover.gameObject.SetActive(false);
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(true);
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(false);
			});
		}

		// Token: 0x040081A7 RID: 33191
		[SerializeField]
		public CButton chapterBtn;

		// Token: 0x040081A8 RID: 33192
		[SerializeField]
		private CImage stateIcon;

		// Token: 0x040081A9 RID: 33193
		[SerializeField]
		public TextMeshProUGUI chapterName;

		// Token: 0x040081AA RID: 33194
		[SerializeField]
		public GameObject selected;

		// Token: 0x040081AB RID: 33195
		[SerializeField]
		public GameObject hover;

		// Token: 0x040081AC RID: 33196
		[SerializeField]
		public PointerTrigger pointerTrigger;
	}
}
