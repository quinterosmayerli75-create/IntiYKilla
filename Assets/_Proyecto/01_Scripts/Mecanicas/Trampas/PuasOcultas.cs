// =====================================================================
// PuasOcultas.cs
// Proyecto: Inti y Killa
// Descripción: Púas escondidas en el suelo. Salen cuando alguien camina
//              encima, y se esconden de nuevo después de un rato.
//              La zona que las dispara es baja: quien la conoce la salta.
// Uso: objeto vacío con las púas como hijos (dibujos); está en el nivel a la
//      altura del piso.
// =====================================================================

using System.Collections;
using UnityEngine;

public class PuasOcultas : TrampaActivable
{
    [SerializeField] private float ancho = 1.5f;
    [SerializeField] private float altoZonaDeDisparo = 0.5f;
    [SerializeField] private float altoPuas = 0.6f;
    [SerializeField] private float retardoAntesDeSalir = 0.25f;
    [SerializeField] private float tiempoAfuera = 1.2f;
    [SerializeField] private string mensajeDeMuerte = "Ese piso tenía dientes...";

    private SpriteRenderer[] dibujos;
    private bool estanAfuera = false;

    public void Configurar(float nuevoAncho)
    {
        ancho = nuevoAncho;
    }

    protected override void Awake()
    {
        base.Awake();
        dibujos = GetComponentsInChildren<SpriteRenderer>();
        CambiarVisibilidad(false);
    }

    private void Update()
    {
        Vector2 baseDeLaZona = transform.position;

        if (estanAfuera == true)
        {
            Vector2 centroPuas = baseDeLaZona + new Vector2(0f, altoPuas / 2f);
            ControladorJugador herido = BuscarJugadorEnRectangulo(centroPuas, new Vector2(ancho, altoPuas));
            if (herido != null)
            {
                herido.Morir(mensajeDeMuerte);
            }
            return;
        }

        if (SeActivaSola == true && YaActivada == false)
        {
            Vector2 centroZona = baseDeLaZona + new Vector2(0f, altoZonaDeDisparo / 2f);
            ControladorJugador jugador = BuscarJugadorEnRectangulo(centroZona, new Vector2(ancho, altoZonaDeDisparo));
            if (jugador != null)
            {
                Activar(jugador);
            }
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        StartCoroutine(SalirYEsconderse());
    }

    private IEnumerator SalirYEsconderse()
    {
        yield return new WaitForSeconds(retardoAntesDeSalir);

        estanAfuera = true;
        CambiarVisibilidad(true);
        yield return new WaitForSeconds(tiempoAfuera);

        estanAfuera = false;
        CambiarVisibilidad(false);
        Rearmar();
    }

    private void CambiarVisibilidad(bool esVisible)
    {
        foreach (SpriteRenderer dibujo in dibujos)
        {
            if (dibujo.gameObject.name != "Pista sutil")
            {
                dibujo.enabled = esVisible;
            }
        }
    }
}
