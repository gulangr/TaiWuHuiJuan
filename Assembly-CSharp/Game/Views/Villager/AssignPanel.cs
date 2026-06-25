using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Villager
{
	// Token: 0x0200073D RID: 1853
	public class AssignPanel : MonoBehaviour
	{
		// Token: 0x06005993 RID: 22931 RVA: 0x00298DD5 File Offset: 0x00296FD5
		private void Awake()
		{
			this.currentWork.allowSwitchOff = true;
			this.currentWork.allowUncheck = false;
			this.currentWork.Init(-1);
		}

		// Token: 0x06005994 RID: 22932 RVA: 0x00298E00 File Offset: 0x00297000
		private void SetCurrentWork(int index)
		{
			this._invokeToggleCallback = false;
			bool flag = index != -1;
			if (flag)
			{
				this.currentWork.Set(index, false);
			}
			else
			{
				this.currentWork.DeSelect(false);
			}
			this._invokeToggleCallback = true;
		}

		// Token: 0x06005995 RID: 22933 RVA: 0x00298E44 File Offset: 0x00297044
		public void Hide()
		{
			base.gameObject.SetActive(false);
			Action deactiveAction = this.DeactiveAction;
			if (deactiveAction != null)
			{
				deactiveAction();
			}
			bool flag = UIManager.Instance.CheckEscHandler(new Action(this.Hide));
			if (flag)
			{
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x06005996 RID: 22934 RVA: 0x00298E98 File Offset: 0x00297098
		public void OnAssign(AssignPanel.EWorkType workType)
		{
			this._workType = workType;
			switch (workType)
			{
			case AssignPanel.EWorkType.FarmerCollectFood:
				TaiwuDomainMethod.AsyncCall.SetFarmerCollectResourceWork(this.Parent, this.charId, (short)this.areaId, 0, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerCollect1:
				TaiwuDomainMethod.AsyncCall.SetFarmerCollectResourceWork(this.Parent, this.charId, (short)this.areaId, 1, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerCollect2:
				TaiwuDomainMethod.AsyncCall.SetFarmerCollectResourceWork(this.Parent, this.charId, (short)this.areaId, 2, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerCollect3:
				TaiwuDomainMethod.AsyncCall.SetFarmerCollectResourceWork(this.Parent, this.charId, (short)this.areaId, 3, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerCollect4:
				TaiwuDomainMethod.AsyncCall.SetFarmerCollectResourceWork(this.Parent, this.charId, (short)this.areaId, 4, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerCollect5:
				TaiwuDomainMethod.AsyncCall.SetFarmerCollectResourceWork(this.Parent, this.charId, (short)this.areaId, 5, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerMigrate0:
				TaiwuDomainMethod.AsyncCall.SetFarmerMigrateWork(this.Parent, this.charId, (short)this.areaId, 0, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerMigrate1:
				TaiwuDomainMethod.AsyncCall.SetFarmerMigrateWork(this.Parent, this.charId, (short)this.areaId, 1, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerMigrate2:
				TaiwuDomainMethod.AsyncCall.SetFarmerMigrateWork(this.Parent, this.charId, (short)this.areaId, 2, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerMigrate3:
				TaiwuDomainMethod.AsyncCall.SetFarmerMigrateWork(this.Parent, this.charId, (short)this.areaId, 3, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerMigrate4:
				TaiwuDomainMethod.AsyncCall.SetFarmerMigrateWork(this.Parent, this.charId, (short)this.areaId, 4, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.FarmerMigrate5:
				TaiwuDomainMethod.AsyncCall.SetFarmerMigrateWork(this.Parent, this.charId, (short)this.areaId, 5, delegate(int _, RawDataPool _)
				{
					this.Parent.RequestData(delegate
					{
						this.RefreshData();
					});
				});
				break;
			case AssignPanel.EWorkType.MerchantSell:
				AssignPanel.MerchantBtn0(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy1:
				AssignPanel.MerchantBtn1(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy2:
				AssignPanel.MerchantBtn2(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy3:
				AssignPanel.MerchantBtn3(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy4:
				AssignPanel.MerchantBtn4(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy5:
				AssignPanel.MerchantBtn5(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy6:
				AssignPanel.MerchantBtn6(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy7:
				AssignPanel.MerchantBtn7(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy8:
				AssignPanel.MerchantBtn8(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy9:
				AssignPanel.MerchantBtn9(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy10:
				AssignPanel.MerchantBtn10(this);
				break;
			case AssignPanel.EWorkType.MerchantBuy11:
				AssignPanel.MerchantBtn11(this);
				break;
			case AssignPanel.EWorkType.GeneralAssign:
				this.QuickAssign();
				break;
			case AssignPanel.EWorkType.GeneralCancel:
				this.QuickCancel();
				break;
			default:
				throw new ArgumentOutOfRangeException("workType");
			}
		}

		// Token: 0x06005997 RID: 22935 RVA: 0x002991B8 File Offset: 0x002973B8
		private void RefreshData()
		{
			bool flag = this.roleId == 5;
			if (flag)
			{
				Action<int> onAssignTombVillager = this.OnAssignTombVillager;
				if (onAssignTombVillager != null)
				{
					onAssignTombVillager(this.charId);
				}
			}
			else
			{
				Action<int, string> onAssignVillager = this.OnAssignVillager;
				if (onAssignVillager != null)
				{
					onAssignVillager(this.charId, (from x in AssignPanel.Farmer.Concat(AssignPanel.Merchant)
					select new ValueTuple<string, AssignPanel.EWorkType>(x.Item2.Tr(), x.Item4)).FirstOrDefault(([TupleElementNames(new string[]
					{
						null,
						"workType"
					})] ValueTuple<string, AssignPanel.EWorkType> x) => x.Item2 == this._workType).Item1);
				}
			}
		}

		// Token: 0x06005998 RID: 22936 RVA: 0x00299250 File Offset: 0x00297450
		private static void MerchantBtn0(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, TemplateKey.Invalid);
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x06005999 RID: 22937 RVA: 0x002992C0 File Offset: 0x002974C0
		private static void MerchantBtn1(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(5, -1));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x0600599A RID: 22938 RVA: 0x00299330 File Offset: 0x00297530
		private static void MerchantBtn2(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(8, 800));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x0600599B RID: 22939 RVA: 0x002993A4 File Offset: 0x002975A4
		private static void MerchantBtn3(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(8, 801));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x0600599C RID: 22940 RVA: 0x00299418 File Offset: 0x00297618
		private static void MerchantBtn4(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(7, -1));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x0600599D RID: 22941 RVA: 0x00299488 File Offset: 0x00297688
		private static void MerchantBtn5(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(9, -1));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x0600599E RID: 22942 RVA: 0x002994F8 File Offset: 0x002976F8
		private static void MerchantBtn6(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(6, -1));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x0600599F RID: 22943 RVA: 0x00299568 File Offset: 0x00297768
		private static void MerchantBtn7(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(2, 200));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x060059A0 RID: 22944 RVA: 0x002995DC File Offset: 0x002977DC
		private static void MerchantBtn8(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(0, -1));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x060059A1 RID: 22945 RVA: 0x0029964C File Offset: 0x0029784C
		private static void MerchantBtn9(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(1, -1));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x060059A2 RID: 22946 RVA: 0x002996BC File Offset: 0x002978BC
		private static void MerchantBtn10(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(10, 1001));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x060059A3 RID: 22947 RVA: 0x00299730 File Offset: 0x00297930
		private static void MerchantBtn11(AssignPanel panel)
		{
			TaiwuDomainMethod.Call.AssignTargetItem(-1, panel.charId, new TemplateKey(10, 1000));
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(panel.Parent, panel.charId, 8, new Location((short)panel.areaId, -1), delegate(int _, RawDataPool _)
			{
				panel.Parent.RequestData(new Action(panel.RefreshData));
			});
		}

		// Token: 0x060059A4 RID: 22948 RVA: 0x002997A4 File Offset: 0x002979A4
		private void QuickAssign()
		{
			VillagerRoleArrangementItem villagerRoleArrangementItem = VillagerRoleArrangement.Instance.FirstOrDefault((VillagerRoleArrangementItem item) => !item.InvisibleInGui && (int)item.VillagerRole == this.roleId);
			short templateId = (villagerRoleArrangementItem != null) ? villagerRoleArrangementItem.TemplateId : -1;
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(this.Parent, this.charId, templateId, (templateId == 13) ? new Location(SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageAreaId(), (short)this.areaId) : new Location((short)((templateId == -1) ? -1 : this.areaId), -1), delegate(int _, RawDataPool _)
			{
				this._workType = AssignPanel.EWorkType.GeneralAssign;
				this.Parent.RequestData(new Action(this.RefreshData));
			});
		}

		// Token: 0x060059A5 RID: 22949 RVA: 0x00299825 File Offset: 0x00297A25
		private void QuickCancel()
		{
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(this.Parent, this.charId, -1, Location.Invalid, delegate(int _, RawDataPool _)
			{
				this._workType = AssignPanel.EWorkType.GeneralCancel;
				this.Parent.RequestData(new Action(this.RefreshData));
				this.currentWork.DeSelect(false);
			});
		}

		// Token: 0x17000AAE RID: 2734
		// (get) Token: 0x060059A6 RID: 22950 RVA: 0x0029984B File Offset: 0x00297A4B
		// (set) Token: 0x060059A7 RID: 22951 RVA: 0x00299860 File Offset: 0x00297A60
		[TupleElementNames(new string[]
		{
			"charId",
			"roleId"
		})]
		public ValueTuple<int, int> Character
		{
			[return: TupleElementNames(new string[]
			{
				"charId",
				"roleId"
			})]
			get
			{
				return new ValueTuple<int, int>(this.charId, this.roleId);
			}
			[param: TupleElementNames(new string[]
			{
				"charId",
				"roleId"
			})]
			set
			{
				this.charId = value.Item1;
				this.roleId = value.Item2;
				bool flag = this.areaId != -1;
				if (flag)
				{
					this.RefreshPanel();
				}
			}
		}

		// Token: 0x060059A8 RID: 22952 RVA: 0x0029989F File Offset: 0x00297A9F
		private void OnDisable()
		{
			this.areaId = -1;
		}

		// Token: 0x060059A9 RID: 22953 RVA: 0x002998A9 File Offset: 0x00297AA9
		public void SelectArea(Transform area, int selectedAreaId)
		{
			this.positionFollower.Target = area;
			this.areaId = selectedAreaId;
			this.RefreshPanel();
		}

		// Token: 0x060059AA RID: 22954 RVA: 0x002998C8 File Offset: 0x00297AC8
		public void RefreshPanel()
		{
			switch (this.roleId)
			{
			case -1:
				base.gameObject.SetActive(false);
				return;
			case 0:
				this.positionFollower.Offset = new Vector3(0f, this.areaButtonOffset, 0f);
				this.RefreshButtons(AssignPanel.Farmer);
				break;
			case 2:
				this.positionFollower.Offset = new Vector3(0f, this.areaNoButtonOffset, 0f);
				this.RefreshButtons(AssignPanel.Doctor);
				break;
			case 3:
				this.positionFollower.Offset = new Vector3(0f, this.areaButtonOffset, 0f);
				this.RefreshButtons(AssignPanel.Merchant);
				break;
			case 4:
				this.positionFollower.Offset = new Vector3(0f, this.areaNoButtonOffset, 0f);
				this.RefreshButtons(AssignPanel.Literati);
				break;
			case 5:
				this.positionFollower.Offset = new Vector3(0f, this.swordTombKeeperOffset, 0f);
				this.RefreshButtons(AssignPanel.SwordTombKeeper);
				break;
			case 6:
				this.positionFollower.Offset = new Vector3(0f, this.areaNoButtonOffset, 0f);
				this.RefreshButtons(AssignPanel.VillageHead);
				break;
			}
			base.gameObject.SetActive(true);
			UIManager.Instance.SetEscHandler(new Action(this.Hide));
		}

		// Token: 0x060059AB RID: 22955 RVA: 0x00299A5C File Offset: 0x00297C5C
		public void RefreshButtons([TupleElementNames(new string[]
		{
			"sprite",
			"title",
			"desc",
			"workType"
		})] ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[] buttonDefines)
		{
			int i;
			for (i = 0; i < buttonDefines.Length; i++)
			{
				ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType> define = buttonDefines[i];
				TooltipInvoker tooltipInvoker = this.buttonDisplayers[i];
				ref string ptr = ref this.buttonDisplayers[i].PresetParam[0];
				string[] presetParam = this.buttonDisplayers[i].PresetParam;
				int num = 1;
				string text = define.Item2.Tr();
				string text2 = define.Item3.Tr();
				tooltipInvoker.enabled = true;
				ptr = text;
				presetParam[num] = text2;
				tooltipInvoker = this.buttonDisplayers[i];
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("arg0", this.buttonDisplayers[i].PresetParam[0]).Set("arg1", this.buttonDisplayers[i].PresetParam[1]);
				CImage cimage = this.buttonImages[i];
				SimpleToggleSpriteSetter btn = this.buttons[i];
				btn.toggle.onValueChanged.ResetListener(delegate(bool isOn)
				{
					bool flag = isOn && this._invokeToggleCallback;
					if (flag)
					{
						this.OnAssign(define.Item4);
					}
				});
				btn.gameObject.SetActive(true);
				btn.SetSprite(define.Item1);
				float theta = ((float)(i * 2) + 1f) / (float)buttonDefines.Length;
				btn.transform.localPosition = new Vector3(Mathf.Sin(theta * 3.1415927f) * this.buttonRadius, Mathf.Cos(theta * 3.1415927f) * this.buttonRadius, 0f);
			}
			while (i < this.buttons.Length)
			{
				this.buttons[i].gameObject.SetActive(false);
				i++;
			}
			this.assign.gameObject.SetActive(buttonDefines.Length == 0);
			this.assign.onClick.ResetListener(new Action(this.QuickAssign));
			this.unassign.onClick.ResetListener(new Action(this.QuickCancel));
		}

		// Token: 0x060059AC RID: 22956 RVA: 0x00299C80 File Offset: 0x00297E80
		public void SetToggleWithData(VillagerRoleCharacterDisplayData data)
		{
			VillagerRoleCharacterDisplayData data2 = data;
			bool flag = ((data2 != null) ? data2.VillagerWorkData : null) != null && this.areaId == (int)data.VillagerWorkData.AreaId;
			if (flag)
			{
				int num = this.roleId;
				int num2 = num;
				if (num2 != 0)
				{
					if (num2 == 3)
					{
						this.SetCurrentWork(Array.FindIndex<ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>>(AssignPanel.Merchant, delegate([TupleElementNames(new string[]
						{
							"sprite",
							"title",
							"desc",
							"workType"
						})] ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType> item)
						{
							AssignPanel.EWorkType item2 = item.Item4;
							sbyte itemType = data.ItemTemplateKey.ItemType;
							short templateId = data.ItemTemplateKey.TemplateId;
							if (!true)
							{
							}
							AssignPanel.EWorkType eworkType;
							switch (itemType)
							{
							case -1:
								if (templateId == -1)
								{
									eworkType = AssignPanel.EWorkType.MerchantSell;
									goto IL_10D;
								}
								break;
							case 0:
								if (templateId == -1)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy8;
									goto IL_10D;
								}
								break;
							case 1:
								if (templateId == -1)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy9;
									goto IL_10D;
								}
								break;
							case 2:
								if (templateId == 200)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy7;
									goto IL_10D;
								}
								break;
							case 5:
								if (templateId == -1)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy1;
									goto IL_10D;
								}
								break;
							case 6:
								if (templateId == -1)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy6;
									goto IL_10D;
								}
								break;
							case 7:
								if (templateId == -1)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy4;
									goto IL_10D;
								}
								break;
							case 8:
								if (templateId == 800)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy2;
									goto IL_10D;
								}
								if (templateId == 801)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy3;
									goto IL_10D;
								}
								break;
							case 9:
								if (templateId == -1)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy5;
									goto IL_10D;
								}
								break;
							case 10:
								if (templateId == 1000)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy11;
									goto IL_10D;
								}
								if (templateId == 1001)
								{
									eworkType = AssignPanel.EWorkType.MerchantBuy10;
									goto IL_10D;
								}
								break;
							}
							eworkType = AssignPanel.EWorkType.GeneralCancel;
							IL_10D:
							if (!true)
							{
							}
							return item2 == eworkType;
						}));
					}
				}
				else
				{
					this.SetCurrentWork(Array.FindIndex<ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>>(AssignPanel.Farmer, delegate([TupleElementNames(new string[]
					{
						"sprite",
						"title",
						"desc",
						"workType"
					})] ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType> item)
					{
						AssignPanel.EWorkType item2 = item.Item4;
						sbyte workType = data.VillagerWorkData.WorkType;
						sbyte resourceType = data.VillagerWorkData.ResourceType;
						if (!true)
						{
						}
						AssignPanel.EWorkType eworkType;
						if (workType != 10)
						{
							if (workType == 14)
							{
								switch (resourceType)
								{
								case 0:
									eworkType = AssignPanel.EWorkType.FarmerMigrate0;
									goto IL_B3;
								case 1:
									eworkType = AssignPanel.EWorkType.FarmerMigrate1;
									goto IL_B3;
								case 2:
									eworkType = AssignPanel.EWorkType.FarmerMigrate2;
									goto IL_B3;
								case 3:
									eworkType = AssignPanel.EWorkType.FarmerMigrate3;
									goto IL_B3;
								case 4:
									eworkType = AssignPanel.EWorkType.FarmerMigrate4;
									goto IL_B3;
								case 5:
									eworkType = AssignPanel.EWorkType.FarmerMigrate5;
									goto IL_B3;
								}
							}
						}
						else
						{
							switch (resourceType)
							{
							case 0:
								eworkType = AssignPanel.EWorkType.FarmerCollectFood;
								goto IL_B3;
							case 1:
								eworkType = AssignPanel.EWorkType.FarmerCollect1;
								goto IL_B3;
							case 2:
								eworkType = AssignPanel.EWorkType.FarmerCollect2;
								goto IL_B3;
							case 3:
								eworkType = AssignPanel.EWorkType.FarmerCollect3;
								goto IL_B3;
							case 4:
								eworkType = AssignPanel.EWorkType.FarmerCollect4;
								goto IL_B3;
							case 5:
								eworkType = AssignPanel.EWorkType.FarmerCollect5;
								goto IL_B3;
							}
						}
						eworkType = AssignPanel.EWorkType.GeneralCancel;
						IL_B3:
						if (!true)
						{
						}
						return item2 == eworkType;
					}));
				}
			}
			else
			{
				this.SetCurrentWork(-1);
			}
		}

		// Token: 0x04003D9D RID: 15773
		[SerializeField]
		private float buttonRadius = 225f;

		// Token: 0x04003D9E RID: 15774
		[SerializeField]
		private int charId;

		// Token: 0x04003D9F RID: 15775
		[SerializeField]
		private int roleId;

		// Token: 0x04003DA0 RID: 15776
		[SerializeField]
		private CToggleGroup currentWork;

		// Token: 0x04003DA1 RID: 15777
		[SerializeField]
		private SimpleToggleSpriteSetter[] buttons;

		// Token: 0x04003DA2 RID: 15778
		[SerializeField]
		private CImage[] buttonImages;

		// Token: 0x04003DA3 RID: 15779
		[SerializeField]
		private TooltipInvoker[] buttonDisplayers;

		// Token: 0x04003DA4 RID: 15780
		[SerializeField]
		private PositionFollower positionFollower;

		// Token: 0x04003DA5 RID: 15781
		[SerializeField]
		internal CButton assign;

		// Token: 0x04003DA6 RID: 15782
		[SerializeField]
		internal CButton unassign;

		// Token: 0x04003DA7 RID: 15783
		[SerializeField]
		public int areaId;

		// Token: 0x04003DA8 RID: 15784
		public IRequestData Parent;

		// Token: 0x04003DA9 RID: 15785
		public Action<int, string> OnAssignVillager;

		// Token: 0x04003DAA RID: 15786
		public Action<int> OnAssignTombVillager;

		// Token: 0x04003DAB RID: 15787
		[NonSerialized]
		public Action DeactiveAction;

		// Token: 0x04003DAC RID: 15788
		private bool _invokeToggleCallback = true;

		// Token: 0x04003DAD RID: 15789
		private AssignPanel.EWorkType _workType;

		// Token: 0x04003DAE RID: 15790
		[TupleElementNames(new string[]
		{
			"sprite",
			"title",
			"desc",
			"workType"
		})]
		public static ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[] Farmer = new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[]
		{
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_0", LanguageKey.LK_VillagerRole_Farmer_Collect_0_Title, LanguageKey.LK_VillagerRole_Farmer_Collect_0_Desc, AssignPanel.EWorkType.FarmerCollectFood),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_1", LanguageKey.LK_VillagerRole_Farmer_Collect_1_Title, LanguageKey.LK_VillagerRole_Farmer_Collect_1_Desc, AssignPanel.EWorkType.FarmerCollect1),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_2", LanguageKey.LK_VillagerRole_Farmer_Collect_2_Title, LanguageKey.LK_VillagerRole_Farmer_Collect_2_Desc, AssignPanel.EWorkType.FarmerCollect2),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_3", LanguageKey.LK_VillagerRole_Farmer_Collect_3_Title, LanguageKey.LK_VillagerRole_Farmer_Collect_3_Desc, AssignPanel.EWorkType.FarmerCollect3),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_4", LanguageKey.LK_VillagerRole_Farmer_Collect_4_Title, LanguageKey.LK_VillagerRole_Farmer_Collect_4_Desc, AssignPanel.EWorkType.FarmerCollect4),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_5", LanguageKey.LK_VillagerRole_Farmer_Collect_5_Title, LanguageKey.LK_VillagerRole_Farmer_Collect_5_Desc, AssignPanel.EWorkType.FarmerCollect5),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_11", LanguageKey.LK_VillagerRole_Farmer_Migrate_5_Title, LanguageKey.LK_VillagerRole_Farmer_Migrate_5_Desc, AssignPanel.EWorkType.FarmerMigrate5),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_10", LanguageKey.LK_VillagerRole_Farmer_Migrate_4_Title, LanguageKey.LK_VillagerRole_Farmer_Migrate_4_Desc, AssignPanel.EWorkType.FarmerMigrate4),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_9", LanguageKey.LK_VillagerRole_Farmer_Migrate_3_Title, LanguageKey.LK_VillagerRole_Farmer_Migrate_3_Desc, AssignPanel.EWorkType.FarmerMigrate3),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_8", LanguageKey.LK_VillagerRole_Farmer_Migrate_2_Title, LanguageKey.LK_VillagerRole_Farmer_Migrate_2_Desc, AssignPanel.EWorkType.FarmerMigrate2),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_7", LanguageKey.LK_VillagerRole_Farmer_Migrate_1_Title, LanguageKey.LK_VillagerRole_Farmer_Migrate_1_Desc, AssignPanel.EWorkType.FarmerMigrate1),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_6", LanguageKey.LK_VillagerRole_Farmer_Migrate_0_Title, LanguageKey.LK_VillagerRole_Farmer_Migrate_0_Desc, AssignPanel.EWorkType.FarmerMigrate0)
		};

		// Token: 0x04003DAF RID: 15791
		[TupleElementNames(new string[]
		{
			"sprite",
			"title",
			"desc",
			"workType"
		})]
		public static ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[] Doctor = new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[0];

		// Token: 0x04003DB0 RID: 15792
		[TupleElementNames(new string[]
		{
			"sprite",
			"title",
			"desc",
			"workType"
		})]
		public static ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[] Merchant = new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[]
		{
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_12", LanguageKey.LK_VillagerRole_Merchant_Button_0_Title, LanguageKey.LK_VillagerRole_Merchant_Button_0_Desc, AssignPanel.EWorkType.MerchantSell),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_13", LanguageKey.LK_VillagerRole_Merchant_Button_1_Title, LanguageKey.LK_VillagerRole_Merchant_Button_1_Desc, AssignPanel.EWorkType.MerchantBuy1),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_14", LanguageKey.LK_VillagerRole_Merchant_Button_2_Title, LanguageKey.LK_VillagerRole_Merchant_Button_2_Desc, AssignPanel.EWorkType.MerchantBuy2),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_15", LanguageKey.LK_VillagerRole_Merchant_Button_3_Title, LanguageKey.LK_VillagerRole_Merchant_Button_3_Desc, AssignPanel.EWorkType.MerchantBuy3),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_16", LanguageKey.LK_VillagerRole_Merchant_Button_4_Title, LanguageKey.LK_VillagerRole_Merchant_Button_4_Desc, AssignPanel.EWorkType.MerchantBuy4),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_17", LanguageKey.LK_VillagerRole_Merchant_Button_5_Title, LanguageKey.LK_VillagerRole_Merchant_Button_5_Desc, AssignPanel.EWorkType.MerchantBuy5),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_18", LanguageKey.LK_VillagerRole_Merchant_Button_6_Title, LanguageKey.LK_VillagerRole_Merchant_Button_6_Desc, AssignPanel.EWorkType.MerchantBuy6),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_19", LanguageKey.LK_VillagerRole_Merchant_Button_7_Title, LanguageKey.LK_VillagerRole_Merchant_Button_7_Desc, AssignPanel.EWorkType.MerchantBuy7),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_20", LanguageKey.LK_VillagerRole_Merchant_Button_8_Title, LanguageKey.LK_VillagerRole_Merchant_Button_8_Desc, AssignPanel.EWorkType.MerchantBuy8),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_21", LanguageKey.LK_VillagerRole_Merchant_Button_9_Title, LanguageKey.LK_VillagerRole_Merchant_Button_9_Desc, AssignPanel.EWorkType.MerchantBuy9),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_22", LanguageKey.LK_VillagerRole_Merchant_Button_10_Title, LanguageKey.LK_VillagerRole_Merchant_Button_10_Desc, AssignPanel.EWorkType.MerchantBuy10),
			new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>("ui9_btn_assignment_btn_3_23", LanguageKey.LK_VillagerRole_Merchant_Button_11_Title, LanguageKey.LK_VillagerRole_Merchant_Button_11_Desc, AssignPanel.EWorkType.MerchantBuy11)
		};

		// Token: 0x04003DB1 RID: 15793
		[TupleElementNames(new string[]
		{
			"sprite",
			"title",
			"desc",
			"workType"
		})]
		public static ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[] Literati = new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[0];

		// Token: 0x04003DB2 RID: 15794
		[TupleElementNames(new string[]
		{
			"sprite",
			"title",
			"desc",
			"workType"
		})]
		public static ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[] SwordTombKeeper = new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[0];

		// Token: 0x04003DB3 RID: 15795
		[TupleElementNames(new string[]
		{
			"sprite",
			"title",
			"desc",
			"workType"
		})]
		public static ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[] VillageHead = new ValueTuple<string, LanguageKey, LanguageKey, AssignPanel.EWorkType>[0];

		// Token: 0x04003DB4 RID: 15796
		[SerializeField]
		private float swordTombKeeperOffset = 300f;

		// Token: 0x04003DB5 RID: 15797
		[SerializeField]
		private float areaButtonOffset = 0f;

		// Token: 0x04003DB6 RID: 15798
		[SerializeField]
		private float areaNoButtonOffset = 100f;

		// Token: 0x02001C13 RID: 7187
		public enum EWorkType
		{
			// Token: 0x0400BF76 RID: 49014
			FarmerCollectFood,
			// Token: 0x0400BF77 RID: 49015
			FarmerCollect1,
			// Token: 0x0400BF78 RID: 49016
			FarmerCollect2,
			// Token: 0x0400BF79 RID: 49017
			FarmerCollect3,
			// Token: 0x0400BF7A RID: 49018
			FarmerCollect4,
			// Token: 0x0400BF7B RID: 49019
			FarmerCollect5,
			// Token: 0x0400BF7C RID: 49020
			FarmerMigrate0,
			// Token: 0x0400BF7D RID: 49021
			FarmerMigrate1,
			// Token: 0x0400BF7E RID: 49022
			FarmerMigrate2,
			// Token: 0x0400BF7F RID: 49023
			FarmerMigrate3,
			// Token: 0x0400BF80 RID: 49024
			FarmerMigrate4,
			// Token: 0x0400BF81 RID: 49025
			FarmerMigrate5,
			// Token: 0x0400BF82 RID: 49026
			MerchantSell,
			// Token: 0x0400BF83 RID: 49027
			MerchantBuy1,
			// Token: 0x0400BF84 RID: 49028
			MerchantBuy2,
			// Token: 0x0400BF85 RID: 49029
			MerchantBuy3,
			// Token: 0x0400BF86 RID: 49030
			MerchantBuy4,
			// Token: 0x0400BF87 RID: 49031
			MerchantBuy5,
			// Token: 0x0400BF88 RID: 49032
			MerchantBuy6,
			// Token: 0x0400BF89 RID: 49033
			MerchantBuy7,
			// Token: 0x0400BF8A RID: 49034
			MerchantBuy8,
			// Token: 0x0400BF8B RID: 49035
			MerchantBuy9,
			// Token: 0x0400BF8C RID: 49036
			MerchantBuy10,
			// Token: 0x0400BF8D RID: 49037
			MerchantBuy11,
			// Token: 0x0400BF8E RID: 49038
			GeneralAssign,
			// Token: 0x0400BF8F RID: 49039
			GeneralCancel
		}
	}
}
