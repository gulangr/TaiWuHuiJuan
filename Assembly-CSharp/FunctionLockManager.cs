using System;
using FrameWork;

// Token: 0x0200012B RID: 299
public class FunctionLockManager : ISingletonInit, IDisposable
{
	// Token: 0x06000CBE RID: 3262 RVA: 0x00054833 File Offset: 0x00052A33
	public void Dispose()
	{
		this._functionUnlockStates = null;
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x0005483D File Offset: 0x00052A3D
	public void Init()
	{
		this._init = false;
		this._functionUnlockStates = new bool[64];
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00054854 File Offset: 0x00052A54
	public bool IsFunctionUnlock(byte functionId)
	{
		bool flag = this._functionUnlockStates.CheckIndex((int)functionId);
		return flag && this._functionUnlockStates[(int)functionId];
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00054884 File Offset: 0x00052A84
	public void UpdateFunctionUnlockStates(ulong stateFlag)
	{
		byte i = 0;
		while ((int)i < this._functionUnlockStates.Length)
		{
			bool preState = this._functionUnlockStates[(int)i];
			bool newState = (stateFlag & 1UL << (int)i) > 0UL;
			this._functionUnlockStates[(int)i] = newState;
			bool flag = preState != newState && this._init;
			if (flag)
			{
				GEvent.OnEvent(EEvents.OnFunctionLockStateChange, EasyPool.Get<ArgumentBox>().Set("FunctionId", i));
			}
			i += 1;
		}
		this._init = true;
	}

	// Token: 0x04000DD8 RID: 3544
	private bool _init;

	// Token: 0x04000DD9 RID: 3545
	private bool[] _functionUnlockStates;
}
