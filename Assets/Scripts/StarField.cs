using UnityEngine;

// Synchronise la vitesse de simulation du Particle System "StarField"
// avec le SpeedMultiplier du Spawner.
// Quand un pickup SpeedUp est actif, les étoiles défilent plus vite ;
// quand un pickup SlowDown est actif, elles ralentissent.
public class StarField : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem System;     // Le Particle System à piloter (lié dans l'Inspector)

    [SerializeField]
    private Spawner Spawner;           // Référence au Spawner pour lire SpeedMultiplier

    private ParticleSystem.MainModule _main;   // Module Main du Particle System (mis en cache)

    void Awake()
    {
        // Si on n'a pas glissé une ref dans l'Inspector, on cherche le composant sur ce GameObject
        if (System == null) System = GetComponent<ParticleSystem>();

        // On cache le module Main pour ne pas faire l'accès chaque frame
        _main = System.main;
    }

    void Update()
    {
        // Pas de Spawner lié → on ne touche à rien (multiplicateur reste = 1)
        if (Spawner == null) return;

        // simulationSpeed accélère/ralentit toute la simulation des particules
        // sans avoir à toucher Velocity, Lifetime, Emission Rate séparément
        _main.simulationSpeed = Spawner.SpeedMultiplier;
    }
}
