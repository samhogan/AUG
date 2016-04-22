using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//handles the generation of terrain in multiple levels of detail
public class LODSystem
{

	private Planet planet;//a reference to its planet
	int chunkSize = TerrainObject.chunkSize;//voxels per chunk side

	//all chunks and their positions
	public Dictionary<LODPos, TerrainObject> chunks = new Dictionary<LODPos, TerrainObject>();

	//
	List<LODPos> splitChunks = new List<LODPos>();

	//the list of chunks that exist and are not split (are visable)
	List<LODPos> visChunks = new List<LODPos>();

	//list of chunks that have been split, but thier pieces still need to be rendered
	public List<LODPos> chunksToSplitRender = new List<LODPos>();

	//the level at which chunks appear in unispace rather than in normal space
	public static int uniCutoff = 5;

	public LODSystem(Planet p)
	{
		planet = p;
	}

	//updates level of detail by subdividing/combining chunks
	//given a level 0 lodpos (which is just a worldpos)
	public void updateLOD(WorldPos pos)
	{
		List<LODPos> chunksToSplit = new List<LODPos>();
		//for every chunk that is not split, check if it is close enough to be split
		foreach(LODPos lpos in visChunks)
		{
			//if the distance from the player's position to the lpos is close enough and its not a level 0 pos, add it to the split list
			if(lodposInRange(pos, lpos) && lpos.level > 0)
			{
				//Debug.Log(lpos);
				chunksToSplit.Add(lpos);
			}
		}


		List<LODPos> chunksToCombine = new List<LODPos>();

		//for every chunk that is split, check if it is far enough to be combined
		foreach(LODPos lpos in splitChunks)
		{
			//Debug.Log(lpos.ToString() + " " + chunks.ContainsKey(lpos));
			if(!lodposInRange(pos, lpos))
				chunksToCombine.Add(lpos);
		}
		//Debug.Log(splitChunks.Count + " " + chunksToCombine.Count);

		for(int i=chunksToSplit.Count-1; i>=0; i--)
		{
			splitChunk(chunksToSplit[i]);
			//Debug.Log("Apparently a chunk was split");
		}

		for(int i=chunksToCombine.Count-1; i>=0; i--)
		{
			
			//Debug.Log(lpos.ToString() + " " + chunks.ContainsKey(lpos));
			//Debug.Log("a chunk is being combined");
			combineChunk(chunksToCombine[i]);
		}


	}

	//is the lodpos close enough to be split up?
	bool lodposInRange(WorldPos pos, LODPos lpos)
	{
		//length of the side of lpos in units of 16 (or one side length of a level 0 lodpos)
		float sideLength = Mathf.Pow(2,lpos.level);

		//if the distance from the player's position to the lpos is less than its side length times some constant, return true
		return Vector3.Distance(new Vector3(pos.x+.5f, pos.y+.5f, pos.z+.5f), 
		                        new Vector3((lpos.x+.5f)*sideLength, (lpos.y+0.5f)*sideLength, (lpos.z+.5f)*sideLength)) < sideLength*1.2;
	}

	//renders the pieces of a chunk that has already been split
	public void splitRender(LODPos pos)
	{

		//if(!chunks.ContainsKey(pos))
		//	return;
		

		//Debug.Log("Chunks contains key :" + pos.ToString() + " " + chunks.ContainsKey(pos));
		//hide this terrain object but don't delete it
		//chunks[pos].gameObject.SetActive(false);

		//now find all of its pieces and render them

		//1 level lower
		int newLev = pos.level-1;
		//the position of the first subchunk in this chunk
		WorldPos newStart = new WorldPos(pos.x*2, pos.y*2, pos.z*2);
		
		//build all of the 8 chunks that contain land
		for(int x=0; x<=1; x++)
		{
			for(int y=0; y<=1; y++)
			{
				for(int z=0; z<=1; z++)
				{	
					LODPos newChunk = new LODPos(newLev, newStart.x+x, newStart.y+y, newStart.z+z);
					TerrainObject tobj = null;
					if(chunks.TryGetValue(newChunk, out tobj))
					{
						tobj.Render();	
					}
				}
			}
		}

		//splitChunks.
		//Debug.Log(pos.ToString() + " is being splitrendered " + chunksToSplitRender.IndexOf(pos) + " " + chunksToSplitRender.LastIndexOf(pos));
		chunks[pos].gameObject.SetActive(false);
		//Debug.Log(pos.ToString() + " was deactivated");		
	}

