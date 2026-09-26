// Original lightning behavior, with isolated namespace and explicit source shader.
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.DynamicCards.SourceLightning
{

public class LightningToolAnimationListener : MonoBehaviour
{
	public List<LightningAnimator> LightningTools = new List<LightningAnimator>();

	public void Play(int _index)
	{
		if (_index < LightningTools.Count && LightningTools[_index] != null)
		{
			LightningTools[_index].Play();
		}
	}
}

}
