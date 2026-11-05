// =====================================================================
// BotonNivel.cs
// Proyecto: Inti y Killa
// Descripción: Un botón de la pantalla de niveles. Sabe qué posición ocupa
//              dentro del mundo (1 a 5); el mundo lo lee de GestorProgreso.
//              Muestra el título del nivel y su estado:
//              - Estrellas ganadas (★★☆) si está desbloqueado.
//              - "Bloqueado" si todavía no se ganó el nivel anterior.
//              - "Pronto" si la escena todavía no existe en el proyecto.
// =====================================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BotonNivel : MonoBehaviour
{
    [SerializeField] private int numeroEnMundo = 1;
    [SerializeField] private Text textoEstado;
    [SerializeField] private Text textoTitulo;

    private Button boton;

    public void Configurar(int nuevoNumeroEnMundo, Text nuevoTextoEstado, Text nuevoTextoTitulo)
    {
        numeroEnMundo = nuevoNumeroEnMundo;
        textoEstado = nuevoTextoEstado;
        textoTitulo = nuevoTextoTitulo;
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
        if (boton == null)
        {
            boton = GetComponent<Button>();
        }

        int numeroMundo = GestorProgreso.MundoElegido;
        DatosNivel datos = DatosJuego.ObtenerNivel(numeroMundo, numeroEnMundo);
        bool escenaExiste = datos != null && Application.CanStreamedLevelBeLoaded(datos.nombreEscena);
        bool estaDesbloqueado = GestorProgreso.NivelEstaDesbloqueado(numeroMundo, numeroEnMundo);

        if (textoTitulo != null && datos != null)
        {
            textoTitulo.text = datos.titulo;
        }

        if (escenaExiste == false)
        {
            boton.interactable = false;
            CambiarTexto("Pronto");
        }
        else if (estaDesbloqueado == false)
        {
            boton.interactable = false;
            CambiarTexto("Bloqueado");
        }
        else
        {
            boton.interactable = true;
            int estrellas = GestorProgreso.ObtenerEstrellas(numeroMundo, numeroEnMundo);
            CambiarTexto(ConvertirEstrellasATexto(estrellas));
        }
    }

    private void CambiarTexto(string nuevoTexto)
    {
        if (textoEstado != null)
        {
            textoEstado.text = nuevoTexto;
        }
    }

    private string ConvertirEstrellasATexto(int cantidad)
    {
        string texto = "";
        for (int indice = 1; indice <= DatosJuego.estrellasPorNivel; indice++)
        {
            if (indice <= cantidad)
            {
                texto = texto + "★";
            }
            else
            {
                texto = texto + "☆";
            }
        }
        return texto;
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
