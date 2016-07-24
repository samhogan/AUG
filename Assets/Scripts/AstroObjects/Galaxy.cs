using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Galaxy : AstroObject
{

    //a list of the currently built star systems in this galaxy
    public List<StarSystem> systems = new List<StarSystem>();


    public Galaxy()
    {

    }

    public void generateStuff()
    {
        System.Random rand = new System.Random(Random.Range(int.MinValue, int.MaxValue));

        for(int i = 0; i<50; i++)
        {
            LongPos pos;
            if(i == 0)
                pos = new LongPos(0, 0, 0);
            else
                pos = new LongPos(rand.Next(-30000, 30000) * 10000L, rand.Next(-20000, 20000) * 10000L, rand.Next(-30000, 30000) * 10000L);
            StarSystem sys = new StarSystem(pos);
            systems.Add(sys);
            
        }

    }
}
