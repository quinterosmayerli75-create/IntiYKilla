// =====================================================================
// PlataformaMovil.cs
// Proyecto: Inti y Killa
// Descripción: Plataforma que va y viene entre su posición inicial (A)
//              y un segundo punto (B) mientras está activa. Sirve como
//              elevador (vertical) o vagoneta (horizontal).
//              Si "activaDesdeElInicio" está marcado, se mueve sola sin
//              necesidad de botón ni palanca.
// Uso: bloque sólido con BoxCollider2D + Rigidbody2D Kinematic.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlataformaMovil : ObjetoActivable
{
    [SerializeField] private Vector2 desplazamientoHastaB = new Vector2(0f, 4f);
    [SerializeField] private float velocidad = 2.5f;
    [SerializeField] private float pausaEnLosExtremos = 0.6f;
    [SerializeField] private bool activaDesdeElInicio = false;

    private Rigidbody2D cuerpoFisico;
    private Vector2 puntoA;
    private Vector2 puntoB;
    private bool vaHaciaB = true;
    private bool seEstaMoviendo = false;
    private float tiempoDePausa = 0f;

    public void Configurar(Vector2 nuevoDesplazamiento, float nuevaVelocidad, bool moverseDesdeElInicio)
    {
        desplazamientoHastaB = nuevoDesplazamiento;
        velocidad = nuevaVelocidad;
        activaDesdeElInicio = moverseDesdeElInicio;
    }

    private void Awake()
    {
        cuerpoFisico = GetComponent<Rigidbody2D>();
        cuerpoFisico.bodyType = RigidbodyType2D.Kinematic;
        // Interpolar hace que el movimiento se vea suave en pantalla.
        cuerpoFisico.interpolation = RigidbodyInterpolation2D.Interpolate;

        puntoA = transform.position;
        puntoB = puntoA + desplazamientoHastaB;
    }

    private void Start()
    {
        if (activaDesdeElInicio == true)
        {
            seEstaMoviendo = true;
        }
    }

    protected override void CambiarEstado(bool activo)
    {
        if (activaDesdeElInicio == true)
        {
            // Una plataforma automática no se detiene con botones.
            return;
        }

        seEstaMoviendo = activo;
    }

    private void FixedUpdate()
    {
        if (seEstaMoviendo == false)
        {
            return;
        }

        // Pequeña espera en cada extremo para que los jugadores puedan subir o bajar.
        if (tiempoDePausa > 0f)
        {
            tiempoDePausa = tiempoDePausa - Time.fixedDeltaTime;
            return;
        }

        Vector2 destino = puntoA;
        if (vaHaciaB == true)
        {
            destino = puntoB;
        }

        Vector2 nuevaPosicion = Vector2.MoveTowards(cuerpoFisico.position, destino, velocidad * Time.fixedDeltaTime);
        cuerpoFisico.MovePosition(nuevaPosicion);

        // Al llegar al destino, damos media vuelta.
        if (Vector2.Distance(nuevaPosicion, destino) < 0.01f)
        {
            vaHaciaB = !vaHaciaB;
            tiempoDePausa = pausaEnLosExtremos;
        }
    }
}
