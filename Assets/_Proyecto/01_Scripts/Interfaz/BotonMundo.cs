// =====================================================================
// BotonMundo.cs
// Proyecto: Inti y Killa
// Descripción: Una tarjeta de la pantalla de mundos. Sabe qué mundo abre y
//              muestra su estado:
//              - Estrellas ganadas del total (9/15) si está desbloqueado.
//              - Un candado y qué falta ganar si todavía está bloqueado.
// =====================================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BotonMundo : MonoBehaviour
{
    [SerializeField] private int numeroMundo = 1;
    [SerializeField] private Text textoEstrellas;
    [SerializeField] private GameObject grupoEstrellas;
    [SerializeField] private GameObject grupoBloqueado;
    [SerializeField] private Text textoBloqueado;

    private Button boton;

    public void Configurar(int nuevoNumeroMundo, Text nuevoTextoEstrellas, GameObject nuevoGrupoEstrellas, GameObject nuevoGrupoBloqueado, Text nuevoTextoBloqueado)
    {
        numeroMundo = nuevoNumeroMundo;
        textoEstrellas = nuevoTextoEstrellas;
        grupoEstrellas = nuevoGrupoEstrellas;
        grupoBloqueado = nuevoGrupoBloqueado;
        textoBloqueado = nuevoTextoBloqueado;
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
        AplicarApariencia(GestorProgreso.MundoEstaDesbloqueado(numeroMundo));
    }

    public void AplicarApariencia(bool estaDesbloqueado)
    {
        boton.interactable = estaDesbloqueado;
        grupoEstrellas.SetActive(estaDesbloqueado);
        grupoBloqueado.SetActive(estaDesbloqueado == false);
        textoBloqueado.gameObject.SetActive(estaDesbloqueado == false);

        int estrellas = GestorProgreso.ObtenerEstrellasDelMundo(numeroMundo);
        int estrellasMaximas = GestorProgreso.ObtenerEstrellasMaximasPorMundo();
        textoEstrellas.text = estrellas + "/" + estrellasMaximas;
        textoBloqueado.text = "Gana el Mundo " + (numeroMundo - 1);
    }

    public void AbrirMundo()
    {
        if (boton.interactable == false)
        {
            return;
        }

        // La pantalla de niveles lee este dato para saber qué mundo mostrar.
        GestorProgreso.MundoElegido = numeroMundo;
        SceneManager.LoadScene(DatosJuego.escenaSeleccionNivel);
    }
}
