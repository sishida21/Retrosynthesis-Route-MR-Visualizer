using System.Collections.Generic;
using UnityEngine;
using OpenBabel;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using System.IO;

public class MoleculeCreator: MonoBehaviour
{
    private float moleculeScale = 0.1f;
    public GameObject textPrefab;
    public GameObject nodeSphere;

    // Start is called before the first frame update
    void Start()
    {
        //CreateMolecule(smilesString);
    }

    public GameObject CreateMolecule(string smilesString, string nodeId)
    {
        GameObject moleculeObject = new GameObject(smilesString);
        moleculeObject.name = smilesString;
        moleculeObject.transform.SetParent(transform);

        OBConversion conv = new OBConversion();
        conv.SetInAndOutFormats("smi", "mol");
        OBMol mol = new OBMol();
        conv.ReadString(mol, smilesString);
        OBBuilder builder = new OBBuilder();
        builder.Build(mol);


        OBForceField forceField = OBForceField.FindForceField("mmff94"); // mmff94, UFF
        if (forceField != null )
        {
            forceField.Setup(mol);
            forceField.SteepestDescent(500);
            forceField.GetCoordinates(mol);
        }

        List<GameObject> atomObjects = new List<GameObject>();
        for (int i = 1; i <= mol.NumAtoms(); ++i)
        {
            OBAtom atom = mol.GetAtom(i);
            string element = GetElementSymbol(atom.GetAtomicNum());
            Vector3 position = new Vector3((float)atom.GetX(), (float)atom.GetY(), (float)atom.GetZ());
            GameObject atomObject = CreateAtomObject(element, position);
            atomObject.transform.SetParent(moleculeObject.transform);
            atomObjects.Add(atomObject);
        }
        // Create bond objects.
        for (int i = 0; i < mol.NumBonds(); ++i)
        {
            OBBond bond = mol.GetBond(i);
            uint start = bond.GetBeginAtomIdx();
            uint end = bond.GetEndAtomIdx();
            uint order = bond.GetBondOrder();
            List<GameObject> bondObjects = CreateBondObjects(atomObjects[(int)start - 1].transform.position, atomObjects[(int)end - 1].transform.position, order);

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
        GameObject sphere = createTransparentSphere(bounds);
        sphere.transform.SetParent(moleculeObject.transform);

        string pngPath = draw2DMolecule(smilesString, nodeId);
        Texture2D texture = PngToTex2D(pngPath);
        GameObject pngObj = new GameObject("Png_" + nodeId);
        pngObj.transform.SetParent(moleculeObject.transform);
        pngObj.transform.localScale = Vector3.one * 2.0f;
        pngObj.transform.position = bounds.center;
        pngObj.AddComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        SpriteRenderer renderer = pngObj.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        Rigidbody rb = moleculeObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        BoxCollider boxCollider = moleculeObject.AddComponent<BoxCollider>();
        boxCollider.center = moleculeObject.transform.InverseTransformPoint(bounds.center);
        boxCollider.size = moleculeObject.transform.InverseTransformVector(bounds.size);
        moleculeObject.AddComponent<ObjectManipulator>();
        moleculeObject.AddComponent<NearInteractionGrabbable>();
        DisplayInteraction interaction = moleculeObject.AddComponent<DisplayInteraction>();
        interaction.textPrefab = textPrefab;
        Debug.Log("Molecule creation complete");

        return moleculeObject;
    }

    private Texture2D PngToTex2D(string path)
    {
        BinaryReader binaryReader = new BinaryReader(new FileStream(path, FileMode.Open));
        byte[] rb = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
        binaryReader.Close();
        int pos = 16, width = 0, height = 0;
        for (int i = 0; i < 4; i++) width = width * 256 + rb[pos++];
        for (int i = 0; i < 4; i++) height = height * 256 + rb[pos++];
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(rb);
        return texture;
    }

    private string draw2DMolecule(string smilesString, string nodeId)
    {
        OBConversion conv = new OBConversion();
        conv.SetInAndOutFormats("smi", "_png2");
        OBMol mol = new OBMol();
        conv.ReadString(mol, smilesString);
        string filePath = Path.Combine(Application.dataPath, "Resources/images/", nodeId) + ".png";
        conv.AddOption("p", OBConversion.Option_type.OUTOPTIONS, "500");
        conv.AddOption("b", OBConversion.Option_type.OUTOPTIONS, "none");
        conv.AddOption("t", OBConversion.Option_type.OUTOPTIONS);
        conv.WriteFile(mol, filePath);
        conv.CloseOutFile();
        return filePath;
    }
    private GameObject createTransparentSphere(Bounds bounds)
    {
        GameObject sphere = Instantiate(nodeSphere);
        sphere.transform.position = bounds.center;
        float maxScale = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 1.3f;
        sphere.transform.localScale = Vector3.one * maxScale;
        //sphereMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
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

    private List<GameObject> CreateBondObjects(Vector3 startPosition, Vector3 endPosition, uint order)
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
    
    private static readonly string[] ElementSymbols = {
        "",
        "H", "He", "Li", "Be", "B", "C", "N", "O", "F", "Ne",
        "Na", "Mg", "Al", "Si", "P", "S", "Cl", "Ar", "K", "Ca",
        "Sc", "Ti", "V", "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn",
        "Ga", "Ge", "As", "Se", "Br", "Kr", "Rb", "Sr", "Y", "Zr",
        "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "In", "Sn",
        "Sb", "Te", "I", "Xe", "Cs", "Ba", "La", "Ce", "Pr", "Nd",
        "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb",
        "Lu", "Hf", "Ta", "W", "Re", "Os", "Ir", "Pt", "Au", "Hg",
        "Tl", "Pb", "Bi", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm",
        "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr", "Rf", "Db", "Sg",
        "Bh", "Hs", "Mt", "Ds", "Rg", "Cn", "Nh", "Fl", "Mc", "Lv",
        "Ts", "Og"
    };

    private string GetElementSymbol(uint atomicNumber)
    {
        if (atomicNumber >= 0 && atomicNumber < ElementSymbols.Length)
        {
            return ElementSymbols[atomicNumber];
        }
        return ""; // Unknown element
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
