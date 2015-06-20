#region License

// Namespace : Stability.Data.Compression.DataStructure
// FileName  : Struple.cs
// Created   : 2015-6-15
// Author    : Bennett R. Stabile 
// Copyright : Stability Systems LLC, 2015
// License   : GPL v3
// Website   : http://DeltaCodec.CodePlex.com

#endregion // License
namespace Stability.Data.Compression.DataStructure
{
    public struct Struple<T>
        where T : struct
    {
        public Struple(T item1 = default(T))
        {
            Item1 = item1;
        }

        public T Item1;
    }

    public struct Struple<T1, T2>
        where T1 : struct
        where T2 : struct
    {
        public Struple(T1 item1 = default(T1), T2 item2 = default(T2))
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1;
        public T2 Item2;
    }

    public struct Struple<T1, T2, T3>
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        public Struple(T1 item1 = default(T1), T2 item2 = default(T2), T3 item3 = default(T3))
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
    }

    public struct Struple<T1, T2, T3, T4>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4))
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
    }

    public struct Struple<T1, T2, T3, T4, T5>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5))
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }

        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
        }

        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6, T7>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            , T7 item7 = default(T7)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
        }

        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6, T7, T8>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            , T7 item7 = default(T7)
            , T8 item8 = default(T8)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Item8 = item8;
        }

        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            , T7 item7 = default(T7)
            , T8 item8 = default(T8)
            , T9 item9 = default(T9)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Item8 = item8;
            Item9 = item9;
        }
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
        public T9 Item9;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            , T7 item7 = default(T7)
            , T8 item8 = default(T8)
            , T9 item9 = default(T9)
            , T10 item10 = default(T10)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Item8 = item8;
            Item9 = item9;
            Item10 = item10;
        }
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
        public T9 Item9;
        public T10 Item10;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            , T7 item7 = default(T7)
            , T8 item8 = default(T8)
            , T9 item9 = default(T9)
            , T10 item10 = default(T10)
            , T11 item11 = default(T11)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Item8 = item8;
            Item9 = item9;
            Item10 = item10;
            Item11 = item11;
        }
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
        public T9 Item9;
        public T10 Item10;
        public T11 Item11;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
        where T12 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            , T7 item7 = default(T7)
            , T8 item8 = default(T8)
            , T9 item9 = default(T9)
            , T10 item10 = default(T10)
            , T11 item11 = default(T11)
            , T12 item12 = default(T12)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Item8 = item8;
            Item9 = item9;
            Item10 = item10;
            Item11 = item11;
            Item12 = item12;
        }
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
        public T9 Item9;
        public T10 Item10;
        public T11 Item11;
        public T12 Item12;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
        where T12 : struct
        where T13 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            , T7 item7 = default(T7)
            , T8 item8 = default(T8)
            , T9 item9 = default(T9)
            , T10 item10 = default(T10)
            , T11 item11 = default(T11)
            , T12 item12 = default(T12)
            , T13 item13 = default(T13)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Item8 = item8;
            Item9 = item9;
            Item10 = item10;
            Item11 = item11;
            Item12 = item12;
            Item13 = item13;
        }
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
        public T9 Item9;
        public T10 Item10;
        public T11 Item11;
        public T12 Item12;
        public T13 Item13;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
        where T12 : struct
        where T13 : struct
        where T14 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            , T7 item7 = default(T7)
            , T8 item8 = default(T8)
            , T9 item9 = default(T9)
            , T10 item10 = default(T10)
            , T11 item11 = default(T11)
            , T12 item12 = default(T12)
            , T13 item13 = default(T13)
            , T14 item14 = default(T14)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Item8 = item8;
            Item9 = item9;
            Item10 = item10;
            Item11 = item11;
            Item12 = item12;
            Item13 = item13;
            Item14 = item14;
        }
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
        public T9 Item9;
        public T10 Item10;
        public T11 Item11;
        public T12 Item12;
        public T13 Item13;
        public T14 Item14;
    }

    public struct Struple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct
        where T11 : struct
        where T12 : struct
        where T13 : struct
        where T14 : struct
        where T15 : struct
    {
        public Struple(
            T1 item1 = default(T1)
            , T2 item2 = default(T2)
            , T3 item3 = default(T3)
            , T4 item4 = default(T4)
            , T5 item5 = default(T5)
            , T6 item6 = default(T6)
            , T7 item7 = default(T7)
            , T8 item8 = default(T8)
            , T9 item9 = default(T9)
            , T10 item10 = default(T10)
            , T11 item11 = default(T11)
            , T12 item12 = default(T12)
            , T13 item13 = default(T13)
            , T14 item14 = default(T14)
            , T15 item15 = default(T15)
            )
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Item8 = item8;
            Item9 = item9;
            Item10 = item10;
            Item11 = item11;
            Item12 = item12;
            Item13 = item13;
            Item14 = item14;
            Item15 = item15;
        }
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
        public T9 Item9;
        public T10 Item10;
        public T11 Item11;
        public T12 Item12;
        public T13 Item13;
        public T14 Item14;
        public T15 Item15;
    }
}
