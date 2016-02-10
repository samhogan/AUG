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
		//first check that the chunk contains data (or probably does)


		//build the terrainobject and add its gameobject to the chunks list(may remove this last thing later)
		//TerrainObject chunk = Build.buildObject<TerrainObject>(pos.toVector3(), Quaternion.identity);
		GameObject terrainGO = new GameObject("Terrain Chunk " + pos.ToString());
		TerrainObject chunk = terrainGO.AddComponent<TerrainObject>();
		chunks.Add(pos, chunk);

		//this lod chunk scale
		int scale = (int)(Mathf.Pow(2,pos.level));
		chunk.scale = scale;


		//positions and scales the gameobject (maybe should move this elsewhere)
		//if it should be in unispace
		if(pos.level > uniCutoff)
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
			terrainGO.transform.localPosition = Unitracker.getFloatingPos(pos.toVector3()*scale*16);

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
					voxPos.x = (pos.x*16+x)*scale;
					voxPos.y = (pos.y*16+y)*scale;
					voxPos.z = (pos.z*16+z)*scale;

	
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

	//splits up a terrain chunk into 8 smaller chunks 
	public void splitChunk(LODPos pos)
	{
		TerrainObject to;
		//don't do anything if the chunk list does not contain the pos
		if(!chunks.TryGetValue(pos, out to))
			return;

		//hide this terrain object but don't delete it
		to.gameObject.SetActive(false);
		//1 level lower
		int newLev = pos.level-1;
		//the position of the first subchunk in this chunk
		WorldPos newStart = new WorldPos(pos.x*2, pos.y*2, pos.z*2);

		//build all of the 8 chunks that contain land
		for(int x=0; x<=1; x++)
			for(int y=0; y<=1; y++)
				for(int z=0; z<=1; z++)
				{	
					LODPos newChunk = new LODPos(newLev, newStart.x+x, newStart.y+y, newStart.z+z);
					if(containsLand(newChunk))
						CreateChunk(newChunk);
				}

		//it is now split, this is kind of a useless comment and the more i type the more time i waste soo um yeah. how's your day been reader of my code? I know this comment is getting unjustifiably long so I should probably stop typing it but i wont today is feb. 10 2016 and i am currently working on the lod system for this game. hopefully this game will get popular in the future which will cause me to hire employees and one of them will see this comment and think "what the heck?" but then they will laugh and show it to their coworker and call me in from my luxury office to ask me if i remember writing this comment
		to.isSplit = true;

	}

	//generates a chunk if it will exist but first generates all chunk levels above it and subdivides them
	//returns true if the chunk is successfully created
	public bool requestChunk(LODPos pos)
	{
		//Debug.Log(pos.ToString());
		//if it is already built, we are done
		if(chunks.ContainsKey(pos))
			return true;

		LODPos abovePos = getAbove(pos);
		if(chunks.ContainsKey(getAbove(pos)))
		{
			TerrainObject above = null;
			chunks.TryGetValue(abovePos, out above);

			//if it is already split, the current terrian chunk will never exist
			if(above.isSplit)
				return false;
			else
			{
				//split the chunk and then check if the chunk exists now
				splitChunk(abovePos);
				return chunks.ContainsKey(pos);
			}
		}
		else//the above chunk does not exist
		{
			//request the above chunk to be generated, and if it is, split it and check if the current chunk is generated
			if(requestChunk(abovePos))
			{
				splitChunk(abovePos);
				return chunks.ContainsKey(pos);
			}

			return false;
		}
	}

	//returns the lodpos of 1 higher level that contains the given lodpos
	private LODPos getAbove(LODPos pos)
	{
		return new LODPos(pos.level+1,
		                  Mathf.FloorToInt(pos.x/2.0f),
		                  Mathf.FloorToInt(pos.y/2.0f),
		                  Mathf.FloorToInt(pos.z/2.0f));

	}

	//returns true if land (probably) exists within the given lod chunk
	public bool containsLand(LODPos pos)
	{
		//length of this specific lod chunk
		float sideLength = 16*Mathf.Pow(2,pos.level);

		//the position of the lod chunk in relation to the center in unity units
		Vector3 absPos = new Vector3(sideLength*(pos.x+0.5f),
		                             sideLength*(pos.y+0.5f),
		                             sideLength*(pos.z+0.5f));
		//Debug.Log(absPos.magnitude + " " + planet.noise.getAltitude(absPos)+" " +sideLength);
		//contains land if the dist to center of lod chunk is less than dist to land
		return absPos.magnitude < (planet.noise.getAltitude(absPos)+sideLength);


	}
	//public static splitList 
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

	public string ToString()
	{
		return "level " + level + " pos " + x + "," + y + "," + z;   
	}
}