using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System;

namespace heitech.nydy.Tests
{
    public sealed class MaybeTests
    {
        [Theory]
        [MemberData(nameof(None))]
        public void CreateMaybeFromDefault_ReturnsNone(Func<object> result, object shouldBe)
        {
            result().Should().BeEquivalentTo(shouldBe);
        }

        private static NonNullableType TheNull => new();
        private sealed class NonNullableType 
        {
            public int Id = 42;
        }

        public static IEnumerable<object[]> None()
        {
            yield return new object[] { () => (object)Maybe<int?>.Return(default).Fold(100)!, 100 };
            yield return new object[] { () => (object)Maybe<string>.Return(default).Fold("100"), "100" };
            yield return new object[] { () => (object)Maybe<NonNullableType>.Return(default).Fold(TheNull), TheNull };
        }

        [Fact]
        public void ApplyOnNone_ReturnsNone()
        {
            var result = Maybe<int>.Return(default).Apply(x => x + 1);
            var r = result.Fold(100);
            r.Should().Be(100);
        }

        [Fact]
        public void BindOnNoneReturnsNone()
        {
            var result = Maybe<int>.Return(default).Bind(x => x > 2);
            result.Fold(true).Should().Be(true);
        }


        [Fact]
        public void ApplyOnSome_ReturnsSome()
        {
            // Given
            var some = Maybe<int>.Return(10);

            // When
            var result = some.Apply(x => x + 1);

            // Then
            result.Fold(-42).Should().Be(11);
        }

        [Fact]
        public void BindOnSome_ReturnsSome()
        {
            // Given
            var some = Maybe<int>.Return(10);

            // When
            var result = some.Bind(x => x > 2);
            
            // Then
            result.Fold(false).Should().Be(true);
        }

        [Fact]
        public void MaybeNone_Equals_Default()
        {
            // Given
            int def = default;
            var none = Maybe<int>.Return(default);
        
            // When
            none.Equals(def).Should().BeTrue();
        }

        [Fact]
        public void MaybeSome_Equals_Value()
        {
            var ten = 10;
            var otherTen = Maybe<int>.Return(10);

            ten.Equals(otherTen).Should().BeTrue();
            otherTen.Equals(ten).Should().BeTrue();
        }

        [Fact]
        public void MaybeSome_Equals_OtherMaybeSome()
        {
            var ten = Maybe<int>.Return(10);
            var otherTen = Maybe<int>.Return(10);

            ten.Equals(otherTen).Should().BeTrue();
            otherTen.Equals(ten).Should().BeTrue();
        }

        [Fact]
        public void MaybeSome_DoesNotEqual_None()
        {
            var none = Maybe<int>.Return(default);
            var some = Maybe<int>.Return(10);

            none.Equals(some).Should().BeFalse();
            some.Equals(none).Should().BeFalse();
        }
    }
}