using System;
using System.Threading.Tasks;
using Alsein.Extensions.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.Audio;
using Alsein.Extensions;

namespace Cynthia.Card.Client
{
    [Singleton]
    public class SourceService
    {
        private AudioSource _audioSource;
        private AudioClip _mainBgm;
        private AudioMixer _mainMixer;
        public SourceService()
        {
            _audioSource = GameObject.Find("MusicSource").GetComponent<AudioSource>();
            _mainBgm = (AudioClip)Resources.Load("Music/Main",typeof(AudioClip));
            _mainMixer = (AudioMixer)Resources.Load("Music/AudioMixer", typeof(AudioMixer));
        }
        public void MusicToMain()
        {
            _audioSource.clip = _mainBgm;
            _audioSource.Play();
        }
    }
}
