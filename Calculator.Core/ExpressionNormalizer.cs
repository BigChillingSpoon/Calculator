using Calculator.Core.Interfaces;
using Calculator.Core.Models.Enums;
using Calculator.Core.Models;
using Calculator.Core.Models.Interfaces;
using Calculator.Core.Exceptions;

public class ExpressionNormalizer : IExpressionNormalizer
{
    private readonly IUnaryClassifier _unaryClassifier;
    private readonly ISignNormalizer _signNormalizer;
    private readonly IUnaryMerger _unaryMerger;

    public ExpressionNormalizer(IUnaryClassifier unaryClassifier, ISignNormalizer signNormalizer, IUnaryMerger unaryMerger)
    {
        _unaryClassifier = unaryClassifier;
        _signNormalizer = signNormalizer;
        _unaryMerger = unaryMerger;
    }

    public ParsingResult NormalizeExpression(List<IExpressionToken> expressionTokens)
    {
        try
        {
            var signNormalizedTokens = _signNormalizer.NormalizeSigns(expressionTokens);
            var unaryClassifiedTokens = _unaryClassifier.ClassifyUnaryOperators(signNormalizedTokens);
            var unaryMergedTokens = _unaryMerger.MergeUnaryOperators(unaryClassifiedTokens);
            expressionTokens.Clear();
            expressionTokens.AddRange(unaryMergedTokens);
            return new ParsingResult
            {
                Success = true
            };
        }
        catch(InvalidUnaryPlacementException ex)
        {
            return new ParsingResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorType = ErrorTypeCore.Error
            };
        }
        catch(UnknownOperatorException ex)
        {
            return new ParsingResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorType = ErrorTypeCore.Error
            };
        }
    }
}