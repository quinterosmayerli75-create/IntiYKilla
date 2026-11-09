// =====================================================================
// AnimacionLiquido.cs
// Proyecto: Inti y Killa
// Descripción: Hace que la lava, el agua y el veneno se vean vivos, como un
//              río que corre:
//              - Ondas claras que avanzan por la superficie y suben y bajan.
//              - Una franja de brillo arriba y una sombra abajo (profundidad).
//              - Burbujas que suben y revientan (lava y veneno) o destellos
//                de luz que parpadean (agua).
//              Solo es visual: no cambia quién muere ni cómo. Si el agua se
//              congela (AguaCongelable), las ondas se esconden.
// Uso: se pone en el mismo objeto que el charco (lo hace el generador).
//      Las piezas se crean al empezar el nivel, fuera del charco, porque el
//      charco está estirado y deformaría los círculos.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimacionLiquido : MonoBehaviour
{
    [Header("Tipo y dibujos")]
    [SerializeField] private TipoZona tipoLiquido = TipoZona.Lava;
    [SerializeField] private Sprite spriteCirculo;
    [SerializeField] private Sprite spriteCuadrado;
    // Sin luces: la lava brilla aunque el nivel esté a oscuras.
    [SerializeField] private Material materialSinLuces;

    [Header("Ondas")]
    [SerializeField] private float separacionOndas = 0.55f;
    [SerializeField] private float velocidadFlujo = 0.9f;
    [SerializeField] private float alturaOnda = 0.06f;
    [SerializeField] private float velocidadOleaje = 3f;

    [Header("Burbujas y destellos")]
    [SerializeField] private int cantidadBurbujas = 5;
    [SerializeField] private float segundosEntreBurbujas = 0.45f;
    [SerializeField] private float segundosDeVidaBurbuja = 0.9f;
    [SerializeField] private float alturaBurbuja = 0.45f;

    private SpriteRenderer dibujoLiquido;
    private AguaCongelable aguaCongelable;
    private Transform raiz;

    private List<Transform> ondas = new List<Transform>();
    private List<SpriteRenderer> dibujosOndas = new List<SpriteRenderer>();
    private SpriteRenderer franjaBrillo;
    private SpriteRenderer franjaSombra;

    private List<Transform> burbujas = new List<Transform>();
    private List<SpriteRenderer> dibujosBurbujas = new List<SpriteRenderer>();
    private List<float> edadesBurbujas = new List<float>();
    private List<float> origenesXBurbujas = new List<float>();
    private float tiempoParaLaSiguienteBurbuja = 0f;

    private Color colorOnda;
    private Color colorBurbuja;

    public void Configurar(TipoZona nuevoTipo, Sprite nuevoCirculo, Sprite nuevoCuadrado, Material nuevoMaterial)
    {
        tipoLiquido = nuevoTipo;
        spriteCirculo = nuevoCirculo;
        spriteCuadrado = nuevoCuadrado;
        materialSinLuces = nuevoMaterial;
    }

    private void Start()
    {
        dibujoLiquido = GetComponent<SpriteRenderer>();
        aguaCongelable = GetComponent<AguaCongelable>();

        // Los colores salen del propio líquido: así sirve igual para la lava, el
        // agua, el veneno o el río turbio de la crecida.
        Color colorBase = dibujoLiquido.color;
        colorOnda = Color.Lerp(colorBase, Color.white, 0.45f);
        colorOnda.a = 0.9f;
        colorBurbuja = Color.Lerp(colorBase, Color.white, 0.6f);
        colorBurbuja.a = 0.9f;

        // La raíz no es hija del charco (está estirado), pero sí de su mismo
        // padre, para que todo se borre junto al reiniciar el nivel.
        GameObject objetoRaiz = new GameObject("Animacion de " + name);
        raiz = objetoRaiz.transform;
        raiz.SetParent(transform.parent);

        CrearFranjas(colorBase);
        CrearOndas();
        CrearBurbujas();
    }

    private void CrearFranjas(Color colorBase)
    {
        Color colorBrillo = Color.Lerp(colorBase, Color.white, 0.6f);
        colorBrillo.a = 0.55f;
        franjaBrillo = CrearPieza("Brillo", spriteCuadrado, colorBrillo, 1);

        Color colorSombra = Color.Lerp(colorBase, Color.black, 0.45f);
        colorSombra.a = 0.45f;
        franjaSombra = CrearPieza("Sombra", spriteCuadrado, colorSombra, 1);
    }

    private void CrearOndas()
    {
        float ancho = dibujoLiquido.bounds.size.x;
        int cantidadOndas = Mathf.CeilToInt(ancho / separacionOndas) + 1;
        for (int indice = 0; indice < cantidadOndas; indice++)
        {
            SpriteRenderer onda = CrearPieza("Onda " + (indice + 1), spriteCirculo, colorOnda, 2);
            onda.transform.localScale = new Vector3(separacionOndas * 0.8f, 0.14f, 1f);
            ondas.Add(onda.transform);
            dibujosOndas.Add(onda);
        }
    }

    private void CrearBurbujas()
    {
        for (int indice = 0; indice < cantidadBurbujas; indice++)
        {
            SpriteRenderer burbuja = CrearPieza("Burbuja " + (indice + 1), spriteCirculo, colorBurbuja, 3);
            burbuja.enabled = false;
            burbujas.Add(burbuja.transform);
            dibujosBurbujas.Add(burbuja);
            edadesBurbujas.Add(-1f);
            origenesXBurbujas.Add(0f);
        }
    }

    private SpriteRenderer CrearPieza(string nombre, Sprite sprite, Color color, int ordenSobreElLiquido)
    {
        GameObject pieza = new GameObject(nombre);
        pieza.transform.SetParent(raiz);

        SpriteRenderer dibujo = pieza.AddComponent<SpriteRenderer>();
        dibujo.sprite = sprite;
        dibujo.color = color;
        dibujo.sortingOrder = dibujoLiquido.sortingOrder + ordenSobreElLiquido;

        if (tipoLiquido == TipoZona.Lava && materialSinLuces != null)
        {
            dibujo.sharedMaterial = materialSinLuces;
        }
        return dibujo;
    }

    private void Update()
    {
        if (raiz == null)
        {
            return;
        }

        // Se leen los límites cada vez: la crecida del río sube y se estira.
        Bounds limites = dibujoLiquido.bounds;
        bool estaCongelada = aguaCongelable != null && aguaCongelable.EstaCongelada == true;
        bool seVe = dibujoLiquido.enabled == true && estaCongelada == false;

        ActualizarFranjas(limites, seVe);
        ActualizarOndas(limites, seVe);
        ActualizarBurbujas(limites, seVe);
    }

    private void ActualizarFranjas(Bounds limites, bool seVe)
    {
        float altoBrillo = Mathf.Min(0.07f, limites.size.y * 0.3f);
        franjaBrillo.enabled = seVe;
        franjaBrillo.transform.position = new Vector3(limites.center.x, limites.max.y - altoBrillo / 2f, 0f);
        franjaBrillo.transform.localScale = new Vector3(limites.size.x, altoBrillo, 1f);

        // El brillo late suavemente, como la luz sobre el agua.
        Color colorBrillo = franjaBrillo.color;
        colorBrillo.a = 0.35f + Mathf.Sin(Time.time * 2f) * 0.15f;
        franjaBrillo.color = colorBrillo;

        float altoSombra = limites.size.y * 0.45f;
        franjaSombra.enabled = seVe;
        franjaSombra.transform.position = new Vector3(limites.center.x, limites.min.y + altoSombra / 2f, 0f);
        franjaSombra.transform.localScale = new Vector3(limites.size.x, altoSombra, 1f);
    }

    // Las ondas avanzan hacia la derecha y vuelven a empezar por la izquierda:
    // así parece que el líquido corre como un río.
    private void ActualizarOndas(Bounds limites, bool seVe)
    {
        float desplazamiento = Mathf.Repeat(Time.time * velocidadFlujo, separacionOndas);
        float margen = separacionOndas * 0.4f;

        for (int indice = 0; indice < ondas.Count; indice++)
        {
            float posicionX = limites.min.x + indice * separacionOndas + desplazamiento;
            bool estaDentro = posicionX >= limites.min.x + margen && posicionX <= limites.max.x - margen;

            dibujosOndas[indice].enabled = seVe == true && estaDentro == true;
            float oleaje = Mathf.Sin(Time.time * velocidadOleaje + indice * 1.3f) * alturaOnda;
            ondas[indice].position = new Vector3(posicionX, limites.max.y + oleaje, 0f);
        }
    }

    private void ActualizarBurbujas(Bounds limites, bool seVe)
    {
        tiempoParaLaSiguienteBurbuja = tiempoParaLaSiguienteBurbuja - Time.deltaTime;
        if (tiempoParaLaSiguienteBurbuja <= 0f && seVe == true)
        {
            SoltarBurbuja(limites);
            tiempoParaLaSiguienteBurbuja = segundosEntreBurbujas * Random.Range(0.6f, 1.4f);
        }

        for (int indice = 0; indice < burbujas.Count; indice++)
        {
            if (edadesBurbujas[indice] < 0f)
            {
                continue;
            }

            edadesBurbujas[indice] = edadesBurbujas[indice] + Time.deltaTime;
            float avance = edadesBurbujas[indice] / segundosDeVidaBurbuja;
            if (avance >= 1f || seVe == false)
            {
                edadesBurbujas[indice] = -1f;
                dibujosBurbujas[indice].enabled = false;
                continue;
            }

            if (tipoLiquido == TipoZona.Agua)
            {
                AnimarDestello(indice, avance, limites);
            }
            else
            {
                AnimarBurbuja(indice, avance, limites);
            }
        }
    }

    private void SoltarBurbuja(Bounds limites)
    {
        for (int indice = 0; indice < burbujas.Count; indice++)
        {
            if (edadesBurbujas[indice] < 0f)
            {
                edadesBurbujas[indice] = 0f;
                origenesXBurbujas[indice] = Random.Range(limites.min.x + 0.2f, limites.max.x - 0.2f);
                dibujosBurbujas[indice].enabled = true;
                return;
            }
        }
    }

    // Lava y veneno: la burbuja sube un poco, crece y revienta.
    private void AnimarBurbuja(int indice, float avance, Bounds limites)
    {
        float tamano = Mathf.Lerp(0.08f, 0.2f, avance);
        float altura = limites.max.y - 0.05f + avance * alturaBurbuja;
        burbujas[indice].position = new Vector3(origenesXBurbujas[indice], altura, 0f);
        burbujas[indice].localScale = new Vector3(tamano, tamano, 1f);

        Color color = colorBurbuja;
        color.a = 0.9f * (1f - avance);
        dibujosBurbujas[indice].color = color;
    }

    // Agua: un destello de luz sobre la superficie que aparece y se apaga.
    private void AnimarDestello(int indice, float avance, Bounds limites)
    {
        float tamano = 0.1f;
        burbujas[indice].position = new Vector3(origenesXBurbujas[indice], limites.max.y - 0.04f, 0f);
        burbujas[indice].localScale = new Vector3(tamano * 1.8f, tamano * 0.6f, 1f);

        Color color = Color.white;
        color.a = Mathf.Sin(avance * Mathf.PI) * 0.85f;
        dibujosBurbujas[indice].color = color;
    }
}
