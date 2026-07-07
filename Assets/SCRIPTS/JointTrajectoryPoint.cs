[System.Serializable]
public class JointTrajectoryPoint
{
    // Array to store joint positions
    public float[] positions;

    // Array to store joint velocities
    public float[] velocities;

    // Array to store joint efforts (if necessary)
    public float[] efforts;

    // Constructor that initializes the arrays for the number of joints
    public JointTrajectoryPoint(int numJoints)
    {
        positions = new float[numJoints];
        velocities = new float[numJoints];
        efforts = new float[numJoints];
    }
}