using Microsoft.IdentityModel.Tokens;
using ProductService.Protos;

namespace ProductService.Grpc.Validators
{
    public class GetAllProductsValidator : IValidator<GetAllProductsByOwnerIdRequest>
    {
        public ValidationResult Validate(GetAllProductsByOwnerIdRequest request)
        {
            if (request.OwnerId.IsNullOrEmpty())
            {
                return ValidationResult.Invalid("OwnerId cannot be null or empty");
            }

            return ValidationResult.Valid();
        }
    }
}
