using System;
using System.Collections.Generic;
using System.Text;
using ECommerceApp.Application.DTOs.CategoryDtos;

namespace ECommerceApp.Application.Interfaces.Services
{
   public interface ICategoryService
    {
        public Task<CategoryDashboardDto> GetDashboard();

        public Task<List<GetCategoryDto>> GetAll();
        public Task<GetCategoryDto> GetById(int id);
        public Task Add(AddCategoryDto dto);
        public  Task Update(UpdateCategoryDto dto);
        public Task Delete(int id);


    }
}
