using RobotDynamics.Controller;
using RobotDynamics.MathUtilities;
using RobotDynamics.Robots;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class RevoluteRobot : RobotBase
{
    public List<Link> Links; // Lista de links do robô
    public List<double> angles; // Ângulos para cada junta
    public List<XRKnob> knobs; // Lista de XRKnobs para controle das juntas
    public GameObject Target; // Objeto alvo para a cinemática inversa
    public float angleRange = 720f; // Faixa de rotação dos knobs
    private bool wasInverseKinematicsActive = false; // Flag para detectar a troca de modo

    void Awake()
    {
        Robot Ro = new Robot()
            .AddJoint('z', new Vector(0, 0, 0.17))
            .AddJoint('y', new Vector(-0.0500, 0, 0.160))
            .AddJoint('y', new Vector(0, 0, 0.4400))
            .AddJoint('x', new Vector(-0.08850, 0, 0.0350))
            .AddJoint('y', new Vector(-0.331500, 0, 0))
            .AddJoint('x', new Vector(-0.0800, 0, 0));

        Links = Ro.Links;
        Robot = Ro;

        angles = new List<double>(new double[Links.Count]);
    }

    void Update()
    {
        switch (currentKinematicsMode)
        {
            case KinematicsMode.Forward:
                if (EnableForwardKinematics)
                {
                    // Se antes estava em cinemática inversa, sincroniza os valores para que os knobs mostrem os ângulos atuais
                    if (wasInverseKinematicsActive)
                    {
                        SyncInverseToForwardKinematics();
                        wasInverseKinematicsActive = false;
                    }

                    if (knobs.Count != angles.Count)
                    {
                        Debug.LogWarning("A quantidade de XRKnobs e juntas não corresponde!");
                        return;
                    }

                    // Atualiza os ângulos com base nos valores dos knobs (modo direto)
                    for (int i = 0; i < angles.Count; i++)
                    {
                        angles[i] = ((knobs[i].value * angleRange) - (angleRange / 2f)) * Mathf.Deg2Rad;
                    }

                    ForwardKinematicsQ = angles.ToArray();
                    SetQ(ForwardKinematicsQ);

                    UseLastQAsInit = true; // Usa o último Q para iniciar a próxima iteração
                }
                break;

            case KinematicsMode.Inverse:
                if (EnableInverseKinematics)
                {
                    // Executa a cinemática inversa para mover o robô em direção ao target
                    FollowTargetOneStep(Target);

                    // Aqui chamamos a sincronização para atualizar os knobs com os novos ângulos calculados via IK
                    SyncInverseToForwardKinematics();

                    wasInverseKinematicsActive = true;
                    UseLastQAsInit = false; // Não utiliza o último Q como inicialização
                }
                break;

            case KinematicsMode.None:
                break;
        }

        if (Robot?.JointController != null)
        {
            Robot.JointController.ReportNewFrame(Time.deltaTime);
        }
        else
        {
            Debug.LogWarning("JointController não está inicializado!");
        }
    }


    // ⚡ Método que sincroniza os valores da cinemática inversa com a direta ⚡
    protected void SyncInverseToForwardKinematics()
    {
        // Verifica se ForwardKinematicsQ está disponível e se seu tamanho bate com o número de juntas (angles)
        if (ForwardKinematicsQ != null && ForwardKinematicsQ.Length == angles.Count)
        {
            // Atualiza a lista 'angles' com os valores atuais calculados (seja por IK ou FK)
            for (int i = 0; i < ForwardKinematicsQ.Length; i++)
            {
                angles[i] = ForwardKinematicsQ[i];
            }
        }
        else
        {
            Debug.LogWarning("ForwardKinematicsQ está nulo ou com tamanho incompatível!");
            return;
        }

        // Atualiza cada XRKnob com o ângulo correspondente
        // O cálculo converte de radianos para graus e mapeia para o intervalo [0,1] esperado pelo knob
        for (int i = 0; i < knobs.Count; i++)
        {
            knobs[i].value = (float)((angles[i] * Mathf.Rad2Deg + (angleRange / 2f)) / angleRange);
          //  Debug.Log($"Knob {i} atualizado para: {knobs[i].value} (Modo: {currentKinematicsMode})");
        }
    }


    public void ToggleKinematicsMode()
    {
        if (currentKinematicsMode == KinematicsMode.Forward)
        {
            SetKinematicsMode(KinematicsMode.Inverse);
            Debug.Log("Cinemática alterada para Inversa.");
        }
        else
        {
            SetKinematicsMode(KinematicsMode.Forward);
            Debug.Log("Cinemática alterada para Direta.");
        }
    }









}

//for (int i = 0; i < angles.Count; i++)
//{
//    angles[i] = ((knobs[i].value * 2 * angleRange) - (angleRange * 2 / 2f)) / 100;
//}