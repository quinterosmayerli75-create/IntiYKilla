// =====================================================================
// BloqueQueCierraElRegreso.cs
// Proyecto: Inti y Killa
// Descripción: Un bloque escondido en el piso que sube y tapa el camino de
//              vuelta cuando alguien ya pasó. Es una trampa psicológica:
//              "ya no hay vuelta atrás". No mata a nadie: si alguien está
//              encima del lugar, espera a que se mueva antes de cerrarse.
//              El generador solo la pone donde el regreso no hace falta para
//              ganar (regla de justicia).
// Uso: plataforma con BoxCollider2D; la dispara un SensorTrampa colocado
//      más adelante en el camino.
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BloqueQueCierraElRegreso : TrampaActivable
{
    [SerializeField] private float segundosParaSubir = 0.4f;

    private BoxCollider2D colisionador;
    private SpriteRenderer[] dibujos;
    private Vector3 escalaFinal;
    private Vector3 posicionFinal;
    // Se guarda al empezar: con el choque apagado Unity informa tamaño cero.
    private Vector2 tamanoBloque;

    protected override void Awake()
    {
        base.Awake();
        colisionador = GetComponent<BoxCollider2D>();
        dibujos = GetComponentsInChildren<SpriteRenderer>();
        escalaFinal = transform.localScale;
        posicionFinal = transform.position;
        tamanoBloque = colisionador.bounds.size;

        // Empieza escondido: sin choque y sin dibujo.
        colisionador.enabled = false;
        MostrarDibujos(false);
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        StartCoroutine(Subir());
    }

    private IEnumerator Subir()
    {
        // Nunca se cierra encima de alguien: espera a que el lugar esté libre.
        while (BuscarJugadorEnRectangulo(posicionFinal, tamanoBloque) != null)
        {
            yield return null;
        }

        MostrarDibujos(true);
        float baseDelBloque = posicionFinal.y - tamanoBloque.y / 2f;
        float tiempo = 0f;
        while (tiempo < segundosParaSubir)
        {
            tiempo = tiempo + Time.deltaTime;
            float fraccion = Mathf.Clamp01(tiempo / segundosParaSubir);
            // Crece desde el piso hacia arriba, como si saliera de la tierra.
            float fraccionVisible = Mathf.Max(0.01f, fraccion);
            transform.localScale = new Vector3(escalaFinal.x, escalaFinal.y * fraccionVisible, escalaFinal.z);
            transform.position = new Vector3(posicionFinal.x, baseDelBloque + tamanoBloque.y * fraccionVisible / 2f, posicionFinal.z);
            yield return null;
        }

        transform.localScale = escalaFinal;
        transform.position = posicionFinal;
        colisionador.enabled = true;

        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("Algo se cerró atrás... ya no hay vuelta.", 2f);
        }
    }

    private void MostrarDibujos(bool mostrar)
    {
        foreach (SpriteRenderer dibujo in dibujos)
        {
            dibujo.enabled = mostrar;
        }
    }
}
