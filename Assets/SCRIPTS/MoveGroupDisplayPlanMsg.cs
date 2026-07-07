[System.Serializable]
public class MoveGroupDisplayPlanMsg
{
    public JointTrajectoryPoint[] trajectory_points;  // Array de pontos de trajetória

    public MoveGroupDisplayPlanMsg(int numPoints, int numJoints)
    {
        trajectory_points = new JointTrajectoryPoint[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            trajectory_points[i] = new JointTrajectoryPoint(numJoints);
        }
    }
}
