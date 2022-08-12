using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer)), RequireComponent(typeof(Rigidbody))]
public class Rope : MonoBehaviour
{
    [Header("Rope settings")]
    [SerializeField] private float RopeLength = 2;
    [SerializeField, Range(1, 5), Tooltip("How many colliders are in each m/cm")] private int RopeQuality = 3;
    [SerializeField, Tooltip("Anchors the end of the rope")] private bool AnchorEnd = false;

    [Header("Rope colliders settings")]
    [SerializeField] private float CollidersRadius = 0.1f;
    [SerializeField] private float CollidersYOffset = 0.1f;

    [Header("Rope rigidbody settings")]
    [SerializeField, Tooltip("How may weights each node")] private float NodeWeight = 0.2f;
    [SerializeField] private CollisionDetectionMode CollisionQuality = CollisionDetectionMode.Continuous;

    [Header("Rope material")]
    [SerializeField] private Material RopeMaterial;
    [SerializeField] private LineTextureMode TextureMode = LineTextureMode.Tile;
    [SerializeField] private bool UseWorldSpace;
    private LineRenderer Lrend;

    // ROPE NODES SETTINGS //
    List<Transform> RopeNodes = new List<Transform>();
    Transform lastNode;
    bool RopeReady = false;

    void Start()
    {
        InitializeRope();
    }

    void InitializeRope()
    {
        RopeReady = false;

        GetComponent<Rigidbody>().isKinematic = true;
        Lrend = GetComponent<LineRenderer>();

        if (RopeNodes.Count == 0)
            lastNode = transform;

        for (int i = 0; i < RopeLength; i++)
        {
            for (int j = 0; j < RopeQuality; j++)
            {
                // Create GameObject //
                GameObject node = new GameObject();
                node.transform.parent = transform;
                node.name = $"Rope Node (m:{i}, q:{j})";

                Vector3 lastNodePosition = lastNode.transform.localPosition;
                Vector3 MyPosition = new Vector3(0, lastNodePosition.y - CollidersYOffset, 0);

                node.transform.localPosition = MyPosition;

                // Add components (Rigidbody, Hinge joint, ...) //
                SphereCollider collider = node.AddComponent<SphereCollider>();
                collider.radius = CollidersRadius;

                Rigidbody NodeRb = node.AddComponent<Rigidbody>();
                NodeRb.mass = NodeWeight;

                HingeJoint NodeJoint = node.AddComponent<HingeJoint>();
                NodeJoint.connectedBody = lastNode.GetComponent<Rigidbody>();

                // Set variable "lastNode" to "node" //
                lastNode = node.transform;
                RopeNodes.Add(node.transform);

                Debug.Log($"Created new node ({node.name})");
            }
        }

        if (AnchorEnd)
            RopeNodes[RopeNodes.Count - 1].GetComponent<Rigidbody>().isKinematic = true;

        RopeReady = true;
        SetupMaterial();
    }

    void SetupMaterial()
    {
        Lrend.material = RopeMaterial;
        Lrend.textureMode = TextureMode;

        if (UseWorldSpace)
            Lrend.useWorldSpace = true;
        else
            Lrend.useWorldSpace = false;
    }

    void Update()
    {
        if (RopeReady)
        {
            UpdateRopePosition();
        }
    }

    void UpdateRopePosition()
    {
        Lrend.positionCount = RopeNodes.Count;
        for (int i = 0; i < Lrend.positionCount; i++)
        {
            Lrend.SetPosition(i, RopeNodes[i].position);
        }
    }
}
