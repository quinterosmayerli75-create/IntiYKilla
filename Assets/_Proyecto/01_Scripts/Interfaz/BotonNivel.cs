// =====================================================================
// BotonNivel.cs
// Proyecto: Inti y Killa
// Descripción: Un cuadro de la pantalla de niveles. Sabe qué posición ocupa
//              dentro del mundo (1 a 5); el mundo lo lee de GestorProgreso.
//              Muestra el título del nivel y su estado:
//              - Número y estrellas ganadas si está disponible.
//              - Un candado si todavía no se ganó el nivel anterior.
//              - "Pronto" si la escena todavía no existe en el proyecto.
// =====================================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum EstadoNivel
{
    Disponible,
    Bloqueado,
    Pronto
}

[RequireComponent(typeof(Button))]
public class BotonNivel : MonoBehaviour
{
    [SerializeField] private int numeroEnMundo = 1;
    [SerializeField] private Image imagenCuadro;
    [SerializeField] private Text textoNumero;
    [SerializeField] private Text textoTitulo;
    [SerializeField] private Text textoPronto;
    [SerializeField] private GameObject candado;
    [SerializeField] private Image[] estrellas;

    private static readonly Color colorCuadroApagado = new Color(0.34f, 0.34f, 0.40f);
    private static readonly Color colorEstrellaGanada = new Color(1f, 0.82f, 0.20f);
    private static readonly Color colorEstrellaVacia = new Color(0f, 0f, 0f, 0.35f);

    private Button boton;

    public void Configurar(int nuevoNumeroEnMundo, Image nuevaImagenCuadro, Text nuevoTextoNumero, Text nuevoTextoTitulo, Text nuevoTextoPronto, GameObject nuevoCandado, Image[] nuevasEstrellas)
    {
        numeroEnMundo = nuevoNumeroEnMundo;
        imagenCuadro = nuevaImagenCuadro;
        textoNumero = nuevoTextoNumero;
        textoTitulo = nuevoTextoTitulo;
        textoPronto = nuevoTextoPronto;
        candado = nuevoCandado;
        estrellas = nuevasEstrellas;
    }

    private void Awake()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(CargarNivel);
    }

    private void Start()
    {
        ActualizarEstado();
    }

    public void ActualizarEstado()
    {
        int numeroMundo = GestorProgreso.MundoElegido;
        DatosNivel datos = DatosJuego.ObtenerNivel(numeroMundo, numeroEnMundo);
        bool escenaExiste = datos != null && Application.CanStreamedLevelBeLoaded(datos.nombreEscena);

        EstadoNivel estado = EstadoNivel.Disponible;
        if (escenaExiste == false)
        {
            estado = EstadoNivel.Pronto;
        }
        else if (GestorProgreso.NivelEstaDesbloqueado(numeroMundo, numeroEnMundo) == false)
        {
            estado = EstadoNivel.Bloqueado;
        }

        AplicarApariencia(estado, GestorProgreso.ObtenerEstrellas(numeroMundo, numeroEnMundo));
    }

    public void AplicarApariencia(EstadoNivel estado, int estrellasGanadas)
    {
        int numeroMundo = GestorProgreso.MundoElegido;
        DatosNivel datos = DatosJuego.ObtenerNivel(numeroMundo, numeroEnMundo);
        if (datos != null)
        {
            textoTitulo.text = datos.titulo;
        }

        bool estaDisponible = estado == EstadoNivel.Disponible;
        boton.interactable = estaDisponible;
        textoNumero.gameObject.SetActive(estaDisponible);
        textoPronto.gameObject.SetActive(estado == EstadoNivel.Pronto);
        candado.SetActive(estado == EstadoNivel.Bloqueado);

        if (estaDisponible == true)
        {
            imagenCuadro.color = ColoresMundos.ObtenerColor(numeroMundo);
        }
        else
        {
            imagenCuadro.color = colorCuadroApagado;
        }

        for (int indice = 0; indice < estrellas.Length; indice++)
        {
            // Las estrellas solo tienen sentido en un nivel que se puede jugar.
            estrellas[indice].gameObject.SetActive(estaDisponible);
            if (indice < estrellasGanadas)
            {
                estrellas[indice].color = colorEstrellaGanada;
            }
            else
            {
                estrellas[indice].color = colorEstrellaVacia;
            }
        }
    }

    private void CargarNivel()
    {
        DatosNivel datos = DatosJuego.ObtenerNivel(GestorProgreso.MundoElegido, numeroEnMundo);
        if (datos != null)
        {
            SceneManager.LoadScene(datos.nombreEscena);
        }
    }
}
