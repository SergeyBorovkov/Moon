using UnityEngine;

public class ProducerVisualizer : ResourceVisualizer
{
    protected override void OnCurrentResourceChanged()
    {
        if (Cluster.CurrentResources < GetActivatedBoxes())
        {           
            ActivateBoxes(Cluster.CurrentResources);
        }

        if (Cluster.CurrentResources > GetActivatedBoxes())
        {            
            VisualizationBox.gameObject.SetActive(true);
            StartVisualization();            
        }
    }

    protected override Vector3 GetStartBoxPosition()
    {       
        return VisualizationPoint.position;
    }    

    protected override Vector3 GetFirstTravelPosition()
    {
        return new Vector3(VisualizationPoint.position.x - PointShift, VisualizationPoint.position.y, VisualizationPoint.position.z);
    }

    protected override Vector3 GetTargetBoxPosition()
    {
        return GetInClusterBoxPosition();
    }
}