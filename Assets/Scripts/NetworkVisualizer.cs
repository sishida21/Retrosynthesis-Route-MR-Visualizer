using System.Collections.Generic;
using UnityEngine;

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

public class NetworkNode
{
    public List<NetworkNode> children = new List<NetworkNode>();
    public GameObject molObject;
    public Vector3 previousPosition;
    public List<DispEdge> connectedEdges = new List<DispEdge>();

    public NetworkNode(GameObject molObject)
    {
        this.molObject = molObject;
    }
    public void AddChild(NetworkNode child)
    {
        children.Add(child);
    }
    public void SetPosition(Vector3 newPosition)
    {
        this.molObject.transform.position = newPosition;
        this.previousPosition = newPosition;
    }
}

public class NetworkVisualizer : MonoBehaviour
{
    public EdgeCreator edgeCreator;
    public MoleculeCreator molCreator;
    public ReactionCreator reactionCreator;
    private Root root;
    public NetworkNode rootNode;
    public string jsonFileName = "medium_network";
    public GameObject reactionNetwork;
    private float startTime;

    public Dictionary<string, GameObject> nodeObjectLookup = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> edgeObjectLookup = new Dictionary<string, GameObject>();
    Dictionary<string, DataNode> nodeDataLookup = new Dictionary<string, DataNode>();
    Dictionary<string, DispNode> nodeDispDataLookup = new Dictionary<string, DispNode>();
    Dictionary<string, NetworkNode> networkNodeLookup = new Dictionary<string, NetworkNode>();

    // Start is called before the first frame update
    void Start()
    {
        InitializeData();
        ReactionNetwork network = new ReactionNetwork(rootNode);
        network.PlaceNodes();
        startTime = Time.time;
    }

    public void InitializeData()
    {
        //GameObject reactionNetwork = new GameObject("ReactionNetwork");
        root = LoadJsonData(jsonFileName);

        foreach (DataNode node in root.dataGraph.nodes)
        {
            nodeDataLookup.Add(node.id, node);
        }

        foreach (DispNode node in root.dispGraph.nodes)
        {
            nodeDispDataLookup.Add(node.id, node);
            if (node.type == "chemical")
            {
                GameObject molecule = molCreator.CreateMolecule(node.id);
                molecule.transform.SetParent(reactionNetwork.transform);
                nodeObjectLookup.Add(node.id, molecule);
            } else  // type == reaction
            {
                GameObject reaction = reactionCreator.CreateTransparentSphere(nodeDataLookup[node.smiles]);
                reaction.transform.SetParent(reactionNetwork.transform);
                nodeObjectLookup.Add(node.id, reaction);
            }
        }

        foreach (DispEdge edge in root.dispGraph.edges)
        {
            GameObject source = nodeObjectLookup[edge.from];
            GameObject target = nodeObjectLookup[edge.to];
            NetworkNode sNode;
            NetworkNode tNode;
            if (networkNodeLookup.ContainsKey(edge.from))
            {
                sNode = networkNodeLookup[edge.from];
            } else
            {
                sNode = new NetworkNode(source);
                networkNodeLookup.Add(edge.from, sNode);
            }
            if (networkNodeLookup.ContainsKey(edge.to))
            {
                tNode = networkNodeLookup[edge.to];
            } else
            {
                tNode = new NetworkNode(target);
                networkNodeLookup[edge.to] = tNode;
            }
            if (edge.from == "00000000-0000-0000-0000-000000000000")
            {
                rootNode = sNode;
            }
            sNode.AddChild(tNode);
        }

        foreach ( DispEdge edge in root.dispGraph.edges )
        {
            GameObject source = nodeObjectLookup[edge.from];
            GameObject target = nodeObjectLookup[edge.to];
            NetworkNode sNode = networkNodeLookup[edge.from];
            NetworkNode tNode = networkNodeLookup[edge.to];
            GameObject edgeObj = edgeCreator.CreateEdge(source, target);
            edgeObj.transform.SetParent(reactionNetwork.transform);
            edgeObjectLookup.Add(edge.id, edgeObj);
            sNode.connectedEdges.Add(edge);
            tNode.connectedEdges.Add(edge);
        }
        reactionNetwork.transform.localScale = Vector3.one * 0.4f;
    }

    public class ReactionNetwork
    {
        public NetworkNode rootNode;
        public GameObject reactionNetwork = GameObject.FindGameObjectWithTag("MainNetwork");
        public float verticalSpacing = 0.25f;
        public float initialRadius = 1.0f;
        public float radiusDecay = 0.85f;

        public ReactionNetwork(NetworkNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public void PlaceNodes()
        {
            if (rootNode == null)
            {
                Debug.LogError("Root node is null");
                return;
            }
            rootNode.molObject.transform.position = reactionNetwork.transform.position;
            PlaceNode(rootNode, initialRadius);
        }

        private void PlaceNode(NetworkNode node, float currentRadius)
        {
            float angleStep = 2 * Mathf.PI / node.children.Count;
            for (int i = 0; i < node.children.Count; i++)
            {
                NetworkNode child = node.children[i];
                Vector3 newPosition;
                if (node.children.Count == 1)
                {
                    newPosition = new Vector3(0, -verticalSpacing, 0);
                }
                else
                {
                    float angle = angleStep * i;
                    newPosition = new Vector3(
                        currentRadius * Mathf.Cos(angle),
                        -verticalSpacing,
                        currentRadius * Mathf.Sin(angle));
                }
                child.SetPosition(node.molObject.transform.position + newPosition);
            }

            float nextRadius = currentRadius * radiusDecay;
            for (int i = 0; i < node.children.Count;i++)
            {
                PlaceNode(node.children[i], nextRadius);
            }
        }
    }

    private Root LoadJsonData(string fileName)
    {
        string jsonData = Resources.Load<TextAsset>(fileName).text;
        Root root = JsonUtility.FromJson<Root>(jsonData);
        return root;
    }

    public GameObject source;
    public GameObject target;
    public NetworkNode sNode;
    public NetworkNode tNode;
    private void UpdateEdgePosition()
    {
        foreach (DispEdge edge in root.dispGraph.edges)
        {
            source = nodeObjectLookup[edge.from];
            target = nodeObjectLookup[edge.to];
            sNode = networkNodeLookup[edge.from];
            tNode = networkNodeLookup[edge.to];
            if (Time.time - startTime < 2.0f)
            {
                sNode.previousPosition = source.transform.position;
                tNode.previousPosition = target.transform.position;
                edgeCreator.UpdateEdgePosition(edgeObjectLookup[edge.id], source, target);
                continue;
            }
            if (sNode.previousPosition != source.transform.position)
            {
                UpdateConnectedEdgesPositions(sNode);
            }
            if (tNode.previousPosition != target.transform.position)
            {
                UpdateConnectedEdgesPositions(tNode);
            }
        }
    }

    private void UpdateConnectedEdgesPositions(NetworkNode node)
    {
        GameObject _source;
        GameObject _target;
        NetworkNode _sNode;
        NetworkNode _tNode;
        foreach (DispEdge _edge in node.connectedEdges)
        {
            _source = nodeObjectLookup[_edge.from];
            _target = nodeObjectLookup[_edge.to];
            _sNode = networkNodeLookup[_edge.from];
            _tNode = networkNodeLookup[_edge.to];
            edgeCreator.UpdateEdgePosition(edgeObjectLookup[_edge.id], _source, _target);
            _sNode.previousPosition = _source.transform.position;
            _tNode.previousPosition = _target.transform.position;
        }
        //Debug.Log("Update Edge");
    }

    void Update()
    {
        UpdateEdgePosition();
    }
}
