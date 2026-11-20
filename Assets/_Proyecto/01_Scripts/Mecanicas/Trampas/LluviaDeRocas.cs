// =====================================================================
// LluviaDeRocas.cs
// Proyecto: Inti y Killa
// Descripción: Al activarse caen rocas del techo una tras otra, en fila, de
//              un extremo al otro del tramo. Antes de cada roca aparece su
//              sombra en el piso (regla de justicia): quien mira el piso ve
//              dónde va a caer y puede adelantarse o esperar.
//              Las rocas desaparecen al tocar el piso, nunca tapan el camino.
// Uso: objeto vacío; se activa sola al pasar por debajo o con un SensorTrampa.
// =====================================================================

using System.Collections;
using UnityEngine;

public class LluviaDeRocas : TrampaActivable
{
    [Header("Tramo")]
    [SerializeField] private float inicioX = -3f;
    [SerializeField] private float finX = 3f;
    [SerializeField] private float alturaTecho = 6f;
    [SerializeField] private float alturaPiso = -3.5f;

    [Header("Rocas")]
    [SerializeField] private int cantidadDeRocas = 4;
    [SerializeField] private float tamanoRoca = 0.8f;
    [SerializeField] private float segundosEntreRocas = 0.45f;
    [SerializeField] private float segundosDeSombra = 0.6f;
    [SerializeField] private float velocidadCaida = 16f;
    [SerializeField] private Color colorRoca = new Color(0.45f, 0.38f, 0.32f);
    [SerializeField] private string mensajeDeMuerte = "Una roca del techo le cayó a {0}. ¡Miren las sombras!";

    public void Configurar(float nuevoInicioX, float nuevoFinX, float nuevaAlturaTecho, float nuevaAlturaPiso, int nuevaCantidad)
    {
        inicioX = nuevoInicioX;
        finX = nuevoFinX;
        alturaTecho = nuevaAlturaTecho;
        alturaPiso = nuevaAlturaPiso;
        cantidadDeRocas = nuevaCantidad;
    }

    public void ConfigurarRitmo(float nuevosSegundosEntreRocas)
    {
        segundosEntreRocas = nuevosSegundosEntreRocas;
    }

    // Con esto la misma trampa sirve para rocas o para púas que caen del techo.
    public void ConfigurarAspecto(Color nuevoColor, float nuevoTamano, string nuevoMensaje)
    {
        colorRoca = nuevoColor;
        tamanoRoca = nuevoTamano;
        mensajeDeMuerte = nuevoMensaje;
    }

    private void Update()
    {
        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        // Se activa sola cuando alguien entra a la columna bajo el tramo.
        float centroX = (inicioX + finX) / 2f;
        float ancho = Mathf.Abs(finX - inicioX);
        Vector2 centro = new Vector2(centroX, (alturaTecho + alturaPiso) / 2f);
        ControladorJugador jugador = BuscarJugadorEnRectangulo(centro, new Vector2(ancho, alturaTecho - alturaPiso));
        if (jugador != null)
        {
            Activar(jugador);
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        if (causante != null)
        {
            causante.RecordarTrampa("Las sombras en el piso avisan dónde cae cada roca.");
        }
        StartCoroutine(SoltarRocas());
    }

    private IEnumerator SoltarRocas()
    {
        for (int indice = 0; indice < cantidadDeRocas; indice++)
        {
            // Reparte las rocas de forma pareja entre el inicio y el final.
            float fraccion = 0f;
            if (cantidadDeRocas > 1)
            {
                fraccion = (float)indice / (float)(cantidadDeRocas - 1);
            }
            float posicionX = Mathf.Lerp(inicioX, finX, fraccion);
            StartCoroutine(SombraYRoca(posicionX));
            yield return new WaitForSeconds(segundosEntreRocas);
        }
    }

    private IEnumerator SombraYRoca(float posicionX)
    {
        // 1) La sombra en el piso: el aviso.
        GameObject sombra = CrearDibujo("Sombra de roca", new Vector2(posicionX, alturaPiso + 0.05f), new Vector2(tamanoRoca, 0.12f), new Color(0f, 0f, 0f, 0.45f));
        yield return new WaitForSeconds(segundosDeSombra);

        // 2) La roca cae del techo y mata a quien toque mientras cae.
        GameObject roca = CrearDibujo("Roca que cae", new Vector2(posicionX, alturaTecho), new Vector2(tamanoRoca, tamanoRoca), colorRoca);
        float alturaFinal = alturaPiso + tamanoRoca / 2f;
        while (roca.transform.position.y > alturaFinal)
        {
            roca.transform.position = roca.transform.position + new Vector3(0f, -velocidadCaida * Time.deltaTime, 0f);
            ControladorJugador aplastado = BuscarJugadorEnRectangulo(roca.transform.position, new Vector2(tamanoRoca * 0.85f, tamanoRoca * 0.85f));
            if (aplastado != null)
            {
                aplastado.Morir(mensajeDeMuerte.Replace("{0}", aplastado.NombreJugador));
            }
            yield return null;
        }

        // 3) Al tocar el piso se deshace: nunca queda bloqueando el paso.
        Destroy(roca);
        Destroy(sombra);
    }

    private GameObject CrearDibujo(string nombre, Vector2 posicion, Vector2 tamano, Color color)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(transform);
        objeto.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        objeto.transform.localScale = new Vector3(tamano.x, tamano.y, 1f);
        SpriteRenderer dibujo = objeto.AddComponent<SpriteRenderer>();
        dibujo.sprite = SpriteBase;
        dibujo.color = color;
        dibujo.sortingOrder = 9;
        return objeto;
    }
}
