using System;
using System.Collections.Generic;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

// Token: 0x02000339 RID: 825
public class SelectableCharacterLongItem : MonoBehaviour
{
	// Token: 0x1700054E RID: 1358
	// (get) Token: 0x06003015 RID: 12309 RVA: 0x00177D67 File Offset: 0x00175F67
	// (set) Token: 0x06003016 RID: 12310 RVA: 0x00177D6F File Offset: 0x00175F6F
	public bool Init { get; private set; }

	// Token: 0x1700054F RID: 1359
	// (get) Token: 0x06003017 RID: 12311 RVA: 0x00177D78 File Offset: 0x00175F78
	// (set) Token: 0x06003018 RID: 12312 RVA: 0x00177D80 File Offset: 0x00175F80
	public int CharacterId
	{
		get
		{
			return this._characterId;
		}
		set
		{
			bool flag = !this.Init;
			if (flag)
			{
				this.InitHandlerList();
			}
			this._characterId = value;
			this._handlerList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = this._characterId;
			});
			this.Refers.CGet<Refers>("FavorInfoCell").gameObject.SetActive(this.ShowFavor);
			this.Refers.CGet<Refers>("HappinessInfoCell").gameObject.SetActive(this.ShowHappiness);
			GEvent.Remove(UiEvents.RefreshFollowing, new GEvent.Callback(this.OnFollowingChanged));
			GEvent.Add(UiEvents.RefreshFollowing, new GEvent.Callback(this.OnFollowingChanged));
		}
	}

	// Token: 0x06003019 RID: 12313 RVA: 0x00177E3C File Offset: 0x0017603C
	private void OnFollowingChanged(ArgumentBox argbox)
	{
		int characterId;
		argbox.Get("CharacterId", out characterId);
		bool flag = characterId == this.CharacterId;
		if (flag)
		{
			this.RefreshByGetCharacterDisplayData(true);
		}
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x00177E70 File Offset: 0x00176070
	private void OnDisable()
	{
		this.CharacterId = -1;
		this._handlerList.Clear();
		this._handlerList = null;
		base.name = "-1";
		this.Init = false;
		GEvent.Remove(UiEvents.RefreshFollowing, new GEvent.Callback(this.OnFollowingChanged));
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x00177ECC File Offset: 0x001760CC
	private void InitHandlerList()
	{
		bool init = this.Init;
		if (!init)
		{
			this._handlerList = new List<CharacterUIElement>();
			this.Refers = base.GetComponent<Refers>();
			Game.Components.Avatar.Avatar avatar = this.Refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
			CharacterAvatar avatarHandler = new CharacterAvatar(avatar, true);
			this._handlerList.Add(avatarHandler);
			CharacterName nameHandler = new CharacterName(this.Refers.CGet<TextMeshProUGUI>("Name"), null, null);
			nameHandler.Anonymous = this.UseAnonymousName;
			this._handlerList.Add(nameHandler);
			Refers favorRefers = this.Refers.CGet<Refers>("FavorInfoCell");
			CImage icon = favorRefers.CGet<CImage>("Icon");
			TextMeshProUGUI favorLabel = favorRefers.CGet<TextMeshProUGUI>("InfoValue");
			CImage favorDebtIcon = favorRefers.CGet<CImage>("FavorDebt");
			CharacterFavorability favorabilityHandler = new CharacterFavorability(icon, favorLabel, null, null, favorDebtIcon);
			this._handlerList.Add(favorabilityHandler);
			CharacterHappiness happinessHandler = new CharacterHappiness(this.Refers.CGet<Refers>("HappinessInfoCell"), false);
			this._handlerList.Add(happinessHandler);
			this.FavorItem = favorabilityHandler;
			this.HappinessItem = happinessHandler;
			this.Init = true;
		}
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x00177FEC File Offset: 0x001761EC
	public void OnCharacterItemHoverIn()
	{
		bool flag = !this.Init;
		if (flag)
		{
			this.InitHandlerList();
		}
		bool flag2 = !this.Refers.CGet<CToggleObsolete>("Toggle").isOn;
		if (flag2)
		{
			this.Refers.CGet<GameObject>("HoverLight").SetActive(true);
		}
	}

	// Token: 0x0600301D RID: 12317 RVA: 0x00178044 File Offset: 0x00176244
	public void OnCharacterItemHoverOut()
	{
		bool flag = !this.Init;
		if (flag)
		{
			this.InitHandlerList();
		}
		this.Refers.CGet<GameObject>("HoverLight").SetActive(false);
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x00178080 File Offset: 0x00176280
	public void SetCharacterRelationShow(ushort relationTypes)
	{
		bool flag = relationTypes == ushort.MaxValue;
		if (flag)
		{
			this.Refers.CGet<GameObject>("Love").SetActive(false);
			this.Refers.CGet<GameObject>("Hate").SetActive(false);
		}
		else
		{
			bool hasLove = RelationType.HasRelation(relationTypes, 16384);
			bool hasHate = RelationType.HasRelation(relationTypes, 32768);
			this.Refers.CGet<GameObject>("Love").SetActive(hasLove);
			this.Refers.CGet<GameObject>("Hate").SetActive(hasHate);
		}
	}

	// Token: 0x0600301F RID: 12319 RVA: 0x00178114 File Offset: 0x00176314
	public void SetCharacterRelationDefaultShow()
	{
		this.Refers.CGet<GameObject>("Love").SetActive(false);
		this.Refers.CGet<GameObject>("Hate").SetActive(false);
	}

	// Token: 0x06003020 RID: 12320 RVA: 0x00178148 File Offset: 0x00176348
	public void RefreshByGetCharacterDisplayData(bool showFollowed = false)
	{
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this.CharacterId, delegate(int offset, RawDataPool pool)
		{
			CharacterDisplayData characterDisplayData = null;
			Serializer.Deserialize(pool, offset, ref characterDisplayData);
			this.RefreshByCharacterDisplayData(showFollowed, characterDisplayData);
		});
	}

	// Token: 0x06003021 RID: 12321 RVA: 0x00178184 File Offset: 0x00176384
	public void RefreshByCharacterDisplayData(bool showFollowed, CharacterDisplayData characterDisplayData)
	{
		this.Refers.CGet<TextMeshProUGUI>("OrganizationName").text = CommonUtils.GetOrganizationGradeString(characterDisplayData.OrgInfo, characterDisplayData.Gender, characterDisplayData.PhysiologicalAge, (int)characterDisplayData.TemplateId);
		this.Refers.CGet<CImage>("OrganizationIcon").gameObject.SetActive(true);
		this.Refers.CGet<CImage>("OrganizationIcon").SetSprite(CommonUtils.GetIdentityIcon(characterDisplayData.OrgInfo.Grade), false, null);
		bool flag = this._handlerList != null;
		if (flag)
		{
			this._handlerList.ForEach(delegate(CharacterUIElement e)
			{
				e.SetIsDead(characterDisplayData.AliveState > 0);
			});
		}
		GameObject followedMark;
		bool flag2 = this.Refers.CTryGet<GameObject>("FollowedMark", out followedMark);
		if (flag2)
		{
			followedMark.SetActive(showFollowed && characterDisplayData.IsFollowedByTaiwu);
		}
	}

	// Token: 0x040022E1 RID: 8929
	public bool UseAnonymousName;

	// Token: 0x040022E2 RID: 8930
	private int _characterId;

	// Token: 0x040022E3 RID: 8931
	public Refers Refers;

	// Token: 0x040022E4 RID: 8932
	public bool ShowHappiness;

	// Token: 0x040022E5 RID: 8933
	public bool ShowFavor;

	// Token: 0x040022E6 RID: 8934
	private List<CharacterUIElement> _handlerList;

	// Token: 0x040022E7 RID: 8935
	public CharacterFavorability FavorItem;

	// Token: 0x040022E8 RID: 8936
	public CharacterHappiness HappinessItem;
}
