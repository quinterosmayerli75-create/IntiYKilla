# GDD – Inti y Killa

## 1. Ficha general
- **Género:** plataformas y puzzles cooperativos 2D
- **Jugadores:** 2, local, mismo teclado
- **Motor:** Unity 6 LTS, plantilla Universal 2D (URP), C#
- **Plataforma:** PC (Windows) y WebGL
- **Publicación:** itch.io (gratis)
- **Duración:** 20 niveles de 1 a 3 minutos
- **Plazo:** 28 oct – 27 nov 2026 (proyecto final individual)
- **Referencia:** juegos cooperativos de elementos tipo "agua y fuego" (solo mecánica; personajes, arte e historia son originales)

## 2. Concepto y plus
Inti (Sol) y Killa (Luna) cooperan para recorrer Bolivia y llegar juntos a la salida. Cada uno sobrevive en lo que destruye al otro.

Plus diferenciadores:
- **Eclipse:** altares donde, si los dos se paran a la vez, intercambian elementos.
- **Escenarios reales de Bolivia:** 6 mundos.
- **Mundo musical de Aiquile:** cuerdas de charango trampolín y puertas melódicas.
- **Día y noche:** plataformas que aparecen o desaparecen.
- **Estrellas por tiempo:** 1 a 3 estrellas por nivel.
- **Niveles finales muy difíciles** (precisión y coordinación).

## 3. Personajes
| Personaje | Jugador | Pisa | Muere con | Habilidad (S / Flecha abajo) |
|---|---|---|---|---|
| Inti (Sol) | 1 | Fuego, lava | Agua | Ilumina zonas oscuras, derrite hielo |
| Killa (Luna) | 2 | Agua, hielo | Fuego | Congela agua (plataforma temporal) |

- Ambos mueren con veneno verde y al caer al vacío.
- Si uno muere, el nivel se reinicia para los dos.
- Historia: un eclipse rompió el equilibrio entre día y noche; deben reunir las gemas de luz desde Uyuni hasta la Isla del Sol.

## 4. Controles
| Acción | Inti | Killa |
|---|---|---|
| Moverse | A / D | ← / → |
| Saltar | W | ↑ |
| Habilidad / interactuar | S | ↓ |
| Pausa | Esc | Esc |

## 5. Mecánicas (componentes reutilizables)
| Mecánica | Desde nivel |
|---|---|
| Movimiento y salto (coyote time) | 1 |
| Charcos: lava (solo Inti), agua (solo Killa), veneno (ninguno) | 1 |
| Gemas naranjas (Inti) y azules (Killa) | 1 |
| Puertas de salida; ganan cuando ambos están en la suya | 1 |
| Botones y palancas (algunos requieren a los dos) | 2 |
| Altar de eclipse (intercambio de elementos) | 3 |
| Oscuridad con luz 2D; Inti ilumina | 4 |
| Vagonetas y plataformas móviles | 5 |
| Cajas empujables entre los dos | 6 |
| Niebla | 7 |
| Lluvia que apaga a Inti | 8 |
| Día y noche | 9 |
| Lianas y hojas trampolín | 10 |
| Monos que lanzan frutas | 11 |
| Cuerdas de charango (trampolín + nota) | 14 |
| Puerta melódica (notas en orden) | 15 |
| Eclipse automático cada pocos segundos | 18 |

Estrellas: 3 = bajo tiempo objetivo + todas las gemas; 2 = una de las dos; 1 = terminar.

## 6. Mundos y niveles
| Mundo | Niveles | Mecánica nueva |
|---|---|---|
| 1. Salar de Uyuni (tutorial) | 1–3 | Movimiento, botones, eclipse |
| 2. Minas de Potosí | 4–6 | Luz, vagonetas, cajas |
| 3. Yungas | 7–9 | Niebla, lluvia, día y noche |
| 4. Trópico (Chapare) | 10–13 | Lianas, hojas trampolín, monos |
| 5. Aiquile, tierra del charango | 14–17 | Cuerdas trampolín, puerta melódica |
| 6. Isla del Sol | 18–20 | Eclipse automático, todo mezclado |

