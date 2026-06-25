using System;
using System.Collections.Generic;
using FrameWork;
using Game.Views.Following;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020003C1 RID: 961
public class FollowingChar : Refers
{
	// Token: 0x170005E9 RID: 1513
	// (get) Token: 0x06003A21 RID: 14881 RVA: 0x001D9AEE File Offset: 0x001D7CEE
	public int CharId
	{
		get
		{
			return this.UserInt;
		}
	}

	// Token: 0x170005EA RID: 1514
	// (get) Token: 0x06003A22 RID: 14882 RVA: 0x001D9AF6 File Offset: 0x001D7CF6
	public List<LoongInfo> LoongInfos
	{
		get
		{
			FollowingCharCommonData data = this.Data;
			return (data != null) ? data.LoongInfos : null;
		}
	}

	// Token: 0x170005EB RID: 1515
	// (get) Token: 0x06003A23 RID: 14883 RVA: 0x001D9B0A File Offset: 0x001D7D0A
	public HashSet<int> UsingLocalNickNameIdSet
	{
		get
		{
			FollowingCharCommonData data = this.Data;
			return (data != null) ? data.UsingLocalNickNameIdSet : null;
		}
	}

	// Token: 0x170005EC RID: 1516
	// (get) Token: 0x06003A24 RID: 14884 RVA: 0x001D9B1E File Offset: 0x001D7D1E
	public IAsyncMethodRequestHandler RequestHandler
	{
		get
		{
			FollowingCharCommonData data = this.Data;
			return (data != null) ? data.RequestHandler : null;
		}
	}

	// Token: 0x06003A25 RID: 14885 RVA: 0x001D9B34 File Offset: 0x001D7D34
	public void Init(bool canInteract, MapBlockData mapBlock, CharacterDisplayData characterDisplayData)
	{
		this.dead.gameObject.SetActive(false);
		this.alive.Init(canInteract, mapBlock, characterDisplayData, this.LoongInfos);
		this.alive.DisableButtonClick();
		this.alive.Button.ClearAndAddListener(delegate
		{
			this.OnCharClicked(true);
		});
		this.location = characterDisplayData.Location;
		this.alive.gameObject.SetActive(true);
	}

	// Token: 0x06003A26 RID: 14886 RVA: 0x001D9BB0 File Offset: 0x001D7DB0
	public void Init(bool canInteract, MapBlockData mapBlock, GraveDisplayData graveDisplayData)
	{
		this.alive.gameObject.SetActive(false);
		this.dead.Init(canInteract, mapBlock, graveDisplayData);
		this.dead.DisableButtonClick();
		this.dead.Button.ClearAndAddListener(delegate
		{
			this.OnCharClicked(false);
		});
		this.location = ((graveDisplayData != null) ? graveDisplayData.Location : Location.Invalid);
		this.dead.gameObject.SetActive(true);
	}

	// Token: 0x06003A27 RID: 14887 RVA: 0x001D9C30 File Offset: 0x001D7E30
	public void OnCharClicked(bool isAlive)
	{
		(isAlive ? this.OnAliveCharacterClicked : this.OnDeadCharacterClicked)(this.CharId);
	}

	// Token: 0x06003A28 RID: 14888 RVA: 0x001D9C50 File Offset: 0x001D7E50
	public void SetIsSelect(bool isSelect)
	{
		this.selectObj.SetActive(isSelect);
	}

	// Token: 0x06003A29 RID: 14889 RVA: 0x001D9C60 File Offset: 0x001D7E60
	public void Unfollow()
	{
		TaiwuDomainMethod.Call.TaiwuUnfollowNpc(this.CharId);
		GEvent.OnEvent(UiEvents.NickNameChanged, EasyPool.Get<ArgumentBox>().Set("CharacterId", this.CharId).Set("NickNameKey", -1).Set("NickName", ""));
	}

	// Token: 0x06003A2A RID: 14890 RVA: 0x001D9CBC File Offset: 0x001D7EBC
	public void StartRename(Renamer renamer, string _)
	{
		renamer.gameObject.SetActive(true);
		foreach (TMP_Text nameLabel in this.nameLabels)
		{
			nameLabel.gameObject.SetActive(false);
		}
		renamer.Input.text = (renamer.Name.text = this.nameLabels[this.alive.gameObject.activeSelf ? 0 : 1].text);
		bool flag = this.Data != null;
		if (flag)
		{
			renamer.TriggerStartRename();
		}
	}

	// Token: 0x06003A2B RID: 14891 RVA: 0x001D9D54 File Offset: 0x001D7F54
	public void OnRenameEnd(Renamer renamer, string _)
	{
		renamer.gameObject.SetActive(false);
		foreach (TMP_Text nameLabel in this.nameLabels)
		{
			nameLabel.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003A2C RID: 14892 RVA: 0x001D9D98 File Offset: 0x001D7F98
	public void OnSubmitRenameDone(Renamer renamer, string newName)
	{
		TaiwuDomainMethod.AsyncCall.SetFollowingNpcNickName(this.RequestHandler, this.CharId, newName, delegate(int offset, RawDataPool pool)
		{
			int nickNameKey = -1;
			Serializer.Deserialize(pool, offset, ref nickNameKey);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", this.CharId);
			argBox.Set("NickNameKey", nickNameKey);
			argBox.Set("NickName", newName);
			Coroutine newCoroutine = SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				GEvent.OnEvent(UiEvents.NickNameChanged, argBox);
			});
		});
		foreach (TMP_Text nameLabel in this.nameLabels)
		{
			nameLabel.text = newName;
		}
	}

	// Token: 0x040029EC RID: 10732
	[SerializeField]
	private ViewFollowing parent;

	// Token: 0x040029ED RID: 10733
	[SerializeField]
	private MapBlockCharNormal2 alive;

	// Token: 0x040029EE RID: 10734
	[SerializeField]
	private MapBlockCharGrave2 dead;

	// Token: 0x040029EF RID: 10735
	[SerializeField]
	private CButtonObsolete unfollow;

	// Token: 0x040029F0 RID: 10736
	[SerializeField]
	private CButtonObsolete rename;

	// Token: 0x040029F1 RID: 10737
	[SerializeField]
	private TMP_Text[] nameLabels;

	// Token: 0x040029F2 RID: 10738
	public FollowingCharCommonData Data;

	// Token: 0x040029F3 RID: 10739
	public Location location = Location.Invalid;

	// Token: 0x040029F4 RID: 10740
	[NonSerialized]
	public Action<int> OnAliveCharacterClicked = delegate(int _)
	{
	};

	// Token: 0x040029F5 RID: 10741
	[NonSerialized]
	public Action<int> OnDeadCharacterClicked = delegate(int _)
	{
	};

	// Token: 0x040029F6 RID: 10742
	[SerializeField]
	private GameObject selectObj;
}
