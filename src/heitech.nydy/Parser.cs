using System.Linq.Expressions;
using System.Reflection;

namespace heitech.nydy.Parse
{
    ///<summary>
    /// Parser to convert one type to another and apply different validations
    ///</summary>
    public sealed class Parser<TInput, TResult>
        where TResult : class, new()
    {
        private bool isValid = true;
        private readonly TResult _value;
        private readonly TInput _input;
        public Parser(TInput input)
            => (_value, _input) = (new(), input);

        ///<summary>
        /// Add a simple validation per predicate
        ///</summary>
        public Parser<TInput, TResult> Validate(Func<TInput, bool> predicate)
        {
            if (!isValid)
                return this;

            try
            {
                isValid = predicate(_input);
            }
            catch
            {
                isValid = false;
            }

            return this;
        }

        ///<summary>
        /// Map a property from the input to the result
        ///</summary>
        public Parser<TInput, TResult> Map<TFrom, TTo>(Func<TInput, TFrom> from, Expression<Func<TResult, TTo>> to)
        {
            if (isValid == false)
                return this;

            var valueFrom = from(_input);
            if (to.Body is not MemberExpression member)
                return this;

            if (member.Member is not PropertyInfo property)
                return this;

            try
            {
                property.SetValue(_value, valueFrom, null);
            }
            catch
            {
                isValid = false;
                return this;
            }
            return this;
        }

        ///<summary>
        /// Parse the input to the result, when fully configured
        ///</summary>
        public Maybe<TResult> Parse()
            => Maybe<TResult>.Return(isValid ? _value : default);
    }

    // todo add predifined validations like alll props mapped
    // adjust interface to a builder like so you cannot call Parse on the initial Parser object
}