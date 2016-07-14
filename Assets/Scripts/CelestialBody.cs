using UnityEngine;
using System.Collections;

public abstract class CelestialBody 
{

	protected int seed;


	public float radius;//radius of the planet
	protected float scaledRadius;//the radius of the scaledRep in unispace
	protected float atmosRadius;//the radius of the atmosphere/direct gravitational influence/distance terrain is split from(distance from planet to make it curplanet)
	public float scaledAtmosRadius;//atmosRadius in unispace
	public float buildHeight;//the height the player must under for surface objects to be generated, and consequently, the build height (is actually radius+build height but whatever)

	//the position of the planet in stellar space
	public LongPos scaledPos;

	//the large scale representation of the planet in unispace
	public GameObject scaledRep;

	public float gravity;


	public CelestialBody(int _seed, float r, LongPos pos)
	{
		radius = r;
//		scaledRadius = r/PositionController.;
		seed = _seed;
		scaledPos = pos;

		atmosRadius = r+200000;//atmosphere is 200 km above surface
		//scaledAtmosRadius = atmosRadius/Unitracker.uniscale;

		buildHeight = r+10000;//build height is 10 km above surface

		//gravity = 9.8f;
		//TODO: use real gravity equations
		gravity = radius/32000-1;

	}

	protected abstract void createRep();

}
