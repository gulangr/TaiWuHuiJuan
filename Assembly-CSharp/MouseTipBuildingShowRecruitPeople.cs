using System;
using System.Collections.Generic;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000277 RID: 631
public class MouseTipBuildingShowRecruitPeople : MouseTipBase
{
	// Token: 0x17000482 RID: 1154
	// (get) Token: 0x06002929 RID: 10537 RVA: 0x00132F64 File Offset: 0x00131164
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x00132F68 File Offset: 0x00131168
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get<List<IntPair>>("charList", out this._charList);
		argsBox.Get<BuildingBlockKey>("blockKey", out this._buildingBlockKey);
		sbyte maxCount;
		argsBox.Get("maxCount", out maxCount);
		this.title.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Building_ShopPeopleCount, this._charList.Count, maxCount), true);
		this.ShowPeople();
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x00132FDC File Offset: 0x001311DC
	private void ShowPeople()
	{
		CommonUtils.PrepareEnoughChildren(this.peopleHolder, this.peoplePrefab.gameObject, this._charList.Count, null);
		for (int i = 0; i < this._charList.Count; i++)
		{
			this.SetupCharCell(this.peopleHolder.GetChild(i).GetComponent<Refers>(), i);
		}
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x0013304C File Offset: 0x0013124C
	private void SetupCharCell(Refers avatarWithName, int index)
	{
		ExtraDomainMethod.AsyncCall.RequestRecruitCharacterData(this, this._buildingBlockKey, index, delegate(int offset, RawDataPool pool)
		{
			RecruitCharacterData recruitCharacterData = null;
			Serializer.Deserialize(pool, offset, ref recruitCharacterData);
			bool flag = recruitCharacterData != null;
			if (flag)
			{
				ValueTuple<string, string> name = recruitCharacterData.FullName.GetName(recruitCharacterData.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
				string surname = name.Item1;
				string givenName = name.Item2;
				avatarWithName.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(recruitCharacterData.GenerateAvatarRelatedData());
				avatarWithName.CGet<TextMeshProUGUI>("Name").text = surname + givenName;
			}
		});
	}

	// Token: 0x04001DFB RID: 7675
	[SerializeField]
	private TextMeshProUGUI title;

	// Token: 0x04001DFC RID: 7676
	[SerializeField]
	private Refers peoplePrefab;

	// Token: 0x04001DFD RID: 7677
	[SerializeField]
	private Transform peopleHolder;

	// Token: 0x04001DFE RID: 7678
	private List<IntPair> _charList;

	// Token: 0x04001DFF RID: 7679
	private BuildingBlockKey _buildingBlockKey;
}
