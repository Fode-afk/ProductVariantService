using Microsoft.IdentityModel.Tokens;
using ProductService.Protos;

namespace ProductService.Grpc.Validators
{
    public class GetProductValidator : IValidator<GetProductRequest>
    {
        public ValidationResult Validate(GetProductRequest request)
        {
            if (request.ProductId.IsNullOrEmpty())
            {
                return ValidationResult.Invalid("ProductId cannot be null or empty");
            }

            return ValidationResult.Valid();
        }
    }
}
