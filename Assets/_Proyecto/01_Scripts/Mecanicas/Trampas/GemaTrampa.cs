// =====================================================================
// GemaTrampa.cs
// Proyecto: Inti y Killa
// Descripción: Parece una gema, pero al tocarla activa otras trampas.
//              No es una Gema de verdad: no cuenta para las estrellas, así
//              que nadie está obligado a tocarla.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

public class GemaTrampa : TrampaBase
{
    [SerializeField] private List<TrampaActivable> trampasQueActiva = new List<TrampaActivable>();
    [SerializeField] private string mensajeDeMuerte = "Te dije que no confiaras en las gemas.";

    [Header("Animación de flotar (igual que una gema normal)")]
    [SerializeField] private float alturaFlotacion = 0.15f;
    [SerializeField] private float velocidadFlotacion = 3f;

    private Vector3 posicionInicial;
    private Vector2 tamanoZona;

    public void Configurar(List<TrampaActivable> nuevasTrampas)
    {
        trampasQueActiva = nuevasTrampas;
    }

    protected override void Awake()
    {
        base.Awake();
        posicionInicial = transform.position;
        tamanoZona = new Vector2(0.5f, 0.5f);
    }

    private void Update()
    {
        float desplazamiento = Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = new Vector3(posicionInicial.x, posicionInicial.y + desplazamiento, posicionInicial.z);

        ControladorJugador jugador = BuscarJugadorEnRectangulo(transform.position, tamanoZona);
        if (jugador != null)
        {
            Disparar(jugador);
        }
    }

    private void Disparar(ControladorJugador jugador)
    {
        jugador.RecordarTrampa(mensajeDeMuerte);

        foreach (TrampaActivable trampa in trampasQueActiva)
        {
            if (trampa != null)
            {
                trampa.Activar(jugador);
            }
        }

        Destroy(gameObject);
    }
}
