// =====================================================================
// PisoQueSeCalienta.cs
// Proyecto: Inti y Killa
// Descripción: Un tramo de piso que parece normal. Cuando alguien lo pisa,
//              parpadea un momento (aviso) y se convierte en lava, agua o
//              veneno durante unos segundos. Después vuelve a ser piso y se
//              puede disparar otra vez.
//              Con lava o agua solo daña al elemento contrario: a veces quien
//              lo pisa no sufre nada, pero su compañero sí.
// Uso: objeto vacío a la altura del piso, con un hijo SpriteRenderer que es
//      la capa que cambia de color (la crea el generador).
// =====================================================================

using System.Collections;
using UnityEngine;

public class PisoQueSeCalienta : TrampaActivable
{
    [Header("Zona")]
    [SerializeField] private TipoZona zonaEnQueSeConvierte = TipoZona.Lava;
    [SerializeField] private float ancho = 2f;
    [SerializeField] private SpriteRenderer capaDeColor;

    [Header("Tiempos")]
    [SerializeField] private float segundosDeAviso = 0.6f;
    [SerializeField] private float segundosEncendido = 2.5f;
    [SerializeField] private float parpadeosPorSegundo = 8f;

    [SerializeField] private string mensajeDeMuerte = "El piso se calentó bajo los pies de {0}.";

    private bool estaEncendido = false;
    private Color colorDeLaZona;

    public void Configurar(TipoZona nuevaZona, float nuevoAncho, SpriteRenderer nuevaCapa, Color nuevoColor)
    {
        zonaEnQueSeConvierte = nuevaZona;
        ancho = nuevoAncho;
        capaDeColor = nuevaCapa;
        colorDeLaZona = nuevoColor;
    }

    // El mensaje cambia según en qué se convierte el piso (lo pone el generador).
    public void ConfigurarMensaje(string nuevoMensaje)
    {
        mensajeDeMuerte = nuevoMensaje;
    }

    protected override void Awake()
    {
        base.Awake();

        // La capa empieza invisible: el piso se ve igual que todos los demás.
        if (capaDeColor != null)
        {
            colorDeLaZona = capaDeColor.color;
            PintarCapa(0f);
        }
    }

    private void Update()
    {
        // Zona finita justo encima del piso: solo cuenta quien está parado ahí.
        Vector2 centroZona = new Vector2(transform.position.x, transform.position.y + 0.15f);
        Vector2 tamanoZona = new Vector2(ancho, 0.3f);

        if (estaEncendido == true)
        {
            DaniarAQuienEsteEncima(centroZona, tamanoZona);
            return;
        }

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
        StartCoroutine(AvisarEncenderYApagar());
    }

    private IEnumerator AvisarEncenderYApagar()
    {
        // Aviso: la capa parpadea; quien sigue caminando alcanza a salir.
        float tiempo = 0f;
        while (tiempo < segundosDeAviso)
        {
            tiempo = tiempo + Time.deltaTime;
            float onda = Mathf.Sin(tiempo * parpadeosPorSegundo * Mathf.PI * 2f);
            if (onda > 0f)
            {
                PintarCapa(0.6f);
            }
            else
            {
                PintarCapa(0.1f);
            }
            yield return null;
        }

        PintarCapa(1f);
        estaEncendido = true;
        yield return new WaitForSeconds(segundosEncendido);

        estaEncendido = false;
        PintarCapa(0f);
        Rearmar();
    }

    private void DaniarAQuienEsteEncima(Vector2 centroZona, Vector2 tamanoZona)
    {
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador == null || jugador.EstaVivo == false)
            {
                continue;
            }

            if (JugadorTocaRectangulo(jugador, centroZona, tamanoZona) == true && EsPeligrosoPara(jugador.Elemento) == true)
            {
                jugador.Morir(mensajeDeMuerte.Replace("{0}", jugador.NombreJugador));
            }
        }
    }

    // Las mismas reglas que un charco normal (ver ZonaElemental).
    private bool EsPeligrosoPara(TipoElemento elementoJugador)
    {
        if (zonaEnQueSeConvierte == TipoZona.Veneno || zonaEnQueSeConvierte == TipoZona.CristalesSal)
        {
            return true;
        }

        if (CicloEclipse.EstaEnGracia == true)
        {
            return false;
        }

        if (zonaEnQueSeConvierte == TipoZona.Lava && elementoJugador == TipoElemento.Agua)
        {
            return true;
        }

        if (zonaEnQueSeConvierte == TipoZona.Agua && elementoJugador == TipoElemento.Fuego)
        {
            return true;
        }

        return false;
    }

    private void PintarCapa(float opacidad)
    {
        if (capaDeColor == null)
        {
            return;
        }

        capaDeColor.color = new Color(colorDeLaZona.r, colorDeLaZona.g, colorDeLaZona.b, opacidad);
    }
}
