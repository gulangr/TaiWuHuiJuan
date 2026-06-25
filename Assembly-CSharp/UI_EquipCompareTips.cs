using System;
using System.Collections.Generic;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

// Token: 0x0200037B RID: 891
public class UI_EquipCompareTips : UIBase
{
	// Token: 0x06003405 RID: 13317 RVA: 0x0019C298 File Offset: 0x0019A498
	public override void OnInit(ArgumentBox argsBox)
	{
		this.InitRefers();
		this._tipsArea.gameObject.SetActive(false);
		argsBox.Get<ItemDisplayData>("NewData", out this._newItemData);
		argsBox.Get<MouseTipBase>("NewTips", out this._newTipsBase);
		argsBox.Get("CharId", out this._charId);
		argsBox.Get<List<ItemDisplayData>>("Equipments", out this._equipItems);
		bool flag = !argsBox.Get<JiaoLoongDisplayData>("OriginJiaoLoongData", out this._originJiaoLoongDisplayData);
		if (flag)
		{
			this._originJiaoLoongDisplayData = null;
		}
		bool flag2 = !argsBox.Get<JiaoLoongDisplayData>("NewJiaoLoongData", out this._newJiaoLoongDisplayData);
		if (flag2)
		{
			this._newJiaoLoongDisplayData = null;
		}
		bool flag3 = null != this._compareTipsTuple.Item1;
		if (flag3)
		{
			Object.Destroy(this._compareTipsTuple.Item1);
		}
		bool flag4 = null != this._compareTipsTuple.Item2;
		if (flag4)
		{
			Object.Destroy(this._compareTipsTuple.Item2);
		}
		bool flag5 = this._newTipsBase != null;
		if (flag5)
		{
			this.CreateCompareData();
		}
	}

	// Token: 0x06003406 RID: 13318 RVA: 0x0019C3AC File Offset: 0x0019A5AC
	private void CreateCompareData()
	{
		this.SetOriginTips();
		this.SetNewTips();
	}

	// Token: 0x06003407 RID: 13319 RVA: 0x0019C3C0 File Offset: 0x0019A5C0
	private void SetOriginTips()
	{
		bool flag = this._newItemData.Key.ItemType == 4;
		if (flag)
		{
			MouseTipBase oriTips = null;
			bool flag2 = this._originJiaoLoongDisplayData != null;
			if (flag2)
			{
				ResLoader.LoadByName<GameObject>("UI_MouseTipJiao", delegate(GameObject obj)
				{
					oriTips = Object.Instantiate<GameObject>(obj, this._tipsArea).GetComponent<MouseTipJiao>();
					oriTips.transform.position = this._newTipsBase.transform.position;
					oriTips.name = "OriginTips";
					this._compareTipsTuple.Item1 = oriTips;
					this.SetOriginArgsBox();
					this.RefreshTips(this._compareTipsTuple.Item1);
				}, null);
			}
			else
			{
				oriTips = Object.Instantiate<MouseTipCarrier>(base.CGet<MouseTipCarrier>("UI_MouseTipCarrier"), this._tipsArea);
				oriTips.transform.position = this._newTipsBase.transform.position;
				oriTips.name = "OriginTips";
				this._compareTipsTuple.Item1 = oriTips;
				this.SetOriginArgsBox();
				this.RefreshTips(this._compareTipsTuple.Item1);
			}
		}
		else
		{
			MouseTipBase oriTips2 = Object.Instantiate<MouseTipBase>(this._newTipsBase, this._tipsArea);
			oriTips2.name = "OriginTips";
			this._compareTipsTuple.Item1 = oriTips2;
			this.SetOriginArgsBox();
			this.RefreshTips(this._compareTipsTuple.Item1);
		}
	}

	// Token: 0x06003408 RID: 13320 RVA: 0x0019C4E8 File Offset: 0x0019A6E8
	private void SetNewTips()
	{
		MouseTipBase newTips = Object.Instantiate<MouseTipBase>(this._newTipsBase, this._tipsArea);
		newTips.name = "NewTips";
		this._compareTipsTuple.Item2 = newTips;
		this.RefreshTips(this._compareTipsTuple.Item2);
		bool flag = this._newJiaoLoongDisplayData != null;
		if (flag)
		{
			newTips.SetNewData(EasyPool.Get<ArgumentBox>().Set("TemplateDataOnly", false).Set("CharId", this._charId).SetObject("JiaoLoongData", this._newJiaoLoongDisplayData).Set("IsInCompareUI", true));
		}
		else
		{
			newTips.SetNewData(EasyPool.Get<ArgumentBox>().Set("TemplateDataOnly", false).Set("CharId", this._charId).SetObject("ItemData", this._newItemData).Set("IsInCompareUI", true));
		}
	}

