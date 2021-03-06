﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eternity.Input
{
    public class EternityEvent : EventArgs
    {
        public EventType Type { get; set; }
        public MouseButton Button { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int LastX { get; set; }
        public int LastY { get; set; }
        public int Delta { get; set; }
        public Key Key { get; set; }
        public bool Focused { get; set; }

        public bool Handled { get; set; }

        public EternityEvent()
        {
            Handled = false;
        }

        public void Translate(double x, double y)
        {
            X -= (int) x;
            Y -= (int) y;
            LastX -= (int) x;
            LastY -= (int) y;
        }

        public EternityEvent Clone()
        {
            return new EternityEvent
                       {
                           Type = Type,
                           Button = Button,
                           X = X,
                           Y = Y,
                           LastX = LastX,
                           LastY = LastY,
                           Delta = Delta,
                           Key = Key,
                           Focused = Focused,
                           Handled = Handled
                       };
        }
    }
}
