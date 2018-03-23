using UnityEngine;
using UnityEngine.EventSystems;

public class FractGen : MonoBehaviour
{
    GameObject sphere;

    public Material[] refMats = new Material[8];

    public PrimitiveType[] pvt = new PrimitiveType[8];

    public float mouserotationSpeed = 128.0f;

    [Range(0, 512)]
    public float anirotationSpeed = 4.0f;

    public float dice_to_stop_recurse = 0f;
    private float old_dice_to_stop_recurse = 0f;

    [Range(0, 16)]
    public int maxRotationDepth = 3;

    [Range(1, 7)]
    public int IterationDepth = 4;
    private int oldIterationDepth = 4;


    [Range(0, 0.5f)]
    public float scaleRandomizer = 0;
    private float oldscaleRandomizer = 0;

    public bool[] chk_it = new bool[6];// { true, true, true, true, true, true };
    private bool[] old_chk_it = new bool[6];// { true, true, true, true, true, true };

    [Range(0, 42)]
    public int scale_seed = 12345;
    private int old_scale_seed = 12345;

    private bool drag;

    [Range(0.001f, 0.02f)]
    public float min_scale_to_create = 0.01f;
    private float old_min_scale_to_create = 0.01f;

    // Sinon l'export foire en disant qu'il manque des symboles en WebGL
    private MeshFilter m;
    private SphereCollider o;
    private MeshRenderer l;

    bool GUIenabled = true;
    bool LightParented = true;
    bool old_LightParented = true;

    void OnGUI()
    {
        float dy = 25;
        float y = 1;
        float x2 = 175;

        float XOFFS = 50;
        float YOFFS = 50;


        if (GUIenabled == true)
        {
            GUI.Label(new Rect(XOFFS, YOFFS + dy * (y - 1), 180, 30), "F1 to hide/show GUI");

            GUI.Label(new Rect(XOFFS, YOFFS + dy * y, 180, 30), "ScaleRandomizer");
            scaleRandomizer = GUI.HorizontalSlider(new Rect(XOFFS + x2, YOFFS + dy * y+5, 100, 15), scaleRandomizer, 0.0f, 0.5f);
            GUI.Label(new Rect(XOFFS + x2 + 125, YOFFS + dy * y, 60, 30), "" + scaleRandomizer);
            y++;

            GUI.Label(new Rect(XOFFS, YOFFS + dy * y, 180, 30), "IterationDepth");
            IterationDepth = (int)GUI.HorizontalSlider(new Rect(XOFFS + x2, YOFFS + dy * y + 5, 100, 15), (float)IterationDepth, 0.0f, 7.0f);
            GUI.Label(new Rect(XOFFS + x2 + 125, YOFFS + dy * y, 60, 30), "" + IterationDepth);
            y++;

            GUI.Label(new Rect(XOFFS, YOFFS + dy * y, 180, 30), "MaxrotDepth");
            maxRotationDepth = (int)GUI.HorizontalSlider(new Rect(XOFFS + x2, YOFFS + dy * y + 5, 100, 15), (float)maxRotationDepth, 0.0f, 7.0f);
            GUI.Label(new Rect(XOFFS + x2 + 125, YOFFS + dy * y, 60, 30), "" + maxRotationDepth);
            y++;

            GUI.Label(new Rect(XOFFS, YOFFS + dy * y, 180, 30), "anirotationSpeed");
            anirotationSpeed = GUI.HorizontalSlider(new Rect(XOFFS + x2, YOFFS + dy * y + 5, 100, 15), (float)anirotationSpeed, 0.0f, 7.0f);
            GUI.Label(new Rect(XOFFS + x2 + 125, YOFFS + dy * y, 60, 30), "" + anirotationSpeed);
            y++;

            GUI.Label(new Rect(XOFFS, YOFFS + dy * y, 180, 30), "ParentedLight");
            LightParented = GUI.Toggle(new Rect(XOFFS + x2, YOFFS + dy * y + 0, 100, 15), LightParented, "");
            GUI.Label(new Rect(XOFFS + x2 + 125, YOFFS + dy * y, 60, 30), "" + LightParented);
            y++;



        }
        /*
        GUI.Label(new Rect(25, dy * y, 100, 30), "Intensity");
        intensity = GUI.HorizontalSlider(new Rect(x2, dy * y++, 100, 30), intensity, 0.0f, 1.0f);

        GUI.Label(new Rect(25, dy * y, 100, 30), "Alpha");
        alpha = GUI.HorizontalSlider(new Rect(x2, dy * y++, 100, 30), alpha, 0.0f, 2.5f);

        GUI.Label(new Rect(25, dy * y, 100, 30), "AlphaSub");
        alphasub = GUI.HorizontalSlider(new Rect(x2, dy * y++, 100, 30), alphasub, 0.0f, 1.0f);

        GUI.Label(new Rect(25, dy * y, 100, 30), "Pow");
        pow = GUI.HorizontalSlider(new Rect(x2, dy * y++, 100, 30), pow, 0.0f, 4.0f);

        GUI.Label(new Rect(25, dy * y, 100, 30), "Red");
        color.r = GUI.HorizontalSlider(new Rect(x2, dy * y++, 100, 30), color.r, 0.0f, 1.0f);

        GUI.Label(new Rect(25, dy * y, 100, 30), "Green");
        color.g = GUI.HorizontalSlider(new Rect(x2, dy * y++, 100, 30), color.g, 0.0f, 1.0f);

        GUI.Label(new Rect(25, dy * y, 100, 30), "Blue");
        color.b = GUI.HorizontalSlider(new Rect(x2, dy * y++, 100, 30), color.b, 0.0f, 1.0f);
        */
    }

