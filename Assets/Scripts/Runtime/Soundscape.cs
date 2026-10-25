using UnityEngine;
namespace LittleElsewhere
{
    public sealed class Soundscape : MonoBehaviour
    {
        AudioSource music,wind,effects;
        AudioClip step,bell,pickup;
        public void Setup()
        {
            music=gameObject.AddComponent<AudioSource>();wind=gameObject.AddComponent<AudioSource>();effects=gameObject.AddComponent<AudioSource>();
            music.clip=Resources.Load<AudioClip>("Audio/music");wind.clip=Resources.Load<AudioClip>("Audio/wind");music.loop=wind.loop=true;
            music.Play();wind.Play();
            step=Resources.Load<AudioClip>("Audio/step");bell=Resources.Load<AudioClip>("Audio/bell");pickup=Resources.Load<AudioClip>("Audio/pickup");
        }
        public void Apply(Preferences p){music.volume=Mathf.Clamp01(p.music);wind.volume=Mathf.Clamp01(p.sound)*.5f;effects.volume=Mathf.Clamp01(p.sound);}
        public void Step(){effects.pitch=Random.Range(.85f,1.12f);if(step!=null)effects.PlayOneShot(step,.55f);}
        public void Bell(){effects.pitch=1;if(bell!=null)effects.PlayOneShot(bell);}
        public void Pickup(){effects.pitch=1;if(pickup!=null)effects.PlayOneShot(pickup);}
    }
}
