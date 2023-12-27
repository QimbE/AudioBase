using Application.Behaviors;
using FluentAssertions;
using FluentValidation;
using LanguageExt.Common;
using MediatR;

namespace ApplicationTests;

public class ValidationPipelineBehaviorTests
{
    public record TestRequest(string TestField): IRequest<Result<string>>;

    public class TestRequestHandler : IRequestHandler<TestRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            return "hehehehuh";
        }
    }

    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(r => r.TestField)
                .MaximumLength(5);
        }
    }

    [Fact]
    public void ValidationBehavior_ShouldReturnValidationException_OnInvalidRequest()
    {
        // Arrange
        var request = new TestRequest("invalidfield");

        var validator = new TestRequestValidator();

        var requestHandler = new TestRequestHandler();

        RequestHandlerDelegate<Result<string>> handlerDelegate = () => requestHandler.Handle(request, default);

        var behavior = new ValidationPipelineBehavior<TestRequest, string>(validator);
        
        // Act
        var res = behavior.Handle(request, handlerDelegate, default).GetAwaiter().GetResult();
        
        // Assert
        res.IsFaulted.Should().BeTrue();
        
        res.Match(
            success => success,
            fail => fail.ToString()
            ).Should().NotBeEquivalentTo("hehehehuh");
    }
    
    [Fact]
    public void ValidationBehavior_ShouldReturnString_OnValidRequest()
    {
        // Arrange
        var request = new TestRequest("Valid");

        var validator = new TestRequestValidator();

        var requestHandler = new TestRequestHandler();

        RequestHandlerDelegate<Result<string>> handlerDelegate = () => requestHandler.Handle(request, default);

        var behavior = new ValidationPipelineBehavior<TestRequest, string>(validator);
        
        // Act
        var res = behavior.Handle(request, handlerDelegate, default).GetAwaiter().GetResult();
        
        // Assert
        res.IsSuccess.Should().BeTrue();
        
        res.Match(
            success => success,
            fail => fail.ToString()
        ).Should().BeEquivalentTo("hehehehuh");
    }
}