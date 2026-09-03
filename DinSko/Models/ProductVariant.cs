namespace DinSko.Models
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }
        public int ProductId { get; set; }
        public int Size { get; set; }
        public int Stock { get; set; }
    }
}
