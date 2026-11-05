// =====================================================================
// SueloFalso.cs
// Proyecto: Inti y Killa
// Descripción: Parece suelo normal, pero se cae cuando alguien lo pisa.
//              Después de un rato vuelve a su lugar, por si alguien tiene
//              que pasar otra vez.
// Uso: plataforma con BoxCollider2D (el dibujo puede estar en piezas hijas).
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SueloFalso : TrampaActivable
{
    [SerializeField] private float retardoAntesDeCaer = 0.15f;
    [SerializeField] private float velocidadCaida = 12f;
    [SerializeField] private float distanciaCaida = 4f;
    [SerializeField] private float tiempoParaReaparecer = 3f;
    [SerializeField] private string mensajeSiMuere = "Ese suelo no era de verdad...";

    private BoxCollider2D colisionador;
    private SpriteRenderer[] dibujos;
    private Vector3 posicionOriginal;
    private Vector2 centroZona;
    private Vector2 tamanoZona;

    protected override void Awake()
    {
        base.Awake();
        colisionador = GetComponent<BoxCollider2D>();
        dibujos = GetComponentsInChildren<SpriteRenderer>();
        posicionOriginal = transform.position;

        // Zona finita justo encima de la superficie: solo cuenta quien está parado ahí.
        Bounds limites = colisionador.bounds;
        tamanoZona = new Vector2(limites.size.x, 0.15f);
        centroZona = new Vector2(limites.center.x, limites.max.y + 0.05f);
    }

    private void Update()
    {
        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        ControladorJugador jugador = BuscarJugadorEnRectangulo(centroZona, tamanoZona);
        if (jugador != null)
        {
            Activar(jugador);
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        StartCoroutine(CaerYReaparecer(causante));
    }

    private IEnumerator CaerYReaparecer(ControladorJugador causante)
    {
        yield return new WaitForSeconds(retardoAntesDeCaer);

        colisionador.enabled = false;

        // Si muere poco después, el mensaje será el de este suelo.
        if (causante != null)
        {
            causante.RecordarTrampa(mensajeSiMuere);
        }

        float recorrido = 0f;
        while (recorrido < distanciaCaida)
        {
            float paso = velocidadCaida * Time.deltaTime;
            recorrido = recorrido + paso;
            transform.position = transform.position + new Vector3(0f, -paso, 0f);
            yield return null;
        }

        CambiarVisibilidad(false);
        yield return new WaitForSeconds(tiempoParaReaparecer);

        transform.position = posicionOriginal;
        colisionador.enabled = true;
        CambiarVisibilidad(true);
        Rearmar();
    }

    private void CambiarVisibilidad(bool esVisible)
    {
        foreach (SpriteRenderer dibujo in dibujos)
        {
            dibujo.enabled = esVisible;
        }
    }
}