    // Use this for initialization
    void Start()
    {
        
        Camera.main.orthographicSize = 2.0f;
        NewMethod();
    }

    private int idx = 0;

    private void NewMethod()
    {
        idx = 1;
        float scale = 1.0f;

        Light pl1 = GameObject.Find("PLight1").GetComponent<Light>();
        Light pl2 = GameObject.Find("PLight2").GetComponent<Light>();
        Light pl3 = GameObject.Find("PLight3").GetComponent<Light>();

        Debug.Log(""+  pl3.transform.parent);

        // Detach les lampes attachées.
        pl1.transform.SetParent(null);
        pl2.transform.SetParent(null);
        pl3.transform.SetParent(null);
        pl1.transform.position = new Vector3(0.9f, -0.68f, -1.81f);
        pl2.transform.position = new Vector3(0.947f, 0.813f, -1.87f);
        pl3.transform.position = new Vector3(-1.292f, -0.847f, -1.81f);

        if (sphere != null)
        {
            GameObject.Destroy(sphere);
        }

        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0, 0, 0);
        sphere.transform.localScale = new Vector3(scale, scale, scale);

        if (refMats[0] != null)
            sphere.GetComponent<Renderer>().material = refMats[0];

        Random.InitState(scale_seed);

        Vector3 pos = sphere.transform.position;
        dotruc(sphere, scale / 2, 1, -1);

        Debug.Log("Amount : " + idx);

