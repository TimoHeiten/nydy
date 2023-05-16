using FluentAssertions;
using heitech.nydy.Parse;
using Xunit;

namespace heitech.nydy.Tests
{
    public sealed class ParserTests
    {
        private static InputObject GetValidInput()
            => new() { Amount = 10, Name = "Heitech", Email = "any@email.com" };

        private static Parser<InputObject, ParsedObject> GetConfiguredParser(InputObject input)
        {
            var parser = new Parser<InputObject, ParsedObject>(input);
            return parser.Validate(x => 0 < x.Amount && x.Amount < ushort.MaxValue)
                         .Validate(x => !string.IsNullOrWhiteSpace(x.Name))
                         .Validate(x => !string.IsNullOrWhiteSpace(x.Email) && x.Email.Contains("@"))

                         .Map(v => v.Name, x => x.Name)
                         .Map(v => (ushort)v.Amount, x => x.Amount)
                         .Map(v => new Email { Value = v.Email }, x => x.Email);
        }

        [Fact]
        public void ParserForValidObjectReturnsMaybeSome()
        {
            // Given
            var input = GetValidInput();
            var parser = GetConfiguredParser(input);

            // When
            var result = parser.Parse();

            // Then
            var liberated = result.Fold(null!);
            liberated.Should().NotBeNull();
            liberated.Should().BeEquivalentTo(new ParsedObject
            {
                Amount = 10,
                Name = "Heitech",
                Email = new Email { Value = "any@email.com" }
            });
        }

        [Theory]
        [InlineData(0, "name", "email@provider")]
        [InlineData(-1, "name", "email@provider")]
        [InlineData(int.MaxValue, "name", "email@provider")]

        [InlineData(1, "", "email@provider")]
        [InlineData(1, "\t", "email@provider")]
        [InlineData(1, null, "email@provider")]

        [InlineData(1, "name", "")]
        [InlineData(1, "name", null)]
        [InlineData(1, "name", "emailprovider")]
        public void ParserForInvalidObjectReturnsMaybeNone(int amount, string name, string email)
        {
            InputObject input = new() { Amount = amount, Name = name, Email = email };
            var parser = GetConfiguredParser(input);

            // When
            var result = parser.Parse().Fold(null!);

            // Then
            result.Should().BeNull();
        }

        [Fact]
        public void ParserValidationThrowsReturnsMaybeNone()
        {
            // Given
            var parser = new Parser<int, object>(0);
            parser.Validate(x => throw new System.Exception());
        
            // When
            var result = parser.Parse().Fold(-1);
        
            // Then
            result.Should().Be(-1);
        }

        [Fact]
        public void ParserMappingThrowsReturnsMaybeNone()
        {
            // Given
            var parser = new Parser<int, ParsedObject>(0);
            parser.Map(x => x, x => x.Amount);

            // When
            var result = parser.Parse().Fold(null!);

            // Then
            result.Should().BeNull();
        }


        private sealed class InputObject
        {
            public int Amount { get; set; }
            public string Name { get; set; } = default!;
            public string Email { get; set; } = default!;
        }

        private sealed class ParsedObject
        {
            public Email Email { get; set; }
            public ushort Amount { get; set; }
            public string Name { get; set; } = default!;
        }

        private struct Email { public string Value { get; set; } }
    }
}