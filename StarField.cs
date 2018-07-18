using System;
using System.Collections.Generic;

public class StarField
{

    #region " Sub Classes "

    class Star
    {

        #region " Properties " 

        int _X;
        int _Y;
        int _FrameCount;
        int _SkipFrames;
        char _Ascii;

        public bool OffScreen
        {
            get { return _Y < 0 || _Y > Textify.BottomEdge; }
        }

        #endregion

        #region " Constructor "

        public Star(int Y, int skipframes, char ascii)
        {
            Random r = new Random();
            this._X = r.Next(Textify.LeftEdge + 1, Textify.RightEdge - 1);
            this._Y = Y;
            this._SkipFrames = skipframes;
            this._FrameCount = this._SkipFrames;
            this._Ascii = ascii;
        }

        public Star(int skipframes, char ascii) : this(-1, skipframes, ascii) { }

        #endregion

        #region " Animation "

        void Hide()
        {
            if (!this.OffScreen)
            {
                Console.SetCursorPosition(_X, _Y);
                Console.Write(' ');
            }
        }

        void Show()
        {
            if (!this.OffScreen)
            {
                Console.SetCursorPosition(_X, _Y);
                Console.Write(this._Ascii);
            }
        }

        public void Move()
        {
            if (this._FrameCount >= this._SkipFrames)
            {
                this.Hide();
                _Y++;
                _FrameCount = 0;
                this.Show();
            }
            _FrameCount++;
        }

        #endregion
    }

    #endregion

    #region " Properties "

    List<Star> starfield = new List<Star>();
    int _SkipFrames;
    int _MaxStars;
    char _Ascii;

    #endregion

    #region " Methods "

    public void Execute()
    {
        this.Sweep();
        this.Spawn();
        this.Animate();
    }

    void Sweep()
    {
        starfield.Remove(starfield.Find(x => x.OffScreen));
    }

    void Spawn()
    {
        while (starfield.Count < this._MaxStars)
        {
            starfield.Add(new Star(this._SkipFrames, this._Ascii));
        }
    }

    void Animate()
    {
        foreach (var s in starfield) { s.Move(); }
    }

    #endregion

    #region " Constructor "

    public StarField(int skipframes, int maximum, char ascii)
    {
        Random r = new Random();
        this._SkipFrames = skipframes;
        this._MaxStars = maximum;
        this._Ascii = ascii;
        for (int count = 0; count < maximum; count++)
        {
            starfield.Add(new Star(r.Next(0, Textify.Height), skipframes, this._Ascii));
        }
    }

    #endregion

}
