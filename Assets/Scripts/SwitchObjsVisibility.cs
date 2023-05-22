using System.Collections.Generic;
using UnityEngine;

public class SwitchObjsVisibility : MonoBehaviour
{
    private bool is3DObjectActive = true;
    private List<GameObject> objects2D;
    private List<GameObject> objects3D;

    // Start is called before the first frame update
    void Start()
    {
        objects2D = GetAllMolObjectsWithTag("2dMol");
        objects3D = GetAllMolObjectsWithTag("3dMol");
    }

    public void Switch()
    {
        foreach (GameObject obj in objects2D)
        {
            obj.SetActive(!is3DObjectActive);
        }
        foreach (GameObject obj in objects3D)
        {
            obj.SetActive(is3DObjectActive);
        }
        is3DObjectActive = !is3DObjectActive;
    }

    private List<GameObject> GetAllMolObjectsWithTag(string tag)
    {
        List<GameObject> objects = new List<GameObject>();
        GameObject molManager = GameObject.FindGameObjectWithTag("MolManager");
        foreach (Transform child in molManager.transform)
        {
            foreach (Transform gChild in child.transform)
            {
                if (gChild.tag == tag)
                {
                    objects.Add(gChild.gameObject);
                }
            }
        }
        return objects;

    }

    //// Update is called once per frame
    //void Update()
    //{
    //    
    //}
}
