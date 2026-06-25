using System;
using Config;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using TMPro;
using UICommon.Character;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class SecretInformationElement : MonoBehaviour
{
	// Token: 0x06002287 RID: 8839 RVA: 0x00100160 File Offset: 0x000FE360
	public void Clear()
	{
		bool flag = null != this.Avatar && this._currDisplayMode != SecretInformationElement.DisplayMode.Avatar;
		if (flag)
		{
			SecretInformationElementPool.Instance.DestroyMaskedAvatar(this.Avatar.transform.parent.parent.gameObject);
			this.Avatar = null;
		}
		bool flag2 = null != this.ItemView && this._currDisplayMode != SecretInformationElement.DisplayMode.Item;
		if (flag2)
		{
			SecretInformationElementPool.Instance.DestroyItemView(this.ItemView.gameObject);
			this.ItemView = null;
		}
		this.Location.enabled = false;
		this.Skill.enabled = false;
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x00100218 File Offset: 0x000FE418
	public void DisplayAvatar(CharacterDisplayData characterDisplayData)
	{
		this._currDisplayMode = SecretInformationElement.DisplayMode.Avatar;
		this.Clear();
		bool flag = null == this.Avatar;
		if (flag)
		{
			GameObject maskedAvatar = SecretInformationElementPool.Instance.GetMaskedAvatar();
			maskedAvatar.transform.SetParent(this.Anchor, false);
			maskedAvatar.transform.localPosition = Vector3.zero;
			maskedAvatar.transform.localScale = Vector3.one;
			this.Avatar = maskedAvatar.transform.GetChild(0).GetChild(0).GetComponent<Game.Components.Avatar.Avatar>();
		}
		bool flag2 = characterDisplayData.AliveState == 1;
		if (flag2)
		{
			this.Avatar.ResetToBlank(false);
			ResLoader.LoadModOrGameResource<Sprite>(CharacterAvatar.SmallSizeGraveResPath, delegate(Sprite sprite)
			{
				this.Avatar.Refresh(sprite, new Vector2?(new Vector2(0f, 56f)));
			}, null);
		}
		else
		{
			this.Avatar.Refresh(characterDisplayData, true);
		}
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x001002EC File Offset: 0x000FE4EC
	public void DisplayLocation(Location location)
	{
		this._currDisplayMode = SecretInformationElement.DisplayMode.Location;
		this.Clear();
		bool flag = location.IsValid();
		if (flag)
		{
			MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
			bool flag2 = areas.CheckIndex((int)location.AreaId);
			if (flag2)
			{
				short areaTemplateId = areas[(int)location.AreaId].GetTemplateId();
				MapAreaItem areaConfig = MapArea.Instance.GetItem(areaTemplateId);
				bool flag3 = areaConfig != null;
				if (flag3)
				{
					this.Location.enabled = true;
					this.Location.SetSprite(areaConfig.BigMapIcon, false, null);
				}
			}
		}
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x0010037C File Offset: 0x000FE57C
	public void DisplayItem(ItemKey itemKey)
	{
		this._currDisplayMode = SecretInformationElement.DisplayMode.Item;
		this.Clear();
		bool flag = null == this.ItemView;
		if (flag)
		{
			GameObject itemViewObject = SecretInformationElementPool.Instance.GetItemView();
			itemViewObject.transform.SetParent(this.Anchor);
			itemViewObject.transform.localPosition = SecretInformationElement.ItemViewOffset;
			RectTransform itemViewRectTrans = itemViewObject.transform as RectTransform;
			itemViewRectTrans.pivot = 0.5f * Vector2.one;
			itemViewRectTrans.anchoredPosition = SecretInformationElement.ItemViewOffset;
			itemViewRectTrans.localScale = SecretInformationElement.ItemViewScale;
			this.ItemView = itemViewObject.GetComponent<ItemView>();
		}
		this.ItemView.SetData(new ItemDisplayData
		{
			Key = itemKey
		}, false, -1, false, false, null, true, true);
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x00100444 File Offset: 0x000FE644
	public void DisplayResourceType(sbyte resourceType)
	{
		this._currDisplayMode = SecretInformationElement.DisplayMode.ResourceType;
		this.Clear();
		ResourceTypeItem config = ResourceType.Instance.GetItem(resourceType);
		bool flag = config != null;
		if (flag)
		{
			this.Skill.enabled = true;
			this.Skill.SetSprite(config.Icon, true, null);
		}
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x00100498 File Offset: 0x000FE698
	public void DisplayLifeSkill(short lifeSkillTemplateId)
	{
		this._currDisplayMode = SecretInformationElement.DisplayMode.LifeSkill;
		this.Clear();
		LifeSkillItem config = LifeSkill.Instance.GetItem(lifeSkillTemplateId);
		bool flag = config != null;
		if (flag)
		{
			this.Skill.enabled = true;
			this.Skill.SetSprite(string.Format("sp_14_iconone_{0}", config.Type), true, null);
		}
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x001004FC File Offset: 0x000FE6FC
	public void DisplayCombatSkill(short combatSkillTemplateId)
	{
		this._currDisplayMode = SecretInformationElement.DisplayMode.CombatSkill;
		this.Clear();
		CombatSkillItem config = CombatSkill.Instance.GetItem(combatSkillTemplateId);
		bool flag = config != null;
		if (flag)
		{
			this.Skill.enabled = true;
			this.Skill.SetSprite(string.Format("sp_18_iconwuxue_{0}", config.Type), true, null);
		}
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x0010055E File Offset: 0x000FE75E
	public void DisplayInteger()
	{
		this._currDisplayMode = SecretInformationElement.DisplayMode.Integer;
		this.Clear();
	}

	// Token: 0x04001A8D RID: 6797
	private static readonly Vector2 ItemViewOffset = new Vector2(-6f, 2f);

	// Token: 0x04001A8E RID: 6798
	private static readonly Vector3 ItemViewScale = new Vector3(0.85f, 0.85f, 1f);

	// Token: 0x04001A8F RID: 6799
	public RectTransform Anchor;

	// Token: 0x04001A90 RID: 6800
	public TextMeshProUGUI UiName;

	// Token: 0x04001A91 RID: 6801
	public Game.Components.Avatar.Avatar Avatar;

	// Token: 0x04001A92 RID: 6802
	public ItemView ItemView;

	// Token: 0x04001A93 RID: 6803
	public CImage Location;

	// Token: 0x04001A94 RID: 6804
	public CImage Skill;

	// Token: 0x04001A95 RID: 6805
	private SecretInformationElement.DisplayMode _currDisplayMode = SecretInformationElement.DisplayMode.None;

	// Token: 0x020014F8 RID: 5368
	private enum DisplayMode
	{
		// Token: 0x0400A322 RID: 41762
		None,
		// Token: 0x0400A323 RID: 41763
		Avatar,
		// Token: 0x0400A324 RID: 41764
		Location,
		// Token: 0x0400A325 RID: 41765
		Item,
		// Token: 0x0400A326 RID: 41766
		ResourceType,
		// Token: 0x0400A327 RID: 41767
		LifeSkill,
		// Token: 0x0400A328 RID: 41768
		CombatSkill,
		// Token: 0x0400A329 RID: 41769
		Integer
	}
}
