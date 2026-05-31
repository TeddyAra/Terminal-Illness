using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] deathSounds;
    
    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
    }
}