	//overloaded createchunk
	public void CreateChunk(LODPos pos)
	{
		CreateChunk(pos, false);
	}
	//creates and instantiates a terrain chunk (but does not render it yet)
	public void CreateChunk(LODPos pos, bool render) 
	{
		//if the chunk has aready been created, dont build it again!
		if(chunks.ContainsKey(pos))
			return;


		//build the terrainobject and add its gameobject to the chunks list(may remove this last thing later)
		//TerrainObject chunk = Build.buildObject<TerrainObject>(pos.toVector3(), Quaternion.identity);
		GameObject terrainGO = Pool.getTerrain();
		terrainGO.name = "Terrain Chunk " + pos.ToString();
		TerrainObject chunk = terrainGO.GetComponent<TerrainObject>();
		chunks.Add(pos, chunk);
		//Debug.Log("chunks added key :" + pos.ToString());
		visChunks.Add(pos);

		//this lod chunk scale
		//level 0 has scale 1, level 1 has scale 2, level 2 has scale 4, etc.
		float scale = (int)(Mathf.Pow(2,pos.level));//TerrainObject.wsRatio;
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
			terrainGO.transform.localScale = new Vector3(absScale, absScale, absScale)*TerrainObject.wsRatio;

			//the adjusted position of this terrain obj whin in unispace
			terrainGO.transform.localPosition = pos.toVector3()*TerrainObject.chunkWidth*absScale;
			terrainGO.transform.localRotation = Quaternion.identity;

		}
		else//it is in normal space
		{
			terrainGO.layer = 0;
			//terrainGO.transform.parent = null;
			terrainGO.transform.localScale = new Vector3(scale, scale, scale)*TerrainObject.wsRatio;
			terrainGO.transform.localPosition = Unitracker.getFloatingPos(pos.toVector3()*scale*TerrainObject.chunkWidth);

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
					voxPos.x = (pos.x*16+x*TerrainObject.wsRatio)*scale;
					voxPos.y = (pos.y*16+y*TerrainObject.wsRatio)*scale;
					voxPos.z = (pos.z*16+z*TerrainObject.wsRatio)*scale;

	


					Sub sub;//the substance of this voxel
					float voxVal;//the voxel value of this voxel for marching cubes

					//retrieve the voxel data from noise
					planet.noise.getVoxData(voxPos, out voxVal, out sub);

					//fills in the appropriate voxel data for marching cubes
					chunk.voxVals[x,y,z] = voxVal;//Noise.GetNoise((x+pos.x)/scale,(y+pos.y)/scale,(z+pos.z)/scale);
					//if(x+voxPos.x > 0)
					//	chunk.voxVals[x, y, z] = 2;
					//get the texture point of the substace at this vector
					chunk.voxType[x, y, z] = Substance.subs[sub].colorPoint;
					//puts a hole in the planet(just for fun
					//if(voxPos.x<10 && voxPos.x>-10 && voxPos.z<10 && voxPos.z>-10)
					//chunk.voxVals[x,y,z] = 2;
					
					
				}
				
			}
			
		}

		//here is a museum of the history of 
		//TerrainLoader.addToRender(chunk);
		//Loader.addToRender(chunk);
		//if(Time.time<3)
		if(render)
			chunk.Render();//renders the chunk immediately
		//RequestSystem.terrainToRender.Add(chunk);
		
		
	}

	//splits up a terrain chunk into 8 smaller chunks 
	public void splitChunk(LODPos pos)
	{
	//	TerrainObject to;
		//don't do anything if the chunk list does not contain the pos
	//	if(!chunks.TryGetValue(pos, out to))
	//		return;

		//hide this terrain object but don't delete it
		//to.gameObject.SetActive(false);
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
		chunks[pos].isSplit = true;
		splitChunks.Add(pos);
		visChunks.Remove(pos);
		//RequestSystem.terrainToSplitRender.Add(to);
		chunksToSplitRender.Add(pos);
		//Debug.Log(pos.ToString() + " was added to splitrenderlist");

	}

	//deletes all pieces of the given chunk and reactivates it or "combines" all its pieces into it
	public void combineChunk(LODPos pos)
	{
		//reactivate the chunk
		chunks[pos].gameObject.SetActive(true);
		//Debug.Log(pos.ToString() + " was activated");		
		
		
		//now find all of its pieces and destroy them
		
		//1 level lower
		int newLev = pos.level-1;
		//the position of the first subchunk in this chunk
		WorldPos newStart = new WorldPos(pos.x*2, pos.y*2, pos.z*2);

		for(int x=0; x<=1; x++)
		{
			for(int y=0; y<=1; y++)
			{
				for(int z=0; z<=1; z++)
				{	
					LODPos newChunk = new LODPos(newLev, newStart.x+x, newStart.y+y, newStart.z+z);
					TerrainObject tobj = null;
					if(chunks.TryGetValue(newChunk, out tobj))
					{
						//if the chunk to be deleted is split, combine it first
						if(splitChunks.Contains(newChunk))
						{
							combineChunk(newChunk);
						}

						//remove it from vischunks
						visChunks.Remove(newChunk);

						//remove from chunkstosplitrender if it is in it
						if(chunksToSplitRender.Contains(newChunk))
							chunksToSplitRender.Remove(newChunk);

						//finally remove from the chunks dictionary and utterly DESTROY IT (but it will later be pooled
						chunks.Remove(newChunk);
						//Object.Destroy(tobj);
						Pool.deleteTerrain(tobj);
						//Debug.Log("chunk " + newChunk.ToString() + " was deleted");
					}
				}
			}
		}

		chunks[pos].isSplit = false;
		if(chunksToSplitRender.Contains(pos))
			chunksToSplitRender.Remove(pos);
		//move this from splitchunks to vischunks
		splitChunks.Remove(pos);
		visChunks.Add(pos);
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

		for(int x = 0; x <= 1; x++)
			for(int y = 0; y <= 1; y++)
				for(int z = 0; z <= 1; z++)
				{
					if(pointContainsLand(sideLength, new Vector3(x,y,z), pos))
						return true;
				}


		return false;


	}

	//checks a single corner of a chunks to see if it is worthy of containing land
	bool pointContainsLand(float sideLength, Vector3 corner, LODPos pos)
	{
		//the position of the corner chunk in relation to the center in unity units
		Vector3 absPos = new Vector3(sideLength*(pos.x+corner.x),
									sideLength*(pos.y+corner.y),
									sideLength*(pos.z+corner.z));
		//altitude of land below or above this point
		float alt = planet.noise.getAltitude(absPos);

		//contains land if the dist to center of lod chunk is within a side length of the altitude
		//NOTE: may later change it to half a side legth because the corners are closer
		return absPos.magnitude < (alt+sideLength) && absPos.magnitude > (alt-sideLength);

		
	}

	//returns the side length of a given lod level
	int getSideLength(int lev)
	{
		return (int)(16*Mathf.Pow(2,lev));

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