
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;




namespace terraria.Music;


public interface IMusic
{
    public Song Song {get; set;}
    public bool Looping{get; set;}
    public void Update(double v);
    public void Play();
    public void Stop();
    public void NextSong();
}