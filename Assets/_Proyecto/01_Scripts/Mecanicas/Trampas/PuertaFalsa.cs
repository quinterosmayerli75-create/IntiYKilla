// =====================================================================
// PuertaFalsa.cs
// Proyecto: Inti y Killa
// Descripción: Parece la salida, pero se escapa cuando alguien se acerca.
//              No es una PuertaSalida: el nivel no la espera, así que la
//              puerta real siempre está en otro lado.
// =====================================================================

using UnityEngine;

public class PuertaFalsa : TrampaBase
{
    [SerializeField] private Vector2 desplazamientoAlHuir = new Vector2(0f, 6f);
    [SerializeField] private float velocidadHuida = 10f;
    [SerializeField] private Vector2 tamanoZonaDeDeteccion = new Vector2(3f, 2.5f);

    private Vector3 posicionFinal;
    private bool estaHuyendo = false;

    public void Configurar(Vector2 nuevoDesplazamiento)
    {
        desplazamientoAlHuir = nuevoDesplazamiento;
    }

    private void Update()
    {
        if (estaHuyendo == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionFinal, velocidadHuida * Time.deltaTime);
            return;
        }

        ControladorJugador jugador = BuscarJugadorEnRectangulo(transform.position, tamanoZonaDeDeteccion);
        if (jugador != null)
        {
            estaHuyendo = true;
            posicionFinal = transform.position + new Vector3(desplazamientoAlHuir.x, desplazamientoAlHuir.y, 0f);
        }
    }
}
