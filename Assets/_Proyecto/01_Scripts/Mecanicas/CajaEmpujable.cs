// =====================================================================
// CajaEmpujable.cs
// Proyecto: Inti y Killa
// Descripción: Caja pesada que solo se mueve si la empujan los jugadores
//              necesarios a la vez (por defecto, los dos). Sirve de escalón.
//              Tiene límites: nunca se atasca en un lugar sin solución, y
//              siempre se puede devolver empujándola desde el otro lado.
// Uso: bloque con Rigidbody2D y BoxCollider2D.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CajaEmpujable : MonoBehaviour
{
    [SerializeField] private int jugadoresNecesarios = 2;
    [SerializeField] private float velocidadEmpuje = 2.5f;
    [SerializeField] private float limiteIzquierdo = -14f;
    [SerializeField] private float limiteDerecho = 14f;
    // Hasta qué distancia cuenta un jugador que empuja (el segundo empuja por detrás del primero).
    [SerializeField] private float distanciaDeEmpuje = 1.1f;

    private Rigidbody2D cuerpoFisico;
    private BoxCollider2D colisionador;
    private ControladorJugador[] jugadores;

    public void Configurar(int nuevosNecesarios, float nuevoLimiteIzquierdo, float nuevoLimiteDerecho)
    {
        jugadoresNecesarios = nuevosNecesarios;
        limiteIzquierdo = nuevoLimiteIzquierdo;
        limiteDerecho = nuevoLimiteDerecho;
    }

    private void Awake()
    {
        cuerpoFisico = GetComponent<Rigidbody2D>();
        colisionador = GetComponent<BoxCollider2D>();
        jugadores = FindObjectsByType<ControladorJugador>();
    }

    private void FixedUpdate()
    {
        int empujanHaciaDerecha = ContarEmpujes(1f);
        int empujanHaciaIzquierda = ContarEmpujes(-1f);

        float velocidadX = 0f;
        float posicionX = cuerpoFisico.position.x;

        if (empujanHaciaDerecha >= jugadoresNecesarios && posicionX < limiteDerecho)
        {
            velocidadX = velocidadEmpuje;
        }
        else if (empujanHaciaIzquierda >= jugadoresNecesarios && posicionX > limiteIzquierdo)
        {
            velocidadX = -velocidadEmpuje;
        }

        // Sin empuje suficiente queda fija en X: los jugadores chocan con ella como con un muro.
        if (velocidadX == 0f)
        {
            cuerpoFisico.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            cuerpoFisico.constraints = RigidbodyConstraints2D.FreezeRotation;
            cuerpoFisico.linearVelocity = new Vector2(velocidadX, cuerpoFisico.linearVelocity.y);
        }
    }

    // Cuenta quienes caminan hacia la caja por el lado contrario a la dirección.
    private int ContarEmpujes(float direccion)
    {
        Bounds caja = colisionador.bounds;
        int cantidad = 0;

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador == null || jugador.EstaVivo == false || jugador.DireccionHorizontal * direccion <= 0f)
            {
                continue;
            }

            Bounds cuerpo = jugador.LimitesCuerpo;
            bool mismaAltura = cuerpo.min.y < caja.max.y - 0.1f && cuerpo.max.y > caja.min.y;
            float distancia = 0f;
            if (direccion > 0f)
            {
                distancia = caja.min.x - cuerpo.max.x;
            }
            else
            {
                distancia = cuerpo.min.x - caja.max.x;
            }

            if (mismaAltura == true && distancia >= -0.1f && distancia <= distanciaDeEmpuje)
            {
                cantidad = cantidad + 1;
            }
        }

        return cantidad;
    }
}