        // Accroche les lampes pour qu'elle tournes avec la fractale.
        if (LightParented)
        { 
            pl1.transform.SetParent(sphere.transform);
            pl2.transform.SetParent(sphere.transform);
            pl3.transform.SetParent(sphere.transform);
        }

    }

    private void dotruc(GameObject sphere, float scale, int lvl, int cas)
    {
        if (lvl > IterationDepth) return;

        if (scale < min_scale_to_create)
            return;

        if (dice_to_stop_recurse != 0.0f)
            if (Random.Range(0.0f, 1.0f)< dice_to_stop_recurse)
                return;

        Material refMat = refMats[lvl % refMats.Length];

        Vector3[] translate =
        {
            -Vector3.left,
            Vector3.left,
            -Vector3.up,
            Vector3.up,
            -Vector3.forward,
            Vector3.forward
        };

        string[] names =
        {
            "X",
            "X",
            "Y",
            "Y",
            "Z",
            "Z",
        };

        // Pour savoir qui doit etre appeler pour la boucle
        int[] cases =
        {
           1,
           0,
           3,
           2,
           5,
           4,
        };


        float rnd = -Random.Range(0.0f, scale * 2) * scaleRandomizer;

        for (int i = 0; i < cases.Length; i++)
        {
            if ((cas != i) && (chk_it[i]))
            {
                rnd = -Random.Range(0.0f, scale * 2) * scaleRandomizer;
                GameObject sphere1 = GameObject.CreatePrimitive(pvt[lvl % pvt.Length]);
                idx++;
                sphere1.name = names[i];
                if (refMat != null) sphere1.GetComponent<Renderer>().material = refMat;
                sphere1.transform.position = sphere.transform.position;
                sphere1.transform.localScale = new Vector3(scale + rnd, scale + rnd, scale + rnd);
                sphere1.transform.Translate(translate[i] * (scale + (scale + rnd) / 2));
                dotruc(sphere1, (scale + rnd) / 2, lvl + 1, cases[i]);
                sphere1.transform.SetParent(sphere.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update la structure si on change l'iteration depth ou un autre truc qui change la structure.
        if (oldIterationDepth != IterationDepth)
        {
            NewMethod();
            oldIterationDepth = IterationDepth;
        }
        if (oldscaleRandomizer != scaleRandomizer)
        {
            NewMethod();
            oldscaleRandomizer = scaleRandomizer;
        }
        if (old_scale_seed != scale_seed)
        {
            NewMethod();
            old_scale_seed = scale_seed;
        }
        if (old_min_scale_to_create != min_scale_to_create)
        {
            NewMethod();
            old_min_scale_to_create = min_scale_to_create;
        }
        if (old_dice_to_stop_recurse != dice_to_stop_recurse)
        {
            NewMethod();
            old_dice_to_stop_recurse = dice_to_stop_recurse;
        }
        if (old_LightParented != LightParented)
        {
            NewMethod();
            old_LightParented = LightParented;
        }
        
        for (int i = 0; i < chk_it.Length; i++)
        {
            if (old_chk_it[i] != chk_it[i])
            {
                NewMethod();
                old_chk_it[i] = chk_it[i];
            }
        }





        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            sphere.transform.localPosition.Set(sphere.transform.localPosition.x, sphere.transform.localPosition.y, sphere.transform.localPosition.z + 0.1f);
         }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            sphere.transform.position.Set(sphere.transform.localPosition.x, sphere.transform.localPosition.y, sphere.transform.localPosition.z - 0.1f);
        }
       // Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 0.1f, 2.5f);

        if (Input.GetMouseButtonDown(0))
        {
            drag = true;
            
            // Debug.Log("Event : " + EventSystem.current);
            //Debug.Log("CLK   : " + EventSystem.current.IsPointerOverGameObject());
            //if (EventSystem.current != null)

            if (EventSystem.current.IsPointerOverGameObject())
                drag = false;

        }
        if (Input.GetMouseButtonUp(0))
            drag = false;

        if (drag == true)
            Rotate();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (GUIenabled == true)
                GUIenabled = false;
            else
                GUIenabled = true;
            Debug.Log("Gui : " + GUI.enabled);
        }

        // Rotation des elements de la sphere...
        float rotX = Time.deltaTime / 55.0f * anirotationSpeed;
        recurserotator(sphere.transform, rotX, 0);
    }

    void Rotate()
    {
        float rotX = Input.GetAxis("Mouse X") * mouserotationSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * mouserotationSpeed * Mathf.Deg2Rad;
        //Debug.Log("Rot : " + rotX + ", " + rotY);
        sphere.transform.Rotate(Vector3.up, -rotX, Space.World);
        sphere.transform.Rotate(Vector3.right, rotY, Space.World);
    }
    

    /**
     * Provoque une rotation de tout les elements de la factale en fct des axes de bases...
     */
    void recurserotator(Transform child, float rotX, int lvl)
    {
        if (lvl < maxRotationDepth)
            foreach (Transform child3 in child.transform)
            {
                if (child3.name.Equals("X"))
                    child3.transform.Rotate(new Vector3(1, 0, 0), -rotX * Mathf.Rad2Deg, Space.Self);
                if (child3.name.Equals("Y"))
                    child3.transform.Rotate(new Vector3(0, 1, 0), -rotX * Mathf.Rad2Deg, Space.Self);
                if (child3.name.Equals("Z"))
                    child3.transform.Rotate(new Vector3(0, 0, 1), -rotX * Mathf.Rad2Deg, Space.Self);

                recurserotator(child3.transform, rotX, lvl + 1);
            }
    }
}
