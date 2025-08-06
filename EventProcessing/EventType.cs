namespace ProductService.EventProcessing
{
    public enum EventType
    {
        ImageUrlPublished,
        ImageDeletePublished,
        ProductPublished,
        ProductUpdatePublished,
        ProductDeletePublished,
        CardDeletePublished,
        DeleteParentCardIdFromProducts,
        ProductUpdateParentCardIdPublished,
        UserDeletePublished,
        VendorDeletePublished,
        ProductsDeletePublished,
        Undetermined
    }
}
