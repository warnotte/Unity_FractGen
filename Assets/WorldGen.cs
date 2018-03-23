using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour {

    public GameObject instanciable;

    int w = 50;
    int h = 50;

    GameObject[,] grid = null;

    // Use this for initialization
    void Start()
    {
        grid = new GameObject[w * 2, h * 2];
        
        for (int x = -w; x < w; x++)
        {
            for (int y = -h; y < h; y++)
            {
                grid[x + w, y + h] = Instantiate(instanciable, new Vector3(x * 1.0F, 0, y * 1.0F), Quaternion.Euler(new Vector3(-90, 0, 90)));
                
            }
    }
    }

    // Update is called once per frame
    void Update()
    {
        for (int x = 0; x < w * 2; x++)
        {
            for (int y = 0; y < h * 2; y++)
            {
                Transform trans = grid[x, y].transform;
                //trans.position.Set(trans.position.x, 10 * Mathf.Sin(x + y * Time.time), trans.position.y);

               // float height = height_for_pos(new Vector2(x, y));

                trans.position = new Vector3(trans.position.x, 1 * (Mathf.Sin(x/5f + Time.time/1f)+ Mathf.Cos( y/5f + Time.time / 1f)), trans.position.z);
               // trans.position = new Vector3(trans.position.x, height, trans.position.z);
            }
        }
    }

    float height_for_pos(Vector2 p)
    {
        p *= .1f;
  //      float d = p.x + p.y;
  //      p.x += d;
  //      p.y += d;

        //shift origin a bit randomly
        //p+=vec2(2.*sin(iGlobalTime*.3+.2),2.*cos(iGlobalTime*0.1+0.5));
        //cosine of distance from origin, modulated by Gaussian
        float q = Vector2.Dot(p, p);
        float x = Mathf.Sqrt(q);
        return 6f* Mathf.Cos(x * 2f + Time.time) * Mathf.Exp(-q / 128f);
    }
}
