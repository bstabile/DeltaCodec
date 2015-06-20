#region License

// Namespace : Stability.Data.Compression.DataStructure
// FileName  : ListSegment.cs
// Created   : 2015-6-15
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
using System;
using System.Collections.Generic;

namespace Stability.Data.Compression.DataStructure
{
    /// <summary>
    /// WARNING: This type is inherently unsafe! 
    /// We can't be sure that the client is not manipulating the list while 
    /// we are holding a reference to it. The ArraySegment is at least safe
    /// from random insert and remove operations. So that is the preferred type 
    /// to use when you can afford the extra memory of calling ToArray() on a list.
    /// </summary>
    internal struct ListSegment<T>
    {
        private readonly IList<T> _list; 
        private readonly int _offset; 
        private readonly int _count;

        public ListSegment(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list"); 
 
            _list = list; 
            _offset = 0; 
            _count = list.Count;
        } 
 
        public ListSegment(IList<T> list, int offset, int count)
        {
            if (list == null) 
                throw new ArgumentNullException("list");
            if (offset < 0) 
                throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum"); 
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_NeedNonNegNum"); 
            if (list.Count - offset < count)
                throw new ArgumentException("Argument_InvalidOffLen");
 
            _list = list; 
            _offset = offset;
            _count = count;
        } 
 
        public IList<T> List { 
            get { return _list; }
        }
 
        public int Offset { 
            get { return _offset; }
        } 
  
        public int Count {
            get { return _count; } 
        }
 
        public override int GetHashCode()
        { 
            return _list.GetHashCode() ^ _offset   ^ _count;
        } 
  
        public override bool Equals(Object obj)
        { 
            if (obj is ListSegment<T>)
                return Equals((ListSegment<T>)obj);
            else
                return false; 
        }
  
        public bool Equals(ListSegment<T> obj) 
        {
            return obj._list.Equals(_list) && obj._offset == _offset && obj._count == _count; 
        }
 
        public static bool operator ==(ListSegment<T> a, ListSegment<T> b)
        { 
            return a.Equals(b);
        } 
  
        public static bool operator !=(ListSegment<T> a, ListSegment<T> b)
        { 
            return !a.Equals(b);
        }
 
    } 
}

