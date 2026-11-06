// =====================================================================
// EscalaAlPasarMouse.cs
// Proyecto: Inti y Killa
// Descripción: Agranda un poco un botón cuando el mouse está encima, para
//              que el jugador note que se puede presionar. Un botón
//              desactivado no reacciona, así tampoco parece disponible.
// Uso: en cualquier objeto con Button.
// =====================================================================

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EscalaAlPasarMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float escalaConMouse = 1.07f;
    [SerializeField] private float velocidad = 12f;

    private Button boton;
    private bool mouseEncima = false;

    private void Awake()
    {
        boton = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData datos)
    {
        mouseEncima = true;
    }

    public void OnPointerExit(PointerEventData datos)
    {
        mouseEncima = false;
    }

    private void Update()
    {
        float escalaBuscada = 1f;
        if (mouseEncima == true && boton.interactable == true)
        {
            escalaBuscada = escalaConMouse;
        }

        // Se usa el tiempo real para que funcione aunque el juego esté en pausa.
        float escala = Mathf.Lerp(transform.localScale.x, escalaBuscada, velocidad * Time.unscaledDeltaTime);
        transform.localScale = new Vector3(escala, escala, 1f);
    }
}
