using AutoMapper;

namespace raBudget.Core.Interfaces.Mapping
{
    public interface IHaveCustomMapping
    {
        /// <summary>
        /// Definition of class specific mapping configuration loaded automatically by custom profile AutoMapperProfile constructor
        /// </summary>
        /// <param name="configuration"></param>
        void CreateMappings(Profile configuration);
    }
}
