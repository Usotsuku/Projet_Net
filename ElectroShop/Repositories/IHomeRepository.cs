namespace ElectroShop
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Product>> GetProducts(string sterm = "", int categoryId = 0);
        Task<IEnumerable<Category>> Categories();
    }
}