
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;



namespace terraria.Music;


//maybe should create enum list of songs representing IDs
public class MusicPlayer : IMusic
{
    private List<Song> _playlist;
    private int _currentSongIndex = 0;
    public Song Song {get; set;}
    public bool Looping{get; set;}
    public float Volume{get;set;}

    public MusicPlayer(Song initSong, bool loop = false, float volume = 1.0f)
    {
        _playlist = [initSong];
        Song = initSong;
        Looping = loop;
        Volume = volume;
        MediaPlayer.IsRepeating = loop;
    }

    public MusicPlayer(List<Song> playList, bool loop = false, float volume = 1.0f)
    {
        _playlist = playList;
        Looping = loop;
        Volume = volume;
        MediaPlayer.IsRepeating = loop;
    }

    public void SetPlaylist(List<Song> playList)
    {
        _playlist = playList;
    }

    public void Update(double v)
    {
        MediaPlayer.Volume = (float) v / 100f;
        if (!Looping && MediaPlayer.State == MediaState.Stopped && _playlist.Count > 1)
        {
            NextSong();
        }
    }
    public void Start()
    {
        MediaPlayer.Play(_playlist[_currentSongIndex]);
    }
    public void Play()
    {
        if (_playlist.Count > 0)
        {
            MediaPlayer.Play(_playlist[_currentSongIndex]);
        }
    }

    public void NextSong()
    {
        if (_playlist.Count == 0) {return;}

        _currentSongIndex++;
        if (_currentSongIndex >= _playlist.Count)
        {
            _currentSongIndex = 0; //wrap around to the start
        }
        MediaPlayer.Play(_playlist[_currentSongIndex]);
    }

    public void Stop()
    {
        MediaPlayer.Stop();
    }
}