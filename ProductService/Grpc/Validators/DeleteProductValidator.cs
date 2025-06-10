using Microsoft.IdentityModel.Tokens;
using ProductService.Protos;

namespace ProductService.Grpc.Validators
{
    public class DeleteProductValidator : IValidator<DeleteProductRequest>
    {
        public ValidationResult Validate(DeleteProductRequest request)
        {
            if (request.ProductId.IsNullOrEmpty())
            {
                return ValidationResult.Invalid("ProductId cannot be null or empty");
            }

            return ValidationResult.Valid();
        }
    }
}
