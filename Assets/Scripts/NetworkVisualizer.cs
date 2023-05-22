using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;


[System.Serializable]
public class DataNode
{
    public string id;  // smiles string
    public bool terminal;  // chemical
    public float ffScore;  // reaction
    public float templateScore;  // reaction
    public float numExamples;  // reaction
    public string type;  // chemical / reaction
}

[System.Serializable]
public class DataEdge
{
    public string id;
    public string from;  // smiles string
    public string to;  // smiles string
}

[System.Serializable]
public class DispNode
{
    public string id;
    public string smiles;
    public string type;  // chemical / reaction
}

[System.Serializable]
public class DispEdge
{
    public string id;
    public string from;
    public string to;
}

[System.Serializable]
public class DataGraph
{
    public List<DataNode> nodes;
    public List<DataEdge> edges;
}


[System.Serializable]
public class DispGraph
{
    public List<DispNode> nodes;
    public List<DispEdge> edges;
}

[System.Serializable]
public class Root
{
    public DataGraph dataGraph;
    public DispGraph dispGraph;
    public int version;
}

public class NetworkVisualizer : MonoBehaviour
{
    public EdgeCreator edgeCreator;
    public MoleculeCreator molCreator;
    public ReactionCreator reactionCreator;
    public float range = 2;

    public Dictionary<string, GameObject> nodeObjectLookup = new Dictionary<string, GameObject>();
    Dictionary<string, DataNode> nodeDataLookup = new Dictionary<string, DataNode>();
    Dictionary<string, DispNode> nodeDispDataLookup = new Dictionary<string, DispNode>();

    // Start is called before the first frame update
    void Start()
    {
        InitializeData();
    }

    public void InitializeData()
    {
        Root root = LoadJsonData("minimum_network");

        foreach (DataNode node in root.dataGraph.nodes)
        {
            nodeDataLookup.Add(node.id, node);
        }

        foreach (DispNode node in root.dispGraph.nodes)
        {
            nodeDispDataLookup.Add(node.id, node);
            if (node.type == "chemical")
            {
                GameObject molecule = molCreator.CreateMolecule(node.smiles, node.id);
                PositionNodes(molecule);
                nodeObjectLookup.Add(node.id, molecule);
            } else  // type == reaction
            {
                GameObject reaction = reactionCreator.CreateTransparentSphere(node.id);
                PositionNodes(reaction);
                nodeObjectLookup.Add(node.id, reaction);
            }
        }

        foreach (DispEdge edge in root.dispGraph.edges)
        {
            GameObject source = nodeObjectLookup[edge.from];
            GameObject target = nodeObjectLookup[edge.to];
            GameObject edgeObj = edgeCreator.CreateEdge(source, target);
        }
    }

    private Root LoadJsonData(string fileName)
    {
        //string jsonData = File.ReadAllText(filePath);
        string jsonData = Resources.Load<TextAsset>(fileName).text;
        Root root = JsonUtility.FromJson<Root>(jsonData);
        return root;
    }

    private void PositionNodes(GameObject nodeObj)
    {
        float x = Random.Range(-range, range);
        float y = Random.Range(-1, range);
        nodeObj.transform.position = new Vector3(x, y, 3);
    }

    private void OnApplicationQuit()
    {
        string dirPath = Path.Combine(Application.dataPath, "Resources/images/");
        FileUtil.DeleteFileOrDirectory(dirPath);
        Directory.CreateDirectory(dirPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
