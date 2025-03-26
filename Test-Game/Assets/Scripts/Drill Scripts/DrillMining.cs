using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // For the new Input System

public class DrillController : MonoBehaviour
{
    [Tooltip("Time interval (in seconds) between mining (removing) each fragment.")]
    public float chunkMiningInterval = 1f;
    [Tooltip("Time interval (in seconds) between spawning rock particles while mining.")]
    public float particleSpawnInterval = 0.2f;
    [Tooltip("Rotation offset (in degrees) to add to the drill's rotation.")]
    public float rotationOffset = 0f;
    [Tooltip("Rock particle prefab spawned at the mining spot.")]
    public GameObject rockParticlePrefab;
    [Tooltip("Transform representing the tip of the drill where particles should spawn.")]
    public Transform drillTip;

    // Audio fields:
    [Tooltip("Sound effect for the drill when it is not hitting ore.")]
    public AudioClip drillIdleSound;
    [Tooltip("Sound effect for the drill when it is hitting ore.")]
    public AudioClip drillMiningSound;
    [Tooltip("Audio source for playing drill sounds.")]
    public AudioSource audioSource;

    private float holdTimer = 0f;
    private float particleTimer = 0f;
    // This will hold the ore fragment that is currently in contact with the drill.
    private GameObject currentFragment = null;
    private GameInput gameInput;
    private Animator drillAnimator;

    // Drill state flags.
    private bool drillActive = false;       // True if drill is activated (after activation animation).
    private bool isTransitioning = false;     // True while activation/deactivation animation is playing.

    private void Awake()
    {
        gameInput = FindFirstObjectByType<GameInput>();
        drillAnimator = GetComponent<Animator>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Toggle drill activation/deactivation with the X key.
        if (gameInput != null && gameInput.ToggleDrillTriggered && !isTransitioning)
        {
            if (!drillActive)
                StartCoroutine(ActivateDrill());
            else
                StartCoroutine(DeactivateDrill());
        }

        // Only allow drilling and rotation if the drill is active and not transitioning.
        if (drillActive && !isTransitioning)
        {
            // --- Drilling Logic ---
            if (gameInput != null && gameInput.IsMinePressed)
            {
                if (drillAnimator != null)
                    drillAnimator.SetBool("IsDrilling", true);

                // If an ore fragment is in contact, accumulate timers.
                if (currentFragment != null)
                {
                    holdTimer += Time.deltaTime;
                    particleTimer += Time.deltaTime;

                    // Spawn a rock particle at intervals from the drill tip.
                    if (particleTimer >= particleSpawnInterval)
                    {
                        SpawnRockParticle();
                        particleTimer = 0f;
                    }

                    // When the hold timer reaches the mining interval, remove the fragment.
                    if (holdTimer >= chunkMiningInterval)
                    {
                        Transform parentTransform = currentFragment.transform.parent;
                        if (parentTransform != null)
                        {
                            Ore ore = parentTransform.GetComponent<Ore>();
                            if (ore != null)
                            {
                                ore.MineFragment(currentFragment);
                            }
                            else
                            {
                                Debug.LogError("No Ore component found on the parent of the current fragment!");
                            }
                        }
                        else
                        {
                            Debug.LogError("Current fragment does not have a parent!");
                        }
                        currentFragment = null;
                        holdTimer = 0f;
                        particleTimer = 0f;
                    }
                }
            }
            else
            {
                if (drillAnimator != null)
                    drillAnimator.SetBool("IsDrilling", false);
                ResetMining();
            }

            // --- Rotation Logic ---
            if (Mouse.current != null)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector2 direction = mousePos - (Vector2)transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }
        }
        else
        {
            if (drillAnimator != null)
                drillAnimator.SetBool("IsDrilling", false);
        }

        // --- Audio Management ---
        // --- Audio Management ---
        if (audioSource != null)
        {
            if (drillActive && !isTransitioning)
            {
                if (gameInput != null && gameInput.IsMinePressed)
                {
                    if (currentFragment != null)
                    {
                        // Left click held and hitting ore: play mining sound.
                        if (audioSource.clip != drillMiningSound)
                        {
                            audioSource.clip = drillMiningSound;
                            audioSource.loop = true;
                            audioSource.Play();
                        }
                    }
                    else
                    {
                        // Left click held and not hitting ore: play idle sound.
                        if (audioSource.clip != drillIdleSound)
                        {
                            audioSource.clip = drillIdleSound;
                            audioSource.loop = true;
                            audioSource.Play();
                        }
                    }
                }
                else
                {
                    // Left click not held: stop audio.
                    if (audioSource.isPlaying)
                        audioSource.Stop();
                }
            }
            else
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();
            }
        }
    }

    private IEnumerator ActivateDrill()
    {
        isTransitioning = true;
        if (drillAnimator != null)
            drillAnimator.SetTrigger("ActivateDrill");
        yield return new WaitForSeconds(2f);
        drillActive = true;
        isTransitioning = false;
    }

    private IEnumerator DeactivateDrill()
    {
        isTransitioning = true;
        if (drillAnimator != null)
            drillAnimator.SetTrigger("DeactivateDrill");
        yield return new WaitForSeconds(2f);
        drillActive = false;
        isTransitioning = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void ResetMining()
    {
        holdTimer = 0f;
        particleTimer = 0f;
    }

    // Spawns a rock particle at the tip of the drill and rotates it to face the player.
    private void SpawnRockParticle()
    {
        if (rockParticlePrefab != null && drillTip != null)
        {
            Vector3 spawnPos = drillTip.position;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float angle = 0f;
            if (player != null)
            {
                Vector3 direction = player.transform.position - spawnPos;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            }
            Quaternion particleRotation = Quaternion.Euler(0, 0, angle);
            GameObject particle = Instantiate(rockParticlePrefab, spawnPos, particleRotation);
            Destroy(particle, 1f); // Automatically destroy the particle after 1 second.
        }
    }

    // These methods are called by the trigger handler on the drill child.
    public void SetCurrentFragment(GameObject fragment)
    {
        currentFragment = fragment;
    }

    public void ClearCurrentFragment(GameObject fragment)
    {
        if (currentFragment == fragment)
        {
            currentFragment = null;
            ResetMining();
        }
    }
}



