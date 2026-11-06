// =====================================================================
// FondoMundo.cs
// Proyecto: Inti y Killa
// Descripción: Fondo de pantalla de los menús, que muestra el paisaje del
//              mundo elegido. Al cambiar de mundo, el paisaje nuevo aparece
//              poco a poco encima del anterior para que el cambio no sea brusco.
// Uso: dos imágenes a pantalla completa; "imagenActual" va encima.
// =====================================================================

using UnityEngine;
using UnityEngine.UI;

public class FondoMundo : MonoBehaviour
{
    [SerializeField] private Image imagenAnterior;
    [SerializeField] private Image imagenActual;
    // Índice 0 = Mundo 1.
    [SerializeField] private Sprite[] fondos;
    [SerializeField] private float segundosDelFundido = 0.35f;

    private int mundoMostrado = 0;
    private float tiempoDelFundido = 0f;
    private bool estaFundiendo = false;

    public void Configurar(Image nuevaImagenAnterior, Image nuevaImagenActual, Sprite[] nuevosFondos)
    {
        imagenAnterior = nuevaImagenAnterior;
        imagenActual = nuevaImagenActual;
        fondos = nuevosFondos;
    }

    public void Mostrar(int numeroMundo, bool conFundido)
    {
        int indice = numeroMundo - 1;
        bool mundoValido = indice >= 0 && indice < fondos.Length;
        if (mundoValido == false || numeroMundo == mundoMostrado)
        {
            return;
        }

        // Lo que estaba encima pasa abajo, y el paisaje nuevo aparece sobre él.
        imagenAnterior.sprite = imagenActual.sprite;
        imagenAnterior.color = Color.white;
        imagenActual.sprite = fondos[indice];
        mundoMostrado = numeroMundo;

        if (conFundido == true)
        {
            tiempoDelFundido = 0f;
            estaFundiendo = true;
            imagenActual.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            estaFundiendo = false;
            imagenActual.color = Color.white;
        }
    }

    private void Update()
    {
        if (estaFundiendo == false)
        {
            return;
        }

        tiempoDelFundido = tiempoDelFundido + Time.unscaledDeltaTime;
        float progreso = Mathf.Clamp01(tiempoDelFundido / segundosDelFundido);
        imagenActual.color = new Color(1f, 1f, 1f, progreso);

        if (progreso >= 1f)
        {
            estaFundiendo = false;
        }
    }
}
