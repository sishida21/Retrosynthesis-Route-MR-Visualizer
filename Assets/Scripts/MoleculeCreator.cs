using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;


public class Atom
{
    public string Element { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
}

public class Bond
{
    public int BeginAtom { get; set; }
    public int EndAtom { get; set; }
    public int Type { get; set; }
}

public class Molecule
{
    public List<Atom> Atoms { get; set; } = new List<Atom>();
    public List<Bond> Bonds { get; set; } = new List<Bond>();
}

public class MoleculeLoader
{
    public static Molecule ReadMolecule(string molText)
    {
        Molecule molecule = new Molecule();
        string[] lines = molText.Split('\n');

        // Read atom count and bond count from line 4
        string line = lines[3];
        int atomCount = int.Parse(line.Substring(0, 3).Trim());
        int bondCount = int.Parse(line.Substring(3, 3).Trim());

        // Read atoms
        for (int i = 0; i < atomCount; i++)
        {
            line = lines[4 + i];
            Atom atom = new Atom();
            atom.X = double.Parse(line.Substring(0, 10).Trim());
            atom.Y = double.Parse(line.Substring(10, 10).Trim());
            atom.Z = double.Parse(line.Substring(20, 10).Trim());
            atom.Element = line.Substring(31, 3).Trim();
            molecule.Atoms.Add(atom);
        }

        // Read bonds
        for (int i = 0; i < bondCount; i++)
        {
            line = lines[4 + atomCount + i];
            Bond bond = new Bond();
            bond.BeginAtom = int.Parse(line.Substring(0, 3).Trim());
            bond.EndAtom = int.Parse(line.Substring(3, 3).Trim());
            bond.Type = int.Parse(line.Substring(6, 3).Trim());
            molecule.Bonds.Add(bond);
        }

        return molecule;
    }
}


public class MoleculeCreator: MonoBehaviour
{
    private float moleculeScale = 0.05f;
    public GameObject textPrefab;
    public GameObject nodeSphere;
    //public GameObject mainNetwork;

    public struct Mol3D
    {
        public GameObject objects;
        public Bounds bounds;
        public int atomNum;
    }

    public GameObject CreateMolecule(string nodeId)
    {
        GameObject moleculeObject = new GameObject(nodeId);
        moleculeObject.tag = "MolContainer";
        //moleculeObject.transform.SetParent(mainNetwork.transform);
        moleculeObject.transform.localScale = Vector3.one * 0.4f;

        Mol3D mol3d = build3DMolecule(nodeId);
        mol3d.objects.transform.SetParent(moleculeObject.transform);
        GameObject sphere = createTransparentSphere(mol3d.bounds);
        sphere.transform.SetParent(moleculeObject.transform);

        GameObject mol2d = draw2DMolecule(nodeId, mol3d.atomNum);
        mol2d.transform.SetParent(moleculeObject.transform);

        Rigidbody rb = moleculeObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        BoxCollider boxCollider = moleculeObject.AddComponent<BoxCollider>();
        boxCollider.size = moleculeObject.transform.InverseTransformVector(mol3d.bounds.size);
        moleculeObject.AddComponent<ObjectManipulator>();
        moleculeObject.AddComponent<NearInteractionGrabbable>();
        moleculeObject.AddComponent<NodeForce>();
        Debug.Log("Molecule creation complete");

        return moleculeObject;
    }

    private Mol3D build3DMolecule(string nodeId)
    {
        GameObject moleculeObject = new GameObject("3D_molecule");
        moleculeObject.tag = "3dMol";

        TextAsset molFile = Resources.Load<TextAsset>("molfiles/" + nodeId);
        Molecule mol = MoleculeLoader.ReadMolecule(molFile.text);

        List<GameObject> atomObjects = new List<GameObject>();
        for (int i = 0; i < mol.Atoms.Count; ++i)
        {
            Atom atom = mol.Atoms[i];
            string element = atom.Element;
            Vector3 position = new Vector3((float)atom.X, (float)atom.Y, (float)atom.Z);
            GameObject atomObject = CreateAtomObject(element, position);
            atomObject.transform.SetParent(moleculeObject.transform);
            atomObjects.Add(atomObject);
        }

        List<GameObject> bondObjects;
        for (int i = 0; i < mol.Bonds.Count; ++i)
        {
            Bond bond = mol.Bonds[i];
            int start = bond.BeginAtom;
            int end = bond.EndAtom;
            int order = bond.Type;
            bondObjects = CreateBondObjects(atomObjects[start - 1].transform.position, atomObjects[end - 1].transform.position, order);
            foreach (GameObject bondObject in bondObjects)
            {
                bondObject.transform.SetParent(moleculeObject.transform);
            }
        }

        moleculeObject.transform.localScale = Vector3.one * moleculeScale;

        Bounds bounds = new Bounds(atomObjects[0].transform.position, Vector3.zero);
        foreach (GameObject atom in atomObjects)
        {
            SphereCollider collider = atom.GetComponent<SphereCollider>();
            if (collider != null)
            {
                Bounds atomBounds = new Bounds(atom.transform.position, Vector3.one * collider.radius * 2 * atom.transform.lossyScale.x);
                bounds.Encapsulate(atomBounds);
            }
        }
        moleculeObject.transform.position = Vector3.zero - bounds.center;
        Mol3D mol3d;
        mol3d.objects = moleculeObject;
        mol3d.bounds = bounds;
        mol3d.atomNum = mol.Atoms.Count;
        return mol3d;
    }