| Nivel | Objetivo | Dif. |
|---|---|---|
| 1 | Moverse, saltar, gemas | 1 |
| 2 | Botones y puertas de dos | 1 |
| 3 | Primer eclipse | 2 |
| 4 | Inti ilumina para Killa | 2 |
| 5 | Vagonetas y plataformas | 2 |
| 6 | Cajas entre los dos | 3 |
| 7 | Caminos angostos con niebla | 2 |
| 8 | Lluvia y refugios | 3 |
| 9 | Día y noche | 3 |
| 10 | Ríos y lianas | 3 |
| 11 | Hojas trampolín y monos | 3 |
| 12 | Eclipse en la selva | 3 |
| 13 | Persecución por el río | 4 |
| 14 | Cuerdas de charango | 3 |
| 15 | Primera puerta melódica | 3 |
| 16 | Melodía coordinada | 4 |
| 17 | Melodía + día y noche | 4 |
| 18 | Eclipse automático | 4 |
| 19 | Todo mezclado | 5 |
| 20 | Gran final | 5 |

Reglas: cada nivel dura 1–3 min, introduce una sola idea nueva, y la dificultad final viene de la precisión, no de la longitud.

## 7. Escenas e interfaz
Escenas: `Intro` → `MenuPrincipal` → `SeleccionNivel` → `Nivel01`…`Nivel20` → `Final`.

Ventanas encima del nivel (no escenas): HUD (gemas, cronómetro), Pausa (continuar, reiniciar, opciones, salir al mapa), Opciones (música, efectos, idioma, pantalla completa), Victoria (estrellas, tiempo, gemas), Derrota (reinicio automático).

Guardado: PlayerPrefs (niveles desbloqueados, estrellas, opciones).

## 8. Arte y sonido
- 2D dibujado a mano, colores planos, luces 2D.
- Personajes por partes en Photopea → PSD → 2D Animation (idle, correr, saltar, caer, morir, celebrar).
- Un tileset base con Rule Tiles recoloreado por mundo + decoración propia.
- Parallax de 2–3 capas por mundo.
- Juice: partículas, temblor de cámara, bloom, animaciones de UI con DOTween.
- Música andina/tropical libre de derechos (licencias en créditos); efectos de salto, gema, botón, puerta, muerte, eclipse, victoria y 5 notas de charango; Audio Mixer con música y efectos separados.

## 9. Tecnología
Paquetes: Universal RP (2D Renderer), 2D Tilemap Editor, 2D Tilemap Extras, 2D Sprite, 2D Animation, 2D PSD Importer, Input System, Cinemachine, TextMeshPro (uGUI), Localization, DOTween (Asset Store).

Skills del plugin oficial de Unity para Claude Code: new-unity-project, unity-cli, unity-package-management, tilemap-palette-create, tilemap-ruletile-createfromsegment, sprite-editor, manage-sprite-atlas, urp-postprocessing, ui-ugui, audio-setup-mixers, localization, optimize-web.

Plugin: https://unity.com/blog/unity-plugin-for-claude-code

## 10. Cronograma
| Semana | Fechas | Entregable |
|---|---|---|
| 1 | 28 oct – 3 nov | Proyecto, movimiento 2 jugadores, muerte/reinicio, salida, niveles 1–3 |
| 2 | 4 – 10 nov | Todas las mecánicas, niveles 4–9 |
| 3 | 11 – 17 nov | Niveles 10–20, menú, selección de nivel, guardado |
| 4 | 18 – 27 nov | Arte final, audio, juice, pruebas, publicación en itch.io, defensa |

Plan B: si al cierre de la semana 3 faltan niveles, publicar con 15 niveles pulidos.
