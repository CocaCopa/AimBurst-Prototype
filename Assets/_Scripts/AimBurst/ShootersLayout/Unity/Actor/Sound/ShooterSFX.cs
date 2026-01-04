using System;
using AimBurst.ShootersLayout.Runtime.Abstractions;
using AimBurst.ShootersLayout.Unity.Actor;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity {
    [RequireComponent(typeof(AudioSource))]
    internal sealed class ShooterSFX : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Shooter shooter;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip shootClip;
        [SerializeField] private PitchRange shootPitch;
        [SerializeField] private AudioClip selectClip;

        private AudioSource audioSrc;

        private void Start() {
            audioSrc = GetComponent<AudioSource>();
            shooter.OnAmmoChanged += Shooter_OnAmmoChanged;
            shooter.OnPositioningChanged += Shooter_OnPositioningChanged;
        }

        private void Shooter_OnAmmoChanged(int _) {
            audioSrc.pitch = shootPitch.GetRandom();
            audioSrc.PlayOneShot(shootClip);
        }

        private void Shooter_OnPositioningChanged() {
            if (shooter.Positioning != ShooterPositioning.Slot) { return; }

            audioSrc.pitch = 1f;
            audioSrc.PlayOneShot(selectClip);
        }

        [Serializable]
        private class PitchRange {
            private const float Min = 0.5f;
            private const float Max = 1.5f;
            [Range(Min, Max)] public float min;
            [Range(Min, Max)] public float max;

            public float GetRandom() {
                return UnityEngine.Random.Range(min, max);
            }
        }
    }
}
