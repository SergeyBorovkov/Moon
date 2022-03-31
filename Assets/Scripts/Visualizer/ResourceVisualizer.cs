using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceCluster))]
public abstract class ResourceVisualizer : MonoBehaviour
{
    private List<Resource> _boxes;    

    private Vector3 _boxInitialRotation = new Vector3(0, 90);
    private Vector3 _rotationAxis = new Vector3(1, 0, 0);
    private float _movingSpeed = 14f;
    private float _rotationSpeed = 300f;
    private float _yPosGap = 0.45f;

    protected ResourceCluster Cluster;
    [SerializeField] protected Transform VisualizationPoint;
    protected Coroutine MoveJob;
    protected Resource VisualizationBox;
    protected float PointShift = 1.5f;

    public Vector3 CurrentBoxPosition => GetInClusterBoxPosition();

    private void Awake()
    {
        Cluster = GetComponent<ResourceCluster>();
    }

    protected virtual void OnEnable()
    {
        Cluster.CurrentResourceChanged += OnCurrentResourceChanged;
    }

    protected virtual void OnDisable()
    {
        Cluster.CurrentResourceChanged -= OnCurrentResourceChanged;
    }

    protected abstract void OnCurrentResourceChanged();    

    protected void Start()
    {
        _boxes = new List<Resource>();

        for (int i = 0; i < Cluster.MaxResources; i++)
            _boxes.Add(InstantiateBox(transform.position, VisualizationPoint, i));

        ActivateBoxes(Cluster.CurrentResources);

        VisualizationBox = InstantiateBox(GetStartBoxPosition(), VisualizationPoint);
    }

    protected virtual void StartVisualization()
    {
        VisualizationBox.transform.position = GetStartBoxPosition();
        MoveJob = StartCoroutine(TwoPointMove(VisualizationBox));
    }

    protected abstract Vector3 GetStartBoxPosition();

    protected abstract Vector3 GetTargetBoxPosition();    

    protected Vector3 GetInClusterBoxPosition()
    {
        Vector3 position;

        if (Cluster.CurrentResources == 0)
            position = _boxes[0].transform.position;
        else
            position = _boxes[Cluster.CurrentResources - 1].transform.position;

        return position;
    }

    protected abstract Vector3 GetFirstTravelPosition();    

    protected virtual IEnumerator TwoPointMove(Resource box)
    {
        while (box.transform.position != GetFirstTravelPosition())
        {
            Move(box, GetFirstTravelPosition());
            Rotate(box);
            yield return null;
        }

        while (box.transform.position != GetTargetBoxPosition())
        {
            Move(box, GetTargetBoxPosition());
            Rotate(box);
            yield return null;
        }

        VisualizationBox.gameObject.SetActive(false);
        ActivateBoxes(Cluster.CurrentResources);
    }

    protected virtual void Move(Resource box, Vector3 targetPosition)
    {
        box.transform.position = Vector3.MoveTowards(box.transform.position, targetPosition, _movingSpeed * Time.deltaTime);
    }

    protected virtual void Rotate(Resource box)
    {
        box.transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }

    protected Resource InstantiateBox(Vector3 position, Transform parent, int number = 1, bool isActive = false)
    {
        float yPos = position.y + _yPosGap * number;

        Vector3 targetPosition = new Vector3(position.x, yPos, position.z);

        var box = Instantiate(Cluster.Resource, targetPosition, Quaternion.Euler(_boxInitialRotation));

        box.transform.SetParent(parent);

        box.gameObject.SetActive(isActive);

        return box;
    }

    protected int GetActivatedBoxes()
    {
        int amount = 0;

        foreach (var box in _boxes)
        {
            if (box.gameObject.activeSelf == true)
                amount++;
        }

        return amount;
    }

    protected void DeactivateAllBoxes()
    {
        foreach (var box in _boxes)        
            box.gameObject.SetActive(false);        
    }

    protected void ActivateBoxes(int count)
    {
        if (count < 0)
        {
            print("Count of activated boxes should be positive!");
        }
        else
        {
            if (count > _boxes.Count)
            {
                print("Count of activated boxes more than _boxes!");
            }
            else
            {
                DeactivateAllBoxes();

                for (int i = 0; i < count; i++)                
                    _boxes[i].gameObject.SetActive(true);                
            }
        }
    }
}