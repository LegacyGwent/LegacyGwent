// Recovered original source behavior; namespace isolated.
using UnityEngine;

namespace Assets.Script.DynamicCards.SourceParticles
{
public class Braenn_BowFix : MonoBehaviour
{
	public Transform[] followingObjects;

	private int affectedObjectNumber;

	private void Start()
	{
		affectedObjectNumber = followingObjects.Length;
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		for (int i = 0; i < affectedObjectNumber; i++)
		{
			followingObjects[i].position = position;
		}
	}
}

}
