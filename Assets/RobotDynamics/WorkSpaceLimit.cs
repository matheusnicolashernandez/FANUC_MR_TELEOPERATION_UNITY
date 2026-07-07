using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkSpaceLimit : MonoBehaviour
    {
    // Definindo o limite do espaço alcançável do robô (como uma esfera)
    public float maxReachDistance = 5.0f;  // Raio máximo do espaço de trabalho

    // Função para verificar se o alvo está dentro do espaço alcançável
    bool IsTargetInReach(Vector3 target)
    {
        // Calcula a distância entre o alvo e a posição atual do robô
        float distanceToTarget = (target - transform.position).magnitude;

        // Retorna verdadeiro se o alvo estiver dentro do alcance
        return distanceToTarget <= maxReachDistance;
    }

    // Método para ajustar o alvo se ele estiver fora do espaço alcançável
    Vector3 GetAdjustedTargetPosition(Vector3 target)
    {
        // Verifica se o alvo está fora do alcance
        if ((target - transform.position).magnitude > maxReachDistance)
        {
            // Ajusta a posição do alvo para o limite do espaço alcançável
            Vector3 directionToTarget = (target - transform.position).normalized;
            return transform.position + directionToTarget * maxReachDistance;  // Posição mais próxima ao limite
        }

        // Se o alvo já estiver dentro do alcance, retorna a posição original
        return target;
    }

    // Método para mover o robô em direção ao alvo ajustado
    void MoveRobotTowardsTarget(Vector3 targetPosition)
    {
        // Aqui você pode implementar a lógica de movimentação (por exemplo, cinemática inversa)
        // Para simplificação, vamos apenas mover o robô diretamente para o alvo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime);
    }

    // Método chamado a cada frame
    void Update()
    {
        // Exemplo de cálculo do alvo (pode ser uma posição de destino no espaço)
        Vector3 targetPosition = CalculateTargetPosition(); // Aqui você vai calcular o destino desejado

        // Verifica se o alvo está dentro do espaço alcançável
        if (IsTargetInReach(targetPosition))
        {
            // Mover o robô para o alvo
            MoveRobotTowardsTarget(targetPosition);
        }
        else
        {
            // Ajusta o alvo para dentro do espaço alcançável e move o robô
            Vector3 adjustedTarget = GetAdjustedTargetPosition(targetPosition);
            MoveRobotTowardsTarget(adjustedTarget);
        }
    }

    // Método de cálculo do alvo, substitua com a lógica real do seu robô
    Vector3 CalculateTargetPosition()
    {
        // Exemplo de cálculo de posição de destino (aqui você deve colocar a lógica de seu sistema)
        return new Vector3(10, 0, 0); // Apenas um exemplo de destino fora do alcance
    }
}

