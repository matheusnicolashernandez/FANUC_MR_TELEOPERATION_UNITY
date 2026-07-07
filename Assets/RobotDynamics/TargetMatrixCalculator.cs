using UnityEngine;

public class TargetMatrixCalculator : MonoBehaviour
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

        // Exibe a matriz local no console
        Debug.Log("Matriz de Transformação Local do Target em relação ao RobotBase:\n" + MatrixToString(localMatrix));
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
}
