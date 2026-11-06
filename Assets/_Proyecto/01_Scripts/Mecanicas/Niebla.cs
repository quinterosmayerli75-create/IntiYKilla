// =====================================================================
// Niebla.cs
// Proyecto: Inti y Killa
// Descripción: Niebla de los Yungas que tapa el nivel, salvo alrededor de los
//              personajes: cada uno solo ve de cerca, así que conviene mirar
//              también lo que descubre el compañero.
//              Se dibuja con una sola imagen de poca resolución, que se pinta
//              de nuevo en cada cuadro. Es mucho más liviano para el navegador
//              que cientos de nubecitas superpuestas.
// Uso: un solo objeto en la escena, creado por el generador.
// =====================================================================

using UnityEngine;

public class Niebla : MonoBehaviour
{
    [Header("Zona que cubre")]
    [SerializeField] private Vector2 centro = new Vector2(0f, 4f);
    [SerializeField] private Vector2 tamano = new Vector2(38f, 22f);

    [Header("Visibilidad (en unidades del juego)")]
    // Hasta aquí se ve perfecto. Es más que un salto a media altura, para que
    // el jugador siempre alcance a ver dónde va a caer (regla de justicia).
    [SerializeField] private float radioClaro = 4f;
    // Desde aquí la niebla ya está en su punto más espeso.
    [SerializeField] private float radioEspeso = 7f;
    [SerializeField] private float opacidadMaxima = 0.96f;

    [Header("Aspecto")]
    [SerializeField] private Color32 colorNiebla = new Color32(226, 234, 230, 255);
    [SerializeField] private Material materialSinLuces;
    [SerializeField] private float pixelesPorUnidad = 2f;
    [SerializeField] private float velocidadOndulacion = 0.25f;

    private ControladorJugador[] jugadores;
    private Texture2D textura;
    private Color32[] pixeles;
    private int anchoTextura;
    private int altoTextura;

    public void Configurar(Material nuevoMaterial)
    {
        materialSinLuces = nuevoMaterial;
    }

    private void Awake()
    {
        jugadores = FindObjectsByType<ControladorJugador>();

        anchoTextura = Mathf.RoundToInt(tamano.x * pixelesPorUnidad);
        altoTextura = Mathf.RoundToInt(tamano.y * pixelesPorUnidad);
        pixeles = new Color32[anchoTextura * altoTextura];

        textura = new Texture2D(anchoTextura, altoTextura, TextureFormat.RGBA32, false);
        // El filtro suaviza los píxeles grandes: el borde de la niebla se ve difuso.
        textura.filterMode = FilterMode.Bilinear;
        textura.wrapMode = TextureWrapMode.Clamp;

        Rect rectangulo = new Rect(0f, 0f, anchoTextura, altoTextura);
        Vector2 pivote = new Vector2(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(textura, rectangulo, pivote, pixelesPorUnidad, 0, SpriteMeshType.FullRect);

        transform.position = new Vector3(centro.x, centro.y, 0f);
        SpriteRenderer dibujo = gameObject.AddComponent<SpriteRenderer>();
        dibujo.sprite = sprite;
        // Por encima del nivel y de los enemigos.
        dibujo.sortingOrder = 15;
        if (materialSinLuces != null)
        {
            dibujo.sharedMaterial = materialSinLuces;
        }
    }

    private void Update()
    {
        Vector2 esquina = centro - tamano / 2f;
        float tiempo = Time.time * velocidadOndulacion;

        for (int y = 0; y < altoTextura; y++)
        {
            float mundoY = esquina.y + (y + 0.5f) / pixelesPorUnidad;

            for (int x = 0; x < anchoTextura; x++)
            {
                float mundoX = esquina.x + (x + 0.5f) / pixelesPorUnidad;
                float distancia = DistanciaAlJugadorMasCercano(mundoX, mundoY);

                // La ondulación hace que el borde se mueva como niebla de verdad.
                float ondulacion = Mathf.PerlinNoise(mundoX * 0.3f + tiempo, mundoY * 0.3f - tiempo);
                float avance = (distancia + ondulacion * 1.5f - radioClaro) / (radioEspeso - radioClaro);
                float opacidad = Mathf.SmoothStep(0f, opacidadMaxima, Mathf.Clamp01(avance));

                Color32 color = colorNiebla;
                color.a = (byte)(opacidad * 255f);
                pixeles[y * anchoTextura + x] = color;
            }
        }

        textura.SetPixels32(pixeles);
        textura.Apply(false);
    }

    private float DistanciaAlJugadorMasCercano(float mundoX, float mundoY)
    {
        float menorDistancia = float.MaxValue;

        foreach (ControladorJugador jugador in jugadores)
        {
            Vector3 posicion = jugador.transform.position;
            float diferenciaX = posicion.x - mundoX;
            float diferenciaY = posicion.y - mundoY;
            float distancia = Mathf.Sqrt(diferenciaX * diferenciaX + diferenciaY * diferenciaY);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
            }
        }

        return menorDistancia;
    }

    private void OnDestroy()
    {
        // Lo creado por código no se borra solo al cambiar de escena.
        Destroy(textura);
    }
}
