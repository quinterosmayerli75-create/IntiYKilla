// =====================================================================
// PlataformaQueHuye.cs
// Proyecto: Inti y Killa
// Descripción: Se aleja justo cuando alguien salta cerca de ella. Huye una
//              sola vez y se queda en su nuevo lugar: quien ya la conoce
//              primero la hace huir y después salta a donde quedó.
// Uso: plataforma con Rigidbody2D Kinematic y collider.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlataformaQueHuye : TrampaBase
{
    [SerializeField] private Vector2 desplazamientoAlHuir = new Vector2(2.5f, 0f);
    [SerializeField] private float velocidadHuida = 8f;
    [SerializeField] private float distanciaDeDeteccion = 3f;

    private Rigidbody2D cuerpoFisico;
    private Vector2 posicionFinal;
    private bool estaHuyendo = false;

    public void Configurar(Vector2 nuevoDesplazamiento)
    {
        desplazamientoAlHuir = nuevoDesplazamiento;
    }

    protected override void Awake()
    {
        base.Awake();
        cuerpoFisico = GetComponent<Rigidbody2D>();
        cuerpoFisico.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        if (estaHuyendo == true)
        {
            return;
        }

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador != null && JugadorSaltaCerca(jugador) == true)
            {
                estaHuyendo = true;
                posicionFinal = cuerpoFisico.position + desplazamientoAlHuir;
                return;
            }
        }
    }

    // Se usa FixedUpdate porque es un cuerpo físico.
    private void FixedUpdate()
    {
        if (estaHuyendo == true)
        {
            Vector2 nuevaPosicion = Vector2.MoveTowards(cuerpoFisico.position, posicionFinal, velocidadHuida * Time.fixedDeltaTime);
            cuerpoFisico.MovePosition(nuevaPosicion);
        }
    }

    private bool JugadorSaltaCerca(ControladorJugador jugador)
    {
        bool estaSubiendo = jugador.VelocidadVertical > 0.1f;
        float distancia = Vector2.Distance(jugador.LimitesCuerpo.center, transform.position);
        return estaSubiendo == true && distancia <= distanciaDeDeteccion;
    }
}
