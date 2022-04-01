using System.Collections;
using UnityEngine;

public class PlayerVisualizer : ResourceVisualizer
{
    [SerializeField] private Player _player;
    private Vector3 _interactionClusterBoxPosition;

    private void Update()
    {
        if (_player.InteractionCluster == null && MoveJob !=null)
        {
            StopCoroutine(MoveJob);
            MoveJob = null;
            VisualizationBox.gameObject.SetActive(false);
            ActivateBoxes(Cluster.CurrentResources);
        }
    }

    protected override void OnCurrentResourceChanged()
    {
        if (Cluster.CurrentResources > GetActivatedBoxes())
        {        
            VisualizationBox.gameObject.SetActive(true);            
            StartVisualization();
        }

        if (Cluster.CurrentResources < GetActivatedBoxes())
        {
            VisualizationBox.gameObject.SetActive(true);
            StartVisualization();
            ActivateBoxes(Cluster.CurrentResources);           
        }
    }      

    protected override Vector3 GetStartBoxPosition()
    {
        Vector3 startPosition;                

        if (Cluster.CurrentResources > GetActivatedBoxes())        
            startPosition = GetBoxPositionInInteractionCluster();
        else
            startPosition = GetInClusterBoxPosition();

        return startPosition;
    }

    protected override Vector3 GetFirstTravelPosition()
    {
        return new Vector3(VisualizationPoint.position.x, GetInClusterBoxPosition().y + PointShift, VisualizationPoint.position.z);        
    }

    protected override Vector3 GetTargetBoxPosition()
    {
        Vector3 targetPosition;

        if (Cluster.CurrentResources > GetActivatedBoxes() )
            targetPosition = GetInClusterBoxPosition(); 
        else 
            targetPosition = GetBoxPositionInInteractionCluster();

        return targetPosition;
    } 

    private Vector3 GetBoxPositionInInteractionCluster()
    {
        if (_player.InteractionCluster != null && _player.InteractionCluster.TryGetComponent(out ResourceVisualizer visualizer))        
            return visualizer.CurrentBoxPosition;        
        else
            return _interactionClusterBoxPosition;
    }

    protected override IEnumerator TwoPointMove(Resource box)
    {
        if (_player.InteractionCluster != null)
        {
            while (box.transform.position != GetFirstTravelPosition() && GetFirstTravelPosition() != Vector3.zero)
            {
                Move(box, GetFirstTravelPosition());
                Rotate(box);
                yield return null;
            }

            while (box.transform.position != GetTargetBoxPosition() && GetTargetBoxPosition() != Vector3.zero)
            {
                Move(box, GetTargetBoxPosition());
                Rotate(box);
                yield return null;
            }
        }

        VisualizationBox.gameObject.SetActive(false);
        ActivateBoxes(Cluster.CurrentResources);        
    }    
}