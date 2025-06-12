using AutoMapper;
using Grpc.Core;
using ProductService.Data;
using ProductService.Data.Images;
using ProductService.Grpc.Validators;
using ProductService.Models;
using ProductService.Protos;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ProductService.Grpc
{
    /// <summary>
    /// gRPC server responsible for handling product-related operations such as retrieval, creation, update, and deletion.
    /// Utilizes validation and AutoMapper for clean architecture and data transformation.
    /// </summary>
    public class GrpcServer(IProductRepo productRepo, IMapper mapper, IWebHostEnvironment environment, IImageProcessor imageProcessor) : GrpcProducts.GrpcProductsBase
    {
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IWebHostEnvironment _env = environment;
        private readonly IImageProcessor _imageProcessor = imageProcessor;

        private readonly IValidator<CreateProductRequest> _createProductValidator = new CreateProductValidator();
        private readonly IValidator<GetProductRequest> _getProductValidator = new GetProductValidator();
        private readonly IValidator<GetAllProductsByOwnerIdRequest> _getAllProductsValidator = new GetAllProductsValidator();
        private readonly IValidator<UpdateProductRequest> _updateProductValidator = new UpdateProductValidator();
        private readonly IValidator<DeleteProductRequest> _deleteProductValidator = new DeleteProductValidator();

        /// <summary>
        /// Retrieves all products that belong to a specific owner.
        /// </summary>
        /// <param name="request">The request containing the owner ID.</param>
        /// <param name="context">The server call context.</param>
        /// <returns>A response with the list of products or failure status.</returns>
        public override async Task<GetAllProductsByOwnerIdResponse> GetAllProductsByOwnerId(GetAllProductsByOwnerIdRequest request, ServerCallContext context)
        {
            var valres = _getAllProductsValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var products = await _productRepo.GetAllProductsByOwnerIdAsync(request.OwnerId);

            if (products == null)
            {
                return new GetAllProductsByOwnerIdResponse { Status = false };
            }

            return new GetAllProductsByOwnerIdResponse
            {
                Status = true,
                Products = { _mapper.Map<IEnumerable<ProductGrpc>>(products) }
            };
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="request">The request containing the product ID.</param>
        /// <param name="context">The server call context.</param>
        /// <returns>A response with the product details or failure status.</returns>
        public override async Task<GetProductResponse> GetProductById(GetProductRequest request, ServerCallContext context)
        {
            var valres = _getProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var product = await _productRepo.GetProductByIdAsync(request.ProductId);

            if (product == null)
            {
                return new GetProductResponse { Status = false };
            }

            return new GetProductResponse
            {
                Status = true,
                Product = _mapper.Map<ProductGrpc>(product)
            };
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="request">The request containing the product to be created.</param>
        /// <param name="context">The server call context.</param>
        /// <returns>A response indicating whether the product was successfully created.</returns>
        public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            var valres = _createProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var product = _mapper.Map<Product>(request.Product);

            await _productRepo.CreateProductAsync(product);

            return new CreateProductResponse { Status = true };
        }

        /// <summary>
        /// Updates an existing product by its ID.
        /// </summary>
        /// <param name="request">The request containing updated product data and the product ID.</param>
        /// <param name="context">The server call context.</param>
        /// <returns>A response indicating whether the update was successful.</returns>
        public override async Task<UpdateProductResponse> UpdateProductById(UpdateProductRequest request, ServerCallContext context)
        {
            var valres = _updateProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            var existingProduct = await _productRepo.GetProductByIdAsync(request.ProductId);

            if (existingProduct == null)
            {
                return new UpdateProductResponse { Status = false };
            }

            _mapper.Map(request, existingProduct);

            await _productRepo.UpdateProductAsync(existingProduct);

            return new UpdateProductResponse { Status = true };
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="request">The request containing the product ID to be deleted.</param>
        /// <param name="context">The server call context.</param>
        /// <returns>A response indicating whether the deletion was successful.</returns>
        public override async Task<DeleteProductResponse> DeleteProductById(DeleteProductRequest request, ServerCallContext context)
        {
            var valres = _deleteProductValidator.Validate(request);

            if (!valres.IsValid)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, valres.ErrorMessage));
            }

            await _productRepo.DeleteProductAsync(request.ProductId);

            return new DeleteProductResponse { Status = true };
        }

        public override async Task<UploadImageResponse> UploadImage(UploadImageRequest request, ServerCallContext context)
        {
            var product = await _productRepo.GetProductByIdAsync(request.ProductId);

            if (product == null)
                return new UploadImageResponse { Status = false };

           var result =  await _imageProcessor.ProcessImageAsync(request.Base64Data, request.ProductId, request.FileName);
          
           return new UploadImageResponse { Status = result };           
        }

        public override async Task<GetImageResponse> GetImage(GetImageRequest request, ServerCallContext context)
        {
            if(!File.Exists(request.ImageURL))
                return new GetImageResponse { Status = false };

            byte[] imageBytes;

            try
            {
                imageBytes = File.ReadAllBytes(request.ImageURL);
            }
            catch
            {
                return new GetImageResponse { Status = false };
            }

            var base64 = await _imageProcessor.ConvertImageAsync(imageBytes);

            if (base64 == null)
                return new GetImageResponse { Status = false };
            else
                return new GetImageResponse { Status = true, Base64Data = base64 };
        }

        public override async Task<DeleteImageResponse> DeleteImage(DeleteImageRequest request, ServerCallContext context)
        {
            if (!File.Exists(request.ImageURL))
                return new DeleteImageResponse { Status = false };

            var result = await _imageProcessor.DeleteImageAsync(request.ImageURL);

            return new DeleteImageResponse { Status = result };
        }
    }
}
