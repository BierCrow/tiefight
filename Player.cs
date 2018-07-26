using AsciiEngine;
using Easy;
using System;

public class Player : Sprite
{

    public SpriteField Missiles = new SpriteField();
    public SpriteField Messages = new SpriteField();

    public int MaxMissiles = 1;
    SpriteField Debris = new SpriteField();

    public Player()
    {
        this.Ascii = ":><:".ToCharArray();
        this.FlyZone = new FlyZoneClass(0, 0, 0, 0, FlyZoneClass.eEdgeMode.Stop);
        this.Trajectory = new Screen.Trajectory(0, 0);
        this.Trail = new Screen.CoordinateHistory(new Screen.Coordinate(Screen.Width / 2 - this.Width / 2, Screen.BottomEdge));
        this.HP = 1;
    }

    public void BigExplosion()
    {
        foreach (char c in "\x00d7*#-".ToCharArray())
        {
            for (int i = 0; i < 10; i++)
            {
                double Rise = -2 * Numbers.Random.NextDouble();
                double Run = 2 * Numbers.Random.NextDouble();
                if (Numbers.Random.NextDouble() < .5) { Run *= -1; }
                Debris.Items.Add(new Sprite(new[] { c }, new Screen.Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Screen.Trajectory(Rise, Run, 20)));
            }
        }
    }

    public void Fire()
    {
        if (this.Missiles.Items.Count < this.MaxMissiles)
        {
            this.Missiles.Items.Add(new Sprite(new[] { '|' }, new Screen.Coordinate(this.XY.X + this.Width / 2, this.XY.Y), new Screen.Trajectory(-1, 0, this.XY.Y)));
        }
    }

    public void CheckBadGuyHits(BadGuyField badguys)
    {
        foreach (Sprite missile in this.Missiles.Items.FindAll(x => x.Alive))
        {
            foreach (BadGuy badguy in badguys.Items.FindAll(x => x.Alive))
            {
                if (badguy.Hit(missile.XY))
                {
                    missile.Terminate();
                    badguy.MakeDebris();
                }

            }
        }

        if (this.MaxMissiles < badguys.MaxBadGuys / 3 && this.Alive)
        {
            this.MaxMissiles++;
            this.Messages.Items.Add(new Sprite("Extra Missile".ToCharArray(), this.XY, new Screen.Trajectory(-1, 0, Screen.Height / 2)));
        }

    }

    override public void DoActivities()
    {
        Missiles.Animate();
        Messages.Animate();
        Debris.Animate();

        if (!this.Alive && this.Debris.Items.Count < 1 && this.Missiles.Items.Count < 1) { this.Active = false; }
    }

    public void CheckHitByBadGuys(BadGuyField badguys)
    {
        foreach (BadGuy badguy in badguys.Items)
        {
            foreach (Sprite missile in badguy.Missiles.Items)
            {
                if (this.Hit(missile.XY))
                {
                    missile.Terminate();
                    this.BigExplosion();
                }

            }
        }
    }


}