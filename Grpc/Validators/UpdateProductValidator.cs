using Microsoft.IdentityModel.Tokens;
using ProductService.Protos;

namespace ProductService.Grpc.Validators
{
    public class UpdateProductValidator : IValidator<UpdateProductRequest>
    {
        public ValidationResult Validate(UpdateProductRequest request)
        {
            if (request.ProductId.IsNullOrEmpty())
            {
                return ValidationResult.Invalid("ProductId cannot be null or empty");
            }  

            return ValidationResult.Valid();
        }
    }
}