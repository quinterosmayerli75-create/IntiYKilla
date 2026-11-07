// =====================================================================
// EnemigoPatrulla.cs
// Proyecto: Inti y Killa
// Descripción: Camina de un punto a otro sin parar (serpientes). Su recorrido
//              siempre es igual: se puede saltar o, como todo enemigo, pisar
//              para aturdirlo.
// =====================================================================

using UnityEngine;

public class EnemigoPatrulla : EnemigoBase
{
    [SerializeField] private float limiteIzquierdo = -2f;
    [SerializeField] private float limiteDerecho = 2f;
    [SerializeField] private float velocidad = 2f;

    private float direccion = 1f;

    public void Configurar(float nuevoLimiteIzquierdo, float nuevoLimiteDerecho, float nuevaVelocidad)
    {
        limiteIzquierdo = nuevoLimiteIzquierdo;
        limiteDerecho = nuevoLimiteDerecho;
        velocidad = nuevaVelocidad;
    }

    protected override void MoverEnemigo()
    {
        float posicionX = transform.position.x + direccion * velocidad * Time.deltaTime;

        if (posicionX >= limiteDerecho)
        {
            posicionX = limiteDerecho;
            direccion = -1f;
        }
        else if (posicionX <= limiteIzquierdo)
        {
            posicionX = limiteIzquierdo;
            direccion = 1f;
        }

        transform.position = new Vector3(posicionX, transform.position.y, transform.position.z);
    }
}
