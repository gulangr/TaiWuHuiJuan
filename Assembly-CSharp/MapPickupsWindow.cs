using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Views.Map;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.Utilities;
using Map.RenderSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000338 RID: 824
public class MapPickupsWindow : MonoBehaviour
{
	// Token: 0x1700054B RID: 1355
	// (get) Token: 0x06003001 RID: 12289 RVA: 0x00177393 File Offset: 0x00175593
	public WorldMapModel _mapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x1700054C RID: 1356
	// (get) Token: 0x06003002 RID: 12290 RVA: 0x0017739A File Offset: 0x0017559A
	public MapBlockData PlayerAtBlock
	{
		get
		{
			return this._mapModel.PlayerAtBlock;
		}
	}

	// Token: 0x1700054D RID: 1357
	// (get) Token: 0x06003003 RID: 12291 RVA: 0x001773A7 File Offset: 0x001755A7
	public MapBlockData SelectedBlock
	{
		get
		{
			return this._mapModel.SelectedBlock;
		}
	}

	// Token: 0x06003004 RID: 12292 RVA: 0x001773B4 File Offset: 0x001755B4
	public void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._inited = true;
			this.rect = base.GetComponent<RectTransform>();
			this._worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
			this._btnPickupAll.onClick.AddListener(delegate()
			{
				Vector2 targetLocation = MapRenderSystem.GetBlockLocalPos(this._currentBlockData.GetLocation());
				Debug.Log(string.Format("test :{0} -- {1}", targetLocation, this._worldMap.LastCameraMoveTarget));
				bool flag2 = targetLocation != this._worldMap.LastCameraMoveTarget;
				if (flag2)
				{
					GEvent.OnEvent(UiEvents.WorldMapSetCameraToLocation, EasyPool.Get<ArgumentBox>().Set<Location>("location", this._currentBlockData.GetLocation()).Set("doTween", false).Set("tweenTime", 0f).Set("ease", Ease.Unset).SetObject("tweenCallBack", null));
				}
				else
				{
					bool flag3 = this.SelectedBlock != this.PlayerAtBlock;
					if (flag3)
					{
						this._pathFinding = true;
						this._lockedLocation = new Location?(this._currentBlockData.GetLocation());
						GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", this.SelectedBlock.GetLocation()));
						this._currentPickupCoor = this.SelectedBlock.GetBlockPos();
						this._actionWhenMoveFinish = delegate()
						{
							bool flag4 = this.SelectedBlock == this.PlayerAtBlock;
							if (flag4)
							{
								this.PickupAll();
							}
						};
					}
					else
					{
						this.PickupAll();
					}
				}
			});
			this._btnPickupAll.interactable = true;
			PointerTrigger pointerTrigger = this._btnPickupAll.GetComponent<PointerTrigger>();
			bool flag = pointerTrigger != null;
			if (flag)
			{
				PointerTrigger pointerTrigger2 = pointerTrigger;
				if (pointerTrigger2.EnterEvent == null)
				{
					pointerTrigger2.EnterEvent = new UnityEvent();
				}
				pointerTrigger2 = pointerTrigger;
				if (pointerTrigger2.ExitEvent == null)
				{
					pointerTrigger2.ExitEvent = new UnityEvent();
				}
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					this.SetPickupItemHighlight(true);
				});
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					this.SetPickupItemHighlight(false);
				});
			}
		}
	}

	// Token: 0x06003005 RID: 12293 RVA: 0x00177491 File Offset: 0x00175691
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x0017749C File Offset: 0x0017569C
	private void Update()
	{
		bool flag = !this._pathFinding;
		if (!flag)
		{
			bool flag2 = this._mapModel.TaiwuMoveState == WorldMapModel.MoveState.WaitEventShow;
			if (flag2)
			{
				this._pathFinding = false;
				this._lockedLocation = null;
				this._actionWhenMoveFinish = null;
			}
			else
			{
				bool flag3 = !this._worldMap.IsMoving && this._currentPickupCoor == this.PlayerAtBlock.GetBlockPos();
				if (flag3)
				{
					GlobalSettings setting = SingletonObject.getInstance<GlobalSettings>();
					bool enableAutoTriggerNormalMapPickup = setting.EnableAutoTriggerNormalMapPickup;
					if (enableAutoTriggerNormalMapPickup)
					{
						this._pathFinding = false;
						this._lockedLocation = null;
					}
					else
					{
						Action actionWhenMoveFinish = this._actionWhenMoveFinish;
						if (actionWhenMoveFinish != null)
						{
							actionWhenMoveFinish();
						}
						this._pathFinding = false;
						this._lockedLocation = null;
					}
				}
			}
		}
	}

	// Token: 0x06003007 RID: 12295 RVA: 0x00177564 File Offset: 0x00175764
	public void SetPickupItemHighlight(bool isHighlight)
	{
		foreach (Refers item in this._currentPickupItems)
		{
			item.CGet<GameObject>("CommonSelected").SetActive(isHighlight);
		}
	}

	// Token: 0x06003008 RID: 12296 RVA: 0x001775C8 File Offset: 0x001757C8
	private void PickupAll()
	{
		TaiwuEventDomainMethod.Call.OnClickMapPickupBatchEvent(new BatchMapPickupInfo
		{
			Location = this._currentBlockData.GetLocation(),
			PickAll = true
		});
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x00177600 File Offset: 0x00175800
	public void RefreshCarrierInfo(short carrierTemplateId)
	{
		bool flag = carrierTemplateId < 0;
		if (flag)
		{
			this._carrierBonusRate.text = string.Empty;
		}
		else
		{
			this._carrierBonusRate.text = string.Format("{0}%", Carrier.Instance[carrierTemplateId].BaseExploreBonusRate);
		}
	}

	// Token: 0x0600300A RID: 12298 RVA: 0x00177658 File Offset: 0x00175858
	public void RefreshPickupItemInfos(MapBlockData blockData)
	{
		bool flag = this._pathFinding && this._lockedLocation != null;
		if (flag)
		{
			bool flag2 = blockData == null || blockData.GetLocation() != this._lockedLocation.Value;
			if (flag2)
			{
				return;
			}
		}
		this._currentBlockData = blockData;
		bool flag3 = blockData == null || UIManager.Instance.IsElementActive(UIElement.PartWorld);
		if (flag3)
		{
			this.ClearDisplay();
			base.gameObject.SetActive(false);
		}
		else
		{
			bool flag4 = !SingletonObject.getInstance<WorldMapModel>().IsLocationShouldInSight(blockData.GetLocation());
			if (flag4)
			{
				this.RefreshItemsDisplay(null);
			}
			else
			{
				List<MapElementPickupDisplayData> pickupDisplayInfo;
				SingletonObject.getInstance<WorldMapModel>().VisibleMapPickupDict.TryGetValue(blockData.GetLocation(), out pickupDisplayInfo);
				this.RefreshItemsDisplay(pickupDisplayInfo);
			}
		}
	}

	// Token: 0x0600300B RID: 12299 RVA: 0x00177720 File Offset: 0x00175920
	public void RefreshPickupItemInfos()
	{
		this.RefreshPickupItemInfos(this._currentBlockData);
	}

	// Token: 0x0600300C RID: 12300 RVA: 0x00177730 File Offset: 0x00175930
	private void ClearDisplay()
	{
		CommonUtils.PrepareEnoughChildren(this._container, this._pickUpItemPrefab.gameObject, 0, null);
	}

	// Token: 0x0600300D RID: 12301 RVA: 0x00177760 File Offset: 0x00175960
	private void RefreshItemsDisplay(List<MapElementPickupDisplayData> pickupDisplayInfo)
	{
		bool flag = pickupDisplayInfo == null || pickupDisplayInfo.Count == 0 || this.rect == null;
		if (flag)
		{
			this.ClearDisplay();
			base.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(true);
			this._btnPickupAll.gameObject.SetActive(pickupDisplayInfo.Count >= 2);
			this._currentPickupItems.Clear();
			CommonUtils.PrepareEnoughChildren(this._container, this._pickUpItemPrefab.gameObject, pickupDisplayInfo.Count, null);
			for (int i = 0; i < pickupDisplayInfo.Count; i++)
			{
				Refers refers = this._container.GetChild(i).GetComponent<Refers>();
				this._currentPickupItems.Add(refers);
				MapPickup mapPickup = pickupDisplayInfo[i].Pickup;
				bool flag2 = mapPickup.ItemType < 0;
				if (flag2)
				{
					refers.CGet<CommonItemBack>("CommonItemBack").SetData(new ItemDisplayData(ItemKey.Invalid, -1), -1);
				}
				else
				{
					refers.CGet<CommonItemBack>("CommonItemBack").SetData(new ItemDisplayData(mapPickup.ItemType, mapPickup.ItemTemplateId), -1);
				}
				refers.CGet<TextMeshProUGUI>("Name").text = mapPickup.Template.Name;
				refers.CGet<TextMeshProUGUI>("Amount").text = this.GetPickAmount(mapPickup);
				bool hasBattle = mapPickup.HasXiangshuMinion;
				bool autoClearBattle = pickupDisplayInfo[i].CanAutoBeatXiangshuMinion;
				refers.CGet<CImage>("BattleSign").gameObject.SetActive(hasBattle);
				refers.CGet<CImage>("BattleSign").SetSprite(autoClearBattle ? "map_eventbutton_4_0" : "map_eventbutton_3_0", false, null);
				int tempIndex = i;
				refers.CGet<CButtonObsolete>("Button").ClearAndAddListener(delegate
				{
					this.PickUpSingle(tempIndex);
				});
			}
			Vector2 tempSize = this.rect.sizeDelta;
			int elementCountLimit = Math.Min(pickupDisplayInfo.Count, 3);
			tempSize.y = this.basicHeight + this.elementHeight * (float)elementCountLimit;
			this.rect.SetSize(tempSize);
		}
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x001779B4 File Offset: 0x00175BB4
	private void PickUpSingle(int targetIndex)
	{
		Vector2 targetLocation = MapRenderSystem.GetBlockLocalPos(this._currentBlockData.GetLocation());
		Debug.Log(string.Format("test :{0} -- {1}", targetLocation, this._worldMap.LastCameraMoveTarget));
		bool flag = targetLocation != this._worldMap.LastCameraMoveTarget;
		if (flag)
		{
			GEvent.OnEvent(UiEvents.WorldMapSetCameraToLocation, EasyPool.Get<ArgumentBox>().Set<Location>("location", this._currentBlockData.GetLocation()).Set("doTween", false).Set("tweenTime", 0f).Set("ease", Ease.Unset).SetObject("tweenCallBack", null));
		}
		else
		{
			bool flag2 = this.SelectedBlock != this.PlayerAtBlock;
			if (flag2)
			{
				this._pathFinding = true;
				this._lockedLocation = new Location?(this._currentBlockData.GetLocation());
				GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", this.SelectedBlock.GetLocation()));
				this._currentPickupCoor = this.SelectedBlock.GetBlockPos();
				this._actionWhenMoveFinish = delegate()
				{
					bool flag3 = this.SelectedBlock == this.PlayerAtBlock;
					if (flag3)
					{
						TaiwuEventDomainMethod.Call.OnClickMapPickupBatchEvent(new BatchMapPickupInfo
						{
							Location = this._currentBlockData.GetLocation(),
							PickAll = false,
							PickupIndex = targetIndex
						});
					}
				};
			}
			else
			{
				TaiwuEventDomainMethod.Call.OnClickMapPickupBatchEvent(new BatchMapPickupInfo
				{
					Location = this._currentBlockData.GetLocation(),
					PickAll = false,
					PickupIndex = targetIndex
				});
			}
		}
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x00177B3C File Offset: 0x00175D3C
	private string GetPickAmount(MapPickup mapPickup)
	{
		switch (mapPickup.Type)
		{
		case MapPickup.EMapPickupType.Resource:
			return mapPickup.ResourceCount.ToString();
		case MapPickup.EMapPickupType.Item:
			return "1";
		case MapPickup.EMapPickupType.ExpBonus:
			return mapPickup.ExpCount.ToString();
		case MapPickup.EMapPickupType.DebtBonus:
			return mapPickup.DebtCount.ToString();
		}
		return string.Empty;
	}

	// Token: 0x040022D1 RID: 8913
	[SerializeField]
	private Refers _pickUpItemPrefab;

	// Token: 0x040022D2 RID: 8914
	[SerializeField]
	private TextMeshProUGUI _carrierBonusRate;

	// Token: 0x040022D3 RID: 8915
	[SerializeField]
	private CButtonObsolete _btnPickupAll;

	// Token: 0x040022D4 RID: 8916
	[SerializeField]
	private RectTransform _container;

	// Token: 0x040022D5 RID: 8917
	public float basicHeight;

	// Token: 0x040022D6 RID: 8918
	public float elementHeight;

	// Token: 0x040022D7 RID: 8919
	private RectTransform rect;

	// Token: 0x040022D8 RID: 8920
	private MapBlockData _currentBlockData;

	// Token: 0x040022D9 RID: 8921
	private List<Refers> _currentPickupItems = new List<Refers>();

	// Token: 0x040022DA RID: 8922
	private ByteCoordinate _currentPickupCoor;

	// Token: 0x040022DB RID: 8923
	private bool _pathFinding = false;

	// Token: 0x040022DC RID: 8924
	private Location? _lockedLocation = null;

	// Token: 0x040022DD RID: 8925
	private ViewWorldMap _worldMap;

	// Token: 0x040022DE RID: 8926
	private Action _actionWhenMoveFinish;

	// Token: 0x040022DF RID: 8927
	private bool _inited = false;
}
