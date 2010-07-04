using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Level.Generator
{
    internal struct ParenthoodElement
    {
        public MapChunk Chunk;
        public Point UpperLeft;
        public Point Seam;

        public ParenthoodElement(MapChunk chunk, Point upperLeft, Point seam)
        {
            Chunk = chunk;
            UpperLeft = upperLeft;
            Seam = seam;
        }
    }

    internal sealed class ParenthoodChain
    {
        public Stack<MapChunk> Chunks { get; set; }
        public Stack<Point> UpperLefts { get; set; }
        public Stack<Point> Seams { get; set; }

        public ParenthoodChain()
        {
            Chunks = new Stack<MapChunk>();
            UpperLefts = new Stack<Point>();
            Seams = new Stack<Point>();
        }

        public ParenthoodChain(ParenthoodChain p)
        {
            // There is no Stack constructor that take a stack, just the enumerable one
            // Since we want a shallow copy, we'll just reverse the list and use that
            Chunks = new Stack<MapChunk>(p.Chunks.Reverse());
            UpperLefts = new Stack<Point>(p.UpperLefts.Reverse());
            Seams = new Stack<Point>(p.Seams.Reverse());
        }

        public void Push(MapChunk c, Point upperLeft, Point seam)
        {
            Chunks.Push(c);
            UpperLefts.Push(upperLeft);
            Seams.Push(seam);
        }

        public ParenthoodElement Pop()
        {
            ParenthoodElement top = new ParenthoodElement(Chunks.Peek(), UpperLefts.Peek(), Seams.Peek());
            Chunks.Pop();
            UpperLefts.Pop();
            Seams.Pop();
            return top;
        }

        public ParenthoodElement Peek()
        {
            return new ParenthoodElement(Chunks.Peek(), UpperLefts.Peek(), Seams.Peek());
        }
    }
}
