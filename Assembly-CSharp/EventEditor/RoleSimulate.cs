using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Migrate;
using GameData.Domains.Character.AvatarSystem;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000638 RID: 1592
	public class RoleSimulate : MonoBehaviour
	{
		// Token: 0x06004B47 RID: 19271 RVA: 0x00236440 File Offset: 0x00234640
		public void Init()
		{
			this._allRoles = new List<EventEditorRole>();
			this.scroll.OnItemRender += this.OnRoleItemRender;
			this.btnAddNewRole.ClearAndAddListener(new Action(this.OnAddRole));
			GameObject scrollPrefab = this.scroll.srcPrefab;
			RoleSimulateRoleObjectInfo roleObjectInfo = scrollPrefab.GetComponent<RoleSimulateRoleObjectInfo>();
			CDropdown behaviorDropdown = roleObjectInfo.behaviorDropDown;
			behaviorDropdown.ClearOptions();
			behaviorDropdown.AddOptions(new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_Goodness_0),
				LocalStringManager.Get(LanguageKey.LK_Goodness_1),
				LocalStringManager.Get(LanguageKey.LK_Goodness_2),
				LocalStringManager.Get(LanguageKey.LK_Goodness_3),
				LocalStringManager.Get(LanguageKey.LK_Goodness_4)
			});
			CDropdown identityDropdown = roleObjectInfo.identityDropDown;
			identityDropdown.ClearOptions();
			identityDropdown.AddOptions(new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_Identity_1_A),
				LocalStringManager.Get(LanguageKey.LK_Identity_1_B),
				LocalStringManager.Get(LanguageKey.LK_Identity_2_A),
				LocalStringManager.Get(LanguageKey.LK_Identity_2_B),
				LocalStringManager.Get(LanguageKey.LK_Identity_3_A),
				LocalStringManager.Get(LanguageKey.LK_Identity_3_B),
				LocalStringManager.Get(LanguageKey.LK_Identity_4_A),
				LocalStringManager.Get(LanguageKey.LK_Identity_4_B),
				LocalStringManager.Get(LanguageKey.LK_Identity_5_A),
				LocalStringManager.Get(LanguageKey.LK_Identity_5_B),
				LocalStringManager.Get(LanguageKey.LK_Identity_6_A),
				LocalStringManager.Get(LanguageKey.LK_Identity_6_B),
				LocalStringManager.Get(LanguageKey.LK_Identity_7_A),
				LocalStringManager.Get(LanguageKey.LK_Identity_7_B),
				LocalStringManager.Get(LanguageKey.LK_Identity_8_A),
				LocalStringManager.Get(LanguageKey.LK_Identity_8_B),
				LocalStringManager.Get(LanguageKey.LK_Identity_9_A),
				LocalStringManager.Get(LanguageKey.LK_Identity_9_B)
			});
		}

		// Token: 0x06004B48 RID: 19272 RVA: 0x00236657 File Offset: 0x00234857
		public void Refresh()
		{
			this.scroll.UpdateData(this._allRoles.Count);
		}

		// Token: 0x06004B49 RID: 19273 RVA: 0x00236674 File Offset: 0x00234874
		private void OnRoleItemRender(int index, GameObject roleObject)
		{
			EventEditorRole role = this._allRoles[index];
			RoleSimulateRoleObjectInfo objectInfo = roleObject.GetComponent<RoleSimulateRoleObjectInfo>();
			objectInfo.avatar.Refresh(role.AvatarData, (short)role.Age);
			objectInfo.roleKeyInput.onEndEdit.RemoveAllListeners();
			objectInfo.roleKeyInput.text = role.Key;
			objectInfo.roleKeyInput.onEndEdit.AddListener(delegate(string str)
			{
				role.Key = str;
				bool flag = str == "RoleTaiwu";
				if (flag)
				{
					this._roleTaiwuIndex = index;
					this.OnSetRoleTaiwu();
				}
				role.Dirty = true;
			});
			objectInfo.roleNameInput.onEndEdit.RemoveAllListeners();
			objectInfo.roleNameInput.text = role.GetName();
			objectInfo.roleNameInput.onEndEdit.AddListener(delegate(string str)
			{
				role.CustomName = str;
				role.Dirty = true;
			});
			objectInfo.btnRandomName.ClearAndAddListener(delegate
			{
				role.CustomName = string.Empty;
				objectInfo.roleNameInput.text = role.GetName();
				role.Dirty = true;
			});
			objectInfo.roleAgeInput.onEndEdit.RemoveAllListeners();
			objectInfo.roleAgeInput.text = role.Age.ToString();
			objectInfo.roleAgeInput.onEndEdit.AddListener(delegate(string str)
			{
				byte age;
				bool flag = !byte.TryParse(str, out age);
				if (flag)
				{
					objectInfo.roleAgeInput.text = role.Age.ToString();
				}
				else
				{
					age = (byte)Mathf.Clamp((int)age, 16, 150);
					role.Age = age;
					bool flag2 = age.ToString() != str;
					if (flag2)
					{
						objectInfo.roleAgeInput.text = age.ToString();
					}
					objectInfo.avatar.AvatarAge = (short)role.Age;
					objectInfo.avatar.Refresh();
					role.Dirty = true;
				}
			});
			objectInfo.behaviorDropDown.onValueChanged.RemoveAllListeners();
			objectInfo.behaviorDropDown.value = role.Behavior - 1;
			objectInfo.behaviorDropDown.onValueChanged.AddListener(delegate(int behaviorIndex)
			{
				role.Behavior = behaviorIndex + 1;
				role.Dirty = true;
			});
			objectInfo.identityDropDown.onValueChanged.RemoveAllListeners();
			objectInfo.identityDropDown.value = role.Identity;
			objectInfo.identityDropDown.onValueChanged.AddListener(delegate(int identityIndex)
			{
				role.Identity = identityIndex;
				role.Dirty = true;
			});
			objectInfo.btnRandom.ClearAndAddListener(delegate
			{
				AvatarData avatarData = SingletonObject.getInstance<AvatarManager>().GetRandomAvatar(GameApp.Random, role.AvatarData.Gender, false, -1, null, null);
				role.AvatarData.Copy(avatarData);
				objectInfo.avatar.Refresh();
				role.Dirty = true;
			});
			objectInfo.btnPreset.ClearAndAddListener(delegate
			{
				OpenFileName openFileName = new OpenFileName();
				openFileName.structSize = Marshal.SizeOf<OpenFileName>(openFileName);
				openFileName.filter = "(*.twa)\0*.twa";
				openFileName.file = new string(new char[256]);
				openFileName.maxFile = openFileName.file.Length;
				openFileName.fileTitle = new string(new char[64]);
				openFileName.maxFileTitle = openFileName.fileTitle.Length;
				openFileName.initialDir = PlayerPrefs.GetString("LastAvatarPresetDirectory", Application.dataPath.Replace('/', '\\'));
				openFileName.flags = 530440;
				bool unityOpenFileName = LocalDialog.GetUnityOpenFileName(openFileName);
				if (unityOpenFileName)
				{
					bool flag = !File.Exists(openFileName.file) || !openFileName.file.EndsWith(".twa");
					if (flag)
					{
						return;
					}
					Dictionary<string, string> infoMap = null;
					AvatarData loadAvatar = AvatarDataSaveLoadHelper.Load(openFileName.file, ref infoMap);
					bool flag2 = loadAvatar != null;
					if (flag2)
					{
						role.AvatarData.Copy(loadAvatar);
						objectInfo.avatar.Refresh();
						PlayerPrefs.SetString("LastAvatarPresetDirectory", new FileInfo(openFileName.file).DirectoryName);
					}
				}
				role.Dirty = true;
			});
			objectInfo.btnDeleteRole.ClearAndAddListener(delegate
			{
				this.OnDeleteRole(index);
			});
		}

		// Token: 0x06004B4A RID: 19274 RVA: 0x00236900 File Offset: 0x00234B00
		public EventEditorRole GetRole(string key)
		{
			List<EventEditorRole> allRoles = this._allRoles;
			return (allRoles != null) ? allRoles.Find((EventEditorRole e) => e.Key == key) : null;
		}

		// Token: 0x06004B4B RID: 19275 RVA: 0x0023693D File Offset: 0x00234B3D
		public void SendDirtyInfo()
		{
			this._allRoles.ForEach(delegate(EventEditorRole e)
			{
				e.SendDirty();
			});
		}

		// Token: 0x06004B4C RID: 19276 RVA: 0x0023696C File Offset: 0x00234B6C
		private void OnAddRole()
		{
			EventEditorRole role = new EventEditorRole
			{
				Age = (byte)GameApp.RandomRange(16, 150),
				AvatarData = SingletonObject.getInstance<AvatarManager>().GetRandomAvatar(GameApp.Random, -1, false, -1, null, null),
				Behavior = GameApp.RandomRange(1, 6),
				Identity = GameApp.RandomRange(0, 18)
			};
			this._allRoles.Add(role);
			this.Refresh();
		}

		// Token: 0x06004B4D RID: 19277 RVA: 0x002369DC File Offset: 0x00234BDC
		private void OnDeleteRole(int index)
		{
			bool flag = this._allRoles.CheckIndex(index);
			if (flag)
			{
				this._allRoles.RemoveAt(index);
				this.Refresh();
				EventEditorSimulateEnvironment.Instance.RolePageDirty = true;
			}
		}

		// Token: 0x06004B4E RID: 19278 RVA: 0x00236A1C File Offset: 0x00234C1C
		private void OnSetRoleTaiwu()
		{
			for (int i = 0; i < this._allRoles.Count; i++)
			{
				bool flag = this._allRoles[i].Key == "RoleTaiwu" && i != this._roleTaiwuIndex;
				if (flag)
				{
					this._allRoles[i].Key = string.Empty;
				}
			}
			this.Refresh();
		}

		// Token: 0x0400344D RID: 13389
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x0400344E RID: 13390
		[SerializeField]
		private CButton btnAddNewRole;

		// Token: 0x0400344F RID: 13391
		private List<EventEditorRole> _allRoles;

		// Token: 0x04003450 RID: 13392
		private int _roleTaiwuIndex;

		// Token: 0x04003451 RID: 13393
		private const string RoleTaiwu = "RoleTaiwu";
	}
}
