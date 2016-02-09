using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//handles the generation of terrain in multiple levels of detail
public class LODSystem
{

	private Planet planet;//a reference to its planet
	int chunkSize = TerrainObject.chunkSize;//voxels per chunk side

	public Dictionary<LODPos, TerrainObject> chunks = new Dictionary<LODPos, TerrainObject>();

	//the level at which chunks appear in unispace rather than in normal space
	int uniCutoff = 5;

	public LODSystem(Planet p)
	{
		planet = p;
	}


	//creates and instantiates a terrain chunk (but does not render it yet)
	public void CreateChunk(LODPos pos) 
	{
		//build the terrainobject and add its gameobject to the chunks list(may remove this last thing later)
		//TerrainObject chunk = Build.buildObject<TerrainObject>(pos.toVector3(), Quaternion.identity);
		GameObject terrainGO = new GameObject("Terrain Chunk lev: " + pos.level);
		TerrainObject chunk = terrainGO.AddComponent<TerrainObject>();
		chunks.Add(pos, chunk);

		//this lod chunk scale
		int scale = (int)(Mathf.Pow(2,pos.level));
		chunk.scale = scale;

		//if it should be in unispace
		if(pos.level == 14)
		{

			//add it to unispace and make its parent proud
			terrainGO.transform.SetParent(planet.scaledRep.transform);
			terrainGO.layer = 8;//add to Unispace layer


			//finds the adjusted scale of this terrain obj when in unispace
			float absScale = ((float)scale)/Unitracker.uniscale;
			terrainGO.transform.localScale = new Vector3(absScale, absScale, absScale);

			//the adjusted position of this terrain obj whin in unispace
			terrainGO.transform.localPosition = pos.toVector3()*16*absScale;


		}
		else
		{
			terrainGO.transform.localScale = new Vector3(scale, scale, scale);


		}

		//loops through every voxel in the chunk (make own funtion later)
		for (int x = 0; x<=chunkSize; x++) 
		{
			for (int y = 0; y<=chunkSize; y++) 
			{
				for (int z = 0; z<=chunkSize; z++) 
				{
					
					//the world position of the current voxel
					Vector3 voxPos = new Vector3();//position of chunk+position of voxel within chunk
					voxPos.x = (pos.x*16+x+0.0f)*scale;
					voxPos.y = (pos.y*16+y+0.0f)*scale;
					voxPos.z = (pos.z*16+z+0.0f)*scale;

	
					//the distance from the center of the planet to the current voxel
					float distxyz = Vector3.Distance(Vector3.zero, voxPos);
					
					//the height of land below (or above) this voxel
					float altitude = planet.noise.getAltitude(voxPos);

					//fills in the appropriate voxel data for marching cubes
					chunk.voxVals[x,y,z] = distxyz/altitude;//Noise.GetNoise((x+pos.x)/scale,(y+pos.y)/scale,(z+pos.z)/scale);
					
					//puts a hole in the planet(just for fun
					//if(voxPos.x<10 && voxPos.x>-10 && voxPos.z<10 && voxPos.z>-10)
					//chunk.voxVals[x,y,z] = 2;
					
					
				}
				
			}
			
		}
		
		//TerrainLoader.addToRender(chunk);
		//Loader.addToRender(chunk);
		chunk.Render();//renders the chunk (be sure to remove later)
		
	}

	//public 

}

//position of a lod chunk
public struct LODPos
{
	public int level, x, y, z;
	public LODPos(int lev, int xp, int yp, int zp)
	{
		level = lev;
		x = xp;
		y = yp;
		z = zp;

	}

	public Vector3 toVector3()
	{
		return new Vector3(x,y,z);
	}
}