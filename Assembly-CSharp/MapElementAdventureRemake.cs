using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Domains.Adventure;
using GameData.Domains.Map;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003E0 RID: 992
public class MapElementAdventureRemake : MapElementBase
{
	// Token: 0x17000609 RID: 1545
	// (get) Token: 0x06003BA8 RID: 15272 RVA: 0x001E35B6 File Offset: 0x001E17B6
	protected static AdventureRemakeModel AdventureRemakeModel
	{
		get
		{
			return SingletonObject.getInstance<AdventureRemakeModel>();
		}
	}

	// Token: 0x06003BA9 RID: 15273 RVA: 0x001E35C0 File Offset: 0x001E17C0
	public static bool CheckMaybeExist(Location location)
	{
		bool crossArchiveLockMoveTime = MapElementBase.MapModel.CrossArchiveLockMoveTime;
		bool result;
		if (crossArchiveLockMoveTime)
		{
			result = false;
		}
		else
		{
			bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = location.AreaId != MapElementBase.MapModel.ShowingAreaId;
				if (flag2)
				{
					result = false;
				}
				else
				{
					MapBlockData blockData = MapElementBase.MapModel.GetBlockData(location);
					bool flag3 = blockData == null || !blockData.Visible;
					if (flag3)
					{
						result = false;
					}
					else
					{
						AdventureBlockCacheData blockCacheData = MapElementAdventureRemake.AdventureRemakeModel.TryGetLocationAdventureRemake(location);
						bool flag4 = blockCacheData == null;
						result = (!flag4 && blockCacheData.ActiveRuntimeIds.Count > 0);
					}
				}
			}
		}
		return result;
	}

	// Token: 0x1700060A RID: 1546
	// (get) Token: 0x06003BAA RID: 15274 RVA: 0x001E3680 File Offset: 0x001E1880
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.Adventure;
		}
	}

	// Token: 0x06003BAB RID: 15275 RVA: 0x001E3684 File Offset: 0x001E1884
	public override void Scale(float wheel)
	{
		float scale = Mathf.Sqrt(wheel + 0.73f) * 0.76f;
		this.root.SetGlobalScale(Vector3.one * (0.005f * scale));
	}

	// Token: 0x06003BAC RID: 15276 RVA: 0x001E36C4 File Offset: 0x001E18C4
	protected override void OnRefresh()
	{
		AdventureBlockCacheData adventureSiteData = MapElementAdventureRemake.AdventureRemakeModel.TryGetLocationAdventureRemake(base.BlockLocation);
		this.RefreshBySite(adventureSiteData, base.BlockLocation);
	}

	// Token: 0x06003BAD RID: 15277 RVA: 0x001E36F1 File Offset: 0x001E18F1
	protected override void OnCollect()
	{
		this._adventureBlockCacheData = null;
		this._index = 0;
	}

	// Token: 0x06003BAE RID: 15278 RVA: 0x001E3704 File Offset: 0x001E1904
	public void RefreshBySite(AdventureBlockCacheData adventureBlockCacheData, Location adventureLocation)
	{
		GameObject gameObject = base.gameObject;
		bool active;
		if (adventureBlockCacheData != null)
		{
			if (adventureBlockCacheData.AnyAdventure || adventureBlockCacheData.AnyMajorEvent)
			{
				active = true;
				goto IL_2E;
			}
		}
		active = false;
		IL_2E:
		gameObject.SetActive(active);
		bool flag = adventureBlockCacheData == null;
		if (!flag)
		{
			this._adventureBlockCacheData = adventureBlockCacheData;
			IReadOnlyList<int> runtimeIds = adventureBlockCacheData.ActiveRuntimeIds;
			bool flag2 = this._index >= runtimeIds.Count;
			if (flag2)
			{
				this._index = 0;
			}
			int currentRuntimeId = runtimeIds[this._index];
			bool isAdventure = MapElementAdventureRemake.AdventureRemakeModel.AdventureRemakeDict.ContainsKey(currentRuntimeId);
			bool isMajorEvent = !isAdventure && MapElementAdventureRemake.AdventureRemakeModel.AdventureMajorEventDict.ContainsKey(currentRuntimeId);
			this.adventureRoot.SetActive(isAdventure);
			this.majorEventRoot.SetActive(isMajorEvent);
			CButton activeBtn = isAdventure ? this.advBtn : this.majorEventBtn;
			TooltipInvoker tips;
			bool flag3 = activeBtn.TryGetComponent<TooltipInvoker>(out tips);
			if (flag3)
			{
				string title = "";
				string desc = "";
				bool flag4 = isAdventure;
				if (flag4)
				{
					AdventureRuntime runtime = MapElementAdventureRemake.AdventureRemakeModel.AdventureRemakeDict[currentRuntimeId];
					AdventureData data = AdventureRemakeModel.Core.GetAdventureData(runtime.CoreId);
					string name = data.Name;
					title = (((name != null) ? name.ColorReplace() : null) ?? "");
					string desc2 = data.Desc;
					desc = (((desc2 != null) ? desc2.ColorReplace() : null) ?? "");
				}
				else
				{
					bool flag5 = isMajorEvent;
					if (flag5)
					{
						AdventureMajorEvent runtime2 = MapElementAdventureRemake.AdventureRemakeModel.AdventureMajorEventDict[currentRuntimeId];
						AdventureMajorEventData data2 = AdventureRemakeModel.Core.GetAdventureMajorEventData(runtime2.CoreId);
						string name2 = data2.Name;
						title = (((name2 != null) ? name2.ColorReplace() : null) ?? "");
						string desc3 = data2.Desc;
						desc = (((desc3 != null) ? desc3.ColorReplace() : null) ?? "");
					}
				}
				tips.PresetParam = new string[]
				{
					title,
					desc
				};
			}
			activeBtn.gameObject.SetActive(true);
			activeBtn.ClearAndAddListener(new Action(this.OnClickAdventure));
			this.UpdateRemainingMonths(currentRuntimeId, isAdventure, isMajorEvent);
			bool hasMultipleRuntime = runtimeIds.Count > 1;
			this.prevButton.gameObject.SetActive(hasMultipleRuntime);
			this.nextButton.gameObject.SetActive(hasMultipleRuntime);
			this.prevButton.ClearAndAddListener(new Action(this.OnClickPrev));
			this.nextButton.ClearAndAddListener(new Action(this.OnClickNext));
			this.root.gameObject.SetActive(true);
			bool flag6 = isAdventure;
			if (flag6)
			{
				this.SetStyleByTags(currentRuntimeId);
			}
		}
	}

	// Token: 0x06003BAF RID: 15279 RVA: 0x001E39A4 File Offset: 0x001E1BA4
	private void OnClickNext()
	{
		bool flag = this._adventureBlockCacheData == null;
		if (!flag)
		{
			IReadOnlyList<int> runtimeIds = this._adventureBlockCacheData.ActiveRuntimeIds;
			this._index = (this._index + 1) % runtimeIds.Count;
			this.RefreshBySite(this._adventureBlockCacheData, base.BlockLocation);
		}
	}

	// Token: 0x06003BB0 RID: 15280 RVA: 0x001E39F8 File Offset: 0x001E1BF8
	private void OnClickPrev()
	{
		bool flag = this._adventureBlockCacheData == null;
		if (!flag)
		{
			IReadOnlyList<int> runtimeIds = this._adventureBlockCacheData.ActiveRuntimeIds;
			this._index = (this._index - 1 + runtimeIds.Count) % runtimeIds.Count;
			this.RefreshBySite(this._adventureBlockCacheData, base.BlockLocation);
		}
	}

	// Token: 0x06003BB1 RID: 15281 RVA: 0x001E3A50 File Offset: 0x001E1C50
	private void SetStyleByTags(int runtimeId)
	{
		this.commonFly.gameObject.SetActive(false);
		this.mainStoryFly.gameObject.SetActive(false);
		this.sectStoryFly.gameObject.SetActive(false);
		this.swordTomb.gameObject.SetActive(false);
		AdventureRuntime adventureRuntime = MapElementAdventureRemake.AdventureRemakeModel.AdventureRemakeDict[runtimeId];
		AdventureData adventureData = AdventureRemakeModel.Core.GetAdventureData(adventureRuntime.CoreId);
		bool flag = XiangshuAvatarIds.IsSwordTombAdventure(adventureRuntime.CoreId);
		if (flag)
		{
			this.SetBtnSprite(this.mainStoryBtnBackSprite);
			this.swordTomb.gameObject.SetActive(true);
		}
		else
		{
			RepeatedField<EAdventureTag> tags = adventureData.Tags;
			bool flag2 = tags != null && tags.Count > 0;
			if (flag2)
			{
				EAdventureTag typeTag = adventureData.Tags.First<EAdventureTag>();
				bool flag3 = typeTag == EAdventureTag.MainStory;
				if (flag3)
				{
					this.SetBtnSprite(this.mainStoryBtnBackSprite);
					this.mainStoryFly.gameObject.SetActive(true);
				}
				else
				{
					bool flag4 = typeTag == EAdventureTag.SectStory;
					if (flag4)
					{
						this.SetBtnSprite(this.sectStoryBtnBackSprite);
						this.sectStoryFly.gameObject.SetActive(true);
					}
					else
					{
						this.SetBtnSprite(this.otherStoryBtnBackSprite);
						this.commonFly.gameObject.SetActive(true);
					}
				}
			}
			else
			{
				this.SetBtnSprite(this.otherStoryBtnBackSprite);
				this.commonFly.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06003BB2 RID: 15282 RVA: 0x001E3BCC File Offset: 0x001E1DCC
	private void SetBtnSprite(Sprite[] sprites)
	{
		this.advBtnBack.sprite = sprites[0];
		SpriteState spriteState = this.advBtn.spriteState;
		spriteState.highlightedSprite = sprites[1];
		spriteState.pressedSprite = sprites[1];
		spriteState.selectedSprite = sprites[1];
		spriteState.disabledSprite = sprites[0];
		this.advBtn.spriteState = spriteState;
	}

	// Token: 0x06003BB3 RID: 15283 RVA: 0x001E3C2E File Offset: 0x001E1E2E
	private void OnClickAdventure()
	{
		MapDomainMethod.AsyncCall.IsContinuousMovingBreak(null, delegate(int offset, RawDataPool dataPoll)
		{
			bool hasEvent = false;
			Serializer.Deserialize(dataPoll, offset, ref hasEvent);
			bool flag = hasEvent;
			if (!flag)
			{
				bool flag2 = this._adventureBlockCacheData == null;
				if (!flag2)
				{
					WorldMapModel mapModel = MapElementBase.MapModel;
					bool isMapMoving = MapElementBase.IsMapMoving;
					if (!isMapMoving)
					{
						bool flag3 = mapModel.CurrentAreaId != base.BlockLocation.AreaId || mapModel.CurrentBlockId != base.BlockLocation.BlockId;
						if (flag3)
						{
							GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", base.BlockLocation));
						}
						else
						{
							IReadOnlyList<int> runtimeIds = this._adventureBlockCacheData.ActiveRuntimeIds;
							bool flag4 = this._index >= runtimeIds.Count;
							if (!flag4)
							{
								int currentRuntimeId = runtimeIds[this._index];
								ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
								argBox.Clear();
								bool flag5 = MapElementAdventureRemake.AdventureRemakeModel.AdventureRemakeDict.ContainsKey(currentRuntimeId);
								if (flag5)
								{
									argBox.Set("AdventureId", currentRuntimeId);
								}
								else
								{
									argBox.Set("MajorEventId", currentRuntimeId);
								}
								GEvent.OnEvent(UiEvents.AdventureRemakeIconClick, argBox);
							}
						}
					}
				}
			}
		});
	}

	// Token: 0x06003BB4 RID: 15284 RVA: 0x001E3C44 File Offset: 0x001E1E44
	private void UpdateRemainingMonths(int currentRuntimeId, bool isAdventure, bool isMajorEvent)
	{
		AdventureRemakeModel model = MapElementAdventureRemake.AdventureRemakeModel;
		GameObject timeContainer = this.goTimeBg;
		if (isAdventure)
		{
			AdventureRuntime adventureRuntime;
			bool flag = !model.AdventureRemakeDict.TryGetValue(currentRuntimeId, out adventureRuntime);
			if (flag)
			{
				timeContainer.SetActive(false);
			}
			else
			{
				int remainingMonths = adventureRuntime.CalcRemainMonths(SingletonObject.getInstance<BasicGameData>().CurrDate);
				timeContainer.SetActive(remainingMonths >= 0);
				bool activeSelf = timeContainer.activeSelf;
				if (activeSelf)
				{
					this.txtMeshTime.text = remainingMonths.ToString();
				}
			}
		}
		else
		{
			AdventureMajorEvent majorEvent;
			bool flag2 = !model.AdventureMajorEventDict.TryGetValue(currentRuntimeId, out majorEvent);
			if (flag2)
			{
				timeContainer.SetActive(false);
			}
			else
			{
				int remainingMonths2 = majorEvent.CalcRemainMonths(SingletonObject.getInstance<BasicGameData>().CurrDate);
				timeContainer.SetActive(remainingMonths2 >= 0);
				bool activeSelf2 = timeContainer.activeSelf;
				if (activeSelf2)
				{
					this.txtMeshTime.text = remainingMonths2.ToString();
				}
			}
		}
	}

	// Token: 0x04002AF5 RID: 10997
	private int _index = 0;

	// Token: 0x04002AF6 RID: 10998
	private AdventureBlockCacheData _adventureBlockCacheData;

	// Token: 0x04002AF7 RID: 10999
	[SerializeField]
	private RectTransform root;

	// Token: 0x04002AF8 RID: 11000
	[SerializeField]
	private TextMeshProUGUI txtMeshTime;

	// Token: 0x04002AF9 RID: 11001
	[SerializeField]
	private GameObject goTimeBg;

	// Token: 0x04002AFA RID: 11002
	[SerializeField]
	private CButton prevButton;

	// Token: 0x04002AFB RID: 11003
	[SerializeField]
	private CButton nextButton;

	// Token: 0x04002AFC RID: 11004
	[SerializeField]
	private GameObject adventureRoot;

	// Token: 0x04002AFD RID: 11005
	[SerializeField]
	private GameObject majorEventRoot;

	// Token: 0x04002AFE RID: 11006
	[SerializeField]
	private CButton advBtn;

	// Token: 0x04002AFF RID: 11007
	[SerializeField]
	private CImage advBtnBack;

	// Token: 0x04002B00 RID: 11008
	[SerializeField]
	private GameObject mainStoryFly;

	// Token: 0x04002B01 RID: 11009
	[SerializeField]
	private GameObject sectStoryFly;

	// Token: 0x04002B02 RID: 11010
	[SerializeField]
	private GameObject commonFly;

	// Token: 0x04002B03 RID: 11011
	[SerializeField]
	private GameObject swordTomb;

	// Token: 0x04002B04 RID: 11012
	[SerializeField]
	private Sprite[] mainStoryBtnBackSprite = new Sprite[2];

	// Token: 0x04002B05 RID: 11013
	[SerializeField]
	private Sprite[] sectStoryBtnBackSprite = new Sprite[2];

	// Token: 0x04002B06 RID: 11014
	[SerializeField]
	private Sprite[] otherStoryBtnBackSprite = new Sprite[2];

	// Token: 0x04002B07 RID: 11015
	[SerializeField]
	private CButton majorEventBtn;
}
