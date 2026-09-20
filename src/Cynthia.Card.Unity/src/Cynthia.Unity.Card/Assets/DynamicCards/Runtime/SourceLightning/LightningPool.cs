using System.Collections.Generic;

namespace Assets.Script.DynamicCards.SourceLightning
{

public static class LightningPool
{
	private const int BRANCHES_INITIAL_POOL_SIZE = 2048;

	private static Queue<LightningTool.Branch> m_BranchesPool;

	static LightningPool()
	{
		if (m_BranchesPool == null)
		{
			Initialize();
		}
	}

	public static void Initialize()
	{
		if (m_BranchesPool == null)
		{
			m_BranchesPool = new Queue<LightningTool.Branch>(2048);
			for (int i = 0; i < 2048; i++)
			{
				m_BranchesPool.Enqueue(new LightningTool.Branch());
			}
		}
	}

	public static LightningTool.Branch GetEmptyBranch()
	{
		if (m_BranchesPool.Count > 0)
		{
			return m_BranchesPool.Dequeue();
		}
		return new LightningTool.Branch();
	}
}

}
