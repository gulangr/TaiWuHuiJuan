using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.World
{
	// Token: 0x0200072F RID: 1839
	public class ViewKongsangSelectMapArea : UIBase
	{
		// Token: 0x060057E8 RID: 22504 RVA: 0x0028CAD1 File Offset: 0x0028ACD1
		private void Awake()
		{
			this.scaler.OnScale = delegate(Vector3 _)
			{
				this.areaMap.dragMove.AdjustOffsetAfterScale();
			};
			this.areaMap.PostRender = delegate(Area area, AreaDisplayData data)
			{
				area.Displayer.enabled = false;
				area.Displayer.Type = TipType.KongSangDing;
				TooltipInvoker displayer = area.Displayer;
				ArgumentBox argumentBox;
				if ((argumentBox = displayer.RuntimeParam) == null)
				{
					argumentBox = (displayer.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("arg0", area.Config.Name).Set("arg1", area.Config.Desc);
				SettlementDisplayData[] settlementDisplayData = data.SettlementDisplayData;
				bool flag = settlementDisplayData != null && settlementDisplayData.Length > 0;
				if (flag)
				{
					OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 10, SectFunctionStatuses.SectFunctionStatusType.UpgradedInteractionUnlocked, delegate(int offset, RawDataPool pool)
					{
						bool unlock = false;
						Serializer.Deserialize(pool, offset, ref unlock);
						TooltipInvoker displayer2 = area.Displayer;
						ArgumentBox argumentBox2;
						if ((argumentBox2 = displayer2.RuntimeParam) == null)
						{
							argumentBox2 = (displayer2.RuntimeParam = EasyPool.Get<ArgumentBox>());
						}
						argumentBox2.SetObject("arg2", unlock ? data.SettlementDisplayData : null);
						area.Displayer.enabled = true;
					});
				}
			};
		}

		// Token: 0x060057E9 RID: 22505 RVA: 0x0028CB04 File Offset: 0x0028AD04
		public override void OnInit(ArgumentBox argsBox)
		{
			if (argsBox != null)
			{
				argsBox.Get<Action<short>>("OnFinishSelect", out this._onFinishSelectHandler);
			}
			if (argsBox != null)
			{
				argsBox.Get("SelectConfirmContent", out this._selectConfirmContent);
			}
			this._worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			this.areaMap.OnSelectArea = new Action<short>(this.OnClickArea);
			this.areaMap.Init(false, this, false);
			this.avatar.Refresh(null, 755);
			this.dingName.text = NameCenter.FormatName(Character.DefValue.TripodVesselOfMedicine.Surname, Character.DefValue.TripodVesselOfMedicine.GivenName);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x060057EA RID: 22506 RVA: 0x0028CBD6 File Offset: 0x0028ADD6
		public void RequestData()
		{
			AreaMap.RequestData(this, delegate(AreaDisplayData[] data)
			{
				this.areaMap.Refresh(data);
				this.areaMap.SetAreaInteractable(this._worldMapModel.CurrentAreaId, false);
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x060057EB RID: 22507 RVA: 0x0028CBEC File Offset: 0x0028ADEC
		private void OnDisable()
		{
			short areaId = this.areaMap.SelectedAreaId;
			Action<short> onFinishSelectHandler = this._onFinishSelectHandler;
			if (onFinishSelectHandler != null)
			{
				onFinishSelectHandler(areaId);
			}
			TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("SelectMapAreaOver", "SelectedAreaId", (int)areaId);
			TaiwuEventDomainMethod.Call.TriggerListener("SelectMapAreaOver", true);
			this.areaMap.CancelSelect();
		}

		// Token: 0x060057EC RID: 22508 RVA: 0x0028CC44 File Offset: 0x0028AE44
		private void OnClickArea(short areaId)
		{
			bool flag = areaId == this._worldMapModel.CurrentAreaId;
			if (flag)
			{
				this.areaMap.SelectedAreaTemplateId = -1;
			}
			else
			{
				bool flag2 = string.IsNullOrEmpty(this._selectConfirmContent);
				if (flag2)
				{
					this.QuickHide();
				}
				else
				{
					MapAreaItem areaConfig = this.areaMap.MapModel.Areas[(int)areaId].GetConfig();
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
					{
						Title = LanguageKey.LK_UI_SelectArea_Title.Tr(),
						Content = this._selectConfirmContent.GetFormat(MapState.Instance[areaConfig.StateID].Name + "·" + areaConfig.Name).ColorReplace(),
						Type = 1,
						Yes = new Action(this.QuickHide),
						No = new Action(this.areaMap.CancelSelect)
					}));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x04003C56 RID: 15446
		[SerializeField]
		private AreaMap areaMap;

		// Token: 0x04003C57 RID: 15447
		[SerializeField]
		private TMP_Text dingName;

		// Token: 0x04003C58 RID: 15448
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04003C59 RID: 15449
		[SerializeField]
		private MouseWheelScale scaler;

		// Token: 0x04003C5A RID: 15450
		private string _selectConfirmContent;

		// Token: 0x04003C5B RID: 15451
		private WorldMapModel _worldMapModel;

		// Token: 0x04003C5C RID: 15452
		private Action<short> _onFinishSelectHandler;

		// Token: 0x04003C5D RID: 15453
		private Dictionary<sbyte, List<Refers>> _stateAreasMap;
	}
}
