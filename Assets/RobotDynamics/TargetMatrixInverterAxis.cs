using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMatrixInverterAxis : MonoBehaviour
{
    // Objeto que representa o referencial do robô
    public GameObject RobotBase;

    // Objeto alvo cuja posição e orientação serão calculadas
    public GameObject Target;

    void Update()
    {
        if (RobotBase == null || Target == null)
        {
            Debug.LogWarning("RobotBase ou Target não atribuídos!");
            return;
        }

        // Matriz global do RobotBase (referencial do robô)
        Matrix4x4 robotMatrix = RobotBase.transform.localToWorldMatrix;

        // Matriz global do Target
        Matrix4x4 targetMatrix = Target.transform.localToWorldMatrix;

        // Calcula a matriz local do Target em relação ao referencial do RobotBase
        Matrix4x4 localMatrix = robotMatrix.inverse * targetMatrix;

        // Aplica a transformação de troca de eixos (X <-> Z)
        Matrix4x4 transformedMatrix = SwapAxesTransformation(localMatrix);

        // Exibe a matriz transformada no console
        Debug.Log("Matriz Transformada Local do Target em relação ao RobotBase:\n" + MatrixToString(transformedMatrix));
    }

    /// <summary>
    /// Converte uma matriz 4x4 para string formatada para exibição
    /// </summary>
    /// <param name="matrix">Matriz 4x4</param>
    /// <returns>String formatada</returns>
    private string MatrixToString(Matrix4x4 matrix)
    {
        return string.Format(
            "{0:F3}\t{1:F3}\t{2:F3}\t{3:F3}\n{4:F3}\t{5:F3}\t{6:F3}\t{7:F3}\n{8:F3}\t{9:F3}\t{10:F3}\t{11:F3}\n{12:F3}\t{13:F3}\t{14:F3}\t{15:F3}",
            matrix.m00, matrix.m01, matrix.m02, matrix.m03,
            matrix.m10, matrix.m11, matrix.m12, matrix.m13,
            matrix.m20, matrix.m21, matrix.m22, matrix.m23,
            matrix.m30, matrix.m31, matrix.m32, matrix.m33
        );
    }

    /// <summary>
    /// Aplica a transformação para trocar os eixos X e Z, mantendo Y.
    /// </summary>
    /// <param name="matrix">A matriz a ser transformada</param>
    /// <returns>A matriz transformada</returns>
    private Matrix4x4 SwapAxesTransformation(Matrix4x4 matrix)
    {
        // Matriz de rotação que troca os eixos X e Z
        Matrix4x4 swapMatrix = new Matrix4x4
        {
            m00 = 0f,  // X -> Z
            m01 = 0f,
            m02 = 1f,  // X -> Z
            m03 = 0f,
            m10 = 0f,
            m11 = 1f,  // Y -> Y (não muda)
            m12 = 0f,
            m13 = 0f,
            m20 = 1f,  // Z -> X
            m21 = 0f,
            m22 = 0f,  // Z -> X
            m23 = 0f,
            m30 = 0f,
            m31 = 0f,
            m32 = 0f,
            m33 = 1f   // Não modifica a transladação
        };

        // Multiplica a matriz original pela matriz de troca de eixos
        return swapMatrix * matrix;
    }
}
