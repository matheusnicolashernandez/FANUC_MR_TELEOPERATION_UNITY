using RobotDynamics.Controller;
using RobotDynamics.MathUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RobotBase : MonoBehaviour
{
    [Range(0, 1)]
    [Tooltip("A value used for pseudo inverses of matrices to stabilize in the case of singularities. Is added to the diagonal entries of the matrix before inverting.")]
    public float Lambda = 0.001f;
    [Range(0, 1)]
    [Tooltip("Step size factor in the numeric algorithm. [q = q + dq * alpha]")]
    public float Alpha = 0.05f;
    [Tooltip("If true, then linear joints will make larger step sizes")]
    public bool PrioritizeLinearJoints = true;
    [Range(0, 5)]
    [Tooltip("Proportional controller parameter used for smoothing the angles")]
    public float Kp = 2;
    [Range(0, 500)]
    [Tooltip("The maximum number of iterations per update inverse kinematics call")]
    public int MaxIter = 100;
    [Tooltip("The tolerance of the end effector's position and orientation")]
    public float Tolerance = 0.001f;

    // Limites personalizados para cada junta (em radianos)
    public List<JointLimits> jointLimits;

    protected RobotDynamics.Robots.Robot Robot;
    public List<JointHandler> Joints;

    [HideInInspector] public bool EnableInverseKinematics = true;
    [HideInInspector] public bool EnableForwardKinematics = true;
    [HideInInspector] public bool isOutOfWorkspace = false;

    public bool EnablePController = false;
    public bool UseLastQAsInit = false;

    // Modo de cinemática (direta ou inversa)
    public enum KinematicsMode { None, Inverse, Forward }
    public KinematicsMode currentKinematicsMode = KinematicsMode.None;

    private double[] last_q;
    public double[] ForwardKinematicsQ;  // Variável adicionada para armazenar FK
    private Matrix alpha;

    public void Start()
    {
        Robot.AttachJointController(Kp, 0.01f);
        Robot.JointController.jointsChangedEvent += ControlledJointsChanged;

        alpha = Matrix.Eye(Robot.Links.Count) * Alpha;

        if (PrioritizeLinearJoints)
        {
            for (int i = 0; i < Robot.Links.Count; i++)
            {
                if (Robot.Links[i].Type == RobotDynamics.Robots.Link.JointType.Linear)
                {
                    alpha.matrix[i, i] *= 20;
                }
            }
        }

        // Inicializando limites de cada junta diretamente em radianos
        jointLimits = new List<JointLimits>
        {
            new JointLimits(-2.96706, 2.96706),
            new JointLimits(-2.53073, 1.74533),
            new JointLimits(-1.13446,  3.71683),
            new JointLimits(-3.316125, 3.316125), 
            new JointLimits(-1.8326, 1.8326),
            new JointLimits(-2.96706, 2.96706)
        };

        // Inicializa as juntas com valores neutros
        last_q = new double[Robot.Links.Count];
        ForwardKinematicsQ = new double[Robot.Links.Count];
    }

    public void SetKinematicsMode(KinematicsMode mode)
    {
        currentKinematicsMode = mode;

        switch (currentKinematicsMode)
        {
            case KinematicsMode.Inverse:
                EnableInverseKinematics = true;
                EnableForwardKinematics = false;
                break;

            case KinematicsMode.Forward:
                EnableInverseKinematics = false;
                EnableForwardKinematics = true;
                ComputeForwardKinematics(); // Atualiza FK com base no estado atual
                break;

            case KinematicsMode.None:
                EnableInverseKinematics = false;
                EnableForwardKinematics = false;
                break;
        }
    }

    public void SetQ(double[] q) // era private esse ----------------------
    {
        // Limitar os ângulos das juntas antes de aplicar
        for (int i = 0; i < q.Length; i++)
        {
            q[i] = LimitJointAngle(q[i], jointLimits[i]);
        }

        last_q = q; // Atualiza o estado das juntas
        ForwardKinematicsQ = q; // Garante que FK use os mesmos valores da IK

        var transformations = Robot.ComputeForwardKinematics(q);

        for (int i = 0; i < transformations.Count - 1; i++)
        {
            Joints[i].SetJointValue(transformations[i], transformations[i + 1], Robot.Links[i], q[i]);
        }
    }

    public void ComputeForwardKinematics()
    {
        if (EnableForwardKinematics)
        {
            var transformations = Robot.ComputeForwardKinematics(ForwardKinematicsQ);

            for (int i = 0; i < transformations.Count - 1; i++)
            {
                Joints[i].SetJointValue(transformations[i], transformations[i + 1], Robot.Links[i], ForwardKinematicsQ[i]);
            }
        }
    }

    private Vector3 lastPos;
    private Quaternion lastRot;


    public void FollowTargetOneStep(GameObject Target)
    {
        if (EnableInverseKinematics)
        {
            
            // Verifica se a posição atual é diferente da última posição
            if ((lastPos - Target.transform.localPosition).sqrMagnitude != 0)
            {
                if (Target.transform.localPosition != lastPos || Target.transform.localRotation != lastRot)
                {
                    lastPos = Target.transform.localPosition;
                    lastRot = Target.transform.localRotation;

                    // Inverse Kinematics
                    Vector r_des = Target.transform.localPosition.ToVector();
                    RotationMatrix C_des = Target.transform.EulerAnglesToRotationMatrix();

                    var result = Robot.ComputeInverseKinematics(r_des, C_des, alpha, Lambda, MaxIter, Tolerance, UseLastQAsInit ? last_q : null);

                    if (result.DidConverge)
                    {
                        isOutOfWorkspace = false;
                        ForwardKinematicsQ = result.q; // Sincroniza FK com IK
                        last_q = result.q; // Atualiza o estado das juntas

                        if (!EnablePController)
                        {
                            SetQ(result.q);
                        }
                    }
                    else
                    {
                        isOutOfWorkspace = true;
                        Debug.Log("Fora da área de trabalho do robô.");
                    }
                }
            }
        }
    }



    private void ControlledJointsChanged(object sender, JointsChangedEventArgs e)
    {
        if (!EnablePController) return;

        if (e.DidConverge)
            SetQ(e.joint_values);
    }

    // Função para limitar os ângulos de uma junta
    private double LimitJointAngle(double angle, JointLimits limits)
    {
        return Mathf.Clamp((float)angle, (float)limits.MinAngle, (float)limits.MaxAngle);
    }
}



// Estrutura para armazenar os limites das juntas
public struct JointLimits
{
    public double MinAngle;
    public double MaxAngle;

    public JointLimits(double minAngle, double maxAngle)
    {
        MinAngle = minAngle;
        MaxAngle = maxAngle;
    }
}