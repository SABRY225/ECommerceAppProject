using System;
using System.Collections.Generic;
using System.Text;
using ECommerceApp.Application.DTOs.CategoryDtos;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Application.Services
{
    public class CategoryService(IGenericRebository<Category> genericRebository) : ICategoryService
    {
        private readonly IGenericRebository<Category> _genericRebository = genericRebository;

        public async Task<CategoryDashboardDto> GetDashboard()
        {
            var categories = _genericRebository.GetAll().Include(c=>c.Products).Where(c=>c.IsDeleted==false);
            var totalCategories = await categories.CountAsync();
            

            var activeProducts=await categories.SelectMany(c=>c.Products).CountAsync();

            var topCategory = await categories.OrderByDescending(c => c.Products.Count).FirstOrDefaultAsync();

            var lastUpdated = await categories.MaxAsync(c => c.UpdatedAt ?? c.CreatedAt);
            var categoryDashDto = new CategoryDashboardDto
            {
                TotalCategories = totalCategories,
                ActiveProducts = activeProducts,
                TopPerforming = topCategory?.CategoryName ?? "no category",
                LastUpdated = lastUpdated
            };

            return categoryDashDto;
        }

        public async Task<List<GetCategoryDto>> GetAll()
        {
            var categories = await _genericRebository.GetAll().ToListAsync();
            
            var categoriesDto = categories.Adapt<List<GetCategoryDto>>();
            return categoriesDto;
        }

        //public async Task<GetCategoryDto> GetById(int id)
        //{
        //    var category = await _genericRebository.GetAll().FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        //    if (category == null)
        //    {
        //        Console.WriteLine("Category not found");
        //        return null;

        //    }
        //    var categoryDto = category.Adapt<GetCategoryDto>();
        //    return categoryDto;
        //}
        public async Task<GetCategoryDto?> GetById(int id)
        {
            return await _genericRebository
                .GetAll()
                .Where(c => c.Id == id && !c.IsDeleted)
                .ProjectToType<GetCategoryDto>()
                .FirstOrDefaultAsync();
        }
        public async Task Add(AddCategoryDto newCategoryDto)
        {
            var newCategory = newCategoryDto.Adapt<Category>();
            await _genericRebository.Add(newCategory);
        }

        public async Task Delete(int id)
        {
            var category = _genericRebository.GetAll().FirstOrDefault(c => c.Id == id && !c.IsDeleted);

            await _genericRebository.Delete(category);
        }

        public async Task Update(UpdateCategoryDto categoryDto)
        {
            var category = await _genericRebository.GetAll().FirstOrDefaultAsync(c => c.Id == categoryDto.Id);

            if (category == null) return;

            category.CategoryName = categoryDto.CategoryName;
            category.Description = categoryDto.Description;
            category.ImagePath = categoryDto.ImagePath;
            category.UpdatedAt = DateTime.Now;
            category.ImagePath = categoryDto.ImagePath;
            await _genericRebository.Update(category);
        }
    }
}
