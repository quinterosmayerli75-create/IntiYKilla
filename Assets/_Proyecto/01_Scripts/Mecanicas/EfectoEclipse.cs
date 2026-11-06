// =====================================================================
// EfectoEclipse.cs
// Proyecto: Inti y Killa
// Descripción: Lo que se ve cuando ocurre un eclipse: la pantalla se oscurece
//              mientras la luna pasa por delante del sol, y luego vuelve la luz.
//              Se dibuja como interfaz para quedar encima de todo el nivel y
//              para que la oscuridad no dependa de las luces de la escena.
//              Se crea mientras el juego corre y se borra solo al terminar;
//              el juego no se detiene durante el efecto.
// =====================================================================

using UnityEngine;
using UnityEngine.UI;

public class EfectoEclipse : MonoBehaviour
{
    private const float duracion = 2.4f;
    private const float oscuridadMaxima = 0.65f;
    // Distancia desde el centro a la que empieza y termina la luna.
    private const float recorridoLuna = 620f;
    private const float alturaAstros = 300f;
    private const float tamanoSol = 230f;
    private const float tamanoCorona = 560f;

    private static EfectoEclipse efectoActual = null;

    private Image imagenOscuridad;
    private Image imagenCorona;
    private Image imagenSol;
    private Image imagenLuna;
    private RectTransform posicionLuna;
    private float tiempo = 0f;
    private bool lunaConDibujoPropio = false;

    // Si ya hay un eclipse en pantalla, se reemplaza en vez de sumar otra oscuridad.
    public static EfectoEclipse Reproducir(Sprite spriteCirculo, Sprite spriteResplandor, Sprite spriteLuna)
    {
        if (efectoActual != null)
        {
            Destroy(efectoActual.gameObject);
        }

        GameObject objeto = new GameObject("Efecto de eclipse");
        efectoActual = objeto.AddComponent<EfectoEclipse>();
        efectoActual.Construir(spriteCirculo, spriteResplandor, spriteLuna);
        efectoActual.AplicarProgreso(0f);
        return efectoActual;
    }

    private void Construir(Sprite spriteCirculo, Sprite spriteResplandor, Sprite spriteLuna)
    {
        Canvas lienzo = gameObject.AddComponent<Canvas>();
        lienzo.renderMode = RenderMode.ScreenSpaceOverlay;
        lienzo.sortingOrder = 100;

        CanvasScaler escalador = gameObject.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920f, 1080f);
        escalador.matchWidthOrHeight = 0.5f;

        imagenOscuridad = CrearImagen("Oscuridad", null, Vector2.zero, Vector2.zero);
        RectTransform rectanguloOscuridad = imagenOscuridad.rectTransform;
        rectanguloOscuridad.anchorMin = Vector2.zero;
        rectanguloOscuridad.anchorMax = Vector2.one;
        rectanguloOscuridad.offsetMin = Vector2.zero;
        rectanguloOscuridad.offsetMax = Vector2.zero;

        Vector2 posicionAstros = new Vector2(0f, alturaAstros);
        imagenCorona = CrearImagen("Corona", spriteResplandor, posicionAstros, new Vector2(tamanoCorona, tamanoCorona));
        imagenSol = CrearImagen("Sol", spriteCirculo, posicionAstros, new Vector2(tamanoSol, tamanoSol));

        // Si todavía no hay dibujo de la luna, se usa el círculo.
        Sprite dibujoLuna = spriteLuna;
        lunaConDibujoPropio = true;
        if (dibujoLuna == null)
        {
            dibujoLuna = spriteCirculo;
            lunaConDibujoPropio = false;
        }
        imagenLuna = CrearImagen("Luna", dibujoLuna, posicionAstros, new Vector2(tamanoSol, tamanoSol));
        posicionLuna = imagenLuna.rectTransform;
    }

    private Image CrearImagen(string nombre, Sprite sprite, Vector2 posicion, Vector2 tamano)
    {
        GameObject objeto = new GameObject(nombre, typeof(RectTransform));
        objeto.transform.SetParent(transform, false);

        RectTransform rectangulo = objeto.GetComponent<RectTransform>();
        rectangulo.anchoredPosition = posicion;
        rectangulo.sizeDelta = tamano;

        Image imagen = objeto.AddComponent<Image>();
        imagen.sprite = sprite;
        // Es solo un dibujo: no debe bloquear clics.
        imagen.raycastTarget = false;
        return imagen;
    }

    private void Update()
    {
        tiempo = tiempo + Time.deltaTime;
        float progreso = tiempo / duracion;

        if (progreso >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        AplicarProgreso(progreso);
    }

    // progreso va de 0 (empieza) a 1 (termina). 0.5 es el momento del eclipse total.
    public void AplicarProgreso(float progreso)
    {
        // Entra y sale en el primer y último cuarto; en el medio se mantiene.
        float intensidad = Mathf.Clamp01(Mathf.Min(progreso, 1f - progreso) * 4f);

        imagenOscuridad.color = new Color(0.02f, 0.02f, 0.10f, oscuridadMaxima * intensidad);
        imagenSol.color = new Color(1f, 0.82f, 0.30f, intensidad);

        // La luna avanza despacio cerca del centro, para que se vea el eclipse total.
        float avance = progreso * 2f - 1f;
        float posicionX = avance * avance * avance * recorridoLuna;
        posicionLuna.anchoredPosition = new Vector2(posicionX, alturaAstros);
        // Sin dibujo propio la luna es un disco oscuro, como se ve en un eclipse real.
        // Con dibujo propio se muestra con sus colores.
        Color colorLuna = new Color(0.10f, 0.11f, 0.20f);
        if (lunaConDibujoPropio == true)
        {
            colorLuna = Color.white;
        }
        imagenLuna.color = new Color(colorLuna.r, colorLuna.g, colorLuna.b, intensidad);

        // La corona brilla más cuanto mejor tapa la luna al sol.
        float cobertura = Mathf.Clamp01(1f - Mathf.Abs(posicionX) / tamanoSol);
        imagenCorona.color = new Color(1f, 0.85f, 0.55f, intensidad * cobertura * 0.8f);
    }

    private void OnDestroy()
    {
        if (efectoActual == this)
        {
            efectoActual = null;
        }
    }
}
