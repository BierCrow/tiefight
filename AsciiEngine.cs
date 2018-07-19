using AsciiEngine;
using Easy;
using System;
using System.Collections.Generic;

namespace AsciiEngine
{

    #region " Sprites "

    public class Sprite
    {

        double _OriginalX;
        double _OriginalY;
        double _X;
        double _Y;
        double _IncrementX;
        double _IncrementY;
        char _Ascii;
        double _Range;
        bool _Killed = false;

        public int X { get { return EzMath.RoundInt(this._X); } }
        public int Y { get { return EzMath.RoundInt(this._Y); } }

        public bool Alive
        {
            get { return EzMath.Distance(this._X, this._OriginalX, this._Y, this._OriginalY) < this._Range && !this._Killed; }
        }

        public void Hide()
        {
            Screen.TryWrite(EzMath.RoundInt(this._X), EzMath.RoundInt(this._Y), ' ');
        }

        public void Kill()
        {
            this._Killed = true;
        }
        public void Animate()
        {
            this.Hide();
            this._X += this._IncrementX;
            this._Y += this._IncrementY;
            Screen.TryWrite(EzMath.RoundInt(this._X), EzMath.RoundInt(this._Y), this._Ascii);
        }

        public Sprite(char c, double x, double y, double range) : this(c, x, y, -1, -1, range) { } // random direction increments

        public Sprite(char c, double x, double y, double incx, double incy, double range)
        {
            this._Ascii = c;
            this._OriginalX = x;
            this._OriginalY = y;
            this._X = x;
            this._Y = y;
            this._Range = range;

            if (incx == -1 && incy == -1)
            {
                // add a fraction to make sure it's not zero
                this._IncrementX = EzMath.Random.NextDouble() + .1;
                this._IncrementY = EzMath.Random.NextDouble() + .1;
                if (EzMath.Random.NextDouble() < .5) { this._IncrementX *= -1; }
                if (EzMath.Random.NextDouble() < .5) { this._IncrementY *= -1; }

            }
            else
            {
                this._IncrementX = incx;
                this._IncrementY = incy;

            }

        }

    }

    public class SpriteField
    {

        List<Sprite> _sprites = new List<Sprite>();

        public List<Sprite> Sprites
        {
            get { return _sprites; }
        }

        public void RemoveSprite(Sprite s)
        {
            this.Sprites.Find(x => s.Equals(x)).Kill();
        }

        public void Animate()
        {

            foreach (Sprite s in this._sprites.FindAll(x => !x.Alive))
            {
                s.Hide();
                this.Sprites.Remove(s);
            }

            foreach (Sprite s in this._sprites.FindAll(x => x.Alive)) { s.Animate(); }
        }

        public SpriteField() { }

    }

    #endregion

    #region " Static Properties "

    public class Screen
    {

        public static int TopEdge { get { return 0; } }

        public static int BottomEdge { get { return Console.WindowHeight - 1; } }

        public static int LeftEdge { get { return 0; } }

        public static int RightEdge { get { return Console.WindowWidth - 1; } }

        public static int Width { get { return Console.WindowWidth; } }

        public static int Height { get { return Console.WindowHeight; } }

        #endregion

        #region " Screen Manipulation "

        public static bool TrySetWindowSize(int setwidth, int setheight)
        {
            return TrySetWindowSize(setwidth, setheight, true);
        }

        public static bool TrySetWindowSize(int setwidth, int setheight, bool manualadjust)
        {

            try
            {
                Console.SetWindowSize(setwidth, setheight);
                Console.SetBufferSize(setwidth, setheight);
            }
            catch
            {

                while ((Console.WindowWidth != setwidth || Console.WindowHeight != setheight) && (!Console.KeyAvailable) && manualadjust)
                {

                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("can't resize window.");
                    Console.WriteLine("please adjust manually.");
                    Console.WriteLine();

                    Console.Write("adjust width: ");
                    if (Console.WindowWidth > setwidth) { Console.WriteLine(new String('\x2190', Console.WindowWidth - setwidth)); }
                    else if (Console.WindowWidth < setwidth) { Console.WriteLine(new String('\x2192', setwidth - Console.WindowWidth)); }
                    else { Console.WriteLine("Perfect!"); }

                    Console.Write("adjust height: ");
                    if (Console.WindowHeight > setheight) { Console.Write(new String('\x2191', Console.WindowHeight - setheight)); }
                    else if (Console.WindowHeight < setheight) { Console.Write(new String('\x2193', setheight - Console.WindowHeight)); }
                    else { Console.Write("Perfect!"); }

                    System.Threading.Thread.Sleep(100); // good enough so the CPU doesn't go crazy

                }

            }

            return (Console.WindowHeight == setheight && Console.WindowWidth == setwidth);

        }

        #endregion

        public static char CharPrompt(string s)
        {
            Console.Write(s);
            return Console.ReadKey(true).KeyChar;
        }

        public static bool TryWrite(int x, int y, string s)
        {
            try
            {
                if (y >= Screen.TopEdge && y <= Screen.BottomEdge)
                {
                    // the whole string should fit on the screen
                    if (x >= Screen.LeftEdge && x + s.Length < Screen.RightEdge)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(s);
                        return true;
                    }
                    // some or all of the text is off the screen, so go character by character
                    else
                    {
                        char[] chars = s.ToCharArray();
                        for (int c = 0; c < chars.Length; c++) { Screen.TryWrite(x + c, y, chars[c]); }
                        return (x >= Screen.LeftEdge && x + s.Length - 1 <= Screen.RightEdge && y >= Screen.TopEdge && y <= Screen.BottomEdge);
                    }
                }
                else
                { return false; }
            }
            catch { return false; }
        }

        public static bool TryWrite(int x, int y, char c)
        {
            // don't write anything past the screen edges, nor in lower right corner
            if (x >= Screen.LeftEdge && x <= Screen.RightEdge && y >= Screen.TopEdge && y <= Screen.BottomEdge && !(x == Screen.RightEdge && y == Screen.BottomEdge))
            {
                Console.SetCursorPosition(x, y);
                Console.Write(c);
                return true;
            }
            else { return false; }
        }

    }
}
