using Microsoft.IdentityModel.Tokens;
using ProductService.Protos;

namespace ProductService.Grpc.Validators
{
    public class ReplaceProductValidator : IValidator<ReplaceProductRequest>
    {
        public ValidationResult Validate(ReplaceProductRequest request)
        {
            if (request.ProductId.IsNullOrEmpty())
            {
                return ValidationResult.Invalid("ProductId cannot be null or empty");
            }  
            else if (request.ImageURLs != null && request.ImageURLs.Any(url => string.IsNullOrWhiteSpace(url)))
            {
                return ValidationResult.Invalid("All ImageURLs must be non-empty");
            }
            else if (request.Attributes != null && request.Attributes.Any(attr => attr.Value.IsNullOrEmpty()))
            {
                return ValidationResult.Invalid("All attribute names and values must be non-empty");
            }

            return ValidationResult.Valid();
        }
    }
}