using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.FSharp.Core;

namespace Descriptio.Tests.UnitTests.Extensions
{
    public class FSharpOptionAssertions<T> : ReferenceTypeAssertions<FSharpOption<T>, FSharpOptionAssertions<T>>
    {
        public FSharpOptionAssertions(FSharpOption<T> subject)
        {
            Subject = subject;
        }

        protected override string Identifier => nameof(FSharpOption<T>);

        public AndConstraint<FSharpOptionAssertions<T>> BeSome(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                   .ForCondition(FSharpOption<T>.get_IsSome(Subject))
                   .BecauseOf(because, becauseArgs)
                   .FailWith($"Expected {Subject} to be 'Some'.");

            return new AndConstraint<FSharpOptionAssertions<T>>(this);
        }

        public AndConstraint<FSharpOptionAssertions<T>> BeNone(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                   .ForCondition(FSharpOption<T>.get_IsNone(Subject))
                   .BecauseOf(because, becauseArgs)
                   .FailWith($"Expected {Subject} to be '{FSharpOption<T>.None}'.");

            return new AndConstraint<FSharpOptionAssertions<T>>(this);
        }
    }
}
