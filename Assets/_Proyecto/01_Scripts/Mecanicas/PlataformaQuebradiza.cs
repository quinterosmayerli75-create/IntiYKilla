// =====================================================================
// PlataformaQuebradiza.cs
// Proyecto: Inti y Killa
// Descripción: Trampa del Salar de Uyuni. Un bloque de sal que tiembla
//              cuando un personaje se para encima, se rompe al poco
//              tiempo y vuelve a aparecer después de unos segundos.
//              Obliga a los jugadores a moverse rápido.
// Uso: agregar a una plataforma que tenga BoxCollider2D (no trigger).
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlataformaQuebradiza : MonoBehaviour
{
    [SerializeField] private float tiempoAntesDeRomperse = 0.6f;
    [SerializeField] private float tiempoParaReaparecer = 3f;
    [SerializeField] private float intensidadTemblor = 0.06f;

    private Vector3 posicionOriginal;
    private Collider2D colisionador;
    // Pueden ser varios: la plataforma puede estar armada con piezas hijas.
    private SpriteRenderer[] dibujos;
    private bool estaRompiendose = false;

    private void Awake()
    {
        posicionOriginal = transform.position;
        colisionador = GetComponent<Collider2D>();
        dibujos = GetComponentsInChildren<SpriteRenderer>();
    }

    private void CambiarVisibilidad(bool esVisible)
    {
        foreach (SpriteRenderer dibujo in dibujos)
        {
            dibujo.enabled = esVisible;
        }
    }

    private void OnCollisionEnter2D(Collision2D choque)
    {
        if (estaRompiendose == true)
        {
            return;
        }

        ControladorJugador jugador = choque.collider.GetComponent<ControladorJugador>();
        if (jugador == null)
        {
            return;
        }

        // Solo se rompe si el jugador cae ENCIMA (no si la golpea de costado o desde abajo).
        float piesDelJugador = choque.collider.bounds.min.y;
        float parteSuperiorPlataforma = colisionador.bounds.max.y;

        if (piesDelJugador >= parteSuperiorPlataforma - 0.1f)
        {
            StartCoroutine(RomperYReaparecer());
        }
    }

    // Una corrutina permite esperar tiempo sin congelar el juego.
    private IEnumerator RomperYReaparecer()
    {
        estaRompiendose = true;

        // 1) Tiembla para avisar que se va a romper.
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < tiempoAntesDeRomperse)
        {
            Vector2 desplazamiento = Random.insideUnitCircle * intensidadTemblor;
            transform.position = new Vector3(posicionOriginal.x + desplazamiento.x, posicionOriginal.y + desplazamiento.y, posicionOriginal.z);
            tiempoTranscurrido = tiempoTranscurrido + Time.deltaTime;
            yield return null;
        }

        // 2) Desaparece.
        transform.position = posicionOriginal;
        colisionador.enabled = false;
        CambiarVisibilidad(false);

        // 3) Espera y vuelve a aparecer.
        yield return new WaitForSeconds(tiempoParaReaparecer);

        colisionador.enabled = true;
        CambiarVisibilidad(true);

        estaRompiendose = false;
    }
}
