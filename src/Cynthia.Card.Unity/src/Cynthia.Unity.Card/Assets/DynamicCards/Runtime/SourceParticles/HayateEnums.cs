// Recovered from the supplied Unity 2017 card particle implementation.
// Namespace isolated; editor-time simulation is intentionally omitted.
namespace Assets.Script.DynamicCards.SourceParticles
{
public static class HayateEnums
{
	public enum CalculationMethod
	{
		none,
		sine,
		cosine,
		animationCurve,
		perlin,
		precalculatedTexture,
		simplex
	}

	public enum TurbulenceType
	{
		relative,
		absolute
	}

	public enum DivisionType
	{
		center,
		edge
	}

	public enum AssignTo
	{
		velocity,
		position
	}

	public enum MeshFollow
	{
		byDistance,
		byTime,
		physical
	}

	public enum BuildOrder
	{
		TopBottom,
		TopLeftBottomRight,
		LeftRight,
		BottomLeftTopRight,
		BottomTop,
		BottomRightTopLeft,
		RightLeft,
		TopRightBottomLeft
	}
}

}
