using System.Media;
using System.Threading;

public class Sounds
{
    private SoundPlayer backgroundPlayer;

    public Sounds()
    {
        backgroundPlayer = new SoundPlayer("main.wav");
    }

    public void PlayBackground()
    {
        backgroundPlayer.PlayLooping();
    }

    public void StopBackground()
    {
        backgroundPlayer.Stop();
    }

    public void PlayEat()
    {
        new Thread(() =>
        {
            try
            {
                SoundPlayer eatPlayer = new SoundPlayer("eat.wav");
                eatPlayer.Play();
            }
            catch { }
        }).Start();
    }

    public void PlayDead()
    {
        new Thread(() =>
        {
            try
            {
                SoundPlayer deadPlayer = new SoundPlayer("dead.wav");
                deadPlayer.Play();
            }
            catch { }
        }).Start();
    }
}
