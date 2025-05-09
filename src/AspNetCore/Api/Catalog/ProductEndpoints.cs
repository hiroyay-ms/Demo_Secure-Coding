using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Api.Data;
using Api.Models;

public static class ProductEndpoints
{
    public static void RegisterProductEndpoints(this WebApplication app)
    {
        app.MapGet("/api/catalog/category", GetProductCategories)
            .WithName("GetProductCategories");

        app.MapGet("/api/catalog/category/{category}", GetProductCategoryByName)
            .WithName("GetProductCategoryByName");

        app.MapGet("/api/catalog/products/{id}", GetProductsByCategoryId)
            .WithName("GetProductsByCategoryId");
    }

    static async Task<IResult> GetProductCategories(AdventureWorksContext db)
    {
        var query = from a in db.ProductCategory
                    join b in db.ProductCategory on a.ProductCategoryID equals b.ParentProductCategoryID
                    join p in db.Product on b.ProductCategoryID equals p.ProductCategoryID
                    group new { a, b, p } by new { b.ProductCategoryID, Category = a.Name, SubCategory = b.Name } into g
                    orderby g.Key.ProductCategoryID
                    select new
                    {
                        ProductCategoryID = g.Key.ProductCategoryID,
                        Category = g.Key.Category,
                        SubCategory = g.Key.SubCategory,
                        ProductCount = g.Count()
                    };

        var productCategories = await query.ToListAsync();

        return productCategories.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(productCategories);
    }

    static async Task<IResult> GetProductCategoryByName(string category, AdventureWorksContext db)
    {
        if (string.IsNullOrEmpty(category))
            return TypedResults.BadRequest();

        var query = from a in db.ProductCategory
                    join b in db.ProductCategory on a.ProductCategoryID equals b.ParentProductCategoryID
                    join p in db.Product on b.ProductCategoryID equals p.ProductCategoryID
                    where a.Name == category
                    group new { a, b, p } by new { b.ProductCategoryID, Category = a.Name, SubCategory = b.Name } into g
                    orderby g.Key.ProductCategoryID
                    select new
                    {
                        ProductCategoryID = g.Key.ProductCategoryID,
                        Category = g.Key.Category,
                        SubCategory = g.Key.SubCategory,
                        ProductCount = g.Count()
                    };

        var productCategories = await query.ToListAsync();

        return productCategories.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(productCategories);
    }

    static async Task<IResult> GetProductsByCategoryId(int id, AdventureWorksContext db)
    {
        if (id < 5)
            return TypedResults.BadRequest();

        var conn = db.Database.GetDbConnection() as SqlConnection;
        if (conn == null)
            return TypedResults.Problem("Database connection is null.");

        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        List<Product> products = new List<Product>();

        using (var cmd = conn.CreateCommand())
        {
            var qyery = $"SELECT * FROM SalesLT.Product WHERE ProductCategoryID = {id}" ;

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = qyery;

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                var product = new Product
                {
                    ProductID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    ProductNumber = reader.GetString(2),
                    Color = reader.IsDBNull(3) ? null : reader.GetString(3),
                    StandardCost = reader.GetDecimal(4),
                    ListPrice = reader.GetDecimal(5),
                    Size = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Weight = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                    ProductCategoryID = reader.IsDBNull(8) ? null : (int?)reader.GetInt32(8),
                    ProductModelID = reader.IsDBNull(9) ? null : (int?)reader.GetInt32(9),
                    SellStartDate = reader.GetDateTime(10),
                    SellEndDate = reader.IsDBNull(11) ? null : (DateTime?)reader.GetDateTime(11),
                    DiscontinuedDate = reader.IsDBNull(12) ? null : (DateTime?)reader.GetDateTime(12),
                    ThumbnailPhotoFileName = reader.IsDBNull(14) ? null : reader.GetString(14),
                    rowguid = reader.GetGuid(15),
                    ModifiedDate = (DateTime)reader.GetDateTime(16)
                };
                products.Add(product);
            }
        }

        return TypedResults.Ok(products);
    }
}
