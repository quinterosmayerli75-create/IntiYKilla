// =====================================================================
// GemaIman.cs
// Proyecto: Inti y Killa
// Descripción: Una gema que parece normal. Al tomarla se vuelve un imán:
//              desde los dos lados llegan piedras atraídas hacia el lugar
//              donde estaba el jugador al tomarla. Quien se queda quieto
//              admirando su gema recibe las piedras; quien se mueve enseguida
//              se salva. La pista: alrededor de esta gema late un anillo gris.
// Uso: objeto con SpriteRenderer (la gema). No cuenta para las estrellas.
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GemaIman : TrampaBase
{
    [SerializeField] private Vector2 tamanoParaTomar = new Vector2(0.7f, 0.7f);
    [SerializeField] private float distanciaDeLasPiedras = 6f;
    [SerializeField] private float segundosDeViaje = 1.1f;
    [SerializeField] private float tamanoPiedra = 0.6f;
    [SerializeField] private string mensajeDeMuerte = "La gema era un imán: las piedras golpearon a {0}.";

    private SpriteRenderer dibujo;
    private bool yaLaTomaron = false;

    protected override void Awake()
    {
        base.Awake();
        dibujo = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (yaLaTomaron == true)
        {
            return;
        }

        ControladorJugador jugador = BuscarJugadorEnRectangulo(transform.position, tamanoParaTomar);
        if (jugador != null)
        {
            yaLaTomaron = true;
            dibujo.enabled = false;
            foreach (Transform hijo in transform)
            {
                hijo.gameObject.SetActive(false);
            }
            jugador.RecordarTrampa("La gema con anillo gris era un imán: muévete apenas la tomes.");
            if (GestorNivel.Instancia != null)
            {
                GestorNivel.Instancia.MostrarAviso("¡La gema era un imán!", 1.5f);
            }

            // Las piedras bajan en diagonal desde arriba hasta el lugar donde estaba
            // el jugador al tomarla: quien se aleja no queda en su camino.
            Vector2 destino = jugador.LimitesCuerpo.center;
            StartCoroutine(TraerPiedra(destino + new Vector2(-distanciaDeLasPiedras * 0.5f, distanciaDeLasPiedras), destino));
            StartCoroutine(TraerPiedra(destino + new Vector2(distanciaDeLasPiedras * 0.5f, distanciaDeLasPiedras), destino));
        }
    }

    private IEnumerator TraerPiedra(Vector2 origen, Vector2 destino)
    {
        GameObject piedra = new GameObject("Piedra atraida");
        piedra.transform.position = new Vector3(origen.x, origen.y, 0f);
        piedra.transform.localScale = new Vector3(tamanoPiedra, tamanoPiedra, 1f);
        SpriteRenderer dibujoPiedra = piedra.AddComponent<SpriteRenderer>();
        dibujoPiedra.sprite = SpriteBase;
        dibujoPiedra.color = new Color(0.45f, 0.45f, 0.5f);
        dibujoPiedra.sortingOrder = 9;

        float tiempo = 0f;
        while (tiempo < segundosDeViaje)
        {
            tiempo = tiempo + Time.deltaTime;
            float fraccion = tiempo / segundosDeViaje;
            // Aceleran al acercarse, como atraídas por un imán.
            Vector2 posicion = Vector2.Lerp(origen, destino, fraccion * fraccion);
            piedra.transform.position = new Vector3(posicion.x, posicion.y, 0f);

            ControladorJugador golpeado = BuscarJugadorEnRectangulo(posicion, new Vector2(tamanoPiedra, tamanoPiedra));
            if (golpeado != null)
            {
                golpeado.Morir(mensajeDeMuerte.Replace("{0}", golpeado.NombreJugador));
            }
            yield return null;
        }

        Destroy(piedra);
    }
}
