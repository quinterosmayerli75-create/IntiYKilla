// =====================================================================
// BotonNivel.cs
// Proyecto: Inti y Killa
// Descripción: Un botón del mapa de niveles. Sabe qué nivel abre y
//              muestra su estado:
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
    [SerializeField] private int numeroNivel = 1;
    [SerializeField] private Text textoEstado;

    private Button boton;

    public void Configurar(int nuevoNumero, Text nuevoTextoEstado)
    {
        numeroNivel = nuevoNumero;
        textoEstado = nuevoTextoEstado;
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

        DatosNivel datos = DatosJuego.ObtenerNivel(numeroNivel);
        bool escenaExiste = datos != null && Application.CanStreamedLevelBeLoaded(datos.nombreEscena);
        bool estaDesbloqueado = GestorProgreso.EstaDesbloqueado(numeroNivel);

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
            int estrellas = GestorProgreso.ObtenerEstrellas(numeroNivel);
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
        for (int indice = 1; indice <= 3; indice++)
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
        DatosNivel datos = DatosJuego.ObtenerNivel(numeroNivel);
        if (datos != null)
        {
            SceneManager.LoadScene(datos.nombreEscena);
        }
    }
}
