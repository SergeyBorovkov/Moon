using UnityEngine;

public class ConsumerVisualizer : ResourceVisualizer
{
    protected override void OnCurrentResourceChanged()
    {
        if (Cluster.CurrentResources < GetActivatedBoxes())
        {
            VisualizationBox.gameObject.SetActive(true);
            StartVisualization();
        }

        ActivateBoxes(Cluster.CurrentResources);
    }

    protected override Vector3 GetStartBoxPosition()
    {
        return GetInClusterBoxPosition();
    }

    protected override Vector3 GetFirstTravelPosition()
    {
        return new Vector3(VisualizationPoint.position.x + PointShift, VisualizationPoint.position.y, VisualizationPoint.position.z);
    }

    protected override Vector3 GetTargetBoxPosition()
    {
        return VisualizationPoint.position;
    }
}