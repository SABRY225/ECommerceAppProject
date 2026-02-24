using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.Application.DTOs.CategoryDtos;
using ECommerceApp.Domain.Entities;
using Mapster;

namespace ECommerceApp.Application.Mapper
{
    public static class MapsterConfig
    {
        public async static void Config()
        {
            //   TypeAdapterConfig<Category, GetCategoryDto>
            //.NewConfig()
            //.Map(des => des.Id, src => src.Id);

            TypeAdapterConfig<Category, GetCategoryDto>.NewConfig();

            TypeAdapterConfig<Category, AddCategoryDto>.NewConfig();

            TypeAdapterConfig<Category, UpdateCategoryDto>.NewConfig();


            TypeAdapterConfig<Category, CategoryDashboardDto>.NewConfig();




        }

    }
}
