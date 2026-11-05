// =====================================================================
// BotonMundo.cs
// Proyecto: Inti y Killa
// Descripción: Un botón de la pantalla de mundos. Sabe qué mundo abre y
//              muestra su estado:
//              - Estrellas ganadas del total (★ 9/15) si está desbloqueado.
//              - "Bloqueado" si todavía no se ganó el nivel 5 del anterior.
// =====================================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BotonMundo : MonoBehaviour
{
    [SerializeField] private int numeroMundo = 1;
    [SerializeField] private Text textoEstado;

    private Button boton;

    public void Configurar(int nuevoNumeroMundo, Text nuevoTextoEstado)
    {
        numeroMundo = nuevoNumeroMundo;
        textoEstado = nuevoTextoEstado;
    }

    private void Awake()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(AbrirMundo);
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

        bool estaDesbloqueado = GestorProgreso.MundoEstaDesbloqueado(numeroMundo);
        boton.interactable = estaDesbloqueado;

        if (textoEstado == null)
        {
            return;
        }

        if (estaDesbloqueado == true)
        {
            int estrellas = GestorProgreso.ObtenerEstrellasDelMundo(numeroMundo);
            int estrellasMaximas = GestorProgreso.ObtenerEstrellasMaximasPorMundo();
            textoEstado.text = "★ " + estrellas + "/" + estrellasMaximas;
        }
        else
        {
            textoEstado.text = "Bloqueado";
        }
    }

    private void AbrirMundo()
    {
        // La pantalla de niveles lee este dato para saber qué mundo mostrar.
        GestorProgreso.MundoElegido = numeroMundo;
        SceneManager.LoadScene(DatosJuego.escenaSeleccionNivel);
    }
}