	// Token: 0x06003409 RID: 13321 RVA: 0x0019C5C8 File Offset: 0x0019A7C8
	private void SetOriginArgsBox()
	{
		ItemDisplayData originData = null;
		bool flag = this._newItemData.Key.ItemType == 0 || this._newItemData.Key.ItemType == 2;
		if (flag)
		{
			this.SetEquipListActive(true);
			this._equipList.InitPreOnToggle(-1);
			this._equipList.OnActiveToggleChange = delegate(CToggleObsolete newTog, CToggleObsolete oldTog)
			{
				this._selectedEquipToggle = newTog.Key;
				originData = this.GetOriginData(newTog.Key);
				bool flag5 = originData.Key.Equals(ItemKey.Invalid);
				if (!flag5)
				{
					this._compareTipsTuple.Item1.SetNewData(EasyPool.Get<ArgumentBox>().Set("TemplateDataOnly", false).Set("CharId", this._charId).SetObject("ItemData", originData).Set("IsInCompareUI", true));
				}
			};
			this.NotifyEquipInit(this._newItemData.Key.ItemType);
		}
		else
		{
			bool flag2 = this._originJiaoLoongDisplayData != null;
			if (flag2)
			{
				this.SetEquipListActive(false);
				originData = this.GetOriginData(0);
				bool flag3 = originData.Key.Equals(ItemKey.Invalid);
				if (!flag3)
				{
					this._compareTipsTuple.Item1.SetNewData(EasyPool.Get<ArgumentBox>().Set("TemplateDataOnly", false).Set("CharId", this._charId).SetObject("ItemData", originData).Set("IsInCompareUI", true).SetObject("JiaoLoongData", this._originJiaoLoongDisplayData));
					this._tipsArea.gameObject.SetActive(true);
				}
			}
			else
			{
				this.SetEquipListActive(false);
				originData = this.GetOriginData(0);
				bool flag4 = originData.Key.Equals(ItemKey.Invalid);
				if (!flag4)
				{
					this._compareTipsTuple.Item1.SetNewData(EasyPool.Get<ArgumentBox>().Set("TemplateDataOnly", false).Set("CharId", this._charId).SetObject("ItemData", originData).Set("IsInCompareUI", true));
					this._tipsArea.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x0600340A RID: 13322 RVA: 0x0019C7B4 File Offset: 0x0019A9B4
	private void SetEquipListActive(bool active)
	{
		this._equipList.gameObject.SetActive(active);
		this._commonWarning.gameObject.SetActive(active);
	}

	// Token: 0x0600340B RID: 13323 RVA: 0x0019C7DC File Offset: 0x0019A9DC
	private void NotifyEquipInit(sbyte itemType)
	{
		bool interactable = !this._equipItems[(itemType == 0) ? 0 : 8].Key.Equals(ItemKey.Invalid);
		bool interactable2 = !this._equipItems[(itemType == 0) ? 1 : 9].Key.Equals(ItemKey.Invalid);
		bool interactable3 = !this._equipItems[(itemType == 0) ? 2 : 10].Key.Equals(ItemKey.Invalid);
		List<bool> interactableList = new List<bool>
		{
			interactable,
			interactable2,
			interactable3
		};
		CToggleGroupObsolete equipList = this._equipList;
		for (int i = 0; i < interactableList.Count; i++)
		{
			equipList.Get(i).GetComponent<DisableStyleRoot>().SetStyleEffect(!interactableList[i], false);
			equipList.Get(i).interactable = interactableList[i];
		}
		bool flag = !interactable && !interactable2 && !interactable3;
		if (!flag)
		{
			int lastToggle;
			bool hasLastToggle = this._lastWeaponEquipToggle.TryGetValue(itemType, out lastToggle);
			int targetToggle = -1;
			bool flag2 = hasLastToggle && interactableList[lastToggle];
			if (flag2)
			{
				targetToggle = lastToggle;
			}
			else
			{
				for (int j = 0; j < interactableList.Count; j++)
				{
					bool flag3 = interactableList[j];
					if (flag3)
					{
						targetToggle = j;
						break;
					}
				}
			}
			bool flag4 = targetToggle != -1;
			if (flag4)
			{
				equipList.Set(targetToggle, true, false);
				equipList.NotifyToggle(equipList.Get(targetToggle), true, true);
			}
			this._tipsArea.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600340C RID: 13324 RVA: 0x0019C9A0 File Offset: 0x0019ABA0
	private ItemDisplayData GetOriginData(int index)
	{
		int equipmentType = ItemTemplateHelper.GetEquipmentType(this._newItemData.Key.ItemType, this._newItemData.Key.TemplateId);
		ItemDisplayData originData = null;
		switch (equipmentType)
		{
		case 0:
			originData = this._equipItems[(index == 0) ? 0 : ((index == 1) ? 1 : 2)];
			break;
		case 1:
			originData = this._equipItems[3];
			break;
		case 2:
			originData = this._equipItems[4];
			break;
		case 3:
			originData = this._equipItems[5];
			break;
		case 4:
			originData = this._equipItems[6];
			break;
		case 5:
			originData = this._equipItems[7];
			break;
		case 6:
			originData = this._equipItems[(index == 0) ? 8 : ((index == 1) ? 9 : 10)];
			break;
		case 7:
			originData = this._equipItems[11];
			break;
		}
		return originData;
	}

	// Token: 0x0600340D RID: 13325 RVA: 0x0019CAA4 File Offset: 0x0019ACA4
	private void OnDisable()
	{
		this._originJiaoLoongDisplayData = null;
		this._newJiaoLoongDisplayData = null;
		CToggleObsolete activeToggle = this._equipList.GetActive();
		bool flag = activeToggle != null;
		if (flag)
		{
			this._lastWeaponEquipToggle[this._newItemData.Key.ItemType] = activeToggle.Key;
		}
		for (int i = 0; i < this._tipsArea.childCount; i++)
		{
			Transform child = this._tipsArea.GetChild(i);
			bool flag2 = child.name == "NewTips" || child.name == "OriginTips";
			if (flag2)
			{
				Object.Destroy(child.gameObject);
			}
		}
	}

	// Token: 0x0600340E RID: 13326 RVA: 0x0019CB60 File Offset: 0x0019AD60
	private void Update()
	{
		bool flag = !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl);
		if (flag)
		{
			UIManager.Instance.HideUI(this.Element);
			TooltipManager manager = SingletonObject.getInstance<TooltipManager>();
			manager.Tick(true);
		}
		float scrollValue = Input.GetAxis("Mouse ScrollWheel");
		bool flag2 = scrollValue > 0.05f;
		if (flag2)
		{
			bool flag3 = this._equipList.gameObject.activeSelf && this._selectedEquipToggle != -1;
			if (flag3)
			{
				this.TrySwitchEquipToggle(this._selectedEquipToggle, -1);
			}
		}
		else
		{
			bool flag4 = scrollValue < -0.05f;
			if (flag4)
			{
				bool flag5 = this._equipList.gameObject.activeSelf && this._selectedEquipToggle != -1;
				if (flag5)
				{
					this.TrySwitchEquipToggle(this._selectedEquipToggle, 1);
				}
			}
		}
		this.AlignTwoTipsTop();
		this.RefreshTopPosition();
	}

	// Token: 0x0600340F RID: 13327 RVA: 0x0019CC58 File Offset: 0x0019AE58
	private void TrySwitchEquipToggle(int startIndex, int deltaIndex)
	{
		int endIndex = (deltaIndex > 0) ? 3 : -1;
		for (int i = startIndex + deltaIndex; i != endIndex; i += deltaIndex)
		{
			bool interactable = this._equipList.Get(i).interactable;
			if (interactable)
			{
				this._equipList.Set(i, true, false);
				break;
			}
		}
	}

	// Token: 0x06003410 RID: 13328 RVA: 0x0019CCB0 File Offset: 0x0019AEB0
	private void RefreshTopPosition()
	{
		RectTransform originTips = this._compareTipsTuple.Item1.GetComponent<RectTransform>();
		RectTransform newTips = this._compareTipsTuple.Item2.GetComponent<RectTransform>();
		bool flag = originTips == null || newTips == null;
		if (!flag)
		{
			Vector3[] originWorldCorners = new Vector3[4];
			Vector3[] newWorldCorners = new Vector3[4];
			originTips.GetWorldCorners(originWorldCorners);
			newTips.GetWorldCorners(newWorldCorners);
			Vector3 originTop = originWorldCorners[1];
			Vector3 newTop = newWorldCorners[1];
			float maxY = Mathf.Max(originTop.y, newTop.y);
			RectTransform commonParent = this._commonWarning.transform.parent.GetComponent<RectTransform>();
			Vector3 commonTopLocal = commonParent.InverseTransformPoint(new Vector3(this._commonWarning.transform.position.x, maxY, 0f));
			this._commonWarning.transform.localPosition = commonTopLocal;
			Vector3 infoTopLocal = commonParent.InverseTransformPoint(new Vector3(this._originInfo.transform.position.x, originTop.y, 0f));
			this._originInfo.transform.localPosition = new Vector3(infoTopLocal.x, infoTopLocal.y + 12f, 0f);
		}
	}

	// Token: 0x06003411 RID: 13329 RVA: 0x0019CDF8 File Offset: 0x0019AFF8
	private void AlignTwoTipsTop()
	{
		RectTransform originTips = this._compareTipsTuple.Item1.GetComponent<RectTransform>();
		RectTransform newTips = this._compareTipsTuple.Item2.GetComponent<RectTransform>();
		bool flag = originTips == null || newTips == null;
		if (!flag)
		{
			originTips.anchoredPosition = new Vector2(-211f, originTips.anchoredPosition.y);
			newTips.anchoredPosition = new Vector2(498f, newTips.anchoredPosition.y);
			Canvas canvas = this._tipsArea.GetComponent<Canvas>();
			Vector3[] canvasCorners = new Vector3[4];
			canvas.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);
			float screenTopY = canvasCorners[1].y;
			float screenBottomY = canvasCorners[0].y;
			float targetTopY = screenTopY - (screenTopY - screenBottomY) / 6f;
			this.AlignOneTips(originTips, canvasCorners, targetTopY);
			this.AlignOneTips(newTips, canvasCorners, targetTopY);
		}
	}

	// Token: 0x06003412 RID: 13330 RVA: 0x0019CEE4 File Offset: 0x0019B0E4
	private void AlignOneTips(RectTransform tips, Vector3[] canvasCorners, float targetTopY)
	{
		Vector3[] worldCorners = new Vector3[4];
		tips.GetWorldCorners(worldCorners);
		float heightInWorld = worldCorners[1].y - worldCorners[0].y;
		bool flag = heightInWorld > targetTopY - canvasCorners[0].y;
		Vector3 targetWorldPos;
		if (flag)
		{
			targetWorldPos = new Vector3(0f, 0f, 0f);
		}
		else
		{
			targetWorldPos = new Vector3(0f, targetTopY - heightInWorld / 2f, 0f);
		}
		Vector3 adjustedLocal = this._tipsArea.InverseTransformPoint(targetWorldPos);
		tips.localPosition = new Vector3(tips.localPosition.x, adjustedLocal.y, 0f);
	}

	// Token: 0x06003413 RID: 13331 RVA: 0x0019CF98 File Offset: 0x0019B198
	private void RefreshTips(MouseTipBase tipBase)
	{
		RectTransform tipsTransform = tipBase.GetComponent<RectTransform>();
		tipBase.HasStick = true;
		tipBase.OnSticked();
		tipsTransform.pivot = Vector2.one * 0.5f;
		tipsTransform.anchorMin = tipsTransform.pivot;
		tipsTransform.anchorMax = tipsTransform.pivot;
		tipsTransform.SetSiblingIndex(1);
		tipsTransform.anchoredPosition = Vector2.zero;
		tipsTransform.gameObject.SetActive(true);
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x06003414 RID: 13332 RVA: 0x0019D018 File Offset: 0x0019B218
	private void InitRefers()
	{
		this._tipsArea = base.CGet<RectTransform>("TipsArea");
		this._equipList = base.CGet<CToggleGroupObsolete>("EquipList");
		this._commonWarning = base.CGet<GameObject>("CommonWarning");
		this._originInfo = base.CGet<GameObject>("OriginInfo");
	}

	// Token: 0x040025E7 RID: 9703
	private MouseTipBase _newTipsBase;

	// Token: 0x040025E8 RID: 9704
	private ValueTuple<MouseTipBase, MouseTipBase> _compareTipsTuple;

	// Token: 0x040025E9 RID: 9705
	private int _charId;

	// Token: 0x040025EA RID: 9706
	private ItemDisplayData _newItemData;

	// Token: 0x040025EB RID: 9707
	private List<ItemDisplayData> _equipItems;

	// Token: 0x040025EC RID: 9708
	private JiaoLoongDisplayData _originJiaoLoongDisplayData;

	// Token: 0x040025ED RID: 9709
	private JiaoLoongDisplayData _newJiaoLoongDisplayData;

	// Token: 0x040025EE RID: 9710
	private int _selectedEquipToggle = -1;

	// Token: 0x040025EF RID: 9711
	private readonly Dictionary<sbyte, int> _lastWeaponEquipToggle = new Dictionary<sbyte, int>();

	// Token: 0x040025F0 RID: 9712
	private RectTransform _tipsArea;

	// Token: 0x040025F1 RID: 9713
	private CToggleGroupObsolete _equipList;

	// Token: 0x040025F2 RID: 9714
	private GameObject _commonWarning;

	// Token: 0x040025F3 RID: 9715
	private GameObject _originInfo;
}
