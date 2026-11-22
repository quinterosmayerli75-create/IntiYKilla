// =====================================================================
// TechoQueCae.cs
// Proyecto: Inti y Killa
// Descripción: Bloque en el techo que cae cuando alguien pasa por debajo.
//              Espera un momento antes de caer: quien sigue corriendo se
//              salva; quien se detiene debajo, no. Cae una sola vez y
//              desaparece, para no tapar nunca el camino.
// Uso: plataforma con BoxCollider2D (el dibujo puede estar en piezas hijas).
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TechoQueCae : TrampaActivable
{
    [SerializeField] private float alturaDelPiso = -3.5f;
    [SerializeField] private float retardoAntesDeCaer = 0.4f;
    [SerializeField] private float velocidadCaida = 45f;
    [SerializeField] private string mensajeDeMuerte = "¿Mirabas al techo? Yo tampoco.";

    private BoxCollider2D colisionador;
    private SpriteRenderer[] dibujos;
    private Vector2 tamanoBloque;
    private bool estaCayendo = false;

    public void Configurar(float nuevaAlturaDelPiso)
    {
        alturaDelPiso = nuevaAlturaDelPiso;
    }

    protected override void Awake()
    {
        base.Awake();
        colisionador = GetComponent<BoxCollider2D>();
        dibujos = GetComponentsInChildren<SpriteRenderer>();
        tamanoBloque = colisionador.bounds.size;
    }

    private void Update()
    {
        Vector2 centroBloque = transform.position;

        if (estaCayendo == true)
        {
            ControladorJugador aplastado = BuscarJugadorEnRectangulo(centroBloque, tamanoBloque);
            if (aplastado != null)
            {
                aplastado.Morir(mensajeDeMuerte);
            }
            return;
        }

        if (SeActivaSola == true && YaActivada == false)
        {
            // La zona de disparo es la columna vacía entre el bloque y el piso.
            float parteInferior = centroBloque.y - tamanoBloque.y / 2f;
            float altoColumna = parteInferior - alturaDelPiso;
            Vector2 centroColumna = new Vector2(centroBloque.x, alturaDelPiso + altoColumna / 2f);
            ControladorJugador jugador = BuscarJugadorEnRectangulo(centroColumna, new Vector2(tamanoBloque.x, altoColumna));
            if (jugador != null)
            {
                Activar(jugador);
            }
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        StartCoroutine(Caer());
    }

    private IEnumerator Caer()
    {
        yield return new WaitForSeconds(retardoAntesDeCaer);

        // Sin collider mientras cae: mata por contacto, pero no empuja a nadie.
        colisionador.enabled = false;
        estaCayendo = true;

        float alturaFinal = alturaDelPiso + tamanoBloque.y / 2f;
        while (transform.position.y > alturaFinal)
        {
            transform.position = transform.position + new Vector3(0f, -velocidadCaida * Time.deltaTime, 0f);
            yield return null;
        }

        estaCayendo = false;
        foreach (SpriteRenderer dibujo in dibujos)
        {
            dibujo.enabled = false;
        }
    }
}
