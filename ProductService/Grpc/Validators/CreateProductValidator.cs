using Google.Protobuf;
using Microsoft.IdentityModel.Tokens;
using ProductService.Protos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductService.Grpc.Validators
{
    public class CreateProductValidator : IValidator<CreateProductRequest>
    {
        public ValidationResult Validate(CreateProductRequest request)
        {
            if (request.Product == null)
            {
                return ValidationResult.Invalid("Product cannot be null");
            }
            else if (request.Product.Name.IsNullOrEmpty())
            {
                return ValidationResult.Invalid("Product name cannot be null or empty");
            }
            else if (request.Product.Description.IsNullOrEmpty())
            {
                return ValidationResult.Invalid("Description cannot be null or empty");
            }
            else if (request.Product.Price <= 0)
            {
                return ValidationResult.Invalid("Price cannot be zero or negative");
            }
            else if (request.Product.ImageURLs == null || !request.Product.ImageURLs.Any())
            {
                return ValidationResult.Invalid("ImageURLs cannot be null or empty");
            }
            else if (request.Product.ImageURLs.Any(url => string.IsNullOrWhiteSpace(url)))
            {
                return ValidationResult.Invalid("All ImageURLs must be non-empty");
            }
            else if (request.Product.Attributes == null || !request.Product.Attributes.Any())
            {
                return ValidationResult.Invalid("Product should contain at least one attribute");
            }
            else if (request.Product.Attributes.Any(attr => attr.Name.IsNullOrEmpty() || attr.Value.IsNullOrEmpty()))
            {
                return ValidationResult.Invalid("All attribute names and values must be non-empty");
            }

            return ValidationResult.Valid();
        }
    }
}
