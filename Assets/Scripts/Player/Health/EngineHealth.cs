using UnityEngine;
using System.Collections;

public class EngineHealth : Health {

	[Header("Effects")]
	public ParticleSystem smokeByTime;
	public ParticleSystem smokeByDist;
//	public ParticleSystem flame;

	public override void CheckForSpriteChanging ()
	{
		base.CheckForSpriteChanging ();
		if (spriteCount - spriteNum > 1){
			smokeByTime.emissionRate = (1f - healthPercent) * 10f;
			smokeByDist.emissionRate = (1f - healthPercent);
		} else {
			smokeByTime.emissionRate = 0;
			smokeByDist.emissionRate = 0;
		}
	}

}
