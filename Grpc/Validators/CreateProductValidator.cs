using Microsoft.IdentityModel.Tokens;
using ProductService.Protos;

namespace ProductService.Grpc.Validators
{
    public class CreateProductValidator : IValidator<CreateProductRequest>
    {
        public ValidationResult Validate(CreateProductRequest request)
        {
            if (request.Name.IsNullOrEmpty())
            {
                return ValidationResult.Invalid("Product name cannot be null or empty");
            }
            else if (request.OwnerId.IsNullOrEmpty())
            {
                return ValidationResult.Invalid("Owner Id cannot be null or empty");
            }          

            return ValidationResult.Valid();
        }
    }
}
