using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GameData.Domains.Item;
using GameData.Domains.Map;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x020003D7 RID: 983
public class MapPickupEffectItem : Refers
{
	// Token: 0x06003B1D RID: 15133 RVA: 0x001DEE45 File Offset: 0x001DD045
	private void Awake()
	{
		this.resourceSpine.AnimationState.Complete += delegate(TrackEntry entry)
		{
			bool flag = entry.Animation.Name == "animation";
			if (flag)
			{
				this.DestroySelf();
			}
		};
		this.combatSkillView.EnableTips = false;
	}

	// Token: 0x06003B1E RID: 15134 RVA: 0x001DEE74 File Offset: 0x001DD074
	public void Refresh(MapElementPickupDisplayData pickupDisplayData, MapElementPickupEffect parent)
	{
		this._parent = parent;
		MapPickup pickup = pickupDisplayData.Pickup;
		EMapPickupsType type = pickup.Template.Type;
		this.HideAll();
		switch (type)
		{
		case EMapPickupsType.Resource:
			this.PlayResourceSpine(pickup);
			AudioManager.Instance.PlaySound("ui_GetResources", false, false);
			break;
		case EMapPickupsType.Item:
		{
			string text = ItemTemplateHelper.GetName(pickup.ItemType, pickup.ItemTemplateId).SetGradeColor((int)ItemTemplateHelper.GetGrade(pickup.ItemType, pickup.ItemTemplateId));
			string icon = ItemTemplateHelper.GetIcon(pickup.ItemType, pickup.ItemTemplateId);
			this.itemIcon.gameObject.SetActive(true);
			this.combatSkillView.gameObject.SetActive(false);
			this.itemIcon.transform.localScale = Vector3.one * 0.75f;
			this.PlayItemAnimation(pickup, text, icon, this.itemEffect);
			AudioManager.Instance.PlaySound("ui_GetItem", false, false);
			break;
		}
		case EMapPickupsType.Growth:
			this.PlayGrowthAnimation(pickupDisplayData);
			AudioManager.Instance.PlaySound("ui_GetItem", false, false);
			break;
		}
	}

	// Token: 0x06003B1F RID: 15135 RVA: 0x001DEFA4 File Offset: 0x001DD1A4
	private void PlayGrowthAnimation(MapElementPickupDisplayData displayData)
	{
		bool flag = displayData.BanReason > 0U;
		if (!flag)
		{
			MapPickup pickup = displayData.Pickup;
			bool isLoop = displayData.Pickup.Type == MapPickup.EMapPickupType.LoopEffect;
			this.itemIcon.gameObject.SetActive(!isLoop);
			this.combatSkillView.gameObject.SetActive(isLoop);
			bool flag2 = isLoop;
			if (flag2)
			{
				this.RefreshCombatSkillView(displayData);
			}
			MapPickup.EMapPickupType type = pickup.Type;
			if (!true)
			{
			}
			string text2;
			switch (type)
			{
			case MapPickup.EMapPickupType.ReadEffect:
				text2 = ItemTemplateHelper.GetIcon(displayData.TaiwuReadingBookKey.ItemType, displayData.TaiwuReadingBookKey.TemplateId);
				break;
			case MapPickup.EMapPickupType.ExpBonus:
				text2 = "sp_tuan_lilian";
				break;
			case MapPickup.EMapPickupType.DebtBonus:
				text2 = "sp_tuan_enyi";
				break;
			default:
				text2 = null;
				break;
			}
			if (!true)
			{
			}
			string icon = text2;
			MapPickup.EMapPickupType type2 = pickup.Type;
			if (!true)
			{
			}
			switch (type2)
			{
			case MapPickup.EMapPickupType.LoopEffect:
				text2 = LocalStringManager.Get(LanguageKey.LK_Neigong_Looping);
				break;
			case MapPickup.EMapPickupType.ReadEffect:
				text2 = LocalStringManager.Get(LanguageKey.LK_Reading_Title);
				break;
			case MapPickup.EMapPickupType.ExpBonus:
				text2 = LocalStringManager.Get(LanguageKey.LK_Exp);
				break;
			case MapPickup.EMapPickupType.DebtBonus:
				text2 = LocalStringManager.Get(LanguageKey.Event_Pickup_SpiritualDebt);
				break;
			default:
				text2 = null;
				break;
			}
			if (!true)
			{
			}
			string text = text2;
			Transform transform = this.itemIcon.transform;
			MapPickup.EMapPickupType type3 = pickup.Type;
			if (!true)
			{
			}
			Vector3 localScale;
			if (type3 != MapPickup.EMapPickupType.ReadEffect)
			{
				localScale = Vector3.one;
			}
			else
			{
				localScale = Vector3.one * 0.75f;
			}
			if (!true)
			{
			}
			transform.localScale = localScale;
			this.PlayItemAnimation(pickup, text, icon, this.growthEffect);
		}
	}

	// Token: 0x06003B20 RID: 15136 RVA: 0x001DF139 File Offset: 0x001DD339
	private void RefreshCombatSkillView(MapElementPickupDisplayData displayData)
	{
		this.combatSkillView.SetByCombatSkillTemplateId(displayData.TaiwuLoopingNeigong);
	}

	// Token: 0x06003B21 RID: 15137 RVA: 0x001DF14E File Offset: 0x001DD34E
	private void DestroySelf()
	{
		this.HideAll();
		this._parent.DestroyEffect(this);
	}

