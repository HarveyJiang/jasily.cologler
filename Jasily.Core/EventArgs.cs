﻿namespace System
{
    public class EventArgs<T1> : EventArgs
    {
        public EventArgs(T1 value1)
        {
            this.Value1 = value1;
        }
        
        public T1 Value1 { get; private set; }
    }

    public class EventArgs<T1, T2> : EventArgs<T1>
    {
        public EventArgs(T1 value1, T2 value2)
            : base(value1)
        {
            this.Value2 = value2;
        }

        public T2 Value2 { get; private set; }
    }

    public class EventArgs<T1, T2, T3> : EventArgs<T1, T2>
    {
        public EventArgs(T1 value1, T2 value2, T3 value3)
            : base(value1, value2)
        {
            this.Value3 = value3;
        }

        public T3 Value3 { get; private set; }
    }

    public class EventArgs<T1, T2, T3, T4> : EventArgs<T1, T2, T3>
    {
        public EventArgs(T1 value1, T2 value2, T3 value3, T4 value4)
            : base(value1, value2, value3)
        {
            this.Value4 = value4;
        }

        public T4 Value4 { get; private set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5> : EventArgs<T1, T2, T3, T4>
    {
        public EventArgs(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            : base(value1, value2, value3, value4)
        {
            this.Value5 = value5;
        }

        public T5 Value5 { get; private set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6> : EventArgs<T1, T2, T3, T4, T5>
    {
        public EventArgs(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
            : base(value1, value2, value3, value4, value5)
        {
            this.Value6 = value6;
        }

        public T6 Value6 { get; private set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7> : EventArgs<T1, T2, T3, T4, T5, T6>
    {
        public EventArgs(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
            : base(value1, value2, value3, value4, value5, value6)
        {
            this.Value7 = value7;
        }

        public T7 Value7 { get; private set; }
    }
}