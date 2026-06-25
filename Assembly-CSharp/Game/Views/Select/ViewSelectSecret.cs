using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Information;
using GameData.Domains.Information;
using TMPro;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x020007B1 RID: 1969
	public class ViewSelectSecret : UIBase
	{
		// Token: 0x06005FE2 RID: 24546 RVA: 0x002C0344 File Offset: 0x002BE544
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = !argsBox.Get("SelectType", out this._type);
			if (flag)
			{
				this._type = 0;
			}
			string title;
			bool flag2 = !argsBox.Get("title", out title);
			if (flag2)
			{
				title = LanguageKey.LK_SecretInformation.Tr();
			}
			object callback;
			bool flag3 = argsBox.Get<object>("callback", out callback);
			if (flag3)
			{
				this._onSecretInformationConfirm = (callback as Action<SecretInformationDisplayData>);
			}
			else
			{
				this._onSecretInformationConfirm = null;
			}
			bool flag4 = !argsBox.Get<SecretInformationDisplayPackage>("secretInformation", out this._package);
			if (flag4)
			{
				this._package = null;
			}
			this.btnConfirm.gameObject.SetActive(this._type != 4);
			this.titleLabel.text = title;
		}

		// Token: 0x06005FE3 RID: 24547 RVA: 0x002C0404 File Offset: 0x002BE604
		private void Awake()
		{
			this.btnConfirm.ClearAndAddListener(new Action(this.OnConfirm));
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.panel.Init(new Action<SecretInformationDisplayData>(this.OnSelect));
		}

		// Token: 0x06005FE4 RID: 24548 RVA: 0x002C045C File Offset: 0x002BE65C
		private void OnEnable()
		{
			this.panel.Clear();
			this.panel.Set(this._package, true);
			SecretInformationPanel secretInformationPanel = this.panel;
			int type = this._type;
			secretInformationPanel.SetController(type == 0 || type == 2 || type == 4);
			this.OnSelect(null);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06005FE5 RID: 24549 RVA: 0x002C04C4 File Offset: 0x002BE6C4
		public override void QuickHide()
		{
			this.SelectInvalid();
			Action<SecretInformationDisplayData> onSecretInformationConfirm = this._onSecretInformationConfirm;
			if (onSecretInformationConfirm != null)
			{
				onSecretInformationConfirm(this._selected);
			}
			base.QuickHide();
		}

		// Token: 0x06005FE6 RID: 24550 RVA: 0x002C04F0 File Offset: 0x002BE6F0
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.btnConfirm.interactable;
			if (flag)
			{
				this.OnConfirm();
			}
		}

		// Token: 0x06005FE7 RID: 24551 RVA: 0x002C0530 File Offset: 0x002BE730
		private void OnSelect(SecretInformationDisplayData data)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SelectInvalid();
				this.btnConfirm.interactable = false;
				this.tips.PresetParam = new string[]
				{
					LanguageKey.LK_SecretInformation_Not_Selected.Tr()
				};
				this.tips.enabled = true;
			}
			else
			{
				this._selected = data;
				int type = this._type;
				bool flag2 = type == 1 || type == 3;
				if (flag2)
				{
					this.btnConfirm.interactable = true;
					this.tips.enabled = false;
				}
				else
				{
					bool flag3;
					if (this._type == 2)
					{
						short secretInformationTemplateId = data.SecretInformationTemplateId;
						flag3 = (secretInformationTemplateId == 112 || secretInformationTemplateId == 113 || secretInformationTemplateId == 114 || secretInformationTemplateId == 115);
					}
					else
					{
						flag3 = false;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						this.tips.PresetParam = new string[]
						{
							LanguageKey.LK_SecretInformation_Cannot_Use_Profession_Skill.Tr()
						};
						this.tips.enabled = true;
						this.btnConfirm.interactable = false;
					}
					else
					{
						int own = SingletonObject.getInstance<WorldMapModel>().TaiwuResources.Resources[7];
						int cost = (this._type == 2) ? data.AuthorityCostWhenDisseminatingForBroadcast : data.AuthorityCostWhenDisseminating;
						bool flag5 = own < cost;
						if (flag5)
						{
							this.tips.PresetParam = new string[]
							{
								LanguageKey.LK_SecretInformation_Not_Enough_Authority.Tr()
							};
							this.tips.enabled = true;
							this.btnConfirm.interactable = false;
						}
						else
						{
							this.btnConfirm.interactable = true;
							this.tips.enabled = false;
						}
						int canUseCount = Math.Max(((data.AuthorityCostWhenDisseminating == 0) ? GlobalConfig.Instance.SecretInformationInBroadcastMaxUseCount : GlobalConfig.Instance.SecretInformationInPrivateMaxUseCount) - data.UsedCount, 0);
						bool flag6 = canUseCount <= 0;
						if (flag6)
						{
							this.btnConfirm.interactable = false;
						}
					}
				}
			}
		}

		// Token: 0x06005FE8 RID: 24552 RVA: 0x002C071E File Offset: 0x002BE91E
		private void SelectInvalid()
		{
			this._selected = new SecretInformationDisplayData
			{
				SecretInformationTemplateId = -1,
				SecretInformationId = SecretInformationId.Invalid
			};
		}

		// Token: 0x06005FE9 RID: 24553 RVA: 0x002C073E File Offset: 0x002BE93E
		private void OnConfirm()
		{
			Action<SecretInformationDisplayData> onSecretInformationConfirm = this._onSecretInformationConfirm;
			if (onSecretInformationConfirm != null)
			{
				onSecretInformationConfirm(this._selected);
			}
			base.QuickHide();
		}

		// Token: 0x0400425C RID: 16988
		[SerializeField]
		private SecretInformationPanel panel;

		// Token: 0x0400425D RID: 16989
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x0400425E RID: 16990
		[SerializeField]
		private CButton btnClose;

		// Token: 0x0400425F RID: 16991
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x04004260 RID: 16992
		[SerializeField]
		private TooltipInvoker tips;

		// Token: 0x04004261 RID: 16993
		private SecretInformationDisplayPackage _package;

		// Token: 0x04004262 RID: 16994
		private Action<SecretInformationDisplayData> _onSecretInformationConfirm;

		// Token: 0x04004263 RID: 16995
		private SecretInformationDisplayData _selected = null;

		// Token: 0x04004264 RID: 16996
		private int _type;
	}
}
