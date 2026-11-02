// =====================================================================
// PuertaMecanica.cs
// Proyecto: Inti y Killa
// Descripción: Compuerta de piedra que se desliza (normalmente hacia
//              arriba) cuando se activa con botones o palancas, y vuelve
//              a cerrarse cuando se desactiva.
// Uso: bloque sólido con BoxCollider2D + Rigidbody2D Kinematic.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PuertaMecanica : ObjetoActivable
{
    [SerializeField] private Vector2 desplazamientoAlAbrir = new Vector2(0f, 4.5f);
    [SerializeField] private float velocidad = 6f;

    private Rigidbody2D cuerpoFisico;
    private Vector2 posicionCerrada;
    private Vector2 posicionAbierta;
    private bool debeEstarAbierta = false;

    public void Configurar(Vector2 nuevoDesplazamiento, float nuevaVelocidad)
    {
        desplazamientoAlAbrir = nuevoDesplazamiento;
        velocidad = nuevaVelocidad;
    }

    private void Awake()
    {
        cuerpoFisico = GetComponent<Rigidbody2D>();
        cuerpoFisico.bodyType = RigidbodyType2D.Kinematic;

        posicionCerrada = transform.position;
        posicionAbierta = posicionCerrada + desplazamientoAlAbrir;
    }

    protected override void CambiarEstado(bool activo)
    {
        debeEstarAbierta = activo;
    }

    private void FixedUpdate()
    {
        Vector2 destino = posicionCerrada;
        if (debeEstarAbierta == true)
        {
            destino = posicionAbierta;
        }

        // MoveTowards avanza un poquito cada paso de física hasta llegar.
        Vector2 nuevaPosicion = Vector2.MoveTowards(cuerpoFisico.position, destino, velocidad * Time.fixedDeltaTime);
        cuerpoFisico.MovePosition(nuevaPosicion);
    }
}