	// Token: 0x06003B22 RID: 15138 RVA: 0x001DF165 File Offset: 0x001DD365
	private void HideAll()
	{
		this.growthEffect.SetActive(false);
		this.resourceSpine.GetComponent<CanvasGroup>().alpha = 0f;
		this.itemAnimationRoot.SetActive(false);
	}

	// Token: 0x06003B23 RID: 15139 RVA: 0x001DF198 File Offset: 0x001DD398
	private void PlayResourceSpine(MapPickup pickup)
	{
		this.resourceSpine.gameObject.SetActive(true);
		this.resourceSpine.GetComponent<CanvasGroup>().alpha = 0f;
		string attachmentName = MapPickupEffectItem.<PlayResourceSpine>g__GetAttachmentName|7_1(pickup.ResourceType);
		for (int i = 0; i < 5; i++)
		{
			string slotName = string.Format("resources_{0}", i + 1);
			Slot slot = this.resourceSpine.Skeleton.FindSlot(slotName);
			slot.Data.AttachmentName = attachmentName;
			slot.Attachment = this.resourceSpine.Skeleton.GetAttachment(slotName, attachmentName);
		}
		this.resourceSpine.AnimationState.ClearTracks();
		this.resourceSpine.Skeleton.SetToSetupPose();
		this.resourceSpine.Skeleton.SetSlotsToSetupPose();
		this.resourceSpine.Skeleton.UpdateWorldTransform();
		this.resourceSpine.AnimationState.SetAnimation(0, "animation", false);
		base.DelayCall(delegate
		{
			this.resourceSpine.GetComponent<CanvasGroup>().alpha = 1f;
		}, 0.05f);
	}

	// Token: 0x06003B24 RID: 15140 RVA: 0x001DF2B0 File Offset: 0x001DD4B0
	private void PlayItemAnimation(MapPickup pickup, string text, string iconName, GameObject effect)
	{
		this.HideItemEffects();
		this.itemAnimationRoot.SetActive(true);
		bool needText = text != null;
		bool flag = needText;
		if (flag)
		{
			this.itemNameRoot.gameObject.SetActive(true);
			this.itemNameLabel.text = text;
			this.itemNameRoot.localScale = new Vector3(1f, 0f, 1f);
			this.itemNameRoot.DOKill(false);
			this.itemNameRoot.DOScaleY(1f, 0.08f).SetDelay(0.15f);
		}
		else
		{
			this.itemNameRoot.gameObject.SetActive(false);
		}
		this.itemIcon.SetSprite(iconName, true, null);
		CanvasGroup canvasGroup = this.itemAnimationRoot.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 1f;
		RectTransform rectTransform = this.itemAnimationRoot.GetComponent<RectTransform>();
		rectTransform.anchoredPosition = Vector2.zero;
		rectTransform.localScale = Vector3.zero;
		rectTransform.DOKill(false);
		rectTransform.DOLocalMoveY(120f, 1f, false).SetEase(this.itemAnimationCurve).OnComplete(new TweenCallback(this.DestroySelf));
		rectTransform.DOScale(Vector3.one, 0.15f);
		canvasGroup.DOKill(false);
		canvasGroup.DOFade(0f, 0.15f).SetDelay(0.85f);
		this.PlayEffect(effect);
	}

	// Token: 0x06003B25 RID: 15141 RVA: 0x001DF41E File Offset: 0x001DD61E
	private void HideItemEffects()
	{
		this.growthEffect.SetActive(false);
		this.itemEffect.SetActive(false);
	}

	// Token: 0x06003B26 RID: 15142 RVA: 0x001DF43C File Offset: 0x001DD63C
	private void PlayEffect(GameObject effectRoot)
	{
		effectRoot.SetActive(true);
		Transform top = effectRoot.transform.Find("Top");
		for (int i = 0; i < top.childCount; i++)
		{
			top.GetChild(i).GetComponent<ParticleSystem>().Play();
		}
	}

	// Token: 0x06003B2A RID: 15146 RVA: 0x001DF4E4 File Offset: 0x001DD6E4
	[CompilerGenerated]
	internal static string <PlayResourceSpine>g__GetAttachmentName|7_1(sbyte resourceType)
	{
		string result;
		switch (resourceType)
		{
		case 0:
			result = "images/food";
			break;
		case 1:
			result = "images/wood";
			break;
		case 2:
			result = "images/stone";
			break;
		case 3:
			result = "images/jade";
			break;
		case 4:
			result = "images/silk";
			break;
		case 5:
			result = "images/herbal";
			break;
		case 6:
			result = "images/money";
			break;
		case 7:
			result = "images/authority";
			break;
		default:
			result = null;
			break;
		}
		return result;
	}

	// Token: 0x04002A81 RID: 10881
	private MapElementPickupEffect _parent;

	// Token: 0x04002A82 RID: 10882
	public GameObject growthEffect;

	// Token: 0x04002A83 RID: 10883
	public GameObject itemEffect;

	// Token: 0x04002A84 RID: 10884
	public SkeletonGraphic resourceSpine;

	// Token: 0x04002A85 RID: 10885
	public GameObject itemAnimationRoot;

	// Token: 0x04002A86 RID: 10886
	public RectTransform itemNameRoot;

	// Token: 0x04002A87 RID: 10887
	public TextMeshProUGUI itemNameLabel;

	// Token: 0x04002A88 RID: 10888
	public CImage itemIcon;

	// Token: 0x04002A89 RID: 10889
	public SmallCombatSkillView combatSkillView;

	// Token: 0x04002A8A RID: 10890
	public AnimationCurve itemAnimationCurve;
}
