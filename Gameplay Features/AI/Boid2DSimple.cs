using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A boid simple implementation which listens to the three rules cohesion, separation and alignment. Additionally, object attraction and avoidance can be set.
/// Requires <see cref="ColliderInfo"/> to work.
/// </summary>
public class Boid2DSimple : Boid2DBase
{
    protected override bool AdditionalAttractionCheck(Collider2D nearest)
    {
        return true;
    }

    protected override bool AdditionalAvoidanceCheck(Collider2D nearest)
    {
        return true;
    }

    protected override bool AdditionalBoidCheck(Collider2D nearest)
    {
        return true;
    }
}