    private GameObject draw2DMolecule(string nodeId, int atomNum)
    {
        Texture2D texture = Resources.Load<Texture2D>("images/" + nodeId);
        GameObject pngObj = new GameObject("2D_molecule");
        pngObj.tag = "2dMol";
        pngObj.SetActive(false);
        if (atomNum > 15)
        {
            pngObj.transform.localScale = Vector3.one * moleculeScale * 2.5f;
        }
        else if (atomNum > 7 || atomNum < 15)
        {
            pngObj.transform.localScale = Vector3.one * moleculeScale * 1.2f;
        }
        else
        {
            pngObj.transform.localScale = Vector3.one * moleculeScale * 0.8f;
        }
        pngObj.transform.position = Vector3.zero;
        pngObj.AddComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        SpriteRenderer renderer = pngObj.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        return pngObj;
    }

    private GameObject createTransparentSphere(Bounds bounds)
    {
        GameObject sphere = Instantiate(nodeSphere);
        //sphere.transform.position = bounds.center;
        sphere.transform.position = Vector3.zero;
        float maxScale = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 1.25f;
        Debug.LogFormat("{0} {1}", maxScale, bounds.ToString());
        sphere.transform.localScale = Vector3.one * maxScale;
        return sphere;
    }

    private Color GetElementColor(string element)
    {
        switch (element)
        {
            case "H":
                return Color.white;
            case "C":
                return Color.gray;
            case "N":
                return Color.blue;
            case "O":
                return Color.red;
            case "F":
            case "Cl":
                return Color.green;
            case "Br":
                return new Color(0.6f, 0.3f, 0.1f); // Dark red-brown
            case "I":
                return new Color(0.4f, 0.0f, 0.7f); // Dark violet
            case "S":
                return Color.yellow;
            case "P":
                return new Color(1.0f, 0.5f, 0.0f); // Orange
            default:
                return new Color(0.7f, 0.7f, 0.7f); // Unknown elements
        }
    }
    private GameObject CreateAtomObject(string element, Vector3 position)
    {
        GameObject atomObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        atomObject.transform.position = position;
    
        Color elementColor = GetElementColor(element);
        atomObject.GetComponent<Renderer>().material.color = elementColor;
    
        return atomObject;
    }

    private List<GameObject> CreateBondObjects(Vector3 startPosition, Vector3 endPosition, int order)
    {
        List<GameObject> bondObjects = new List<GameObject>();
        Vector3 bondDirection = endPosition - startPosition;
        float bondLength = bondDirection.magnitude;
    
        for (int i = 0; i < order; i++)
        {
            Vector3 offset = Vector3.zero;
            float bondOffset = 0.2f;
    
            if (order > 1)
            {
                if (i % 2 == 0)
                {
                    offset = Quaternion.Euler(0, 0, 90) * bondDirection.normalized * bondOffset * (i + 1) / 2;
                }
                else
                {
                    offset = Quaternion.Euler(0, 0, -90) * bondDirection.normalized * bondOffset * (i + 1) / 2;
                }
            }
    
            Vector3 bondCenter = (startPosition + endPosition) / 2 + offset;
            GameObject bondObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bondObject.transform.position = bondCenter;
            bondObject.transform.localScale = new Vector3(0.1f, bondLength / 2, 0.1f);
            bondObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, bondDirection);
    
            bondObjects.Add(bondObject);
        }
    
        return bondObjects;
    }
}
