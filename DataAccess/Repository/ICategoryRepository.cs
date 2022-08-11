using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace DataAccess.Repository
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetCategorys();
        Category GetCategoryByID(int CategoryID);
        void InsertCategory(Category Category);
        void DeleteCategory(int CategoryID);
        void UpdateCategory(Category Category);
    }
}
