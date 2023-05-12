using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenBabel;
using Unity.VisualScripting;

public class MoleculeCreator : MonoBehaviour
{
    public string smilesString = "CC(=O)Oc1ccccc1C(=O)O";
    public float moleculeScale = 0.1f;
    public float distanceFromCamera = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        OBConversion conv = new OBConversion();
        conv.SetInFormat("smi");
        OBMol mol = new OBMol();
        conv.ReadString(mol, smilesString);

        conv.SetOutFormat("pdb");
        OBBuilder builder = new OBBuilder();
        builder.Build(mol);

        OBForceField forceField = OBForceField.FindForceField("mmff94");
        if (forceField != null )
        {
            forceField.Setup(mol);
            forceField.SteepestDescent(500);
            forceField.GetCoordinates(mol);
        }

        CreateMoleculeObjects(mol);
    }

    private void CreateMoleculeObjects(OBMol mol)
    {
        GameObject moleculeObject = new GameObject("Molecule");
        moleculeObject.transform.SetParent(transform);

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

        moleculeObject.transform.localScale = new Vector3(moleculeScale, moleculeScale, moleculeScale);
        moleculeObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCamera;
        Debug.Log("Molecule creation complete");

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
            float bondOffset = 0.2f; // オフセット量を調整できます
    
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
    
            // 結合次数に応じた色を設定することもできます。
            // bondObject.GetComponent<Renderer>().material.color = Color.grey;
    
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
