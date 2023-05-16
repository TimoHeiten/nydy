namespace heitech.nydy
{
    ///<summary>
    /// Maybe Monad
    ///</summary>
    public abstract class Maybe<T> : IEquatable<Maybe<T>>, IEquatable<T>
    {
        protected internal T? Value { get; }
        protected internal Maybe(T? value)
            => Value = value;

        public abstract Maybe<T> Apply(Func<T, T> apply);
        public abstract Maybe<TResult> Bind<TResult>(Func<T, TResult> bind);

        public T Fold(T whenNone)
        {
            if (EqualityComparer<T>.Default.Equals(Value, default))
                return whenNone;

            return Value!;
        }

        public static Maybe<T> Return(T? value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
                return new None();

            return new Some(value);
        }

        private sealed class Some : Maybe<T>
        {
            public Some(T? value): base(value) {}

            public override Maybe<T> Apply(Func<T, T> apply)
                => Return(apply(Value!));

            public override Maybe<TResult> Bind<TResult>(Func<T, TResult> bind)
                => Maybe<TResult>.Return(bind(Value!));
        }
    
        private sealed class None : Maybe<T>
        {
            internal None(): base(default) {}

            public override Maybe<T> Apply(Func<T, T> apply)
                => this;

            public override Maybe<TResult> Bind<TResult>(Func<T, TResult> bind)
                => Maybe<TResult>.Return(default);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Maybe<T> maybe)
                return Equals(maybe);
            else if (obj is T t)
                return Equals(t);
            else
                return false;
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            if (Value is not null)
                return Value.GetHashCode();
            else
                return base.GetHashCode();
        }

        public bool Equals(Maybe<T>? other)
            => other is not null && other.Value is not null && other.Value.Equals(Value);

        public bool Equals(T? other)
            => other is not null && other.Equals(Value);

        public static implicit operator T(Maybe<T> maybe) => maybe.Fold(default!);
    }
}