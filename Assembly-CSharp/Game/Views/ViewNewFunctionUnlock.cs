using System;
using System.Collections.Generic;
using System.Text;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia;
using GameData.Domains.Global;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x0200070A RID: 1802
	public class ViewNewFunctionUnlock : UIBase
	{
		// Token: 0x06005541 RID: 21825 RVA: 0x0027887B File Offset: 0x00276A7B
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06005542 RID: 21826 RVA: 0x00278888 File Offset: 0x00276A88
		private void EndDisplay()
		{
			bool flag = ViewNewFunctionUnlock.Queue.Count > 0;
			if (flag)
			{
				this.ExcuteDisplay();
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x06005543 RID: 21827 RVA: 0x002788B7 File Offset: 0x00276AB7
		public override void QuickHide()
		{
			base.QuickHide();
			this.TriggerAllGuide();
			ViewNewFunctionUnlock.NotifyHintShowedIfQueueEmpty();
		}

		// Token: 0x06005544 RID: 21828 RVA: 0x002788CE File Offset: 0x00276ACE
		public static void ClearQueue()
		{
			ViewNewFunctionUnlock.Queue.Clear();
		}

		// Token: 0x06005545 RID: 21829 RVA: 0x002788DC File Offset: 0x00276ADC
		public static void DrainQueueAndNotifyHintShowed(bool clearQueue = true)
		{
			if (clearQueue)
			{
				ViewNewFunctionUnlock.Queue.Clear();
			}
			TaiwuEventDomainMethod.Call.TriggerListener("NewFeatureHintShowed", true);
		}

		// Token: 0x06005546 RID: 21830 RVA: 0x00278908 File Offset: 0x00276B08
		private static void NotifyHintShowedIfQueueEmpty()
		{
			bool flag = ViewNewFunctionUnlock.Queue.Count == 0;
			if (flag)
			{
				TaiwuEventDomainMethod.Call.TriggerListener("NewFeatureHintShowed", true);
			}
		}

		// Token: 0x06005547 RID: 21831 RVA: 0x00278934 File Offset: 0x00276B34
		private void Init()
		{
			bool flag = this.inited;
			if (!flag)
			{
				this.inited = true;
				this._sb = new StringBuilder();
				this.btnJump.ClearAndAddListener(new Action(this.Jump));
				this.btnEncyclopedia.ClearAndAddListener(new Action(this.JumpToEncyclopedia));
				this._canJump = new HashSet<int>
				{
					0,
					1,
					2,
					4,
					5,
					9,
					12,
					13,
					14,
					15,
					16,
					20,
					21,
					22
				};
			}
		}

		// Token: 0x06005548 RID: 21832 RVA: 0x00278A18 File Offset: 0x00276C18
		private void JumpToEncyclopedia()
		{
			NewFunctionUnlockItem config = NewFunctionUnlock.Instance[this._currentUnlockId];
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.Instance[config.EncyclopediaTabId]);
			this.CheckClose();
		}

		// Token: 0x06005549 RID: 21833 RVA: 0x00278A54 File Offset: 0x00276C54
		private void Jump()
		{
			bool flag = !this._tempHide;
			if (flag)
			{
				this.EndDisplay();
			}
		}

		// Token: 0x0600554A RID: 21834 RVA: 0x00278A78 File Offset: 0x00276C78
		private void CheckClose()
		{
			this.SetDomainHide(true);
		}

		// Token: 0x0600554B RID: 21835 RVA: 0x00278A84 File Offset: 0x00276C84
		public override void OnInit(ArgumentBox argsBox)
		{
			this._guidingUnlocked.Clear();
			this.Init();
			int templateId;
			bool flag = argsBox != null && argsBox.Get("FunctionUnlockTemplateId", out templateId);
			if (flag)
			{
				this.QueueUp(templateId);
			}
		}

		// Token: 0x0600554C RID: 21836 RVA: 0x00278AC6 File Offset: 0x00276CC6
		private void QueueUp(int unlockId)
		{
			ViewNewFunctionUnlock.Queue.Enqueue(unlockId);
		}

		// Token: 0x0600554D RID: 21837 RVA: 0x00278AD8 File Offset: 0x00276CD8
		private void SetInfo(int unlockId)
		{
			this._currentUnlockId = unlockId;
			NewFunctionUnlockItem config = NewFunctionUnlock.Instance[unlockId];
			this.desc.text = config.Desc;
			this.mainBg.SetTexture(string.Format("{0}{1}", "ui9_tex_system_newfeature_specialbase_", (int)config.Type));
			this.title.SetSprite(string.Format("ui9_tex_system_newfeature_specialtitle_{0}_{1}", unlockId, this._titleImage[LocalStringManager.CurLanguageType]), false, null);
			bool hasEffect = this.effectGroup != null && this.effectGroup.TryPlay((int)config.Type);
			this.mainBg.gameObject.SetActive(!hasEffect);
		}

		// Token: 0x0600554E RID: 21838 RVA: 0x00278B9C File Offset: 0x00276D9C
		private void Reset()
		{
			this.mask.color.SetAlpha(0f);
			this.mainArea.alpha = 0f;
			NewFunctionUnlockEffectGroup newFunctionUnlockEffectGroup = this.effectGroup;
			if (newFunctionUnlockEffectGroup != null)
			{
				newFunctionUnlockEffectGroup.StopAll();
			}
			this._canHide = false;
		}

		// Token: 0x0600554F RID: 21839 RVA: 0x00278BEA File Offset: 0x00276DEA
		private void OnDisable()
		{
			this.Reset();
		}

		// Token: 0x06005550 RID: 21840 RVA: 0x00278BF4 File Offset: 0x00276DF4
		private void OnEnable()
		{
			this.ExcuteDisplay();
		}

		// Token: 0x06005551 RID: 21841 RVA: 0x00278C00 File Offset: 0x00276E00
		private void ExcuteDisplay()
		{
			AudioManager.Instance.PlaySound("SFX_sectstory_title", false, false);
			int unlockId = ViewNewFunctionUnlock.Queue.Dequeue();
			bool flag = unlockId == 14;
			if (flag)
			{
				this._guidingUnlocked.Add(319);
			}
			else
			{
				bool flag2 = unlockId == 3 || unlockId == 19;
				if (flag2)
				{
					this._guidingUnlocked.Add(320);
				}
			}
			this.Reset();
			this.SetInfo(unlockId);
			this.mask.DOFade(1f, 0.5f);
			this.mainArea.DOFade(1f, 0.7f).SetDelay(0.3f).OnComplete(delegate
			{
				this._canHide = true;
			});
			this.SetButtonActive(unlockId);
		}

		// Token: 0x06005552 RID: 21842 RVA: 0x00278CCA File Offset: 0x00276ECA
		private void SetButtonActive(int unlockId)
		{
			this.btnJump.gameObject.SetActive(true);
			this.btnEncyclopedia.gameObject.SetActive(NewFunctionUnlock.Instance[unlockId].EncyclopediaTabId > 0);
		}

		// Token: 0x06005553 RID: 21843 RVA: 0x00278D04 File Offset: 0x00276F04
		private void Update()
		{
			bool flag = this._tempHide && UIManager.Instance.IsFocusElement(this.Element);
			if (flag)
			{
				this.SetDomainHide(false);
			}
		}

		// Token: 0x06005554 RID: 21844 RVA: 0x00278D3C File Offset: 0x00276F3C
		private void SetDomainHide(bool tempHide)
		{
			this.domainArea.alpha = (float)(tempHide ? 0 : 1);
			this.domainArea.blocksRaycasts = !tempHide;
			this.effectGroup.gameObject.SetActive(!tempHide);
			this._tempHide = tempHide;
		}

		// Token: 0x06005555 RID: 21845 RVA: 0x00278D90 File Offset: 0x00276F90
		private void TriggerAllGuide()
		{
			foreach (short item in this._guidingUnlocked)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(item);
			}
			this._guidingUnlocked.Clear();
		}

		// Token: 0x04003A15 RID: 14869
		[SerializeField]
		private CanvasGroup domainArea;

		// Token: 0x04003A16 RID: 14870
		[SerializeField]
		private CanvasGroup mainArea;

		// Token: 0x04003A17 RID: 14871
		[SerializeField]
		private CButton btnJump;

		// Token: 0x04003A18 RID: 14872
		[SerializeField]
		private CButton btnEncyclopedia;

		// Token: 0x04003A19 RID: 14873
		[SerializeField]
		private CRawImage mask;

		// Token: 0x04003A1A RID: 14874
		[SerializeField]
		private CRawImage mainBg;

		// Token: 0x04003A1B RID: 14875
		[SerializeField]
		private CImage title;

		// Token: 0x04003A1C RID: 14876
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04003A1D RID: 14877
		[SerializeField]
		private NewFunctionUnlockEffectGroup effectGroup;

		// Token: 0x04003A1E RID: 14878
		private bool _canHide = false;

		// Token: 0x04003A1F RID: 14879
		public static readonly Queue<int> Queue = new Queue<int>();

		// Token: 0x04003A20 RID: 14880
		private bool inited = false;

		// Token: 0x04003A21 RID: 14881
		private int _currentUnlockId;

		// Token: 0x04003A22 RID: 14882
		private bool _tempHide = false;

		// Token: 0x04003A23 RID: 14883
		private HashSet<int> _canJump;

		// Token: 0x04003A24 RID: 14884
		private StringBuilder _sb;

		// Token: 0x04003A25 RID: 14885
		private HashSet<short> _guidingUnlocked = new HashSet<short>();

		// Token: 0x04003A26 RID: 14886
		private readonly Dictionary<LocalStringManager.LanguageType, string> _titleImage = new Dictionary<LocalStringManager.LanguageType, string>
		{
			{
				LocalStringManager.LanguageType.CN,
				"0"
			},
			{
				LocalStringManager.LanguageType.EN,
				"1"
			},
			{
				LocalStringManager.LanguageType.CNH,
				"cnh"
			},
			{
				LocalStringManager.LanguageType.KO,
				"ko"
			}
		};
	}
}
