namespace ProductService.Grpc.Validators
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T request);
    }
}
