// =====================================================================
// CarruselMundos.cs
// Proyecto: Inti y Killa
// Descripción: Fila de tarjetas de mundo que se pasa hacia los lados, con el
//              mouse o con las flechas. Al soltar, la tarjeta más cercana se
//              acomoda sola en el centro de la pantalla. La tarjeta central se
//              ve más grande y decide el paisaje del fondo y el punto encendido.
// Uso: en el mismo objeto que el ScrollRect. Las tarjetas se ordenan desde
//      aquí, y no con un Layout Group, porque el centrado depende del ancho
//      real de la pantalla, que solo se conoce con el juego abierto.
// =====================================================================

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class CarruselMundos : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("Partes")]
    [SerializeField] private RectTransform[] tarjetas;
    [SerializeField] private Image[] puntos;
    [SerializeField] private FondoMundo fondo;
    [SerializeField] private Button flechaAnterior;
    [SerializeField] private Button flechaSiguiente;

    [Header("Movimiento")]
    [SerializeField] private float pasoEntreTarjetas = 470f;
    [SerializeField] private float escalaTarjetaLejana = 0.84f;
    [SerializeField] private float suavidad = 10f;
    // Con arrastrar esta parte de una tarjeta ya se pasa a la siguiente.
    [SerializeField] private float parteParaCambiar = 0.15f;

    private ScrollRect desplazamiento;
    private RectTransform contenido;
    private RectTransform ventana;
    private float anchoVentanaAnterior = -1f;
    private int indiceObjetivo = 0;
    private bool estaArrastrando = false;
    private float posicionAlEmpezarArrastre = 0f;

    public int MundoCentral
    {
        get { return indiceObjetivo + 1; }
    }

    public void Configurar(RectTransform[] nuevasTarjetas, Image[] nuevosPuntos, FondoMundo nuevoFondo, Button nuevaFlechaAnterior, Button nuevaFlechaSiguiente)
    {
        tarjetas = nuevasTarjetas;
        puntos = nuevosPuntos;
        fondo = nuevoFondo;
        flechaAnterior = nuevaFlechaAnterior;
        flechaSiguiente = nuevaFlechaSiguiente;
    }

    private void Awake()
    {
        desplazamiento = GetComponent<ScrollRect>();
        contenido = desplazamiento.content;
        ventana = desplazamiento.viewport;
    }

    private void Start()
    {
        // Al volver de la pantalla de niveles, se sigue en el mismo mundo.
        indiceObjetivo = Mathf.Clamp(GestorProgreso.MundoElegido - 1, 0, tarjetas.Length - 1);
        Distribuir();
        fondo.Mostrar(MundoCentral, false);
        ActualizarAspecto();
    }

    // Coloca cada tarjeta y ajusta el contenido al ancho actual de la pantalla.
    private void Distribuir()
    {
        float anchoVentana = ventana.rect.width;
        anchoVentanaAnterior = anchoVentana;

        // Sobra media pantalla a cada lado para que la primera y la última
        // tarjeta también puedan quedar en el centro.
        float anchoContenido = pasoEntreTarjetas * (tarjetas.Length - 1) + anchoVentana;
        contenido.sizeDelta = new Vector2(anchoContenido, 0f);

        for (int indice = 0; indice < tarjetas.Length; indice++)
        {
            float posicionX = anchoVentana / 2f + indice * pasoEntreTarjetas;
            tarjetas[indice].anchoredPosition = new Vector2(posicionX, tarjetas[indice].anchoredPosition.y);
        }

        contenido.anchoredPosition = new Vector2(PosicionDeTarjeta(indiceObjetivo), contenido.anchoredPosition.y);
    }

    private float PosicionDeTarjeta(int indice)
    {
        return -indice * pasoEntreTarjetas;
    }

    private void Update()
    {
        // La pantalla puede cambiar de tamaño, por ejemplo al poner pantalla completa.
        if (Mathf.Approximately(ventana.rect.width, anchoVentanaAnterior) == false)
        {
            Distribuir();
        }

        if (estaArrastrando == false)
        {
            float posicionX = Mathf.Lerp(contenido.anchoredPosition.x, PosicionDeTarjeta(indiceObjetivo), suavidad * Time.unscaledDeltaTime);
            contenido.anchoredPosition = new Vector2(posicionX, contenido.anchoredPosition.y);
        }
        else
        {
            // Mientras se arrastra, el fondo ya sigue a la tarjeta más cercana.
            indiceObjetivo = BuscarTarjetaMasCercana();
        }

        ActualizarAspecto();
    }

    private int BuscarTarjetaMasCercana()
    {
        int indice = Mathf.RoundToInt(-contenido.anchoredPosition.x / pasoEntreTarjetas);
        return Mathf.Clamp(indice, 0, tarjetas.Length - 1);
    }

    private void ActualizarAspecto()
    {
        // Las tarjetas se achican según qué tan lejos están del centro.
        for (int indice = 0; indice < tarjetas.Length; indice++)
        {
            float distancia = Mathf.Abs(contenido.anchoredPosition.x - PosicionDeTarjeta(indice));
            float cercania = Mathf.Clamp01(distancia / pasoEntreTarjetas);
            float escala = Mathf.Lerp(1f, escalaTarjetaLejana, cercania);
            tarjetas[indice].localScale = new Vector3(escala, escala, 1f);
        }

        for (int indice = 0; indice < puntos.Length; indice++)
        {
            float transparencia = 0.35f;
            if (indice == indiceObjetivo)
            {
                transparencia = 1f;
            }
            puntos[indice].color = new Color(1f, 1f, 1f, transparencia);
        }

        flechaAnterior.gameObject.SetActive(indiceObjetivo > 0);
        flechaSiguiente.gameObject.SetActive(indiceObjetivo < tarjetas.Length - 1);
        fondo.Mostrar(MundoCentral, true);
    }

    public void MoverAnterior()
    {
        MoverA(indiceObjetivo - 1);
    }

    public void MoverSiguiente()
    {
        MoverA(indiceObjetivo + 1);
    }

    private void MoverA(int nuevoIndice)
    {
        indiceObjetivo = Mathf.Clamp(nuevoIndice, 0, tarjetas.Length - 1);
        GestorProgreso.MundoElegido = MundoCentral;

        // Si la flecha quedara seleccionada, Enter la presionaría además de abrir el mundo.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void AbrirMundoCentral()
    {
        BotonMundo botonCentral = tarjetas[indiceObjetivo].GetComponent<BotonMundo>();
        botonCentral.AbrirMundo();
    }

    public void OnBeginDrag(PointerEventData datos)
    {
        estaArrastrando = true;
        posicionAlEmpezarArrastre = contenido.anchoredPosition.x;
    }

    public void OnEndDrag(PointerEventData datos)
    {
        estaArrastrando = false;

        int indiceInicial = Mathf.Clamp(Mathf.RoundToInt(-posicionAlEmpezarArrastre / pasoEntreTarjetas), 0, tarjetas.Length - 1);
        int nuevoIndice = BuscarTarjetaMasCercana();

        // Un arrastre corto también cambia de tarjeta, para que no se sienta pesado.
        if (nuevoIndice == indiceInicial)
        {
            float recorrido = contenido.anchoredPosition.x - posicionAlEmpezarArrastre;
            if (recorrido < -pasoEntreTarjetas * parteParaCambiar)
            {
                nuevoIndice = indiceInicial + 1;
            }
            else if (recorrido > pasoEntreTarjetas * parteParaCambiar)
            {
                nuevoIndice = indiceInicial - 1;
            }
        }

        MoverA(nuevoIndice);
    }
}
