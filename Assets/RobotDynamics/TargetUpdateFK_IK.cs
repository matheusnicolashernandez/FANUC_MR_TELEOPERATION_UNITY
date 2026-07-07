using RobotDynamics.Controller;
using RobotDynamics.MathUtilities;
using RobotDynamics.Robots;
using System.Collections.Generic;
using UnityEngine;
using static RobotBase;

public class TargetUpdateFK_IK : MonoBehaviour
{
    // Referência ao RevoluteRobot // é uma declaração de uma referência pública ao componente RevoluteRobot.
    // Isso significa que você está criando uma variável RobotAssembly que pode armazenar a instância do script RevoluteRobot (o objeto que contém o código do seu robô).
    public RevoluteRobot RobotAssembly;

    // Link final (Link 6) do robô
    public GameObject Link6;

    // Objeto que será atualizado (target)
    public GameObject targetObject;

    private Vector3 lastPos;
    private Quaternion lastRot;

   
    void Update()
    {
        // Verifica se o RevoluteRobot está atribuído corretamente
        if (RobotAssembly == null || targetObject == null || Link6 == null)
        {
            Debug.LogWarning("RevoluteRobot, Link6 ou targetObject não atribuídos!");
            return;
        }

        // Verifica o modo de cinemática no RevoluteRobot
        switch (RobotAssembly.currentKinematicsMode)
        {
            case KinematicsMode.Forward:
                // Cinemática direta: ativa o código do TargetUpdateFK_IK
                if (RobotAssembly.EnableForwardKinematics)
                {
                    UpdateTargetPositionFromLink6();
                }
                break;

            case KinematicsMode.Inverse:
                // Cinemática inversa: desativa o código do TargetUpdateFK_IK
                // Não faz nada aqui, então o targetObject não é atualizado
                break;
        }
    }

    private void UpdateTargetPositionFromLink6()
    {
        // Matriz global do link 6
        Matrix4x4 link6Matrix = Link6.transform.localToWorldMatrix;

        // Extrai a posição e rotação diretamente do link 6 no espaço global
        lastPos = link6Matrix.GetColumn(3);  // Posição do link 6
        lastRot = link6Matrix.rotation;      // Rotação do link 6

        // Atualiza a posição e rotação do targetObject com base no Link 6
        targetObject.transform.position = lastPos;
        targetObject.transform.rotation = lastRot;
    }
}
